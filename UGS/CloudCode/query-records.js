const { LeaderboardsApi } = require("@unity-services/leaderboards-1.1");
const project = "c76d55cf-7846-494b-9dce-a0797b179b36";
const environment = "a20a46fa-1edb-4d79-9c35-02f2fed31896";
const stage = "fs-stage-stage-001-r1";
const infinite = "fs-infinite-v2";
module.exports = async ({ params, context }) => {
  const fail = reason => ({ status: "TransientFailure", reason, entries: [] });
  if (context.projectId !== project || context.environmentId !== environment || !context.playerId)
    return fail("ContextMismatch");
  let p;
  try { p = JSON.parse(params.request); } catch (_) { return fail("InvalidRequest"); }
  if (!p || ![stage, infinite].includes(p.boardId) || !["top", "around", "me"].includes(p.kind) ||
      !Number.isInteger(p.limit) || p.limit < 1 || p.limit > 20) return fail("InvalidQuery");
  try {
    const api = new LeaderboardsApi(context);
    // Complete single-page snapshot only; never rank a truncated global table.
    const response = await api.getLeaderboardScores(project, p.boardId, 0, 100,
      { params: { includeMetadata: true } });
    const data = response.data;
    if (!Number.isInteger(data.total) || data.total > 100 || data.results.length !== data.total)
      return fail("VerificationBoardCapacity");
    const entries = data.results.map(e => ({ playerId: e.playerId, score: e.score,
      acceptedAt: e.metadata && e.metadata.acceptedAt, rank: 0 }));
    if (entries.some(e => !Number.isSafeInteger(e.score) || e.score < 0 ||
      !Number.isSafeInteger(e.acceptedAt) || e.acceptedAt <= 0)) return fail("MissingServerMetadata");
    entries.sort((a, b) => (p.boardId === stage ? a.score - b.score : b.score - a.score) ||
      a.acceptedAt - b.acceptedAt || a.playerId.localeCompare(b.playerId));
    for (let i = 0; i < entries.length; i++) {
      entries[i].rank = i > 0 && entries[i].score === entries[i - 1].score &&
        entries[i].acceptedAt === entries[i - 1].acceptedAt ? entries[i - 1].rank : i + 1;
    }
    const me = entries.findIndex(e => e.playerId === context.playerId);
    let selected;
    if (p.kind === "top") selected = entries.slice(0, p.limit);
    else if (p.kind === "me") selected = me < 0 ? [] : [entries[me]];
    else {
      const start = Math.max(0, Math.min(me - Math.floor(p.limit / 2), entries.length - p.limit));
      selected = me < 0 ? [] : entries.slice(start, start + p.limit);
    }
    return { status: "Success", reason: "", entries: selected };
  } catch (_) { return fail("ServiceUnavailable"); }
};
module.exports.params = { request: { type: "String", required: true } };
