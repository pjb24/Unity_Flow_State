using FlowState.Runtime.Core;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class GameStateTests
    {
        [Test]
        public void NewState_DefaultsToNone()
        {
            GameState gameState = new GameState();

            Assert.That(gameState.CurrentState, Is.EqualTo(E_GameState.None));
        }

        [Test]
        public void TryPause_PlayingState_ChangesToPaused()
        {
            GameState gameState = CreatePlayingState();

            bool result = gameState.TryPause();

            Assert.That(result, Is.True);
            Assert.That(gameState.CurrentState, Is.EqualTo(E_GameState.Paused));
        }

        [Test]
        public void TryPause_PausedState_IsRejectedWithoutMutation()
        {
            GameState gameState = CreatePausedState();

            bool result = gameState.TryPause();

            Assert.That(result, Is.False);
            Assert.That(gameState.CurrentState, Is.EqualTo(E_GameState.Paused));
        }

        [Test]
        public void TryResume_PausedState_ChangesToPlaying()
        {
            GameState gameState = CreatePausedState();

            bool result = gameState.TryResume();

            Assert.That(result, Is.True);
            Assert.That(gameState.CurrentState, Is.EqualTo(E_GameState.Playing));
        }

        [TestCase(E_GameState.None)]
        [TestCase(E_GameState.Initializing)]
        [TestCase(E_GameState.Ready)]
        [TestCase(E_GameState.Ending)]
        [TestCase(E_GameState.Ended)]
        public void TryPause_NonPlayingState_IsRejectedWithoutMutation(
            E_GameState initialState)
        {
            GameState gameState = CreateState(initialState);

            bool result = gameState.TryPause();

            Assert.That(result, Is.False);
            Assert.That(gameState.CurrentState, Is.EqualTo(initialState));
        }

        [TestCase(E_GameState.None)]
        [TestCase(E_GameState.Initializing)]
        [TestCase(E_GameState.Ready)]
        [TestCase(E_GameState.Playing)]
        [TestCase(E_GameState.Ending)]
        [TestCase(E_GameState.Ended)]
        public void TryResume_NonPausedState_IsRejectedWithoutMutation(
            E_GameState initialState)
        {
            GameState gameState = CreateState(initialState);

            bool result = gameState.TryResume();

            Assert.That(result, Is.False);
            Assert.That(gameState.CurrentState, Is.EqualTo(initialState));
        }

        [Test]
        public void TryTransitionTo_EndedState_AllowsRetryInitialization()
        {
            GameState gameState = CreateState(E_GameState.Ended);

            bool result = gameState.TryTransitionTo(E_GameState.Initializing);

            Assert.That(result, Is.True);
            Assert.That(
                gameState.CurrentState,
                Is.EqualTo(E_GameState.Initializing));
        }

        [Test]
        public void TryTransitionTo_PausedState_AllowsEnding()
        {
            GameState gameState = CreatePausedState();

            bool result = gameState.TryTransitionTo(E_GameState.Ending);

            Assert.That(result, Is.True);
            Assert.That(gameState.CurrentState, Is.EqualTo(E_GameState.Ending));
        }

        [Test]
        public void TryTransitionTo_InvalidTransition_IsRejectedWithoutMutation()
        {
            GameState gameState = new GameState();

            bool result = gameState.TryTransitionTo(E_GameState.Playing);

            Assert.That(result, Is.False);
            Assert.That(gameState.CurrentState, Is.EqualTo(E_GameState.None));
        }

        [Test]
        public void Reset_PausedState_RestoresNone()
        {
            GameState gameState = CreatePausedState();

            gameState.Reset();

            Assert.That(gameState.CurrentState, Is.EqualTo(E_GameState.None));
        }

        private GameState CreatePlayingState()
        {
            return CreateState(E_GameState.Playing);
        }

        private GameState CreatePausedState()
        {
            GameState gameState = CreatePlayingState();
            Assert.That(gameState.TryPause(), Is.True);
            return gameState;
        }

        private GameState CreateState(E_GameState targetState)
        {
            GameState gameState = new GameState();

            if (targetState == E_GameState.None)
            {
                return gameState;
            }

            Assert.That(
                gameState.TryTransitionTo(E_GameState.Initializing),
                Is.True);

            if (targetState == E_GameState.Initializing)
            {
                return gameState;
            }

            Assert.That(gameState.TryTransitionTo(E_GameState.Ready), Is.True);

            if (targetState == E_GameState.Ready)
            {
                return gameState;
            }

            Assert.That(
                gameState.TryTransitionTo(E_GameState.Playing),
                Is.True);

            if (targetState == E_GameState.Playing)
            {
                return gameState;
            }

            if (targetState == E_GameState.Paused)
            {
                Assert.That(gameState.TryPause(), Is.True);
                return gameState;
            }

            Assert.That(
                gameState.TryTransitionTo(E_GameState.Ending),
                Is.True);

            if (targetState == E_GameState.Ending)
            {
                return gameState;
            }

            Assert.That(gameState.TryTransitionTo(E_GameState.Ended), Is.True);
            return gameState;
        }
    }
}
