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
        public void Initialize_WithoutMode_UsesInfiniteMode()
        {
            _runtimeData.Initialize();

            Assert.That(_runtimeData.GameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_runtimeData.InfiniteModeRuntimeData, Is.Not.Null);
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
            Assert.That(_runtimeData.InfiniteModeRuntimeData.ScoringVersion,
                Is.EqualTo(ScoringVersion.Current));
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

            Assert.That(_runtimeData.GameMode, Is.EqualTo(E_GameMode.Infinite));
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
        public void SetGameState_Paused_PreservesMovementRuntimeData()
        {
            _runtimeData.Initialize(E_GameMode.Infinite);
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
            Assert.That(UpdateCurrent(infiniteData, 12.5f, 125, 0), Is.True);
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
            UpdateCurrent(_runtimeData.InfiniteModeRuntimeData, 12.5f, 125, 0);
            _runtimeData.SetGameState(E_GameState.Paused);

            _runtimeData.Clear();

            Assert.That(_runtimeData.GameState, Is.EqualTo(E_GameState.None));
            Assert.That(_runtimeData.PlayerMovementRuntimeData, Is.Null);
            Assert.That(_runtimeData.InfiniteModeRuntimeData, Is.Null);
            Assert.That(_runtimeData.IsCreated, Is.False);
        }

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
            UpdateCurrent(previousInfiniteData, 10.0f, 100, 0);
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

        [TestCase(E_GameMode.Stage)]
        [TestCase((E_GameMode)999)]
        public void Initialize_UnsupportedMode_BeforeCreationOrAfterClear_DoesNotCreateRun(E_GameMode mode)
        {
            _runtimeData.Initialize(mode);
            Assert.That(_runtimeData.IsCreated, Is.False);
            Assert.That(_runtimeData.GameMode, Is.EqualTo(E_GameMode.Infinite));
            Assert.That(_runtimeData.PlayerMovementRuntimeData, Is.Null);
            Assert.That(_runtimeData.CollectibleRuntimeData, Is.Null);
            Assert.That(_runtimeData.InfiniteModeRuntimeData, Is.Null);
            _runtimeData.Initialize();
            _runtimeData.Clear();
            _runtimeData.Initialize(mode);
            Assert.That(_runtimeData.IsCreated, Is.False);
            Assert.That(_runtimeData.GameState, Is.EqualTo(E_GameState.None));
            Assert.That(_runtimeData.UIState, Is.EqualTo(E_UIState.None));
            Assert.That(_runtimeData.PlayerMovementRuntimeData, Is.Null);
            Assert.That(_runtimeData.CollectibleRuntimeData, Is.Null);
            Assert.That(_runtimeData.InfiniteModeRuntimeData, Is.Null);
        }

        [TestCase(E_GameState.None)]
        [TestCase(E_GameState.Ready)]
        [TestCase(E_GameState.Playing)]
        [TestCase(E_GameState.Paused)]
        [TestCase(E_GameState.Ended)]
        public void Initialize_UnsupportedMode_PreservesRunAtEveryState(E_GameState state)
        {
            _runtimeData.Initialize();
            PlayerMovementRuntimeData movement = _runtimeData.PlayerMovementRuntimeData;
            InfiniteModeRuntimeData infinite = _runtimeData.InfiniteModeRuntimeData;
            CollectibleRuntimeData collectibles = _runtimeData.CollectibleRuntimeData;
            Assert.That(UpdateCurrent(infinite, 12.5f, 125, 0), Is.True);
            Assert.That(collectibles.TryCreateScope(out long scope), Is.True);
            Assert.That(collectibles.TryRegister(scope, "collected"), Is.True);
            Assert.That(collectibles.TryRegister(scope, "pending"), Is.True);
            Assert.That(collectibles.TryCollect(scope, "collected"), Is.True);
            movement.UpdateState(E_PlayerMovementState.Airborne, 8.0f, 4.0f, false, true, true);
            if (state == E_GameState.Ended) Assert.That(infinite.TryFinalize(), Is.True);
            _runtimeData.SetGameState(state);
            E_UIState uiState = state == E_GameState.Ended ? E_UIState.Result : E_UIState.StageHud;
            _runtimeData.SetUIState(uiState);

            foreach (E_GameMode mode in new[] { E_GameMode.Stage, (E_GameMode)999 })
            {
                _runtimeData.Initialize(mode);
                Assert.That(_runtimeData.IsCreated, Is.True);
                Assert.That(_runtimeData.GameMode, Is.EqualTo(E_GameMode.Infinite));
                Assert.That(_runtimeData.GameState, Is.EqualTo(state));
                Assert.That(_runtimeData.UIState, Is.EqualTo(uiState));
                Assert.That(_runtimeData.PlayerMovementRuntimeData, Is.SameAs(movement));
                Assert.That(_runtimeData.InfiniteModeRuntimeData, Is.SameAs(infinite));
                Assert.That(_runtimeData.CollectibleRuntimeData, Is.SameAs(collectibles));
                Assert.That(movement.CurrentMovementState, Is.EqualTo(E_PlayerMovementState.Airborne));
                Assert.That(movement.CurrentHorizontalSpeed, Is.EqualTo(8.0f));
                Assert.That(movement.CurrentVerticalSpeed, Is.EqualTo(4.0f));
                Assert.That(infinite.CurrentDistance, Is.EqualTo(12.5f));
                Assert.That(infinite.CurrentScore, Is.EqualTo(125));
                Assert.That(infinite.IsFinalized, Is.EqualTo(state == E_GameState.Ended));
                Assert.That(collectibles.ActiveScopeCount, Is.EqualTo(1));
                Assert.That(collectibles.RegisteredCount, Is.EqualTo(2));
                Assert.That(collectibles.CurrentScore, Is.EqualTo(10));
                Assert.That(collectibles.TryCollect(scope, "collected"), Is.False);
            }
            Assert.That(collectibles.TryCollect(scope, "pending"), Is.True);
            Assert.That(collectibles.CurrentScore, Is.EqualTo(20));
        }

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
            Assert.That(UpdateCurrent(infinite, 12.5f, 125, 0), Is.True);
            CollectibleRuntimeData data = _runtimeData.CollectibleRuntimeData;
            data.TryCreateScope(out long scope);
            data.TryRegister(scope, "coin");
            Assert.That(data.TryCollect(scope, "coin"), Is.True);
            Assert.That(UpdateCurrent(infinite, 12.5f, 125, 0, 10), Is.True);
            Assert.That(infinite.CurrentDistance, Is.EqualTo(12.5f));
            Assert.That(infinite.CurrentScore, Is.EqualTo(125));
            Assert.That(infinite.TryFinalize(), Is.True);

            ScoreRecord record = new ScoreRecord();
            Assert.That(record.TryRecord(
                infinite.ScoringVersion,
                E_GameMode.Infinite,
                true,
                infinite.IsFinalized,
                infinite.CurrentDistance,
                infinite.BaseDistanceScore,
                infinite.MomentumBonus,
                infinite.CurrentScore,
                infinite.CollectibleScore,
                infinite.MaximumMomentumMultiplier), Is.True);
            Assert.That(record.ResultData.DistanceScore, Is.EqualTo(125));
            Assert.That(record.ResultData.CollectibleScore, Is.EqualTo(10));
            Assert.That(record.ResultData.TotalScore, Is.EqualTo(135));
            Assert.That(data.CurrentScore, Is.EqualTo(10));
        }

        private static bool UpdateCurrent(
            InfiniteModeRuntimeData data,
            float distance,
            int baseDistanceScore,
            int momentumBonus,
            int collectibleScore = 0)
        {
            int distanceScore = baseDistanceScore + momentumBonus;
            int totalScore = distanceScore + collectibleScore;
            double multiplier = momentumBonus > 0 ? 1.25 : 1.0;
            double duration = momentumBonus > 0 ? 10.0 : 0.0;
            return data.TryUpdate(
                ScoringVersion.Current,
                distance,
                baseDistanceScore,
                momentumBonus,
                distanceScore,
                collectibleScore,
                totalScore,
                multiplier,
                multiplier,
                duration,
                duration);
        }
    }
}
