// Provider-neutral receipt persistence over Cloud Save Private Custom data.
// It deliberately does not reuse account-store.locator: receipt containers use
// their own bounded custom-ID layout and may contain many keys.
const { projectId, environmentId, validateContext, fault } = require("./account-store");
const guardKey = "fs_receipt_guard_v1";
const manifestKey = "manifest";
const scope = () => ({ schemaVersion: 1, projectId, environmentId });
const uuid = /^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;

function receiptContainerId(accountId, bucket, serial) {
  if (!uuid.test(accountId) || !/^\d{6}$/.test(bucket) || !Number.isInteger(serial) || serial < 1 || serial > 999)
    throw fault("InvalidReceiptLocation");
  const id = `r-${accountId.toLowerCase()}-${bucket}-${String(serial).padStart(3, "0")}`;
  if (id.length > 50) throw fault("InvalidReceiptLocation");
  return id;
}
function locatorContainerId(accountId, prefix) {
  if (!uuid.test(accountId) || !/^[0-9a-f]{3}$/i.test(prefix)) throw fault("InvalidReceiptLocation");
  return `l-${accountId.toLowerCase()}-${prefix.toLowerCase()}`;
}
function receiptKey(submissionId) {
  if (!uuid.test(submissionId)) throw fault("InvalidSubmissionId");
  return `r-${submissionId.toLowerCase()}`;
}
function shardKey(serial) {
  if (!Number.isInteger(serial) || serial < 1 || serial > 999) throw fault("InvalidReceiptLocation");
  return `s-${String(serial).padStart(3, "0")}`;
}
function validateValue(value) {
  if (!value || value.schemaVersion !== 1 || value.projectId !== projectId || value.environmentId !== environmentId)
    throw fault("StoredScopeMismatch");
}

function createReceiptStore(context, api) {
  validateContext(context);
  if (!api) {
    const { DataApi } = require("@unity-services/cloud-save-1.4");
    api = new DataApi(context);
  }
  async function get(container, key) {
    const response = await api.getPrivateCustomItems(projectId, container, [key]);
    if (!response || !response.data || !Array.isArray(response.data.results)) throw fault("InvalidStorageResponse");
    const item = response.data.results.find(value => value.key === key);
    if (!item) return null;
    validateValue(item.value);
    if (typeof item.writeLock !== "string" || !item.writeLock) throw fault("WriteLockUnavailable");
    return { container, key, value: item.value, writeLock: item.writeLock };
  }
  return {
    receiptContainerId, locatorContainerId, receiptKey, shardKey,
    read: get,
    async compareExchange(item, value) {
      if (!item || !item.writeLock) throw fault("WriteLockUnavailable");
      validateValue(value);
      await api.setPrivateCustomItem(projectId, item.container, { key: item.key, value, writeLock: item.writeLock });
    },
    async compareExchangeBatch(container, changes) {
      if (!Array.isArray(changes) || changes.length < 1 || changes.length > 20) throw fault("InvalidReceiptBatch");
      const data = changes.map(change => {
        if (!change || !change.key || !change.value) throw fault("InvalidReceiptBatch");
        validateValue(change.value);
        if (change.item && (change.item.container !== container || !change.item.writeLock)) throw fault("WriteLockUnavailable");
        return change.item ? { key: change.key, value: change.value, writeLock: change.item.writeLock } : { key: change.key, value: change.value };
      });
      await api.setPrivateCustomItemBatch(projectId, container, { data });
    },
    async create(container, key, value) {
      validateValue(value);
      let guard = await get(container, guardKey);
      if (!guard) {
        await api.setPrivateCustomItem(projectId, container, { key: guardKey, value: scope() });
        guard = await get(container, guardKey);
      }
      const existing = await get(container, key);
      if (existing) return existing;
      // The guard's CAS serializes competing creators without overwriting an
      // already-created receipt or manifest.
      await api.setPrivateCustomItemBatch(projectId, container, { data: [
        { key: guardKey, value: guard.value, writeLock: guard.writeLock }, { key, value }
      ] });
      return get(container, key);
    },
    async remove(item) {
      if (!item || !item.writeLock) throw fault("WriteLockUnavailable");
      await api.deletePrivateCustomItem(projectId, item.container, item.key, item.writeLock);
    },
    async ensureManifest(accountId, bucket, serial) {
      const container = receiptContainerId(accountId, bucket, serial);
      return this.create(container, manifestKey, { ...scope(), version: 1, accountId: accountId.toLowerCase(), bucket,
        serial, shards: [], receiptCount: 0, byteCount: 0, cleanup: null });
    },
    async readLocator(accountId, submissionId) {
      return get(locatorContainerId(accountId, submissionId.slice(0, 3)), receiptKey(submissionId));
    },
    async createLocator(accountId, submissionId, value) {
      if (!value || value.id !== submissionId.toLowerCase()) throw fault("InvalidReceiptLocation");
      return this.create(locatorContainerId(accountId, submissionId.slice(0, 3)), receiptKey(submissionId),
        { ...scope(), ...value });
    },
    scope
  };
}
module.exports = { createReceiptStore, receiptContainerId, locatorContainerId, receiptKey, shardKey, guardKey, manifestKey, scope };
