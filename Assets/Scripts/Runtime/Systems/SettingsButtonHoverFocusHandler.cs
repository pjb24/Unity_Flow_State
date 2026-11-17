using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FlowState.Runtime.Systems
{
    public sealed class SettingsButtonHoverFocusHandler : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private SettingsUIController _controller;
        [SerializeField] private Button _button;

        public bool IsConfiguredFor(SettingsUIController controller, Button button)
        {
            return _controller == controller && _button == button;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_controller != null) _controller.FocusButtonFromPointer(_button);
        }
    }
}
