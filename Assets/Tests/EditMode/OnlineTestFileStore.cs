using System.Threading.Tasks;
using FlowState.Runtime.Features;

namespace FlowState.Tests.EditMode
{
    internal sealed class OnlineTestFileStore : ILocalSaveFileStore
    {
        public string Contents;
        public bool FailWrite;
        public bool HasSave => Contents != null;
        public bool TryRead(out string contents) { contents = Contents; return HasSave; }
        public bool TryWriteAtomically(string contents)
        {
            if (FailWrite) return false;
            Contents = contents;
            return true;
        }
    }
}
