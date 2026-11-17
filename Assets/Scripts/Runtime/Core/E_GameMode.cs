namespace FlowState.Runtime.Core
{
    public enum E_GameMode
    {
        // Deprecated production mode. Preserve value 0 for serialized identities;
        // new Runs must use Infinite (value 1), and explicit Stage starts are rejected.
        Stage,
        Infinite
    }
}
