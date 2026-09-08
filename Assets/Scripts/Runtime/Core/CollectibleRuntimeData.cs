using System;
using System.Collections.Generic;

namespace FlowState.Runtime.Core
{
    public class CollectibleRuntimeData
    {
        public const int DefaultScorePerCollectible = 10;

        private readonly Dictionary<long, Dictionary<string, bool>> _scopes =
            new Dictionary<long, Dictionary<string, bool>>();
        private long _lastScopeId;
        private int _scorePerCollectible;
        private int _currentScore;
        private int _registeredCount;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;

        public int ScorePerCollectible => _scorePerCollectible;

        public int CurrentScore => _currentScore;

        public int ActiveScopeCount => _scopes.Count;

        public int RegisteredCount => _registeredCount;

        public bool Initialize(int scorePerCollectible = DefaultScorePerCollectible)
        {
            if (_isInitialized || scorePerCollectible <= 0)
            {
                return false;
            }

            _scorePerCollectible = scorePerCollectible;
            _isInitialized = true;
            return true;
        }

        public bool TryCreateScope(out long scopeId)
        {
            scopeId = 0;
            if (!_isInitialized || _lastScopeId == long.MaxValue)
            {
                return false;
            }

            scopeId = ++_lastScopeId;
            _scopes.Add(scopeId, new Dictionary<string, bool>(StringComparer.Ordinal));
            return true;
        }

        public bool TryRegister(long scopeId, string collectibleId)
        {
            if (!_isInitialized ||
                string.IsNullOrWhiteSpace(collectibleId) ||
                _registeredCount == int.MaxValue ||
                !_scopes.TryGetValue(scopeId, out Dictionary<string, bool> entries) ||
                entries.ContainsKey(collectibleId))
            {
                return false;
            }

            entries.Add(collectibleId, false);
            _registeredCount++;
            return true;
        }

        public bool TryCollect(long scopeId, string collectibleId)
        {
            if (!_isInitialized ||
                string.IsNullOrWhiteSpace(collectibleId) ||
                !_scopes.TryGetValue(scopeId, out Dictionary<string, bool> entries) ||
                !entries.TryGetValue(collectibleId, out bool isCollected) ||
                isCollected)
            {
                return false;
            }

            entries[collectibleId] = true;
            _currentScore = _currentScore > int.MaxValue - _scorePerCollectible
                ? int.MaxValue
                : _currentScore + _scorePerCollectible;
            return true;
        }

        public bool TryReleaseScope(long scopeId)
        {
            if (!_isInitialized ||
                !_scopes.TryGetValue(scopeId, out Dictionary<string, bool> entries))
            {
                return false;
            }

            _registeredCount -= entries.Count;
            _scopes.Remove(scopeId);
            return true;
        }

        public void Clear()
        {
            _scopes.Clear();
            _registeredCount = 0;
            _currentScore = 0;
            _scorePerCollectible = 0;
            _isInitialized = false;
            // Keep issued IDs retired even if this instance is initialized again.
        }
    }
}
