// Writes the pure v1→v2 plan in a resumable order: terminal receipts first,
// then one ledger CAS. Extra receipt writes after a lost response are harmless
// because receipt-store.create is create-if-absent and payload is verified.
const { fault } = require("./account-store");
const { plan } = require("./ledger-v2-migration");
const { createReceiptJournal } = require("./receipt-journal");

async function migrateV1Ledger(store, receipts, ledgerItem) {
  const accountId = ledgerItem && ledgerItem.value && ledgerItem.value.accountId;
  const prepared = plan(ledgerItem.value, accountId);
  const journal = createReceiptJournal(receipts);
  for (const entry of prepared.receipts) await journal.append(accountId, entry.bucket, entry);
  prepared.ledger.migration = { ...prepared.ledger.migration, state: "Complete", cursor: prepared.receipts.length };
  await store.compareExchange(ledgerItem, prepared.ledger);
  return prepared.ledger;
}
module.exports = { migrateV1Ledger };
