using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace FlowState.Tests.PlayMode
{
    public class SettingsInteractiveRebindTests
    {
        private Keyboard _keyboard;
        private Gamepad _gamepad;

        [TearDown]
        public void TearDown()
        {
            RemoveTestDevice(_gamepad);
            RemoveTestDevice(_keyboard);
            _gamepad = null;
            _keyboard = null;
        }

        [UnityTest]
        public IEnumerator RebindButtons_UseVirtualKeyboardAndApplyExpectedResults()
        {
            yield return SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Single);
            MonoBehaviour gameSystem = FindBehaviour("GameSystem");
            yield return WaitForCondition(
                () => GetPropertyValue(gameSystem, "CurrentNavigationScreen") == "MainMenu",
                "GameSystem did not complete boot before the rebind test started.");
            _keyboard = InputSystem.AddDevice<Keyboard>();
            yield return null;

            GameObject settingsPanel = FindGameObject("SettingsPanel");
            settingsPanel.SetActive(true);
            yield return null;

            MonoBehaviour settingsSystem = FindBehaviour("SettingsSystem");
            MonoBehaviour playerInputSystem = FindBehaviour("PlayerInputSystem");
            MonoBehaviour controller = FindBehaviour("SettingsUIController");
            Button[] rebindButtons = GetField<Button[]>(controller, "_rebindButtons");
            TMP_Text[] bindingTexts = GetField<TMP_Text[]>(controller, "_bindingTexts");
            TMP_Text statusText = GetField<TMP_Text>(controller, "_rebindStatusText");
            Button restoreDefaultsButton = GetField<Button>(controller, "_restoreDefaultsButton");
            Button restoreDefaultsConfirmButton = GetField<Button>(controller, "_restoreDefaultsConfirmButton");
            GameObject restoreDefaultsConfirmationPanel =
                GetField<GameObject>(controller, "_restoreDefaultsConfirmationPanel");

            Assert.That(rebindButtons, Has.Length.EqualTo(6));
            Assert.That(bindingTexts, Has.Length.EqualTo(6));

            for (int i = 0; i < rebindButtons.Length; i++)
            {
                rebindButtons[i].onClick.Invoke();
                Assert.That(GetBoolProperty(settingsSystem, "IsRebinding"), Is.True,
                    $"Rebind button {i} did not start an interactive rebind.");
                InvokePublicMethod(settingsSystem, "TryCancelRebind");
                yield return null;
                Assert.That(GetBoolProperty(settingsSystem, "IsRebinding"), Is.False);
            }

            _gamepad = InputSystem.AddDevice<Gamepad>();
            EventSystem.current.SetSelectedGameObject(null);
            rebindButtons[0].onClick.Invoke();
            yield return PressAndReleaseLikeUser(_gamepad.buttonSouth);
            Assert.That(GetBoolProperty(settingsSystem, "IsRebinding"), Is.True,
                "A Gamepad button must not complete a keyboard-only rebind.");
            yield return PressAndReleaseLikeUser(_keyboard.escapeKey);
            Assert.That(GetBoolProperty(settingsSystem, "IsRebinding"), Is.False);

            rebindButtons[0].onClick.Invoke();
            Assert.That(GetBoolProperty(settingsSystem, "IsRebinding"), Is.True);
            yield return PressRebindKeyAndWaitForCompletion(_keyboard.rKey, settingsSystem);

            Assert.That(GetBoolProperty(settingsSystem, "IsRebinding"), Is.False);
            Assert.That(GetBindingPath(settingsSystem, 0), Is.EqualTo("<Keyboard>/r"));
            Assert.That(bindingTexts[0].text, Is.EqualTo("R"));
            yield return AssertPlayerActionResolvesReboundControl(
                _keyboard.rKey,
                playerInputSystem,
                settingsSystem,
                0);

            rebindButtons[1].onClick.Invoke();
            yield return PressRebindKeyAndWaitForCompletion(_keyboard.fKey, settingsSystem);

            Assert.That(GetBindingPath(settingsSystem, 1), Is.EqualTo("<Keyboard>/f"));
            Assert.That(bindingTexts[1].text, Is.EqualTo("F"));
            yield return AssertPlayerActionResolvesReboundControl(
                _keyboard.fKey,
                playerInputSystem,
                settingsSystem,
                1);

            rebindButtons[3].onClick.Invoke();
            yield return PressRebindKeyAndWaitForCompletion(_keyboard.eKey, settingsSystem);

            Assert.That(GetBindingPath(settingsSystem, 3), Is.EqualTo("<Keyboard>/e"));
            Assert.That(bindingTexts[3].text, Is.EqualTo("E"));

            rebindButtons[2].onClick.Invoke();
            yield return PressRebindKeyAndWaitForCompletion(_keyboard.iKey, settingsSystem);
            Assert.That(GetBindingPath(settingsSystem, 2), Is.EqualTo("<Keyboard>/i"));
            Assert.That(bindingTexts[2].text, Is.EqualTo("I"));

            rebindButtons[4].onClick.Invoke();
            yield return PressRebindKeyAndWaitForCompletion(_keyboard.jKey, settingsSystem);
            Assert.That(GetBindingPath(settingsSystem, 4), Is.EqualTo("<Keyboard>/j"));
            Assert.That(bindingTexts[4].text, Is.EqualTo("J"));

            rebindButtons[5].onClick.Invoke();
            yield return PressRebindKeyAndWaitForCompletion(_keyboard.lKey, settingsSystem);
            Assert.That(GetBindingPath(settingsSystem, 5), Is.EqualTo("<Keyboard>/l"));
            Assert.That(bindingTexts[5].text, Is.EqualTo("L"));

            rebindButtons[3].onClick.Invoke();
            yield return PressRebindKeyAndWaitForCompletion(_keyboard.iKey, settingsSystem);

            Assert.That(GetBindingPath(settingsSystem, 3), Is.EqualTo("<Keyboard>/e"));
            Assert.That(statusText.text, Does.Contain("conflict"));

            rebindButtons[0].onClick.Invoke();
            yield return PressAndReleaseLikeUser(_keyboard.escapeKey);

            Assert.That(GetBoolProperty(settingsSystem, "IsRebinding"), Is.False);
            Assert.That(settingsPanel.activeSelf, Is.True);
            Assert.That(GetBindingPath(settingsSystem, 0), Is.EqualTo("<Keyboard>/r"));

            restoreDefaultsButton.onClick.Invoke();
            Assert.That(restoreDefaultsConfirmationPanel.activeSelf, Is.True);
            restoreDefaultsConfirmButton.onClick.Invoke();
            Assert.That(GetBindingPath(settingsSystem, 0), Is.EqualTo("<Keyboard>/space"));
            Assert.That(GetBindingPath(settingsSystem, 1), Is.EqualTo("<Keyboard>/leftShift"));
            Assert.That(GetBindingPath(settingsSystem, 3), Is.EqualTo("<Keyboard>/s"));
        }

        private IEnumerator PressRebindKeyAndWaitForCompletion(
            ButtonControl button,
            MonoBehaviour settingsSystem)
        {
            yield return PressAndReleaseLikeUser(button);
            yield return WaitForCondition(
                () => !GetBoolProperty(settingsSystem, "IsRebinding"),
                "Rebind did not complete after the simulated key input.");
        }

        private IEnumerator PressAndReleaseLikeUser(ButtonControl button)
        {
            QueuePressedState(button);
            yield return new WaitForSecondsRealtime(0.1f);
            QueueReleasedState(button);
            yield return null;
        }

        private IEnumerator AssertPlayerActionResolvesReboundControl(
            ButtonControl button,
            MonoBehaviour playerInputSystem,
            MonoBehaviour settingsSystem,
            int definitionIndex)
        {
            InputAction action = GetPlayerBindingAction(
                playerInputSystem,
                GetBindingTarget(settingsSystem, definitionIndex));
            InvokePublicMethod(playerInputSystem, "EnablePlayerActionMap");
            yield return WaitForCondition(
                () => action.enabled && HasResolvedControl(action, button),
                "The rebound Player action did not resolve the simulated keyboard control.");
            InvokePublicMethod(playerInputSystem, "DisablePlayerActionMap");
        }

        private static bool HasResolvedControl(InputAction action, InputControl expectedControl)
        {
            foreach (InputControl control in action.controls)
            {
                if (control == expectedControl)
                {
                    return true;
                }
            }

            return false;
        }

        private static void QueuePressedState(ButtonControl button)
        {
            if (button is KeyControl keyControl && keyControl.device is Keyboard keyboard)
            {
                InputSystem.QueueStateEvent(keyboard, new KeyboardState(keyControl.keyCode));
                return;
            }

            if (button.device is Gamepad gamepad && button == gamepad.buttonSouth)
            {
                InputSystem.QueueStateEvent(
                    gamepad,
                    new GamepadState().WithButton(GamepadButton.South));
                return;
            }

            Assert.Fail($"Unsupported simulated button '{button}'.");
        }

        private static void QueueReleasedState(ButtonControl button)
        {
            if (button.device is Keyboard keyboard)
            {
                InputSystem.QueueStateEvent(keyboard, new KeyboardState());
                return;
            }

            if (button.device is Gamepad)
            {
                InputSystem.QueueStateEvent(
                    (Gamepad)button.device,
                    new GamepadState());
                return;
            }

            Assert.Fail($"Unsupported simulated button '{button}'.");
        }

        private static void RemoveTestDevice(InputDevice device)
        {
            if (device != null && device.added)
            {
                InputSystem.RemoveDevice(device);
            }
        }

        private static IEnumerator WaitForCondition(Func<bool> condition, string failureMessage)
        {
            const float timeoutSeconds = 1.0f;
            float deadline = Time.realtimeSinceStartup + timeoutSeconds;
            while (!condition())
            {
                if (Time.realtimeSinceStartup >= deadline)
                {
                    Assert.Fail(failureMessage);
                }

                yield return null;
            }
        }

        private static object GetBindingTarget(
            MonoBehaviour settingsSystem,
            int definitionIndex)
        {
            MethodInfo method = settingsSystem.GetType().GetMethod(
                "TryGetBindingTarget",
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(method, Is.Not.Null);

            object[] arguments = { definitionIndex, null };
            Assert.That((bool)method.Invoke(settingsSystem, arguments), Is.True);
            return arguments[1];
        }

        private static InputAction GetPlayerBindingAction(
            MonoBehaviour playerInputSystem,
            object target)
        {
            MethodInfo method = playerInputSystem.GetType().GetMethod(
                "TryGetBindingAction",
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(method, Is.Not.Null);

            object[] arguments = { target, null, -1 };
            Assert.That((bool)method.Invoke(playerInputSystem, arguments), Is.True);
            Assert.That(arguments[1], Is.TypeOf<InputAction>());
            return (InputAction)arguments[1];
        }

        private static string GetBindingPath(MonoBehaviour settingsSystem, int definitionIndex)
        {
            MethodInfo method = settingsSystem.GetType().GetMethod(
                "TryGetBindingDisplayPath",
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(method, Is.Not.Null);

            object[] arguments = { definitionIndex, null };
            Assert.That((bool)method.Invoke(settingsSystem, arguments), Is.True);
            return (string)arguments[1];
        }

        private static T GetField<T>(MonoBehaviour component, string fieldName)
        {
            FieldInfo field = component.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            return (T)field.GetValue(component);
        }

        private static bool GetBoolProperty(MonoBehaviour component, string propertyName)
        {
            return (bool)GetProperty(component, propertyName);
        }

        private static string GetPropertyValue(MonoBehaviour component, string propertyName)
        {
            object value = GetProperty(component, propertyName);
            return value?.ToString();
        }

        private static object GetProperty(MonoBehaviour component, string propertyName)
        {
            PropertyInfo property = component.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(property, Is.Not.Null);
            return property.GetValue(component);
        }

        private static object InvokePublicMethod(MonoBehaviour component, string methodName)
        {
            MethodInfo method = component.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(method, Is.Not.Null);
            return method.Invoke(component, null);
        }

        private static MonoBehaviour FindBehaviour(string typeName)
        {
            foreach (MonoBehaviour behaviour in Resources.FindObjectsOfTypeAll<MonoBehaviour>())
            {
                if (behaviour != null && behaviour.gameObject.scene.isLoaded &&
                    behaviour.GetType().Name == typeName)
                {
                    return behaviour;
                }
            }

            Assert.Fail($"{typeName} was not found.");
            return null;
        }

        private static GameObject FindGameObject(string name)
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.name == name && gameObject.scene.isLoaded)
                    return gameObject;
            }

            Assert.Fail($"{name} was not found.");
            return null;
        }
    }
}
