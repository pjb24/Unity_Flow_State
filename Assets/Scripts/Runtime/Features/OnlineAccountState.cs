namespace FlowState.Runtime.Features
{
    public sealed class OnlineAccountState
    {
        public bool HasConfirmedRecoveryNotice { get; }
        public string PlayerId { get; }

        public OnlineAccountState(bool hasConfirmedRecoveryNotice = false,
            string playerId = "")
        {
            HasConfirmedRecoveryNotice = hasConfirmedRecoveryNotice;
            PlayerId = hasConfirmedRecoveryNotice && playerId != null ? playerId : string.Empty;
        }
    }
}
