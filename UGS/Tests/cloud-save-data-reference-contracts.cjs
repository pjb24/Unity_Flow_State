// Keeps the human-readable Cloud Save reference aligned with local source. No UGS or Unity calls.
const assert = require("assert").strict;
const fs = require("fs");
const path = require("path");
const root = path.resolve(__dirname, "../..");
const read = relative => fs.readFileSync(path.join(root, relative), "utf8");
const reference = read("UGS/CLOUD_SAVE_DATA_REFERENCE.md");
const accountStore = read("UGS/CloudCode/account-store.js");
const receiptStore = read("UGS/CloudCode/receipt-store.js");
const legacyStore = read("UGS/CloudCode/legacy-account-store.js");
const retention = read("UGS/CloudCode/submission-retention-policy.js");

for (const value of ["fs_account_v1", "fs_creation_guard_v1", "fs_submission_ledger_v1", "fs_receipt_guard_v1", "manifest"])
  assert.ok(reference.includes(value), value);
for (const pattern of ["fs8-account-", "fs8-player-", "fs8-owner-", "fs8-number-", "fs8-ledger-", "fs8-allocator-public", "fs8-t-", "fs8-receipt-", "p2v-<검사 실행 ID>", "r-<논리 계정 ID>", "l-<논리 계정 ID>"])
  assert.ok(reference.includes(pattern), pattern);
assert.ok(reference.includes("180일"));
assert.ok(reference.includes("수동 편집·삭제·초기화하지 않는다"));
assert.ok(accountStore.includes('const key = "fs_account_v1"'));
assert.ok(accountStore.includes('const guardKey = "fs_creation_guard_v1"'));
assert.ok(receiptStore.includes('const guardKey = "fs_receipt_guard_v1"'));
assert.ok(receiptStore.includes('const manifestKey = "manifest"'));
assert.ok(legacyStore.includes('const ledgerKey = "fs_submission_ledger_v1"'));
assert.ok(retention.includes("180 * 24 * 60 * 60 * 1000"));
console.log("PASS Cloud Save human-readable reference: namespace, access classes, guards, receipts and retention match local source.");
