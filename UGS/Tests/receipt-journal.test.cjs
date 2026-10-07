const assert = require("node:assert/strict");
const { fixture } = require("./account-sdk-double.cjs");
const { createReceiptStore } = require("../CloudCode/receipt-store");
const { createReceiptJournal } = require("../CloudCode/receipt-journal");
(async () => {
  const f = fixture(), account = "11111111-1111-4111-8111-111111111111", id = "22222222-2222-4222-8222-222222222222";
  const store = createReceiptStore(f.context, f.save), journal = createReceiptJournal(store);
  const entry = { id, payload: "[]", status: "Rejected", reason: "InvalidScore", acceptedAt: 1 };
  const first = await journal.append(account, "202610", entry);
  const replay = await journal.append(account, "202610", entry);
  assert.equal(first.shard, "s-001"); assert.equal(replay.receipt.id, id);
  assert.equal((await store.readLocator(account, id)).value.receiptContainer, first.container);
  await assert.rejects(journal.append(account, "202610", { ...entry, payload: "[1]" }));
  const cleanup = await journal.cleanupOneShard(account, "202610", 180 * 24 * 60 * 60 * 1000 + 1);
  assert.equal(cleanup.cleaned, 1); assert.equal(await store.readLocator(account, id), null);
  assert.equal(await journal.find(account, id), null);
  const lost = fixture(), lostStore = createReceiptStore(lost.context, lost.save), lostJournal = createReceiptJournal(lostStore);
  await lostJournal.append(account, "202610", entry);
  lost.state.loseAt = lost.state.writes + 2; // receipt CAS succeeds; locator delete response is lost.
  await assert.rejects(lostJournal.cleanupOneShard(account, "202610", 180 * 24 * 60 * 60 * 1000 + 1));
  lost.state.loseAt = 0;
  assert.equal((await lostJournal.cleanupOneShard(account, "202610", 180 * 24 * 60 * 60 * 1000 + 1)).more, true);
  assert.equal(await lostStore.readLocator(account, id), null);
  const race = fixture(), raceStore = createReceiptStore(race.context, race.save), raceJournal = createReceiptJournal(raceStore);
  const nextId = "33333333-3333-4333-8333-333333333333";
  await raceJournal.append(account, "202610", entry);
  await Promise.allSettled([
    raceJournal.cleanupOneShard(account, "202610", 180 * 24 * 60 * 60 * 1000 + 1),
    raceJournal.append(account, "202610", { ...entry, id: nextId, acceptedAt: 180 * 24 * 60 * 60 * 1000 + 2 })
  ]);
  // A single CAS loser is allowed; retry converges without reviving the old ID.
  await raceJournal.cleanupOneShard(account, "202610", 180 * 24 * 60 * 60 * 1000 + 1);
  await raceJournal.append(account, "202610", { ...entry, id: nextId, acceptedAt: 180 * 24 * 60 * 60 * 1000 + 2 });
  assert.equal(await raceStore.readLocator(account, id), null);
  assert.equal((await raceJournal.find(account, nextId)).id, nextId);
  console.log("PASS receipt journal: shard append, locator replay, cleanup and payload conflict");
})().catch(error => { console.error(error); process.exitCode = 1; });
