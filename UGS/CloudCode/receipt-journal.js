// Receipt journal over receipt-store. A journal append only changes one
// manifest/shard pair; locators are separate idempotent records for replay.
const { fault } = require("./account-store");
const { scope, shardKey } = require("./receipt-store");
const { isTerminalReceiptExpired } = require("./submission-retention-policy");
const maximumReceiptBytes = 1024, targetShardBytes = 256 * 1024;
// Jint's minimal Buffer shim intentionally has no byteLength; keep this
// portable UTF-8 counter in bundled Cloud Code and Node tests.
function bytes(value) {
  const text = JSON.stringify(value); let total = 0;
  for (let index = 0; index < text.length; index++) {
    const code = text.charCodeAt(index);
    if (code < 0x80) total++;
    else if (code < 0x800) total += 2;
    else if (code >= 0xd800 && code <= 0xdbff && index + 1 < text.length && text.charCodeAt(index + 1) >= 0xdc00 && text.charCodeAt(index + 1) <= 0xdfff) { total += 4; index++; }
    else total += 3;
  }
  return total;
}

function terminalReceipt(entry) {
  const value = { id: entry.id, payload: entry.payload, status: entry.status, reason: entry.reason, acceptedAt: entry.acceptedAt };
  if (!Number.isSafeInteger(value.acceptedAt) || value.acceptedAt <= 0 || bytes(value) > maximumReceiptBytes)
    throw fault("InvalidReceipt");
  return value;
}
function createReceiptJournal(receipts) {
  async function find(accountId, submissionId) {
    const locator = await receipts.readLocator(accountId, submissionId);
    if (!locator) return null;
    const shard = await receipts.read(locator.value.receiptContainer, locator.value.shard);
    if (!shard || !Array.isArray(shard.value.entries)) throw fault("ReceiptLocatorConflict");
    const receipt = shard.value.entries.find(entry => entry.id === submissionId.toLowerCase());
    if (!receipt) throw fault("ReceiptLocatorConflict");
    return receipt;
  }
  async function cleanupOneShard(accountId, bucket, now) {
    for (let serial = 1; serial <= 999; serial++) {
      const manifest = await receipts.read(receipts.receiptContainerId(accountId, bucket, serial), "manifest");
      if (!manifest) break;
      if (manifest.value.cleanup && Array.isArray(manifest.value.cleanup.expiredIds)) {
        for (const id of manifest.value.cleanup.expiredIds) {
          const locator = await receipts.readLocator(accountId, id);
          if (locator) await receipts.remove(locator);
        }
        await receipts.compareExchange(manifest, { ...manifest.value, cleanup: null });
        return { cleaned: 0, more: true };
      }
      for (const descriptor of manifest.value.shards || []) {
        const shard = await receipts.read(manifest.container, shardKey(descriptor.serial));
        if (!shard || !Array.isArray(shard.value.entries)) throw fault("ReceiptManifestConflict");
        const expired = shard.value.entries.filter(entry => isTerminalReceiptExpired(entry.acceptedAt, now));
        if (!expired.length) continue;
        const kept = shard.value.entries.filter(entry => !isTerminalReceiptExpired(entry.acceptedAt, now));
        const nextShard = { ...shard.value, entries: kept }, nextBytes = bytes(nextShard);
        const nextShards = manifest.value.shards.map(value => value.serial === descriptor.serial ?
          { ...value, bytes: nextBytes, count: kept.length } : value);
        await receipts.compareExchangeBatch(manifest.container, [
          { item: manifest, key: manifest.key, value: { ...manifest.value, shards: nextShards,
            receiptCount: manifest.value.receiptCount - expired.length,
            byteCount: Math.max(0, manifest.value.byteCount - expired.reduce((sum, entry) => sum + bytes(entry), 0)),
            cleanup: { bucket, serial, completedAt: now, expiredIds: expired.map(entry => entry.id) } } },
          { item: shard, key: shard.key, value: nextShard }
        ]);
        // Locator deletion is deliberately after the durable receipt delete.
        // An ambiguous failure leaves a missing receipt with a stale locator;
        // the next cleanup/replay rechecks it instead of treating it as success.
        for (const entry of expired) {
          const locator = await receipts.readLocator(accountId, entry.id);
          if (locator) await receipts.remove(locator);
        }
        const committed = await receipts.read(manifest.container, manifest.key);
        if (!committed) throw fault("ReceiptManifestConflict");
        await receipts.compareExchange(committed, { ...committed.value, cleanup: null });
        return { cleaned: expired.length, more: true };
      }
    }
    return { cleaned: 0, more: false };
  }
  async function append(accountId, bucket, entry) {
    const receipt = terminalReceipt(entry);
    for (let serial = 1; serial <= 999; serial++) {
      const manifest = await receipts.ensureManifest(accountId, bucket, serial);
      const container = manifest.container;
      const used = Array.isArray(manifest.value.shards) ? manifest.value.shards : [];
      const last = used.length ? used[used.length - 1] : null;
      const number = last && last.bytes + bytes(receipt) <= targetShardBytes ? last.serial : used.length + 1;
      if (number > 999) continue;
      const key = shardKey(number), existing = await receipts.read(container, key);
      const entries = existing ? existing.value.entries : [];
      const duplicate = entries.find(value => value.id === receipt.id);
      if (duplicate) {
        if (duplicate.payload !== receipt.payload || duplicate.status !== receipt.status) throw fault("SubmissionIdConflict");
        return { container, shard: key, receipt: duplicate };
      }
      const nextEntries = [...entries, receipt], shard = { ...scope(), version: 1, entries: nextEntries };
      const shardBytes = bytes(shard);
      if (shardBytes > targetShardBytes) continue;
      const nextShards = last && last.serial === number ? used.map(item => item.serial === number ? { ...item, bytes: shardBytes, count: nextEntries.length } : item) :
        [...used, { serial: number, bytes: shardBytes, count: nextEntries.length }];
      // Manifest and shard share a container, so this is one Cloud Save batch:
      // a timeout can be recovered by locator/shard re-read without a half pair.
      await receipts.compareExchangeBatch(container, [
        { item: manifest, key: manifest.key, value: { ...manifest.value, shards: nextShards,
          receiptCount: manifest.value.receiptCount + 1, byteCount: manifest.value.byteCount + bytes(receipt) } },
        { item: existing, key, value: shard }
      ]);
      await receipts.createLocator(accountId, receipt.id, { id: receipt.id, receiptContainer: container, shard: key, terminalAt: receipt.acceptedAt });
      return { container, shard: key, receipt };
    }
    throw fault("ReceiptCapacity");
  }
  return { append, find, cleanupOneShard };
}
module.exports = { createReceiptJournal, terminalReceipt, maximumReceiptBytes, targetShardBytes };
