using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace FlowState.Runtime.Features
{
    public sealed class PersistentLocalSaveFileStore : ILocalSaveFileStore
    {
        private const string SaveFileName = "flow-state-save.json";
        private const string TemporaryFileName = "flow-state-save.tmp";

        private readonly string _savePath;
        private readonly string _temporaryPath;

        public bool HasSave => File.Exists(_savePath);

        public PersistentLocalSaveFileStore()
            : this(Application.persistentDataPath)
        {
        }

        public PersistentLocalSaveFileStore(string directoryPath)
        {
            _savePath = Path.Combine(directoryPath, SaveFileName);
            _temporaryPath = Path.Combine(
                directoryPath,
                TemporaryFileName);
        }

        public bool TryRead(out string contents)
        {
            contents = string.Empty;

            try
            {
                if (!HasSave)
                {
                    return false;
                }

                contents = File.ReadAllText(_savePath, Encoding.UTF8);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[PersistentLocalSaveFileStore] Save read failed: {exception.Message}");
                return false;
            }
        }

        public bool TryWriteAtomically(string contents)
        {
            if (string.IsNullOrEmpty(contents))
            {
                return false;
            }

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_savePath));
                File.WriteAllText(_temporaryPath, contents, Encoding.UTF8);

                if (HasSave)
                {
                    File.Replace(_temporaryPath, _savePath, null);
                }
                else
                {
                    File.Move(_temporaryPath, _savePath);
                }

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[PersistentLocalSaveFileStore] Save write failed: {exception.Message}");
                return false;
            }
        }
    }
}
