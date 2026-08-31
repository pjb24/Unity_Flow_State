using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;

namespace FlowState.Tests.PlayMode
{
    internal static class ProductionSceneGameModeTestUtility
    {
        private const string GameSystemObjectName = "GameSystem";
        private const string SelectedGameModeFieldName = "_selectedGameMode";

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
            if (currentGameState != E_GameState.Ended)
            {
                InvokePublicMethod(gameSystem, "EndGame");
            }

            FieldInfo selectedGameModeField = gameSystem.GetType().GetField(
                SelectedGameModeFieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(
                selectedGameModeField,
                Is.Not.Null,
                $"{SelectedGameModeFieldName} was not found on GameSystem.");

            selectedGameModeField.SetValue(gameSystem, gameMode);
            InvokePublicMethod(gameSystem, "StartGame");

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
    }
}
