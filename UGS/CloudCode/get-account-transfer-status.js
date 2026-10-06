const { createAccountStore } = require("./account-store");
const { createTransferLifecycleService } = require("./transfer-lifecycle-service");
const { createTransferCompletionService } = require("./transfer-completion-service");
module.exports = async ({ context, logger }) => {
  try {
    const store = createAccountStore(context);
    // First authentication (including a crash before provisioning) can have no
    // binding. Never provision over an existing inactive/recovery binding.
    const binding = await store.read("player", context.playerId);
    // A completed source needs neither target recovery nor provisioning. Reuse
    // only this invocation's scoped server read, never a client/cache binding.
    if (binding && binding.value.status === "Inactive" && !binding.value.transferOperation)
      return await createTransferLifecycleService(store).inactiveStatus(context, binding);
    if (!binding) {
      const { createLegacyAccountStore } = require("./legacy-account-store");
      const { createAccountProvisioningService } = require("./account-provisioning-service");
      await createAccountProvisioningService(store, createLegacyAccountStore(context)).ensure(context);
    }
    const recovered = await createTransferCompletionService(store, null).recover(context);
    if (recovered) return recovered;
    return await createTransferLifecycleService(store).status(context);
  } catch (_) {
    if (logger) logger.warning("Account transfer status unavailable; request deferred.");
    return { status: "TransientFailure", reason: "TransferUnavailable" };
  }
};
module.exports.params = {};
module.exports.bundling = true;
