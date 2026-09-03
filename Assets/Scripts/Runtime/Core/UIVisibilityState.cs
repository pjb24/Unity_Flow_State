namespace FlowState.Runtime.Core
{
    public class UIVisibilityState
    {
        public bool IsStageHudVisible { get; private set; }

        public bool IsInfiniteHudVisible { get; private set; }

        public bool IsPausePanelVisible { get; private set; }

        public bool IsResultPanelVisible { get; private set; }

        public bool IsStageResultContentVisible { get; private set; }

        public bool IsInfiniteResultContentVisible { get; private set; }

        public bool Apply(
            E_GameMode gameMode,
            E_GameState gameState,
            E_UIState uiState)
        {
            Reset();

            if (!IsValidGameMode(gameMode) ||
                !IsValidGameState(gameState) ||
                !IsValidUIState(uiState))
            {
                return false;
            }

            if (gameState == E_GameState.None ||
                gameState == E_GameState.Initializing ||
                gameState == E_GameState.Ready)
            {
                return true;
            }

            ApplyCurrentModeHud(gameMode);

            if (gameState == E_GameState.Paused &&
                uiState == E_UIState.Pause)
            {
                IsPausePanelVisible = true;
            }

            if (gameState == E_GameState.Ended &&
                uiState == E_UIState.Result)
            {
                IsResultPanelVisible = true;
                IsStageResultContentVisible = gameMode == E_GameMode.Stage;
                IsInfiniteResultContentVisible =
                    gameMode == E_GameMode.Infinite;
            }

            return true;
        }

        public void Reset()
        {
            IsStageHudVisible = false;
            IsInfiniteHudVisible = false;
            IsPausePanelVisible = false;
            IsResultPanelVisible = false;
            IsStageResultContentVisible = false;
            IsInfiniteResultContentVisible = false;
        }

        private void ApplyCurrentModeHud(E_GameMode gameMode)
        {
            IsStageHudVisible = gameMode == E_GameMode.Stage;
            IsInfiniteHudVisible = gameMode == E_GameMode.Infinite;
        }

        private bool IsValidGameMode(E_GameMode gameMode)
        {
            return gameMode == E_GameMode.Stage ||
                   gameMode == E_GameMode.Infinite;
        }

        private bool IsValidGameState(E_GameState gameState)
        {
            return gameState == E_GameState.None ||
                   gameState == E_GameState.Initializing ||
                   gameState == E_GameState.Ready ||
                   gameState == E_GameState.Playing ||
                   gameState == E_GameState.Paused ||
                   gameState == E_GameState.Ending ||
                   gameState == E_GameState.Ended;
        }

        private bool IsValidUIState(E_UIState uiState)
        {
            return uiState == E_UIState.None ||
                   uiState == E_UIState.StageHud ||
                   uiState == E_UIState.Pause ||
                   uiState == E_UIState.Result;
        }
    }
}
