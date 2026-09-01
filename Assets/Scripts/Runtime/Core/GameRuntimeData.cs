namespace FlowState.Runtime.Core
{
    public class GameRuntimeData
    {
        private E_GameState _gameState;
        private E_GameMode _gameMode;
        private E_UIState _uiState;
        private PlayerMovementRuntimeData _playerMovementRuntimeData;
        private InfiniteModeRuntimeData _infiniteModeRuntimeData;
        private bool _isCreated;

        public E_GameState GameState => _gameState;

        public E_GameMode GameMode => _gameMode;

        public E_UIState UIState => _uiState;

        public PlayerMovementRuntimeData PlayerMovementRuntimeData =>
            _playerMovementRuntimeData;

        public InfiniteModeRuntimeData InfiniteModeRuntimeData =>
            _infiniteModeRuntimeData;

        public bool IsCreated => _isCreated;

        public void Initialize()
        {
            Initialize(E_GameMode.Stage);
        }

        public void Initialize(E_GameMode gameMode)
        {
            _gameState = E_GameState.None;
            _gameMode = gameMode;
            _uiState = E_UIState.None;
            _playerMovementRuntimeData = new PlayerMovementRuntimeData();
            _playerMovementRuntimeData.Initialize();

            if (_gameMode == E_GameMode.Infinite)
            {
                _infiniteModeRuntimeData = new InfiniteModeRuntimeData();
                _infiniteModeRuntimeData.Initialize();
            }
            else
            {
                _infiniteModeRuntimeData = null;
            }

            _isCreated = true;
        }

        public void SetGameState(E_GameState gameState)
        {
            if (!_isCreated)
            {
                return;
            }

            _gameState = gameState;
        }

        public void SetUIState(E_UIState uiState)
        {
            if (!_isCreated)
            {
                return;
            }

            _uiState = uiState;
        }

        public void Clear()
        {
            _gameState = E_GameState.None;
            _gameMode = E_GameMode.Stage;
            _uiState = E_UIState.None;
            _playerMovementRuntimeData = null;

            if (_infiniteModeRuntimeData != null)
            {
                _infiniteModeRuntimeData.Clear();
                _infiniteModeRuntimeData = null;
            }

            _isCreated = false;
        }
    }
}
