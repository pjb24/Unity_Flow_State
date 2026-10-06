namespace FlowState.Runtime.Features
{
    public enum E_AccountTransferPage { Overview, Issued, Input, Result, DiscardConfirmation }
    public enum E_AccountTransferAction
    {
        Start, OpenInput, Complete, Reissue, CancelTransfer, Refresh,
        ConfirmConsent, RetryPending, RequestDiscard, ConfirmDiscard, CancelDiscard, Back
    }

    // Runtime-only presentation. Never serialized or restored from Local Save.
    public sealed class AccountTransferScreenState
    {
        public bool IsOpen { get; internal set; }
        public bool IsBusy { get; internal set; }
        public E_AccountTransferPage Page { get; internal set; }
        public string NumberText { get; internal set; } = string.Empty;
        public string Message { get; internal set; } = string.Empty;
        public string Code { get; internal set; } = string.Empty;
        public string VerificationValue { get; internal set; } = string.Empty;
        public long ExpiresAtMilliseconds { get; internal set; }
        public int PendingCount { get; internal set; }
        public bool HasConsent { get; internal set; }
        public bool IsTransferPending { get; internal set; }
    }
}
