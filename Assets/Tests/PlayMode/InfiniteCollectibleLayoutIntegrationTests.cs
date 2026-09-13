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
        [UnityTest]
        public IEnumerator ProductionPatterns_KeepEmptyPhase2RootsAndTwoScopes()
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
            Assert.That(firstPattern.CollectibleRoot.childCount, Is.Zero);
            Assert.That(secondPattern.CollectibleRoot.childCount, Is.Zero);
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
                Assert.That(firstCached.CollectibleRoot.childCount, Is.Zero);
                Assert.That(secondCached.CollectibleRoot.childCount, Is.Zero);
            }

            MonoBehaviour runtimeSystem = FindRuntimeDataSystem();
            PropertyInfo property = runtimeSystem.GetType().GetProperty(
                "RuntimeData", BindingFlags.Instance | BindingFlags.Public);
            Assert.That(property, Is.Not.Null);
            GameRuntimeData runtimeData =
                (GameRuntimeData)property.GetValue(runtimeSystem);
            Assert.That(runtimeData, Is.Not.Null);
            Assert.That(
                runtimeData.CollectibleRuntimeData.RegisteredCount, Is.Zero);
            Assert.That(
                runtimeData.CollectibleRuntimeData.ActiveScopeCount,
                Is.EqualTo(2));
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
