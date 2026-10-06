using System;
using System.Collections.Generic;
using FlowState.Runtime.Features;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlowState.Runtime.Systems
{
    // Keep this host outside Root; hiding Root must not disable its listeners.
    public sealed class AccountTransferView : MonoBehaviour, IAccountTransferView
    {
        private const int PageCount = 5;
        private const int ActionCount = 12;
        [SerializeField] private GameObject _root;
        [SerializeField] private GameObject[] _pages;
        [SerializeField] private CanvasGroup _settingsContent;
        [SerializeField] private TMP_Text _accountText;
        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private TMP_Text _credentialText;
        [SerializeField] private TMP_InputField _codeInput;
        [SerializeField] private TMP_InputField _verificationInput;
        [SerializeField] private Button _openButton;
        [SerializeField] private Button[] _actionButtons;
        [UnityEngine.Serialization.FormerlySerializedAs("_copyDetailsButton")]
        [SerializeField] private Button _copyCodeButton;
        [UnityEngine.Serialization.FormerlySerializedAs("_pasteDetailsButton")]
        [SerializeField] private Button _copyVerificationButton;
        private AccountTransferController _controller;
        private Func<bool> _canEnter;
        private UnityAction[] _handlers;
        private bool _areListenersRegistered;
        private bool _isConfigured;
        private bool _wasOpen;
        private bool _backgroundInteractable;
        private bool _backgroundBlocksRaycasts;
        private E_AccountTransferPage _lastPage;
        private GameObject _returnSelection;
        public bool IsOpen => _controller != null && _controller.State.IsOpen;

        private void OnEnable()
        {
            if (_isConfigured) RegisterListeners();
        }

        private void OnDisable()
        {
            UnregisterListeners();
            if (_controller != null) _controller.Close();
            ClearSensitiveInputs();
        }

        public bool Initialize(AccountTransferController controller, Func<bool> canEnter = null)
        {
            UnregisterListeners();
            if (_controller != null) _controller.Close();
            _isConfigured = false;
            if (controller == null || !HasValidReferences())
            {
                Debug.LogWarning("[AccountTransferView] Configuration invalid; account UI disabled.");
                return false;
            }
            _controller = controller;
            _canEnter = canEnter;
            _handlers = new UnityAction[] { HandleStart, HandleOpenInput, HandleComplete, HandleReissue,
                HandleCancelTransfer, HandleRefresh, HandleConsent, HandleRetryPending, HandleRequestDiscard,
                HandleConfirmDiscard, HandleCancelDiscard, HandleBack };
            _isConfigured = true;
            _codeInput.contentType = TMP_InputField.ContentType.Standard;
            _codeInput.characterLimit = 9;
            _verificationInput.contentType = TMP_InputField.ContentType.Password;
            _verificationInput.characterValidation = TMP_InputField.CharacterValidation.Digit;
            _verificationInput.characterLimit = 9;
            _accountText.richText = false; _statusText.richText = false; _credentialText.richText = false;
            if (_codeInput.textComponent != null) _codeInput.textComponent.richText = false;
            if (_verificationInput.textComponent != null) _verificationInput.textComponent.richText = false;
            ClearSensitiveInputs();
            Render(controller.State);
            if (isActiveAndEnabled) RegisterListeners();
            return true;
        }

        public void SetEntryAllowed(bool allowed)
        {
            if (_openButton != null) _openButton.interactable = allowed && _isConfigured &&
                _controller != null && !_controller.State.IsBusy && !_controller.State.IsOpen;
        }

        public void Close()
        {
            if (_controller != null) _controller.Close();
        }

        public void ClearSensitiveInputs()
        {
            if (_codeInput != null) _codeInput.SetTextWithoutNotify(string.Empty);
            if (_verificationInput != null) _verificationInput.SetTextWithoutNotify(string.Empty);
            if (_credentialText != null) _credentialText.text = string.Empty;
        }

        public void Render(AccountTransferScreenState state)
        {
            if (!_isConfigured || state == null) return;
            bool opened = state.IsOpen && !_wasOpen;
            bool pageChanged = state.Page != _lastPage;
            if (opened)
            {
                _returnSelection = EventSystem.current == null ? _openButton.gameObject : EventSystem.current.currentSelectedGameObject;
                if (_returnSelection == null) _returnSelection = _openButton.gameObject;
                _backgroundInteractable = _settingsContent.interactable;
                _backgroundBlocksRaycasts = _settingsContent.blocksRaycasts;
                _settingsContent.interactable = false;
                _settingsContent.blocksRaycasts = false;
            }
            if (!state.IsOpen && _wasOpen)
            {
                _settingsContent.interactable = _backgroundInteractable;
                _settingsContent.blocksRaycasts = _backgroundBlocksRaycasts;
            }
            _root.SetActive(state.IsOpen);
            _accountText.text = state.IsOpen ? (state.NumberText.Length == 0 ? "Public number: unavailable" : "Public number: " + state.NumberText) +
                "\nA public number cannot recover your account. Use a transfer code and verification value." : string.Empty;
            _statusText.text = state.IsOpen ? state.Message : string.Empty;
            _credentialText.text = state.IsOpen && state.Page == E_AccountTransferPage.Issued && state.Code.Length > 0
                ? "Transfer code: " + state.Code + "\nVerification value: " + state.VerificationValue + "\nExpires (UTC): " + FormatExpiry(state.ExpiresAtMilliseconds) : string.Empty;
            for (int i = 0; i < _pages.Length; i++) _pages[i].SetActive(state.IsOpen && i == (int)state.Page);
            for (int i = 0; i < _actionButtons.Length; i++)
            {
                E_AccountTransferAction action = (E_AccountTransferAction)i;
                _actionButtons[i].gameObject.SetActive(state.IsOpen && IsActionVisible(action, state));
                _actionButtons[i].interactable = _controller.CanExecute(action);
            }
            _codeInput.interactable = !state.IsBusy;
            _verificationInput.interactable = !state.IsBusy;
            bool canCopy = CanCopyCredentials();
            SetClipboardButtonState(_copyCodeButton, canCopy, canCopy);
            SetClipboardButtonState(_copyVerificationButton, canCopy, canCopy);
            if (!state.IsOpen || state.Page != E_AccountTransferPage.Input) ClearInputFields();
            ConfigureNavigation(opened || pageChanged);
            if (!state.IsOpen && _wasOpen && EventSystem.current != null && _returnSelection != null && _returnSelection.activeInHierarchy)
                EventSystem.current.SetSelectedGameObject(_returnSelection);
            _wasOpen = state.IsOpen; _lastPage = state.Page;
        }

        private bool HasValidReferences()
        {
            if (_root == null || _root == gameObject || !_root.transform.IsChildOf(transform) || _settingsContent == null ||
                transform.IsChildOf(_settingsContent.transform) || _accountText == null || _statusText == null ||
                _credentialText == null || _codeInput == null || _verificationInput == null || _openButton == null ||
                _pages == null || _pages.Length != PageCount || _actionButtons == null || _actionButtons.Length != ActionCount ||
                _openButton.onClick.GetPersistentEventCount() != 0) return false;
            for (int i = 0; i < _pages.Length; i++)
            {
                if (_pages[i] == null || !_pages[i].transform.IsChildOf(_root.transform)) return false;
                for (int j = 0; j < i; j++) if (_pages[i] == _pages[j]) return false;
            }
            if (!_codeInput.transform.IsChildOf(_pages[(int)E_AccountTransferPage.Input].transform) ||
                !_verificationInput.transform.IsChildOf(_pages[(int)E_AccountTransferPage.Input].transform)) return false;
            for (int i = 0; i < _actionButtons.Length; i++)
            {
                if (_actionButtons[i] == null || _actionButtons[i] == _openButton ||
                    !_actionButtons[i].transform.IsChildOf(_root.transform) || _actionButtons[i].onClick.GetPersistentEventCount() != 0) return false;
                for (int j = 0; j < i; j++) if (_actionButtons[i] == _actionButtons[j]) return false;
                for (int j = 0; j < _pages.Length; j++)
                    if (_actionButtons[i].transform.IsChildOf(_pages[j].transform)) return false;
            }
            if (_copyCodeButton != null && _copyCodeButton == _copyVerificationButton) return false;
            return HasValidClipboardReference(_copyCodeButton, E_AccountTransferPage.Issued) &&
                HasValidClipboardReference(_copyVerificationButton, E_AccountTransferPage.Issued);
        }

        private bool HasValidClipboardReference(Button button, E_AccountTransferPage page)
        {
            // Missing copy controls leave the existing account screen usable until Scene wiring.
            if (button == null) return true;
            if (!button.transform.IsChildOf(_pages[(int)page].transform) ||
                button == _openButton || button.onClick.GetPersistentEventCount() != 0) return false;
            for (int i = 0; i < _actionButtons.Length; i++) if (button == _actionButtons[i]) return false;
            return true;
        }

        private bool CanCopyCredentials()
        {
            return _isConfigured && _controller != null && _controller.State.IsOpen &&
                !_controller.State.IsBusy && _controller.State.Page == E_AccountTransferPage.Issued &&
                _controller.State.Code.Length > 0 && _controller.State.VerificationValue.Length > 0;
        }

        private static void SetClipboardButtonState(Button button, bool visible, bool interactable)
        {
            if (button == null) return;
            button.gameObject.SetActive(visible);
            button.interactable = interactable;
        }

        private void HandleCopyCode() { CopyCredential(true); }
        private void HandleCopyVerification() { CopyCredential(false); }
        private void CopyCredential(bool copyCode)
        {
            if (!CanCopyCredentials()) return;
            GUIUtility.systemCopyBuffer = copyCode ? _controller.State.Code : _controller.State.VerificationValue;
            _statusText.text = copyCode
                ? "Transfer code copied. Clipboard keeps it after closing. Treat it as a private credential."
                : "Verification value copied. Clipboard keeps it after closing. Treat it as a private credential.";
        }

        private void RegisterListeners()
        {
            if (_areListenersRegistered) return;
            _openButton.onClick.AddListener(HandleOpen);
            for (int i = 0; i < _actionButtons.Length; i++) _actionButtons[i].onClick.AddListener(_handlers[i]);
            if (_copyCodeButton != null) _copyCodeButton.onClick.AddListener(HandleCopyCode);
            if (_copyVerificationButton != null) _copyVerificationButton.onClick.AddListener(HandleCopyVerification);
            _areListenersRegistered = true;
        }

        private void UnregisterListeners()
        {
            if (!_areListenersRegistered) return;
            _openButton.onClick.RemoveListener(HandleOpen);
            for (int i = 0; i < _actionButtons.Length; i++) _actionButtons[i].onClick.RemoveListener(_handlers[i]);
            if (_copyCodeButton != null) _copyCodeButton.onClick.RemoveListener(HandleCopyCode);
            if (_copyVerificationButton != null) _copyVerificationButton.onClick.RemoveListener(HandleCopyVerification);
            _areListenersRegistered = false;
        }

        private void ClearInputFields()
        {
            _codeInput.SetTextWithoutNotify(string.Empty);
            _verificationInput.SetTextWithoutNotify(string.Empty);
        }

        private static bool IsActionVisible(E_AccountTransferAction action, AccountTransferScreenState state)
        {
            bool home = state.Page == E_AccountTransferPage.Overview || state.Page == E_AccountTransferPage.Result;
            switch (action)
            {
                case E_AccountTransferAction.Start:
                case E_AccountTransferAction.OpenInput: return home && !state.IsTransferPending;
                case E_AccountTransferAction.Complete: return state.Page == E_AccountTransferPage.Input;
                case E_AccountTransferAction.Reissue:
                case E_AccountTransferAction.CancelTransfer: return state.IsTransferPending && state.Page != E_AccountTransferPage.DiscardConfirmation;
                case E_AccountTransferAction.ConfirmConsent: return home && !state.HasConsent;
                case E_AccountTransferAction.RetryPending:
                case E_AccountTransferAction.RequestDiscard: return state.PendingCount > 0 && state.Page != E_AccountTransferPage.DiscardConfirmation;
                case E_AccountTransferAction.ConfirmDiscard:
                case E_AccountTransferAction.CancelDiscard: return state.Page == E_AccountTransferPage.DiscardConfirmation;
                case E_AccountTransferAction.Refresh: return state.HasConsent && state.Page != E_AccountTransferPage.DiscardConfirmation;
                default: return true;
            }
        }

        private void ConfigureNavigation(bool selectFirst)
        {
            List<Selectable> controls = new List<Selectable>();
            if (_codeInput.isActiveAndEnabled && _codeInput.interactable) controls.Add(_codeInput);
            if (_verificationInput.isActiveAndEnabled && _verificationInput.interactable) controls.Add(_verificationInput);
            if (_copyCodeButton != null && _copyCodeButton.isActiveAndEnabled && _copyCodeButton.interactable) controls.Add(_copyCodeButton);
            if (_copyVerificationButton != null && _copyVerificationButton.isActiveAndEnabled && _copyVerificationButton.interactable) controls.Add(_copyVerificationButton);
            for (int i = 0; i < _actionButtons.Length; i++)
                if (_actionButtons[i].isActiveAndEnabled && _actionButtons[i].interactable) controls.Add(_actionButtons[i]);
            for (int i = 0; i < controls.Count; i++)
            {
                Navigation navigation = new Navigation { mode = Navigation.Mode.Explicit,
                    selectOnUp = controls[Math.Max(0, i - 1)], selectOnDown = controls[Math.Min(controls.Count - 1, i + 1)] };
                controls[i].navigation = navigation;
            }
            if (controls.Count == 0 || EventSystem.current == null) return;
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            bool hasSelection = false;
            for (int i = 0; i < controls.Count; i++) if (selected == controls[i].gameObject) hasSelection = true;
            if (selectFirst || !hasSelection) EventSystem.current.SetSelectedGameObject(controls[0].gameObject);
        }

        private static string FormatExpiry(long milliseconds)
        {
            if (milliseconds < 0 || milliseconds > 253402300799999L) return "Unavailable";
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private async void HandleOpen()
        { if (_controller != null && (_canEnter == null || _canEnter())) await _controller.OpenAsync(); }
        private async void HandleStart() { await _controller.ExecuteAsync(E_AccountTransferAction.Start); }
        private async void HandleOpenInput() { await _controller.ExecuteAsync(E_AccountTransferAction.OpenInput); }
        private async void HandleComplete() { await _controller.ExecuteAsync(E_AccountTransferAction.Complete, _codeInput.text, _verificationInput.text); }
        private async void HandleReissue() { await _controller.ExecuteAsync(E_AccountTransferAction.Reissue); }
        private async void HandleCancelTransfer() { await _controller.ExecuteAsync(E_AccountTransferAction.CancelTransfer); }
        private async void HandleRefresh() { await _controller.ExecuteAsync(E_AccountTransferAction.Refresh); }
        private async void HandleConsent() { await _controller.ExecuteAsync(E_AccountTransferAction.ConfirmConsent); }
        private async void HandleRetryPending() { await _controller.ExecuteAsync(E_AccountTransferAction.RetryPending); }
        private async void HandleRequestDiscard() { await _controller.ExecuteAsync(E_AccountTransferAction.RequestDiscard); }
        private async void HandleConfirmDiscard() { await _controller.ExecuteAsync(E_AccountTransferAction.ConfirmDiscard); }
        private async void HandleCancelDiscard() { await _controller.ExecuteAsync(E_AccountTransferAction.CancelDiscard); }
        private void HandleBack() { Close(); }
    }
}
