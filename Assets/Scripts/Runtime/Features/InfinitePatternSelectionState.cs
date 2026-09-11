using System.Collections.Generic;

namespace FlowState.Runtime.Features
{
    public class InfinitePatternSelectionState
    {
        public const int MaximumConsecutiveSelections = 2;

        private readonly List<InfinitePatternDefinition> _candidates =
            new List<InfinitePatternDefinition>();

        private InfinitePatternCatalog _catalog;
        private string _currentPatternId;
        private int _consecutiveSelectionCount;
        private int _lastRequestId;
        private uint _randomState;
        private bool _isInitialized;
        private bool _isRunning;
        private bool _isPaused;
        private bool _hasEnded;
        private bool _hasRequest;

        public string CurrentPatternId => _currentPatternId;

        public int ConsecutiveSelectionCount => _consecutiveSelectionCount;

        public bool IsInitialized => _isInitialized;

        public bool IsRunning => _isRunning;

        public bool IsPaused => _isPaused;

        public bool HasEnded => _hasEnded;

        public bool Initialize(InfinitePatternCatalog catalog)
        {
            ResetAllState();

            if (catalog == null ||
                !catalog.TryGet(
                    InfinitePatternCatalogFactory.FlatId,
                    out InfinitePatternDefinition flatPattern))
            {
                return false;
            }

            for (int i = 0; i < catalog.Count; i++)
            {
                if (!catalog.TryGetAt(i, out InfinitePatternDefinition pattern) ||
                    !pattern.CanConnectTo(flatPattern))
                {
                    return false;
                }
            }

            _catalog = catalog;
            _isInitialized = true;
            return true;
        }

        public bool StartRun(int seed)
        {
            if (!_isInitialized || _isRunning)
            {
                return false;
            }

            ResetRunState();
            _randomState = unchecked((uint)seed);

            if (_randomState == 0u)
            {
                _randomState = 0x6D2B79F5u;
            }

            _currentPatternId = InfinitePatternCatalogFactory.FlatId;
            _consecutiveSelectionCount = 1;
            _isRunning = true;
            return true;
        }

        public bool TrySelectNext(
            int requestId,
            E_InfinitePatternDifficulty difficulty,
            out string patternId)
        {
            patternId = null;

            if (!CanSelect() ||
                !IsValidDifficulty(difficulty) ||
                (_hasRequest && requestId == _lastRequestId))
            {
                return false;
            }

            _candidates.Clear();

            for (int i = 0; i < _catalog.Count; i++)
            {
                _catalog.TryGetAt(i, out InfinitePatternDefinition candidate);

                if (candidate.MinimumDifficulty <= difficulty &&
                    _catalog.CanConnect(_currentPatternId, candidate.Id) &&
                    (candidate.Id != _currentPatternId ||
                     _consecutiveSelectionCount < MaximumConsecutiveSelections))
                {
                    _candidates.Add(candidate);
                }
            }

            InfinitePatternDefinition selectedPattern;

            if (_candidates.Count > 0)
            {
                selectedPattern = _candidates[NextIndex(_candidates.Count)];
            }
            else if (!_catalog.TryGet(
                         InfinitePatternCatalogFactory.FlatId,
                         out selectedPattern) ||
                     selectedPattern.MinimumDifficulty > difficulty ||
                     !_catalog.CanConnect(_currentPatternId, selectedPattern.Id))
            {
                return false;
            }

            if (selectedPattern.Id == _currentPatternId)
            {
                _consecutiveSelectionCount++;
            }
            else
            {
                _currentPatternId = selectedPattern.Id;
                _consecutiveSelectionCount = 1;
            }

            _lastRequestId = requestId;
            _hasRequest = true;
            patternId = selectedPattern.Id;
            return true;
        }

        public bool Pause()
        {
            if (!_isInitialized || !_isRunning || _isPaused || _hasEnded)
            {
                return false;
            }

            _isPaused = true;
            return true;
        }

        public bool Resume()
        {
            if (!_isInitialized || !_isRunning || !_isPaused || _hasEnded)
            {
                return false;
            }

            _isPaused = false;
            return true;
        }

        public bool EndRun()
        {
            if (!_isInitialized || !_isRunning || _hasEnded)
            {
                return false;
            }

            _isRunning = false;
            _isPaused = false;
            _hasEnded = true;
            return true;
        }

        private bool CanSelect()
        {
            return _isInitialized && _isRunning && !_isPaused && !_hasEnded;
        }

        private int NextIndex(int count)
        {
            _randomState ^= _randomState << 13;
            _randomState ^= _randomState >> 17;
            _randomState ^= _randomState << 5;
            return (int)(_randomState % (uint)count);
        }

        private void ResetAllState()
        {
            _catalog = null;
            _isInitialized = false;
            ResetRunState();
        }

        private void ResetRunState()
        {
            _currentPatternId = null;
            _consecutiveSelectionCount = 0;
            _lastRequestId = 0;
            _randomState = 0u;
            _isRunning = false;
            _isPaused = false;
            _hasEnded = false;
            _hasRequest = false;
            _candidates.Clear();
        }

        private static bool IsValidDifficulty(
            E_InfinitePatternDifficulty difficulty)
        {
            return difficulty == E_InfinitePatternDifficulty.D1 ||
                   difficulty == E_InfinitePatternDifficulty.D2 ||
                   difficulty == E_InfinitePatternDifficulty.D3;
        }
    }
}
