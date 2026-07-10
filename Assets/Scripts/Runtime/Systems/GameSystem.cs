using FlowState.Runtime.Core;
using UnityEngine;

namespace FlowState.Runtime.Systems
{
    public class GameSystem : MonoBehaviour
    {
        [SerializeField] private RuntimeDataSystem _runtimeDataSystem;
        [SerializeField] private UIManagementSystem _uiManagementSystem;

        private GameRuntimeData _runtimeData;
        private E_GameState _currentGameState;

        public E_GameState CurrentGameState => _currentGameState;

        private void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            if (!HasRequiredSystems())
            {
                return;
            }

            if (_currentGameState == E_GameState.Playing)
            {
                Debug.LogWarning("[GameSystem] Game is already running.");
                return;
            }

            SetGameState(E_GameState.Initializing);

            _runtimeData = _runtimeDataSystem.CreateRuntimeData();
            _runtimeData.SetGameState(_currentGameState);

            _uiManagementSystem.Initialize();
            SetUIState(E_UIState.None);

            SetGameState(E_GameState.Ready);
            SetUIState(E_UIState.StageHud);
            SetGameState(E_GameState.Playing);

            Debug.Log("[GameSystem] Game started.");
        }

        [ContextMenu("End Game")]
        public void EndGame()
        {
            if (!HasRequiredSystems())
            {
                return;
            }

            if (_currentGameState == E_GameState.Ending ||
                _currentGameState == E_GameState.Ended)
            {
                Debug.LogWarning("[GameSystem] Game is already ending or ended.");
                return;
            }

            if (_runtimeData == null || !_runtimeDataSystem.HasRuntimeData)
            {
                Debug.LogWarning("[GameSystem] Runtime Data does not exist.");
                return;
            }

            SetGameState(E_GameState.Ending);
            SetUIState(E_UIState.Result);

            _runtimeDataSystem.ClearRuntimeData();
            _runtimeData = null;

            SetGameState(E_GameState.Ended);

            Debug.Log("[GameSystem] Game ended.");
        }

        private bool HasRequiredSystems()
        {
            bool hasRequiredSystems = true;

            if (_runtimeDataSystem == null)
            {
                Debug.LogError("[GameSystem] RuntimeDataSystem is not assigned.");
                hasRequiredSystems = false;
            }

            if (_uiManagementSystem == null)
            {
                Debug.LogError("[GameSystem] UIManagementSystem is not assigned.");
                hasRequiredSystems = false;
            }

            return hasRequiredSystems;
        }

        private void SetGameState(E_GameState gameState)
        {
            _currentGameState = gameState;
            _runtimeData?.SetGameState(gameState);

            Debug.Log($"[GameSystem] Game State changed to {_currentGameState}.");
        }

        private void SetUIState(E_UIState uiState)
        {
            _runtimeData?.SetUIState(uiState);
            _uiManagementSystem.SetUIState(uiState);
        }
    }
}
