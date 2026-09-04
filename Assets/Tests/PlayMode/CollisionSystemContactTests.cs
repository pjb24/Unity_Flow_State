using System;
using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class CollisionSystemContactTests
    {
        private const int GroundLayer = 6;
        private static readonly Vector3 FixtureOrigin =
            new Vector3(10000.0f, 1000.0f, 0.0f);

        private GameObject _playerObject;
        private Rigidbody _playerRigidbody;
        private CapsuleCollider _playerCollider;
        private Transform _groundCheck;
        private MonoBehaviour _collisionSystem;

        [SetUp]
        public void SetUp()
        {
            _playerObject = new GameObject("CollisionSystemContactTests.Player");
            _playerObject.transform.position =
                FixtureOrigin + new Vector3(0.0f, 2.0f, 0.0f);
            _playerRigidbody = _playerObject.AddComponent<Rigidbody>();
            _playerRigidbody.useGravity = false;
            _playerRigidbody.constraints =
                RigidbodyConstraints.FreezePositionZ |
                RigidbodyConstraints.FreezeRotation;
            _playerRigidbody.collisionDetectionMode =
                CollisionDetectionMode.Continuous;
            _playerCollider = _playerObject.AddComponent<CapsuleCollider>();

            GameObject groundCheckObject =
                new GameObject("CollisionSystemContactTests.GroundCheck");
            _groundCheck = groundCheckObject.transform;
            _groundCheck.SetParent(_playerObject.transform, false);
            _groundCheck.localPosition = new Vector3(0.0f, -0.75f, 0.0f);

            Type collisionSystemType = Type.GetType(
                "FlowState.Runtime.Systems.CollisionSystem, Assembly-CSharp");
            Assert.That(collisionSystemType, Is.Not.Null);
            _collisionSystem = (MonoBehaviour)_playerObject.AddComponent(
                collisionSystemType);

            SetPrivateField("_playerCollider", _playerCollider);
            SetPrivateField("_groundCheck", _groundCheck);
            SetPrivateField("_groundLayer", (LayerMask)(1 << GroundLayer));
            SetPrivateField("_groundCheckRadius", 0.25f);
            SetPrivateField("_groundedDistance", 0.05f);
            SetPrivateField("_groundPredictionDistance", 3.0f);

            Assert.That(InvokeBool("Initialize"), Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (GameObject gameObject in
                     Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (gameObject.name.StartsWith(
                        "CollisionSystemContactTests.",
                        StringComparison.Ordinal))
                {
                    UnityEngine.Object.DestroyImmediate(gameObject);
                }
            }
        }

        [Test]
        public void RefreshCollisionState_GroundBelow_ReportsGroundOnly()
        {
            CreateBox(
                "Ground",
                new Vector3(0.0f, 0.5f, 0.0f),
                new Vector3(10.0f, 1.0f, 4.0f),
                false);
            Physics.SyncTransforms();

            PlayerCollisionState state = RefreshCollisionState();

            Assert.That(state.IsGrounded, Is.True);
            Assert.That(state.WallContacts.HasWallContact, Is.False);
            Assert.That(state.SurfaceNormal.y, Is.GreaterThan(0.99f));
        }

        [UnityTest]
        public IEnumerator VerticalWallContact_ReportsWallWithoutGround()
        {
            CreateBox(
                "RightWall",
                new Vector3(1.1f, 2.0f, 0.0f),
                new Vector3(1.0f, 6.0f, 4.0f),
                false);
            Physics.SyncTransforms();
            _playerRigidbody.linearVelocity = new Vector3(5.0f, 0.0f, 0.0f);

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            PlayerCollisionState state = RefreshCollisionState();

            Assert.That(state.IsGrounded, Is.False);
            Assert.That(state.WallContacts.HasWallContact, Is.True);
            Assert.That(state.WallContacts.HasRightWall, Is.True);
            Assert.That(state.WallContacts.RightWallNormal.x, Is.LessThan(-0.9f));
        }

        [UnityTest]
        public IEnumerator GroundAndWallContact_PreservesBothResults()
        {
            CreateBox(
                "Ground",
                new Vector3(0.0f, 0.5f, 0.0f),
                new Vector3(10.0f, 1.0f, 4.0f),
                false);
            CreateBox(
                "RightWall",
                new Vector3(1.1f, 2.0f, 0.0f),
                new Vector3(1.0f, 6.0f, 4.0f),
                false);
            Physics.SyncTransforms();
            _playerRigidbody.linearVelocity = new Vector3(5.0f, 0.0f, 0.0f);

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            PlayerCollisionState state = RefreshCollisionState();

            Assert.That(state.IsGrounded, Is.True);
            Assert.That(state.WallContacts.HasRightWall, Is.True);
            Assert.That(state.SurfaceNormal.y, Is.GreaterThan(0.99f));
        }

        [Test]
        public void RefreshCollisionState_TriggerBelow_IsIgnored()
        {
            CreateBox(
                "Trigger",
                new Vector3(0.0f, 0.5f, 0.0f),
                new Vector3(10.0f, 1.0f, 4.0f),
                true);
            Physics.SyncTransforms();

            PlayerCollisionState state = RefreshCollisionState();

            Assert.That(state.IsGrounded, Is.False);
            Assert.That(state.GroundDistance, Is.EqualTo(float.PositiveInfinity));
        }

        [Test]
        public void RefreshCollisionState_PlayerChildCollider_IsExcluded()
        {
            BoxCollider childCollider = CreateBox(
                "PlayerChild",
                new Vector3(0.0f, 0.48f, 0.0f),
                new Vector3(2.0f, 1.0f, 2.0f),
                false);
            childCollider.transform.SetParent(_playerObject.transform, true);
            CreateBox(
                "Ground",
                Vector3.zero,
                new Vector3(10.0f, 1.0f, 4.0f),
                false);
            Physics.SyncTransforms();

            PlayerCollisionState state = RefreshCollisionState();

            Assert.That(state.IsGrounded, Is.False);
            Assert.That(state.GroundDistance, Is.EqualTo(0.5f).Within(0.02f));
            Assert.That(
                state.ContactPoint.y,
                Is.EqualTo(FixtureOrigin.y + 0.5f).Within(0.02f));
        }

        [Test]
        public void RefreshCollisionState_FullHitBuffer_SelectsClosestGround()
        {
            const int hitBufferSize = 16;

            for (int index = 0; index < hitBufferSize; index++)
            {
                float surfaceHeight = 0.96f - index * 0.14f;
                CreateBox(
                    $"Ground_{index}",
                    new Vector3(0.0f, surfaceHeight - 0.025f, 0.0f),
                    new Vector3(10.0f, 0.05f, 4.0f),
                    false);
            }

            Physics.SyncTransforms();

            PlayerCollisionState state = RefreshCollisionState();

            Assert.That(state.IsGrounded, Is.True);
            Assert.That(state.GroundDistance, Is.EqualTo(0.04f).Within(0.02f));
            Assert.That(
                state.ContactPoint.y,
                Is.EqualTo(FixtureOrigin.y + 0.96f).Within(0.02f));
        }

        private BoxCollider CreateBox(
            string suffix,
            Vector3 position,
            Vector3 size,
            bool isTrigger)
        {
            GameObject boxObject = new GameObject(
                $"CollisionSystemContactTests.{suffix}");
            boxObject.layer = GroundLayer;
            boxObject.transform.position = FixtureOrigin + position;
            BoxCollider boxCollider = boxObject.AddComponent<BoxCollider>();
            boxCollider.size = size;
            boxCollider.isTrigger = isTrigger;
            return boxCollider;
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
