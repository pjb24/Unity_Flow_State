const { createAccountStore } = require("./account-store");
const { createTransferLifecycleService } = require("./transfer-lifecycle-service");
module.exports = async ({ context, params, logger }) => {
  try {
    return await createTransferLifecycleService(createAccountStore(context)).cancel(context, params && params.transferId);
  } catch (_) {
    if (logger) logger.warning("Account transfer cancellation unavailable; request deferred.");
    return { status: "TransientFailure", reason: "TransferUnavailable" };
  }
};
module.exports.params = { transferId: { type: "String", required: true } };
module.exports.bundling = true;
