// Embedded Module endpoint; dependencies are included in the same Module.
const { createAccountStore } = require("./account-store");
const { createLegacyAccountStore } = require("./legacy-account-store");
const { createAccountProvisioningService } = require("./account-provisioning-service");
module.exports = async ({ context, logger }) => {
  try {
    const store = createAccountStore(context);
    const legacy = createLegacyAccountStore(context, undefined, undefined, store.scope);
    return await createAccountProvisioningService(store, legacy).ensure(context);
  } catch (_) {
    if (logger) logger.warning("Account provisioning unavailable; request deferred.");
    return { status: "TransientFailure", reason: "AccountUnavailable" };
  }
};
module.exports.params = {};
module.exports.bundling = true;
