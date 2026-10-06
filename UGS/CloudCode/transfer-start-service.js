const { fault, projectId, environmentId } = require("./account-store");
const { readSourceAccount, validatePendingTransfer } = require("./transfer-state");
const { createAccountService } = require("./account-service");
const { generateId } = require("./account-provisioning-service");
const { createTransferCryptography } = require("./transfer-cryptography");
const policy = require("./transfer-credential-policy");
const ownerPolicy = require("./leaderboard-owner-policy");
const maximumCodeAttempts = 8;
const scope = { schemaVersion: 1, projectId, environmentId };

function createTransferStartService(store, secretProvider, options = {}) {
  const clock = options.clock || Date.now;
  const idGenerator = options.idGenerator || generateId;
  function time() {
    const now = clock();
    if (!Number.isSafeInteger(now) || now < 0) throw fault("InvalidServerTime");
    return now;
  }
  async function authority(context) {
    return readSourceAccount(store, context);
  }
  function validateTransfer(account) {
    return validatePendingTransfer(account, time());
  }
  async function reserveCode(account) {
    const transfer = validateTransfer(account);
    for (let attempt = 0; attempt < 3; attempt++) {
      let lookup;
      try {
        lookup = await store.create("t", transfer.codeDigest, { ...scope,
          accountId: account.value.accountId, transferId: transfer.transferId, credentialRevision: transfer.credentialRevision });
      } catch (error) {
        if (!error.response || error.response.status !== 409) throw error;
        lookup = await store.read("t", transfer.codeDigest);
        // A competing immutable-guard initializer can rotate a token without
        // inserting a lookup. Re-read/retry; never bypass a write lock.
        if (!lookup) continue;
      }
      return lookup.value.accountId === account.value.accountId && lookup.value.transferId === transfer.transferId &&
        lookup.value.credentialRevision === transfer.credentialRevision;
    }
    throw fault("TransferCodeUnavailable");
  }
  async function activate(context, expected) {
    const current = await authority(context);
    if (current.value.status !== "TransferPending") throw fault("TransferUnavailable");
    const transfer = validateTransfer(current);
    if (transfer.transferId !== expected.transferId || transfer.codeDigest !== expected.codeDigest ||
        transfer.credentialHmac !== expected.credentialHmac) throw fault("TransferUnavailable");
    if (!transfer.credentialActive)
      await store.compareExchange(current, { ...current.value, transfer: { ...transfer, credentialActive: true } });
    const confirmed = await authority(context);
    const confirmedTransfer = validateTransfer(confirmed);
    if (confirmed.value.status !== "TransferPending" || !confirmedTransfer.credentialActive ||
        confirmedTransfer.transferId !== expected.transferId || confirmedTransfer.codeDigest !== expected.codeDigest ||
        confirmedTransfer.credentialHmac !== expected.credentialHmac) throw fault("TransferUnavailable");
    return confirmed;
  }
  function pendingReply() {
    // Raw credentials cannot be recovered from HMACs after response loss.
    return { status: "TransferPending", reason: "CredentialReissueRequired" };
  }
  async function issue(context, cryptography, credential, transfer) {
    const transferId = transfer.transferId;
    for (let attempt = 0; attempt < maximumCodeAttempts; attempt++) {
      const account = await authority(context);
      const current = validateTransfer(account);
      if (current.transferId !== transferId || current.credentialRevision !== transfer.credentialRevision ||
          current.codeDigest !== transfer.codeDigest || current.credentialHmac !== transfer.credentialHmac ||
          current.credentialActive) throw fault("TransferUnavailable");
      if (await reserveCode(account)) {
        await activate(context, transfer);
        return { status: "Success", code: policy.formatTransferCode(credential.code),
          verificationValue: credential.verificationValue, expiresAtMilliseconds: transfer.expiresAtMilliseconds };
      }
      if (attempt + 1 === maximumCodeAttempts) throw fault("TransferCodeUnavailable");
      credential = cryptography.generateCredential();
      transfer = { ...current, codeDigest: cryptography.codeDigest(credential.code),
        credentialHmac: cryptography.credentialHmac(transferId, current.credentialRevision,
          credential.code, credential.verificationValue) };
      await store.compareExchange(account, { ...account.value, transfer });
    }
    throw fault("TransferCodeUnavailable");
  }
  return {
    async reissue(context) {
      const account = await authority(context);
      if (account.value.status !== "TransferPending") throw fault("TransferUnavailable");
      const current = validateTransfer(account);
      if (typeof secretProvider !== "function") throw fault("TransferSecretUnavailable");
      const cryptography = createTransferCryptography(await secretProvider(), options.randomBytes);
      const credentialRevision = current.credentialRevision + 1;
      if (!Number.isSafeInteger(credentialRevision)) throw fault("TransferUnavailable");
      const credential = cryptography.generateCredential();
      const transfer = { ...current, credentialRevision, credentialActive: false,
        codeDigest: cryptography.codeDigest(credential.code),
        credentialHmac: cryptography.credentialHmac(current.transferId, credentialRevision,
          credential.code, credential.verificationValue) };
      // One Account CAS invalidates the old credential before any new lookup
      // can become active. Original expiry and request-wide throttle survive.
      validateTransfer({ value: { ...account.value, transfer } });
      await store.compareExchange(account, { ...account.value, transfer });
      return issue(context, cryptography, credential, transfer);
    },
    async start(context) {
      let account = await authority(context);
      if (account.value.status === "TransferPending") {
        const transfer = validateTransfer(account);
        if (await reserveCode(account)) await activate(context, transfer);
        return pendingReply();
      }
      account = await createAccountService(store, clock).assertTransferCanStart(context);
      if (typeof secretProvider !== "function") throw fault("TransferSecretUnavailable");
      const secret = await secretProvider();
      const cryptography = createTransferCryptography(secret, options.randomBytes);
      const transferId = ownerPolicy.tryNormalizeGeneratedLeaderboardOwnerId(idGenerator());
      const issuedAtMilliseconds = time();
      const expiresAtMilliseconds = issuedAtMilliseconds + policy.transferLifetimeMilliseconds;
      if (!transferId || !Number.isSafeInteger(expiresAtMilliseconds)) throw fault("TransferUnavailable");
      let credential = cryptography.generateCredential();
      let transfer = { transferId, status: "TransferPending", sourcePlayerId: context.playerId,
        connectionRevision: account.value.connectionRevision, credentialRevision: 1,
        issuedAtMilliseconds, expiresAtMilliseconds, credentialActive: false,
        codeDigest: cryptography.codeDigest(credential.code),
        credentialHmac: cryptography.credentialHmac(transferId, 1, credential.code, credential.verificationValue),
        lastAttemptAtMilliseconds: null, failureCount: 0 };
      // This CAS both locks online work and records hashes. No raw credential is
      // sent to any storage API, even when a response is lost after commit.
      await store.compareExchange(account, { ...account.value, status: "TransferPending", transfer });
      return issue(context, cryptography, credential, transfer);
    }
  };
}
module.exports = { createTransferStartService, maximumCodeAttempts };
