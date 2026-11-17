using System;
using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class StageGoalIntegrationTests
    {
        private StageGoal _goal;
        private Collider _playerCollider;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Single);
            yield return null;
            ProductionSceneGameModeTestUtility.AssertNoProductionRuntime();
            _goal = ProductionSceneGameModeTestUtility.FindSceneObject("Goal").GetComponent<StageGoal>();
            _playerCollider = ProductionSceneGameModeTestUtility.FindSceneObject("Player").GetComponent<Collider>();
            Assert.That(_goal, Is.Not.Null);
            Assert.That(_playerCollider, Is.Not.Null);
            Assert.That(ProductionSceneGameModeTestUtility.FindSceneObject("StageModeRoot").activeSelf, Is.False);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return ProductionSceneGameModeTestUtility.UnloadProductionScene();
        }

        // Synthetic trigger observations exercise the preserved component only.
        // No Stage runtime, StageSystem start, clear result or production bypass.
        [Test]
        public void DeprecatedGoal_RequiresInitializeAndExactPlayer_NotifiesOnce()
        {
            int count = 0;
            Action listener = () => count++;
            _goal.AddListener(listener);
            _goal.AddListener(listener);
            ObserveTrigger(_playerCollider);
            Assert.That(count, Is.Zero);
            Assert.That(_goal.Initialize(), Is.True);
            ObserveTrigger(_goal.GetComponent<Collider>());
            Assert.That(count, Is.Zero);
            ObserveTrigger(_playerCollider);
            ObserveTrigger(_playerCollider);
            Assert.That(_goal.IsReached, Is.True);
            Assert.That(count, Is.EqualTo(1));
            _goal.RemoveListener(listener);
            ProductionSceneGameModeTestUtility.AssertNoProductionRuntime();
        }

        [Test]
        public void DeprecatedGoal_ResetAndRemoveListener_PreserveIndependentBehavior()
        {
            int count = 0;
            Action listener = () => count++;
            Assert.That(_goal.Initialize(), Is.True);
            _goal.AddListener(listener);
            ObserveTrigger(_playerCollider);
            _goal.ResetGoal();
            Assert.That(_goal.IsReached, Is.False);
            ObserveTrigger(_playerCollider);
            Assert.That(count, Is.EqualTo(2));
            _goal.RemoveListener(listener);
            _goal.ResetGoal();
            ObserveTrigger(_playerCollider);
            Assert.That(count, Is.EqualTo(2));
        }

        [UnityTest]
        public IEnumerator InfiniteRun_InitializedLegacyGoalCannotClearOrReplaceRun()
        {
            ProductionSceneGameModeTestUtility.RestartInMode(E_GameMode.Infinite);
            MonoBehaviour game = FindSystem("GameSystem");
            MonoBehaviour stage = FindSystem("StageSystem");
            FieldInfo runtimeField = game.GetType().GetField("_runtimeData",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(runtimeField, Is.Not.Null);
            GameRuntimeData runtime = (GameRuntimeData)runtimeField.GetValue(game);
            Assert.That(_goal.Initialize(), Is.True);
            ObserveTrigger(_playerCollider);
            LogAssert.Expect(LogType.Warning, "[StageSystem] Stage Mode is no longer supported. Use Infinite Mode.");
            Assert.That(stage.GetType().GetMethod("Initialize", new[] { typeof(E_GameMode) })
                .Invoke(stage, new object[] { E_GameMode.Stage }), Is.False);
            yield return null;
            Assert.That(runtimeField.GetValue(game), Is.SameAs(runtime));
            Assert.That(runtime.GameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(runtime.GameState, Is.EqualTo(E_GameState.Playing));
            Assert.That(stage.GetType().GetProperty("IsCleared").GetValue(stage), Is.False);
        }

        private void ObserveTrigger(Collider other)
        {
            MethodInfo trigger = typeof(StageGoal).GetMethod("OnTriggerEnter",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(trigger, Is.Not.Null);
            trigger.Invoke(_goal, new object[] { other });
        }

        private MonoBehaviour FindSystem(string name)
        {
            foreach (MonoBehaviour component in ProductionSceneGameModeTestUtility.FindSceneObject(name)
                .GetComponents<MonoBehaviour>())
                if (component.GetType().Name == name) return component;
            Assert.Fail(name + " component was not found.");
            return null;
        }
    }
}
