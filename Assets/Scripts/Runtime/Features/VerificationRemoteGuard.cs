using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowState.Runtime.Features
{
    // Every SDK boundary rechecks permission, including follow-up requests.
    public sealed class VerificationRemoteGuard : IOnlineAuthenticationSession, IOnlineRecordTransport, IOnlineAccountTransport
    {
        private readonly Func<bool> _isAllowed;
        private readonly IOnlineAuthenticationSession _authentication;
        private readonly IOnlineRecordTransport _records;
        private readonly IOnlineAccountTransport _accounts;
        public const int OperationTimeoutMilliseconds = 5000;
        // A submit probe may include bounded backoff plus before/after reads.
        // Keep it finite while allowing the verification UI's advertised 60-second flow.
        public const int MaximumVerificationOperationTimeoutMilliseconds = 60000;
        private readonly Func<int, Task> _delay;
        public Task OperationDeadline { get; private set; }
        public bool OperationTimedOut => OperationDeadline != null && OperationDeadline.IsCompleted;
        public VerificationRemoteGuard(Func<bool> isAllowed, IOnlineAuthenticationSession authentication,
            IOnlineRecordTransport records, IOnlineAccountTransport accounts, Func<int, Task> delay = null)
        {
            _isAllowed = isAllowed; _authentication = authentication; _records = records; _accounts = accounts;
            _delay = delay == null ? milliseconds => Task.Delay(milliseconds) : delay;
        }
        public void BeginOperation(int timeoutMilliseconds = OperationTimeoutMilliseconds)
        {
            if (OperationDeadline != null) throw new InvalidOperationException("Verification operation already running.");
            if (timeoutMilliseconds <= 0 || timeoutMilliseconds > MaximumVerificationOperationTimeoutMilliseconds)
                throw new ArgumentOutOfRangeException(nameof(timeoutMilliseconds));
            OperationDeadline = _delay(timeoutMilliseconds);
        }
        public void EndOperation() { OperationDeadline = null; }

        public async Task<T> RunWithinOperationAsync<T>(Func<Task<T>> start)
        {
            if (!IsAllowed) throw new InvalidOperationException("Verification remote execution disabled.");
            Task deadline = OperationDeadline;
            if (deadline != null && deadline.IsCompleted) throw new TimeoutException();
            Task<T> work = start();
            if (deadline != null && await Task.WhenAny(work, deadline) != work)
            {
                ObserveDiscardedAsync(work);
                throw new TimeoutException();
            }
            T result = await work;
            if (deadline != null && deadline.IsCompleted) throw new TimeoutException();
            return result;
        }
        public Task WaitWithinOperationAsync(Func<Task> start)
        { return RunWithinOperationAsync(async () => { await start(); return true; }); }

        private static async void ObserveDiscardedAsync<T>(Task<T> work)
        { try { await work; } catch (Exception) { } }
        private bool IsAllowed => _isAllowed != null && _isAllowed();
        public async Task<OnlineAuthenticationResult> TryAuthenticateAsync()
        {
            if (!IsAllowed) return new OnlineAuthenticationResult(false, string.Empty);
            OnlineAuthenticationResult result = await RunWithinOperationAsync(() => _authentication.TryAuthenticateAsync());
            return IsAllowed ? result : new OnlineAuthenticationResult(false, string.Empty);
        }
        public Task<bool> TryClearSessionAsync(string expectedPlayerId)
        { return IsAllowed ? RunWithinOperationAsync(() => _authentication.TryClearSessionAsync(expectedPlayerId)) : Task.FromResult(false); }
        public Task<T> CallAsync<T>(string endpoint, string request)
        {
            if (!IsAllowed) throw new InvalidOperationException("Verification remote execution disabled.");
            return RunWithinOperationAsync(() => _records.CallAsync<T>(endpoint, request));
        }
        public Task<T> CallAsync<T>(string endpoint, IReadOnlyDictionary<string, string> parameters)
        {
            if (!IsAllowed) throw new InvalidOperationException("Verification remote execution disabled.");
            return RunWithinOperationAsync(() => _accounts.CallAsync<T>(endpoint, parameters));
        }
    }
}
