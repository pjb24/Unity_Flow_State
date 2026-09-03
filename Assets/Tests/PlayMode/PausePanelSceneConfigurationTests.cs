using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
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

            Button resumeButton = FindDescendantButton(pausePanel, "ResumeButton");
            Button retryButton = FindDescendantButton(pausePanel, "RetryButton");
            Button quitButton = FindDescendantButton(pausePanel, "QuitButton");
            MonoBehaviour uiManagementSystem = FindRequiredBehaviour(
                "UIManagementSystem",
                "UIManagementSystem");

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
                GetPrivateField<Button>(uiManagementSystem, "_pauseQuitButton"),
                Is.EqualTo(quitButton));

            InvokeSetGameState(uiManagementSystem, E_GameState.Paused);
            InvokeSetUIState(uiManagementSystem, E_UIState.Pause);
            Assert.That(pausePanel.activeSelf, Is.True);
            Assert.That(EventSystem.current, Is.Not.Null);
            Assert.That(EventSystem.current.currentSelectedGameObject,
                Is.EqualTo(resumeButton.gameObject));

            InvokeSetGameState(uiManagementSystem, E_GameState.Playing);
            InvokeSetUIState(uiManagementSystem, E_UIState.StageHud);
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

        private void InvokeSetUIState(MonoBehaviour target, E_UIState state)
        {
            MethodInfo method = target.GetType().GetMethod(
                "SetUIState",
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(method, Is.Not.Null);
            method.Invoke(target, new object[] { state });
        }

        private void InvokeSetGameState(
            MonoBehaviour target,
            E_GameState state)
        {
            MethodInfo method = target.GetType().GetMethod(
                "SetGameState",
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(method, Is.Not.Null);
            method.Invoke(target, new object[] { state });
        }
    }
}
