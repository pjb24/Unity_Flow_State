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
    public class ModeUISceneConfigurationTests
    {
        private const string SceneName = "SampleScene";

        [UnityTest]
        public IEnumerator ModeUI_HasRequiredHierarchyComponentsAndReferences()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(
                SceneName,
                LoadSceneMode.Single);

            while (!loadOperation.isDone)
            {
                yield return null;
            }

            yield return null;

            GameObject uiRoot = FindUniqueSceneObject("UIRoot");
            GameObject stageHud = FindDirectChild(uiRoot, "StageHUD");
            GameObject infiniteHud = FindDirectChild(uiRoot, "InfiniteHUD");
            GameObject resultPanel = FindDirectChild(uiRoot, "ResultPanel");
            GameObject pausePanel = FindDirectChild(uiRoot, "PausePanel");

            RequireDirectCanvas(stageHud);
            GameObject infiniteCanvas = RequireDirectCanvas(infiniteHud);
            Image infiniteHudBackground = FindDirectComponent<Image>(
                infiniteCanvas,
                "Image");
            TMP_Text distanceText = FindDirectComponent<TMP_Text>(
                infiniteHudBackground.gameObject,
                "DistanceText");
            TMP_Text scoreText = FindDirectComponent<TMP_Text>(
                infiniteHudBackground.gameObject,
                "ScoreText");

            GameObject resultCanvas = RequireDirectCanvas(resultPanel);
            GameObject resultContainer = FindDirectChild(resultCanvas, "Panel");
            GameObject stageResultContent = FindDirectChild(
                resultContainer,
                "StageResultContent");
            GameObject infiniteResultContent = FindDirectChild(
                resultContainer,
                "InfiniteResultContent");
            TMP_Text clearTimeText = FindUniqueDescendantComponent<TMP_Text>(
                stageResultContent,
                "ClearTimeText");
            Image infiniteResultBackground = FindDirectComponent<Image>(
                infiniteResultContent,
                "Infinite Result Image");
            TMP_Text finalDistanceText = FindDirectComponent<TMP_Text>(
                infiniteResultBackground.gameObject,
                "FinalDistanceText");
            TMP_Text finalScoreText = FindDirectComponent<TMP_Text>(
                infiniteResultBackground.gameObject,
                "FinalScoreText");
            Button resultRetryButton = FindDirectComponent<Button>(
                resultContainer,
                "RetryButton");
            Button resultQuitButton = FindDirectComponent<Button>(
                resultContainer,
                "QuitButton");

            GameObject pauseCanvas = RequireDirectCanvas(pausePanel);
            GameObject pauseContainer = FindDirectChild(pauseCanvas, "Panel");
            FindUniqueDescendantComponent<TMP_Text>(
                pauseContainer,
                "Pause Title Text");
            Button pauseResumeButton = FindDirectComponent<Button>(
                pauseContainer,
                "ResumeButton");
            Button pauseRetryButton = FindDirectComponent<Button>(
                pauseContainer,
                "RetryButton");
            Button pauseQuitButton = FindDirectComponent<Button>(
                pauseContainer,
                "QuitButton");

            MonoBehaviour uiManagementSystem = FindRequiredBehaviour(
                "UIManagementSystem",
                "UIManagementSystem");
            AssertSerializedReference(uiManagementSystem, "_stageHud", stageHud);
            AssertSerializedReference(
                uiManagementSystem,
                "_infiniteHud",
                infiniteHud);
            AssertSerializedReference(
                uiManagementSystem,
                "_resultPanel",
                resultPanel);
            AssertSerializedReference(
                uiManagementSystem,
                "_pausePanel",
                pausePanel);
            AssertSerializedReference(
                uiManagementSystem,
                "_stageResultContent",
                stageResultContent);
            AssertSerializedReference(
                uiManagementSystem,
                "_infiniteResultContent",
                infiniteResultContent);
            AssertSerializedReference(
                uiManagementSystem,
                "_clearTimeText",
                clearTimeText);
            AssertSerializedReference(
                uiManagementSystem,
                "_distanceText",
                distanceText);
            AssertSerializedReference(
                uiManagementSystem,
                "_scoreText",
                scoreText);
            AssertSerializedReference(
                uiManagementSystem,
                "_finalDistanceText",
                finalDistanceText);
            AssertSerializedReference(
                uiManagementSystem,
                "_finalScoreText",
                finalScoreText);
            AssertSerializedReference(
                uiManagementSystem,
                "_retryButton",
                resultRetryButton);
            AssertSerializedReference(
                uiManagementSystem,
                "_quitButton",
                resultQuitButton);
            AssertSerializedReference(
                uiManagementSystem,
                "_pauseResumeButton",
                pauseResumeButton);
            AssertSerializedReference(
                uiManagementSystem,
                "_pauseRetryButton",
                pauseRetryButton);
            AssertSerializedReference(
                uiManagementSystem,
                "_pauseQuitButton",
                pauseQuitButton);

            EventSystem[] eventSystems = FindSceneComponents<EventSystem>();
            Assert.That(eventSystems, Has.Length.EqualTo(1));
        }

        private GameObject RequireDirectCanvas(GameObject parent)
        {
            GameObject canvasObject = FindDirectChild(parent, "Canvas");
            Assert.That(canvasObject.GetComponent<Canvas>(), Is.Not.Null);
            Assert.That(canvasObject.GetComponent<CanvasScaler>(), Is.Not.Null);
            Assert.That(
                canvasObject.GetComponent<GraphicRaycaster>(),
                Is.Not.Null);
            return canvasObject;
        }

        private GameObject FindUniqueSceneObject(string objectName)
        {
            GameObject[] matches = FindSceneObjects(objectName);
            Assert.That(
                matches,
                Has.Length.EqualTo(1),
                $"Scene must contain exactly one {objectName}.");
            return matches[0];
        }

        private GameObject FindDirectChild(GameObject parent, string childName)
        {
            GameObject match = null;

            foreach (Transform child in parent.transform)
            {
                if (child.name != childName)
                {
                    continue;
                }

                Assert.That(
                    match,
                    Is.Null,
                    $"{parent.name} has duplicate direct child {childName}.");
                match = child.gameObject;
            }

            Assert.That(
                match,
                Is.Not.Null,
                $"{childName} must be a direct child of {parent.name}.");
            return match;
        }

        private T FindDirectComponent<T>(GameObject parent, string childName)
            where T : Component
        {
            GameObject child = FindDirectChild(parent, childName);
            T component = child.GetComponent<T>();
            Assert.That(
                component,
                Is.Not.Null,
                $"{childName} must have {typeof(T).Name}.");
            return component;
        }

        private T FindUniqueDescendantComponent<T>(
            GameObject parent,
            string descendantName)
            where T : Component
        {
            T match = null;

            foreach (T component in parent.GetComponentsInChildren<T>(true))
            {
                if (component.name != descendantName)
                {
                    continue;
                }

                Assert.That(
                    match,
                    Is.Null,
                    $"{parent.name} has duplicate {descendantName}.");
                match = component;
            }

            Assert.That(match, Is.Not.Null);
            return match;
        }

        private GameObject[] FindSceneObjects(string objectName)
        {
            System.Collections.Generic.List<GameObject> matches =
                new System.Collections.Generic.List<GameObject>();

            foreach (GameObject gameObject in
                     Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.name == objectName &&
                    gameObject.scene.IsValid() &&
                    gameObject.scene.isLoaded)
                {
                    matches.Add(gameObject);
                }
            }

            return matches.ToArray();
        }

        private T[] FindSceneComponents<T>() where T : Component
        {
            System.Collections.Generic.List<T> matches =
                new System.Collections.Generic.List<T>();

            foreach (T component in Resources.FindObjectsOfTypeAll<T>())
            {
                if (component.gameObject.scene.IsValid() &&
                    component.gameObject.scene.isLoaded)
                {
                    matches.Add(component);
                }
            }

            return matches.ToArray();
        }

        private MonoBehaviour FindRequiredBehaviour(
            string objectName,
            string typeName)
        {
            GameObject target = FindUniqueSceneObject(objectName);

            foreach (MonoBehaviour behaviour in
                     target.GetComponents<MonoBehaviour>())
            {
                if (behaviour != null && behaviour.GetType().Name == typeName)
                {
                    return behaviour;
                }
            }

            Assert.Fail($"{typeName} was not found on {objectName}.");
            return null;
        }

        private void AssertSerializedReference<T>(
            MonoBehaviour target,
            string fieldName,
            T expected)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            Assert.That(field.GetValue(target), Is.EqualTo(expected));
        }
    }
}
