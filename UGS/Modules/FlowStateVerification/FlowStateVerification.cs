using System;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;

namespace FlowState.Server
{
    public sealed class FlowStateVerification
    {
        [CloudCodeFunction("GetPublicPlayerNumber")]
        public object GetPublicPlayerNumber(IExecutionContext context, IGameApiClient client)
        { return Run("get-public-player-number", context, client, new { }); }

        [CloudCodeFunction("GetAccountTransferStatus")]
        public object GetAccountTransferStatus(IExecutionContext context, IGameApiClient client)
        { return Run("get-account-transfer-status", context, client, new { }); }

        [CloudCodeFunction("GetAccountPersonalBests")]
        public object GetAccountPersonalBests(IExecutionContext context, IGameApiClient client)
        { return Run("get-account-personal-bests", context, client, new { }); }

        [CloudCodeFunction("StartAccountTransfer")]
        public object StartAccountTransfer(IExecutionContext context, IGameApiClient client)
        { return Run("start-account-transfer", context, client, new { }); }

        [CloudCodeFunction("ReissueAccountTransfer")]
        public object ReissueAccountTransfer(IExecutionContext context, IGameApiClient client)
        { return Run("reissue-account-transfer", context, client, new { }); }

        [CloudCodeFunction("CancelAccountTransfer")]
        public object CancelAccountTransfer(IExecutionContext context, IGameApiClient client, string transferId)
        { return Run("cancel-account-transfer", context, client, new { transferId }); }

        [CloudCodeFunction("CompleteAccountTransfer")]
        public object CompleteAccountTransfer(IExecutionContext context, IGameApiClient client, string code, string verificationValue)
        { return Run("complete-account-transfer", context, client, new { code, verificationValue }); }

        [CloudCodeFunction("SubmitRecord")]
        public object SubmitRecord(IExecutionContext context, IGameApiClient client, string request)
        { return Run("submit-record", context, client, new { request }); }

        [CloudCodeFunction("QueryRecords")]
        public object QueryRecords(IExecutionContext context, IGameApiClient client, string request)
        { return Run("query-records", context, client, new { request }); }

        [CloudCodeFunction("EmitVerificationSafeLogProbe")]
        public object EmitVerificationSafeLogProbe(IExecutionContext context, IGameApiClient client)
        {
            if (context == null || string.IsNullOrWhiteSpace(context.ProjectId) || string.IsNullOrWhiteSpace(context.EnvironmentId))
                throw new InvalidOperationException("ContextMismatch");
            if (!SyntheticLogProbeLimiter.TryReserve()) return new { status = "RateLimited" };
            using ServerServices services = new ServerServices(context);
            bool accepted = new SafeLogSinkClient(services,
                (name, remainingMilliseconds) => client.SecretManager.GetSecret(context, name)
                    .WaitAsync(TimeSpan.FromMilliseconds(remainingMilliseconds)).GetAwaiter().GetResult().Value,
                SafeLogSinkClient.SyntheticProbeTelemetryMilliseconds)
                .TryEmit("Accepted", "SyntheticProbe", "Allowed",
                    out string probePhase, out string probeReason, out int probeHttpStatus);
            return new { status = accepted ? "Accepted" : "Unavailable",
                phase = probePhase, reason = probeReason, httpStatus = probeHttpStatus };
        }

        private static object Run(string endpoint, IExecutionContext context, IGameApiClient client, object parameters)
        {
            // Only fixed names select embedded code; players cannot supply executable source.
            return new ServerScriptRuntime().Run(endpoint, context, parameters,
                (name, remainingMilliseconds) => client.SecretManager.GetSecret(context, name)
                    .WaitAsync(TimeSpan.FromMilliseconds(remainingMilliseconds)).GetAwaiter().GetResult().Value);
        }
    }
}
