using UnityEngine;
using UnityEngine.EventSystems;

namespace FlowState.Runtime.Systems
{
    public sealed class SettingsRebindButtonFocusHandler : MonoBehaviour, ISelectHandler
    {
        [SerializeField] private SettingsUIController _controller;
        [SerializeField] private int _index;

        public bool IsConfiguredFor(SettingsUIController controller, int index)
        {
            return _controller == controller && _index == index;
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (_controller != null) _controller.HandleRebindButtonSelected(_index);
        }
    }
}
