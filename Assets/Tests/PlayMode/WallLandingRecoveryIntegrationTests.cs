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
    public class WallLandingRecoveryIntegrationTests
    {
        private const string SceneName = "SampleScene";
        private const int GroundLayer = 6;
        private const int MaximumFixedSteps = 400;
        private static readonly Vector3 FixtureOrigin =
            new Vector3(14000.0f, 1000.0f, 0.0f);

        private GameObject _player;
        private Rigidbody _playerRigidbody;
        private MonoBehaviour _collisionSystem;
        private MonoBehaviour _playerInputSystem;
        private MonoBehaviour _gameSystem;
        private MonoBehaviour _runtimeDataSystem;

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

            _player = GameObject.Find("Player");
            Assert.That(_player, Is.Not.Null);
            _playerRigidbody = _player.GetComponent<Rigidbody>();
            Assert.That(_playerRigidbody, Is.Not.Null);
            _collisionSystem = FindRequiredBehaviour("Player", "CollisionSystem");
            _playerInputSystem = FindRequiredBehaviour(
                "PlayerInputSystem",
                "PlayerInputSystem");
            _gameSystem = FindRequiredBehaviour("GameSystem", "GameSystem");
            _runtimeDataSystem = FindRequiredBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");

            CreateFixture();
            MovePlayerToFixture();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            Assert.That(GetMovementData().IsGrounded, Is.True);
            SetMoveInput(Vector2.right);
            SetPrivateField(_playerInputSystem, "_isJumpPressed", true);
            yield return new WaitForFixedUpdate();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            foreach (GameObject gameObject in
                     Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.name.StartsWith(
                        "WallLandingRecoveryIntegrationTests.",
                        StringComparison.Ordinal))
                {
                    UnityEngine.Object.DestroyImmediate(gameObject);
                }
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator WallExit_NormalLanding_AllowsNextJump()
        {
            int normalLandingFrames = 0;
            int momentumLandingFrames = 0;
            bool didContactWallInAir = false;
            bool didLand = false;

            for (int step = 0; step < MaximumFixedSteps; step++)
            {
                yield return new WaitForFixedUpdate();

                PlayerCollisionState collisionState = GetCollisionState();
                PlayerMovementRuntimeData movementData = GetMovementData();

                if (collisionState.WallContacts.HasWallContact)
                {
                    didContactWallInAir |= !collisionState.IsGrounded;
                }

                if (movementData.CurrentMovementState ==
                    E_PlayerMovementState.NormalLanding)
                {
                    normalLandingFrames++;
                }

                if (movementData.CurrentMovementState ==
                    E_PlayerMovementState.MomentumLanding)
                {
                    momentumLandingFrames++;
                }

                if (didContactWallInAir && movementData.IsGrounded)
                {
                    didLand = true;
                    break;
                }
            }

            Assert.That(didContactWallInAir, Is.True);
            Assert.That(didLand, Is.True);
            Assert.That(normalLandingFrames, Is.EqualTo(1));
            Assert.That(momentumLandingFrames, Is.EqualTo(0));
            Assert.That(GetMovementData().IsLastLandingMomentum, Is.False);

            yield return MoveAwayFromWall();

            SetPrivateField(_playerInputSystem, "_isJumpPressed", true);

            bool didStartNextJump = false;

            for (int step = 0; step < MaximumFixedSteps; step++)
            {
                yield return new WaitForFixedUpdate();

                if (!GetMovementData().IsGrounded &&
                    _playerRigidbody.linearVelocity.y > 0.0f)
                {
                    didStartNextJump = true;
                    break;
                }
            }

            Assert.That(didStartNextJump, Is.True);
        }

        [UnityTest]
        public IEnumerator WallExit_WindowInput_AppliesMomentumLandingOnce()
        {
            int momentumLandingFrames = 0;
            int normalLandingFrames = 0;
            bool didContactWallInAir = false;
            bool didSubmitMomentumInput = false;
            bool didLand = false;

            for (int step = 0; step < MaximumFixedSteps; step++)
            {
                yield return new WaitForFixedUpdate();

                PlayerCollisionState collisionState = GetCollisionState();
                PlayerMovementRuntimeData movementData = GetMovementData();
                didContactWallInAir |=
                    collisionState.WallContacts.HasWallContact &&
                    !collisionState.IsGrounded;

                if (!didSubmitMomentumInput &&
                    movementData.IsMomentumLandingWindowActive)
                {
                    SetPrivateField(
                        _playerInputSystem,
                        "_isMomentumLandingPressed",
                        true);
                    didSubmitMomentumInput = true;
                }

                if (movementData.CurrentMovementState ==
                    E_PlayerMovementState.MomentumLanding)
                {
                    momentumLandingFrames++;
                }

                if (movementData.CurrentMovementState ==
                    E_PlayerMovementState.NormalLanding)
                {
                    normalLandingFrames++;
                }

                if (didContactWallInAir && movementData.IsGrounded)
                {
                    didLand = true;
                    break;
                }
            }

            Assert.That(didContactWallInAir, Is.True);
            Assert.That(didSubmitMomentumInput, Is.True);
            Assert.That(didLand, Is.True);
            Assert.That(momentumLandingFrames, Is.EqualTo(1));
            Assert.That(normalLandingFrames, Is.EqualTo(0));
            Assert.That(GetMovementData().IsLastLandingMomentum, Is.True);
        }

        [UnityTest]
        public IEnumerator WallContact_PausedRetry_ClearsPreviousContactAndLandingState()
        {
            bool didContactWallInAir = false;
            bool didSubmitMomentumInput = false;
            bool didLand = false;

            for (int step = 0; step < MaximumFixedSteps; step++)
            {
                yield return new WaitForFixedUpdate();

                PlayerCollisionState collisionState = GetCollisionState();
                PlayerMovementRuntimeData movementData = GetMovementData();

                if (collisionState.WallContacts.HasWallContact &&
                    !collisionState.IsGrounded)
                {
                    didContactWallInAir = true;
                }

                if (!didSubmitMomentumInput &&
                    movementData.IsMomentumLandingWindowActive)
                {
                    SetPrivateField(
                        _playerInputSystem,
                        "_isMomentumLandingPressed",
                        true);
                    didSubmitMomentumInput = true;
                }

                if (didContactWallInAir && movementData.IsGrounded)
                {
                    didLand = true;
                    break;
                }
            }

            Assert.That(didContactWallInAir, Is.True);
            Assert.That(didSubmitMomentumInput, Is.True);
            Assert.That(didLand, Is.True);
            Assert.That(GetMovementData().IsLastLandingMomentum, Is.True);
            GameRuntimeData previousRuntimeData = GetRuntimeData();
            Assert.That(InvokeBool(_gameSystem, "PauseGame"), Is.True);
            Assert.That(InvokeBool(_gameSystem, "RetryGame"), Is.True);

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            Assert.That(GetRuntimeData(), Is.Not.SameAs(previousRuntimeData));
            Assert.That(
                GetCollisionState().WallContacts.HasWallContact,
                Is.False);
            Assert.That(GetMovementData().IsLastLandingMomentum, Is.False);
        }

        [UnityTest]
        public IEnumerator InfiniteMode_WallContact_FallsAndRestoresGroundedState()
        {
            PrepareInfiniteRunAtFixture();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            Assert.That(GetMovementData().IsGrounded, Is.True);

            SetMoveInput(Vector2.right);
            SetPrivateField(_playerInputSystem, "_isJumpPressed", true);

            bool didContactWallInAir = false;
            bool didFallDuringWallContact = false;
            bool didLand = false;
            float previousHeight = _playerRigidbody.position.y;

            for (int step = 0; step < MaximumFixedSteps; step++)
            {
                yield return new WaitForFixedUpdate();

                PlayerCollisionState collisionState = GetCollisionState();
                PlayerMovementRuntimeData movementData = GetMovementData();
                float currentHeight = _playerRigidbody.position.y;

                if (collisionState.WallContacts.HasWallContact &&
                    !collisionState.IsGrounded)
                {
                    didContactWallInAir = true;
                    didFallDuringWallContact |= currentHeight < previousHeight;
                }

                if (didContactWallInAir && movementData.IsGrounded)
                {
                    didLand = true;
                    break;
                }

                previousHeight = currentHeight;
            }

            Assert.That(didContactWallInAir, Is.True);
            Assert.That(didFallDuringWallContact, Is.True);
            Assert.That(didLand, Is.True);
            Assert.That(GetRuntimeData().GameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(GetGameState(), Is.EqualTo(E_GameState.Playing));
        }

        [UnityTest]
        public IEnumerator InfiniteMode_WallContact_PreservesMaximumDistanceAndFallEnd()
        {
            PrepareInfiniteRunAtFixture();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            SetMoveInput(Vector2.right);
            SetPrivateField(_playerInputSystem, "_isJumpPressed", true);

            bool didContactWallInAir = false;

            for (int step = 0; step < MaximumFixedSteps; step++)
            {
                yield return new WaitForFixedUpdate();

                PlayerCollisionState collisionState = GetCollisionState();

                if (collisionState.WallContacts.HasWallContact &&
                    !collisionState.IsGrounded)
                {
                    didContactWallInAir = true;
                    break;
                }
            }

            Assert.That(didContactWallInAir, Is.True);
            InfiniteModeRuntimeData infiniteData =
                GetRuntimeData().InfiniteModeRuntimeData;
            float maximumDistance = infiniteData.CurrentDistance;
            int maximumScore = infiniteData.CurrentScore;
            Assert.That(maximumDistance, Is.GreaterThan(0.0f));

            SetMoveInput(Vector2.left);
            _playerRigidbody.position += Vector3.left * 2.0f;
            _playerRigidbody.linearVelocity = Vector3.zero;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            Assert.That(infiniteData.CurrentDistance, Is.EqualTo(maximumDistance));
            Assert.That(infiniteData.CurrentScore, Is.EqualTo(maximumScore));

            Vector3 fallPosition = _playerRigidbody.position;
            fallPosition.y = -3.01f;
            _playerRigidbody.position = fallPosition;
            _playerRigidbody.linearVelocity = Vector3.zero;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.That(GetGameState(), Is.EqualTo(E_GameState.Ended));
        }

        private IEnumerator MoveAwayFromWall()
        {
            SetMoveInput(Vector2.left);

            for (int step = 0; step < MaximumFixedSteps; step++)
            {
                yield return new WaitForFixedUpdate();

                if (!GetCollisionState().WallContacts.HasWallContact)
                {
                    yield break;
                }
            }

            Assert.Fail("Player did not leave the Wall contact.");
        }

        private void PrepareInfiniteRunAtFixture()
        {
            MonoBehaviour infiniteModeSystem = FindRequiredBehaviour(
                "InfiniteModeSystem",
                "InfiniteModeSystem");
            SetPrivateField(infiniteModeSystem, "_minimumHorizontalSpeed", 0.0f);
            SetPrivateField(infiniteModeSystem, "_startGraceDuration", 100.0f);
            SetPrivateField(
                infiniteModeSystem,
                "_belowSpeedGraceDuration",
                100.0f);
            ProductionSceneGameModeTestUtility.RestartInMode(
                E_GameMode.Infinite);
            MovePlayerToFixture();
        }

        private void CreateFixture()
        {
            CreateBox(
                "Ground",
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(20.0f, 1.0f, 4.0f));
            CreateBox(
                "RightWall",
                new Vector3(1.1f, 4.0f, 0.0f),
                new Vector3(1.0f, 6.0f, 4.0f));
            Physics.SyncTransforms();
        }

        private void CreateBox(string suffix, Vector3 position, Vector3 size)
        {
            GameObject boxObject = new GameObject(
                $"WallLandingRecoveryIntegrationTests.{suffix}");
            boxObject.layer = GroundLayer;
            boxObject.transform.position = FixtureOrigin + position;
            BoxCollider boxCollider = boxObject.AddComponent<BoxCollider>();
            boxCollider.size = size;
        }

        private void MovePlayerToFixture()
        {
            _playerRigidbody.position =
                FixtureOrigin + new Vector3(0.0f, 1.5f, 0.0f);
            _playerRigidbody.linearVelocity = Vector3.zero;
            _playerRigidbody.angularVelocity = Vector3.zero;
            Physics.SyncTransforms();
            Invoke(_collisionSystem, "RefreshCollisionState");
        }

        private void SetMoveInput(Vector2 moveInput)
        {
            SetPrivateField(_playerInputSystem, "_moveInput", moveInput);
        }

        private PlayerCollisionState GetCollisionState()
        {
            Invoke(_collisionSystem, "RefreshCollisionState");
            return (PlayerCollisionState)Invoke(
                _collisionSystem,
                "GetCollisionState");
        }

        private PlayerMovementRuntimeData GetMovementData()
        {
            return GetRuntimeData().PlayerMovementRuntimeData;
        }

        private GameRuntimeData GetRuntimeData()
        {
            return (GameRuntimeData)GetProperty(
                _runtimeDataSystem,
                "RuntimeData");
        }

        private E_GameState GetGameState()
        {
            return (E_GameState)GetProperty(_gameSystem, "CurrentGameState");
        }

        private MonoBehaviour FindRequiredBehaviour(
            string gameObjectName,
            string typeName)
        {
            GameObject gameObject = GameObject.Find(gameObjectName);
            Assert.That(gameObject, Is.Not.Null);

            foreach (MonoBehaviour behaviour in
                     gameObject.GetComponents<MonoBehaviour>())
            {
                if (behaviour != null && behaviour.GetType().Name == typeName)
                {
                    return behaviour;
                }
            }

            Assert.Fail($"{typeName} was not found on {gameObjectName}.");
            return null;
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

        private object GetProperty(MonoBehaviour target, string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return property.GetValue(target);
        }

        private void SetPrivateField(
            MonoBehaviour target,
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
