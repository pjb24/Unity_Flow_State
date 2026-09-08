using FlowState.Runtime.Core;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class CollectibleRuntimeDataTests
    {
        private CollectibleRuntimeData _data;

        [SetUp]
        public void SetUp()
        {
            _data = new CollectibleRuntimeData();
        }

        [Test]
        public void BeforeInitialize_RejectsAllRequests()
        {
            Assert.That(_data.TryCreateScope(out _), Is.False);
            Assert.That(_data.TryRegister(1, "coin"), Is.False);
            Assert.That(_data.TryCollect(1, "coin"), Is.False);
            Assert.That(_data.TryReleaseScope(1), Is.False);
            Assert.That(_data.CurrentScore, Is.Zero);
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(int.MinValue)]
        public void Initialize_InvalidScore_RejectsConfiguration(int score)
        {
            Assert.That(_data.Initialize(score), Is.False);
            Assert.That(_data.IsInitialized, Is.False);
            Assert.That(_data.TryCreateScope(out _), Is.False);
        }

        [Test]
        public void Initialize_DefaultScore_StartsEmptyRun()
        {
            Assert.That(_data.Initialize(), Is.True);
            Assert.That(_data.ScorePerCollectible, Is.EqualTo(10));
            Assert.That(_data.CurrentScore, Is.Zero);
            Assert.That(_data.ActiveScopeCount, Is.Zero);
            Assert.That(_data.RegisteredCount, Is.Zero);
        }

        [Test]
        public void Initialize_AlreadyInitialized_PreservesRunAndConfiguration()
        {
            _data.Initialize();
            _data.TryCreateScope(out long scope);
            _data.TryRegister(scope, "coin");
            _data.TryCollect(scope, "coin");

            Assert.That(_data.Initialize(20), Is.False);
            Assert.That(_data.CurrentScore, Is.EqualTo(10));
            Assert.That(_data.ScorePerCollectible, Is.EqualTo(10));
            Assert.That(_data.TryCollect(scope, "coin"), Is.False);
        }

        [Test]
        public void TryCollect_MultipleIds_AwardsOncePerRegisteredId()
        {
            _data.Initialize();
            _data.TryCreateScope(out long scope);
            Assert.That(_data.TryRegister(scope, "first"), Is.True);
            Assert.That(_data.TryRegister(scope, "second"), Is.True);
            Assert.That(_data.CurrentScore, Is.Zero);
            Assert.That(_data.TryCollect(scope, "first"), Is.True);
            Assert.That(_data.CurrentScore, Is.EqualTo(10));
            Assert.That(_data.TryCollect(scope, "first"), Is.False);
            Assert.That(_data.TryRegister(scope, "first"), Is.False);
            Assert.That(_data.TryCollect(scope, "second"), Is.True);
            Assert.That(_data.CurrentScore, Is.EqualTo(20));
            Assert.That(_data.RegisteredCount, Is.EqualTo(2));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("\t")]
        public void InvalidId_CannotRegisterOrCollect(string id)
        {
            _data.Initialize();
            _data.TryCreateScope(out long scope);
            Assert.That(_data.TryRegister(scope, id), Is.False);
            Assert.That(_data.TryCollect(scope, id), Is.False);
            Assert.That(_data.RegisteredCount, Is.Zero);
            Assert.That(_data.CurrentScore, Is.Zero);
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(long.MaxValue)]
        public void UnknownScope_RejectsRequests(long scope)
        {
            _data.Initialize();
            Assert.That(_data.TryRegister(scope, "coin"), Is.False);
            Assert.That(_data.TryCollect(scope, "coin"), Is.False);
            Assert.That(_data.TryReleaseScope(scope), Is.False);
            Assert.That(_data.CurrentScore, Is.Zero);
        }

        [Test]
        public void TryCollect_UnregisteredId_DoesNotAwardScore()
        {
            _data.Initialize();
            _data.TryCreateScope(out long scope);
            Assert.That(_data.TryCollect(scope, "missing"), Is.False);
            Assert.That(_data.CurrentScore, Is.Zero);
            Assert.That(_data.RegisteredCount, Is.Zero);
        }

        [Test]
        public void TryRegister_DuplicateBeforeCollection_PreservesOriginal()
        {
            _data.Initialize();
            _data.TryCreateScope(out long scope);
            Assert.That(_data.TryRegister(scope, "coin"), Is.True);
            Assert.That(_data.TryRegister(scope, "coin"), Is.False);
            Assert.That(_data.TryCollect(scope, "coin"), Is.True);
            Assert.That(_data.CurrentScore, Is.EqualTo(10));
        }

        [Test]
        public void TryCollect_SameLocalIdInDifferentScopes_IsIndependent()
        {
            _data.Initialize();
            _data.TryCreateScope(out long first);
            _data.TryCreateScope(out long second);
            Assert.That(first, Is.Not.EqualTo(second));
            _data.TryRegister(first, "coin");
            _data.TryRegister(second, "coin");
            Assert.That(_data.TryCollect(first, "coin"), Is.True);
            Assert.That(_data.TryCollect(second, "coin"), Is.True);
            Assert.That(_data.TryCollect(first, "coin"), Is.False);
            Assert.That(_data.CurrentScore, Is.EqualTo(20));
        }

        [Test]
        public void ReleaseScope_RemovesCollectedAndMissedIds_PreservesScore()
        {
            _data.Initialize();
            _data.TryCreateScope(out long scope);
            _data.TryRegister(scope, "collected");
            _data.TryRegister(scope, "missed");
            _data.TryCollect(scope, "collected");

            Assert.That(_data.TryReleaseScope(scope), Is.True);
            Assert.That(_data.TryReleaseScope(scope), Is.False);
            Assert.That(_data.ActiveScopeCount, Is.Zero);
            Assert.That(_data.RegisteredCount, Is.Zero);
            Assert.That(_data.CurrentScore, Is.EqualTo(10));
            Assert.That(_data.TryRegister(scope, "new"), Is.False);
            Assert.That(_data.TryCollect(scope, "missed"), Is.False);

            _data.TryCreateScope(out long nextScope);
            Assert.That(nextScope, Is.Not.EqualTo(scope));
            _data.TryRegister(nextScope, "collected");
            Assert.That(_data.TryCollect(nextScope, "collected"), Is.True);
            Assert.That(_data.CurrentScore, Is.EqualTo(20));
        }

        [Test]
        public void RepeatedScopeReplacement_KeepsOnlyActiveRegistrations()
        {
            _data.Initialize();
            _data.TryCreateScope(out long retainedScope);
            _data.TryRegister(retainedScope, "retained");
            for (int index = 0; index < 100; index++)
            {
                Assert.That(_data.TryCreateScope(out long scope), Is.True);
                Assert.That(_data.TryRegister(scope, "coin"), Is.True);
                Assert.That(_data.TryCollect(scope, "coin"), Is.True);
                Assert.That(_data.TryReleaseScope(scope), Is.True);
                Assert.That(_data.ActiveScopeCount, Is.EqualTo(1));
                Assert.That(_data.RegisteredCount, Is.EqualTo(1));
            }

            Assert.That(_data.CurrentScore, Is.EqualTo(1000));
            Assert.That(_data.TryCollect(retainedScope, "retained"), Is.True);
            Assert.That(_data.CurrentScore, Is.EqualTo(1010));
        }

        [TestCase(1073741824)]
        [TestCase(int.MaxValue)]
        public void TryCollect_ScoreOverflow_SaturatesAndStillConsumesId(int score)
        {
            _data.Initialize(score);
            _data.TryCreateScope(out long scope);
            _data.TryRegister(scope, "first");
            _data.TryRegister(scope, "second");
            Assert.That(_data.TryCollect(scope, "first"), Is.True);
            Assert.That(_data.CurrentScore, Is.EqualTo(score));
            Assert.That(_data.TryCollect(scope, "second"), Is.True);
            Assert.That(_data.CurrentScore, Is.EqualTo(int.MaxValue));
            Assert.That(_data.TryCollect(scope, "second"), Is.False);
        }

        [Test]
        public void ClearAndInitialize_RemovesRunAndRejectsOldScope()
        {
            _data.Initialize();
            _data.TryCreateScope(out long previousScope);
            _data.TryRegister(previousScope, "coin");
            _data.TryCollect(previousScope, "coin");
            _data.Clear();
            _data.Clear();

            Assert.That(_data.IsInitialized, Is.False);
            Assert.That(_data.CurrentScore, Is.Zero);
            Assert.That(_data.RegisteredCount, Is.Zero);
            Assert.That(_data.ActiveScopeCount, Is.Zero);
            Assert.That(_data.TryCollect(previousScope, "coin"), Is.False);
            Assert.That(_data.Initialize(), Is.True);
            _data.TryCreateScope(out long nextScope);
            Assert.That(nextScope, Is.Not.EqualTo(previousScope));
            _data.TryRegister(nextScope, "coin");
            Assert.That(_data.TryCollect(previousScope, "coin"), Is.False);
            Assert.That(_data.TryCollect(nextScope, "coin"), Is.True);
            Assert.That(_data.CurrentScore, Is.EqualTo(10));
        }
    }
}
