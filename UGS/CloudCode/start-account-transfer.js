const { createAccountStore } = require("./account-store");
const { createTransferSecretProvider } = require("./transfer-cryptography");
const { createTransferStartService } = require("./transfer-start-service");
module.exports = async ({ context, secretManager, logger }) => {
  try {
    const store = createAccountStore(context);
    return await createTransferStartService(store, createTransferSecretProvider(secretManager)).start(context);
  } catch (_) {
    // SDK/Secret errors may embed credentials, tokens or identifiers.
    if (logger) logger.warning("Account transfer start unavailable; request deferred.");
    return { status: "TransientFailure", reason: "TransferUnavailable" };
  }
};
module.exports.params = {};
module.exports.bundling = true;
