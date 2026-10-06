namespace FlowState.Runtime.Features
{
    public sealed class OnlineAccountState
    {
        public bool HasConfirmedRecoveryNotice { get; }
        public string PlayerId { get; }
        public string PublicNumberCache { get; }

        public OnlineAccountState(bool hasConfirmedRecoveryNotice = false,
            string playerId = "", string publicNumberCache = "")
        {
            HasConfirmedRecoveryNotice = hasConfirmedRecoveryNotice;
            PlayerId = hasConfirmedRecoveryNotice && playerId != null ? playerId : string.Empty;
            PublicNumberCache = !string.IsNullOrEmpty(PlayerId) && PublicPlayerNumber.IsValid(publicNumberCache)
                ? publicNumberCache : string.Empty;
        }
    }
}
