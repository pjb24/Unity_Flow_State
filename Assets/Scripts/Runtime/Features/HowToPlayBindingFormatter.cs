using System;
using FlowState.Runtime.Core;
using UnityEngine.InputSystem;

namespace FlowState.Runtime.Features
{
    public static class HowToPlayBindingFormatter
    {
        public static string GetDisplayText(
            InputAction action,
            E_InputDisplayDevice displayDevice)
        {
            if (action == null)
            {
                return "Unassigned";
            }

            for (int index = 0; index < action.bindings.Count; index++)
            {
                InputBinding binding = action.bindings[index];

                if (!MatchesDevice(binding, displayDevice))
                {
                    continue;
                }

                string displayText = action.GetBindingDisplayString(index);

                if (!string.IsNullOrEmpty(displayText))
                {
                    return displayText;
                }
            }

            return "Unassigned";
        }

        private static bool MatchesDevice(
            InputBinding binding,
            E_InputDisplayDevice displayDevice)
        {
            string controlPath = binding.effectivePath;

            if (string.IsNullOrEmpty(controlPath))
            {
                return false;
            }

            if (displayDevice == E_InputDisplayDevice.Gamepad)
            {
                return controlPath.StartsWith(
                    "<Gamepad>", StringComparison.OrdinalIgnoreCase);
            }

            return controlPath.StartsWith(
                       "<Keyboard>", StringComparison.OrdinalIgnoreCase) ||
                   controlPath.StartsWith(
                       "<Mouse>", StringComparison.OrdinalIgnoreCase);
        }
    }
}
