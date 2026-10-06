const { createAccountStore } = require("./account-store");
const { createTransferSecretProvider } = require("./transfer-cryptography");
const { createTransferStartService } = require("./transfer-start-service");
module.exports = async ({ context, secretManager, logger }) => {
  try {
    return await createTransferStartService(createAccountStore(context),
      createTransferSecretProvider(secretManager)).reissue(context);
  } catch (_) {
    if (logger) logger.warning("Account transfer reissue unavailable; request deferred.");
    return { status: "TransientFailure", reason: "TransferUnavailable" };
  }
};
module.exports.params = {};
module.exports.bundling = true;
