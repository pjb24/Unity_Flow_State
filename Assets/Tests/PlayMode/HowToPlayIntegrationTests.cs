using System;
using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace FlowState.Tests.PlayMode
{
    public class HowToPlayIntegrationTests
    {
        [UnityTest]
        public IEnumerator HowToPlay_BindingText_UpdatesAndRestores()
        {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;

            MonoBehaviour uiManagementSystem = FindSystem(
                "FlowState.Runtime.Systems.UIManagementSystem");
            MonoBehaviour playerInputSystem = FindSystem(
                "FlowState.Runtime.Systems.PlayerInputSystem");
            TMP_Text jumpBindingText = GetPrivateField<TMP_Text>(
                uiManagementSystem,
                "_howToPlayJumpBindingText");
            InputAction jumpAction = GetPlayerAction(playerInputSystem, "Jump");
            int keyboardBindingIndex = FindKeyboardBindingIndex(jumpAction);
            jumpAction.RemoveBindingOverride(keyboardBindingIndex);
            string defaultDisplayText = jumpAction.GetBindingDisplayString(
                keyboardBindingIndex);

            InvokePublicMethod(
                uiManagementSystem,
                "SetNavigationScreen",
                E_NavigationScreen.HowToPlay,
                E_NavigationItem.Back);
            yield return null;
            Assert.That(
                jumpBindingText.text,
                Is.EqualTo($"Jump: {defaultDisplayText}"));

            jumpAction.ApplyBindingOverride(keyboardBindingIndex, "<Keyboard>/r");
            yield return null;
            Assert.That(jumpBindingText.text, Is.EqualTo("Jump: R"));

            jumpAction.RemoveBindingOverride(keyboardBindingIndex);
            yield return null;
            Assert.That(
                jumpBindingText.text,
                Is.EqualTo($"Jump: {defaultDisplayText}"));
        }

        [UnityTest]
        public IEnumerator HowToPlayUi_UsesSerializedReferencesAndButtonContracts()
        {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;

            MonoBehaviour gameSystem = FindSystem(
                "FlowState.Runtime.Systems.GameSystem");
            MonoBehaviour uiManagementSystem = FindSystem(
                "FlowState.Runtime.Systems.UIManagementSystem");
            MonoBehaviour playerInputSystem = FindSystem(
                "FlowState.Runtime.Systems.PlayerInputSystem");
            MonoBehaviour uiInputSystem = FindSystem(
                "FlowState.Runtime.Systems.UIInputSystem");

            GameObject howToPlayPanel = GetPrivateField<GameObject>(
                uiManagementSystem,
                "_howToPlayPanel");
            Button backButton = GetPrivateField<Button>(
                uiManagementSystem,
                "_howToPlayBackButton");
            Button startRunButton = GetPrivateField<Button>(
                uiManagementSystem,
                "_howToPlayStartRunButton");
            TMP_Text jumpBindingText = GetPrivateField<TMP_Text>(
                uiManagementSystem,
                "_howToPlayJumpBindingText");
            TMP_Text momentumLandingBindingText = GetPrivateField<TMP_Text>(
                uiManagementSystem,
                "_howToPlayMomentumLandingBindingText");
            TMP_Text pauseBindingText = GetPrivateField<TMP_Text>(
                uiManagementSystem,
                "_howToPlayPauseBindingText");

            Assert.That(howToPlayPanel, Is.Not.Null);
            Assert.That(howToPlayPanel.activeSelf, Is.False);
            Assert.That(howToPlayPanel.GetComponent<Image>(), Is.Not.Null);
            Assert.That(
                howToPlayPanel.GetComponent<Image>().raycastTarget,
                Is.True);
            Assert.That(
                backButton.transform.parent,
                Is.EqualTo(howToPlayPanel.transform));
            Assert.That(
                startRunButton.transform.parent,
                Is.EqualTo(howToPlayPanel.transform));
            Assert.That(
                jumpBindingText.transform.parent,
                Is.EqualTo(howToPlayPanel.transform));
            Assert.That(
                momentumLandingBindingText.transform.parent,
                Is.EqualTo(howToPlayPanel.transform));
            Assert.That(
                pauseBindingText.transform.parent,
                Is.EqualTo(howToPlayPanel.transform));
            Assert.That(
                GetPrivateField<MonoBehaviour>(
                    uiManagementSystem,
                    "_playerInputSystem"),
                Is.SameAs(playerInputSystem));
            Assert.That(
                GetPrivateField<MonoBehaviour>(
                    uiManagementSystem,
                    "_uiInputSystem"),
                Is.SameAs(uiInputSystem));

            AssertButtonContract(backButton, gameSystem, "SelectBack");
            AssertButtonContract(startRunButton, gameSystem, "SelectStartRun");
            Assert.That(jumpBindingText.raycastTarget, Is.False);
            Assert.That(momentumLandingBindingText.raycastTarget, Is.False);
            Assert.That(pauseBindingText.raycastTarget, Is.False);
        }

        [UnityTest]
        public IEnumerator FirstModeSelection_BlocksRunUntilStartRun()
        {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            yield return null;

            MonoBehaviour gameSystem = FindSystem(
                "FlowState.Runtime.Systems.GameSystem");
            MonoBehaviour runtimeDataSystem = FindSystem(
                "FlowState.Runtime.Systems.RuntimeDataSystem");
            MonoBehaviour playerInputSystem = FindSystem(
                "FlowState.Runtime.Systems.PlayerInputSystem");
            MonoBehaviour uiInputSystem = FindSystem(
                "FlowState.Runtime.Systems.UIInputSystem");

            InvokePublicMethod(gameSystem, "SelectPlay");
            InvokePublicMethod(gameSystem, "SelectInfinite");

            Assert.That(
                GetPublicProperty<E_NavigationScreen>(
                    gameSystem,
                    "CurrentNavigationScreen"),
                Is.EqualTo(E_NavigationScreen.AutomaticHowToPlay));
            Assert.That(
                GetPublicProperty<bool>(runtimeDataSystem, "HasRuntimeData"),
                Is.False);
            Assert.That(
                GetPublicProperty<bool>(
                    playerInputSystem,
                    "IsPlayerActionMapEnabled"),
                Is.False);
            Assert.That(
                GetPublicProperty<bool>(uiInputSystem, "IsUIActionMapEnabled"),
                Is.True);

            InvokePublicMethod(gameSystem, "SelectStartRun");
            yield return null;

            Assert.That(
                GetPublicProperty<E_NavigationScreen>(
                    gameSystem,
                    "CurrentNavigationScreen"),
                Is.Not.EqualTo(E_NavigationScreen.AutomaticHowToPlay));
            Assert.That(
                GetPublicProperty<bool>(runtimeDataSystem, "HasRuntimeData"),
                Is.True);
        }

        private static MonoBehaviour FindSystem(string fullTypeName)
        {
            Type systemType = FindType(fullTypeName);
            MonoBehaviour system =
                UnityEngine.Object.FindFirstObjectByType(systemType)
                as MonoBehaviour;
            Assert.That(system, Is.Not.Null, $"{fullTypeName} was not found.");
            return system;
        }

        private static Type FindType(string fullTypeName)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int index = 0; index < assemblies.Length; index++)
            {
                Type type = assemblies[index].GetType(fullTypeName, false);

                if (type != null)
                {
                    return type;
                }
            }

            Assert.Fail($"{fullTypeName} was not found.");
            return null;
        }

        private static void InvokePublicMethod(
            MonoBehaviour target,
            string methodName,
            params object[] arguments)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(method, Is.Not.Null, $"{methodName} was not found.");
            method.Invoke(target, arguments);
        }

        private static T GetPublicProperty<T>(
            MonoBehaviour target,
            string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(property, Is.Not.Null, $"{propertyName} was not found.");
            return (T)property.GetValue(target);
        }

        private static T GetPrivateField<T>(
            MonoBehaviour target,
            string fieldName)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, $"{fieldName} was not found.");
            T value = (T)field.GetValue(target);
            Assert.That(value, Is.Not.Null, $"{fieldName} was not assigned.");
            return value;
        }

        private static void AssertButtonContract(
            Button button,
            MonoBehaviour gameSystem,
            string expectedMethodName)
        {
            Assert.That(
                button.navigation.mode,
                Is.EqualTo(Navigation.Mode.None));
            Assert.That(
                button.onClick.GetPersistentEventCount(),
                Is.EqualTo(1));
            Assert.That(
                button.onClick.GetPersistentTarget(0),
                Is.SameAs(gameSystem));
            Assert.That(
                button.onClick.GetPersistentMethodName(0),
                Is.EqualTo(expectedMethodName));
        }

        private static InputAction GetPlayerAction(
            MonoBehaviour playerInputSystem,
            string actionName)
        {
            MethodInfo method = playerInputSystem.GetType().GetMethod(
                "TryGetPlayerAction",
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(method, Is.Not.Null);

            object[] arguments = { actionName, null };
            Assert.That((bool)method.Invoke(playerInputSystem, arguments), Is.True);
            Assert.That(arguments[1], Is.TypeOf<InputAction>());
            return (InputAction)arguments[1];
        }

        private static int FindKeyboardBindingIndex(InputAction action)
        {
            for (int index = 0; index < action.bindings.Count; index++)
            {
                string controlPath = action.bindings[index].effectivePath;

                if (!string.IsNullOrEmpty(controlPath) &&
                    controlPath.StartsWith("<Keyboard>", StringComparison.OrdinalIgnoreCase))
                {
                    return index;
                }
            }

            Assert.Fail("Jump action does not have a keyboard binding.");
            return -1;
        }
    }
}
