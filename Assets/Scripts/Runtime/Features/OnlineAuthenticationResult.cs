namespace FlowState.Runtime.Features
{
    public struct OnlineAuthenticationResult
    {
        public bool IsAuthenticated { get; }

        public string PlayerId { get; }

        public OnlineAuthenticationResult(bool isAuthenticated, string playerId)
        {
            IsAuthenticated = isAuthenticated;
            PlayerId = playerId == null ? string.Empty : playerId;
        }
    }
}
