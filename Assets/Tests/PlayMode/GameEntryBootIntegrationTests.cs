using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class GameEntryBootIntegrationTests
    {
        private const string SceneName = "SampleScene";

        [UnityTest]
        public IEnumerator Boot_OpensMainMenuWithoutCreatingRun()
        {
            yield return LoadProductionScene();

            MonoBehaviour gameSystem = FindBehaviour("GameSystem", "GameSystem");
            MonoBehaviour runtimeDataSystem = FindBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");
            MonoBehaviour playerInputSystem = FindBehaviour(
                "PlayerInputSystem",
                "PlayerInputSystem");
            MonoBehaviour stageSystem = FindBehaviour("StageSystem", "StageSystem");
            MonoBehaviour movementSystem = FindBehaviour(
                "PlayerMovementSystem",
                "PlayerMovementSystem");
            MonoBehaviour cameraFollow = FindBehaviour("CameraRig", "CameraFollow");

            Assert.That(
                GetProperty<E_NavigationScreen>(
                    gameSystem,
                    "CurrentNavigationScreen"),
                Is.EqualTo(E_NavigationScreen.MainMenu));
            Assert.That(
                GetProperty<E_GameState>(gameSystem, "CurrentGameState"),
                Is.EqualTo(E_GameState.None));
            Assert.That(
                GetProperty<bool>(runtimeDataSystem, "HasRuntimeData"),
                Is.False);
            Assert.That(
                GetProperty<bool>(playerInputSystem, "IsPlayerActionMapEnabled"),
                Is.False);
            Assert.That(GetProperty<bool>(stageSystem, "IsPlaying"), Is.False);
            Assert.That(GetProperty<bool>(movementSystem, "IsRunning"), Is.False);
            Assert.That(GetProperty<bool>(cameraFollow, "IsFollowing"), Is.False);
        }

        [UnityTest]
        public IEnumerator ModeSubmit_CreatesOnlyTheSelectedModeRun()
        {
            yield return LoadProductionScene();

            MonoBehaviour gameSystem = FindBehaviour("GameSystem", "GameSystem");
            MonoBehaviour runtimeDataSystem = FindBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");

            InvokeNavigationSelection(gameSystem, E_NavigationItem.Play);
            Assert.That(
                GetProperty<E_NavigationScreen>(
                    gameSystem,
                    "CurrentNavigationScreen"),
                Is.EqualTo(E_NavigationScreen.ModeSelect));
            Assert.That(
                GetProperty<bool>(runtimeDataSystem, "HasRuntimeData"),
                Is.False);

            InvokeNavigationSelection(gameSystem, E_NavigationItem.Infinite);

            Assert.That(
                GetProperty<E_GameState>(gameSystem, "CurrentGameState"),
                Is.EqualTo(E_GameState.Playing));
            object runtimeData = GetProperty<object>(
                runtimeDataSystem,
                "RuntimeData");
            Assert.That(runtimeData, Is.Not.Null);
            Assert.That(
                GetObjectProperty<E_GameMode>(runtimeData, "GameMode"),
                Is.EqualTo(E_GameMode.Infinite));
        }

        [UnityTest]
        public IEnumerator PauseSettingsBack_PreservesTheCurrentRun()
        {
            yield return LoadProductionScene();

            MonoBehaviour gameSystem = FindBehaviour("GameSystem", "GameSystem");
            MonoBehaviour runtimeDataSystem = FindBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");
            InvokeNavigationSelection(gameSystem, E_NavigationItem.Play);
            InvokeNavigationSelection(gameSystem, E_NavigationItem.Stage);
            object runtimeData = GetProperty<object>(
                runtimeDataSystem,
                "RuntimeData");

            InvokePublic(gameSystem, "PauseGame");
            InvokeNavigationSelection(gameSystem, E_NavigationItem.Settings);

            Assert.That(
                GetProperty<E_NavigationScreen>(
                    gameSystem,
                    "CurrentNavigationScreen"),
                Is.EqualTo(E_NavigationScreen.Settings));
            Assert.That(
                GetProperty<object>(runtimeDataSystem, "RuntimeData"),
                Is.SameAs(runtimeData));

            InvokeNavigationSelection(gameSystem, E_NavigationItem.Back);

            Assert.That(
                GetProperty<E_NavigationScreen>(
                    gameSystem,
                    "CurrentNavigationScreen"),
                Is.EqualTo(E_NavigationScreen.Pause));
            Assert.That(
                GetProperty<E_NavigationItem>(
                    gameSystem,
                    "CurrentNavigationSelection"),
                Is.EqualTo(E_NavigationItem.Settings));
        }

        [UnityTest]
        public IEnumerator PauseMainMenuConfirmation_CancelPreservesAndConfirmClearsRun()
        {
            yield return LoadProductionScene();

            MonoBehaviour gameSystem = FindBehaviour("GameSystem", "GameSystem");
            MonoBehaviour runtimeDataSystem = FindBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");
            InvokeNavigationSelection(gameSystem, E_NavigationItem.Play);
            InvokeNavigationSelection(gameSystem, E_NavigationItem.Stage);
            object runtimeData = GetProperty<object>(
                runtimeDataSystem,
                "RuntimeData");

            InvokePublic(gameSystem, "PauseGame");
            InvokeNavigationSelection(gameSystem, E_NavigationItem.MainMenu);
            InvokeNavigationSelection(gameSystem, E_NavigationItem.Cancel);

            Assert.That(
                GetProperty<E_NavigationScreen>(
                    gameSystem,
                    "CurrentNavigationScreen"),
                Is.EqualTo(E_NavigationScreen.Pause));
            Assert.That(
                GetProperty<object>(runtimeDataSystem, "RuntimeData"),
                Is.SameAs(runtimeData));

            InvokeNavigationSelection(gameSystem, E_NavigationItem.MainMenu);
            InvokeNavigationSelection(gameSystem, E_NavigationItem.MainMenu);

            Assert.That(
                GetProperty<E_NavigationScreen>(
                    gameSystem,
                    "CurrentNavigationScreen"),
                Is.EqualTo(E_NavigationScreen.MainMenu));
            Assert.That(
                GetProperty<bool>(runtimeDataSystem, "HasRuntimeData"),
                Is.False);
        }

        private IEnumerator LoadProductionScene()
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(
                SceneName,
                LoadSceneMode.Single);

            while (!operation.isDone)
            {
                yield return null;
            }

            yield return null;
        }

        private MonoBehaviour FindBehaviour(string objectName, string typeName)
        {
            GameObject target = FindSceneObject(objectName);

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

        private GameObject FindSceneObject(string objectName)
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

        private void InvokeNavigationSelection(
            MonoBehaviour gameSystem,
            E_NavigationItem item)
        {
            MethodInfo method = gameSystem.GetType().GetMethod(
                "RequestNavigationSelection",
                BindingFlags.Instance | BindingFlags.Public,
                null,
                new[] { typeof(E_NavigationItem) },
                null);
            Assert.That(method, Is.Not.Null);
            method.Invoke(gameSystem, new object[] { item });
        }

        private void InvokePublic(MonoBehaviour target, string methodName)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(method, Is.Not.Null);
            method.Invoke(target, null);
        }

        private T GetProperty<T>(MonoBehaviour target, string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(property, Is.Not.Null);
            return (T)property.GetValue(target);
        }

        private T GetObjectProperty<T>(object target, string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(property, Is.Not.Null);
            return (T)property.GetValue(target);
        }
    }
}
