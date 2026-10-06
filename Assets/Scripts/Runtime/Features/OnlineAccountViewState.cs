namespace FlowState.Runtime.Features
{
    public enum E_OnlineAccountDisplayState { NotRequested, Loading, Ready, Offline, Error, TransferPending, Inactive, Completed }

    public sealed class OnlineAccountViewState
    {
        public E_OnlineAccountDisplayState State { get; private set; }
        public string PublicNumber { get; private set; } = string.Empty;
        public string Reason { get; private set; } = string.Empty;
        public string TransferId { get; private set; } = string.Empty;
        public bool CanRetry => State == E_OnlineAccountDisplayState.Error || State == E_OnlineAccountDisplayState.Offline;
        public bool IsOfflinePlayAllowed => true;
        public string RecoveryNotice => "A public number cannot recover your account. Use a transfer code and verification value.";
        public string NumberText => State == E_OnlineAccountDisplayState.Ready ? PublicNumber : string.Empty;

        internal void Set(E_OnlineAccountDisplayState state, string reason = "", string number = "", string transferId = "")
        {
            State = state;
            Reason = reason;
            PublicNumber = state == E_OnlineAccountDisplayState.Ready && PublicPlayerNumber.IsValid(number) ? number : string.Empty;
            TransferId = transferId;
        }
    }
}
