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
        [SerializeField] private TMP_Text _stageCollectibleScoreText;
        [SerializeField] private TMP_Text _resultStatusText;
        [SerializeField] private TMP_Text _clearTimeText;
        [SerializeField] private TMP_Text _stageResultCollectibleScoreText;
        [SerializeField] private TMP_Text _distanceText;
        [SerializeField] private TMP_Text _baseDistanceScoreText;
        [SerializeField] private TMP_Text _momentumBonusText;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _infiniteCollectibleScoreText;
        [SerializeField] private TMP_Text _infiniteTotalScoreText;
        [SerializeField] private TMP_Text _infiniteDifficultyText;
        [SerializeField] private bool _showDifficultyInDevelopment = true;
        [SerializeField] private TMP_Text _finalDistanceText;
        [SerializeField] private TMP_Text _infiniteResultBaseDistanceScoreText;
        [SerializeField] private TMP_Text _infiniteResultMomentumBonusText;
        [SerializeField] private TMP_Text _finalScoreText;
        [SerializeField] private TMP_Text _infiniteResultCollectibleScoreText;
        [SerializeField] private TMP_Text _infiniteResultTotalScoreText;
        [SerializeField] private TMP_Text _infiniteResultMaximumMomentumText;
        [SerializeField] private GameObject _momentumHud;
        [SerializeField] private TMP_Text _momentumMultiplierText;
        [SerializeField] private Image _momentumDurationFillImage;
        [SerializeField] private Gradient _momentumDurationGradient;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _pauseResumeButton;
        [SerializeField] private Button _pauseRetryButton;
        [SerializeField] private Button _pauseQuitButton;
        [SerializeField] private GameObject _mainMenuPanel;
        [SerializeField] private GameObject _modeSelectPanel;
        [SerializeField] private GameObject _pauseMainMenuConfirmPanel;
        [SerializeField] private GameObject _leaderboardPanel;
        [SerializeField] private GameObject _howToPlayPanel;
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private Button _mainMenuPlayButton;
        [SerializeField] private Button _mainMenuHowToPlayButton;
        [SerializeField] private Button _mainMenuLeaderboardButton;
        [SerializeField] private Button _mainMenuSettingsButton;
        [SerializeField] private Button _mainMenuQuitButton;
        [SerializeField] private Button _modeSelectStageButton;
        [SerializeField] private Button _modeSelectInfiniteButton;
        [SerializeField] private Button _modeSelectBackButton;
        [SerializeField] private Button _pauseSettingsButton;
        [SerializeField] private Button _pauseMainMenuButton;
        [SerializeField] private Button _pauseMainMenuConfirmButton;
        [SerializeField] private Button _pauseMainMenuCancelButton;
        [SerializeField] private Button _resultMainMenuButton;
        [SerializeField] private Button _leaderboardBackButton;
        [SerializeField] private Button _howToPlayBackButton;
        [SerializeField] private Button _settingsBackButton;

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
        private int _lastDisplayedCollectibleScore;
        private int _lastDisplayedTotalScore;
        private bool _hasDisplayedDistance;
        private bool _hasDisplayedScore;
        private bool _hasDisplayedCollectibleScore;
        private bool _hasDisplayedTotalScore;
        private bool _lastDistanceWasValid;
        private bool _lastScoreWasValid;
        private bool _lastCollectibleScoreWasValid;
        private bool _lastTotalScoreWasValid;
        private bool _isMomentumHudConfigured;
        private MomentumGradientEffect _momentumGradientEffect;
        private bool _isInitialized;
        private E_NavigationScreen _currentNavigationScreen =
            E_NavigationScreen.Boot;

        public E_UIState CurrentUIState => _currentUIState;

        public E_ResultMenuSelection CurrentResultMenuSelection =>
            _currentResultMenuSelection;

        public E_PauseMenuSelection CurrentPauseMenuSelection =>
            _pauseMenuState.CurrentSelection;

        public bool IsPauseMenuActive => _pauseMenuState.IsActive;

        public E_NavigationScreen CurrentNavigationScreen =>
            _currentNavigationScreen;

        public bool HasNavigationUIConfiguration =>
            _mainMenuPanel != null &&
            _modeSelectPanel != null &&
            _pauseMainMenuConfirmPanel != null &&
            _leaderboardPanel != null &&
            _howToPlayPanel != null &&
            _settingsPanel != null &&
            _mainMenuPlayButton != null &&
            _mainMenuHowToPlayButton != null &&
            _mainMenuLeaderboardButton != null &&
            _mainMenuSettingsButton != null &&
            _mainMenuQuitButton != null &&
            _modeSelectStageButton != null &&
            _modeSelectInfiniteButton != null &&
            _modeSelectBackButton != null &&
            _pauseResumeButton != null &&
            _pauseRetryButton != null &&
            _pauseSettingsButton != null &&
            _pauseMainMenuButton != null &&
            _pauseMainMenuConfirmButton != null &&
            _pauseMainMenuCancelButton != null &&
            _retryButton != null &&
            _resultMainMenuButton != null &&
            _leaderboardBackButton != null &&
            _howToPlayBackButton != null &&
            _settingsBackButton != null;

        private void Update()
        {
            if (!_isInitialized ||
                _runtimeData == null ||
                _currentGameState != E_GameState.Playing)
            {
                return;
            }

            if (_currentGameMode == E_GameMode.Stage)
            {
                UpdateStageHud();
            }
            else if (_currentGameMode == E_GameMode.Infinite)
            {
                UpdateInfiniteHud();
            }
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
            ResetHudDisplay();
            ConfigureDifficultyVisibility();
            ConfigureMomentumHud();
            ResetResultDisplay();
            _isInitialized = true;
            SetUIState(E_UIState.None);

            Debug.Log("[UIManagementSystem] Initialized.");
        }

        public void InitializeMenu()
        {
            _runtimeData = null;
            _currentGameMode = E_GameMode.Stage;
            _currentGameState = E_GameState.None;
            _currentResultMenuSelection = E_ResultMenuSelection.Retry;
            _pauseMenuState.Deactivate();
            _visibilityState.Reset();
            ResetHudDisplay();
            ResetResultDisplay();
            _isInitialized = true;
            SetUIState(E_UIState.None);
        }

        public void SetNavigationScreen(
            E_NavigationScreen screen,
            E_NavigationItem selection)
        {
            _currentNavigationScreen = screen;
            SetNavigationUIActive(
                _mainMenuPanel,
                screen == E_NavigationScreen.MainMenu);
            SetNavigationUIActive(
                _modeSelectPanel,
                screen == E_NavigationScreen.ModeSelect);
            SetNavigationUIActive(
                _pauseMainMenuConfirmPanel,
                screen == E_NavigationScreen.PauseMainMenuConfirmation);
            SetNavigationUIActive(
                _leaderboardPanel,
                screen == E_NavigationScreen.LeaderboardUnavailable);
            SetNavigationUIActive(
                _howToPlayPanel,
                screen == E_NavigationScreen.HowToPlay);
            SetNavigationUIActive(
                _settingsPanel,
                screen == E_NavigationScreen.Settings);

            if (screen == E_NavigationScreen.Playing)
            {
                SetNavigationUIActive(_pausePanel, false);
                SetNavigationUIActive(_resultPanel, false);
            }
            else if (screen == E_NavigationScreen.Pause)
            {
                SetNavigationUIActive(_pausePanel, true);
                SetNavigationUIActive(_resultPanel, false);
            }
            else if (screen == E_NavigationScreen.Result)
            {
                SetNavigationUIActive(_pausePanel, false);
                SetNavigationUIActive(_resultPanel, true);
            }
            else if (screen != E_NavigationScreen.Initializing)
            {
                SetNavigationUIActive(_pausePanel, false);
                SetNavigationUIActive(_resultPanel, false);
            }

            SelectNavigationButton(screen, selection);
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
                    out string resultStatusText,
                    out string elapsedTimeText,
                    out string stageCollectibleScoreText))
            {
                if (_resultStatusText == null ||
                    _clearTimeText == null ||
                    _stageResultCollectibleScoreText == null)
                {
                    Debug.LogError(
                        "[UIManagementSystem] Stage Result Text is not assigned.");
                    return false;
                }

                _resultStatusText.text = resultStatusText;
                _clearTimeText.text = elapsedTimeText;
                _stageResultCollectibleScoreText.text =
                    stageCollectibleScoreText;
                SetTextIfChanged(_finalDistanceText, string.Empty);
                SetTextIfChanged(
                    _infiniteResultBaseDistanceScoreText,
                    string.Empty);
                SetTextIfChanged(
                    _infiniteResultMomentumBonusText,
                    string.Empty);
                SetTextIfChanged(_finalScoreText, string.Empty);
                SetTextIfChanged(
                    _infiniteResultCollectibleScoreText,
                    string.Empty);
                SetTextIfChanged(_infiniteResultTotalScoreText, string.Empty);
                SetTextIfChanged(
                    _infiniteResultMaximumMomentumText,
                    string.Empty);
                return true;
            }

            if (ResultTextFormatter.TryFormatInfiniteResult(
                    resultData,
                    out string finalDistanceText,
                    out string baseDistanceScoreText,
                    out string momentumBonusText,
                    out string distanceScoreText,
                    out string infiniteCollectibleScoreText,
                    out string totalScoreText,
                    out string maximumMomentumText))
            {
                if (_finalDistanceText == null ||
                    _infiniteResultBaseDistanceScoreText == null ||
                    _infiniteResultMomentumBonusText == null ||
                    _finalScoreText == null ||
                    _infiniteResultCollectibleScoreText == null ||
                    _infiniteResultTotalScoreText == null ||
                    _infiniteResultMaximumMomentumText == null)
                {
                    Debug.LogError(
                        "[UIManagementSystem] Infinite Result Text is not assigned.");
                    return false;
                }

                SetTextIfChanged(_resultStatusText, string.Empty);
                SetTextIfChanged(_clearTimeText, string.Empty);
                SetTextIfChanged(
                    _stageResultCollectibleScoreText,
                    string.Empty);
                _finalDistanceText.text = finalDistanceText;
                _infiniteResultBaseDistanceScoreText.text =
                    baseDistanceScoreText;
                _infiniteResultMomentumBonusText.text = momentumBonusText;
                _finalScoreText.text = distanceScoreText;
                _infiniteResultCollectibleScoreText.text =
                    infiniteCollectibleScoreText;
                _infiniteResultTotalScoreText.text = totalScoreText;
                _infiniteResultMaximumMomentumText.text = maximumMomentumText;
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

            ApplyResultLayoutForCurrentMode();

            SetUIActive(
                _stageHud,
                _visibilityState.IsStageHudVisible,
                nameof(_stageHud));
            SetUIActive(
                _infiniteHud,
                _visibilityState.IsInfiniteHudVisible,
                nameof(_infiniteHud));
            if (_isMomentumHudConfigured)
            {
                _momentumHud.SetActive(
                    _visibilityState.IsInfiniteHudVisible);
            }
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

        private void ApplyResultLayoutForCurrentMode()
        {
            if (!_visibilityState.IsResultPanelVisible ||
                _stageResultContent == null ||
                _infiniteResultContent == null ||
                _retryButton == null ||
                _resultMainMenuButton == null)
            {
                return;
            }

            RectTransform stageContent =
                _stageResultContent.transform as RectTransform;
            RectTransform infiniteContent =
                _infiniteResultContent.transform as RectTransform;
            RectTransform retryButton = _retryButton.transform as RectTransform;
            RectTransform mainMenuButton =
                _resultMainMenuButton.transform as RectTransform;
            RectTransform resultWindow = stageContent?.parent as RectTransform;
            if (stageContent == null || infiniteContent == null ||
                retryButton == null || mainMenuButton == null ||
                resultWindow == null ||
                infiniteContent.parent != resultWindow ||
                retryButton.parent != resultWindow ||
                mainMenuButton.parent != resultWindow)
            {
                return;
            }

            if (_currentGameMode == E_GameMode.Stage)
            {
                SetResultRect(resultWindow, new Vector2(520.0f, 400.0f), 0.0f,
                    new Vector2(0.5f, 0.5f));
                SetResultRect(stageContent, new Vector2(472.0f, 150.0f), -36.0f);
                SetResultRect(retryButton, new Vector2(472.0f, 48.0f), -224.0f);
                SetResultRect(mainMenuButton, new Vector2(472.0f, 48.0f), -282.0f);
                return;
            }

            if (_currentGameMode == E_GameMode.Infinite)
            {
                SetResultRect(resultWindow, new Vector2(520.0f, 520.0f), 0.0f,
                    new Vector2(0.5f, 0.5f));
                SetResultRect(infiniteContent, new Vector2(472.0f, 278.0f), -36.0f);
                SetResultRect(retryButton, new Vector2(472.0f, 48.0f), -334.0f);
                SetResultRect(mainMenuButton, new Vector2(472.0f, 48.0f), -392.0f);
            }
        }

        private static void SetResultRect(
            RectTransform rectTransform,
            Vector2 size,
            float yPosition,
            Vector2? centerAnchor = null)
        {
            Vector2 anchor = centerAnchor ?? new Vector2(0.5f, 1.0f);
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = anchor;
            rectTransform.anchoredPosition = new Vector2(0.0f, yPosition);
            rectTransform.sizeDelta = size;
        }

        private void UpdateInfiniteHud()
        {
            InfiniteModeRuntimeData infiniteModeRuntimeData =
                _runtimeData.InfiniteModeRuntimeData;
            CollectibleRuntimeData collectibleRuntimeData =
                _runtimeData.CollectibleRuntimeData;

            if (!_runtimeData.IsCreated ||
                infiniteModeRuntimeData == null ||
                !infiniteModeRuntimeData.IsInitialized ||
                collectibleRuntimeData == null ||
                !collectibleRuntimeData.IsInitialized)
            {
                UpdateDistanceText(-1.0f);
                UpdateBaseDistanceScoreText(-1);
                UpdateMomentumBonusText(-1);
                UpdateScoreText(-1);
                UpdateCollectibleScoreText(
                    _infiniteCollectibleScoreText,
                    -1);
                UpdateTotalScoreText(-1);
                UpdateDifficultyText(E_InfinitePatternDifficulty.None);
                UpdateMomentumHud(double.NaN, double.NaN);
                return;
            }

            UpdateDistanceText(infiniteModeRuntimeData.CurrentDistance);
            UpdateBaseDistanceScoreText(
                infiniteModeRuntimeData.BaseDistanceScore);
            UpdateMomentumBonusText(infiniteModeRuntimeData.MomentumBonus);
            UpdateScoreText(infiniteModeRuntimeData.CurrentScore);
            UpdateCollectibleScoreText(
                _infiniteCollectibleScoreText,
                infiniteModeRuntimeData.CollectibleScore);
            UpdateTotalScoreText(infiniteModeRuntimeData.TotalScore);
            UpdateDifficultyText((E_InfinitePatternDifficulty)
                infiniteModeRuntimeData.CurrentDifficultyLevel);
            UpdateMomentumHud(
                infiniteModeRuntimeData.CurrentMomentumMultiplier,
                infiniteModeRuntimeData.MomentumRemainingRatio);
        }

        private void ConfigureMomentumHud()
        {
            _momentumGradientEffect = _momentumDurationFillImage != null
                ? _momentumDurationFillImage.GetComponent<MomentumGradientEffect>()
                : null;
            _isMomentumHudConfigured =
                _momentumHud != null &&
                _momentumMultiplierText != null &&
                _momentumDurationFillImage != null;

            if (_currentGameMode == E_GameMode.Infinite &&
                !_isMomentumHudConfigured)
            {
                Debug.LogWarning(
                    "[UIManagementSystem] Required Momentum HUD references are invalid. " +
                    $"Root={_momentumHud != null}, " +
                    $"Text={_momentumMultiplierText != null}, " +
                    $"Image={_momentumDurationFillImage != null}, " +
                    "Momentum HUD is disabled.");
            }

            if (_momentumHud != null)
            {
                _momentumHud.SetActive(false);
            }

            if (_isMomentumHudConfigured)
            {
                if (_momentumGradientEffect != null)
                {
                    _momentumGradientEffect.SetGradient(
                        _momentumDurationGradient);
                }
            }
        }

        private void ConfigureDifficultyVisibility()
        {
            if (_infiniteDifficultyText == null)
            {
                return;
            }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _infiniteDifficultyText.gameObject.SetActive(
                _showDifficultyInDevelopment);
#else
            _infiniteDifficultyText.gameObject.SetActive(false);
#endif
        }

        private void UpdateDifficultyText(
            E_InfinitePatternDifficulty difficulty)
        {
            if (_infiniteDifficultyText == null)
            {
                return;
            }

            string value = difficulty == E_InfinitePatternDifficulty.D1 ||
                           difficulty == E_InfinitePatternDifficulty.D2 ||
                           difficulty == E_InfinitePatternDifficulty.D3
                ? $"Difficulty: {difficulty}"
                : "Difficulty: --";
            SetTextIfChanged(_infiniteDifficultyText, value);
        }

        private void UpdateStageHud()
        {
            CollectibleRuntimeData collectibleRuntimeData =
                _runtimeData.CollectibleRuntimeData;

            int collectibleScore = _runtimeData.IsCreated &&
                                   collectibleRuntimeData != null &&
                                   collectibleRuntimeData.IsInitialized
                ? collectibleRuntimeData.CurrentScore
                : -1;
            SetTextIfChanged(
                _stageCollectibleScoreText,
                ResultTextFormatter.FormatStageCollectibleCounter(
                    collectibleScore));
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
                ResultTextFormatter.FormatDistanceScore(score));
            _lastDisplayedScore = score;
            _lastScoreWasValid = isValid;
            _hasDisplayedScore = true;
        }

        private void UpdateBaseDistanceScoreText(int score)
        {
            SetTextIfChanged(
                _baseDistanceScoreText,
                ResultTextFormatter.FormatBaseDistanceScore(score));
        }

        private void UpdateMomentumBonusText(int score)
        {
            SetTextIfChanged(
                _momentumBonusText,
                ResultTextFormatter.FormatMomentumBonus(score));
        }

        private void UpdateMomentumHud(double multiplier, double remainingRatio)
        {
            if (!_isMomentumHudConfigured)
            {
                return;
            }

            MomentumHudPresentation presentation = MomentumHudPresenter.Create(
                multiplier,
                remainingRatio);
            SetTextIfChanged(
                _momentumMultiplierText,
                presentation.MultiplierText);
            _momentumDurationFillImage.fillAmount = presentation.FillAmount;
        }

        private void UpdateCollectibleScoreText(
            TMP_Text targetText,
            int collectibleScore)
        {
            bool isValid = collectibleScore >= 0;

            if (_hasDisplayedCollectibleScore &&
                _lastCollectibleScoreWasValid == isValid &&
                (!isValid ||
                 _lastDisplayedCollectibleScore == collectibleScore))
            {
                return;
            }

            SetTextIfChanged(
                targetText,
                ResultTextFormatter.FormatCollectibleScore(
                    collectibleScore));
            _lastDisplayedCollectibleScore = collectibleScore;
            _lastCollectibleScoreWasValid = isValid;
            _hasDisplayedCollectibleScore = true;
        }

        private void UpdateTotalScoreText(int totalScore)
        {
            bool isValid = totalScore >= 0;

            if (_hasDisplayedTotalScore &&
                _lastTotalScoreWasValid == isValid &&
                (!isValid || _lastDisplayedTotalScore == totalScore))
            {
                return;
            }

            SetTextIfChanged(
                _infiniteTotalScoreText,
                ResultTextFormatter.FormatTotalScore(totalScore));
            _lastDisplayedTotalScore = totalScore;
            _lastTotalScoreWasValid = isValid;
            _hasDisplayedTotalScore = true;
        }

        private void ResetHudDisplay()
        {
            _lastDisplayedDistance = 0.0;
            _lastDisplayedScore = 0;
            _lastDisplayedCollectibleScore = 0;
            _lastDisplayedTotalScore = 0;
            _hasDisplayedDistance = false;
            _hasDisplayedScore = false;
            _hasDisplayedCollectibleScore = false;
            _hasDisplayedTotalScore = false;
            _lastDistanceWasValid = false;
            _lastScoreWasValid = false;
            _lastCollectibleScoreWasValid = false;
            _lastTotalScoreWasValid = false;
            SetTextIfChanged(
                _distanceText,
                ResultTextFormatter.FormatCurrentDistance(-1.0f));
            SetTextIfChanged(
                _scoreText,
                ResultTextFormatter.FormatDistanceScore(-1));
            SetTextIfChanged(
                _baseDistanceScoreText,
                ResultTextFormatter.FormatBaseDistanceScore(-1));
            SetTextIfChanged(
                _momentumBonusText,
                ResultTextFormatter.FormatMomentumBonus(-1));
            SetTextIfChanged(
                _stageCollectibleScoreText,
                ResultTextFormatter.FormatStageCollectibleCounter(-1));
            SetTextIfChanged(
                _infiniteCollectibleScoreText,
                ResultTextFormatter.FormatCollectibleScore(-1));
            SetTextIfChanged(
                _infiniteTotalScoreText,
                ResultTextFormatter.FormatTotalScore(-1));
            UpdateDifficultyText(E_InfinitePatternDifficulty.None);
            MomentumHudPresentation momentum = MomentumHudPresenter.Create(1.0, 0.0);
            SetTextIfChanged(_momentumMultiplierText, momentum.MultiplierText);
            if (_momentumDurationFillImage != null)
            {
                _momentumDurationFillImage.fillAmount = momentum.FillAmount;
            }
        }

        private void ResetResultDisplay()
        {
            SetTextIfChanged(_resultStatusText, string.Empty);
            SetTextIfChanged(_clearTimeText, string.Empty);
            SetTextIfChanged(_stageResultCollectibleScoreText, string.Empty);
            SetTextIfChanged(_finalDistanceText, string.Empty);
            SetTextIfChanged(
                _infiniteResultBaseDistanceScoreText,
                string.Empty);
            SetTextIfChanged(
                _infiniteResultMomentumBonusText,
                string.Empty);
            SetTextIfChanged(_finalScoreText, string.Empty);
            SetTextIfChanged(
                _infiniteResultCollectibleScoreText,
                string.Empty);
            SetTextIfChanged(_infiniteResultTotalScoreText, string.Empty);
            SetTextIfChanged(
                _infiniteResultMaximumMomentumText,
                string.Empty);
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

        private void SelectNavigationButton(
            E_NavigationScreen screen,
            E_NavigationItem selection)
        {
            Button selectedButton = GetNavigationButton(screen, selection);

            if (selectedButton != null)
            {
                selectedButton.Select();
            }
        }

        private Button GetNavigationButton(
            E_NavigationScreen screen,
            E_NavigationItem selection)
        {
            switch (screen)
            {
                case E_NavigationScreen.MainMenu:
                    switch (selection)
                    {
                        case E_NavigationItem.Play:
                            return _mainMenuPlayButton;
                        case E_NavigationItem.HowToPlay:
                            return _mainMenuHowToPlayButton;
                        case E_NavigationItem.Leaderboard:
                            return _mainMenuLeaderboardButton;
                        case E_NavigationItem.Settings:
                            return _mainMenuSettingsButton;
                        case E_NavigationItem.Quit:
                            return _mainMenuQuitButton;
                    }
                    break;

                case E_NavigationScreen.ModeSelect:
                    switch (selection)
                    {
                        case E_NavigationItem.Stage:
                            return _modeSelectStageButton;
                        case E_NavigationItem.Infinite:
                            return _modeSelectInfiniteButton;
                        case E_NavigationItem.Back:
                            return _modeSelectBackButton;
                    }
                    break;

                case E_NavigationScreen.Pause:
                    switch (selection)
                    {
                        case E_NavigationItem.Resume:
                            return _pauseResumeButton;
                        case E_NavigationItem.Retry:
                            return _pauseRetryButton;
                        case E_NavigationItem.Settings:
                            return _pauseSettingsButton;
                        case E_NavigationItem.MainMenu:
                            return _pauseMainMenuButton;
                    }
                    break;

                case E_NavigationScreen.PauseMainMenuConfirmation:
                    return selection == E_NavigationItem.MainMenu
                        ? _pauseMainMenuConfirmButton
                        : _pauseMainMenuCancelButton;

                case E_NavigationScreen.Result:
                    return selection == E_NavigationItem.Retry
                        ? _retryButton
                        : _resultMainMenuButton;

                case E_NavigationScreen.LeaderboardUnavailable:
                    return _leaderboardBackButton;

                case E_NavigationScreen.HowToPlay:
                    return _howToPlayBackButton;

                case E_NavigationScreen.Settings:
                    return _settingsBackButton;
            }

            return null;
        }

        private void SetNavigationUIActive(GameObject uiObject, bool isActive)
        {
            if (uiObject != null)
            {
                uiObject.SetActive(isActive);
            }
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
