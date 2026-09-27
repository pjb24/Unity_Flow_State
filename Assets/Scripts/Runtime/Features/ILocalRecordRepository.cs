namespace FlowState.Runtime.Features
{
    public interface ILocalRecordRepository : IRecordRepository
    {
        bool TryLoad(out LocalSaveData saveData);

        bool TrySave(LocalSaveData saveData);
    }
}
