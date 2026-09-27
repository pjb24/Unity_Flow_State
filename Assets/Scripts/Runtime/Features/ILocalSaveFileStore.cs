namespace FlowState.Runtime.Features
{
    public interface ILocalSaveFileStore
    {
        bool HasSave { get; }

        bool TryRead(out string contents);

        bool TryWriteAtomically(string contents);
    }
}
