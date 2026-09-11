using System;
using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class PlayerJumpIntegrationTests
    {
        private const string SceneName = "SampleScene";
        private const float ExpectedStartHeight = 1.5f;
        private const float ExpectedJumpHeight = 3.0f;
        private const float HeightTolerance = 0.25f;
        private const float LandingTolerance = 0.03f;
        private const int MaximumFixedSteps = 300;

        private GameObject _player;
        private Rigidbody _playerRigidbody;
        private MonoBehaviour _playerInputSystem;
        private MonoBehaviour _playerMovementSystem;
        private GameObject _jumpGround;
        private GameObject _gapLandingGround;

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
            yield return new WaitForFixedUpdate();

            _player = GameObject.Find("Player");
            Assert.That(_player, Is.Not.Null);

            _playerRigidbody = _player.GetComponent<Rigidbody>();
            Assert.That(_playerRigidbody, Is.Not.Null);

            _playerInputSystem = FindRequiredBehaviour(
                "PlayerInputSystem",
                "PlayerInputSystem");
            _playerMovementSystem = FindRequiredBehaviour(
                "PlayerMovementSystem",
                "PlayerMovementSystem");

            // A level runway keeps gravity comparisons independent of stage obstacles.
            _jumpGround = new GameObject("PlayerJumpIntegrationTests.Ground");
            _jumpGround.layer = 6;
            _jumpGround.transform.position = new Vector3(20000.0f, 0.0f, 0.0f);
            _jumpGround.AddComponent<BoxCollider>().size = new Vector3(200.0f, 1.0f, 4.0f);
            _playerRigidbody.position = new Vector3(20000.0f, ExpectedStartHeight, 0.0f);
            _playerRigidbody.linearVelocity = Vector3.zero;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (_jumpGround != null)
            {
                UnityEngine.Object.DestroyImmediate(_jumpGround);
            }

            if (_gapLandingGround != null)
            {
                UnityEngine.Object.DestroyImmediate(_gapLandingGround);
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator Jump_DefaultSettings_ReachesHeightAndReturnsToStart()
        {
            Assert.That(
                _player.transform.position.y,
                Is.EqualTo(ExpectedStartHeight).Within(LandingTolerance));

            float jumpHeight = 0.0f;
            float landingOffset = float.PositiveInfinity;
            bool didLand = false;

            yield return MeasureJump((height, offset, landed) =>
            {
                jumpHeight = height;
                landingOffset = offset;
                didLand = landed;
            });

            Assert.That(didLand, Is.True);
            Assert.That(
                jumpHeight,
                Is.EqualTo(ExpectedJumpHeight).Within(HeightTolerance));
            Assert.That(
                landingOffset,
                Is.EqualTo(0.0f).Within(LandingTolerance));
        }

        [UnityTest]
        public IEnumerator Jump_GravityChanges_PreservesIntegratedJumpHeight()
        {
            float lowGravityHeight = 0.0f;
            float highGravityHeight = 0.0f;

            SetPrivateField(_playerMovementSystem, "_gravityAcceleration", 10.0f);
            yield return MeasureJump((height, landingOffset, landed) =>
            {
                Assert.That(landed, Is.True);
                Assert.That(
                    landingOffset,
                    Is.EqualTo(0.0f).Within(LandingTolerance));
                lowGravityHeight = height;
            });

            SetPrivateField(_playerMovementSystem, "_gravityAcceleration", 40.0f);
            yield return MeasureJump((height, landingOffset, landed) =>
            {
                Assert.That(landed, Is.True);
                Assert.That(
                    landingOffset,
                    Is.EqualTo(0.0f).Within(LandingTolerance));
                highGravityHeight = height;
            });

            Assert.That(
                lowGravityHeight,
                Is.EqualTo(ExpectedJumpHeight).Within(HeightTolerance));
            Assert.That(
                highGravityHeight,
                Is.EqualTo(ExpectedJumpHeight).Within(HeightTolerance));
            Assert.That(
                highGravityHeight,
                Is.EqualTo(lowGravityHeight).Within(HeightTolerance));
        }

        [UnityTest]
        public IEnumerator Jump_AirborneSecondInput_DoesNotRestartJump()
        {
            TriggerJump();
            bool isDescending = false;

            for (int step = 0; step < MaximumFixedSteps; step++)
            {
                yield return new WaitForFixedUpdate();

                if (_playerRigidbody.linearVelocity.y < -1.0f)
                {
                    isDescending = true;
                    break;
                }
            }

            Assert.That(isDescending, Is.True);

            float velocityBeforeInput = _playerRigidbody.linearVelocity.y;
            TriggerJump();
            yield return new WaitForFixedUpdate();
            float velocityAfterInput = _playerRigidbody.linearVelocity.y;

            Assert.That(velocityAfterInput, Is.LessThan(0.0f));
            Assert.That(velocityAfterInput, Is.LessThan(velocityBeforeInput));
        }

        [UnityTest]
        public IEnumerator Jump_BoundaryGapAtBaseSpeed_LandsOnNextGround()
        {
            yield return TraverseBoundaryGap(8.0f, 2.0f);
        }

        [UnityTest]
        public IEnumerator Jump_BoundaryGapAtMaximumSpeed_LandsOnNextGround()
        {
            yield return TraverseBoundaryGap(14.0f, 3.0f);
        }

        private IEnumerator TraverseBoundaryGap(
            float horizontalSpeed,
            float takeoffDistance)
        {
            const float boundaryX = 20000.0f;
            const float gapWidth = 4.0f;
            const float groundHeight = 0.5f;
            const float landingLength = 8.0f;
            const float playerRadius = 0.5f;

            UnityEngine.Object.DestroyImmediate(_jumpGround);
            _jumpGround = CreateGround(
                "PlayerJumpIntegrationTests.TakeoffGround",
                new Vector3(boundaryX - 5.0f, 0.0f, 0.0f),
                new Vector3(10.0f, 1.0f, 4.0f));
            _gapLandingGround = CreateGround(
                "PlayerJumpIntegrationTests.LandingGround",
                new Vector3(
                    boundaryX + gapWidth + landingLength * 0.5f,
                    0.0f,
                    0.0f),
                new Vector3(landingLength, 1.0f, 4.0f));
            _playerRigidbody.position = new Vector3(
                boundaryX - takeoffDistance,
                ExpectedStartHeight,
                0.0f);
            _playerRigidbody.linearVelocity = Vector3.zero;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            _playerRigidbody.position = new Vector3(
                boundaryX - takeoffDistance,
                ExpectedStartHeight,
                0.0f);
            _playerRigidbody.linearVelocity = new Vector3(
                horizontalSpeed,
                0.0f,
                0.0f);
            Physics.SyncTransforms();
            TriggerJump();

            bool hasLeftGround = false;
            bool hasLanded = false;

            for (int step = 0; step < MaximumFixedSteps; step++)
            {
                yield return new WaitForFixedUpdate();

                if (_playerRigidbody.position.y >
                    ExpectedStartHeight + 0.1f)
                {
                    hasLeftGround = true;
                }

                if (hasLeftGround &&
                    _playerRigidbody.position.x >=
                    boundaryX + gapWidth + playerRadius &&
                    Mathf.Abs(
                        _playerRigidbody.position.y -
                        ExpectedStartHeight) <= LandingTolerance &&
                    Mathf.Abs(_playerRigidbody.linearVelocity.y) <= 0.1f)
                {
                    hasLanded = true;
                    break;
                }
            }

            Assert.That(hasLeftGround, Is.True);
            Assert.That(hasLanded, Is.True);
            Assert.That(
                _playerRigidbody.position.x,
                Is.LessThanOrEqualTo(
                    boundaryX + gapWidth + landingLength - playerRadius));
            Assert.That(
                _playerRigidbody.position.y,
                Is.EqualTo(groundHeight + 1.0f).Within(LandingTolerance));
        }

        private GameObject CreateGround(
            string name,
            Vector3 position,
            Vector3 size)
        {
            GameObject ground = new GameObject(name);
            ground.layer = 6;
            ground.transform.position = position;
            ground.AddComponent<BoxCollider>().size = size;
            return ground;
        }

        private IEnumerator MeasureJump(
            Action<float, float, bool> completeMeasurement)
        {
            float startHeight = _player.transform.position.y;
            float maximumHeight = startHeight;
            bool hasLeftGround = false;
            bool hasLanded = false;

            TriggerJump();

            for (int step = 0; step < MaximumFixedSteps; step++)
            {
                yield return new WaitForFixedUpdate();

                float currentHeight = _player.transform.position.y;
                maximumHeight = Mathf.Max(maximumHeight, currentHeight);

                if (currentHeight > startHeight + 0.1f)
                {
                    hasLeftGround = true;
                }

                if (hasLeftGround &&
                    currentHeight <= startHeight + LandingTolerance &&
                    Mathf.Abs(_playerRigidbody.linearVelocity.y) <= 0.1f)
                {
                    hasLanded = true;
                    break;
                }
            }

            completeMeasurement(
                maximumHeight - startHeight,
                _player.transform.position.y - startHeight,
                hasLanded);
        }

        private void TriggerJump()
        {
            SetPrivateField(_playerInputSystem, "_isJumpPressed", true);
        }

        private MonoBehaviour FindRequiredBehaviour(
            string gameObjectName,
            string typeName)
        {
            GameObject targetObject = GameObject.Find(gameObjectName);
            Assert.That(targetObject, Is.Not.Null);

            MonoBehaviour[] behaviours =
                targetObject.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour behaviour in behaviours)
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

            Assert.That(
                field,
                Is.Not.Null,
                $"{fieldName} was not found on {targetBehaviour.GetType().Name}.");
            field.SetValue(targetBehaviour, value);
        }
    }
}
