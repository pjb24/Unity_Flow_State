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
    let shouldProvision = !binding;
    // A first attempt can commit its binding before the Account is created or
    // activated. Resume only that initial identity, never a transferred binding.
    if (binding && binding.value.playerId === context.playerId &&
        binding.value.status === undefined && !binding.value.transferOperation &&
        binding.value.connectionRevision === 1) {
      const normalize = require("./leaderboard-owner-policy").tryNormalizeGeneratedLeaderboardOwnerId;
      const b = binding.value;
      const accountId = normalize(b.accountId), generatedOwner = normalize(b.generatedOwner);
      if (accountId && generatedOwner && accountId === b.accountId && generatedOwner === b.generatedOwner &&
          b.accountId !== b.generatedOwner && b.accountId !== context.playerId && b.generatedOwner !== context.playerId) {
        const account = await store.read("account", b.accountId);
        shouldProvision = !account || account.value.status === "Preparing" &&
          account.value.accountId === b.accountId && account.value.initialPlayerId === context.playerId &&
          account.value.currentPlayerId === context.playerId && account.value.connectionRevision === 1 &&
          !account.value.transfer && !account.value.onlineOperation &&
          (account.value.leaderboardOwnerId === b.generatedOwner || account.value.leaderboardOwnerId === context.playerId);
      }
    }
    if (shouldProvision) {
      const { createLegacyAccountStore } = require("./legacy-account-store");
      const { createAccountProvisioningService } = require("./account-provisioning-service");
      await createAccountProvisioningService(store, createLegacyAccountStore(context, undefined, undefined, store.scope)).ensure(context);
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
