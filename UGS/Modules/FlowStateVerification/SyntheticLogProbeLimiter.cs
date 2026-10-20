using System;
using System.Threading;

namespace FlowState.Server
{
    // This is only a verification-cost guard. It has no player or account key
    // and does not claim to be a distributed production rate limiter.
    internal static class SyntheticLogProbeLimiter
    {
        private const long MinimumIntervalMilliseconds = 60000;
        private static long _lastAcceptedAtMilliseconds;

        internal static bool TryReserve()
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            while (true)
            {
                long previous = Interlocked.Read(ref _lastAcceptedAtMilliseconds);
                if (previous != 0 && now - previous < MinimumIntervalMilliseconds) return false;
                if (Interlocked.CompareExchange(ref _lastAcceptedAtMilliseconds, now, previous) == previous)
                    return true;
            }
        }
    }
}
