using System;
using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class WallFallPhysicsTests
    {
        private const int GroundLayer = 6;
        private const int MaximumFixedSteps = 120;
        private const float GravityAcceleration = 25.0f;
        private const float PenetrationTolerance = 0.05f;
        private const float PositionTolerance = 0.0001f;
        private static readonly Vector3 FixtureOrigin =
            new Vector3(12000.0f, 1000.0f, 0.0f);

        private GameObject _playerObject;
        private Rigidbody _playerRigidbody;
        private CapsuleCollider _playerCollider;
        private PhysicsMaterial _playerPhysicsMaterial;
        private MonoBehaviour _collisionSystem;

        [SetUp]
        public void SetUp()
        {
            _playerObject = new GameObject("WallFallPhysicsTests.Player");
            _playerObject.transform.position =
                FixtureOrigin + new Vector3(0.0f, 6.0f, 0.0f);

            _playerRigidbody = _playerObject.AddComponent<Rigidbody>();
            _playerRigidbody.useGravity = false;
            _playerRigidbody.constraints =
                RigidbodyConstraints.FreezePositionZ |
                RigidbodyConstraints.FreezeRotation;
            _playerRigidbody.collisionDetectionMode =
                CollisionDetectionMode.Continuous;
            _playerCollider = _playerObject.AddComponent<CapsuleCollider>();
            _playerPhysicsMaterial = new PhysicsMaterial(
                "WallFallPhysicsTests.PlayerZeroFriction")
            {
                dynamicFriction = 0.0f,
                staticFriction = 0.0f,
                bounciness = 0.0f,
                frictionCombine = PhysicsMaterialCombine.Minimum,
                bounceCombine = PhysicsMaterialCombine.Minimum
            };
            _playerCollider.sharedMaterial = _playerPhysicsMaterial;

            GameObject groundCheckObject =
                new GameObject("WallFallPhysicsTests.GroundCheck");
            Transform groundCheck = groundCheckObject.transform;
            groundCheck.SetParent(_playerObject.transform, false);
            groundCheck.localPosition = new Vector3(0.0f, -0.75f, 0.0f);

            Type collisionSystemType = Type.GetType(
                "FlowState.Runtime.Systems.CollisionSystem, Assembly-CSharp");
            Assert.That(collisionSystemType, Is.Not.Null);
            _collisionSystem = (MonoBehaviour)_playerObject.AddComponent(
                collisionSystemType);

            SetPrivateField("_playerCollider", _playerCollider);
            SetPrivateField("_groundCheck", groundCheck);
            SetPrivateField("_groundLayer", (LayerMask)(1 << GroundLayer));
            SetPrivateField("_groundCheckRadius", 0.25f);
            SetPrivateField("_groundedDistance", 0.05f);
            SetPrivateField("_groundPredictionDistance", 3.0f);

            Assert.That(InvokeBool("Initialize"), Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            if (_playerPhysicsMaterial != null)
            {
                UnityEngine.Object.DestroyImmediate(_playerPhysicsMaterial);
            }

            foreach (GameObject gameObject in
                     Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.name.StartsWith(
                        "WallFallPhysicsTests.",
                        StringComparison.Ordinal))
                {
                    UnityEngine.Object.DestroyImmediate(gameObject);
                }
            }
        }

        [UnityTest]
        public IEnumerator RightWall_SmallApproachSpeed_FallsAndExits()
        {
            yield return VerifyWallFall(1.0f, 2.0f);
        }

        [UnityTest]
        public IEnumerator LeftWall_SmallApproachSpeed_FallsAndExits()
        {
            yield return VerifyWallFall(-1.0f, 2.0f);
        }

        [UnityTest]
        public IEnumerator RightWall_LargeApproachSpeed_FallsWithoutPenetration()
        {
            yield return VerifyWallFall(1.0f, 14.0f);
        }

        [UnityTest]
        public IEnumerator LeftWall_LargeApproachSpeed_FallsWithoutPenetration()
        {
            yield return VerifyWallFall(-1.0f, 14.0f);
        }

        [UnityTest]
        public IEnumerator RightWall_HeldInputWithZeroFriction_ContinuesFalling()
        {
            const float approachSpeed = 8.0f;
            BoxCollider wallCollider = CreateWall(1.0f);
            bool didContactWall = false;
            bool didFallDuringContact = false;
            float firstContactHeight = 0.0f;

            for (int step = 0; step < MaximumFixedSteps; step++)
            {
                _playerRigidbody.linearVelocity = new Vector3(
                    approachSpeed,
                    PlayerMovementMath.CalculateVerticalSpeed(
                        _playerRigidbody.linearVelocity.y,
                        false,
                        GravityAcceleration,
                        Time.fixedDeltaTime),
                    0.0f);

                yield return new WaitForFixedUpdate();

                PlayerCollisionState collisionState = RefreshCollisionState();
                if (!collisionState.WallContacts.HasRightWall)
                {
                    continue;
                }

                AssertNoWallPenetration(wallCollider, 1.0f);

                if (!didContactWall)
                {
                    didContactWall = true;
                    firstContactHeight = _playerRigidbody.position.y;
                    continue;
                }

                if (_playerRigidbody.position.y <
                    firstContactHeight - 0.5f)
                {
                    didFallDuringContact = true;
                    break;
                }
            }

            Assert.That(didContactWall, Is.True);
            Assert.That(didFallDuringContact, Is.True);
        }

        private IEnumerator VerifyWallFall(float wallSide, float approachSpeed)
        {
            CreateGround();
            BoxCollider wallCollider = CreateWall(wallSide);
            float previousHeight = _playerRigidbody.position.y;
            float firstWallContactHeight = 0.0f;
            float wallExitHeight = 0.0f;
            int wallContactSteps = 0;
            int fallingWallContactSteps = 0;
            bool didContactWall = false;
            bool didExitBelowWall = false;

            _playerRigidbody.linearVelocity =
                new Vector3(wallSide * approachSpeed, 0.0f, 0.0f);

            for (int step = 0; step < MaximumFixedSteps; step++)
            {
                PlayerCollisionState collisionState = RefreshCollisionState();
                Vector3 requestedVelocity = new Vector3(
                    wallSide * approachSpeed,
                    PlayerMovementMath.CalculateVerticalSpeed(
                        _playerRigidbody.linearVelocity.y,
                        false,
                        GravityAcceleration,
                        Time.fixedDeltaTime),
                    0.0f);
                Vector3 constrainedVelocity =
                    PlayerMovementMath.ConstrainVelocityByWalls(
                        requestedVelocity,
                        collisionState.IsGrounded,
                        collisionState.WallContacts);
                _playerRigidbody.linearVelocity = constrainedVelocity;

                yield return new WaitForFixedUpdate();

                float currentHeight = _playerRigidbody.position.y;
                PlayerCollisionState updatedState = RefreshCollisionState();

                if (updatedState.WallContacts.HasWallContact)
                {
                    if (!didContactWall)
                    {
                        didContactWall = true;
                        firstWallContactHeight = currentHeight;
                    }

                    wallContactSteps++;

                    if (currentHeight < previousHeight - PositionTolerance)
                    {
                        fallingWallContactSteps++;
                    }

                    AssertNoWallPenetration(wallCollider, wallSide);
                }

                if (didContactWall &&
                    !updatedState.WallContacts.HasWallContact &&
                    _playerCollider.bounds.max.y <
                    wallCollider.bounds.min.y + PenetrationTolerance)
                {
                    didExitBelowWall = true;
                    wallExitHeight = currentHeight;
                    break;
                }

                previousHeight = currentHeight;
            }

            Assert.That(didContactWall, Is.True);
            Assert.That(wallContactSteps, Is.GreaterThanOrEqualTo(3));
            Assert.That(fallingWallContactSteps, Is.GreaterThanOrEqualTo(3));
            Assert.That(
                _playerRigidbody.position.y,
                Is.LessThan(firstWallContactHeight - 0.5f));
            Assert.That(didExitBelowWall, Is.True);

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            Assert.That(
                _playerRigidbody.position.y,
                Is.LessThan(wallExitHeight - PositionTolerance));
        }

        private BoxCollider CreateWall(float wallSide)
        {
            GameObject wallObject = new GameObject("WallFallPhysicsTests.Wall");
            wallObject.layer = GroundLayer;
            wallObject.transform.position =
                FixtureOrigin + new Vector3(wallSide * 1.1f, 4.0f, 0.0f);
            BoxCollider wallCollider = wallObject.AddComponent<BoxCollider>();
            wallCollider.size = new Vector3(1.0f, 6.0f, 4.0f);
            Physics.SyncTransforms();
            return wallCollider;
        }

        private void CreateGround()
        {
            GameObject groundObject = new GameObject(
                "WallFallPhysicsTests.Ground");
            groundObject.layer = GroundLayer;
            groundObject.transform.position =
                FixtureOrigin + new Vector3(0.0f, -4.5f, 0.0f);
            BoxCollider groundCollider = groundObject.AddComponent<BoxCollider>();
            groundCollider.size = new Vector3(20.0f, 1.0f, 4.0f);
        }

        private void AssertNoWallPenetration(
            BoxCollider wallCollider,
            float wallSide)
        {
            if (wallSide > 0.0f)
            {
                Assert.That(
                    _playerCollider.bounds.max.x,
                    Is.LessThanOrEqualTo(
                        wallCollider.bounds.min.x + PenetrationTolerance));
                return;
            }

            Assert.That(
                _playerCollider.bounds.min.x,
                Is.GreaterThanOrEqualTo(
                    wallCollider.bounds.max.x - PenetrationTolerance));
        }

        private PlayerCollisionState RefreshCollisionState()
        {
            Invoke("RefreshCollisionState");
            return (PlayerCollisionState)Invoke("GetCollisionState");
        }

        private bool InvokeBool(string methodName)
        {
            return (bool)Invoke(methodName);
        }

        private object Invoke(string methodName)
        {
            MethodInfo method = _collisionSystem.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(method, Is.Not.Null);
            return method.Invoke(_collisionSystem, null);
        }

        private void SetPrivateField(string fieldName, object value)
        {
            FieldInfo field = _collisionSystem.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null);
            field.SetValue(_collisionSystem, value);
        }
    }
}
