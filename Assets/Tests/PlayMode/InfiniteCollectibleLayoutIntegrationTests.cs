using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FlowState.Tests.PlayMode
{
    public class InfiniteCollectibleLayoutIntegrationTests
    {
        private const string SceneName = "SampleScene";
        private const float PositionTolerance = 0.02f;

        private static readonly Dictionary<string, Vector3> ExpectedLayout =
            new Dictionary<string, Vector3>
            {
                { "jump-01-01", new Vector3(0.75f, 1.5f, 0.0f) },
                { "jump-01-02", new Vector3(2.5f, 3.58f, 0.0f) },
                { "jump-01-03", new Vector3(4.25f, 4.46f, 0.0f) },
                { "jump-01-04", new Vector3(6.0f, 4.15f, 0.0f) },
                { "jump-01-05", new Vector3(6.8f, 2.5f, 0.0f) },
                { "jump-02-01", new Vector3(7.9f, 2.5f, 0.0f) },
                { "jump-02-02", new Vector3(9.65f, 4.58f, 0.0f) },
                { "jump-02-03", new Vector3(11.4f, 5.46f, 0.0f) },
                { "jump-02-04", new Vector3(13.15f, 5.15f, 0.0f) },
                { "jump-02-05", new Vector3(16.3f, 1.5f, 0.0f) }
            };

        [UnityTest]
        public IEnumerator ProductionPatterns_HaveIndependentConfiguredLayouts()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(
                SceneName,
                LoadSceneMode.Single);

            while (!loadOperation.isDone)
            {
                yield return null;
            }

            yield return null;
            ProductionSceneGameModeTestUtility.RestartInMode(E_GameMode.Infinite);
            yield return null;

            GameObject mapObject = GameObject.Find("InfiniteMapPattern");
            Assert.That(mapObject, Is.Not.Null);
            Transform firstPattern = mapObject.transform.Find("Pattern_0");
            Transform secondPattern = mapObject.transform.Find("Pattern_1");
            AssertPattern(firstPattern);
            AssertPattern(secondPattern);
            AssertOrderedAndNonOverlapping();

            MonoBehaviour runtimeSystem = FindRequiredBehaviour(
                "RuntimeDataSystem",
                "RuntimeDataSystem");
            GameRuntimeData runtimeData = GetProperty<GameRuntimeData>(
                runtimeSystem,
                "RuntimeData");
            Assert.That(
                runtimeData.CollectibleRuntimeData.RegisteredCount,
                Is.EqualTo(ExpectedLayout.Count * 2));
        }

        private void AssertOrderedAndNonOverlapping()
        {
            var orderedPoints = new List<Vector3>(ExpectedLayout.Values);
            orderedPoints.Sort((left, right) => left.x.CompareTo(right.x));

            for (int index = 1; index < orderedPoints.Count; index++)
            {
                Assert.That(
                    orderedPoints[index].x,
                    Is.GreaterThan(orderedPoints[index - 1].x));
                Assert.That(
                    Vector3.Distance(
                        orderedPoints[index],
                        orderedPoints[index - 1]),
                    Is.GreaterThanOrEqualTo(1.0f));
            }
        }

        private void AssertPattern(Transform pattern)
        {
            Assert.That(pattern, Is.Not.Null);
            Transform collectibleRoot = pattern.Find("PatternCollectible");
            Assert.That(collectibleRoot, Is.Not.Null);
            Assert.That(collectibleRoot.localPosition, Is.EqualTo(Vector3.zero));
            Assert.That(collectibleRoot.localRotation, Is.EqualTo(Quaternion.identity));
            Assert.That(collectibleRoot.localScale, Is.EqualTo(Vector3.one));

            ScoreCollectible[] collectibles =
                collectibleRoot.GetComponentsInChildren<ScoreCollectible>(true);
            Assert.That(collectibles, Has.Length.EqualTo(ExpectedLayout.Count));
            HashSet<string> ids = new HashSet<string>();

            foreach (ScoreCollectible collectible in collectibles)
            {
                string id = GetField<string>(collectible, "_collectibleId");
                Collider trigger = GetField<Collider>(collectible, "_triggerCollider");
                Renderer visual = GetField<Renderer>(collectible, "_visual");
                LayerMask playerLayers = GetField<LayerMask>(
                    collectible,
                    "_playerLayers");

                Assert.That(ids.Add(id), Is.True);
                Assert.That(ExpectedLayout.ContainsKey(id), Is.True);
                Assert.That(
                    Vector3.Distance(
                        collectible.transform.localPosition,
                        ExpectedLayout[id]),
                    Is.LessThanOrEqualTo(PositionTolerance));
                Assert.That(collectible.transform.parent, Is.SameAs(collectibleRoot));
                Assert.That(collectible.gameObject.layer, Is.Zero);
                Assert.That(trigger, Is.TypeOf<SphereCollider>());
                Assert.That(trigger.gameObject, Is.SameAs(collectible.gameObject));
                Assert.That(trigger.isTrigger, Is.True);
                Assert.That(visual, Is.Not.Null);
                Assert.That(visual.gameObject, Is.SameAs(collectible.gameObject));
                Assert.That(playerLayers.value, Is.EqualTo(1));
            }
        }

        private MonoBehaviour FindRequiredBehaviour(
            string objectName,
            string typeName)
        {
            GameObject owner = GameObject.Find(objectName);
            Assert.That(owner, Is.Not.Null);

            foreach (MonoBehaviour behaviour in owner.GetComponents<MonoBehaviour>())
            {
                if (behaviour != null && behaviour.GetType().Name == typeName)
                {
                    return behaviour;
                }
            }

            Assert.Fail($"{typeName} was not found on {objectName}.");
            return null;
        }

        private T GetProperty<T>(MonoBehaviour target, string propertyName)
        {
            PropertyInfo property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);
            Assert.That(property, Is.Not.Null);
            return (T)property.GetValue(target);
        }

        private T GetField<T>(ScoreCollectible target, string fieldName)
        {
            FieldInfo field = typeof(ScoreCollectible).GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            return (T)field.GetValue(target);
        }
    }
}
