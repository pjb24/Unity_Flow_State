// Ensures the checked-in inventory remains sanitized and points at current local artifacts.
const assert = require("assert").strict;
const fs = require("fs");
const path = require("path");
const root = path.resolve(__dirname, "../..");
const inventory = fs.readFileSync(path.join(root, "UGS/PRODUCTION_ENVIRONMENT_INVENTORY.md"), "utf8");
for (const marker of ["PRODUCTION_DEPLOY_BLOCKED", "Production 환경 식별자", "FS_TRANSFER_HMAC_VERIFICATION_V1",
  "FlowStateVerification", "a6258ef", "CLOUD_SAVE_DATA_REFERENCE.md", "getProductionDeployGate"])
  assert.ok(inventory.includes(marker), marker);
assert.ok(!inventory.includes("48fb7ca9-891b-4477-bcc1-705c38cbd9dc"));
assert.ok(!/Secret\s*값\s*[:=]\s*\S+/i.test(inventory));
console.log("PASS Production inventory: sanitized target record, explicit blockers and local artifact links.");
