using System;
using System.Collections.Generic;

namespace FlowState.Runtime.Features
{
    [Serializable]
    public sealed class OnlineAccountState
    {
        public bool HasConfirmedRecoveryNotice { get; }
        public string PlayerId { get; }
        public IReadOnlyList<string> SubmittedIds { get; }
        public IReadOnlyList<string> RejectedIds { get; }

        public OnlineAccountState(bool hasConfirmedRecoveryNotice = false,
            string playerId = "", IEnumerable<string> submittedIds = null,
            IEnumerable<string> rejectedIds = null)
        {
            HasConfirmedRecoveryNotice = hasConfirmedRecoveryNotice;
            PlayerId = hasConfirmedRecoveryNotice && playerId != null ? playerId : string.Empty;
            SubmittedIds = CopyIds(submittedIds).AsReadOnly();
            RejectedIds = CopyIds(rejectedIds).AsReadOnly();
        }

        public bool HasFinished(string submissionId)
        {
            return Contains(SubmittedIds, submissionId) || Contains(RejectedIds, submissionId);
        }

        public OnlineAccountState WithResult(string submissionId, E_RecordSubmissionResult result)
        {
            List<string> submitted = CopyIds(SubmittedIds);
            List<string> rejected = CopyIds(RejectedIds);
            if (!HasFinished(submissionId))
            {
                if (result == E_RecordSubmissionResult.Submitted) submitted.Add(submissionId);
                if (result == E_RecordSubmissionResult.Rejected) rejected.Add(submissionId);
            }
            return new OnlineAccountState(HasConfirmedRecoveryNotice, PlayerId, submitted, rejected);
        }

        private static bool Contains(IReadOnlyList<string> ids, string id)
        {
            for (int i = 0; i < ids.Count; i++) if (ids[i] == id) return true;
            return false;
        }

        private static List<string> CopyIds(IEnumerable<string> ids)
        {
            List<string> result = new List<string>();
            if (ids == null) return result;
            foreach (string id in ids)
            {
                if (Guid.TryParse(id, out Guid parsed) && parsed.ToString("D")[14] == '4' &&
                    !result.Contains(id)) result.Add(id);
            }
            return result;
        }
    }
}
