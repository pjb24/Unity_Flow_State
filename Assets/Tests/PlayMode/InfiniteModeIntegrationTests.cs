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
    public class InfiniteModeIntegrationTests
    {
        private const string SceneName = "SampleScene";
        private const float FallThresholdY = -3.0f;

        private MonoBehaviour _gameSystem;
        private MonoBehaviour _runtimeDataSystem;
        private MonoBehaviour _playerInputSystem;
        private MonoBehaviour _uiInputSystem;
        private MonoBehaviour _playerMovementSystem;
        private MonoBehaviour _stageSystem;
        private MonoBehaviour _infiniteModeSystem;
        private MonoBehaviour _resultSystem;
        private MonoBehaviour _cameraFollow;
        private StageGoal _stageGoal;
        private InfiniteMapPattern _mapPattern;
        private InfinitePatternBoundary _secondBoundary;
        private GameObject _player;
        private GameObject _startPoint;
        private GameObject _stageModeRoot;
        private GameObject _infiniteModeRoot;
        private Rigidbody _playerRigidbody;
        private Collider _playerCollider;

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
            yield return new WaitForFixedUpdate();

            _gameSystem = FindRequiredBehaviour("GameSystem", "GameSystem");
            _runtimeDataSystem = FindRequiredBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");
            _playerInputSystem = FindRequiredBehaviour(
                "PlayerInputSystem",
                "PlayerInputSystem");
            _uiInputSystem = FindRequiredBehaviour(
                "UIInputSystem",
                "UIInputSystem");
            _playerMovementSystem = FindRequiredBehaviour(
                "PlayerMovementSystem",
                "PlayerMovementSystem");
            _stageSystem = FindRequiredBehaviour(
                "StageSystem",
                "StageSystem");
            _infiniteModeSystem = FindRequiredBehaviour(
                "InfiniteModeSystem",
                "InfiniteModeSystem");
            _resultSystem = FindRequiredBehaviour(
                "ResultSystem",
                "ResultSystem");
            _cameraFollow = FindRequiredBehaviour(
                "CameraRig",
                "CameraFollow");
            _stageGoal = FindSceneGameObject("Goal")
                .GetComponent<StageGoal>();
            _mapPattern = FindSceneGameObject("InfiniteMapPattern")
                .GetComponent<InfiniteMapPattern>();
            _secondBoundary = FindSceneGameObject("Pattern_1")
                .GetComponentInChildren<InfinitePatternBoundary>(true);
            _player = FindSceneGameObject("Player");
            _startPoint = FindSceneGameObject("StartPoint");
            _stageModeRoot = FindSceneGameObject("StageModeRoot");
            _infiniteModeRoot = FindSceneGameObject("InfiniteModeRoot");
            _playerRigidbody = _player.GetComponent<Rigidbody>();
            _playerCollider = _player.GetComponent<Collider>();

            Assert.That(_stageGoal, Is.Not.Null);
            Assert.That(_mapPattern, Is.Not.Null);
            Assert.That(_secondBoundary, Is.Not.Null);
            Assert.That(_playerRigidbody, Is.Not.Null);
            Assert.That(_playerCollider, Is.Not.Null);

            InvokePublicMethod(_gameSystem, "EndGame");
            yield return null;

            SetPrivateField(
                _gameSystem,
                "_selectedGameMode",
                E_GameMode.Infinite);
            SetInfiniteTiming(100.0f, 0.5f);
            InvokePublicMethod(_gameSystem, "StartGame");
            yield return null;
            yield return new WaitForFixedUpdate();
        }

        [UnityTest]
        public IEnumerator InfiniteStart_GoalAndPattern_DoNotEndRun()
        {
            AssertInfinitePlayingState();

            InvokePrivateMethod(
                _stageGoal,
                "OnTriggerEnter",
                _playerCollider);
            InvokePrivateMethod(
                _secondBoundary,
                "OnTriggerEnter",
                _playerCollider);
            yield return null;

            AssertInfinitePlayingState();
            Assert.That(_mapPattern.AdvanceCount, Is.EqualTo(1));
            Assert.That(
                GetBoolProperty(_resultSystem, "HasResultData"),
                Is.False);
        }

        [UnityTest]
        public IEnumerator BelowMinimumSpeed_EndsWithoutStageClearOrResultData()
        {
            InvokePublicMethod(_gameSystem, "EndGame");
            yield return null;

            SetInfiniteTiming(0.0f, Time.fixedDeltaTime);
            InvokePublicMethod(_gameSystem, "StartGame");
            yield return new WaitForFixedUpdate();
            yield return null;

            AssertInfiniteEndedState();
        }

        [UnityTest]
        public IEnumerator PlayerFallsAtLargeX_EndsAndStopsPlaySystems()
        {
            _player.transform.position = new Vector3(
                10000.0f,
                FallThresholdY - 0.01f,
                0.0f);
            _playerRigidbody.linearVelocity = Vector3.zero;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            yield return null;

            AssertInfiniteEndedState();
        }

        [Test]
        public void PatternGroundConnections_ProvideJumpGap()
        {
            Transform firstPattern = FindSceneGameObject("Pattern_0").transform;
            Transform secondPattern = FindSceneGameObject("Pattern_1").transform;
            Transform firstEndAnchor = firstPattern.Find("EndAnchor");
            Transform secondStartAnchor = secondPattern.Find("StartAnchor");
            Transform firstGroundTransform = firstPattern.Find("Terrain/Ground");
            Transform secondGroundTransform = secondPattern.Find("Terrain/Ground");

            Assert.That(firstEndAnchor, Is.Not.Null);
            Assert.That(secondStartAnchor, Is.Not.Null);
            Assert.That(firstGroundTransform, Is.Not.Null);
            Assert.That(secondGroundTransform, Is.Not.Null);

            BoxCollider firstGround =
                firstGroundTransform.GetComponent<BoxCollider>();
            BoxCollider secondGround =
                secondGroundTransform.GetComponent<BoxCollider>();
            Assert.That(firstGround, Is.Not.Null);
            Assert.That(secondGround, Is.Not.Null);

            float groundGap =
                secondGround.bounds.min.x - firstGround.bounds.max.x;
            float firstAnchorClearance =
                firstEndAnchor.position.x - firstGround.bounds.max.x;
            float secondAnchorClearance =
                secondGround.bounds.min.x - secondStartAnchor.position.x;

            Assert.That(groundGap, Is.EqualTo(4.0f).Within(0.01f));
            Assert.That(
                firstAnchorClearance,
                Is.EqualTo(2.0f).Within(0.01f));
            Assert.That(
                secondAnchorClearance,
                Is.EqualTo(2.0f).Within(0.01f));
            Assert.That(
                firstEndAnchor.position,
                Is.EqualTo(secondStartAnchor.position));
        }

        [UnityTest]
        public IEnumerator ResultMenuRetry_Twice_RestoresIndependentInfiniteRuns()
        {
            Vector3 firstPatternPosition =
                FindSceneGameObject("Pattern_0").transform.position;
            Vector3 secondPatternPosition =
                FindSceneGameObject("Pattern_1").transform.position;

            for (int retryIndex = 0; retryIndex < 2; retryIndex++)
            {
                _player.transform.position = new Vector3(
                    10000.0f,
                    FallThresholdY - 0.01f,
                    0.0f);
                InvokePrivateMethod(
                    _infiniteModeSystem,
                    "ProcessFallThreshold");
                yield return null;

                AssertInfiniteEndedState();
                SetPrivateField(_uiInputSystem, "_isSubmitPressed", true);
                yield return null;
                yield return new WaitForFixedUpdate();

                AssertInfinitePlayingState();
                Assert.That(
                    GetRuntimeData().GameMode,
                    Is.EqualTo(E_GameMode.Infinite));
                Assert.That(
                    _player.transform.position,
                    Is.EqualTo(_startPoint.transform.position));
                Assert.That(
                    _playerRigidbody.linearVelocity,
                    Is.EqualTo(Vector3.zero));
                Assert.That(_mapPattern.AdvanceCount, Is.Zero);
                Assert.That(
                    FindSceneGameObject("Pattern_0").transform.position,
                    Is.EqualTo(firstPatternPosition));
                Assert.That(
                    FindSceneGameObject("Pattern_1").transform.position,
                    Is.EqualTo(secondPatternPosition));
                Assert.That(
                    GetBoolProperty(_resultSystem, "HasResultData"),
                    Is.False);
            }
        }

        private void AssertInfinitePlayingState()
        {
            Assert.That(GetStateName(_gameSystem, "CurrentGameState"), Is.EqualTo("Playing"));
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.True);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.False);
            Assert.That(GetBoolProperty(_infiniteModeSystem, "IsPlaying"), Is.True);
            Assert.That(
                GetBoolProperty(_playerInputSystem, "IsPlayerActionMapEnabled"),
                Is.True);
            Assert.That(
                GetBoolProperty(_uiInputSystem, "IsUIActionMapEnabled"),
                Is.False);
            Assert.That(
                GetBoolProperty(_playerMovementSystem, "IsRunning"),
                Is.True);
            Assert.That(GetBoolProperty(_cameraFollow, "IsFollowing"), Is.True);
            Assert.That(_stageModeRoot.activeSelf, Is.False);
            Assert.That(_infiniteModeRoot.activeSelf, Is.True);
        }

        private void AssertInfiniteEndedState()
        {
            Assert.That(GetStateName(_gameSystem, "CurrentGameState"), Is.EqualTo("Ended"));
            Assert.That(GetBoolProperty(_stageSystem, "IsPlaying"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "IsCleared"), Is.False);
            Assert.That(GetBoolProperty(_stageSystem, "HasEnded"), Is.True);
            Assert.That(
                GetBoolProperty(_playerInputSystem, "IsPlayerActionMapEnabled"),
                Is.False);
            Assert.That(
                GetBoolProperty(_uiInputSystem, "IsUIActionMapEnabled"),
                Is.True);
            Assert.That(
                GetBoolProperty(_playerMovementSystem, "IsRunning"),
                Is.False);
            Assert.That(GetBoolProperty(_cameraFollow, "IsFollowing"), Is.False);
            Assert.That(_playerRigidbody.linearVelocity, Is.EqualTo(Vector3.zero));
            Assert.That(_playerRigidbody.angularVelocity, Is.EqualTo(Vector3.zero));
            Assert.That(GetBoolProperty(_resultSystem, "HasResultData"), Is.False);
        }

        private void SetInfiniteTiming(
            float startGraceDuration,
            float belowSpeedGraceDuration)
        {
            SetPrivateField(
                _infiniteModeSystem,
                "_startGraceDuration",
                startGraceDuration);
            SetPrivateField(
                _infiniteModeSystem,
                "_belowSpeedGraceDuration",
                belowSpeedGraceDuration);
        }

        private GameRuntimeData GetRuntimeData()
        {
            return (GameRuntimeData)InvokePublicMethod(
                _runtimeDataSystem,
                "GetRuntimeData");
        }

        private GameObject FindSceneGameObject(string gameObjectName)
        {
            GameObject[] gameObjects =
                Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.name == gameObjectName &&
                    gameObject.scene.IsValid() &&
                    gameObject.scene.isLoaded)
                {
                    return gameObject;
                }
            }

            Assert.Fail($"{gameObjectName} was not found in the loaded Scene.");
            return null;
        }

        private MonoBehaviour FindRequiredBehaviour(
            string gameObjectName,
            string typeName)
        {
            GameObject targetObject = FindSceneGameObject(gameObjectName);

            foreach (MonoBehaviour behaviour in
                     targetObject.GetComponents<MonoBehaviour>())
            {
                if (behaviour != null && behaviour.GetType().Name == typeName)
                {
                    return behaviour;
                }
            }

            Assert.Fail($"{typeName} was not found on {gameObjectName}.");
            return null;
        }

        private string GetStateName(
            MonoBehaviour target,
            string propertyName)
        {
            return GetProperty(target, propertyName).ToString();
        }

        private bool GetBoolProperty(
            MonoBehaviour target,
            string propertyName)
        {
            return (bool)GetProperty(target, propertyName);
        }

        private object GetProperty(
            MonoBehaviour target,
            string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return property.GetValue(target);
        }

        private object InvokePublicMethod(
            MonoBehaviour target,
            string methodName,
            params object[] arguments)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(method, Is.Not.Null);
            return method.Invoke(target, arguments);
        }

        private void InvokePrivateMethod(
            object target,
            string methodName,
            params object[] arguments)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(method, Is.Not.Null);
            method.Invoke(target, arguments);
        }

        private void SetPrivateField(
            object target,
            string fieldName,
            object value)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }
    }
}
