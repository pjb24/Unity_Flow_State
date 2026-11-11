[CmdletBinding()]
param([switch]$Run, [switch]$ConfirmProduction)
$ErrorActionPreference = 'Stop'
$probeScript = Join-Path $PSScriptRoot 'production-safe-log-probe.cjs'
& node $probeScript --plan
if ($LASTEXITCODE -ne 0) { throw 'Production target check failed. No request sent.' }
if (-not $Run) { return }
if (-not $ConfirmProduction) { throw 'Use -Run -ConfirmProduction after Step 5 prerequisites.' }
function Convert-ProductionProbeCredential([Security.SecureString]$Value) {
    $probePointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($Value)
    try { return [Runtime.InteropServices.Marshal]::PtrToStringBSTR($probePointer) }
    finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($probePointer) }
}
$probePreviousId = [Environment]::GetEnvironmentVariable('FS_PRODUCTION_PROBE_KEY_ID', 'Process')
$probePreviousSecret = [Environment]::GetEnvironmentVariable('FS_PRODUCTION_PROBE_KEY_SECRET', 'Process')
$probeIdInput = $null
$probeSecretInput = $null
$probeSucceeded = $false
try {
    Write-Host 'Production synthetic log only. Enter UGS Service Account credentials, NOT game HMAC secrets.'
    $probeIdInput = Read-Host 'UGS Service Account Key ID' -AsSecureString
    $probeSecretInput = Read-Host 'UGS Service Account Secret Key' -AsSecureString
    [Environment]::SetEnvironmentVariable('FS_PRODUCTION_PROBE_KEY_ID', (Convert-ProductionProbeCredential $probeIdInput), 'Process')
    [Environment]::SetEnvironmentVariable('FS_PRODUCTION_PROBE_KEY_SECRET', (Convert-ProductionProbeCredential $probeSecretInput), 'Process')
    & node $probeScript --run --confirm-production
    $probeSucceeded = $LASTEXITCODE -eq 0
    if ($LASTEXITCODE -ne 0) { Write-Host 'Probe not confirmed. Stop and share only the safe FAIL line.' }
}
finally {
    [Environment]::SetEnvironmentVariable('FS_PRODUCTION_PROBE_KEY_ID', $probePreviousId, 'Process')
    [Environment]::SetEnvironmentVariable('FS_PRODUCTION_PROBE_KEY_SECRET', $probePreviousSecret, 'Process')
    if ($null -ne $probeIdInput) { $probeIdInput.Dispose() }
    if ($null -ne $probeSecretInput) { $probeSecretInput.Dispose() }
}
if (-not $probeSucceeded) { throw 'Production probe not confirmed. No automatic retry.' }
