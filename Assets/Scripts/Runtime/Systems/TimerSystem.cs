using System.Collections.Generic;
using FlowState.Runtime.Core;
using UnityEngine;

namespace FlowState.Runtime.Systems
{
    public class TimerSystem : MonoBehaviour
    {
        private readonly Dictionary<E_TimerKey, TimerRuntimeData> _timers =
            new Dictionary<E_TimerKey, TimerRuntimeData>();

        public bool CreateTimer(E_TimerKey timerKey)
        {
            if (_timers.ContainsKey(timerKey))
            {
                Debug.LogWarning(
                    $"[TimerSystem] Timer already exists: {timerKey}.");
                return false;
            }

            TimerRuntimeData timerData = new TimerRuntimeData();
            timerData.Initialize();
            _timers.Add(timerKey, timerData);
            return true;
        }

        public bool StartTimer(E_TimerKey timerKey)
        {
            return ChangeTimerState(
                timerKey,
                timerData => timerData.Start(GetCurrentTime()));
        }

        public bool PauseTimer(E_TimerKey timerKey)
        {
            return ChangeTimerState(
                timerKey,
                timerData => timerData.Pause(GetCurrentTime()));
        }

        public bool ResumeTimer(E_TimerKey timerKey)
        {
            return ChangeTimerState(
                timerKey,
                timerData => timerData.Resume(GetCurrentTime()));
        }

        public bool StopTimer(E_TimerKey timerKey)
        {
            return ChangeTimerState(
                timerKey,
                timerData => timerData.Stop(GetCurrentTime()));
        }

        public bool RemoveTimer(E_TimerKey timerKey)
        {
            if (!TryGetTimer(timerKey, out TimerRuntimeData timerData))
            {
                return false;
            }

            return _timers.Remove(timerKey);
        }

        public bool HasTimer(E_TimerKey timerKey)
        {
            return _timers.ContainsKey(timerKey);
        }

        public bool TryGetTimerState(
            E_TimerKey timerKey,
            out E_TimerState timerState)
        {
            if (!TryGetTimer(timerKey, out TimerRuntimeData timerData))
            {
                timerState = E_TimerState.Created;
                return false;
            }

            timerState = timerData.State;
            return true;
        }

        public double GetElapsedTime(E_TimerKey timerKey)
        {
            return TryGetTimer(timerKey, out TimerRuntimeData timerData)
                ? timerData.GetElapsedTime(GetCurrentTime())
                : 0.0;
        }

        private bool ChangeTimerState(
            E_TimerKey timerKey,
            System.Func<TimerRuntimeData, bool> changeState)
        {
            if (!TryGetTimer(timerKey, out TimerRuntimeData timerData))
            {
                return false;
            }

            if (changeState(timerData))
            {
                return true;
            }

            Debug.LogWarning(
                $"[TimerSystem] Timer state request is invalid: " +
                $"{timerKey}, {timerData.State}.");
            return false;
        }

        private bool TryGetTimer(
            E_TimerKey timerKey,
            out TimerRuntimeData timerData)
        {
            if (_timers.TryGetValue(timerKey, out timerData))
            {
                return true;
            }

            Debug.LogWarning(
                $"[TimerSystem] Timer does not exist: {timerKey}.");
            return false;
        }

        private double GetCurrentTime()
        {
            return Time.realtimeSinceStartupAsDouble;
        }
    }
}
