using System;

namespace FlowState.Runtime.Features
{
    // Transport DTO only. Credential fields never enter Local Save/view state.
    [Serializable]
    public sealed class OnlineAccountResponse
    {
        public string status;
        public string reason;
        public string publicPlayerNumber;
        public string transferStatus;
        public string transferId;
        public string code;
        public string verificationValue;
        public long expiresAtMilliseconds;
        public bool credentialReissueRequired;
        public int serverElapsedMilliseconds = -1;
        public int serverServiceCalls = -1;
        public bool serverTimingPresent;
    }
}
