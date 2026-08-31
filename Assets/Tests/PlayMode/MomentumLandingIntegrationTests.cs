using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class MomentumLandingIntegrationTests
    {
        private const string SceneName = "SampleScene";
        private const int MaximumFixedSteps = 300;

        private Rigidbody _playerRigidbody;
        private MonoBehaviour _playerInputSystem;
        private GameRuntimeData _runtimeData;

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

            GameObject player = GameObject.Find("Player");
            Assert.That(player, Is.Not.Null);
            _playerRigidbody = player.GetComponent<Rigidbody>();
            Assert.That(_playerRigidbody, Is.Not.Null);

            _playerInputSystem = FindRequiredBehaviour(
                "PlayerInputSystem",
                "PlayerInputSystem");
            MonoBehaviour runtimeDataSystem = FindRequiredBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");
            _runtimeData = (GameRuntimeData)GetPropertyValue(
                runtimeDataSystem,
                "RuntimeData");
            Assert.That(_runtimeData, Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator MomentumLanding_WindowInput_AppliesMomentumLanding()
        {
            yield return ReachGroundMoveSpeed();
            TriggerJump();

            bool didObserveWindow = false;
            bool didLand = false;
            float landingHorizontalSpeed = 0.0f;

            for (int step = 0; step < MaximumFixedSteps; step++)
            {
                yield return new WaitForFixedUpdate();

                PlayerMovementRuntimeData movementData =
                    _runtimeData.PlayerMovementRuntimeData;

                if (movementData.IsMomentumLandingWindowActive)
                {
                    didObserveWindow = true;
                    TriggerMomentumLanding();
                }

                if (didObserveWindow && movementData.IsGrounded)
                {
                    didLand = true;
                    landingHorizontalSpeed =
                        movementData.CurrentHorizontalSpeed;
                    break;
                }
            }

            Assert.That(didObserveWindow, Is.True);
            Assert.That(didLand, Is.True);
            Assert.That(
                _runtimeData.PlayerMovementRuntimeData.IsLastLandingMomentum,
                Is.True);
            Assert.That(landingHorizontalSpeed, Is.GreaterThan(8.0f));
            Assert.That(landingHorizontalSpeed, Is.LessThanOrEqualTo(14.0f));
            Assert.That(_playerRigidbody.linearVelocity.x, Is.GreaterThan(0.0f));
        }

        [UnityTest]
        public IEnumerator NormalLanding_WithoutWindowInput_DoesNotApplyMomentum()
        {
            yield return ReachGroundMoveSpeed();
            TriggerJump();

            bool didLeaveGround = false;
            bool didLand = false;

            for (int step = 0; step < MaximumFixedSteps; step++)
            {
                yield return new WaitForFixedUpdate();

                PlayerMovementRuntimeData movementData =
                    _runtimeData.PlayerMovementRuntimeData;
                didLeaveGround |= !movementData.IsGrounded;

                if (didLeaveGround && movementData.IsGrounded)
                {
                    didLand = true;
                    break;
                }
            }

            Assert.That(didLand, Is.True);
            Assert.That(
                _runtimeData.PlayerMovementRuntimeData.IsLastLandingMomentum,
                Is.False);
        }

        [UnityTest]
        public IEnumerator MomentumLanding_InputBeforeWindow_IsIgnored()
        {
            yield return ReachGroundMoveSpeed();
            TriggerJump();
            TriggerMomentumLanding();

            bool didLeaveGround = false;
            bool didLand = false;

            for (int step = 0; step < MaximumFixedSteps; step++)
            {
                yield return new WaitForFixedUpdate();

                PlayerMovementRuntimeData movementData =
                    _runtimeData.PlayerMovementRuntimeData;
                didLeaveGround |= !movementData.IsGrounded;

                if (didLeaveGround && movementData.IsGrounded)
                {
                    didLand = true;
                    break;
                }
            }

            Assert.That(didLand, Is.True);
            Assert.That(
                _runtimeData.PlayerMovementRuntimeData.IsLastLandingMomentum,
                Is.False);
        }

        private IEnumerator ReachGroundMoveSpeed()
        {
            SetPrivateField(_playerInputSystem, "_moveInput", Vector2.right);

            for (int step = 0; step < 60; step++)
            {
                yield return new WaitForFixedUpdate();

                if (_playerRigidbody.linearVelocity.x >= 7.9f)
                {
                    yield break;
                }
            }

            Assert.Fail("Player did not reach the expected ground move speed.");
        }

        private void TriggerJump()
        {
            SetPrivateField(_playerInputSystem, "_isJumpPressed", true);
        }

        private void TriggerMomentumLanding()
        {
            SetPrivateField(
                _playerInputSystem,
                "_isMomentumLandingPressed",
                true);
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

        private object GetPropertyValue(
            MonoBehaviour targetBehaviour,
            string propertyName)
        {
            PropertyInfo property = targetBehaviour.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return property.GetValue(targetBehaviour);
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
