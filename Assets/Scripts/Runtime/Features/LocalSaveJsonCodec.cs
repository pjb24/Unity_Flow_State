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
            public bool recoveryNoticeConfirmed;
            public string onlinePlayerId;
            public string publicPlayerNumber;
            public string onlineProjectId;
            public string onlineEnvironmentId;
            public OnlineAreaFile[] inactiveOnlineAreas;
        }

        [Serializable]
        private sealed class OnlineAreaFile
        {
            public string projectId;
            public string environmentId;
            public bool recoveryNoticeConfirmed;
            public string onlinePlayerId;
            public string publicPlayerNumber;
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
            if (saveData == null || !saveData.HasValidOnlineScopes)
            {
                return string.Empty;
            }

            SaveFile file = new SaveFile
            {
                version = saveData.Version,
                accountId = saveData.AccountId,
                recoveryNoticeConfirmed = saveData.OnlineAccount.HasConfirmedRecoveryNotice,
                onlinePlayerId = saveData.OnlineAccount.PlayerId,
                publicPlayerNumber = saveData.OnlineAccount.PublicNumberCache,
                onlineProjectId = saveData.OnlineScope.ProjectId,
                onlineEnvironmentId = saveData.OnlineScope.EnvironmentId,
                inactiveOnlineAreas = CreateOnlineAreas(saveData.InactiveOnlineAreas),
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

                // No environment marker in v1-v5: assign verification only,
                // irrespective of the environment opening this file.
                OnlineDataScope scope = file.version < 6 ? OnlineDataScope.CreateVerification() :
                    new OnlineDataScope(file.onlineProjectId, file.onlineEnvironmentId);
                if (!scope.IsValid) return false;
                List<OnlineLocalSaveData> areas = ReadOnlineAreas(file, scope);
                saveData = new LocalSaveData(
                    LocalSaveData.CurrentVersion,
                    file.accountId,
                    new LocalSettingsData(
                        file.hasSettings ? ClampMasterVolume(file.masterVolume) : 100,
                        file.hasSettings && file.isFullscreen,
                        ReadBindings(file.bindingOverrides)),
                    file.hasCompletedTutorial,
                    ReadCandidates(file.personalBests),
                    file.version >= 2 ? new OnlineAccountState(file.recoveryNoticeConfirmed,
                        file.onlinePlayerId, file.publicPlayerNumber) : new OnlineAccountState(),
                    ReadPendingCandidates(file.pendingSubmissions), scope, areas, file.version < 6);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static OnlineAreaFile[] CreateOnlineAreas(IReadOnlyList<OnlineLocalSaveData> areas)
        {
            OnlineAreaFile[] files = new OnlineAreaFile[areas.Count];
            for (int i = 0; i < files.Length; i++)
            {
                OnlineLocalSaveData area = areas[i];
                files[i] = new OnlineAreaFile { projectId = area.Scope.ProjectId,
                    environmentId = area.Scope.EnvironmentId,
                    recoveryNoticeConfirmed = area.Account.HasConfirmedRecoveryNotice,
                    onlinePlayerId = area.Account.PlayerId,
                    publicPlayerNumber = area.Account.PublicNumberCache,
                    personalBests = CreateCandidates(area.PersonalBests),
                    pendingSubmissions = CreateCandidates(area.PendingSubmissions) };
            }
            return files;
        }

        private static List<OnlineLocalSaveData> ReadOnlineAreas(SaveFile file, OnlineDataScope active)
        {
            List<OnlineLocalSaveData> result = new List<OnlineLocalSaveData>();
            if (file.version < 6 || file.inactiveOnlineAreas == null) return result;
            for (int i = 0; i < file.inactiveOnlineAreas.Length; i++)
            {
                OnlineAreaFile area = file.inactiveOnlineAreas[i];
                if (area == null) throw new FormatException("Invalid online area.");
                OnlineDataScope scope = new OnlineDataScope(area.projectId, area.environmentId);
                if (!scope.IsValid || scope.Matches(active)) throw new FormatException("Invalid online scope.");
                for (int j = 0; j < result.Count; j++)
                    if (result[j].Scope.Matches(scope)) throw new FormatException("Duplicate online scope.");
                result.Add(new OnlineLocalSaveData(scope,
                    new OnlineAccountState(area.recoveryNoticeConfirmed, area.onlinePlayerId, area.publicPlayerNumber),
                    ReadCandidates(area.personalBests), ReadPendingCandidates(area.pendingSubmissions)));
            }
            return result;
        }

        private static List<RecordSubmissionCandidate> ReadPendingCandidates(CandidateFile[] files)
        {
            List<RecordSubmissionCandidate> result = ReadCandidates(files);
            // Losing an unreadable Pending would falsely unlock transfer.
            if (files != null && result.Count != files.Length) throw new FormatException("Invalid Pending.");
            for (int i = 0; i < result.Count; i++)
                for (int j = 0; j < i; j++)
                    if (result[i].PlayerId == result[j].PlayerId && result[i].SubmissionId == result[j].SubmissionId)
                        throw new FormatException("Duplicate Pending.");
            return result;
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
