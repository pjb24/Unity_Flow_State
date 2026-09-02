namespace FlowState.Runtime.Core
{
    public class GameState
    {
        private E_GameState _currentState;

        public E_GameState CurrentState => _currentState;

        public bool TryPause()
        {
            return TryTransitionTo(E_GameState.Paused);
        }

        public bool TryResume()
        {
            if (_currentState != E_GameState.Paused)
            {
                return false;
            }

            return TryTransitionTo(E_GameState.Playing);
        }

        public bool TryTransitionTo(E_GameState nextState)
        {
            if (!IsValidTransition(nextState))
            {
                return false;
            }

            _currentState = nextState;
            return true;
        }

        public void Reset()
        {
            _currentState = E_GameState.None;
        }

        private bool IsValidTransition(E_GameState nextState)
        {
            switch (_currentState)
            {
                case E_GameState.None:
                    return nextState == E_GameState.Initializing;

                case E_GameState.Initializing:
                    return nextState == E_GameState.Ready;

                case E_GameState.Ready:
                    return nextState == E_GameState.Playing;

                case E_GameState.Playing:
                    return nextState == E_GameState.Paused ||
                           nextState == E_GameState.Ending;

                case E_GameState.Paused:
                    return nextState == E_GameState.Playing ||
                           nextState == E_GameState.Ending;

                case E_GameState.Ending:
                    return nextState == E_GameState.Ended;

                case E_GameState.Ended:
                    return nextState == E_GameState.Initializing;

                default:
                    return false;
            }
        }
    }
}
