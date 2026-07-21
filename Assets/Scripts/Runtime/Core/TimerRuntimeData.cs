namespace FlowState.Runtime.Core
{
    public class TimerRuntimeData
    {
        private double _startedAt;
        private double _pausedAt;
        private double _totalPausedDuration;
        private double _finalTime;
        private E_TimerState _state;

        public E_TimerState State => _state;

        public void Initialize()
        {
            _startedAt = 0.0;
            _pausedAt = 0.0;
            _totalPausedDuration = 0.0;
            _finalTime = 0.0;
            _state = E_TimerState.Created;
        }

        public bool Start(double currentTime)
        {
            if (_state != E_TimerState.Created)
            {
                return false;
            }

            _startedAt = currentTime;
            _state = E_TimerState.Running;
            return true;
        }

        public bool Pause(double currentTime)
        {
            if (_state != E_TimerState.Running)
            {
                return false;
            }

            _pausedAt = currentTime;
            _state = E_TimerState.Paused;
            return true;
        }

        public bool Resume(double currentTime)
        {
            if (_state != E_TimerState.Paused)
            {
                return false;
            }

            _totalPausedDuration += currentTime - _pausedAt;
            _pausedAt = 0.0;
            _state = E_TimerState.Running;
            return true;
        }

        public bool Stop(double currentTime)
        {
            if (_state != E_TimerState.Running &&
                _state != E_TimerState.Paused)
            {
                return false;
            }

            _finalTime = GetElapsedTime(currentTime);
            _state = E_TimerState.Stopped;
            return true;
        }

        public double GetElapsedTime(double currentTime)
        {
            if (_state == E_TimerState.Created)
            {
                return 0.0;
            }

            if (_state == E_TimerState.Stopped)
            {
                return _finalTime;
            }

            double measurementTime =
                _state == E_TimerState.Paused ? _pausedAt : currentTime;
            return measurementTime - _startedAt - _totalPausedDuration;
        }
    }
}
