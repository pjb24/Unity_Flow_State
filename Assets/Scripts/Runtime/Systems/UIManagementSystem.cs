using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using TMPro;
using UnityEngine;

namespace FlowState.Runtime.Systems
{
    public class UIManagementSystem : MonoBehaviour
    {
        [SerializeField] private GameObject _stageHud;
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private TMP_Text _clearTimeText;

        private E_UIState _currentUIState;

        public E_UIState CurrentUIState => _currentUIState;

        public void Initialize()
        {
            SetUIState(E_UIState.None);

            Debug.Log("[UIManagementSystem] Initialized.");
        }

        public void SetUIState(E_UIState uiState)
        {
            _currentUIState = uiState;
            ApplyUIState();

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

        private void ApplyUIState()
        {
            SetUIActive(_stageHud, _currentUIState == E_UIState.StageHud, nameof(_stageHud));
            SetUIActive(_resultPanel, _currentUIState == E_UIState.Result, nameof(_resultPanel));
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
