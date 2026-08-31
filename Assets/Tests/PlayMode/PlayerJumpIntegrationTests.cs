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
