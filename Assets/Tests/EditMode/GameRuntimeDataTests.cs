using FlowState.Runtime.Core;
using FlowState.Runtime.Features;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class GameRuntimeDataTests
    {
        private GameRuntimeData _runtimeData;

        [SetUp]
        public void SetUp()
        {
            _runtimeData = new GameRuntimeData();
        }

        [Test]
        public void Initialize_WithoutMode_UsesStageMode()
        {
            _runtimeData.Initialize();

            Assert.That(_runtimeData.GameMode, Is.EqualTo(E_GameMode.Stage));
            Assert.That(_runtimeData.InfiniteModeRuntimeData, Is.Null);
            Assert.That(_runtimeData.IsCreated, Is.True);
        }

        [Test]
        public void Initialize_InfiniteMode_StoresCurrentRunMode()
        {
            _runtimeData.Initialize(E_GameMode.Infinite);

            Assert.That(_runtimeData.GameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_runtimeData.InfiniteModeRuntimeData, Is.Not.Null);
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData.IsInitialized,
                Is.True);
            Assert.That(_runtimeData.InfiniteModeRuntimeData.CurrentDistance, Is.Zero);
            Assert.That(_runtimeData.InfiniteModeRuntimeData.CurrentScore, Is.Zero);
            Assert.That(_runtimeData.InfiniteModeRuntimeData.IsFinalized, Is.False);
            Assert.That(_runtimeData.IsCreated, Is.True);
        }

        [Test]
        public void Clear_CreatedData_ResetsRuntimeState()
        {
            _runtimeData.Initialize(E_GameMode.Infinite);
            _runtimeData.SetGameState(E_GameState.Playing);
            _runtimeData.SetUIState(E_UIState.StageHud);

            _runtimeData.Clear();

            Assert.That(_runtimeData.GameMode, Is.EqualTo(E_GameMode.Stage));
            Assert.That(_runtimeData.GameState, Is.EqualTo(E_GameState.None));
            Assert.That(_runtimeData.UIState, Is.EqualTo(E_UIState.None));
            Assert.That(_runtimeData.PlayerMovementRuntimeData, Is.Null);
            Assert.That(_runtimeData.InfiniteModeRuntimeData, Is.Null);
            Assert.That(_runtimeData.IsCreated, Is.False);
        }

        [Test]
        public void SetStates_BeforeInitialize_DoesNotMutateState()
        {
            _runtimeData.SetGameState(E_GameState.Playing);
            _runtimeData.SetUIState(E_UIState.Result);

            Assert.That(_runtimeData.GameState, Is.EqualTo(E_GameState.None));
            Assert.That(_runtimeData.UIState, Is.EqualTo(E_UIState.None));
            Assert.That(_runtimeData.IsCreated, Is.False);
        }

        [Test]
        public void SetGameState_Paused_PreservesStageMovementRuntimeData()
        {
            _runtimeData.Initialize(E_GameMode.Stage);
            PlayerMovementRuntimeData movementData =
                _runtimeData.PlayerMovementRuntimeData;
            movementData.UpdateState(
                E_PlayerMovementState.Airborne,
                8.0f,
                4.0f,
                false,
                true,
                true);
            _runtimeData.SetGameState(E_GameState.Playing);

            _runtimeData.SetGameState(E_GameState.Paused);

            Assert.That(_runtimeData.GameState, Is.EqualTo(E_GameState.Paused));
            Assert.That(
                _runtimeData.PlayerMovementRuntimeData,
                Is.SameAs(movementData));
            Assert.That(
                movementData.CurrentMovementState,
                Is.EqualTo(E_PlayerMovementState.Airborne));
            Assert.That(movementData.CurrentHorizontalSpeed, Is.EqualTo(8.0f));
            Assert.That(movementData.CurrentVerticalSpeed, Is.EqualTo(4.0f));
            Assert.That(movementData.IsGrounded, Is.False);
            Assert.That(movementData.IsMomentumLandingWindowActive, Is.True);
            Assert.That(movementData.IsLastLandingMomentum, Is.True);
        }

        [Test]
        public void SetGameState_Paused_PreservesInfiniteRunRuntimeData()
        {
            _runtimeData.Initialize(E_GameMode.Infinite);
            InfiniteModeRuntimeData infiniteData =
                _runtimeData.InfiniteModeRuntimeData;
            Assert.That(infiniteData.TryUpdate(12.5f, 125), Is.True);
            _runtimeData.SetGameState(E_GameState.Playing);

            _runtimeData.SetGameState(E_GameState.Paused);

            Assert.That(_runtimeData.GameState, Is.EqualTo(E_GameState.Paused));
            Assert.That(
                _runtimeData.InfiniteModeRuntimeData,
                Is.SameAs(infiniteData));
            Assert.That(infiniteData.CurrentDistance, Is.EqualTo(12.5f));
            Assert.That(infiniteData.CurrentScore, Is.EqualTo(125));
            Assert.That(infiniteData.IsFinalized, Is.False);
        }

        [Test]
        public void Clear_PausedData_RemovesPauseAndRunRuntimeData()
        {
            _runtimeData.Initialize(E_GameMode.Infinite);
            _runtimeData.InfiniteModeRuntimeData.TryUpdate(12.5f, 125);
            _runtimeData.SetGameState(E_GameState.Paused);

            _runtimeData.Clear();

            Assert.That(_runtimeData.GameState, Is.EqualTo(E_GameState.None));
            Assert.That(_runtimeData.PlayerMovementRuntimeData, Is.Null);
            Assert.That(_runtimeData.InfiniteModeRuntimeData, Is.Null);
            Assert.That(_runtimeData.IsCreated, Is.False);
        }

        [TestCase(E_GameMode.Stage)]
        [TestCase(E_GameMode.Infinite)]
        public void Initialize_AfterClear_CreatesNewRunWithRequestedMode(
            E_GameMode gameMode)
        {
            _runtimeData.Initialize(gameMode);
            PlayerMovementRuntimeData previousMovementData =
                _runtimeData.PlayerMovementRuntimeData;
            _runtimeData.SetGameState(E_GameState.Ended);
            _runtimeData.SetUIState(E_UIState.Result);
            _runtimeData.Clear();

            _runtimeData.Initialize(gameMode);

            Assert.That(_runtimeData.GameMode, Is.EqualTo(gameMode));
            Assert.That(_runtimeData.GameState, Is.EqualTo(E_GameState.None));
            Assert.That(_runtimeData.UIState, Is.EqualTo(E_UIState.None));
            Assert.That(_runtimeData.PlayerMovementRuntimeData, Is.Not.Null);
            Assert.That(
                _runtimeData.PlayerMovementRuntimeData,
                Is.Not.SameAs(previousMovementData));
            Assert.That(_runtimeData.IsCreated, Is.True);
        }

        [Test]
        public void Initialize_AfterInfiniteClear_CreatesIndependentRunData()
        {
            _runtimeData.Initialize(E_GameMode.Infinite);
            InfiniteModeRuntimeData previousInfiniteData =
                _runtimeData.InfiniteModeRuntimeData;
            previousInfiniteData.TryUpdate(10.0f, 100);
            previousInfiniteData.TryFinalize();
            _runtimeData.Clear();

            _runtimeData.Initialize(E_GameMode.Infinite);
            InfiniteModeRuntimeData currentInfiniteData =
                _runtimeData.InfiniteModeRuntimeData;

            Assert.That(currentInfiniteData, Is.Not.SameAs(previousInfiniteData));
            Assert.That(currentInfiniteData.CurrentDistance, Is.Zero);
            Assert.That(currentInfiniteData.CurrentScore, Is.Zero);
            Assert.That(currentInfiniteData.IsFinalized, Is.False);

            previousInfiniteData.Initialize();
            previousInfiniteData.TryUpdate(20.0f, 200);

            Assert.That(currentInfiniteData.CurrentDistance, Is.Zero);
            Assert.That(currentInfiniteData.CurrentScore, Is.Zero);
        }

        [Test]
        public void Initialize_StageAfterInfiniteClear_DoesNotCreateInfiniteData()
        {
            _runtimeData.Initialize(E_GameMode.Infinite);
            _runtimeData.InfiniteModeRuntimeData.TryUpdate(10.0f, 100);
            _runtimeData.Clear();

            _runtimeData.Initialize(E_GameMode.Stage);

            Assert.That(_runtimeData.GameMode, Is.EqualTo(E_GameMode.Stage));
            Assert.That(_runtimeData.InfiniteModeRuntimeData, Is.Null);
            Assert.That(_runtimeData.IsCreated, Is.True);
        }

        [TestCase(E_GameMode.Stage)]
        [TestCase(E_GameMode.Infinite)]
        public void CollectibleData_PauseAndClear_PreservesThenInvalidatesRun(
            E_GameMode gameMode)
        {
            _runtimeData.Initialize(gameMode);
            CollectibleRuntimeData data = _runtimeData.CollectibleRuntimeData;
            Assert.That(data, Is.Not.Null);
            Assert.That(data.IsInitialized, Is.True);
            data.TryCreateScope(out long scope);
            data.TryRegister(scope, "coin");
            data.TryCollect(scope, "coin");

            _runtimeData.SetGameState(E_GameState.Paused);
            Assert.That(_runtimeData.CollectibleRuntimeData, Is.SameAs(data));
            Assert.That(data.CurrentScore, Is.EqualTo(10));
            _runtimeData.Clear();
            Assert.That(_runtimeData.CollectibleRuntimeData, Is.Null);
            Assert.That(data.IsInitialized, Is.False);
            Assert.That(data.TryCollect(scope, "coin"), Is.False);
        }

        [TestCase(E_GameMode.Stage, E_GameMode.Infinite)]
        [TestCase(E_GameMode.Infinite, E_GameMode.Stage)]
        [TestCase(E_GameMode.Stage, E_GameMode.Stage)]
        [TestCase(E_GameMode.Infinite, E_GameMode.Infinite)]
        public void Initialize_NewRun_ReplacesAndInvalidatesCollectibleData(
            E_GameMode previousMode, E_GameMode nextMode)
        {
            _runtimeData.Initialize(previousMode);
            CollectibleRuntimeData previous = _runtimeData.CollectibleRuntimeData;
            previous.TryCreateScope(out long scope);
            previous.TryRegister(scope, "coin");
            previous.TryCollect(scope, "coin");

            _runtimeData.Initialize(nextMode);

            CollectibleRuntimeData current = _runtimeData.CollectibleRuntimeData;
            Assert.That(current, Is.Not.SameAs(previous));
            Assert.That(current.CurrentScore, Is.Zero);
            Assert.That(current.RegisteredCount, Is.Zero);
            Assert.That(previous.IsInitialized, Is.False);
            Assert.That(previous.TryRegister(scope, "late"), Is.False);
        }

        [Test]
        public void CollectibleScore_DoesNotChangeInfiniteDistanceOrFinalResult()
        {
            _runtimeData.Initialize(E_GameMode.Infinite);
            InfiniteModeRuntimeData infinite = _runtimeData.InfiniteModeRuntimeData;
            Assert.That(infinite.TryUpdate(12.5f, 125), Is.True);
            CollectibleRuntimeData data = _runtimeData.CollectibleRuntimeData;
            data.TryCreateScope(out long scope);
            data.TryRegister(scope, "coin");
            Assert.That(data.TryCollect(scope, "coin"), Is.True);
            Assert.That(infinite.CurrentDistance, Is.EqualTo(12.5f));
            Assert.That(infinite.CurrentScore, Is.EqualTo(125));
            Assert.That(infinite.TryFinalize(), Is.True);

            ScoreRecord record = new ScoreRecord();
            Assert.That(record.TryRecord(
                E_GameMode.Infinite,
                true,
                infinite.IsFinalized,
                infinite.CurrentDistance,
                infinite.CurrentScore,
                data.CurrentScore), Is.True);
            Assert.That(record.ResultData.DistanceScore, Is.EqualTo(125));
            Assert.That(record.ResultData.CollectibleScore, Is.EqualTo(10));
            Assert.That(record.ResultData.TotalScore, Is.EqualTo(135));
            Assert.That(data.CurrentScore, Is.EqualTo(10));
        }
    }
}
