using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlowState.Runtime.Systems
{
    public class UIManagementSystem : MonoBehaviour
    {
        [SerializeField] private GameObject _stageHud;
        [SerializeField] private GameObject _infiniteHud;
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private GameObject _stageResultContent;
        [SerializeField] private GameObject _infiniteResultContent;
        [SerializeField] private TMP_Text _clearTimeText;
        [SerializeField] private TMP_Text _distanceText;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _finalDistanceText;
        [SerializeField] private TMP_Text _finalScoreText;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _pauseResumeButton;
        [SerializeField] private Button _pauseRetryButton;
        [SerializeField] private Button _pauseQuitButton;

        private E_UIState _currentUIState;
        private E_GameMode _currentGameMode;
        private E_GameState _currentGameState;
        private E_ResultMenuSelection _currentResultMenuSelection;
        private readonly PauseMenuState _pauseMenuState = new PauseMenuState();
        private readonly UIVisibilityState _visibilityState =
            new UIVisibilityState();
        private GameRuntimeData _runtimeData;
        private double _lastDisplayedDistance;
        private int _lastDisplayedScore;
        private bool _hasDisplayedDistance;
        private bool _hasDisplayedScore;
        private bool _lastDistanceWasValid;
        private bool _lastScoreWasValid;
        private bool _isInitialized;

        public E_UIState CurrentUIState => _currentUIState;

        public E_ResultMenuSelection CurrentResultMenuSelection =>
            _currentResultMenuSelection;

        public E_PauseMenuSelection CurrentPauseMenuSelection =>
            _pauseMenuState.CurrentSelection;

        public bool IsPauseMenuActive => _pauseMenuState.IsActive;

        private void Update()
        {
            if (!_isInitialized ||
                _currentGameMode != E_GameMode.Infinite ||
                _currentGameState != E_GameState.Playing)
            {
                return;
            }

            UpdateInfiniteHud();
        }

        public void Initialize(GameRuntimeData runtimeData)
        {
            _runtimeData = runtimeData;

            if (_runtimeData == null)
            {
                _isInitialized = false;
                Debug.LogError(
                    "[UIManagementSystem] Runtime Data is null.");
                return;
            }

            _currentGameMode = _runtimeData.GameMode;
            _currentResultMenuSelection = E_ResultMenuSelection.Retry;
            _pauseMenuState.Deactivate();
            _visibilityState.Reset();
            ResetInfiniteHudDisplay();
            ResetResultDisplay();
            _isInitialized = true;
            SetUIState(E_UIState.None);

            Debug.Log("[UIManagementSystem] Initialized.");
        }

        public void SetGameState(E_GameState gameState)
        {
            _currentGameState = gameState;

            if (_isInitialized)
            {
                ApplyUIState();
            }
        }

        public void SetUIState(E_UIState uiState)
        {
            _currentUIState = uiState;
            ApplyUIState();
            UpdatePauseMenuState(uiState);

            if (_currentUIState == E_UIState.Result)
            {
                SetResultMenuSelection(E_ResultMenuSelection.Retry);
            }

            Debug.Log($"[UIManagementSystem] UI State changed to {_currentUIState}.");
        }

        public bool MovePauseMenuSelection(float verticalInput)
        {
            if (_currentUIState != E_UIState.Pause ||
                !_pauseMenuState.TryMove(verticalInput))
            {
                return false;
            }

            return ApplyPauseMenuSelection();
        }

        public bool TrySetPauseMenuSelectionAtPointer(
            Vector2 pointerPosition)
        {
            if (_currentUIState != E_UIState.Pause || _pausePanel == null)
            {
                return false;
            }

            if (IsPointerOverButton(pointerPosition, _pauseResumeButton))
            {
                return SetPauseMenuSelection(E_PauseMenuSelection.Resume);
            }

            if (IsPointerOverButton(pointerPosition, _pauseRetryButton))
            {
                return SetPauseMenuSelection(E_PauseMenuSelection.Retry);
            }

            if (IsPointerOverButton(pointerPosition, _pauseQuitButton))
            {
                return SetPauseMenuSelection(E_PauseMenuSelection.Quit);
            }

            return false;
        }

        public bool TrySubmitPauseMenuSelection(
            out E_PauseMenuSelection selection)
        {
            selection = _pauseMenuState.CurrentSelection;
            return _currentUIState == E_UIState.Pause &&
                   _pauseMenuState.TrySubmit(out selection);
        }

        public bool TryCancelPauseMenu(
            out E_PauseMenuSelection selection)
        {
            selection = E_PauseMenuSelection.Resume;
            return _currentUIState == E_UIState.Pause &&
                   _pauseMenuState.TryCancel(out selection);
        }

        public bool TryClickPauseMenuSelection(
            Vector2 pointerPosition,
            out E_PauseMenuSelection executedSelection)
        {
            executedSelection = _pauseMenuState.CurrentSelection;

            if (!TrySetPauseMenuSelectionAtPointer(pointerPosition))
            {
                return false;
            }

            return _pauseMenuState.TrySubmit(out executedSelection);
        }

        public bool SetResultData(ResultData resultData)
        {
            if (resultData == null)
            {
                Debug.LogError("[UIManagementSystem] Result Data is null.");
                return false;
            }

            if (ResultTextFormatter.TryFormatStageResult(
                    resultData,
                    out string clearTimeText))
            {
                if (_clearTimeText == null)
                {
                    Debug.LogError(
                        "[UIManagementSystem] Clear Time Text is not assigned.");
                    return false;
                }

                _clearTimeText.text = clearTimeText;
                SetTextIfChanged(_finalDistanceText, string.Empty);
                SetTextIfChanged(_finalScoreText, string.Empty);
                return true;
            }

            if (ResultTextFormatter.TryFormatInfiniteResult(
                    resultData,
                    out string finalDistanceText,
                    out string finalScoreText))
            {
                if (_finalDistanceText == null || _finalScoreText == null)
                {
                    Debug.LogError(
                        "[UIManagementSystem] Infinite Result Text is not assigned.");
                    return false;
                }

                SetTextIfChanged(_clearTimeText, string.Empty);
                _finalDistanceText.text = finalDistanceText;
                _finalScoreText.text = finalScoreText;
                return true;
            }

            Debug.LogError(
                "[UIManagementSystem] Result Data contract is invalid.");
            return false;
        }

        public bool MoveResultMenuSelection(float verticalInput)
        {
            if (_currentUIState != E_UIState.Result ||
                Mathf.Abs(verticalInput) < 0.5f)
            {
                return false;
            }

            if (verticalInput < 0.0f &&
                _currentResultMenuSelection == E_ResultMenuSelection.Retry)
            {
                return SetResultMenuSelection(E_ResultMenuSelection.Quit);
            }

            if (verticalInput > 0.0f &&
                _currentResultMenuSelection == E_ResultMenuSelection.Quit)
            {
                return SetResultMenuSelection(E_ResultMenuSelection.Retry);
            }

            return false;
        }

        public bool TrySetResultMenuSelectionAtPointer(Vector2 pointerPosition)
        {
            if (_currentUIState != E_UIState.Result ||
                _resultPanel == null)
            {
                return false;
            }

            if (IsPointerOverButton(pointerPosition, _retryButton))
            {
                return SetResultMenuSelection(E_ResultMenuSelection.Retry);
            }

            if (IsPointerOverButton(pointerPosition, _quitButton))
            {
                return SetResultMenuSelection(E_ResultMenuSelection.Quit);
            }

            return false;
        }

        private void ApplyUIState()
        {
            if (!_visibilityState.Apply(
                    _currentGameMode,
                    _currentGameState,
                    _currentUIState))
            {
                Debug.LogError(
                    "[UIManagementSystem] UI visibility state is invalid.");
            }

            SetUIActive(
                _stageHud,
                _visibilityState.IsStageHudVisible,
                nameof(_stageHud));
            SetUIActive(
                _infiniteHud,
                _visibilityState.IsInfiniteHudVisible,
                nameof(_infiniteHud));
            SetUIActive(
                _resultPanel,
                _visibilityState.IsResultPanelVisible,
                nameof(_resultPanel));
            SetUIActive(
                _pausePanel,
                _visibilityState.IsPausePanelVisible,
                nameof(_pausePanel));
            SetUIActive(
                _stageResultContent,
                _visibilityState.IsStageResultContentVisible,
                nameof(_stageResultContent));
            SetUIActive(
                _infiniteResultContent,
                _visibilityState.IsInfiniteResultContentVisible,
                nameof(_infiniteResultContent));
        }

        private void UpdateInfiniteHud()
        {
            InfiniteModeRuntimeData infiniteModeRuntimeData =
                _runtimeData.InfiniteModeRuntimeData;

            if (!_runtimeData.IsCreated ||
                infiniteModeRuntimeData == null ||
                !infiniteModeRuntimeData.IsInitialized)
            {
                UpdateDistanceText(-1.0f);
                UpdateScoreText(-1);
                return;
            }

            UpdateDistanceText(infiniteModeRuntimeData.CurrentDistance);
            UpdateScoreText(infiniteModeRuntimeData.CurrentScore);
        }

        private void UpdateDistanceText(float distance)
        {
            bool isValid = ResultTextFormatter.TryGetDisplayDistance(
                distance,
                out double displayDistance);

            if (_hasDisplayedDistance &&
                _lastDistanceWasValid == isValid &&
                (!isValid || _lastDisplayedDistance == displayDistance))
            {
                return;
            }

            SetTextIfChanged(
                _distanceText,
                ResultTextFormatter.FormatCurrentDistance(distance));
            _lastDisplayedDistance = displayDistance;
            _lastDistanceWasValid = isValid;
            _hasDisplayedDistance = true;
        }

        private void UpdateScoreText(int score)
        {
            bool isValid = score >= 0;

            if (_hasDisplayedScore &&
                _lastScoreWasValid == isValid &&
                (!isValid || _lastDisplayedScore == score))
            {
                return;
            }

            SetTextIfChanged(
                _scoreText,
                ResultTextFormatter.FormatCurrentScore(score));
            _lastDisplayedScore = score;
            _lastScoreWasValid = isValid;
            _hasDisplayedScore = true;
        }

        private void ResetInfiniteHudDisplay()
        {
            _lastDisplayedDistance = 0.0;
            _lastDisplayedScore = 0;
            _hasDisplayedDistance = false;
            _hasDisplayedScore = false;
            _lastDistanceWasValid = false;
            _lastScoreWasValid = false;
            SetTextIfChanged(
                _distanceText,
                ResultTextFormatter.FormatCurrentDistance(-1.0f));
            SetTextIfChanged(
                _scoreText,
                ResultTextFormatter.FormatCurrentScore(-1));
        }

        private void ResetResultDisplay()
        {
            SetTextIfChanged(_clearTimeText, string.Empty);
            SetTextIfChanged(_finalDistanceText, string.Empty);
            SetTextIfChanged(_finalScoreText, string.Empty);
        }

        private void SetTextIfChanged(TMP_Text targetText, string value)
        {
            if (targetText != null && targetText.text != value)
            {
                targetText.text = value;
            }
        }

        private void UpdatePauseMenuState(E_UIState uiState)
        {
            if (uiState == E_UIState.Pause)
            {
                _pauseMenuState.Activate();
                ApplyPauseMenuSelection();
                return;
            }

            _pauseMenuState.Deactivate();
        }

        private bool SetPauseMenuSelection(E_PauseMenuSelection selection)
        {
            return _pauseMenuState.TrySelectAtPointer(selection) &&
                   ApplyPauseMenuSelection();
        }

        private bool ApplyPauseMenuSelection()
        {
            Button selectedButton = null;

            switch (_pauseMenuState.CurrentSelection)
            {
                case E_PauseMenuSelection.Resume:
                    selectedButton = _pauseResumeButton;
                    break;

                case E_PauseMenuSelection.Retry:
                    selectedButton = _pauseRetryButton;
                    break;

                case E_PauseMenuSelection.Quit:
                    selectedButton = _pauseQuitButton;
                    break;
            }

            if (selectedButton == null)
            {
                Debug.LogError(
                    $"[UIManagementSystem] Pause {_pauseMenuState.CurrentSelection} Button is not assigned.");
                return false;
            }

            selectedButton.Select();
            return true;
        }

        private bool SetResultMenuSelection(
            E_ResultMenuSelection resultMenuSelection)
        {
            Button selectedButton = null;

            switch (resultMenuSelection)
            {
                case E_ResultMenuSelection.Retry:
                    selectedButton = _retryButton;
                    break;

                case E_ResultMenuSelection.Quit:
                    selectedButton = _quitButton;
                    break;
            }

            if (selectedButton == null)
            {
                Debug.LogError(
                    $"[UIManagementSystem] {resultMenuSelection} Button is not assigned.");
                return false;
            }

            _currentResultMenuSelection = resultMenuSelection;
            selectedButton.Select();
            return true;
        }

        private bool IsPointerOverButton(
            Vector2 pointerPosition,
            Button button)
        {
            RectTransform buttonRectTransform =
                button != null ? button.transform as RectTransform : null;

            if (buttonRectTransform == null)
            {
                return false;
            }

            Canvas parentCanvas = button.GetComponentInParent<Canvas>();
            Camera eventCamera = parentCanvas != null &&
                                 parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? parentCanvas.worldCamera
                : null;

            return RectTransformUtility.RectangleContainsScreenPoint(
                buttonRectTransform,
                pointerPosition,
                eventCamera);
        }

        private void SetUIActive(GameObject uiObject, bool isActive, string fieldName)
        {
            if (uiObject == null)
            {
                Debug.LogWarning($"[UIManagementSystem] {fieldName} is not assigned.");
                return;
            }

            uiObject.SetActive(isActive);
        }
    }
}
