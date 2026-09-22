using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace FlowState.Tests.PlayMode
{
    public class MomentumLandingIntegrationTests
    {
        private const string SceneName = "SampleScene";
        private const int MaximumFixedSteps = 300;

        private Rigidbody _playerRigidbody;
        private MonoBehaviour _playerInputSystem;
        private MonoBehaviour _playerMovementSystem;
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
            _playerMovementSystem = FindRequiredBehaviour(
                "PlayerMovementSystem",
                "PlayerMovementSystem");
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
            Assert.That(
                landingHorizontalSpeed,
                Is.EqualTo(GetPlayerMoveSpeed()).Within(0.001f));
            Assert.That(_runtimeData.PlayerMovementRuntimeData.MomentumLandingSuccessId,
                Is.EqualTo(1));
            Assert.That(_playerRigidbody.linearVelocity.x, Is.GreaterThan(0.0f));

            yield return new WaitForFixedUpdate();
            Assert.That(_runtimeData.PlayerMovementRuntimeData.CurrentHorizontalSpeed,
                Is.EqualTo(landingHorizontalSpeed).Within(0.001f));
            Assert.That(_runtimeData.PlayerMovementRuntimeData.MomentumLandingSuccessId,
                Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator InfiniteMomentumLanding_PreservesSpeedAndUpdatesScoreAndHud()
        {
            ProductionSceneGameModeTestUtility.RestartInMode(E_GameMode.Infinite);
            MonoBehaviour runtimeDataSystem = FindRequiredBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");
            _runtimeData = (GameRuntimeData)GetPropertyValue(
                runtimeDataSystem,
                "RuntimeData");
            yield return new WaitForFixedUpdate();
            yield return ReachGroundMoveSpeed();

            TriggerJump();
            bool didObserveWindow = false;
            bool didLand = false;

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
                    break;
                }
            }

            Assert.That(didLand, Is.True);
            Assert.That(_runtimeData.PlayerMovementRuntimeData.CurrentHorizontalSpeed,
                Is.EqualTo(GetPlayerMoveSpeed()).Within(0.001f));

            for (int step = 0; step < 5; step++)
            {
                yield return new WaitForFixedUpdate();
            }

            // UIManagementSystem updates HUD text during Update, after the
            // score-producing FixedUpdate steps above.
            yield return null;

            InfiniteModeRuntimeData infiniteData =
                _runtimeData.InfiniteModeRuntimeData;
            Assert.That(infiniteData.ScoringVersion,
                Is.EqualTo(ScoringVersion.Current));
            Assert.That(infiniteData.CurrentMomentumMultiplier,
                Is.GreaterThan(1.0));
            Assert.That(infiniteData.MaximumMomentumMultiplier,
                Is.GreaterThan(1.0));
            Assert.That(infiniteData.MomentumBonus, Is.GreaterThan(0));
            Assert.That(infiniteData.CurrentScore,
                Is.EqualTo(infiniteData.BaseDistanceScore + infiniteData.MomentumBonus));

            TMP_Text multiplierText = FindRequiredSceneObject(
                "MomentumMultiplierText").GetComponent<TMP_Text>();
            TMP_Text baseDistanceScoreText = FindRequiredSceneObject(
                "BaseDistanceScoreText").GetComponent<TMP_Text>();
            TMP_Text momentumBonusText = FindRequiredSceneObject(
                "MomentumBonusText").GetComponent<TMP_Text>();
            Image durationFill = FindRequiredSceneObject(
                "MomentumDurationFill").GetComponent<Image>();
            Assert.That(multiplierText, Is.Not.Null);
            Assert.That(baseDistanceScoreText, Is.Not.Null);
            Assert.That(momentumBonusText, Is.Not.Null);
            Assert.That(durationFill, Is.Not.Null);
            Assert.That(multiplierText.text,
                Is.EqualTo(
                    MomentumHudPresenter.Create(
                        infiniteData.CurrentMomentumMultiplier,
                        infiniteData.MomentumRemainingRatio).MultiplierText));
            Assert.That(baseDistanceScoreText.text,
                Is.EqualTo(
                    ResultTextFormatter.FormatBaseDistanceScore(
                        infiniteData.BaseDistanceScore)));
            Assert.That(momentumBonusText.text,
                Is.EqualTo(
                    ResultTextFormatter.FormatMomentumBonus(
                        infiniteData.MomentumBonus)));
            Assert.That(durationFill.fillAmount, Is.GreaterThan(0.0f));
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
            yield return new WaitForFixedUpdate();
            Assert.That(_runtimeData.PlayerMovementRuntimeData.CurrentHorizontalSpeed,
                Is.EqualTo(GetPlayerMoveSpeed()).Within(0.001f));
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
            for (int step = 0; step < 60; step++)
            {
                yield return new WaitForFixedUpdate();

                if (_playerRigidbody.linearVelocity.x >=
                    GetPlayerMoveSpeed() - 0.1f)
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

        private float GetPlayerMoveSpeed()
        {
            FieldInfo field = _playerMovementSystem.GetType().GetField(
                "_moveSpeed",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            return (float)field.GetValue(_playerMovementSystem);
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

        private GameObject FindRequiredSceneObject(string gameObjectName)
        {
            foreach (GameObject gameObject in
                     Resources.FindObjectsOfTypeAll<GameObject>())
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
