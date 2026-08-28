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
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private TMP_Text _clearTimeText;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _quitButton;

        private E_UIState _currentUIState;
        private E_ResultMenuSelection _currentResultMenuSelection;

        public E_UIState CurrentUIState => _currentUIState;

        public E_ResultMenuSelection CurrentResultMenuSelection =>
            _currentResultMenuSelection;

        public void Initialize()
        {
            SetUIState(E_UIState.None);

            Debug.Log("[UIManagementSystem] Initialized.");
        }

        public void SetUIState(E_UIState uiState)
        {
            _currentUIState = uiState;
            ApplyUIState();

            if (_currentUIState == E_UIState.Result)
            {
                SetResultMenuSelection(E_ResultMenuSelection.Retry);
            }

            Debug.Log($"[UIManagementSystem] UI State changed to {_currentUIState}.");
        }

        public bool SetResultData(ResultData resultData)
        {
            if (resultData == null)
            {
                Debug.LogError("[UIManagementSystem] Result Data is null.");
                return false;
            }

            if (_clearTimeText == null)
            {
                Debug.LogError(
                    "[UIManagementSystem] Clear Time Text is not assigned.");
                return false;
            }

            _clearTimeText.text =
                ResultTextFormatter.FormatClearTime(resultData.ClearTime);
            return true;
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
            SetUIActive(_stageHud, _currentUIState == E_UIState.StageHud, nameof(_stageHud));
            SetUIActive(_resultPanel, _currentUIState == E_UIState.Result, nameof(_resultPanel));
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
