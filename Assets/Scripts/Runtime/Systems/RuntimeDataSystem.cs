using FlowState.Runtime.Core;
using UnityEngine;

namespace FlowState.Runtime.Systems
{
    public class RuntimeDataSystem : MonoBehaviour
    {
        private GameRuntimeData _runtimeData;

        public bool HasRuntimeData => _runtimeData != null && _runtimeData.IsCreated;

        public GameRuntimeData RuntimeData => _runtimeData;

        public GameRuntimeData CreateRuntimeData()
        {
            if (HasRuntimeData)
            {
                Debug.Log("[RuntimeDataSystem] Runtime Data already exists.");
                return _runtimeData;
            }

            _runtimeData = new GameRuntimeData();
            _runtimeData.Initialize();

            Debug.Log("[RuntimeDataSystem] Runtime Data created.");

            return _runtimeData;
        }

        public GameRuntimeData GetRuntimeData()
        {
            if (!HasRuntimeData)
            {
                Debug.LogWarning("[RuntimeDataSystem] Runtime Data does not exist.");
            }

            return _runtimeData;
        }

        public void ClearRuntimeData()
        {
            if (_runtimeData == null)
            {
                Debug.LogWarning("[RuntimeDataSystem] Runtime Data is already empty.");
                return;
            }

            _runtimeData.Clear();
            _runtimeData = null;

            Debug.Log("[RuntimeDataSystem] Runtime Data cleared.");
        }
    }
}
