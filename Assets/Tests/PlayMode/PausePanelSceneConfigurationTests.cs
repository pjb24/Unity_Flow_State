using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace FlowState.Tests.PlayMode
{
    public class PausePanelSceneConfigurationTests
    {
        private const string SceneName = "SampleScene";

        [UnityTest]
        public IEnumerator PausePanel_HasRequiredHierarchyReferencesAndStateMapping()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(
                SceneName,
                LoadSceneMode.Single);

            while (!loadOperation.isDone)
            {
                yield return null;
            }

            yield return null;

            GameObject uiRoot = FindSceneGameObject("UIRoot");
            GameObject pausePanel = FindSceneGameObject("PausePanel");
            Assert.That(pausePanel.transform.parent, Is.EqualTo(uiRoot.transform));
            Assert.That(pausePanel.activeSelf, Is.False);

            MonoBehaviour uiManagementSystem = FindRequiredBehaviour(
                "UIManagementSystem",
                "UIManagementSystem");
            GameObject confirmation = GetPrivateField<GameObject>(uiManagementSystem, "_pauseMainMenuConfirmPanel");
            Button confirm = FindDescendantButton(confirmation, "MainMenuButton");
            Button cancel = FindDescendantButton(confirmation, "CancelButton");
            TMP_Text title = confirmation.transform.Find("TitleText").GetComponent<TMP_Text>();
            TMP_Text action = confirm.GetComponentInChildren<TMP_Text>(true);
            Assert.That(GetPrivateField<TMP_Text>(uiManagementSystem, "_pauseConfirmationTitleText"), Is.SameAs(title));
            Assert.That(GetPrivateField<TMP_Text>(uiManagementSystem, "_pauseConfirmationActionText"), Is.SameAs(action));
            Assert.That(confirm.onClick.GetPersistentEventCount(), Is.EqualTo(1));
            Assert.That(confirm.onClick.GetPersistentMethodName(0), Is.EqualTo("SelectPauseConfirmation"));
            InvokeSetNavigationScreen(uiManagementSystem, E_NavigationScreen.PauseRetryConfirmation, E_NavigationItem.Cancel);
            Assert.That(confirmation.activeSelf, Is.True);
            Assert.That(pausePanel.activeSelf, Is.False);
            Assert.That(title.text, Is.EqualTo("Retry this Run?"));
            Assert.That(action.text, Is.EqualTo("Retry"));
            Assert.That(EventSystem.current.currentSelectedGameObject, Is.SameAs(cancel.gameObject));
            InvokeSetNavigationScreen(uiManagementSystem, E_NavigationScreen.PauseMainMenuConfirmation, E_NavigationItem.MainMenu);
            Assert.That(title.text, Is.EqualTo("Return to Main Menu?"));
            Assert.That(action.text, Is.EqualTo("Main Menu"));
            Assert.That(EventSystem.current.currentSelectedGameObject, Is.SameAs(confirm.gameObject));

            Button resumeButton = FindDescendantButton(pausePanel, "ResumeButton");
            Button retryButton = FindDescendantButton(pausePanel, "RetryButton");
            Button mainMenuButton = FindDescendantButton(
                pausePanel,
                "MainMenuButton");
            Assert.That(
                GetPrivateField<GameObject>(uiManagementSystem, "_pausePanel"),
                Is.EqualTo(pausePanel));
            Assert.That(
                GetPrivateField<Button>(uiManagementSystem, "_pauseResumeButton"),
                Is.EqualTo(resumeButton));
            Assert.That(
                GetPrivateField<Button>(uiManagementSystem, "_pauseRetryButton"),
                Is.EqualTo(retryButton));
            Assert.That(
                GetPrivateField<Button>(uiManagementSystem, "_pauseMainMenuButton"),
                Is.EqualTo(mainMenuButton));

            InvokeSetNavigationScreen(
                uiManagementSystem,
                E_NavigationScreen.Pause,
                E_NavigationItem.Resume);
            Assert.That(pausePanel.activeSelf, Is.True);
            Assert.That(EventSystem.current, Is.Not.Null);
            Assert.That(EventSystem.current.currentSelectedGameObject,
                Is.EqualTo(resumeButton.gameObject));

            InvokeSetNavigationScreen(
                uiManagementSystem,
                E_NavigationScreen.Playing,
                E_NavigationItem.None);
            Assert.That(pausePanel.activeSelf, Is.False);
        }

        private GameObject FindSceneGameObject(string objectName)
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.name == objectName &&
                    gameObject.scene.IsValid() &&
                    gameObject.scene.isLoaded)
                {
                    return gameObject;
                }
            }

            Assert.Fail($"{objectName} was not found in the loaded Scene.");
            return null;
        }

        private Button FindDescendantButton(GameObject root, string objectName)
        {
            foreach (Button button in root.GetComponentsInChildren<Button>(true))
            {
                if (button.name == objectName)
                {
                    return button;
                }
            }

            Assert.Fail($"{objectName} Button was not found under {root.name}.");
            return null;
        }

        private MonoBehaviour FindRequiredBehaviour(string objectName, string typeName)
        {
            GameObject target = FindSceneGameObject(objectName);

            foreach (MonoBehaviour behaviour in target.GetComponents<MonoBehaviour>())
            {
                if (behaviour != null && behaviour.GetType().Name == typeName)
                {
                    return behaviour;
                }
            }

            Assert.Fail($"{typeName} was not found on {objectName}.");
            return null;
        }

        private T GetPrivateField<T>(MonoBehaviour target, string fieldName)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            return (T)field.GetValue(target);
        }

        private void InvokeSetNavigationScreen(
            MonoBehaviour target,
            E_NavigationScreen screen,
            E_NavigationItem selection)
        {
            MethodInfo method = target.GetType().GetMethod(
                "SetNavigationScreen",
                BindingFlags.Instance | BindingFlags.Public,
                null,
                new[]
                {
                    typeof(E_NavigationScreen),
                    typeof(E_NavigationItem)
                },
                null);
            Assert.That(method, Is.Not.Null);
            method.Invoke(target, new object[] { screen, selection });
        }
    }
}
