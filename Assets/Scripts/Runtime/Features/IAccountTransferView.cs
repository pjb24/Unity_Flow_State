namespace FlowState.Runtime.Features
{
    public interface IAccountTransferView
    {
        void Render(AccountTransferScreenState state);
        void ClearSensitiveInputs();
    }
}
