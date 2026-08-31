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
            return CreateRuntimeData(E_GameMode.Stage);
        }

        public GameRuntimeData CreateRuntimeData(E_GameMode gameMode)
        {
            if (HasRuntimeData)
            {
                Debug.Log("[RuntimeDataSystem] Runtime Data already exists.");
                return _runtimeData;
            }

            _runtimeData = new GameRuntimeData();
            _runtimeData.Initialize(gameMode);

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
