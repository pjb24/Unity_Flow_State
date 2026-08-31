using FlowState.Runtime.Core;
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
            Assert.That(_runtimeData.IsCreated, Is.True);
        }

        [Test]
        public void Initialize_InfiniteMode_StoresCurrentRunMode()
        {
            _runtimeData.Initialize(E_GameMode.Infinite);

            Assert.That(_runtimeData.GameMode, Is.EqualTo(E_GameMode.Infinite));
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
    }
}
