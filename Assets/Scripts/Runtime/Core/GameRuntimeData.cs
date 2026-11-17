namespace FlowState.Runtime.Core
{
    public class GameRuntimeData
    {
        private E_GameState _gameState;
        private E_GameMode _gameMode = E_GameMode.Infinite;
        private E_UIState _uiState;
        private PlayerMovementRuntimeData _playerMovementRuntimeData;
        private InfiniteModeRuntimeData _infiniteModeRuntimeData;
        private CollectibleRuntimeData _collectibleRuntimeData;
        private bool _isCreated;

        public E_GameState GameState => _gameState;

        public E_GameMode GameMode => _gameMode;

        public E_UIState UIState => _uiState;

        public PlayerMovementRuntimeData PlayerMovementRuntimeData =>
            _playerMovementRuntimeData;

        public InfiniteModeRuntimeData InfiniteModeRuntimeData =>
            _infiniteModeRuntimeData;

        public bool IsCreated => _isCreated;

        public CollectibleRuntimeData CollectibleRuntimeData =>
            _collectibleRuntimeData;

        public void Initialize()
        {
            Initialize(E_GameMode.Infinite);
        }

        public void Initialize(E_GameMode gameMode)
        {
            // Stage production Runs are retired. Keep this void API, rejecting
            // explicit unsupported modes before clearing any existing Run data.
            if (gameMode != E_GameMode.Infinite)
            {
                return;
            }

            if (_collectibleRuntimeData != null)
            {
                _collectibleRuntimeData.Clear();
            }

            _collectibleRuntimeData = new CollectibleRuntimeData();
            _collectibleRuntimeData.Initialize();
            _gameState = E_GameState.None;
            _gameMode = gameMode;
            _uiState = E_UIState.None;
            _playerMovementRuntimeData = new PlayerMovementRuntimeData();
            _playerMovementRuntimeData.Initialize();

            if (_gameMode == E_GameMode.Infinite)
            {
                _infiniteModeRuntimeData = new InfiniteModeRuntimeData();
                _infiniteModeRuntimeData.Initialize(
                    global::FlowState.Runtime.Core.ScoringVersion.Current);
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
            _gameMode = E_GameMode.Infinite;
            _uiState = E_UIState.None;
            _playerMovementRuntimeData = null;

            if (_collectibleRuntimeData != null)
            {
                _collectibleRuntimeData.Clear();
                _collectibleRuntimeData = null;
            }

            if (_infiniteModeRuntimeData != null)
            {
                _infiniteModeRuntimeData.Clear();
                _infiniteModeRuntimeData = null;
            }

            _isCreated = false;
        }
    }
}
