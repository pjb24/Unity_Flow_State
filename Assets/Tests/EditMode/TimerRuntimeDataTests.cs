using FlowState.Runtime.Core;
using NUnit.Framework;

namespace FlowState.Tests.EditMode
{
    public class TimerRuntimeDataTests
    {
        private TimerRuntimeData _timerData;

        [SetUp]
        public void SetUp()
        {
            _timerData = new TimerRuntimeData();
            _timerData.Initialize();
        }

        [Test]
        public void Start_ValidRequest_MeasuresElapsedTime()
        {
            bool didStart = _timerData.Start(10.0);

            Assert.That(didStart, Is.True);
            Assert.That(_timerData.State, Is.EqualTo(E_TimerState.Running));
            Assert.That(_timerData.GetElapsedTime(12.5), Is.EqualTo(2.5));
        }

        [Test]
        public void PauseAndResume_ValidRequests_ExcludePausedDuration()
        {
            _timerData.Start(10.0);
            bool didPause = _timerData.Pause(12.0);
            double pausedTime = _timerData.GetElapsedTime(20.0);
            bool didResume = _timerData.Resume(22.0);

            Assert.That(didPause, Is.True);
            Assert.That(pausedTime, Is.EqualTo(2.0));
            Assert.That(didResume, Is.True);
            Assert.That(_timerData.GetElapsedTime(25.0), Is.EqualTo(5.0));
        }

        [Test]
        public void Stop_ValidRequest_PreservesFinalTime()
        {
            _timerData.Start(10.0);
            bool didStop = _timerData.Stop(14.0);

            Assert.That(didStop, Is.True);
            Assert.That(_timerData.State, Is.EqualTo(E_TimerState.Stopped));
            Assert.That(_timerData.GetElapsedTime(30.0), Is.EqualTo(4.0));
        }

        [Test]
        public void Start_SecondRequest_IsRejected()
        {
            _timerData.Start(10.0);

            bool didStartAgain = _timerData.Start(11.0);

            Assert.That(didStartAgain, Is.False);
            Assert.That(_timerData.State, Is.EqualTo(E_TimerState.Running));
        }
    }
}
