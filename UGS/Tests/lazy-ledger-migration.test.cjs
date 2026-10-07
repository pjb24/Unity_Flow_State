const assert = require("node:assert/strict");
const { fixture } = require("./account-sdk-double.cjs");
const { createAccountStore, projectId, environmentId } = require("../CloudCode/account-store");
const { createReceiptStore, receiptContainerId, receiptKey } = require("../CloudCode/receipt-store");
const { migrateV1Ledger } = require("../CloudCode/lazy-ledger-migration");
(async () => {
  const f = fixture(), accounts = createAccountStore(f.context, f.save), receipts = createReceiptStore(f.context, f.save);
  const accountId = "11111111-1111-4111-8111-111111111111", id = "22222222-2222-4222-8222-222222222222";
  await accounts.create("ledger", accountId, { schemaVersion: 1, projectId, environmentId, version: 1, accountId,
    best: {}, active: "", entries: [{ id, payload: "[]", status: "Rejected", reason: "InvalidScore", acceptedAt: Date.UTC(2026, 9, 6) }] });
  const v2 = await migrateV1Ledger(accounts, receipts, await accounts.read("ledger", accountId));
  assert.equal(v2.version, 2); assert.equal(v2.migration.state, "Complete");
  const locator = await receipts.readLocator(accountId, id);
  const saved = await receipts.read(locator.value.receiptContainer, locator.value.shard);
  assert.equal(saved.value.entries[0].reason, "InvalidScore");
  console.log("PASS lazy ledger migration: receipt-first then v2 ledger CAS");
})().catch(error => { console.error(error); process.exitCode = 1; });
