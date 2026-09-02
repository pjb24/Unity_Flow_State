using FlowState.Runtime.Core;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class PauseMenuStateTests
    {
        private PauseMenuState _state;

        [SetUp]
        public void SetUp()
        {
            _state = new PauseMenuState();
        }

        [Test]
        public void Activate_InactiveMenu_SelectsResume()
        {
            Assert.That(_state.Activate(), Is.True);
            Assert.That(_state.IsActive, Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_PauseMenuSelection.Resume));
        }

        [Test]
        public void Activate_ActiveMenu_IsRejectedWithoutMutation()
        {
            Assert.That(_state.Activate(), Is.True);
            Assert.That(_state.TryMove(-1.0f), Is.True);

            Assert.That(_state.Activate(), Is.False);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_PauseMenuSelection.Retry));
        }

        [Test]
        public void Deactivate_ActiveMenu_ResetsSelectionAndRejectsInput()
        {
            Assert.That(_state.Activate(), Is.True);
            Assert.That(_state.TryMove(-1.0f), Is.True);

            Assert.That(_state.Deactivate(), Is.True);
            Assert.That(_state.IsActive, Is.False);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_PauseMenuSelection.Resume));
            Assert.That(_state.TryMove(-1.0f), Is.False);
            Assert.That(_state.TrySubmit(out _), Is.False);
        }

        [Test]
        public void Move_ActiveMenu_UsesResumeRetryQuitOrderWithoutWrapping()
        {
            Assert.That(_state.Activate(), Is.True);
            Assert.That(_state.TryMove(-1.0f), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_PauseMenuSelection.Retry));
            Assert.That(_state.TryMove(-1.0f), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_PauseMenuSelection.Quit));
            Assert.That(_state.TryMove(-1.0f), Is.False);
            Assert.That(_state.TryMove(1.0f), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_PauseMenuSelection.Retry));
        }

        [Test]
        public void Move_BelowThreshold_IsRejected()
        {
            Assert.That(_state.Activate(), Is.True);
            Assert.That(_state.TryMove(0.49f), Is.False);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_PauseMenuSelection.Resume));
        }

        [Test]
        public void Submit_ActiveMenu_ReturnsCurrentSelectionOncePerRequest()
        {
            Assert.That(_state.Activate(), Is.True);
            Assert.That(_state.TryMove(-1.0f), Is.True);

            Assert.That(_state.TrySubmit(out E_PauseMenuSelection selection), Is.True);
            Assert.That(selection, Is.EqualTo(E_PauseMenuSelection.Retry));
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_PauseMenuSelection.Retry));
        }

        [Test]
        public void Cancel_ActiveMenu_ReturnsResumeWithoutChangingSelection()
        {
            Assert.That(_state.Activate(), Is.True);
            Assert.That(_state.TryMove(-1.0f), Is.True);

            Assert.That(_state.TryCancel(out E_PauseMenuSelection selection), Is.True);
            Assert.That(selection, Is.EqualTo(E_PauseMenuSelection.Resume));
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_PauseMenuSelection.Retry));
        }

        [Test]
        public void Pointer_ActiveMenu_SelectsValidTarget()
        {
            Assert.That(_state.Activate(), Is.True);

            Assert.That(_state.TrySelectAtPointer(E_PauseMenuSelection.Quit), Is.True);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_PauseMenuSelection.Quit));
        }

        [Test]
        public void Click_ActiveMenu_SelectsAndReturnsTarget()
        {
            Assert.That(_state.Activate(), Is.True);

            Assert.That(
                _state.TryClick(E_PauseMenuSelection.Retry, out E_PauseMenuSelection selection),
                Is.True);
            Assert.That(selection, Is.EqualTo(E_PauseMenuSelection.Retry));
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_PauseMenuSelection.Retry));
        }

        [Test]
        public void InvalidSelection_IsRejectedWithoutMutation()
        {
            Assert.That(_state.Activate(), Is.True);

            Assert.That(
                _state.TrySelectAtPointer((E_PauseMenuSelection)999),
                Is.False);
            Assert.That(_state.CurrentSelection, Is.EqualTo(E_PauseMenuSelection.Resume));
        }
    }
}
