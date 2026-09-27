using System;
using System.Collections.Generic;
using FlowState.Runtime.Core;
using UnityEngine;

namespace FlowState.Runtime.Features
{
    public static class LocalSaveJsonCodec
    {
        [Serializable]
        private sealed class SaveFile
        {
            public int version;
            public string accountId;
            public bool hasSettings;
            public int masterVolume;
            public bool isFullscreen;
            public bool hasCompletedTutorial;
            public BindingFile[] bindingOverrides;
            public CandidateFile[] personalBests;
            public CandidateFile[] pendingSubmissions;
        }

        [Serializable]
        private sealed class BindingFile
        {
            public int actionMap;
            public int deviceGroup;
            public string actionId;
            public string bindingId;
            public string compositePartName;
            public string controlPath;
        }

        [Serializable]
        private sealed class CandidateFile
        {
            public string playerId;
            public string submissionId;
            public int gameMode;
            public string stageId;
            public int rulesVersion;
            public long rankingValue;
            public long runDurationMilliseconds;
            public int baseDistanceScore;
            public int momentumBonus;
            public int distanceScore;
            public int collectibleScore;
            public int totalScore;
            public double maximumMomentumMultiplier;
        }

        public static string Serialize(LocalSaveData saveData)
        {
            if (saveData == null)
            {
                return string.Empty;
            }

            SaveFile file = new SaveFile
            {
                version = saveData.Version,
                accountId = saveData.AccountId,
                hasSettings = saveData.Settings != null,
                isFullscreen = saveData.Settings != null && saveData.Settings.IsFullscreen,
                masterVolume = saveData.Settings == null ? 100 : saveData.Settings.MasterVolume,
                hasCompletedTutorial = saveData.HasCompletedTutorial,
                bindingOverrides = CreateBindings(saveData.Settings),
                personalBests = CreateCandidates(saveData.PersonalBests),
                pendingSubmissions = CreateCandidates(saveData.PendingSubmissions)
            };
            return JsonUtility.ToJson(file);
        }

