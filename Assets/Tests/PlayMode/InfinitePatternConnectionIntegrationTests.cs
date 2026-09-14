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
    public class InfinitePatternConnectionIntegrationTests
    {
        private const int MaximumFixedSteps = 120;
        private static readonly string[] PatternIds =
        {
            InfinitePatternCatalogFactory.FlatId,
            InfinitePatternCatalogFactory.SingleRiseId,
            InfinitePatternCatalogFactory.LegacyStepsId,
            InfinitePatternCatalogFactory.InternalGapId
        };

        private InfiniteMapPattern _map;
        private InfinitePatternSlot _first;
        private InfinitePatternSlot _second;
        private Rigidbody _player;
        private Collider _playerCollider;
        private MonoBehaviour _input;
        private MonoBehaviour _collision;
        private MonoBehaviour _infiniteMode;
        private MonoBehaviour _gameSystem;
        private MonoBehaviour _runtimeSystem;
        private Transform _cameraTarget;
        private float _minimumSpeed;
        private float _startGrace;
        private float _belowSpeedGrace;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            AsyncOperation load = SceneManager.LoadSceneAsync(
                "SampleScene", LoadSceneMode.Single);
            while (!load.isDone)
            {
                yield return null;
            }

            yield return null;
            _input = FindBehaviour("PlayerInputSystem", "PlayerInputSystem");
            _collision = FindBehaviour("Player", "CollisionSystem");
            _infiniteMode = FindBehaviour("InfiniteModeSystem", "InfiniteModeSystem");
            _gameSystem = FindBehaviour("GameSystem", "GameSystem");
            _runtimeSystem = FindBehaviour("RuntimeDataSystem", "RuntimeDataSystem");
            _minimumSpeed = GetFloat(_infiniteMode, "_minimumHorizontalSpeed");
            _startGrace = GetFloat(_infiniteMode, "_startGraceDuration");
            _belowSpeedGrace = GetFloat(
                _infiniteMode, "_belowSpeedGraceDuration");
            SetField(_infiniteMode, "_minimumHorizontalSpeed", 0.0f);
            SetField(_infiniteMode, "_startGraceDuration", 100.0f);
            SetField(_infiniteMode, "_belowSpeedGraceDuration", 100.0f);
            ProductionSceneGameModeTestUtility.RestartInMode(E_GameMode.Infinite);
            yield return new WaitForFixedUpdate();

            _map = GameObject.Find("InfiniteMapPattern")
                ?.GetComponent<InfiniteMapPattern>();
            _first = GameObject.Find("Slot_0")
                ?.GetComponent<InfinitePatternSlot>();
            _second = GameObject.Find("Slot_1")
                ?.GetComponent<InfinitePatternSlot>();
            _player = GameObject.Find("Player")?.GetComponent<Rigidbody>();
            _playerCollider = _player?.GetComponent<Collider>();
            _cameraTarget = GameObject.Find("CameraFollowTarget")?.transform;
            Assert.That(_map, Is.Not.Null);
            Assert.That(_map.IsInitialized, Is.True);
            Assert.That(_first, Is.Not.Null);
            Assert.That(_second, Is.Not.Null);
            Assert.That(_player, Is.Not.Null);
            Assert.That(_playerCollider, Is.Not.Null);
            Assert.That(_cameraTarget, Is.Not.Null);
            Assert.That(_map.ResetPatterns(), Is.True);
            SetField(_infiniteMode, "_hasPendingPatternRequest", true);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (_infiniteMode != null)
            {
                SetField(_infiniteMode, "_minimumHorizontalSpeed", _minimumSpeed);
                SetField(_infiniteMode, "_startGraceDuration", _startGrace);
                SetField(_infiniteMode, "_belowSpeedGraceDuration",
                    _belowSpeedGrace);
            }

            Time.timeScale = 1.0f;
            yield return null;
        }

        [UnityTest]
        public IEnumerator AllSixteenConnections_BaseSpeed_JumpAndLand()
        {
            yield return TraverseAllConnections(8.0f, 18.0f);
        }

        [UnityTest]
        public IEnumerator AllSixteenConnections_MaximumSpeed_JumpAndLand()
        {
            yield return TraverseAllConnections(14.0f, 13.0f);
        }

        [UnityTest]
        public IEnumerator FrontBoundary_PhysicalTrigger_AdvancesOnlyOnce()
        {
            Assert.That(_map.TryRequestNextPattern(
                1, InfinitePatternCatalogFactory.FlatId), Is.True);
            PlacePlayer(38.0f, 1.5f, 0.0f);
            yield return new WaitForFixedUpdate();
            Assert.That(GetCollisionState().IsGrounded, Is.True);
            _player.linearVelocity = new Vector3(8.0f, 0.0f, 0.0f);

            bool advanced = false;
            for (int step = 0; step < 30; step++)
            {
                yield return new WaitForFixedUpdate();
                if (_map.AdvanceCount == 1)
                {
                    advanced = true;
                    break;
                }
            }

            Assert.That(advanced, Is.True);
            Assert.That(_second.AdvanceBoundary.IsTriggered, Is.True);
            for (int step = 0; step < 10; step++)
            {
                yield return new WaitForFixedUpdate();
            }
            Assert.That(_map.AdvanceCount, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator RepeatedRequests_AlternateSlotsWithoutScopeOrBoundaryLeaks()
        {
            GameRuntimeData runtimeData = GetRuntimeData();
            Assert.That(runtimeData.CollectibleRuntimeData.ActiveScopeCount,
                Is.EqualTo(2));

            for (int index = 0; index < 8; index++)
            {
                InfinitePatternSlot front = index % 2 == 0
                    ? _second : _first;
                InfinitePatternSlot reused = index % 2 == 0
                    ? _first : _second;
                string requested = PatternIds[index % PatternIds.Length];
                Assert.That(_map.TryRequestNextPattern(index + 1, requested),
                    Is.True);
                Assert.That(_map.TryRequestNextPattern(index + 1, requested),
                    Is.False);

                Assert.That(reused.TryGetCurrentPattern(
                    out InfinitePatternAuthoring previous), Is.True);
                PlacePlayer(previous.EndAnchor.position.x + 2.0f, 1.5f, 0.0f);
                InvokeBoundary(front.AdvanceBoundary, _playerCollider);
                Assert.That(_map.AdvanceCount, Is.EqualTo(index + 1));
                Assert.That(front.AdvanceBoundary.IsTriggered, Is.True);
                Assert.That(_map.TryAdvance(front.SlotId), Is.False);
                Assert.That(_map.CurrentPatternId, Is.EqualTo(requested));
                Assert.That(reused.TryGetCurrentPattern(
                    out InfinitePatternAuthoring next), Is.True);
                Assert.That(front.TryGetCurrentPattern(
                    out InfinitePatternAuthoring frontPattern), Is.True);
                Assert.That(next.StartAnchor.position,
                    Is.EqualTo(frontPattern.EndAnchor.position));
                Assert.That(reused.PatternInstanceCount, Is.EqualTo(4));
                Assert.That(front.PatternInstanceCount, Is.EqualTo(4));
                Assert.That(runtimeData.CollectibleRuntimeData.ActiveScopeCount,
                    Is.EqualTo(2));
                Assert.That(runtimeData.CollectibleRuntimeData.RegisteredCount,
                    Is.Zero);
            }

            Assert.That(_map.ResetPatterns(), Is.True);
            Assert.That(_map.AdvanceCount, Is.Zero);
            Assert.That(_first.CurrentPatternId,
                Is.EqualTo(InfinitePatternCatalogFactory.FlatId));
            Assert.That(_second.CurrentPatternId,
                Is.EqualTo(InfinitePatternCatalogFactory.FlatId));
            Assert.That(_first.transform.position.x, Is.EqualTo(0.0f));
            Assert.That(_second.transform.position.x, Is.EqualTo(44.0f));
            Assert.That(_first.AdvanceBoundary.IsTriggered, Is.False);
            Assert.That(_second.AdvanceBoundary.IsTriggered, Is.False);
            Assert.That(runtimeData.CollectibleRuntimeData.ActiveScopeCount,
                Is.EqualTo(2));
            yield return null;
        }

        [UnityTest]
        public IEnumerator PauseResumeAndResultRetry_RestoreSlotState()
        {
            Assert.That((bool)InvokePublic(_gameSystem, "PauseGame"), Is.True);
            Assert.That(_map.AdvanceCount, Is.Zero);
            Assert.That((bool)InvokePublic(_gameSystem, "ResumeGame"), Is.True);

            Assert.That(_map.TryRequestNextPattern(
                1, InfinitePatternCatalogFactory.InternalGapId), Is.True);
            PlacePlayer(24.0f, 1.5f, 0.0f);
            InvokeBoundary(_second.AdvanceBoundary, _playerCollider);
            Assert.That(_map.AdvanceCount, Is.EqualTo(1));

            _player.position = new Vector3(10000.0f, -3.1f, 0.0f);
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            yield return null;
            Assert.That((E_GameState)GetPublicProperty(
                _gameSystem, "CurrentGameState"), Is.EqualTo(E_GameState.Ended));

            ProductionSceneGameModeTestUtility.RestartInMode(E_GameMode.Infinite);
            Assert.That(_map.AdvanceCount, Is.Zero);
            Assert.That(_first.CurrentPatternId,
                Is.EqualTo(InfinitePatternCatalogFactory.FlatId));
            Assert.That(_second.CurrentPatternId,
                Is.EqualTo(InfinitePatternCatalogFactory.FlatId));
            Assert.That(_first.AdvanceBoundary.IsTriggered, Is.False);
            Assert.That(_second.AdvanceBoundary.IsTriggered, Is.False);
            Assert.That(GetRuntimeData().CollectibleRuntimeData.ActiveScopeCount,
                Is.EqualTo(2));
        }

        private IEnumerator TraverseAllConnections(float speed, float startX)
        {
            for (int firstIndex = 0; firstIndex < PatternIds.Length; firstIndex++)
            {
                for (int secondIndex = 0;
                     secondIndex < PatternIds.Length; secondIndex++)
                {
                    string firstId = PatternIds[firstIndex];
                    string secondId = PatternIds[secondIndex];
                    Assert.That(_map.ResetPatterns(), Is.True);
                    Assert.That(_first.TryActivatePattern(firstId), Is.True);
                    Assert.That(_second.TryActivatePattern(secondId), Is.True);
                    Assert.That(_first.TryGetCurrentPattern(
                        out InfinitePatternAuthoring firstPattern), Is.True);
                    Assert.That(_second.TryAlignStartTo(
                        firstPattern.EndAnchor.position), Is.True);
                    Assert.That(_second.TryGetCurrentPattern(
                        out InfinitePatternAuthoring secondPattern), Is.True);
                    AssertConnection(firstPattern, secondPattern);

                    PlacePlayer(startX, 1.5f, 0.0f);
                    yield return new WaitForFixedUpdate();
                    Assert.That(GetCollisionState().IsGrounded, Is.True,
                        firstId + " -> " + secondId + " start not grounded");
                    _player.linearVelocity = new Vector3(speed, 0.0f, 0.0f);
                    SetField(_input, "_isJumpPressed", true);
                    bool leftGround = false;
                    bool landed = false;
                    for (int step = 0; step < MaximumFixedSteps; step++)
                    {
                        yield return new WaitForFixedUpdate();
                        PlayerCollisionState state = GetCollisionState();
                        leftGround |= !state.IsGrounded &&
                            _player.position.y > 1.55f;
                        if (leftGround && state.IsGrounded &&
                            _player.position.x >= 25.0f &&
                            Mathf.Abs(_player.position.y - 1.5f) < 0.12f)
                        {
                            landed = true;
                            break;
                        }

                        if (_player.position.x > 38.0f ||
                            _player.position.y < -2.5f)
                        {
                            break;
                        }
                    }

                    Assert.That(leftGround, Is.True,
                        firstId + " -> " + secondId + " never jumped");
                    Assert.That(landed, Is.True,
                        firstId + " -> " + secondId + " did not land at " +
                        speed + ". Player=" + _player.position);
                    yield return new WaitForEndOfFrame();
                    Assert.That(_cameraTarget.position.x,
                        Is.EqualTo(_player.transform.position.x).Within(0.1f));
                    Assert.That(_map.AdvanceCount, Is.Zero);
                }
            }
        }

        private static void AssertConnection(
            InfinitePatternAuthoring first,
            InfinitePatternAuthoring second)
        {
            Assert.That(first.EndAnchor.position,
                Is.EqualTo(second.StartAnchor.position));
            Assert.That(first.TryGetTerrainCollider(
                first.TerrainColliderCount - 1, out Collider last), Is.True);
            Assert.That(second.TryGetTerrainCollider(
                0, out Collider next), Is.True);
            Assert.That(next.bounds.min.x - last.bounds.max.x,
                Is.EqualTo(4.0f).Within(0.01f));
            Assert.That(last.bounds.max.y,
                Is.EqualTo(next.bounds.max.y).Within(0.01f));
            Assert.That(last.bounds.size.z,
                Is.EqualTo(next.bounds.size.z).Within(0.01f));
        }

        private void PlacePlayer(float x, float y, float speed)
        {
            _player.position = new Vector3(x, y, 0.0f);
            _player.linearVelocity = new Vector3(speed, 0.0f, 0.0f);
            _player.angularVelocity = Vector3.zero;
            Physics.SyncTransforms();
        }

        private PlayerCollisionState GetCollisionState()
        {
            InvokePublic(_collision, "RefreshCollisionState");
            return (PlayerCollisionState)InvokePublic(
                _collision, "GetCollisionState");
        }

        private GameRuntimeData GetRuntimeData()
        {
            return (GameRuntimeData)GetPublicProperty(
                _runtimeSystem, "RuntimeData");
        }

        private static void InvokeBoundary(
            InfinitePatternBoundary boundary, Collider player)
        {
            MethodInfo method = typeof(InfinitePatternBoundary).GetMethod(
                "OnTriggerEnter", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            method.Invoke(boundary, new object[] { player });
        }

        private static object InvokePublic(MonoBehaviour target, string name)
        {
            MethodInfo method = target.GetType().GetMethod(
                name, BindingFlags.Instance | BindingFlags.Public);
            Assert.That(method, Is.Not.Null);
            return method.Invoke(target, null);
        }

        private static object GetPublicProperty(
            MonoBehaviour target, string name)
        {
            PropertyInfo property = target.GetType().GetProperty(
                name, BindingFlags.Instance | BindingFlags.Public);
            Assert.That(property, Is.Not.Null);
            return property.GetValue(target);
        }

        private static MonoBehaviour FindBehaviour(
            string gameObjectName, string typeName)
        {
            GameObject owner = GameObject.Find(gameObjectName);
            Assert.That(owner, Is.Not.Null);
            MonoBehaviour[] behaviours = owner.GetComponents<MonoBehaviour>();
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

        private static float GetFloat(MonoBehaviour target, string name)
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
