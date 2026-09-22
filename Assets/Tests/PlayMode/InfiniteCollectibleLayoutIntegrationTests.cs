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
    public class InfiniteCollectibleLayoutIntegrationTests
    {
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
            ProductionSceneGameModeTestUtility.RestartInMode(E_GameMode.Infinite);
            yield return new WaitForFixedUpdate();

            InfiniteMapPattern map = FindMap();
            MonoBehaviour infiniteMode = FindBehaviour(
                "InfiniteModeSystem", "InfiniteModeSystem");
            infiniteMode.enabled = false;
            Assert.That(map.ResetPatterns(), Is.True);
        }

        [Test]
        public void ProductionPatterns_MatchPhase4LayoutAndTwoScopes()
        {

            InfinitePatternSlot first = GameObject.Find("Slot_0")
                ?.GetComponent<InfinitePatternSlot>();
            InfinitePatternSlot second = GameObject.Find("Slot_1")
                ?.GetComponent<InfinitePatternSlot>();
            Assert.That(first, Is.Not.Null);
            Assert.That(second, Is.Not.Null);
            Assert.That(first.PatternInstanceCount, Is.EqualTo(4));
            Assert.That(second.PatternInstanceCount, Is.EqualTo(4));
            Assert.That(first.TryGetCurrentPattern(
                out InfinitePatternAuthoring firstPattern), Is.True);
            Assert.That(second.TryGetCurrentPattern(
                out InfinitePatternAuthoring secondPattern), Is.True);
            AssertPatternLayout(firstPattern);
            AssertPatternLayout(secondPattern);
            string[] patternIds =
            {
                InfinitePatternCatalogFactory.FlatId,
                InfinitePatternCatalogFactory.SingleRiseId,
                InfinitePatternCatalogFactory.LegacyStepsId,
                InfinitePatternCatalogFactory.InternalGapId
            };
            for (int index = 0; index < patternIds.Length; index++)
            {
                Assert.That(first.TryGetPatternInstance(
                    patternIds[index], out InfinitePatternAuthoring firstCached),
                    Is.True);
                Assert.That(second.TryGetPatternInstance(
                    patternIds[index], out InfinitePatternAuthoring secondCached),
                    Is.True);
                Assert.That(firstCached, Is.Not.SameAs(secondCached));
                AssertPatternLayout(firstCached);
                AssertPatternLayout(secondCached);
            }

            MonoBehaviour runtimeSystem = FindRuntimeDataSystem();
            PropertyInfo property = runtimeSystem.GetType().GetProperty(
                "RuntimeData", BindingFlags.Instance | BindingFlags.Public);
            Assert.That(property, Is.Not.Null);
            GameRuntimeData runtimeData =
                (GameRuntimeData)property.GetValue(runtimeSystem);
            Assert.That(runtimeData, Is.Not.Null);
            Assert.That(
                runtimeData.CollectibleRuntimeData.RegisteredCount,
                Is.EqualTo(firstPattern.CollectibleRoot.childCount +
                           secondPattern.CollectibleRoot.childCount));
            Assert.That(
                runtimeData.CollectibleRuntimeData.ActiveScopeCount,
                Is.EqualTo(2));
        }

        [UnityTest]
        public IEnumerator InitialFlat_OverlappingPlayerCollectsOnceAndDisablesPresentation()
        {
            InfinitePatternSlot first = FindSlot("Slot_0");
            Assert.That(first.TryGetCurrentPattern(
                out InfinitePatternAuthoring pattern), Is.True);
            ScoreCollectible coin = pattern.CollectibleRoot
                .Find("entry-land-01").GetComponent<ScoreCollectible>();
            Rigidbody player = GameObject.Find("Player")
                ?.GetComponent<Rigidbody>();
            Assert.That(player, Is.Not.Null);
            Assert.That(coin.IsBound, Is.True);
            CollectibleRuntimeData data = GetRuntimeData()
                .CollectibleRuntimeData;

            player.position = coin.transform.position;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            Assert.That(coin.IsCollected, Is.True);
            Assert.That(data.CurrentScore, Is.EqualTo(10));
            Assert.That(coin.GetComponent<SphereCollider>().enabled, Is.False);
            Assert.That(coin.GetComponent<Renderer>().enabled, Is.False);
            Assert.That(coin.TryCollectOverlappingPlayer(), Is.False);
            Assert.That(data.CurrentScore, Is.EqualTo(10));
        }

        [UnityTest]
        public IEnumerator InitialFlat_PhysicalTriggerAwardsScore()
        {
            InfinitePatternSlot first = FindSlot("Slot_0");
            Assert.That(first.TryGetCurrentPattern(
                out InfinitePatternAuthoring pattern), Is.True);
            ScoreCollectible coin = pattern.CollectibleRoot
                .Find("entry-land-01").GetComponent<ScoreCollectible>();
            Rigidbody player = GameObject.Find("Player")
                ?.GetComponent<Rigidbody>();
            Assert.That(player, Is.Not.Null);
            player.position = coin.transform.position + Vector3.left * 2.0f;
            player.linearVelocity = new Vector3(
                GetPlayerMoveSpeed(), 0.0f, 0.0f);
            Physics.SyncTransforms();

            for (int step = 0; step < 30 && !coin.IsCollected; step++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.That(coin.IsCollected, Is.True);
            Assert.That(GetRuntimeData().CollectibleRuntimeData.CurrentScore,
                Is.EqualTo(10));
        }

        [Test]
        public void MissedCoins_DoNotBlockAdvanceOrNewScopeRegistration()
        {
            InfiniteMapPattern map = FindMap();
            InfinitePatternSlot first = FindSlot("Slot_0");
            InfinitePatternSlot second = FindSlot("Slot_1");
            Rigidbody player = GameObject.Find("Player")
                ?.GetComponent<Rigidbody>();
            Assert.That(player, Is.Not.Null);
            CollectibleRuntimeData data = GetRuntimeData()
                .CollectibleRuntimeData;

            Assert.That(map.TryRequestNextPattern(
                1, InfinitePatternCatalogFactory.SingleRiseId), Is.True);
            Assert.That(first.TryGetCurrentPattern(
                out InfinitePatternAuthoring previous), Is.True);
            player.position = new Vector3(
                previous.EndAnchor.position.x + 2.0f, 1.5f, 0.0f);
            Physics.SyncTransforms();
            Assert.That(map.TryAdvance(second.SlotId), Is.True);
            Assert.That(map.AdvanceCount, Is.EqualTo(1));
            Assert.That(data.CurrentScore, Is.Zero);
            Assert.That(data.ActiveScopeCount, Is.EqualTo(2));
            Assert.That(data.RegisteredCount, Is.EqualTo(5 + 15));
            Assert.That(first.TryGetCurrentPattern(
                out InfinitePatternAuthoring replacement), Is.True);
            Assert.That(replacement.PatternId,
                Is.EqualTo(InfinitePatternCatalogFactory.SingleRiseId));
            Assert.That(replacement.CollectibleRoot
                .Find("entry-land-01")
                .GetComponent<ScoreCollectible>().IsBound, Is.True);
        }

        [UnityTest]
        public IEnumerator PatternReuse_PreservesScoreAndRestoresPickupInNewScope()
        {
            InfiniteMapPattern map = FindMap();
            InfinitePatternSlot first = FindSlot("Slot_0");
            InfinitePatternSlot second = FindSlot("Slot_1");
            Rigidbody player = GameObject.Find("Player")
                ?.GetComponent<Rigidbody>();
            Assert.That(player, Is.Not.Null);
            CollectibleRuntimeData data = GetRuntimeData()
                .CollectibleRuntimeData;
            int distanceScoreBefore = GetRuntimeData()
                .InfiniteModeRuntimeData.CurrentScore;
            Assert.That(first.TryGetCurrentPattern(
                out InfinitePatternAuthoring flat), Is.True);
            ScoreCollectible oldCoin = flat.CollectibleRoot
                .Find("entry-land-01").GetComponent<ScoreCollectible>();
            player.position = oldCoin.transform.position;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            Assert.That(oldCoin.IsCollected, Is.True);
            Assert.That(data.CurrentScore, Is.EqualTo(10));

            Assert.That(map.TryRequestNextPattern(
                1, InfinitePatternCatalogFactory.SingleRiseId), Is.True);
            player.position = new Vector3(
                flat.EndAnchor.position.x + 2.0f, 1.5f, 0.0f);
            Physics.SyncTransforms();
            Assert.That(map.TryAdvance(second.SlotId), Is.True);
            Assert.That(oldCoin.IsBound, Is.False);
            Assert.That(data.CurrentScore, Is.EqualTo(10));
            Assert.That(GetRuntimeData().InfiniteModeRuntimeData.CurrentScore,
                Is.EqualTo(distanceScoreBefore),
                "Pattern passage must not award Distance Score.");
            Assert.That(first.TryGetCurrentPattern(
                out InfinitePatternAuthoring rise), Is.True);
            ScoreCollectible newCoin = rise.CollectibleRoot
                .Find("entry-land-01").GetComponent<ScoreCollectible>();
            Assert.That(newCoin.IsBound, Is.True);
            player.position = newCoin.transform.position;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            Assert.That(newCoin.IsCollected, Is.True);
            Assert.That(data.CurrentScore, Is.EqualTo(20));
            Assert.That(ScoreRecord.TryCalculateTotalScore(
                GetRuntimeData().InfiniteModeRuntimeData.CurrentScore,
                data.CurrentScore, out int total), Is.True);
            Assert.That(total,
                Is.EqualTo(GetRuntimeData()
                    .InfiniteModeRuntimeData.CurrentScore + 20));
        }

        [UnityTest]
        public IEnumerator FourProductionPatterns_RebindAndAllowPickupAfterAdvance()
        {
            string[] ids =
            {
                InfinitePatternCatalogFactory.FlatId,
                InfinitePatternCatalogFactory.SingleRiseId,
                InfinitePatternCatalogFactory.LegacyStepsId,
                InfinitePatternCatalogFactory.InternalGapId
            };
            InfiniteMapPattern map = FindMap();
            InfinitePatternSlot[] slots =
            {
                FindSlot("Slot_0"),
                FindSlot("Slot_1")
            };
            Rigidbody player = GameObject.Find("Player")
                ?.GetComponent<Rigidbody>();
            Assert.That(player, Is.Not.Null);
            CollectibleRuntimeData data = GetRuntimeData()
                .CollectibleRuntimeData;

            for (int index = 0; index < ids.Length; index++)
            {
                InfinitePatternSlot reused = slots[index % 2];
                InfinitePatternSlot front = slots[(index + 1) % 2];
                Assert.That(reused.TryGetCurrentPattern(
                    out InfinitePatternAuthoring old), Is.True);
                Assert.That(map.TryRequestNextPattern(index + 1, ids[index]),
                    Is.True);
                player.position = new Vector3(
                    old.EndAnchor.position.x + 2.0f, 1.5f, 0.0f);
                Physics.SyncTransforms();
                Assert.That(map.TryAdvance(front.SlotId), Is.True);
                Assert.That(reused.TryGetCurrentPattern(
                    out InfinitePatternAuthoring current), Is.True);
                Assert.That(current.PatternId, Is.EqualTo(ids[index]));
                ScoreCollectible coin = current.CollectibleRoot
                    .Find("entry-land-01")
                    .GetComponent<ScoreCollectible>();
                Assert.That(coin.IsBound, Is.True);
                player.position = coin.transform.position;
                Physics.SyncTransforms();
                yield return new WaitForFixedUpdate();
                Assert.That(coin.IsCollected, Is.True);
                Assert.That(data.CurrentScore, Is.EqualTo((index + 1) * 10));
                Assert.That(data.ActiveScopeCount, Is.EqualTo(2));
            }
        }

        [UnityTest]
        public IEnumerator PauseResumeAndRetry_PreserveThenResetInfinitePickup()
        {
            InfinitePatternSlot first = FindSlot("Slot_0");
            Assert.That(first.TryGetCurrentPattern(
                out InfinitePatternAuthoring pattern), Is.True);
            ScoreCollectible coin = pattern.CollectibleRoot
                .Find("entry-land-01").GetComponent<ScoreCollectible>();
            Rigidbody player = GameObject.Find("Player")
                ?.GetComponent<Rigidbody>();
            Assert.That(player, Is.Not.Null);
            MonoBehaviour gameSystem = FindBehaviour(
                "GameSystem", "GameSystem");
            CollectibleRuntimeData previous = GetRuntimeData()
                .CollectibleRuntimeData;

            Assert.That(InvokeBool(gameSystem, "PauseGame"), Is.True);
            player.position = coin.transform.position;
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            Assert.That(coin.TryCollectOverlappingPlayer(), Is.False);
            Assert.That(previous.CurrentScore, Is.Zero);
            Assert.That(InvokeBool(gameSystem, "ResumeGame"), Is.True);
            Assert.That(coin.IsCollected, Is.True);
            Assert.That(previous.CurrentScore, Is.EqualTo(10));

            Invoke(gameSystem, "EndGame");
            Assert.That(InvokeBool(gameSystem, "RetryGame"), Is.True);
            CollectibleRuntimeData next = GetRuntimeData()
                .CollectibleRuntimeData;
            Assert.That(next, Is.Not.SameAs(previous));
            Assert.That(next.CurrentScore, Is.Zero);
            Assert.That(next.ActiveScopeCount, Is.EqualTo(2));
            Assert.That(next.RegisteredCount, Is.EqualTo(10));
            Assert.That(first.TryGetCurrentPattern(
                out InfinitePatternAuthoring restored), Is.True);
            Assert.That(restored.PatternId,
                Is.EqualTo(InfinitePatternCatalogFactory.FlatId));
            Assert.That(restored.CollectibleRoot.Find("entry-land-01")
                .GetComponent<ScoreCollectible>().IsCollected, Is.False);
        }

        private static bool InvokeBool(MonoBehaviour target, string methodName)
        {
            return (bool)Invoke(target, methodName);
        }

        private static object Invoke(MonoBehaviour target, string methodName)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName, BindingFlags.Instance | BindingFlags.Public);
            Assert.That(method, Is.Not.Null);
            return method.Invoke(target, null);
        }

        private static InfiniteMapPattern FindMap()
        {
            InfiniteMapPattern map = GameObject.Find("InfiniteMapPattern")
                ?.GetComponent<InfiniteMapPattern>();
            Assert.That(map, Is.Not.Null);
            return map;
        }

        private static InfinitePatternSlot FindSlot(string name)
        {
            InfinitePatternSlot slot = GameObject.Find(name)
                ?.GetComponent<InfinitePatternSlot>();
            Assert.That(slot, Is.Not.Null);
            return slot;
        }

        private static GameRuntimeData GetRuntimeData()
        {
            MonoBehaviour runtimeSystem = FindRuntimeDataSystem();
            PropertyInfo property = runtimeSystem.GetType().GetProperty(
                "RuntimeData", BindingFlags.Instance | BindingFlags.Public);
            Assert.That(property, Is.Not.Null);
            return (GameRuntimeData)property.GetValue(runtimeSystem);
        }

        private static MonoBehaviour FindBehaviour(
            string ownerName, string typeName)
        {
            GameObject owner = GameObject.Find(ownerName);
            Assert.That(owner, Is.Not.Null);
            MonoBehaviour[] behaviours = owner.GetComponents<MonoBehaviour>();
            for (int index = 0; index < behaviours.Length; index++)
            {
                if (behaviours[index] != null &&
                    behaviours[index].GetType().Name == typeName)
                {
                    return behaviours[index];
                }
            }

            Assert.Fail(typeName + " was not found.");
            return null;
        }

        private static float GetPlayerMoveSpeed()
        {
            MonoBehaviour movementSystem = FindBehaviour(
                "PlayerMovementSystem", "PlayerMovementSystem");
            return GetField<float>(movementSystem, "_moveSpeed");
        }

        private static void AssertPatternLayout(InfinitePatternAuthoring pattern)
        {
            Assert.That(pattern, Is.Not.Null);
            Assert.That(InfiniteCollectibleLayout.TryGetCount(
                pattern.PatternId, out int expectedCount), Is.True);
            Assert.That(pattern.CollectibleRoot.childCount,
                Is.EqualTo(expectedCount), pattern.PatternId);

            for (int index = 0; index < expectedCount; index++)
            {
                Assert.That(InfiniteCollectibleLayout.TryGetPoint(
                    pattern.PatternId, index, out string id,
                    out Vector3 position), Is.True);
                Transform child = pattern.CollectibleRoot.Find(id);
                Assert.That(child, Is.Not.Null, pattern.PatternId + "/" + id);
                Assert.That(child.parent, Is.SameAs(pattern.CollectibleRoot));
                Assert.That(child.localPosition.x,
                    Is.EqualTo(position.x).Within(0.011f));
                Assert.That(child.localPosition.y,
                    Is.EqualTo(position.y).Within(0.011f));
                Assert.That(child.localPosition.z,
                    Is.EqualTo(position.z).Within(0.011f));
                Assert.That(child.gameObject.layer, Is.Zero);
                SphereCollider trigger = child.GetComponent<SphereCollider>();
                Renderer visual = child.GetComponent<Renderer>();
                ScoreCollectible collectible =
                    child.GetComponent<ScoreCollectible>();
                Assert.That(trigger, Is.Not.Null);
                Assert.That(trigger.isTrigger, Is.True);
                Assert.That(trigger.radius,
                    Is.EqualTo(InfiniteCollectibleLayout.TriggerRadius));
                Assert.That(visual, Is.Not.Null);
                Assert.That(collectible, Is.Not.Null);
                Assert.That(GetField<string>(collectible, "_collectibleId"),
                    Is.EqualTo(id));
                Assert.That(GetField<Collider>(
                    collectible, "_triggerCollider"), Is.SameAs(trigger));
                Assert.That(GetField<Renderer>(
                    collectible, "_visual"), Is.SameAs(visual));
                Assert.That(GetField<LayerMask>(
                    collectible, "_playerLayers").value, Is.EqualTo(1));
            }
        }

        private static T GetField<T>(object target, string fieldName)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            return (T)field.GetValue(target);
        }

        private static MonoBehaviour FindRuntimeDataSystem()
        {
            GameObject owner = GameObject.Find("RuntimeDataSystem");
            Assert.That(owner, Is.Not.Null);
            MonoBehaviour[] behaviours = owner.GetComponents<MonoBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] != null &&
                    behaviours[i].GetType().Name == "RuntimeDataSystem")
                {
                    return behaviours[i];
                }
            }

            Assert.Fail("RuntimeDataSystem was not found.");
            return null;
        }
    }
}
