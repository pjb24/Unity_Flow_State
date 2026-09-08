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
    public class CollectibleLifecycleIntegrationTests
    {
        private const string SceneName = "SampleScene";
        private const string CollectibleName = "StageCollectible_Jump01_03";
        private const int StageCollectibleCount = 10;
        private const int InfiniteCollectibleCount = 20;

        private MonoBehaviour _gameSystem;
        private MonoBehaviour _runtimeDataSystem;
        private ScoreCollectible _collectible;
        private Rigidbody _player;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(
                SceneName,
                LoadSceneMode.Single);

            while (!loadOperation.isDone)
            {
                yield return null;
            }

            yield return null;
            ProductionSceneGameModeTestUtility.RestartInMode(E_GameMode.Stage);
            yield return null;

            _gameSystem = FindRequiredBehaviour("GameSystem", "GameSystem");
            _runtimeDataSystem = FindRequiredBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");
            _collectible = FindRequiredComponent<ScoreCollectible>(
                CollectibleName);
            _player = FindRequiredComponent<Rigidbody>("Player");
        }

        [Test]
        public void StartGame_BindsActiveModeCollectible()
        {
            Assert.That(_collectible.IsBound, Is.True);
            Assert.That(_collectible.IsCollected, Is.False);
            Assert.That(CurrentCollectibleData.IsInitialized, Is.True);
            Assert.That(
                CurrentCollectibleData.RegisteredCount,
                Is.EqualTo(StageCollectibleCount));
        }

        [UnityTest]
        public IEnumerator PauseOverlap_Resume_CollectsInSameRunOnce()
        {
            CollectibleRuntimeData collectibleData = CurrentCollectibleData;
            int scoreBeforeTarget = collectibleData.CurrentScore;
            Assert.That(_collectible.IsCollected, Is.False);
            Assert.That(InvokeBool(_gameSystem, "PauseGame"), Is.True);
            _player.position = _collectible.transform.position;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();

            Assert.That(_collectible.TryCollectOverlappingPlayer(), Is.False);
            Assert.That(
                collectibleData.CurrentScore,
                Is.EqualTo(scoreBeforeTarget));

            Assert.That(InvokeBool(_gameSystem, "ResumeGame"), Is.True);

            Assert.That(CurrentCollectibleData, Is.SameAs(collectibleData));
            Assert.That(
                collectibleData.CurrentScore,
                Is.EqualTo(scoreBeforeTarget + 10));
            Assert.That(_collectible.IsCollected, Is.True);
            Assert.That(_collectible.TryCollectOverlappingPlayer(), Is.False);
        }

        [Test]
        public void EndGame_UnbindsBeforeRuntimeIsCleared()
        {
            CollectibleRuntimeData previousData = CurrentCollectibleData;

            Invoke(_gameSystem, "EndGame");

            Assert.That(_collectible.IsBound, Is.False);
            Assert.That(_collectible.TryCollectOverlappingPlayer(), Is.False);
            Assert.That(previousData.IsInitialized, Is.False);
            Assert.That(
                GetProperty<bool>(_runtimeDataSystem, "HasRuntimeData"),
                Is.False);
        }

        [UnityTest]
        public IEnumerator Retry_CreatesIndependentCollectibleRun()
        {
            CollectibleRuntimeData previousData = CurrentCollectibleData;
            int scoreBeforeTarget = previousData.CurrentScore;
            Assert.That(_collectible.IsCollected, Is.False);
            Assert.That(InvokeBool(_gameSystem, "PauseGame"), Is.True);
            _player.position = _collectible.transform.position;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            Assert.That(InvokeBool(_gameSystem, "ResumeGame"), Is.True);
            Assert.That(
                previousData.CurrentScore,
                Is.EqualTo(scoreBeforeTarget + 10));

            Invoke(_gameSystem, "EndGame");
            Assert.That(InvokeBool(_gameSystem, "RetryGame"), Is.True);

            Assert.That(CurrentCollectibleData, Is.Not.SameAs(previousData));
            Assert.That(CurrentCollectibleData.CurrentScore, Is.Zero);
            Assert.That(
                CurrentCollectibleData.RegisteredCount,
                Is.EqualTo(StageCollectibleCount));
            Assert.That(_collectible.IsBound, Is.True);
            Assert.That(_collectible.IsCollected, Is.False);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ModeSwitch_CreatesEmptyIndependentInfiniteRun()
        {
            CollectibleRuntimeData stageData = CurrentCollectibleData;

            ProductionSceneGameModeTestUtility.RestartInMode(
                E_GameMode.Infinite);
            yield return null;

            CollectibleRuntimeData infiniteData = CurrentCollectibleData;
            Assert.That(infiniteData, Is.Not.SameAs(stageData));
            Assert.That(infiniteData.IsInitialized, Is.True);
            Assert.That(infiniteData.CurrentScore, Is.Zero);
            Assert.That(
                infiniteData.RegisteredCount,
                Is.EqualTo(InfiniteCollectibleCount));
            Assert.That(_collectible.IsBound, Is.False);
            Assert.That(_collectible.gameObject.activeInHierarchy, Is.False);
        }

        private CollectibleRuntimeData CurrentCollectibleData =>
            GetProperty<GameRuntimeData>(_runtimeDataSystem, "RuntimeData")
                .CollectibleRuntimeData;

        private MonoBehaviour FindRequiredBehaviour(
            string objectName,
            string typeName)
        {
            GameObject owner = GameObject.Find(objectName);
            Assert.That(owner, Is.Not.Null);

            foreach (MonoBehaviour behaviour in owner.GetComponents<MonoBehaviour>())
            {
                if (behaviour != null && behaviour.GetType().Name == typeName)
                {
                    return behaviour;
                }
            }

            Assert.Fail($"{typeName} was not found on {objectName}.");
            return null;
        }

        private T GetProperty<T>(MonoBehaviour target, string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(property, Is.Not.Null);
            return (T)property.GetValue(target);
        }

        private bool InvokeBool(MonoBehaviour target, string methodName)
        {
            return (bool)Invoke(target, methodName);
        }

        private object Invoke(MonoBehaviour target, string methodName)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(method, Is.Not.Null);
            return method.Invoke(target, null);
        }

        private T FindRequiredComponent<T>(string objectName)
            where T : Component
        {
            GameObject owner = GameObject.Find(objectName);
            Assert.That(owner, Is.Not.Null);
            T component = owner.GetComponent<T>();
            Assert.That(component, Is.Not.Null);
            return component;
        }
    }
}
