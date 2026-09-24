using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine.InputSystem;

namespace FlowState.Tests.EditMode
{
    public class HowToPlayBindingFormatterTests
    {
        [Test]
        public void GetDisplayText_KeyboardMouse_UsesKeyboardBinding()
        {
            InputAction action = CreateAction();

            try
            {
                Assert.That(
                    HowToPlayBindingFormatter.GetDisplayText(
                        action,
                        E_InputDisplayDevice.KeyboardMouse),
                    Is.EqualTo(action.GetBindingDisplayString(0)));
            }
            finally
            {
                action.Dispose();
            }
        }

        [Test]
        public void GetDisplayText_Gamepad_UsesGamepadBinding()
        {
            InputAction action = CreateAction();

            try
            {
                Assert.That(
                    HowToPlayBindingFormatter.GetDisplayText(
                        action,
                        E_InputDisplayDevice.Gamepad),
                    Is.EqualTo(action.GetBindingDisplayString(1)));
            }
            finally
            {
                action.Dispose();
            }
        }

        [Test]
        public void GetDisplayText_CancelBindings_UseDeviceSpecificDisplay()
        {
            InputAction action = new InputAction();
            action.AddBinding("<Keyboard>/escape");
            action.AddBinding("<Gamepad>/buttonEast");

            try
            {
                Assert.That(
                    HowToPlayBindingFormatter.GetDisplayText(
                        action,
                        E_InputDisplayDevice.KeyboardMouse),
                    Is.EqualTo(action.GetBindingDisplayString(0)));
                Assert.That(
                    HowToPlayBindingFormatter.GetDisplayText(
                        action,
                        E_InputDisplayDevice.Gamepad),
                    Is.EqualTo(action.GetBindingDisplayString(1)));
            }
            finally
            {
                action.Dispose();
            }
        }

        [Test]
        public void GetDisplayText_NullAction_ReturnsUnassigned()
        {
            Assert.That(
                HowToPlayBindingFormatter.GetDisplayText(
                    null,
                    E_InputDisplayDevice.KeyboardMouse),
                Is.EqualTo("Unassigned"));
        }

        private static InputAction CreateAction()
        {
            InputAction action = new InputAction();
            action.AddBinding("<Keyboard>/space");
            action.AddBinding("<Gamepad>/buttonSouth");
            return action;
        }
    }
}
