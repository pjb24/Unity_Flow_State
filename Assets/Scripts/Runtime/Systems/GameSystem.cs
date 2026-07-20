using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using UnityEngine;

namespace FlowState.Runtime.Systems
{
    public class GameSystem : MonoBehaviour
    {
        [SerializeField] private RuntimeDataSystem _runtimeDataSystem;
        [SerializeField] private UIManagementSystem _uiManagementSystem;
        [SerializeField] private PlayerInputSystem _playerInputSystem;
        [SerializeField] private PlayerMovementSystem _playerMovementSystem;
        [SerializeField] private PlayerControllerSystem _playerControllerSystem;
        [SerializeField] private CollisionSystem _collisionSystem;
        [SerializeField] private StageSystem _stageSystem;
        [SerializeField] private CameraSystem _cameraSystem;
        [SerializeField] private CameraFollow _cameraFollow;

        private GameRuntimeData _runtimeData;
        private E_GameState _currentGameState;

        public E_GameState CurrentGameState => _currentGameState;

        private void Start()
        {
            StartGame();
        }

        private void OnDisable()
        {
            if (_stageSystem != null)
            {
                _stageSystem.RemoveStageEndedListener(HandleStageEnded);
            }
        }

        [ContextMenu("Start Game")]
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
            _runtimeData.PlayerMovementRuntimeData.Initialize();

            _uiManagementSystem.Initialize();
            SetUIState(E_UIState.None);

            if (!_playerControllerSystem.Initialize() ||
                !_collisionSystem.Initialize() ||
                !_stageSystem.Initialize() ||
                !_playerMovementSystem.Initialize() ||
                !_cameraSystem.Initialize())
            {
                AbortGameStart();
                return;
            }

            _playerInputSystem.Initialize();

            SetGameState(E_GameState.Ready);
            SetUIState(E_UIState.StageHud);

            _stageSystem.AddStageEndedListener(HandleStageEnded);

            if (!_stageSystem.StartStage())
            {
                AbortGameStart();
                return;
            }

            _playerInputSystem.EnablePlayerActionMap();
            _cameraFollow.StartFollowing();
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

            _stageSystem.StopStage();
            _playerInputSystem.DisablePlayerActionMap();
            _cameraFollow.StopFollowing();
            _playerMovementSystem.StopMovement();

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

            if (_playerInputSystem == null)
            {
                Debug.LogError("[GameSystem] PlayerInputSystem is not assigned.");
                hasRequiredSystems = false;
            }

            if (_playerMovementSystem == null)
            {
                Debug.LogError("[GameSystem] PlayerMovementSystem is not assigned.");
                hasRequiredSystems = false;
            }

            if (_playerControllerSystem == null)
            {
                Debug.LogError("[GameSystem] PlayerControllerSystem is not assigned.");
                hasRequiredSystems = false;
            }

            if (_collisionSystem == null)
            {
                Debug.LogError("[GameSystem] CollisionSystem is not assigned.");
                hasRequiredSystems = false;
            }

            if (_stageSystem == null)
            {
                Debug.LogError("[GameSystem] StageSystem is not assigned.");
                hasRequiredSystems = false;
            }

            if (_cameraSystem == null)
            {
                Debug.LogError("[GameSystem] CameraSystem is not assigned.");
                hasRequiredSystems = false;
            }

            if (_cameraFollow == null)
            {
                Debug.LogError("[GameSystem] CameraFollow is not assigned.");
                hasRequiredSystems = false;
            }

            return hasRequiredSystems;
        }

        private void AbortGameStart()
        {
            _stageSystem.StopStage();
            _playerInputSystem.DisablePlayerActionMap();
            _cameraFollow.StopFollowing();
            _playerMovementSystem.StopMovement();
            _runtimeDataSystem.ClearRuntimeData();
            _runtimeData = null;
            _currentGameState = E_GameState.None;

            Debug.LogError("[GameSystem] Game start was aborted because initialization failed.");
        }

        private void HandleStageEnded()
        {
            if (_currentGameState != E_GameState.Playing)
            {
                return;
            }

            EndGame();
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
