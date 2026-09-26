using System;
using FlowState.Runtime.Core;

namespace FlowState.Runtime.Features
{
    public struct RecordBoardKey : IEquatable<RecordBoardKey>
    {
        private readonly E_GameMode _gameMode;
        private readonly string _stageId;
        private readonly int _rulesVersion;

        public E_GameMode GameMode => _gameMode;

        public string StageId => _stageId;

        public int RulesVersion => _rulesVersion;

        private RecordBoardKey(
            E_GameMode gameMode,
            string stageId,
            int rulesVersion)
        {
            _gameMode = gameMode;
            _stageId = stageId;
            _rulesVersion = rulesVersion;
        }

        public static bool TryCreateStage(
            string stageId,
            int stageRulesVersion,
            out RecordBoardKey boardKey)
        {
            boardKey = default;

            if (string.IsNullOrEmpty(stageId) || stageRulesVersion <= 0)
            {
                return false;
            }

            boardKey = new RecordBoardKey(
                E_GameMode.Stage,
                stageId,
                stageRulesVersion);
            return true;
        }

        public static bool TryCreateInfinite(
            int scoringVersion,
            out RecordBoardKey boardKey)
        {
            boardKey = default;

            if (!ScoringVersion.IsSupported(scoringVersion))
            {
                return false;
            }

            boardKey = new RecordBoardKey(
                E_GameMode.Infinite,
                string.Empty,
                scoringVersion);
            return true;
        }

        public bool Equals(RecordBoardKey other)
        {
            return _gameMode == other._gameMode &&
                   _stageId == other._stageId &&
                   _rulesVersion == other._rulesVersion;
        }

        public override bool Equals(object obj)
        {
            return obj is RecordBoardKey && Equals((RecordBoardKey)obj);
        }

        public override int GetHashCode()
        {
            int hashCode = (int)_gameMode;
            hashCode = hashCode * 31 + _rulesVersion;
            return hashCode * 31 + (_stageId == null ? 0 : _stageId.GetHashCode());
        }
    }
}
