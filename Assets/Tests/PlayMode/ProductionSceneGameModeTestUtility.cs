using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.PlayMode
{
    internal static class ProductionSceneGameModeTestUtility
    {
        private const string GameSystemObjectName = "GameSystem";

        public static void RestartInMode(E_GameMode gameMode)
        {
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
            InvokeNavigationSelection(
                gameSystem,
                gameMode == E_GameMode.Infinite
                    ? E_NavigationItem.Infinite
                    : E_NavigationItem.Stage);

            // The first mode choice in an application session opens the automatic
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
