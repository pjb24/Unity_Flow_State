const assert = require("node:assert/strict");
const { projectId, environmentId } = require("../CloudCode/account-store");
const clone = value => JSON.parse(JSON.stringify(value));
const context = { projectId, environmentId, playerId: "A", serviceToken: "local-double" };
function fixture() {
  const privateItems = new Map();
  const protectedItems = new Map();
  const scores = new Map();
  const state = { writes: 0, serial: 0, failAt: 0, loseAt: 0, afterRead: null, beforeWrite: null };
  function identity(id, key) { return `${id}/${key}`; }
  function seed(items, id, key, value) {
    items.set(identity(id, key), { key, value: clone(value), writeLock: String(++state.serial) });
  }
  async function read(items, id, keys) {
    const results = [];
    for (const key of keys) {
      const item = items.get(identity(id, key));
      if (item) results.push(clone(item));
    }
    if (state.afterRead) await state.afterRead({ items, id, keys });
    return { data: { results } };
  }
  async function write(items, id, data) {
    state.writes++;
    if (state.beforeWrite) await state.beforeWrite({ items, id, data });
    if (state.writes === state.failAt) throw Error("injected-before-commit");
    // Check ALL locks before committing ANY member of the same-entity batch.
    for (const item of data) {
      if (item.writeLock) {
        const old = items.get(identity(id, item.key));
        if (!old || old.writeLock !== item.writeLock)
          throw Object.assign(Error("cas-conflict"), { response: { status: 409 } });
      }
    }
    for (const item of data) seed(items, id, item.key, item.value);
    if (state.writes === state.loseAt) throw Error("injected-after-commit");
    return { data: {} };
  }
  const save = {
    getPrivateCustomItems: async (project, id, keys) => { assert.equal(project, projectId); return read(privateItems, id, keys); },
    setPrivateCustomItem: async (project, id, item) => { assert.equal(project, projectId); return write(privateItems, id, [item]); },
    setPrivateCustomItemBatch: async (project, id, batch) => { assert.equal(project, projectId); return write(privateItems, id, batch.data); },
    getProtectedItems: async (project, id, keys) => { assert.equal(project, projectId); return read(protectedItems, id, keys); },
    setProtectedItem: async (project, id, item) => { assert.equal(project, projectId); return write(protectedItems, id, [item]); },
    setProtectedItemBatch: async (project, id, batch) => { assert.equal(project, projectId); return write(protectedItems, id, batch.data); }
  };
  const leaderboard = {
    async addLeaderboardPlayerScore(project, board, player, value) {
      assert.equal(project, projectId);
      const old = scores.get(`${board}/${player}`);
      if (!old || (board === "fs-stage-stage-001-r1" ? value.score < old.score : value.score > old.score))
        scores.set(`${board}/${player}`, { playerId: player, ...clone(value) });
      return { data: {} };
    },
    async getLeaderboardScores(project, board, offset, limit) {
      assert.equal(project, projectId);
      const rows = [...scores].filter(([id]) => id.startsWith(`${board}/`)).map(([, row]) => clone(row));
      return { data: { total: rows.length, results: rows.slice(offset, offset + limit) } };
    },
    async getLeaderboardPlayerScore(project, board, player) {
      assert.equal(project, projectId);
      const row = scores.get(`${board}/${player}`);
      if (!row) throw Object.assign(Error("absent-score"), { response: { status: 404 } });
      return { data: clone(row) };
    }
  };
  return { context, state, save, leaderboard, privateItems, protectedItems, scores,
    seedPrivate: (id, key, value) => seed(privateItems, id, key, value),
    seedProtected: (id, key, value) => seed(protectedItems, id, key, value),
    value: (id, key = "fs_account_v1") => privateItems.get(identity(id, key)).value };
}
module.exports = { fixture, context, clone };