        public static bool TryDeserialize(string contents, out LocalSaveData saveData)
        {
            saveData = LocalSaveData.CreateDefault(new LocalSettingsData(100, false, null));

            if (string.IsNullOrEmpty(contents))
            {
                return false;
            }

            try
            {
                SaveFile file = JsonUtility.FromJson<SaveFile>(contents);

                if (file == null || file.version < 0 ||
                    file.version > LocalSaveData.CurrentVersion)
                {
                    return false;
                }

                if (file.version == 0)
                {
                    saveData = LocalSaveData.CreateDefault(
                        new LocalSettingsData(100, false, null));
                    return true;
                }

                saveData = new LocalSaveData(
                    file.version,
                    file.accountId,
                    new LocalSettingsData(
                        file.hasSettings ? ClampMasterVolume(file.masterVolume) : 100,
                        file.hasSettings && file.isFullscreen,
                        ReadBindings(file.bindingOverrides)),
                    file.hasCompletedTutorial,
                    ReadCandidates(file.personalBests),
                    ReadCandidates(file.pendingSubmissions));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static BindingFile[] CreateBindings(LocalSettingsData settings)
        {
            if (settings == null || settings.BindingOverrides.Count == 0)
            {
                return new BindingFile[0];
            }

            BindingFile[] bindings = new BindingFile[settings.BindingOverrides.Count];

            for (int i = 0; i < bindings.Length; i++)
            {
                SettingsBindingOverride binding = settings.BindingOverrides[i];
                bindings[i] = new BindingFile
                {
                    actionMap = (int)binding.Target.ActionMap,
                    deviceGroup = (int)binding.Target.DeviceGroup,
                    actionId = binding.Target.ActionId.ToString("D"),
                    bindingId = binding.Target.BindingId.ToString("D"),
                    compositePartName = binding.Target.CompositePartName,
                    controlPath = binding.ControlPath
                };
            }

            return bindings;
        }

        private static CandidateFile[] CreateCandidates(
            IReadOnlyList<RecordSubmissionCandidate> candidates)
        {
            CandidateFile[] files = new CandidateFile[candidates.Count];

            for (int i = 0; i < candidates.Count; i++)
            {
                RecordSubmissionCandidate candidate = candidates[i];
                files[i] = new CandidateFile
                {
                    playerId = candidate.PlayerId,
                    submissionId = candidate.SubmissionId,
                    gameMode = (int)candidate.GameMode,
                    stageId = candidate.BoardKey.StageId,
                    rulesVersion = candidate.BoardKey.RulesVersion,
                    rankingValue = candidate.RankingValue,
                    runDurationMilliseconds = candidate.RunDurationMilliseconds,
                    baseDistanceScore = candidate.BaseDistanceScore,
                    momentumBonus = candidate.MomentumBonus,
                    distanceScore = candidate.DistanceScore,
                    collectibleScore = candidate.CollectibleScore,
                    totalScore = candidate.TotalScore,
                    maximumMomentumMultiplier = candidate.MaximumMomentumMultiplier
                };
            }

            return files;
        }

        private static List<SettingsBindingOverride> ReadBindings(BindingFile[] bindings)
        {
            List<SettingsBindingOverride> results = new List<SettingsBindingOverride>();

            if (bindings == null)
            {
                return results;
            }

            for (int i = 0; i < bindings.Length; i++)
            {
                BindingFile binding = bindings[i];

                if (binding == null || string.IsNullOrEmpty(binding.controlPath) ||
                    !Guid.TryParse(binding.actionId, out Guid actionId) ||
                    !Guid.TryParse(binding.bindingId, out Guid bindingId))
                {
                    continue;
                }

                results.Add(new SettingsBindingOverride(
                    new SettingsBindingTarget(
                        (E_SettingsActionMap)binding.actionMap,
                        (E_SettingsDeviceGroup)binding.deviceGroup,
                        actionId,
                        bindingId,
                        binding.compositePartName),
                    binding.controlPath));
            }

            return results;
        }

        private static List<RecordSubmissionCandidate> ReadCandidates(CandidateFile[] files)
        {
            List<RecordSubmissionCandidate> candidates =
                new List<RecordSubmissionCandidate>();

            if (files == null)
            {
                return candidates;
            }

            for (int i = 0; i < files.Length; i++)
            {
                CandidateFile file = files[i];

                if (file == null || !TryCreateBoardKey(file, out RecordBoardKey boardKey))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(file.playerId) ||
                    !Guid.TryParse(file.submissionId, out Guid submissionId) ||
                    submissionId.ToString("D")[14] != '4' ||
                    file.rankingValue < 0 || file.runDurationMilliseconds < 0 ||
                    file.baseDistanceScore < 0 || file.momentumBonus < 0 ||
                    file.distanceScore < 0 || file.collectibleScore < 0 ||
                    file.totalScore < 0)
                {
                    continue;
                }

                candidates.Add(new RecordSubmissionCandidate(
                    file.playerId,
                    file.submissionId,
                    boardKey,
                    file.rankingValue,
                    file.runDurationMilliseconds,
                    file.baseDistanceScore,
                    file.momentumBonus,
                    file.distanceScore,
                    file.collectibleScore,
                    file.totalScore,
                    file.maximumMomentumMultiplier));
            }

            return candidates;
        }

        private static bool TryCreateBoardKey(
            CandidateFile file,
            out RecordBoardKey boardKey)
        {
            if (file.gameMode == (int)E_GameMode.Stage)
            {
                return RecordBoardKey.TryCreateStage(
                    file.stageId,
                    file.rulesVersion,
                    out boardKey);
            }

            if (file.gameMode == (int)E_GameMode.Infinite)
            {
                return RecordBoardKey.TryCreateInfinite(
                    file.rulesVersion,
                    out boardKey);
            }

            boardKey = default;
            return false;
        }

        private static int ClampMasterVolume(int masterVolume)
        {
            if (masterVolume < 0)
            {
                return 0;
            }

            return masterVolume > 100 ? 100 : masterVolume;
        }
    }
}
