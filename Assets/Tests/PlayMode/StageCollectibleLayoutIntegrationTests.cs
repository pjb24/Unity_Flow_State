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
    public class StageCollectibleLayoutIntegrationTests
    {
        private const string SceneName = "SampleScene";
        private const float PositionTolerance = 0.02f;

        private static readonly Dictionary<string, Vector3> ExpectedLayout =
            new Dictionary<string, Vector3>
            {
                { "stage-jump-01-01", new Vector3(0.75f, 1.5f, 0.0f) },
                { "stage-jump-01-02", new Vector3(2.5f, 3.58f, 0.0f) },
                { "stage-jump-01-03", new Vector3(4.25f, 4.46f, 0.0f) },
                { "stage-jump-01-04", new Vector3(6.0f, 4.15f, 0.0f) },
                { "stage-jump-01-05", new Vector3(6.8f, 2.5f, 0.0f) },
                { "stage-jump-02-01", new Vector3(7.9f, 2.5f, 0.0f) },
                { "stage-jump-02-02", new Vector3(9.65f, 4.58f, 0.0f) },
                { "stage-jump-02-03", new Vector3(11.4f, 5.46f, 0.0f) },
                { "stage-jump-02-04", new Vector3(13.15f, 5.15f, 0.0f) },
                { "stage-jump-02-05", new Vector3(16.3f, 1.5f, 0.0f) }
            };

        [UnityTest]
        public IEnumerator ProductionStage_HasReachableConfiguredCollectibleLayout()
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

            GameObject stageRoot = GameObject.Find("StageModeRoot");
            Assert.That(stageRoot, Is.Not.Null);
            Transform collectibleRoot = stageRoot.transform.Find(
                "StageCollectible");
            Assert.That(collectibleRoot, Is.Not.Null);
            Assert.That(collectibleRoot.localPosition, Is.EqualTo(Vector3.zero));
            Assert.That(collectibleRoot.localRotation, Is.EqualTo(Quaternion.identity));
            Assert.That(collectibleRoot.localScale, Is.EqualTo(Vector3.one));
            ScoreCollectible[] collectibles =
                collectibleRoot.GetComponentsInChildren<ScoreCollectible>(true);
            Assert.That(collectibles, Has.Length.EqualTo(ExpectedLayout.Count));

            HashSet<string> foundIds = new HashSet<string>();

            foreach (ScoreCollectible collectible in collectibles)
            {
                string collectibleId = GetField<string>(
                    collectible,
                    "_collectibleId");
                Collider trigger = GetField<Collider>(
                    collectible,
                    "_triggerCollider");
                Renderer visual = GetField<Renderer>(collectible, "_visual");
                LayerMask playerLayers = GetField<LayerMask>(
                    collectible,
                    "_playerLayers");

                Assert.That(foundIds.Add(collectibleId), Is.True);
                Assert.That(ExpectedLayout.ContainsKey(collectibleId), Is.True);
                Assert.That(
                    Vector3.Distance(
                        collectible.transform.localPosition,
                        ExpectedLayout[collectibleId]),
                    Is.LessThanOrEqualTo(PositionTolerance));
                Assert.That(collectible.transform.parent, Is.SameAs(collectibleRoot));
                Assert.That(collectible.transform.localRotation, Is.EqualTo(Quaternion.identity));
                Assert.That(collectible.transform.localScale, Is.EqualTo(Vector3.one));
                Assert.That(collectible.gameObject.layer, Is.Zero);
                Assert.That(collectible.gameObject.tag, Is.EqualTo("Untagged"));
                Assert.That(trigger, Is.TypeOf<SphereCollider>());
                Assert.That(trigger.gameObject, Is.SameAs(collectible.gameObject));
                Assert.That(trigger.isTrigger, Is.True);
                Assert.That(visual, Is.Not.Null);
                Assert.That(visual.gameObject, Is.SameAs(collectible.gameObject));
                Assert.That(visual.sharedMaterial, Is.Not.Null);
                Assert.That(playerLayers.value, Is.EqualTo(1));
            }

            AssertJumpArc("stage-jump-01", 0.75f, 1.5f, 2.5f);
            AssertJumpArc("stage-jump-02", 7.9f, 2.5f, 1.5f);
            AssertOrderedAndNonOverlapping();
        }

        private void AssertOrderedAndNonOverlapping()
        {
            var orderedPoints = new List<Vector3>(ExpectedLayout.Values);
            orderedPoints.Sort((left, right) => left.x.CompareTo(right.x));

            for (int index = 0; index < orderedPoints.Count; index++)
            {
                Assert.That(orderedPoints[index].z, Is.Zero);
                Assert.That(orderedPoints[index].y, Is.GreaterThanOrEqualTo(1.5f));

                if (index == 0)
                {
                    continue;
                }

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

        private void AssertJumpArc(
            string idPrefix,
            float startX,
            float startY,
            float landingY)
        {
            const float horizontalSpeed = 8.0f;
            const float gravity = 25.0f;
            float verticalSpeed = PlayerMovementMath.CalculateJumpVerticalSpeed(
                3.0f,
                gravity);

            for (int index = 1; index <= 4; index++)
            {
                Vector3 point = ExpectedLayout[$"{idPrefix}-{index:00}"];
                float time = (point.x - startX) / horizontalSpeed;
                float expectedY = startY + verticalSpeed * time -
                                  0.5f * gravity * time * time;
                Assert.That(point.y, Is.EqualTo(expectedY).Within(0.2f));
            }

            Assert.That(
                ExpectedLayout[$"{idPrefix}-05"].y,
                Is.EqualTo(landingY).Within(PositionTolerance));
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
