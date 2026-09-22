using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace FlowState.Runtime.Systems
{
    public sealed class SettingsUIController : MonoBehaviour
    {
        [SerializeField] private SettingsSystem _settingsSystem;
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private TMP_Text _masterVolumeValueText;
        [SerializeField] private Toggle _fullscreenToggle;
        [SerializeField] private TMP_Text _rebindStatusText;
        [SerializeField] private TMP_Text[] _bindingTexts;
        [SerializeField] private Button[] _rebindButtons;
        [SerializeField] private GameObject _restoreDefaultsConfirmationPanel;
        [SerializeField] private Button _restoreDefaultsButton;
        [SerializeField] private Button _restoreDefaultsConfirmButton;
        private ScrollRect _controlsScrollRect;
        private Button _backButton;
        private bool _isPointerHoverFocusChange;

        private void OnEnable()
        {
            if (_settingsSystem == null) return;
            _settingsSystem.RebindStarted += HandleRebindStarted;
            _settingsSystem.RebindFinished += HandleRebindFinished;
            _settingsSystem.RestoreConfirmationCancelRequested += CancelRestoreDefaults;
            if (_masterVolumeSlider != null)
                _masterVolumeSlider.onValueChanged.AddListener(HandleVolumeChanged);
            _controlsScrollRect = GetComponentInChildren<ScrollRect>(true);
            ConfigureButtonFocusHandlers();
            Refresh();
            SelectBackButton();
        }

        private void OnDisable()
        {
            if (_settingsSystem == null) return;
            _settingsSystem.RebindStarted -= HandleRebindStarted;
            _settingsSystem.RebindFinished -= HandleRebindFinished;
            _settingsSystem.RestoreConfirmationCancelRequested -= CancelRestoreDefaults;
            if (_masterVolumeSlider != null)
                _masterVolumeSlider.onValueChanged.RemoveListener(HandleVolumeChanged);
            _controlsScrollRect = null;
        }

        public void Refresh()
        {
            if (_settingsSystem == null || _settingsSystem.State == null) return;
            int volume = _settingsSystem.State.MasterVolume;
            if (_masterVolumeSlider != null) _masterVolumeSlider.SetValueWithoutNotify(volume / 100f);
            if (_masterVolumeValueText != null) _masterVolumeValueText.text = volume + "%";
            if (_fullscreenToggle != null) _fullscreenToggle.SetIsOnWithoutNotify(_settingsSystem.State.IsFullscreen);
            for (int i = 0; i < _bindingTexts.Length; i++)
            {
                if (_bindingTexts[i] != null && _settingsSystem.TryGetBindingDisplayPath(i, out string path))
                    _bindingTexts[i].text = InputControlPath.ToHumanReadableString(path, InputControlPath.HumanReadableStringOptions.OmitDevice);
            }
        }

        public void RequestRestoreDefaults()
        {
            _settingsSystem.SetRestoreConfirmationOpen(true);
            if (_restoreDefaultsConfirmationPanel != null) _restoreDefaultsConfirmationPanel.SetActive(true);
            else ConfirmRestoreDefaults();
            if (_restoreDefaultsConfirmationPanel != null)
            {
                if (_restoreDefaultsConfirmButton != null)
                    EventSystem.current.SetSelectedGameObject(_restoreDefaultsConfirmButton.gameObject);
            }
        }

        public void ConfirmRestoreDefaults()
        {
            _settingsSystem?.TryRestoreDefaults();
            CloseRestoreDefaultsConfirmation();
            if (_rebindStatusText != null) _rebindStatusText.text = "Defaults restored.";
            Refresh();
        }

        public void CancelRestoreDefaults()
        {
            CloseRestoreDefaultsConfirmation();
        }

        private void HandleRebindStarted(FlowState.Runtime.Core.SettingsBindingTarget target)
        {
            if (_rebindStatusText != null) _rebindStatusText.text = "Press a key. Escape cancels.";
        }

        private void HandleVolumeChanged(float normalizedVolume)
        {
            if (_masterVolumeValueText != null)
                _masterVolumeValueText.text = Mathf.RoundToInt(normalizedVolume * 100f) + "%";
        }

        public void HandleRebindButtonSelected(int index)
        {
            if (!_isPointerHoverFocusChange)
                ScrollToRebindButton(index);
        }

        public void FocusButtonFromPointer(Button button)
        {
            if (button == null || EventSystem.current == null || EventSystem.current.currentSelectedGameObject == button.gameObject)
                return;

            _isPointerHoverFocusChange = true;
            try
            {
                EventSystem.current.SetSelectedGameObject(button.gameObject);
            }
            finally
            {
                _isPointerHoverFocusChange = false;
            }
        }

        private void ConfigureButtonFocusHandlers()
        {
            foreach (Button button in GetComponentsInChildren<Button>(true))
            {
                SettingsButtonHoverFocusHandler hoverHandler = button.GetComponent<SettingsButtonHoverFocusHandler>();
                if (hoverHandler == null)
                    hoverHandler = button.gameObject.AddComponent<SettingsButtonHoverFocusHandler>();
                hoverHandler.Initialize(this, button);

                if (_backButton == null && button.name == "BackButton")
                    _backButton = button;
            }

            if (_rebindButtons == null)
                return;

            for (int i = 0; i < _rebindButtons.Length; i++)
            {
                Button rebindButton = _rebindButtons[i];
                if (rebindButton == null)
                    continue;

                SettingsRebindButtonFocusHandler rebindHandler = rebindButton.GetComponent<SettingsRebindButtonFocusHandler>();
                if (rebindHandler == null)
                    rebindHandler = rebindButton.gameObject.AddComponent<SettingsRebindButtonFocusHandler>();
                rebindHandler.Initialize(this, i);
            }
        }

        private void ScrollToRebindButton(int index)
        {
            if (_controlsScrollRect == null || _rebindButtons.Length <= 1)
                return;

            _controlsScrollRect.verticalNormalizedPosition = 1f - (float)index / (_rebindButtons.Length - 1);
        }

        private void SelectBackButton()
        {
            if (_backButton != null && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(_backButton.gameObject);
        }

        private void HandleRebindFinished(FlowState.Runtime.Core.SettingsBindingTarget target, bool succeeded, string message)
        {
            if (_rebindStatusText != null) _rebindStatusText.text = succeeded ? "Binding updated." : message;
            Refresh();
            for (int i = 0; i < _rebindButtons.Length; i++)
                if (_settingsSystem.TryGetBindingTarget(i, out var candidate) && candidate.Equals(target))
                {
                    if (succeeded && _bindingTexts[i] != null)
                        _bindingTexts[i].text = InputControlPath.ToHumanReadableString(message, InputControlPath.HumanReadableStringOptions.OmitDevice);
                    if (_rebindButtons[i] != null) _rebindButtons[i].Select();
                    break;
                }
        }

        private void CloseRestoreDefaultsConfirmation()
        {
            _settingsSystem?.SetRestoreConfirmationOpen(false);
            if (_restoreDefaultsConfirmationPanel != null)
                _restoreDefaultsConfirmationPanel.SetActive(false);
            if (_restoreDefaultsButton != null && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(_restoreDefaultsButton.gameObject);
        }
    }

    public sealed class SettingsRebindButtonFocusHandler : MonoBehaviour, ISelectHandler
    {
        private SettingsUIController _controller;
        private int _index;

        public void Initialize(SettingsUIController controller, int index)
        {
            _controller = controller;
            _index = index;
        }

        public void OnSelect(BaseEventData eventData)
        {
            _controller?.HandleRebindButtonSelected(_index);
        }
    }

    public sealed class SettingsButtonHoverFocusHandler : MonoBehaviour, IPointerEnterHandler
    {
        private SettingsUIController _controller;
        private Button _button;

        public void Initialize(SettingsUIController controller, Button button)
        {
            _controller = controller;
            _button = button;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _controller?.FocusButtonFromPointer(_button);
        }
    }
}
