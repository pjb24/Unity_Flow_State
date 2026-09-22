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
    public class InfinitePatternTraversalIntegrationTests
    {
        private const string SceneName = "SampleScene";
        private const int MaximumFixedSteps = 240;

        private Rigidbody _playerRigidbody;
        private MonoBehaviour _inputSystem;
        private MonoBehaviour _playerMovementSystem;
        private MonoBehaviour _collisionSystem;
        private MonoBehaviour _infiniteModeSystem;
        private InfinitePatternSlot _firstSlot;
        private float _originalMinimumSpeed;
        private float _originalStartGrace;
        private float _originalBelowSpeedGrace;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            AsyncOperation load = SceneManager.LoadSceneAsync(
                SceneName, LoadSceneMode.Single);
            while (!load.isDone)
            {
                yield return null;
            }

            yield return null;
            _playerRigidbody = GameObject.Find("Player")?.GetComponent<Rigidbody>();
            Assert.That(_playerRigidbody, Is.Not.Null);
            _inputSystem = FindBehaviour("PlayerInputSystem", "PlayerInputSystem");
            _playerMovementSystem = FindBehaviour(
                "PlayerMovementSystem", "PlayerMovementSystem");
            _collisionSystem = FindBehaviour("Player", "CollisionSystem");
            _infiniteModeSystem = FindBehaviour(
                "InfiniteModeSystem", "InfiniteModeSystem");
            _originalMinimumSpeed = GetFloatField(
                _infiniteModeSystem, "_minimumHorizontalSpeed");
            _originalStartGrace = GetFloatField(
                _infiniteModeSystem, "_startGraceDuration");
            _originalBelowSpeedGrace = GetFloatField(
                _infiniteModeSystem, "_belowSpeedGraceDuration");
            SetField(_infiniteModeSystem, "_minimumHorizontalSpeed", 0.0f);
            SetField(_infiniteModeSystem, "_startGraceDuration", 100.0f);
            SetField(_infiniteModeSystem, "_belowSpeedGraceDuration", 100.0f);
            ProductionSceneGameModeTestUtility.RestartInMode(E_GameMode.Infinite);
            yield return new WaitForFixedUpdate();

            GameObject slotObject = GameObject.Find("Slot_0");
            Assert.That(slotObject, Is.Not.Null);
            _firstSlot = slotObject.GetComponent<InfinitePatternSlot>();
            Assert.That(_firstSlot, Is.Not.Null);
            Assert.That(_firstSlot.IsInitialized, Is.True);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (_infiniteModeSystem != null)
            {
                SetField(_infiniteModeSystem, "_minimumHorizontalSpeed",
                    _originalMinimumSpeed);
                SetField(_infiniteModeSystem, "_startGraceDuration",
                    _originalStartGrace);
                SetField(_infiniteModeSystem, "_belowSpeedGraceDuration",
                    _originalBelowSpeedGrace);
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator Flat_StationaryStart_AcceleratesAndRemainsGrounded()
        {
            Assert.That(_firstSlot.TryActivatePattern(
                InfinitePatternCatalogFactory.FlatId), Is.True);
            PlacePlayer(-18.0f, 1.5f, 0.0f);
            Assert.That(GetCollisionState().IsGrounded, Is.True);

            bool reachedEnd = false;
            float peakSpeed = 0.0f;
            for (int step = 0; step < MaximumFixedSteps; step++)
            {
                yield return new WaitForFixedUpdate();
                peakSpeed = Mathf.Max(
                    peakSpeed, _playerRigidbody.linearVelocity.x);
                if (_playerRigidbody.position.x > -16.0f &&
                    _playerRigidbody.position.x < 18.0f)
                {
                    Assert.That(GetCollisionState().IsGrounded, Is.True);
                }

                if (_playerRigidbody.position.x >= 16.0f)
                {
                    reachedEnd = true;
                    break;
                }
            }

            Assert.That(reachedEnd, Is.True);
            Assert.That(
                peakSpeed,
                Is.GreaterThanOrEqualTo(GetPlayerMoveSpeed() - 0.5f));
            Assert.That(GetCollisionState().IsGrounded, Is.True);
        }

        [UnityTest]
        public IEnumerator SingleRise_BaseSpeed_EntersAndLandsOnPlatform()
        {
            yield return TraverseGap(
                InfinitePatternCatalogFactory.SingleRiseId,
                -11.0f, 1.5f, -6.0f, 8.0f, 2.5f);
        }

        [UnityTest]
        public IEnumerator LegacySteps_FirstPlatform_Lands()
        {
            yield return TraverseGap(
                InfinitePatternCatalogFactory.LegacyStepsId,
                -17.0f, 1.5f, -10.0f, -2.0f, 2.5f);
        }

        [UnityTest]
        public IEnumerator LegacySteps_SecondPlatform_Lands()
        {
            yield return TraverseGap(
                InfinitePatternCatalogFactory.LegacyStepsId,
                -7.0f, 2.5f, 0.0f, 8.0f, 2.5f);
        }

        [UnityTest]
        public IEnumerator InternalGap_BaseSpeed_CrossesAndLands()
        {
            yield return TraverseGap(
                InfinitePatternCatalogFactory.InternalGapId,
                -4.2f, 1.5f, 3.0f, 20.0f, 1.5f);
        }

        [UnityTest]
        public IEnumerator SingleRise_FrontWallFallThenGroundLanding_ClearsWallContact()
        {
            Assert.That(_firstSlot.TryActivatePattern(
                InfinitePatternCatalogFactory.SingleRiseId), Is.True);
            PlacePlayer(-8.4f, 1.5f, GetPlayerMoveSpeed());
            Assert.That(GetCollisionState().IsGrounded, Is.True);

            bool contactedWallInAir = false;
            bool continuedFalling = false;
            float wallContactHeight = 0.0f;
            for (int step = 0; step < MaximumFixedSteps; step++)
            {
                yield return new WaitForFixedUpdate();
                PlayerCollisionState state = GetCollisionState();
                if (!contactedWallInAir &&
                    state.WallContacts.HasWallContact && !state.IsGrounded)
                {
                    contactedWallInAir = true;
                    wallContactHeight = _playerRigidbody.position.y;
                }

                if (contactedWallInAir &&
                    _playerRigidbody.position.y < wallContactHeight - 0.5f)
                {
                    continuedFalling = true;
                    break;
                }
            }

            Assert.That(contactedWallInAir, Is.True);
            Assert.That(continuedFalling, Is.True);

            PlacePlayer(14.0f, 4.0f, 0.0f);
            bool groundedWithoutWall = false;
            for (int step = 0; step < 80; step++)
            {
                yield return new WaitForFixedUpdate();
                PlayerCollisionState state = GetCollisionState();
                if (state.IsGrounded &&
                    !state.WallContacts.HasWallContact &&
                    Mathf.Abs(_playerRigidbody.position.y - 1.5f) < 0.12f)
                {
                    groundedWithoutWall = true;
                    break;
                }
            }

            Assert.That(groundedWithoutWall, Is.True);
        }

        private IEnumerator TraverseGap(
            string patternId,
            float startX,
            float startY,
            float landingStartX,
            float landingEndX,
            float landingY)
        {
            float speed = GetPlayerMoveSpeed();
            Assert.That(_firstSlot.TryActivatePattern(patternId), Is.True);
            PlacePlayer(startX, startY, 0.0f);
            yield return new WaitForFixedUpdate();
            Assert.That(GetCollisionState().IsGrounded, Is.True,
                patternId + " start was not grounded. Player=" +
                _playerRigidbody.position + ", Slot=" +
                _firstSlot.transform.position);
            _playerRigidbody.linearVelocity =
                new Vector3(speed, 0.0f, 0.0f);
            SetField(_inputSystem, "_isJumpPressed", true);

            bool leftGround = false;
            bool landed = false;
            bool leftLandingPlatform = false;
            for (int step = 0; step < MaximumFixedSteps; step++)
            {
                yield return new WaitForFixedUpdate();
                PlayerCollisionState collision = GetCollisionState();
                float x = _playerRigidbody.position.x;
                leftGround |= !collision.IsGrounded &&
                    _playerRigidbody.position.y > startY + 0.05f;
                if (leftGround && collision.IsGrounded &&
                    x >= landingStartX + 0.5f &&
                    x <= landingEndX - 0.5f &&
                    Mathf.Abs(_playerRigidbody.position.y - landingY) < 0.12f)
                {
                    landed = true;
                    if (x < landingEndX - 2.0f)
                    {
                        for (int following = 0; following < 3; following++)
                        {
                            yield return new WaitForFixedUpdate();
                            if (_playerRigidbody.position.x > x + 0.1f &&
                                GetCollisionState().IsGrounded)
                            {
                                leftLandingPlatform = true;
                            }
                        }
                    }
                    else
                    {
                        leftLandingPlatform =
                            _playerRigidbody.linearVelocity.x > 0.0f;
                    }
                    break;
                }

                if (x > landingEndX + 1.0f ||
                    _playerRigidbody.position.y < -2.5f)
                {
                    break;
                }
            }

            Assert.That(leftGround, Is.True, patternId + " never jumped.");
            Assert.That(landed, Is.True,
                patternId + " did not land on the target surface at speed " +
                speed + ". Player=" + _playerRigidbody.position +
                ", Velocity=" + _playerRigidbody.linearVelocity);
            Assert.That(leftLandingPlatform, Is.True,
                patternId + " did not continue after landing.");
        }

        private void PlacePlayer(float x, float y, float speed)
        {
            _playerRigidbody.position = new Vector3(x, y, 0.0f);
            _playerRigidbody.linearVelocity = new Vector3(speed, 0.0f, 0.0f);
            _playerRigidbody.angularVelocity = Vector3.zero;
            Physics.SyncTransforms();
        }

        private float GetPlayerMoveSpeed()
        {
            return GetFloatField(_playerMovementSystem, "_moveSpeed");
        }

        private PlayerCollisionState GetCollisionState()
        {
            MethodInfo refresh = _collisionSystem.GetType().GetMethod(
                "RefreshCollisionState", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo get = _collisionSystem.GetType().GetMethod(
                "GetCollisionState", BindingFlags.Instance | BindingFlags.Public);
            Assert.That(refresh, Is.Not.Null);
            Assert.That(get, Is.Not.Null);
            refresh.Invoke(_collisionSystem, null);
            return (PlayerCollisionState)get.Invoke(_collisionSystem, null);
        }

        private static MonoBehaviour FindBehaviour(
            string gameObjectName, string typeName)
        {
            GameObject target = GameObject.Find(gameObjectName);
            Assert.That(target, Is.Not.Null);
            MonoBehaviour[] behaviours = target.GetComponents<MonoBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] != null &&
                    behaviours[i].GetType().Name == typeName)
                {
                    return behaviours[i];
                }
            }

            Assert.Fail(typeName + " was not found on " + gameObjectName);
            return null;
        }

        private static float GetFloatField(MonoBehaviour target, string name)
        {
            FieldInfo field = target.GetType().GetField(
                name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            return (float)field.GetValue(target);
        }

        private static void SetField(
            MonoBehaviour target, string name, object value)
        {
            FieldInfo field = target.GetType().GetField(
                name, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            field.SetValue(target, value);
        }
    }
}
