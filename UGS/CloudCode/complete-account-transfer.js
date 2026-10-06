const { createAccountStore } = require("./account-store");
const { createTransferSecretProvider } = require("./transfer-cryptography");
const { createTransferCompletionService } = require("./transfer-completion-service");
module.exports = async ({ context, params, secretManager, logger }) => {
  try {
    return await createTransferCompletionService(createAccountStore(context),
      createTransferSecretProvider(secretManager)).complete(context, params && params.code, params && params.verificationValue);
  } catch (_) {
    if (logger) logger.warning("Account transfer completion unavailable; request deferred.");
    return { status: "TransientFailure", reason: "TransferUnavailable" };
  }
};
module.exports.params = { code: { type: "String", required: true }, verificationValue: { type: "String", required: true } };
module.exports.bundling = true;
