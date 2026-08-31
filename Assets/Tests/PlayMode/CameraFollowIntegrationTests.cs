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
        }

        [UnityTest]
        public IEnumerator HorizontalMovement_TargetAndCameraFollowPlayerX()
        {
            float fixedTargetY = _followTarget.position.y;
            float fixedTargetZ = _followTarget.position.z;
            SetPrivateField(_playerInputSystem, "_moveInput", Vector2.right);

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
    }
}
