using System.Collections;
using System.Reflection;
using FlowState.Runtime.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class StageCollisionConfigurationTests
    {
        private const string SceneName = "SampleScene";
        private const string GroundLayerName = "Ground";

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
            yield return null;
        }

        [Test]
        public void StageTerrain_UsesCollisionSystemGroundLayer()
        {
            int groundLayer = LayerMask.NameToLayer(GroundLayerName);
            Assert.That(groundLayer, Is.GreaterThanOrEqualTo(0));

            AssertSolidGroundObject("Ground", groundLayer);
            AssertSolidGroundObject("Platform_01", groundLayer);
            AssertSolidGroundObject("Platform_02", groundLayer);

            MonoBehaviour collisionSystem = FindRequiredBehaviour(
                "Player",
                "CollisionSystem");
            LayerMask groundLayerMask = GetPrivateField<LayerMask>(
                collisionSystem,
                "_groundLayer");

            Assert.That(
                (groundLayerMask.value & (1 << groundLayer)),
                Is.Not.Zero);
            Assert.That(
                GetPrivateField<Collider>(
                    collisionSystem,
                    "_playerCollider"),
                Is.Not.Null);
            Assert.That(
                GetPrivateField<Transform>(
                    collisionSystem,
                    "_groundCheck"),
                Is.Not.Null);
        }

        [Test]
        public void Goal_UsesNonGroundTriggerWithoutVisualCollider()
        {
            int groundLayer = LayerMask.NameToLayer(GroundLayerName);
            GameObject goal = FindSceneGameObject("Goal");
            BoxCollider goalCollider = goal.GetComponent<BoxCollider>();
            GameObject goalVisual = FindSceneGameObject("GoalVisual");

            Assert.That(goal.layer, Is.Not.EqualTo(groundLayer));
            Assert.That(goalCollider, Is.Not.Null);
            Assert.That(goalCollider.isTrigger, Is.True);
            Assert.That(goalVisual.GetComponent<Collider>(), Is.Null);
        }

        [UnityTest]
        public IEnumerator StageTerrainSurfaces_ProvideActualGroundContact()
        {
            GameObject player = FindSceneGameObject("Player");
            Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
            Collider playerCollider = player.GetComponent<Collider>();
            MonoBehaviour collisionSystem = FindRequiredBehaviour(
                "Player",
                "CollisionSystem");
            string[] groundObjectNames =
            {
                "Ground",
                "Platform_01",
                "Platform_02"
            };

            Assert.That(playerRigidbody, Is.Not.Null);
            Assert.That(playerCollider, Is.Not.Null);

            foreach (string groundObjectName in groundObjectNames)
            {
                BoxCollider groundCollider = FindSceneGameObject(
                    groundObjectName).GetComponent<BoxCollider>();
                Vector3 groundCenter = groundCollider.bounds.center;
                float expectedSurfaceHeight = groundCollider.bounds.max.y;

                player.transform.position = new Vector3(
                    groundCenter.x,
                    expectedSurfaceHeight + playerCollider.bounds.extents.y,
                    groundCenter.z);
                playerRigidbody.linearVelocity = Vector3.zero;
                Physics.SyncTransforms();

                InvokeMethod(collisionSystem, "RefreshCollisionState");
                object collisionState = InvokeMethod(
                    collisionSystem,
                    "GetCollisionState");

                Assert.That(
                    GetProperty<bool>(collisionState, "IsGrounded"),
                    Is.True,
                    $"{groundObjectName} did not provide ground contact.");
                Assert.That(
                    GetProperty<Vector3>(collisionState, "ContactPoint").y,
                    Is.EqualTo(expectedSurfaceHeight).Within(0.03f),
                    $"{groundObjectName} returned an unexpected contact point.");
            }

            yield return null;
        }

        private void AssertSolidGroundObject(
            string gameObjectName,
            int groundLayer)
        {
            GameObject groundObject = FindSceneGameObject(gameObjectName);
            BoxCollider groundCollider =
                groundObject.GetComponent<BoxCollider>();

            Assert.That(groundObject.layer, Is.EqualTo(groundLayer));
            Assert.That(groundCollider, Is.Not.Null);
            Assert.That(groundCollider.isTrigger, Is.False);
        }

        private GameObject FindSceneGameObject(string gameObjectName)
        {
            GameObject[] gameObjects =
                Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.name == gameObjectName &&
                    gameObject.activeInHierarchy &&
                    gameObject.scene.IsValid() &&
                    gameObject.scene.isLoaded)
                {
                    return gameObject;
                }
            }

            Assert.Fail($"{gameObjectName} was not found in the loaded Scene.");
            return null;
        }

        private MonoBehaviour FindRequiredBehaviour(
            string gameObjectName,
            string typeName)
        {
            GameObject targetObject = FindSceneGameObject(gameObjectName);

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

        private T GetPrivateField<T>(
            MonoBehaviour targetBehaviour,
            string fieldName)
        {
            FieldInfo field = targetBehaviour.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null);
            return (T)field.GetValue(targetBehaviour);
        }

        private object InvokeMethod(
            MonoBehaviour targetBehaviour,
            string methodName)
        {
            MethodInfo method = targetBehaviour.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(method, Is.Not.Null);
            return method.Invoke(targetBehaviour, null);
        }

        private T GetProperty<T>(object target, string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(property, Is.Not.Null);
            return (T)property.GetValue(target);
        }
    }
}
