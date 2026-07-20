namespace FlowState.Runtime.Core
{
    public class GameRuntimeData
    {
        private E_GameState _gameState;
        private E_UIState _uiState;
        private PlayerMovementRuntimeData _playerMovementRuntimeData;
        private bool _isCreated;

        public E_GameState GameState => _gameState;

        public E_UIState UIState => _uiState;

        public PlayerMovementRuntimeData PlayerMovementRuntimeData =>
            _playerMovementRuntimeData;

        public bool IsCreated => _isCreated;

        public void Initialize()
        {
            _gameState = E_GameState.None;
            _uiState = E_UIState.None;
            _playerMovementRuntimeData = new PlayerMovementRuntimeData();
            _playerMovementRuntimeData.Initialize();
            _isCreated = true;
        }

        public void SetGameState(E_GameState gameState)
        {
            _gameState = gameState;
        }

        public void SetUIState(E_UIState uiState)
        {
            _uiState = uiState;
        }

        public void Clear()
        {
            _gameState = E_GameState.None;
            _uiState = E_UIState.None;
            _playerMovementRuntimeData = null;
            _isCreated = false;
        }
    }
}
