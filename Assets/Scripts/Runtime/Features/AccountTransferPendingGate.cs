using System;
using System.Threading.Tasks;

namespace FlowState.Runtime.Features
{
    public sealed class AccountTransferPendingGate
    {
        private readonly LocalRecordRepository _local;

        public AccountTransferPendingGate(LocalRecordRepository local)
        {
            _local = local;
        }

        // Both A start and B completion must enter through this gate. The
        // callback is the future transport boundary, not proof of server authority.
        public async Task<bool> TryRequestAsync(Func<Task<bool>> request)
        {
            if (_local == null || request == null || !_local.TryBeginTransferRequest()) return false;
            try { return await request(); }
            finally { _local.EndTransferRequest(); }
        }
    }
}
