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
    public class ScoreCollectibleTests
    {
        private Scene _scene;
        private PhysicsScene _physicsScene;
        private GameRuntimeData _runtime;
        private Rigidbody _player;
        private ScoreCollectible _coin;
        private Collider _trigger;
        private Renderer _visual;
        private long _scope;

        [SetUp]
        public void SetUp()
        {
            _scene = SceneManager.CreateScene("ScoreCollectibleTests",
                new CreateSceneParameters(LocalPhysicsMode.Physics3D));
            _physicsScene = _scene.GetPhysicsScene();
            _runtime = new GameRuntimeData();
            _runtime.Initialize();
            _runtime.SetGameState(E_GameState.Playing);
            Assert.That(_runtime.CollectibleRuntimeData.TryCreateScope(out _scope), Is.True);

            GameObject playerObject = new GameObject("Player");
            SceneManager.MoveGameObjectToScene(playerObject, _scene);
            playerObject.transform.position = new Vector3(5.0f, 0.0f, 0.0f);
            _player = playerObject.AddComponent<Rigidbody>();
            _player.useGravity = false;
            _player.constraints = RigidbodyConstraints.FreezeAll;
            playerObject.AddComponent<CapsuleCollider>();

            GameObject coinObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            SceneManager.MoveGameObjectToScene(coinObject, _scene);
            _trigger = coinObject.GetComponent<Collider>();
            _trigger.isTrigger = true;
            _visual = coinObject.GetComponent<Renderer>();
            _coin = coinObject.AddComponent<ScoreCollectible>();
            SetField("_collectibleId", "coin");
            SetField("_triggerCollider", _trigger);
            SetField("_visual", _visual);
            SetField("_playerLayers", (LayerMask)1);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (_scene.IsValid() && _scene.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(_scene);
            }
            _runtime.Clear();
        }

        [Test]
        public void PlayerContact_CollectsOnceAndDisablesPresentation()
        {
            Bind();
            MovePlayer(Vector3.zero);
            AssertCollected();
            Simulate();
            MovePlayer(Vector3.right * 5.0f);
            MovePlayer(Vector3.zero);
            Assert.That(_runtime.CollectibleRuntimeData.CurrentScore, Is.EqualTo(10));
        }

        [Test]
        public void BeforeBind_ContactDoesNotCollect()
        {
            MovePlayer(Vector3.zero);
            Assert.That(_coin.IsBound, Is.False);
            AssertUncollected();
        }

        [Test]
        public void Bind_MissingRuntimeOrPlayer_DoesNotRegister()
        {
            Assert.That(_coin.Bind(null, _player, _scope), Is.False);
            Assert.That(_coin.Bind(_runtime, null, _scope), Is.False);
            Assert.That(_coin.IsBound, Is.False);
            Assert.That(_runtime.CollectibleRuntimeData.RegisteredCount, Is.Zero);
        }

        [Test]
        public void MultiplePlayerColliders_AwardOnlyOnce()
        {
            GameObject child = new GameObject("ChildCollider");
            child.transform.SetParent(_player.transform, false);
            child.AddComponent<BoxCollider>();
            Bind();
            MovePlayer(Vector3.zero);
            AssertCollected();
        }

        [Test]
        public void ChildColliderWithoutRootCollider_RecognizesRegisteredBody()
        {
            _player.GetComponent<Collider>().enabled = false;
            GameObject child = new GameObject("ChildCollider");
            child.transform.SetParent(_player.transform, false);
            child.AddComponent<BoxCollider>();
            Bind();
            MovePlayer(Vector3.zero);
            AssertCollected();
        }

        [Test]
        public void NonPlayerOnSameLayer_DoesNotCollect()
        {
            Bind();
            GameObject other = new GameObject("Other");
            SceneManager.MoveGameObjectToScene(other, _scene);
            other.AddComponent<BoxCollider>();
            Rigidbody body = other.AddComponent<Rigidbody>();
            body.useGravity = false;
            body.constraints = RigidbodyConstraints.FreezeAll;
            Simulate();
            AssertUncollected();
        }

        [Test]
        public void PlayerOnExcludedLayer_DoesNotCollect()
        {
            SetField("_playerLayers", (LayerMask)(1 << 6));
            Bind();
            MovePlayer(Vector3.zero);
            AssertUncollected();
        }

        [TestCase(E_GameState.None)]
        [TestCase(E_GameState.Initializing)]
        [TestCase(E_GameState.Ready)]
        [TestCase(E_GameState.Paused)]
        [TestCase(E_GameState.Ending)]
        [TestCase(E_GameState.Ended)]
        public void NonPlayingContact_DoesNotCollect(E_GameState state)
        {
            Bind();
            _runtime.SetGameState(state);
            MovePlayer(Vector3.zero);
            AssertUncollected();
        }

        [Test]
        public void ResumeWithDisabledPlayerCollider_DoesNotCollect()
        {
            Bind();
            _runtime.SetGameState(E_GameState.Paused);
            MovePlayer(Vector3.zero);
            _player.GetComponent<Collider>().enabled = false;
            _runtime.SetGameState(E_GameState.Playing);
            Assert.That(_coin.TryCollectOverlappingPlayer(), Is.False);
            AssertUncollected();
        }

        [Test]
        public void ResumeAfterPlayerLeaves_DoesNotReplayPausedContact()
        {
            Bind();
            _runtime.SetGameState(E_GameState.Paused);
            MovePlayer(Vector3.zero);
            MovePlayer(Vector3.right * 5.0f);
            _runtime.SetGameState(E_GameState.Playing);
            Assert.That(_coin.TryCollectOverlappingPlayer(), Is.False);
            AssertUncollected();
        }

        [Test]
        public void DisabledComponentAndReenable_DoNotRestoreBinding()
        {
            Bind();
            _coin.enabled = false;
            MovePlayer(Vector3.zero);
            Assert.That(_runtime.CollectibleRuntimeData.CurrentScore, Is.Zero);
            Assert.That(_coin.IsBound, Is.False);
            _coin.enabled = true;
            Simulate();
            Assert.That(_coin.TryCollectOverlappingPlayer(), Is.False);
            Assert.That(_coin.Bind(_runtime, _player, _scope), Is.False);
            Assert.That(_runtime.CollectibleRuntimeData.CurrentScore, Is.Zero);
        }

        [Test]
        public void DisabledTrigger_DoesNotCollectOrRecheck()
        {
            Bind();
            _trigger.enabled = false;
            MovePlayer(Vector3.zero);
            Assert.That(_coin.TryCollectOverlappingPlayer(), Is.False);
            Assert.That(_runtime.CollectibleRuntimeData.CurrentScore, Is.Zero);
        }

        [Test]
        public void ClearRuntime_RejectsLateContact()
        {
            Bind();
            CollectibleRuntimeData oldData = _runtime.CollectibleRuntimeData;
            _runtime.Clear();
            MovePlayer(Vector3.zero);
            Assert.That(oldData.CurrentScore, Is.Zero);
            Assert.That(_coin.TryCollectOverlappingPlayer(), Is.False);
        }

        [Test]
        public void ReleasedScope_RejectsLateContact()
        {
            Bind();
            Assert.That(_runtime.CollectibleRuntimeData.TryReleaseScope(_scope), Is.True);
            MovePlayer(Vector3.zero);
            AssertUncollected();
        }

        [Test]
        public void RebindNewScope_RestoresVisualAndAcquisition()
        {
            Bind();
            MovePlayer(Vector3.zero);
            AssertCollected();
            _coin.Unbind();
            _runtime.CollectibleRuntimeData.TryReleaseScope(_scope);
            _runtime.CollectibleRuntimeData.TryCreateScope(out long nextScope);
            Assert.That(_coin.Bind(_runtime, _player, nextScope), Is.True);
            Assert.That(_visual.enabled, Is.True);
            Assert.That(_trigger.enabled, Is.True);
            MovePlayer(Vector3.right * 5.0f);
            MovePlayer(Vector3.zero);
            Assert.That(_runtime.CollectibleRuntimeData.CurrentScore, Is.EqualTo(20));
        }

        [Test]
        public void DuplicateBind_DoesNotRestoreCollectedObject()
        {
            Bind();
            MovePlayer(Vector3.zero);
            Assert.That(_coin.Bind(_runtime, _player, _scope), Is.False);
            AssertCollected();
        }

        [Test]
        public void DestroyComponent_LeavesNoAcquisitionPath()
        {
            Bind();
            Object.DestroyImmediate(_coin);
            MovePlayer(Vector3.zero);
            Assert.That(_runtime.CollectibleRuntimeData.CurrentScore, Is.Zero);
            Assert.That(_trigger.enabled, Is.False);
        }

        [Test]
        public void Unbind_RejectsContactAndRecheck()
        {
            Bind();
            _coin.Unbind();
            _coin.Unbind();
            MovePlayer(Vector3.zero);
            Assert.That(_coin.IsBound, Is.False);
            Assert.That(_coin.TryCollectOverlappingPlayer(), Is.False);
            Assert.That(_runtime.CollectibleRuntimeData.CurrentScore, Is.Zero);
        }

        [Test]
        public void Bind_NonTriggerConfiguration_RejectsWithoutRegistration()
        {
            _trigger.isTrigger = false;
            Assert.That(_coin.Bind(_runtime, _player, _scope), Is.False);
            Assert.That(_runtime.CollectibleRuntimeData.RegisteredCount, Is.Zero);
        }

        [Test]
        public void Bind_NonSphereTrigger_RejectsWithoutRegistration()
        {
            Object.DestroyImmediate(_trigger);
            BoxCollider box = _coin.gameObject.AddComponent<BoxCollider>();
            box.isTrigger = true;
            _trigger = box;
            SetField("_triggerCollider", _trigger);

            Assert.That(_coin.Bind(_runtime, _player, _scope), Is.False);
            Assert.That(_runtime.CollectibleRuntimeData.RegisteredCount, Is.Zero);
        }

        private void Bind()
        {
            Assert.That(_coin.Bind(_runtime, _player, _scope), Is.True);
        }

        private void MovePlayer(Vector3 position)
        {
            _player.position = position;
            _player.WakeUp();
            Physics.SyncTransforms();
            Simulate();
        }

        private void Simulate()
        {
            _physicsScene.Simulate(0.02f);
            _physicsScene.Simulate(0.02f);
        }

        private void AssertCollected()
        {
            Assert.That(_runtime.CollectibleRuntimeData.CurrentScore, Is.EqualTo(10));
            Assert.That(_coin.IsCollected, Is.True);
            Assert.That(_visual.enabled, Is.False);
            Assert.That(_trigger.enabled, Is.False);
        }

        private void AssertUncollected()
        {
            Assert.That(_runtime.CollectibleRuntimeData.CurrentScore, Is.Zero);
            Assert.That(_coin.IsCollected, Is.False);
            Assert.That(_trigger.enabled, Is.True);
        }

        private void SetField(string name, object value)
        {
            FieldInfo field = typeof(ScoreCollectible).GetField(name,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            field.SetValue(_coin, value);
        }
    }
}
