[CmdletBinding()]
param([switch]$Run, [switch]$Remaining)
$ErrorActionPreference = 'Stop'
$probeScript = Join-Path $PSScriptRoot 'verify-live-transfer-storage.cjs'
if (-not $Run) {
    & node $probeScript --plan
    if ($LASTEXITCODE -ne 0) { throw 'Probe plan failed.' }
    return
}
Write-Host 'verification only: creates isolated p2v-* Private Game Data. Existing accounts and Leaderboards are unchanged.'
$probeApproval = Read-Host 'Type VERIFY to approve these test writes'
if ($probeApproval -cne 'VERIFY') { throw 'No remote request was made.' }
function Convert-ProbeInput([Security.SecureString]$Value) {
    $probePointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($Value)
    try { return [Runtime.InteropServices.Marshal]::PtrToStringBSTR($probePointer) }
    finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($probePointer) }
}
# Inputs are masked and never placed in command history, arguments, or files.
# A child Node process receives the credentials briefly through its environment.
$probePreviousId = [Environment]::GetEnvironmentVariable('FS_PHASE2_PROBE_KEY_ID', 'Process')
$probePreviousSecret = [Environment]::GetEnvironmentVariable('FS_PHASE2_PROBE_KEY_SECRET', 'Process')
$probeIdInput = $null
$probeSecretInput = $null
try {
    $probeIdInput = Read-Host 'Temporary service account Key ID' -AsSecureString
    $probeSecretInput = Read-Host 'Temporary service account Secret Key' -AsSecureString
    [Environment]::SetEnvironmentVariable('FS_PHASE2_PROBE_KEY_ID', (Convert-ProbeInput $probeIdInput), 'Process')
    [Environment]::SetEnvironmentVariable('FS_PHASE2_PROBE_KEY_SECRET', (Convert-ProbeInput $probeSecretInput), 'Process')
    if ($Remaining) { & node $probeScript --run --confirm-verification-writes --remaining }
    else { & node $probeScript --run --confirm-verification-writes }
    if ($LASTEXITCODE -ne 0) { throw 'Probe unconfirmed. Keep its data and share only the safe FAIL line.' }
}
finally {
    [Environment]::SetEnvironmentVariable('FS_PHASE2_PROBE_KEY_ID', $probePreviousId, 'Process')
    [Environment]::SetEnvironmentVariable('FS_PHASE2_PROBE_KEY_SECRET', $probePreviousSecret, 'Process')
    if ($null -ne $probeIdInput) { $probeIdInput.Dispose() }
    if ($null -ne $probeSecretInput) { $probeSecretInput.Dispose() }
}
