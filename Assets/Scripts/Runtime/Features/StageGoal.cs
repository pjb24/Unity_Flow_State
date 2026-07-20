using System;
using UnityEngine;

namespace FlowState.Runtime.Features
{
    public class StageGoal : MonoBehaviour
    {
        [SerializeField] private Collider _playerCollider;

        private event Action GoalReached;

        private bool _isInitialized;
        private bool _isReached;

        public bool IsReached => _isReached;

        public bool Initialize()
        {
            if (_playerCollider == null)
            {
                _isInitialized = false;
                Debug.LogError("[StageGoal] Player Collider is not assigned.");
                return false;
            }

            ResetGoal();
            _isInitialized = true;
            return true;
        }

        public void ResetGoal()
        {
            _isReached = false;
        }

        public void AddListener(Action listener)
        {
            GoalReached -= listener;
            GoalReached += listener;
        }

        public void RemoveListener(Action listener)
        {
            GoalReached -= listener;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isInitialized || _isReached || other != _playerCollider)
            {
                return;
            }

            _isReached = true;
            GoalReached?.Invoke();
        }
    }
}
