// Cross-entity saga: PlayerBinding reservation -> B Account fence -> C terminal
// CAS -> detach old B -> repair bindings -> activate C. No multi-item transaction.
const crypto = require("crypto");
const { validateContext, fault } = require("./account-store");
const { createTransferCryptography } = require("./transfer-cryptography");
const { createTransferVerificationService } = require("./transfer-verification-service");
const { validatePendingTransfer } = require("./transfer-state");
const policy = require("./transfer-credential-policy");
const resolution = require("./transfer-resolution-policy");
const numbers = require("./public-player-number-policy");
const { createAccountService } = require("./account-service");
function createTransferCompletionService(store, secretProvider, clock = Date.now) {
  function now() {
    const value = clock();
    if (!Number.isSafeInteger(value) || value < 0) throw fault("InvalidServerTime");
    return value;
  }
  function sameOperation(left, right) {
    return !!left && left.targetAccountId === right.targetAccountId && left.transferId === right.transferId &&
      left.oldAccountId === right.oldAccountId && left.oldRevision === right.oldRevision;
  }
  async function releaseOwnPendingFence(context, op) {
    const old = await store.read("account", op.oldAccountId);
    // Own TransferPending fenced every paused Active->Joining CAS. Do not
    // cancel that transfer or release an Active/Joining completion reservation.
    if (!old || old.value.status !== "TransferPending" || old.value.joiningTransfer ||
        old.value.currentPlayerId !== context.playerId || old.value.connectionRevision !== op.oldRevision) return false;
    const binding = await store.read("player", context.playerId);
    if (!binding || !sameOperation(binding.value.transferOperation, op)) return false;
    const restored = { ...binding.value }; delete restored.transferOperation;
    await store.compareExchange(binding, restored);
    return true;
  }
  async function rollback(context, op) {
    const target = await store.read("account", op.targetAccountId);
    if (!target || target.value.transfer && target.value.transfer.status === "Completed" &&
        target.value.transfer.transferId === op.transferId &&
        target.value.transfer.completedPlayerId === context.playerId) throw fault("TransferUnavailable");
    const old = await store.read("account", op.oldAccountId);
    if (!old) throw fault("AccountUnavailable");
    if (sameOperation(old.value.joiningTransfer, op)) {
      if (old.value.status !== "TransferJoining" || old.value.currentPlayerId !== context.playerId ||
          old.value.connectionRevision !== op.oldRevision) throw fault("AccountConflict");
      const restored = { ...old.value, status: "Active", revokedJoiningTransfer: op }; delete restored.joiningTransfer;
      await store.compareExchange(old, restored);
    } else {
      if (old.value.status !== "Active" || old.value.currentPlayerId !== context.playerId ||
          old.value.connectionRevision !== op.oldRevision) throw fault("AccountConflict");
      // Persist revocation before clearing the binding. A paused Joining CAS
      // is fenced, and a worker reading the new token during the clear gap
      // must also reject this old operation (not merely rely on token rotation).
      await store.compareExchange(old, { ...old.value, revokedJoiningTransfer: op });
    }
    const binding = await store.read("player", context.playerId);
    if (!binding || !sameOperation(binding.value.transferOperation, op)) throw fault("AccountConflict");
    const restored = { ...binding.value }; delete restored.transferOperation;
    await store.compareExchange(binding, restored);
  }
  async function reserveTarget(context, source) {
    let binding = await store.read("player", context.playerId);
    if (!binding || binding.value.playerId !== context.playerId || binding.value.status === "Inactive")
      throw fault("ActiveDeviceRequired");
    let op = binding.value.transferOperation;
    if (op) {
      if (op.targetAccountId !== source.value.accountId || op.transferId !== source.value.transfer.transferId)
        throw fault("AccountBusy");
    } else {
      const revision = binding.value.connectionRevision;
      if (binding.value.accountId === source.value.accountId || !Number.isSafeInteger(revision) || revision < 1 ||
          !Number.isSafeInteger(revision + 1)) throw fault("ActiveDeviceRequired");
      const initial = await store.read("account", binding.value.accountId);
      if (!initial || initial.value.currentPlayerId !== context.playerId || initial.value.connectionRevision !== revision ||
          initial.value.status !== "Active" || initial.value.onlineOperation) throw fault("AccountBusy");
      const initialLedger = await store.read("ledger", binding.value.accountId);
      if (!initialLedger || initialLedger.value.accountId !== binding.value.accountId || initialLedger.value.active ||
          !Array.isArray(initialLedger.value.entries) || initialLedger.value.entries.some(e => e.status === "Pending"))
        throw fault("AccountBusy");
      op = { targetAccountId: source.value.accountId, transferId: source.value.transfer.transferId,
        oldAccountId: binding.value.accountId, oldRevision: revision };
      await store.compareExchange(binding, { ...binding.value, transferOperation: op });
    }
    const old = await store.read("account", op.oldAccountId);
    if (!old || old.value.accountId !== op.oldAccountId || old.value.currentPlayerId !== context.playerId ||
        old.value.connectionRevision !== op.oldRevision) throw fault("AccountConflict");
    if (sameOperation(old.value.revokedJoiningTransfer, op)) throw fault("AccountConflict");
    const confirmedBinding = await store.read("player", context.playerId);
    if (!confirmedBinding || confirmedBinding.value.accountId !== op.oldAccountId ||
        confirmedBinding.value.connectionRevision !== op.oldRevision ||
        !sameOperation(confirmedBinding.value.transferOperation, op)) throw fault("AccountConflict");
    if (old.value.status === "TransferJoining" && sameOperation(old.value.joiningTransfer, op)) return op;
    if (old.value.status !== "Active" || old.value.onlineOperation) throw fault("AccountBusy");
    const ledger = await store.read("ledger", op.oldAccountId);
    if (!ledger || ledger.value.accountId !== op.oldAccountId || typeof ledger.value.active !== "string" ||
        !Array.isArray(ledger.value.entries)) throw fault("LedgerConflict");
    if (ledger.value.active || ledger.value.entries.some(e => e.status === "Pending")) throw fault("AccountBusy");
    await store.compareExchange(old, { ...old.value, status: "TransferJoining", joiningTransfer: op });
    return op;
  }
  async function finish(context, target, repeated) {
    const t = target.value.transfer;
    if (!t || t.status !== "Completed" || t.completedPlayerId !== context.playerId ||
        t.credentialActive !== false || target.value.currentPlayerId !== context.playerId ||
        target.value.connectionRevision !== t.completedConnectionRevision ||
        !Number.isSafeInteger(t.completedConnectionRevision) || t.completedConnectionRevision !== t.connectionRevision + 1 ||
        !["TransferCompleting", "Active"].includes(target.value.status)) throw fault("TransferUnavailable");
    const op = t.targetOperation;
    if (!op || op.targetAccountId !== target.value.accountId || op.transferId !== t.transferId)
      throw fault("TransferUnavailable");
    const receipt = await store.create("receipt", t.transferId, { schemaVersion: target.value.schemaVersion,
      projectId: target.value.projectId, environmentId: target.value.environmentId,
      accountId: target.value.accountId, transferId: t.transferId, credentialRevision: t.credentialRevision,
      codeDigest: t.codeDigest, credentialHmac: t.credentialHmac, completedPlayerId: context.playerId,
      sourcePlayerId: t.sourcePlayerId,
      completedConnectionRevision: t.completedConnectionRevision, publicPlayerNumber: target.value.publicPlayerNumber });
    for (const name of ["accountId", "transferId", "credentialRevision", "codeDigest", "credentialHmac", "completedPlayerId",
      "completedConnectionRevision", "publicPlayerNumber", "sourcePlayerId"])
      if (receipt.value[name] !== (name === "accountId" || name === "publicPlayerNumber" ? target.value[name] : t[name]))
        throw fault("AccountConflict");
    let old = await store.read("account", op.oldAccountId);
    if (!old) throw fault("AccountUnavailable");
    if (old.value.status === "TransferJoining" && sameOperation(old.value.joiningTransfer, op) &&
        old.value.currentPlayerId === context.playerId && old.value.connectionRevision === op.oldRevision) {
      const detached = { ...old.value, status: "Detached", currentPlayerId: null,
        connectionRevision: op.oldRevision + 1, detachedByTransferId: t.transferId };
      delete detached.joiningTransfer;
      await store.compareExchange(old, detached);
    } else if (old.value.status !== "Detached" || old.value.currentPlayerId !== null ||
        old.value.connectionRevision !== op.oldRevision + 1 || old.value.detachedByTransferId !== t.transferId)
      throw fault("AccountConflict");
    let binding = await store.read("player", context.playerId);
    if (!binding) throw fault("AccountUnavailable");
    if (binding.value.accountId === op.oldAccountId && binding.value.connectionRevision === op.oldRevision &&
        sameOperation(binding.value.transferOperation, op)) {
      const updated = { ...binding.value, accountId: target.value.accountId,
        connectionRevision: t.completedConnectionRevision, status: "Active" };
      delete updated.transferOperation;
      await store.compareExchange(binding, updated);
    } else if (binding.value.accountId !== target.value.accountId ||
        binding.value.connectionRevision !== t.completedConnectionRevision || binding.value.transferOperation ||
        binding.value.status === "Inactive") throw fault("AccountConflict");
    const sourceBinding = await store.read("player", t.sourcePlayerId);
    if (!sourceBinding || sourceBinding.value.accountId !== target.value.accountId ||
        sourceBinding.value.connectionRevision !== t.connectionRevision) throw fault("AccountConflict");
    if (sourceBinding.value.status !== "Inactive")
      await store.compareExchange(sourceBinding, { ...sourceBinding.value, status: "Inactive", inactiveByTransferId: t.transferId });
    target = await store.read("account", target.value.accountId);
    if (!target || target.value.transfer.transferId !== t.transferId || target.value.transfer.status !== "Completed" ||
        target.value.currentPlayerId !== context.playerId || target.value.connectionRevision !== t.completedConnectionRevision)
      throw fault("TransferUnavailable");
    if (target.value.status === "TransferCompleting")
      await store.compareExchange(target, { ...target.value, status: "Active" });
    target = await store.read("account", target.value.accountId);
    binding = await store.read("player", context.playerId);
    if (!target || target.value.status !== "Active" || target.value.currentPlayerId !== context.playerId ||
        target.value.connectionRevision !== t.completedConnectionRevision || target.value.transfer.transferId !== t.transferId ||
        !binding || binding.value.accountId !== target.value.accountId ||
        binding.value.connectionRevision !== t.completedConnectionRevision || binding.value.transferOperation)
      throw fault("TransferUnavailable");
    const number = target.value.publicPlayerNumber;
    if (!numbers.tryNormalizePublicPlayerNumber(number)) throw fault("PublicNumberUnavailable");
    const confirmedNumber = await createAccountService(store).getPublicNumber(context);
    return { status: repeated ? "AlreadyCompleted" : "Success", publicPlayerNumber: confirmedNumber.publicPlayerNumber };
  }
  return {
    async recover(context) {
      // Authentication + committed terminal ownership, not raw credentials,
      // recovers after a restart. No endpoint can choose an Account ID here.
      validateContext(context);
      const binding = await store.read("player", context.playerId);
      if (!binding || binding.value.playerId !== context.playerId) return null;
      const op = binding.value.transferOperation;
      const target = await store.read("account", op ? op.targetAccountId : binding.value.accountId);
      if (!target) throw fault("AccountUnavailable");
      const t = target.value.transfer;
      if (t && t.status === "Completed" && t.completedPlayerId === context.playerId) {
        if (op && !sameOperation(op, t.targetOperation) || !op &&
            (binding.value.accountId !== target.value.accountId ||
              binding.value.connectionRevision !== t.completedConnectionRevision || binding.value.status === "Inactive"))
          throw fault("AccountConflict");
        return finish(context, target, true);
      }
      if (!op) return null;
      if (await releaseOwnPendingFence(context, op)) return null;
      if (!t || t.transferId !== op.transferId || ["Cancelled", "Expired", "Completed"].includes(t.status)) {
        await rollback(context, op); return null;
      }
      const pending = validatePendingTransfer(target, now(), true);
      if (now() >= pending.expiresAtMilliseconds) {
        const expired = { ...pending, status: "Expired", credentialActive: false, terminalAtMilliseconds: now() };
        delete expired.codeDigest; delete expired.credentialHmac;
        await store.compareExchange(target, { ...target.value, status: "Active", transfer: expired });
        await rollback(context, op); return null;
      }
      return { status: "TransferPending", reason: "TransferRecoveryPending" };
    },
    async complete(context, code, verificationValue) {
      validateContext(context);
      const normalized = policy.tryNormalizeTransferCode(code);
      const input = policy.tryNormalizeVerificationValue(verificationValue);
      if (!normalized) return { status: "InvalidCredential" };
      const cryptography = createTransferCryptography(await secretProvider());
      const digest = cryptography.codeDigest(normalized);
      const lookup = await store.read("t", digest);
      if (!lookup) return { status: "InvalidCredential" };
      let target = await store.read("account", lookup.value.accountId);
      const receipt = await store.read("receipt", lookup.value.transferId);
      if (receipt && target && (!target.value.transfer || target.value.transfer.transferId !== lookup.value.transferId)) {
        const saved = receipt.value;
        const binding = await store.read("player", context.playerId);
        if (!input || saved.accountId !== target.value.accountId || saved.transferId !== lookup.value.transferId ||
            saved.credentialRevision !== lookup.value.credentialRevision || saved.codeDigest !== digest ||
            saved.completedPlayerId !== context.playerId || target.value.currentPlayerId !== context.playerId ||
            target.value.connectionRevision !== saved.completedConnectionRevision ||
            !["Active", "TransferPending"].includes(target.value.status) || !binding || binding.value.transferOperation ||
            binding.value.status === "Inactive" || binding.value.accountId !== target.value.accountId ||
            binding.value.connectionRevision !== saved.completedConnectionRevision ||
            typeof saved.credentialHmac !== "string" || !/^[a-f0-9]{64}$/.test(saved.credentialHmac) ||
            !crypto.timingSafeEqual(Buffer.from(saved.credentialHmac, "hex"), Buffer.from(
              cryptography.credentialHmac(saved.transferId, saved.credentialRevision, normalized, input), "hex")))
          return { status: "InvalidCredential" };
        if (!numbers.tryNormalizePublicPlayerNumber(saved.publicPlayerNumber) ||
            saved.publicPlayerNumber !== target.value.publicPlayerNumber) throw fault("PublicNumberUnavailable");
        const confirmed = await store.read("account", target.value.accountId);
        const confirmedBinding = await store.read("player", context.playerId);
        if (!confirmed || confirmed.value.currentPlayerId !== context.playerId ||
            confirmed.value.connectionRevision !== saved.completedConnectionRevision ||
            !["Active", "TransferPending"].includes(confirmed.value.status) ||
            confirmed.value.publicPlayerNumber !== saved.publicPlayerNumber || !confirmedBinding ||
            confirmedBinding.value.accountId !== confirmed.value.accountId || confirmedBinding.value.transferOperation ||
            confirmedBinding.value.status === "Inactive" ||
            confirmedBinding.value.connectionRevision !== saved.completedConnectionRevision)
          return { status: "InvalidCredential" };
        return { status: "AlreadyCompleted", publicPlayerNumber: saved.publicPlayerNumber };
      }
      const initialBinding = await store.read("player", context.playerId);
      const priorOp = initialBinding && initialBinding.value.transferOperation;
      if (priorOp && target && priorOp.targetAccountId === target.value.accountId &&
          (!target.value.transfer || target.value.transfer.transferId !== priorOp.transferId ||
            ["Cancelled", "Expired"].includes(target.value.transfer.status) ||
            target.value.transfer.status === "Completed" && target.value.transfer.completedPlayerId !== context.playerId))
        await rollback(context, priorOp);
      if (!target || !target.value.transfer || target.value.transfer.transferId !== lookup.value.transferId ||
          target.value.transfer.credentialRevision !== lookup.value.credentialRevision || target.value.transfer.codeDigest !== digest)
        return { status: "InvalidCredential" };
      const t = target.value.transfer;
      if (t.status === "Completed") {
        if (!input || t.completedPlayerId !== context.playerId || typeof t.credentialHmac !== "string" ||
            !/^[a-f0-9]{64}$/.test(t.credentialHmac) || !crypto.timingSafeEqual(Buffer.from(t.credentialHmac, "hex"),
              Buffer.from(cryptography.credentialHmac(t.transferId, t.credentialRevision, normalized, input), "hex")))
          return { status: "InvalidCredential" };
        return finish(context, target, true);
      }
      // If a previously reserved target lost the terminal race, repair only
      // its own reservation. Never roll back a committed C completion.
      const binding = await store.read("player", context.playerId);
      if (t.status !== "TransferPending") {
        if (binding && binding.value.transferOperation &&
            binding.value.transferOperation.targetAccountId === target.value.accountId &&
            binding.value.transferOperation.transferId === t.transferId)
          await rollback(context, binding.value.transferOperation);
        return { status: "InvalidCredential" };
      }
      if (t.sourcePlayerId === context.playerId) return { status: "InvalidCredential" };
      const checked = validatePendingTransfer(target, now(), true);
      if (!Number.isSafeInteger(target.value.connectionRevision + 1)) throw fault("TransferUnavailable");
      if (now() >= checked.expiresAtMilliseconds) {
        const terminal = { ...checked, status: "Expired", credentialActive: false, terminalAtMilliseconds: now() };
        delete terminal.codeDigest; delete terminal.credentialHmac;
        await store.compareExchange(target, { ...target.value, status: "Active", transfer: terminal });
        if (binding && binding.value.transferOperation) await rollback(context, binding.value.transferOperation);
        return { status: "InvalidCredential" };
      }
      const verified = await createTransferVerificationService(store, secretProvider, clock).verify(context, normalized, verificationValue);
      if (verified.status !== "Verified") return verified;
      target = verified.account;
      let op;
      try { op = await reserveTarget(context, target); }
      catch (error) {
        const current = await store.read("player", context.playerId);
        const source = await store.read("account", target.value.accountId);
        if (current && current.value.transferOperation &&
            current.value.transferOperation.targetAccountId === target.value.accountId &&
            current.value.transferOperation.transferId === t.transferId)
          await releaseOwnPendingFence(context, current.value.transferOperation);
        if (source && source.value.transfer && ["Cancelled", "Expired"].includes(source.value.transfer.status) &&
            current && current.value.transferOperation &&
            current.value.transferOperation.targetAccountId === target.value.accountId &&
            current.value.transferOperation.transferId === t.transferId)
          await rollback(context, current.value.transferOperation);
        throw error;
      }
      // Time and connection are revalidated at the terminal commit boundary;
      // the exact verified write lock fences cancel/reissue/other B attempts.
      try {
        const pending = validatePendingTransfer(target, now());
        if (target.value.onlineOperation) throw fault("AccountBusy");
        const terminalTime = now();
        const resolved = resolution.resolveTransfer(pending, { kind: "complete", playerId: context.playerId,
          nowMilliseconds: terminalTime, credentialsValid: true, retryAllowed: true,
          target: { playerId: context.playerId, hasLocalPending: false } });
        if (resolved.outcome !== "Completed") throw fault("TransferUnavailable");
        await store.compareExchange(target, { ...target.value, status: "TransferCompleting", currentPlayerId: context.playerId,
          connectionRevision: resolved.transfer.completedConnectionRevision,
          transfer: { ...resolved.transfer, terminalAtMilliseconds: terminalTime, targetOperation: op } });
      } catch (error) {
        const current = await store.read("account", target.value.accountId);
        if (current && current.value.transfer &&
            (["Cancelled", "Expired"].includes(current.value.transfer.status) ||
              current.value.transfer.status === "Completed" && current.value.transfer.completedPlayerId !== context.playerId))
          await rollback(context, op);
        throw error;
      }
      const committed = await store.read("account", target.value.accountId);
      return finish(context, committed, false);
    }
  };
}
module.exports = { createTransferCompletionService };
