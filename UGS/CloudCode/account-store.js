// Internal library. Never publish as a player-callable script.
const projectId = "c76d55cf-7846-494b-9dce-a0797b179b36";
const environmentId = "a20a46fa-1edb-4d79-9c35-02f2fed31896";
const key = "fs_account_v1";
const guardKey = "fs_creation_guard_v1";
const kinds = new Set(["account", "player", "owner", "number", "ledger", "allocator", "t", "receipt"]);

function fault(reason) { return Object.assign(new Error(reason), { reason }); }
function validateContext(context) {
  if (!context || context.projectId !== projectId || context.environmentId !== environmentId ||
      typeof context.playerId !== "string" || !context.playerId || !context.serviceToken)
    throw fault("ContextMismatch");
}
function locator(kind, id) {
  if (!kinds.has(kind) || typeof id !== "string" || !/^[A-Za-z0-9_-]+$/.test(id) ||
      `fs8-${kind}-${id}`.length > 50)
    throw fault("InvalidStorageKey");
  return `fs8-${kind}-${id}`;
}
function validateScope(value) {
  if (!value || value.schemaVersion !== 1 || value.projectId !== projectId ||
      value.environmentId !== environmentId) throw fault("StoredScopeMismatch");
}

function createAccountStore(context, api) {
  validateContext(context);
  if (!api) {
    const { DataApi } = require("@unity-services/cloud-save-1.4");
    api = new DataApi(context);
  }
  return {
    async read(kind, id) {
      const customId = locator(kind, id);
      const response = await api.getPrivateCustomItems(projectId, customId, [key]);
      if (!response || !response.data || !Array.isArray(response.data.results))
        throw fault("InvalidStorageResponse");
      const item = response.data.results.find(result => result.key === key);
      if (!item) return null;
      validateScope(item.value);
      if (typeof item.writeLock !== "string" || !item.writeLock) throw fault("WriteLockUnavailable");
      return { kind, id, value: item.value, writeLock: item.writeLock };
    },
    async compareExchange(item, value) {
      if (!item || typeof item.writeLock !== "string" || !item.writeLock)
        throw fault("WriteLockUnavailable");
      validateScope(value);
      await api.setPrivateCustomItem(projectId, locator(item.kind, item.id),
        { key, value, writeLock: item.writeLock });
    },
    async create(kind, id, value) {
      validateScope(value);
      const customId = locator(kind, id);
      // Only this immutable guard is unconditionally initialized. A delayed
      // initializer may rotate its lock but never resets account/number data.
      let guards = await api.getPrivateCustomItems(projectId, customId, [guardKey]);
      if (!guards.data.results.some(item => item.key === guardKey)) {
        await api.setPrivateCustomItem(projectId, customId,
          { key: guardKey, value: { schemaVersion: 1, projectId, environmentId } });
      }
      // Guard MUST precede target read: reading target first permits a new guard
      // token to authorize overwriting another creator's intervening insert.
      guards = await api.getPrivateCustomItems(projectId, customId, [guardKey]);
      const guard = guards.data.results.find(item => item.key === guardKey);
      if (!guard || !guard.writeLock) throw fault("WriteLockUnavailable");
      validateScope(guard.value);
      const existing = await this.read(kind, id);
      if (existing) return existing;
      await api.setPrivateCustomItemBatch(projectId, customId, { data: [
        { key: guardKey, value: guard.value, writeLock: guard.writeLock },
        { key, value }
      ] });
      return this.read(kind, id);
    }
  };
}

module.exports = { createAccountStore, validateContext, validateScope, fault, projectId, environmentId, key, guardKey };
