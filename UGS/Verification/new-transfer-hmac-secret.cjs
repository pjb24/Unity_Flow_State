"use strict";
// User-only helper. The AI runs --self-test only, with synthetic values.
const assert = require("node:assert/strict");
const { randomBytes } = require("node:crypto");
const { spawnSync } = require("node:child_process");
const { createTransferCryptography } = require("../CloudCode/transfer-cryptography");
if (process.argv.includes("--self-test")) {
  const synthetic = Buffer.alloc(32).toString("base64");
  assert.doesNotThrow(() => createTransferCryptography(synthetic));
  for (const invalid of ["", "not-a-key", "A".repeat(43), "A".repeat(42) + "B=", synthetic + "\n"])
    assert.throws(() => createTransferCryptography(invalid), error => error.reason === "TransferSecretUnavailable");
  console.log("PASS: production format validation with synthetic values. No deployment secret generated; clipboard untouched.");
} else if (process.argv.length !== 2 || process.platform !== "win32") {
  console.error("Use on Windows without arguments, or use --self-test for synthetic format checks.");
  process.exitCode = 1;
} else {
  let bytes;
  try {
    bytes = randomBytes(32);
    const value = bytes.toString("base64");
    createTransferCryptography(value);
    // Value is stdin only, never command-line arguments, terminal output or a file.
    const result = spawnSync("powershell.exe", ["-NoProfile", "-NonInteractive", "-Command",
      "$clipboardSecretInput = [Console]::In.ReadToEnd(); Set-Clipboard -Value $clipboardSecretInput -ErrorAction Stop; $clipboardSecretInput = $null"],
      { input: value, encoding: "utf8", windowsHide: true, timeout: 10000 });
    if (result.error || result.status !== 0) throw new Error("ClipboardUnavailable");
    console.log("Ready: 32 random bytes encoded as 44-character canonical Base64 copied to clipboard. Paste into verification Secret Value, save, then clear the clipboard. Secret NOT printed or saved to a file.");
  } catch (_) {
    console.error("Secret preparation failed. No value will be printed. Check local PowerShell/clipboard availability.");
    process.exitCode = 1;
  } finally {
    if (bytes) bytes.fill(0);
  }
}
