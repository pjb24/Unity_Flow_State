using UnityEngine;
using FlowState.Runtime.Core;

namespace FlowState.Runtime.Systems
{
    public sealed class ApplicationQuitService : IApplicationQuitService
    {
        public void RequestQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
