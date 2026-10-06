// Deterministic interleavings over production services, not duplicated policies.
const assert = require("node:assert/strict");
const { ready } = require("./transfer-completion.test.cjs");
const { clone } = require("./account-sdk-double.cjs");
const { createAccountService } = require("../CloudCode/account-service");
const { createAccountProvisioningService } = require("../CloudCode/account-provisioning-service");
const { createLegacyAccountStore } = require("../CloudCode/legacy-account-store");
const { createTransferLifecycleService } = require("../CloudCode/transfer-lifecycle-service");
const { createTransferStartService } = require("../CloudCode/transfer-start-service");
const secret = Buffer.alloc(32, 7).toString("base64");
const tests = []; const test = (name, run) => tests.push({ name, run });
async function addPlayer(f, playerId) {
  const context = { ...f.context, playerId };
  await createAccountProvisioningService(f.store, createLegacyAccountStore(context, f.save, f.leaderboard)).ensure(context);
  return context;
}
function lifecycle(f) { return createTransferLifecycleService(f.store, () => f.now); }
function start(f, id = "00000000-0000-4000-8000-000000000002") {
  return createTransferStartService(f.store, async () => secret, { clock: () => f.now, idGenerator: () => id });
}
async function authority(f, player, expectedAccountId) {
  if (expectedAccountId) assert.equal((await createAccountService(f.store).resolve(player)).account.value.accountId, expectedAccountId);
  else await assert.rejects(createAccountService(f.store).resolve(player));
}
test("simultaneous two B verification allows only one terminal connection", async () => {
  const f = await ready(); const b2 = await addPlayer(f, "B2");
  await Promise.allSettled([f.run(), f.run(b2)]);
  if (f.c().transfer.status !== "Completed") { f.now += 5000; await f.run(); }
  const winner = f.c().currentPlayerId;
  const loser = winner === "B" ? b2 : f.b;
  assert.equal((await f.run({ ...f.b, playerId: winner })).status, "AlreadyCompleted");
  assert.equal((await f.run(loser)).status, "InvalidCredential");
  await authority(f, { ...f.b, playerId: winner }, f.cId);
  assert.notEqual(f.value(`fs8-player-${loser.playerId}`).accountId, f.cId);
});
test("later second B fences a paused first B terminal token and restores loser", async () => {
  const f = await ready(); const b2 = await addPlayer(f, "B2"); let injected = false;
  f.state.beforeWrite = async op => {
    if (!injected && op.id === `fs8-account-${f.cId}` && op.data[0].value.status === "TransferCompleting") {
      injected = true; f.now += 5000; assert.equal((await f.run(b2)).status, "Success");
    }
  };
  await assert.rejects(f.run());
  assert.equal(f.c().currentPlayerId, "B2"); assert.equal(f.old().status, "Active");
  assert.equal(f.value("fs8-player-B").transferOperation, undefined);
  await authority(f, f.b, f.oldId); await authority(f, b2, f.cId); await authority(f, f.context, null);
});
test("same B simultaneous completion retries have one revision and one detached account", async () => {
  const f = await ready(); await Promise.allSettled([f.run(), f.run()]);
  if (f.c().transfer.status !== "Completed") { f.now += 5000; await f.run(); }
  await f.run(); assert.equal(f.c().connectionRevision, 2); assert.equal(f.old().connectionRevision, 2);
  assert.equal(f.c().currentPlayerId, "B"); assert.equal(f.old().currentPlayerId, null);
});
test("one B racing for two source accounts cannot join both", async () => {
  const f = await ready(); const d = await addPlayer(f, "D");
  const dId = f.value("fs8-player-D").accountId; const credential = await start(f).start(d);
  await Promise.allSettled([f.run(), f.run(f.b, credential.code, credential.verificationValue)]);
  const completed = [f.c(), f.value(`fs8-account-${dId}`)].filter(a => a.transfer.status === "Completed");
  assert.equal(completed.length, 1); assert.equal(completed[0].currentPlayerId, "B");
  assert.equal(f.value("fs8-player-B").accountId, completed[0].accountId);
});
test("complete winning terminal write prevents late cancel and expiry restoration", async () => {
  const f = await ready(); await f.run(); const before = clone(f.c()); f.now = before.transfer.expiresAtMilliseconds;
  await assert.rejects(lifecycle(f).cancel(f.context, before.transfer.transferId));
  await assert.rejects(lifecycle(f).expire(f.context, before.transfer.transferId));
  assert.deepEqual(f.c(), before); await authority(f, f.context, null); await authority(f, f.b, f.cId);
});
test("expiry winning a paused completion restores both original connections", async () => {
  const f = await ready(); let injected = false;
  f.state.beforeWrite = async op => {
    if (!injected && op.id === `fs8-account-${f.cId}` && op.data[0].value.status === "TransferCompleting") {
      injected = true; f.now = f.c().transfer.expiresAtMilliseconds; await lifecycle(f).status(f.context);
    }
  };
  await assert.rejects(f.run()); assert.equal(f.c().transfer.status, "Expired");
  assert.equal(f.old().status, "Active"); await authority(f, f.b, f.oldId); await authority(f, f.context, f.cId);
});
test("reissue winning completion CAS keeps source Pending and B safely reserved until new code retry", async () => {
  const f = await ready(); let injected = false; let replacement;
  f.state.beforeWrite = async op => {
    if (!injected && op.id === `fs8-account-${f.cId}` && op.data[0].value.status === "TransferCompleting") {
      injected = true; replacement = await start(f).reissue(f.context);
    }
  };
  await assert.rejects(f.run()); await authority(f, f.b, null);
  assert.equal(f.c().transfer.credentialRevision, 2); f.now += 5000;
  assert.equal((await f.run(f.b, replacement.code, replacement.verificationValue)).status, "Success");
});
test("late source A submission and cached pre-transfer token cannot mutate completed C", async () => {
  const f = await ready(); const oldToken = await f.store.read("account", f.cId); await f.run();
  await assert.rejects(f.store.compareExchange(oldToken, { ...oldToken.value, onlineOperation: { id: "late" } }));
  await assert.rejects(createAccountService(f.store).reserveSubmission(f.context, "00000000-0000-4000-8000-000000000009"));
  await assert.rejects(createAccountService(f.store).getPublicNumber(f.context));
  assert.equal(f.c().onlineOperation, undefined);
});
test("target B's own transfer winning the Account fence does not strand either transfer", async () => {
  const f = await ready(); let injected = false; let ownCredential;
  f.state.beforeWrite = async op => {
    if (!injected && op.id === "fs8-player-B" && op.data[0].value.transferOperation) {
      injected = true; ownCredential = await start(f).start(f.b);
    }
  };
  await assert.rejects(f.run());
  await lifecycle(f).cancel(f.b, f.old().transfer.transferId);
  f.now += 5000; assert.equal((await f.run()).status, "Success"); assert.ok(ownCredential);
});
test("failure at every completion SDK read recovers after restart without mixed authority", async () => {
  const baseline = await ready(); let count = 0;
  baseline.state.afterRead = async () => { count++; }; await baseline.run();
  for (let failAt = 1; failAt <= count; failAt++) {
    const f = await ready(); let reads = 0;
    f.state.afterRead = async () => { if (++reads === failAt) throw Error("injected-read-failure"); };
    await assert.rejects(f.run(), `read ${failAt}`); f.state.afterRead = null; f.now += 5000;
    const reply = await f.run(); assert.ok(["Success", "AlreadyCompleted"].includes(reply.status), `read ${failAt}`);
    await authority(f, f.context, null); await authority(f, f.b, f.cId);
  }
  console.log(`checked ${count} completion SDK read boundaries`);
});
test("rollback write failure before/after commit converges after cancelled transfer retry", async () => {
  for (const phase of ["old", "binding"]) for (const mode of ["before", "after"]) {
    const f = await ready(); let injected = false; let failed = false;
    f.state.beforeWrite = async op => {
      if (!injected && op.id === `fs8-account-${f.cId}` && op.data[0].value.status === "TransferCompleting") {
        injected = true; await lifecycle(f).cancel(f.context, f.c().transfer.transferId);
      }
      const oldRollback = op.id === `fs8-account-${f.oldId}` && op.data[0].value.status === "Active";
      const bindingRollback = op.id === "fs8-player-B" && !op.data[0].value.transferOperation;
      if (injected && !failed && (phase === "old" ? oldRollback : bindingRollback)) {
        failed = true; f.state[mode === "before" ? "failAt" : "loseAt"] = f.state.writes;
      }
    };
    await assert.rejects(f.run()); assert.equal(failed, true);
    f.state.beforeWrite = null; f.state.failAt = 0; f.state.loseAt = 0;
    assert.equal((await f.run()).status, "InvalidCredential");
    await authority(f, f.b, f.oldId); await authority(f, f.context, f.cId);
    assert.equal(f.old().joiningTransfer, undefined); assert.equal(f.value("fs8-player-B").transferOperation, undefined);
  }
});
async function interruptedTerminal() {
  const f = await ready(); f.state.loseAt = 4;
  await assert.rejects(f.run()); f.state.loseAt = 0; f.state.writes = 0;
  assert.equal(f.c().status, "TransferCompleting");
  return f;
}
test("every committed-completion read failure recovers without any Secret or raw input", async () => {
  const baseline = await interruptedTerminal(); let count = 0;
  baseline.state.afterRead = async () => { count++; }; await baseline.service(null).recover(baseline.b);
  for (let at = 1; at <= count; at++) {
    const f = await interruptedTerminal(); let reads = 0;
    f.state.afterRead = async () => { if (++reads === at) throw Error("injected-recovery-read"); };
    await assert.rejects(f.service(null).recover(f.b)); f.state.afterRead = null;
    assert.equal((await f.service(null).recover(f.b)).status, "AlreadyCompleted");
    await authority(f, f.b, f.cId); await authority(f, f.context, null);
  }
  console.log(`checked ${count} recovery SDK read boundaries`);
});
test("every mapping-recovery write interruption before/after commit is restart safe", async () => {
  const baseline = await interruptedTerminal(); await baseline.service(null).recover(baseline.b);
  const count = baseline.state.writes;
  for (const mode of ["failAt", "loseAt"]) for (let at = 1; at <= count; at++) {
    const f = await interruptedTerminal(); f.state[mode] = at;
    await assert.rejects(f.service(null).recover(f.b)); f.state[mode] = 0;
    assert.equal((await f.service(null).recover(f.b)).status, "AlreadyCompleted");
    assert.equal(f.c().connectionRevision, 2); assert.equal(f.old().connectionRevision, 2);
    await authority(f, f.b, f.cId); await authority(f, f.context, null);
  }
  console.log(`checked ${count} recovery write boundaries before/after commit`);
});
test("concurrent authenticated restart recovery never doubles detach or connection revision", async () => {
  const f = await interruptedTerminal();
  await Promise.allSettled([f.service(null).recover(f.b), f.service(null).recover(f.b)]);
  await f.service(null).recover(f.b);
  assert.equal(f.c().connectionRevision, 2); assert.equal(f.old().connectionRevision, 2);
  await authority(f, f.b, f.cId);
});
test("uncommitted reservation cannot recover a connection without credential proof", async () => {
  const f = await ready(); f.state.loseAt = 3; await assert.rejects(f.run()); f.state.loseAt = 0;
  assert.deepEqual(await f.service(null).recover(f.b), { status: "TransferPending", reason: "TransferRecoveryPending" });
  assert.equal(f.c().currentPlayerId, "A"); await authority(f, f.b, null);
  f.now = f.c().transfer.expiresAtMilliseconds;
  assert.equal(await f.service(null).recover(f.b), null);
  assert.equal(f.c().transfer.status, "Expired"); await authority(f, f.b, f.oldId); await authority(f, f.context, f.cId);
});
test("committed recovery is unavailable to another B or wrong environment", async () => {
  const f = await interruptedTerminal(); const b2 = await addPlayer(f, "B2"); const before = clone(f.c());
  assert.equal(await f.service(null).recover(b2), null); assert.deepEqual(f.c(), before);
  await assert.rejects(f.service(null).recover({ ...f.b, environmentId: "other" }));
  assert.deepEqual(f.c(), before);
});
test("A restart still sees inactive completion after B starts another transfer", async () => {
  const f = await ready(); await f.run(); await start(f).start(f.b);
  assert.deepEqual(await lifecycle(f).status(f.context), { status: "Inactive", transferStatus: "Completed",
    transferId: "00000000-0000-4000-8000-000000000001" });
  await authority(f, f.context, null);
});
test("historical completion reply is discarded if B loses current C during receipt lookup", async () => {
  const f = await ready(); const d = await addPlayer(f, "D"); await f.run(); const next = await start(f).start(f.b);
  let injected = false;
  f.state.afterRead = async op => {
    if (!injected && op.id === "fs8-player-B") {
      injected = true; await f.run(d, next.code, next.verificationValue);
    }
  };
  assert.equal((await f.run()).status, "InvalidCredential"); assert.equal(f.c().currentPlayerId, "D");
});
function injectCancel(f) {
  let injected = false;
  f.state.beforeWrite = async op => {
    if (!injected && op.id === `fs8-account-${f.cId}` && op.data[0].value.status === "TransferCompleting") {
      injected = true; await lifecycle(f).cancel(f.context, f.c().transfer.transferId);
    }
  };
}
test("all cancellation and rollback read failures converge to the committed winner", async () => {
  const baseline = await ready(); let count = 0; injectCancel(baseline);
  baseline.state.afterRead = async () => { count++; }; await assert.rejects(baseline.run());
  for (let at = 1; at <= count; at++) {
    const f = await ready(); injectCancel(f); let reads = 0;
    f.state.afterRead = async () => { if (++reads === at) throw Error("injected-rollback-read"); };
    await assert.rejects(f.run()); f.state.afterRead = null; f.state.beforeWrite = null; f.now += 5000;
    const result = await f.run();
    if (f.c().transfer.status === "Cancelled") {
      assert.equal(result.status, "InvalidCredential"); await authority(f, f.b, f.oldId); await authority(f, f.context, f.cId);
    } else {
      assert.ok(["Success", "AlreadyCompleted"].includes(result.status)); await authority(f, f.b, f.cId); await authority(f, f.context, null);
    }
  }
  console.log(`checked ${count} cancellation/rollback SDK read boundaries`);
});
test("own-transfer reservation release survives response loss without cancelling B's transfer", async () => {
  for (const mode of ["before", "after"]) {
    const f = await ready(); let started = false; let failed = false;
    f.state.beforeWrite = async op => {
      if (!started && op.id === "fs8-player-B" && op.data[0].value.transferOperation) {
        started = true; await start(f).start(f.b);
      }
      if (started && !failed && op.id === "fs8-player-B" && !op.data[0].value.transferOperation) {
        failed = true; f.state[mode === "before" ? "failAt" : "loseAt"] = f.state.writes;
      }
    };
    await assert.rejects(f.run()); f.state.beforeWrite = null; f.state.failAt = 0; f.state.loseAt = 0;
    assert.equal(f.old().transfer.status, "TransferPending");
    await f.service(null).recover(f.b);
    assert.equal(f.value("fs8-player-B").transferOperation, undefined);
    await lifecycle(f).cancel(f.b, f.old().transfer.transferId);
    f.now += 5000; assert.equal((await f.run()).status, "Success");
  }
});
test("late Joining write after old cancellation/new transfer/recovery cannot strand B", async () => {
  const f = await ready(); let injected = false;
  f.state.beforeWrite = async op => {
    if (!injected && op.id === `fs8-account-${f.oldId}` && op.data[0].value.status === "TransferJoining") {
      injected = true; await lifecycle(f).cancel(f.context, f.c().transfer.transferId);
      await start(f).start(f.context); await f.service(null).recover(f.b);
    }
  };
  await assert.rejects(f.run()); assert.equal(f.old().status, "Active");
  assert.equal(f.value("fs8-player-B").transferOperation, undefined);
  await authority(f, f.b, f.oldId);
});
test("a worker seeing a new Account token before binding clear cannot reuse revoked Joining", async () => {
  const f = await ready();
  const op = { targetAccountId: f.cId, transferId: f.c().transfer.transferId, oldAccountId: f.oldId, oldRevision: 1 };
  // Persisted phase between rollback Account revocation and binding clear.
  // Any worker resuming with an old verified source must reject the marker.
  f.value("fs8-player-B").transferOperation = clone(op);
  f.old().revokedJoiningTransfer = clone(op);
  await assert.rejects(f.run()); assert.equal(f.old().status, "Active"); assert.equal(f.c().currentPlayerId, "A");
  await lifecycle(f).cancel(f.context, op.transferId); await f.service(null).recover(f.b);
  assert.equal(f.value("fs8-player-B").transferOperation, undefined); await authority(f, f.b, f.oldId);
});
(async () => {
  let passed = 0;
  for (const t of tests) { try { await t.run(); passed++; console.log(`PASS ${t.name}`); }
    catch (error) { console.error(`FAIL ${t.name}`, error); process.exitCode = 1; } }
  console.log(`${passed}/${tests.length} passed`);
})();
