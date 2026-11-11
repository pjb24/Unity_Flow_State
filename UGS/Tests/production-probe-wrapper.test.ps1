$ErrorActionPreference = 'Stop'
# Local command doubles: no Node process, network, credential prompt, or real credential access.
$probeWrapper = Join-Path $PSScriptRoot '..\Verification\Invoke-ProductionSafeLogProbe.ps1'
$global:ProductionProbeTestCalls = 0
$global:ProductionProbeTestThrow = $false
function node {
    $global:ProductionProbeTestCalls++
    if ($global:ProductionProbeTestCalls -eq 2 -and $global:ProductionProbeTestThrow) { throw 'Synthetic failure' }
    $global:LASTEXITCODE = 0
}
function Read-Host {
    param($Prompt, [switch]$AsSecureString)
    if (-not $AsSecureString) { throw 'Credential input must be hidden.' }
    ConvertTo-SecureString 'synthetic-only' -AsPlainText -Force
}
$savedId = $env:FS_PRODUCTION_PROBE_KEY_ID
$savedSecret = $env:FS_PRODUCTION_PROBE_KEY_SECRET
try {
    foreach ($case in @($false, $true)) {
        $env:FS_PRODUCTION_PROBE_KEY_ID = 'synthetic-previous-id'
        $env:FS_PRODUCTION_PROBE_KEY_SECRET = 'synthetic-previous-secret'
        $global:ProductionProbeTestCalls = 0
        $global:ProductionProbeTestThrow = $case
        try { & $probeWrapper -Run -ConfirmProduction }
        catch { if (-not $case) { throw } }
        if ($global:ProductionProbeTestCalls -ne 2 -or $env:FS_PRODUCTION_PROBE_KEY_ID -ne 'synthetic-previous-id' -or
            $env:FS_PRODUCTION_PROBE_KEY_SECRET -ne 'synthetic-previous-secret') { throw 'Credential cleanup failed.' }
    }
    Write-Output 'PASS Production PowerShell wrapper: hidden input doubles, prior environment restored on success and failure; no remote requests.'
}
finally {
    $env:FS_PRODUCTION_PROBE_KEY_ID = $savedId
    $env:FS_PRODUCTION_PROBE_KEY_SECRET = $savedSecret
    Remove-Variable ProductionProbeTestCalls, ProductionProbeTestThrow -Scope Global
}
