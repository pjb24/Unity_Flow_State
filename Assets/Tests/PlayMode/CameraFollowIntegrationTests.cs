using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class CameraFollowIntegrationTests
    {
        private const string SceneName = "SampleScene";
        private const float PositionTolerance = 0.05f;

        private GameObject _player;
        private Transform _followTarget;
        private Camera _mainCamera;
        private MonoBehaviour _playerInputSystem;
        private MonoBehaviour _gameSystem;
        private MonoBehaviour _cameraFollow;

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
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();

            _player = GameObject.Find("Player");
            Assert.That(_player, Is.Not.Null);

            GameObject followTargetObject =
                GameObject.Find("CameraFollowTarget");
            Assert.That(followTargetObject, Is.Not.Null);
            _followTarget = followTargetObject.transform;

            _mainCamera = Camera.main;
            Assert.That(_mainCamera, Is.Not.Null);

            _playerInputSystem = FindRequiredBehaviour(
                "PlayerInputSystem",
                "PlayerInputSystem");
            _gameSystem = FindRequiredBehaviour("GameSystem", "GameSystem");
            _cameraFollow = FindRequiredBehaviour("CameraRig", "CameraFollow");
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Time.timeScale = 1.0f;
            yield return null;
        }

        [UnityTest]
        public IEnumerator HorizontalMovement_TargetAndCameraFollowPlayerX()
        {
            float fixedTargetY = _followTarget.position.y;
            float fixedTargetZ = _followTarget.position.z;

            for (int step = 0; step < 20; step++)
            {
                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForEndOfFrame();
            yield return null;

            Assert.That(_player.transform.position.x, Is.GreaterThan(0.5f));
            Assert.That(
                _followTarget.position.x,
                Is.EqualTo(_player.transform.position.x)
                    .Within(PositionTolerance));
            Assert.That(
                _followTarget.position.y,
                Is.EqualTo(fixedTargetY).Within(PositionTolerance));
            Assert.That(
                _followTarget.position.z,
                Is.EqualTo(fixedTargetZ).Within(PositionTolerance));
            Assert.That(
                _mainCamera.transform.position.x,
                Is.EqualTo(_followTarget.position.x)
                    .Within(PositionTolerance));
        }

        [UnityTest]
        public IEnumerator Jump_TargetAndCameraPreserveVerticalReference()
        {
            float playerStartY = _player.transform.position.y;
            float targetStartY = _followTarget.position.y;
            float targetStartZ = _followTarget.position.z;
            float cameraStartY = _mainCamera.transform.position.y;
            float cameraStartZ = _mainCamera.transform.position.z;
            SetPrivateField(_playerInputSystem, "_isJumpPressed", true);

            bool didRise = false;

            for (int step = 0; step < 100; step++)
            {
                yield return new WaitForFixedUpdate();

                if (_player.transform.position.y > playerStartY + 0.5f)
                {
                    didRise = true;
                    break;
                }
            }

            yield return new WaitForEndOfFrame();
            yield return null;

            Assert.That(didRise, Is.True);
            Assert.That(
                _followTarget.position.y,
                Is.EqualTo(targetStartY).Within(PositionTolerance));
            Assert.That(
                _followTarget.position.z,
                Is.EqualTo(targetStartZ).Within(PositionTolerance));
            Assert.That(
                _mainCamera.transform.position.y,
                Is.EqualTo(cameraStartY).Within(PositionTolerance));
            Assert.That(
                _mainCamera.transform.position.z,
                Is.EqualTo(cameraStartZ).Within(PositionTolerance));
        }

        [UnityTest]
        public IEnumerator CameraProjection_RemainsOrthographicWithExpectedSize()
        {
            yield return new WaitForEndOfFrame();

            Assert.That(_mainCamera.orthographic, Is.True);
            Assert.That(_mainCamera.orthographicSize, Is.EqualTo(5.0f));
        }

        [UnityTest]
        public IEnumerator PauseResumeRetry_PreservesAndRestoresCameraFollow()
        {
            for (int step = 0; step < 12; step++)
            {
                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForEndOfFrame();
            Assert.That(GetBoolProperty(_cameraFollow, "IsFollowing"), Is.True);
            Vector3 pausedPlayerPosition = _player.transform.position;
            Vector3 pausedTargetPosition = _followTarget.position;
            Assert.That(InvokeBool(_gameSystem, "PauseGame"), Is.True);

            for (int step = 0; step < 5; step++)
            {
                yield return new WaitForFixedUpdate();
                yield return new WaitForEndOfFrame();
                Assert.That(
                    Vector3.Distance(
                        _player.transform.position,
                        pausedPlayerPosition),
                    Is.LessThanOrEqualTo(PositionTolerance));
                Assert.That(
                    Vector3.Distance(_followTarget.position, pausedTargetPosition),
                    Is.LessThanOrEqualTo(PositionTolerance));
                Assert.That(GetBoolProperty(_cameraFollow, "IsFollowing"), Is.True);
            }

            Assert.That(InvokeBool(_gameSystem, "ResumeGame"), Is.True);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            Assert.That(_player.transform.position.x,
                Is.GreaterThan(pausedPlayerPosition.x));
            Assert.That(_followTarget.position.x,
                Is.EqualTo(_player.transform.position.x).Within(PositionTolerance));

            Assert.That(InvokeBool(_gameSystem, "PauseGame"), Is.True);
            Assert.That(InvokeBool(_gameSystem, "RetryGame"), Is.True);
            Assert.That(GetBoolProperty(_cameraFollow, "IsFollowing"), Is.True);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            Assert.That(_followTarget.position.x,
                Is.EqualTo(_player.transform.position.x).Within(PositionTolerance));
        }

        private MonoBehaviour FindRequiredBehaviour(
            string gameObjectName,
            string typeName)
        {
            GameObject targetObject = GameObject.Find(gameObjectName);
            Assert.That(targetObject, Is.Not.Null);

            foreach (MonoBehaviour behaviour in
                     targetObject.GetComponents<MonoBehaviour>())
            {
                if (behaviour != null && behaviour.GetType().Name == typeName)
                {
                    return behaviour;
                }
            }

            Assert.Fail(
                $"{typeName} was not found on {gameObjectName}.");
            return null;
        }

        private void SetPrivateField(
            MonoBehaviour targetBehaviour,
            string fieldName,
            object value)
        {
            FieldInfo field = targetBehaviour.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null);
            field.SetValue(targetBehaviour, value);
        }

        private bool InvokeBool(MonoBehaviour target, string methodName)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(method, Is.Not.Null);
            return (bool)method.Invoke(target, null);
        }

        private bool GetBoolProperty(MonoBehaviour target, string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(property, Is.Not.Null);
            return (bool)property.GetValue(target);
        }
    }
}
