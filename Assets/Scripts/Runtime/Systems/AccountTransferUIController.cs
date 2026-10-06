using UnityEngine;

namespace FlowState.Runtime.Systems
{
    public sealed class AccountTransferUIController : MonoBehaviour
    {
        [SerializeField] private GameSystem _gameSystem;
        [SerializeField] private AccountTransferView _view;
        private bool _isBound;
        private bool _hasBindingFailure;
        public bool IsOpen => _view != null && _view.IsOpen;

        private void LateUpdate()
        {
            // GameSystem initialization can finish after this host is enabled.
            // Binding is local-only; never initialize a second SDK/save/session.
            if (!_isBound && !_hasBindingFailure && _gameSystem != null && _view != null)
            {
                FlowState.Runtime.Features.AccountTransferController controller = _gameSystem.CreateAccountTransferController(_view);
                if (controller != null)
                {
                    _isBound = _view.Initialize(controller, CanEnter);
                    if (_isBound && !_gameSystem.RegisterAccountTransferUI(this))
                    {
                        _view.Close(); _isBound = false;
                        Debug.LogWarning("[AccountTransferUIController] Duplicate account UI binding refused.");
                    }
                    _hasBindingFailure = !_isBound;
                }
            }
            if (_view != null) _view.SetEntryAllowed(_isBound && CanEnter());
        }

        private void OnDisable() { if (_view != null) _view.Close(); }
        private void OnDestroy() { if (_gameSystem != null) _gameSystem.UnregisterAccountTransferUI(this); }
        public void Close() { if (_view != null) _view.Close(); }
        private bool CanEnter() { return _gameSystem != null && _gameSystem.CanOpenAccountTransfer; }
    }
}
