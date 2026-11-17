using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlowState.Tests.PlayMode
{
    internal static class ProductionSceneGameModeTestUtility
    {
        private const string GameSystemObjectName = "GameSystem";

        // Component physics fixtures use their own runway, not Infinite terrain.
        // Disable only progression/rebase; retain real Infinite data and movement.
        public static void DisableInfiniteProgressionForPhysicsFixture()
        {
            GameObject owner = GameObject.Find("InfiniteModeSystem");
            Assert.That(owner, Is.Not.Null);
            foreach (MonoBehaviour behaviour in owner.GetComponents<MonoBehaviour>())
            {
                if (behaviour != null && behaviour.GetType().Name == "InfiniteModeSystem")
                {
                    behaviour.enabled = false;
                    return;
                }
            }
            Assert.Fail("InfiniteModeSystem was not found.");
        }

        public static GameObject FindSceneObject(string name)
        {
            foreach (GameObject candidate in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (candidate.name == name && candidate.scene.name == "SampleScene" &&
                    candidate.scene.isLoaded)
                {
                    return candidate;
                }
            }
            Assert.Fail($"{name} was not found in SampleScene, including inactive objects.");
            return null;
        }

        public static IEnumerator UnloadProductionScene()
        {
            Time.timeScale = 1.0f;
            Scene scene = SceneManager.GetSceneByName("SampleScene");
            if (!scene.isLoaded)
            {
                yield break;
            }
            Scene cleanup = SceneManager.GetSceneByName("GameEntryTestCleanup");
            if (!cleanup.isLoaded)
            {
                cleanup = SceneManager.CreateScene("GameEntryTestCleanup");
            }
            SceneManager.SetActiveScene(cleanup);
            yield return SceneManager.UnloadSceneAsync(scene);
        }

        public static void AssertNoProductionRuntime()
        {
            MonoBehaviour game = FindGameSystem(FindSceneObject(GameSystemObjectName));
            Assert.That(game, Is.Not.Null);
            FieldInfo runtime = game.GetType().GetField("_runtimeData",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(runtime, Is.Not.Null);
            Assert.That(runtime.GetValue(game), Is.Null,
                "Independent deprecated component tests must not start a production Run.");
        }

        public static void RestartInMode(E_GameMode gameMode)
        {
            // Reject before ending a Run or changing selection. Retired Stage fixtures
            // must become independent tests, not silently start Infinite instead.
            Assert.That(gameMode, Is.EqualTo(E_GameMode.Infinite),
                "Production play fixtures only support Infinite Mode.");

            GameObject gameSystemObject = GameObject.Find(GameSystemObjectName);
            Assert.That(
                gameSystemObject,
                Is.Not.Null,
                "GameSystem was not found in the loaded Scene.");

            MonoBehaviour gameSystem = FindGameSystem(gameSystemObject);
            Assert.That(
                gameSystem,
                Is.Not.Null,
                "GameSystem component was not found in the loaded Scene.");

            PropertyInfo currentGameStateProperty = gameSystem.GetType()
                .GetProperty(
                    "CurrentGameState",
                    BindingFlags.Instance | BindingFlags.Public);
            Assert.That(currentGameStateProperty, Is.Not.Null);

            E_GameState currentGameState = (E_GameState)
                currentGameStateProperty.GetValue(gameSystem);
            if (currentGameState == E_GameState.Playing ||
                currentGameState == E_GameState.Paused)
            {
                InvokePublicMethod(gameSystem, "EndGame");
            }

            E_NavigationScreen screen = (E_NavigationScreen)GetProperty(
                gameSystem,
                "CurrentNavigationScreen");

            if (screen == E_NavigationScreen.Result)
            {
                InvokeNavigationSelection(
                    gameSystem,
                    E_NavigationItem.MainMenu);
            }

            InvokeNavigationSelection(gameSystem, E_NavigationItem.Play);

            // The first Play with an incomplete Tutorial opens the automatic
            // How To Play screen. Production tests intentionally use Start Run here
            // so their shared setup represents the same completed entry flow as play.
            screen = (E_NavigationScreen)GetProperty(
                gameSystem,
                "CurrentNavigationScreen");
            if (screen == E_NavigationScreen.AutomaticHowToPlay)
            {
                InvokeNavigationSelection(
                    gameSystem,
                    E_NavigationItem.StartRun);
            }

            Assert.That(
                (E_GameState)currentGameStateProperty.GetValue(gameSystem),
                Is.EqualTo(E_GameState.Playing),
                $"GameSystem did not start in {gameMode} mode.");
            FieldInfo runtimeField = gameSystem.GetType().GetField("_runtimeData",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(runtimeField, Is.Not.Null);
            GameRuntimeData runtime = (GameRuntimeData)runtimeField.GetValue(gameSystem);
            Assert.That(runtime, Is.Not.Null);
            Assert.That(runtime.GameMode, Is.EqualTo(E_GameMode.Infinite));
        }

        private static MonoBehaviour FindGameSystem(GameObject gameSystemObject)
        {
            MonoBehaviour[] behaviours =
                gameSystemObject.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour.GetType().Name == "GameSystem")
                {
                    return behaviour;
                }
            }

            return null;
        }

        private static void InvokePublicMethod(
            MonoBehaviour target,
            string methodName)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public,
                null,
                System.Type.EmptyTypes,
                null);
            Assert.That(method, Is.Not.Null);
            method.Invoke(target, null);
        }

        private static void InvokeNavigationSelection(
            MonoBehaviour target,
            E_NavigationItem item)
        {
            MethodInfo method = target.GetType().GetMethod(
                "RequestNavigationSelection",
                BindingFlags.Instance | BindingFlags.Public,
                null,
                new[] { typeof(E_NavigationItem) },
                null);
            Assert.That(method, Is.Not.Null);
            method.Invoke(target, new object[] { item });
        }

        private static object GetProperty(
            MonoBehaviour target,
            string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(property, Is.Not.Null);
            return property.GetValue(target);
        }
    }
}
