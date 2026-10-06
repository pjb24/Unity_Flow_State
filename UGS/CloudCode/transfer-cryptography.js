const crypto = require("crypto");
const { fault, projectId, environmentId } = require("./account-store");
const policy = require("./transfer-credential-policy");
const secretName = "FS_TRANSFER_HMAC_VERIFICATION_V1";
const maximumRandomAttempts = 32;
function createTransferCryptography(secretValue, randomBytes = crypto.randomBytes) {
  // Require a canonical base64-encoded 256-bit server key. No default secret.
  if (typeof secretValue !== "string" || !/^[A-Za-z0-9+/]{43}=$/.test(secretValue))
    throw fault("TransferSecretUnavailable");
  const secret = Buffer.from(secretValue, "base64");
  if (secret.length !== 32 || secret.toString("base64") !== secretValue)
    throw fault("TransferSecretUnavailable");
  function random(count) {
    const bytes = randomBytes(count);
    if (!Buffer.isBuffer(bytes) || bytes.length !== count) throw fault("RandomSourceUnavailable");
    return bytes;
  }
  function mac(value) {
    return crypto.createHmac("sha256", secret).update(JSON.stringify(value)).digest();
  }
  return {
    generateCredential() {
      const bytes = random(policy.transferCodeLength);
      let code = "";
      for (const byte of bytes) code += policy.transferCodeSymbols[byte & 31];
      // Rejection sampling avoids modulo bias over the one-billion-value space.
      for (let i = 0; i < maximumRandomAttempts; i++) {
        const candidate = random(4).readUInt32BE(0);
        if (candidate < 4000000000)
          return { code, verificationValue: String(candidate % 1000000000).padStart(9, "0") };
      }
      throw fault("RandomSourceUnavailable");
    },
    codeDigest(code) {
      const normalized = policy.tryNormalizeTransferCode(code);
      if (!normalized) throw fault("InvalidTransferCredential");
      // 43 URL-safe characters + "fs8-t-" fits Cloud Save's 50-character ID limit.
      return mac(["fs-transfer-code-v1", projectId, environmentId, normalized]).toString("base64")
        .replace(/\+/g, "-").replace(/\//g, "_").replace(/=+$/, "");
    },
    credentialHmac(transferId, revision, code, verificationValue) {
      const normalized = policy.tryNormalizeTransferCode(code);
      if (!normalized || !policy.tryNormalizeVerificationValue(verificationValue) ||
          typeof transferId !== "string" || !transferId || !Number.isSafeInteger(revision) || revision < 1)
        throw fault("InvalidTransferCredential");
      return mac(["fs-transfer-credential-v1", projectId, environmentId,
        transferId, revision, normalized, verificationValue]).toString("hex");
    }
  };
}
function createTransferSecretProvider(secretManager) {
  return async () => {
    if (!secretManager || typeof secretManager.getSecret !== "function") throw fault("TransferSecretUnavailable");
    try {
      const secret = await secretManager.getSecret(secretName);
      if (!secret || typeof secret.value !== "string") throw fault("TransferSecretUnavailable");
      return secret.value;
    } catch (_) { throw fault("TransferSecretUnavailable"); }
  };
}
module.exports = { createTransferCryptography, createTransferSecretProvider, secretName };
