# Compatibility wrapper; use the Node command without .ps1 execution policy changes.
[CmdletBinding()]
param([switch]$SelfTest)
$helperPath = Join-Path $PSScriptRoot 'new-transfer-hmac-secret.cjs'
if ($SelfTest) { & node $helperPath --self-test } else { & node $helperPath }
if ($LASTEXITCODE -ne 0) { throw 'Secret helper failed. Do not print or share a secret value.' }
