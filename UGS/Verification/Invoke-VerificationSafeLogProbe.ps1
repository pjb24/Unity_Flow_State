[CmdletBinding()]
param([switch]$Run)
$ErrorActionPreference = 'Stop'
$probeScript = Join-Path $PSScriptRoot 'verification-safe-log-probe.cjs'
if (-not $Run) {
    & node $probeScript --plan
    if ($LASTEXITCODE -ne 0) { throw 'Probe plan failed.' }
    return
}
function Convert-ProbeCredential([Security.SecureString]$Value) {
    $probePointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($Value)
    try { return [Runtime.InteropServices.Marshal]::PtrToStringBSTR($probePointer) }
    finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($probePointer) }
}
$probePreviousId = [Environment]::GetEnvironmentVariable('FS_SAFE_LOG_PROBE_KEY_ID', 'Process')
$probePreviousSecret = [Environment]::GetEnvironmentVariable('FS_SAFE_LOG_PROBE_KEY_SECRET', 'Process')
$probeIdInput = $null
$probeSecretInput = $null
try {
    Write-Host 'Use the UGS verification Service Account key. These are NOT LogSink or transfer HMAC secrets.'
    $probeIdInput = Read-Host 'UGS Service Account Key ID' -AsSecureString
    $probeSecretInput = Read-Host 'UGS Service Account Secret Key' -AsSecureString
    [Environment]::SetEnvironmentVariable('FS_SAFE_LOG_PROBE_KEY_ID', (Convert-ProbeCredential $probeIdInput), 'Process')
    [Environment]::SetEnvironmentVariable('FS_SAFE_LOG_PROBE_KEY_SECRET', (Convert-ProbeCredential $probeSecretInput), 'Process')
    & node $probeScript --run
    if ($LASTEXITCODE -ne 0) {
        Write-Host 'Probe not confirmed. Share only the safe FAIL line above.'
        exit $LASTEXITCODE
    }
}
finally {
    [Environment]::SetEnvironmentVariable('FS_SAFE_LOG_PROBE_KEY_ID', $probePreviousId, 'Process')
    [Environment]::SetEnvironmentVariable('FS_SAFE_LOG_PROBE_KEY_SECRET', $probePreviousSecret, 'Process')
    if ($null -ne $probeIdInput) { $probeIdInput.Dispose() }
    if ($null -ne $probeSecretInput) { $probeSecretInput.Dispose() }
}
