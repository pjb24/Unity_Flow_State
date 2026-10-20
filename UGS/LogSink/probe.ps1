param(
    [string]$BaseUrl = "http://127.0.0.1:8080"
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Net.Http
$envFile = Join-Path $PSScriptRoot ".env"
if (-not (Test-Path -LiteralPath $envFile)) { throw "Missing local .env file." }
$secretLine = Get-Content -LiteralPath $envFile | Where-Object { $_ -match '^FLOW_STATE_LOG_SINK_HMAC=' } | Select-Object -First 1
if ($null -eq $secretLine) { throw "Missing FLOW_STATE_LOG_SINK_HMAC in local .env." }
$secret = $secretLine.Substring("FLOW_STATE_LOG_SINK_HMAC=".Length)
if ($secret.Length -lt 32) { throw "FLOW_STATE_LOG_SINK_HMAC must be at least 32 characters." }

function New-Signature([string]$Timestamp, [string]$Body) {
    $hmac = [System.Security.Cryptography.HMACSHA256]::new([Text.Encoding]::UTF8.GetBytes($secret))
    try { return [Convert]::ToBase64String($hmac.ComputeHash([Text.Encoding]::UTF8.GetBytes("$Timestamp.$Body"))) }
    finally { $hmac.Dispose() }
}

function Send-Request([string]$Path, [string]$Body, [string]$Timestamp, [string]$Signature, [string]$ContentType = "application/json") {
    $client = [System.Net.Http.HttpClient]::new()
    try {
        $request = [System.Net.Http.HttpRequestMessage]::new([System.Net.Http.HttpMethod]::Post, "$BaseUrl$Path")
        $request.Headers.Add("X-Flow-State-Timestamp", $Timestamp)
        $request.Headers.Add("X-Flow-State-Signature", $Signature)
        $request.Content = [System.Net.Http.StringContent]::new($Body, [Text.Encoding]::UTF8, $ContentType)
        return [int]$client.SendAsync($request).GetAwaiter().GetResult().StatusCode
    } finally { $client.Dispose() }
}

function Assert-Status([string]$Name, [int]$Actual, [int]$Expected) {
    if ($Actual -ne $Expected) { throw "$Name expected HTTP $Expected but received HTTP $Actual." }
    Write-Output "PASS / $Name / HTTP $Actual"
}

$client = [System.Net.Http.HttpClient]::new()
try { Assert-Status "health" ([int]$client.GetAsync("$BaseUrl/health").GetAwaiter().GetResult().StatusCode) 204 }
finally { $client.Dispose() }

$timestamp = [DateTimeOffset]::UtcNow.ToUnixTimeMilliseconds().ToString()
$safe = '{"environment":"verification","moduleReference":"synthetic-reference","outcome":"Accepted","reason":"SyntheticProbe","durationBucket":0,"correlationId":"opaque-synthetic-id","rateLimitResult":"Allowed"}'
Assert-Status "safe-event" (Send-Request "/v1/events" $safe $timestamp (New-Signature $timestamp $safe)) 202

$extra = '{"environment":"verification","moduleReference":"synthetic-reference","outcome":"Accepted","reason":"SyntheticProbe","durationBucket":0,"correlationId":"opaque-synthetic-id","rateLimitResult":"Allowed","playerId":"forbidden"}'
Assert-Status "extra-field-rejected" (Send-Request "/v1/events" $extra $timestamp (New-Signature $timestamp $extra)) 400
Assert-Status "invalid-hmac-rejected" (Send-Request "/v1/events" $safe $timestamp "invalid") 401

$stale = ([DateTimeOffset]::UtcNow.ToUnixTimeMilliseconds() - 300001).ToString()
Assert-Status "stale-timestamp-rejected" (Send-Request "/v1/events" $safe $stale (New-Signature $stale $safe)) 401

$invalidJson = '{'
Assert-Status "invalid-json-rejected" (Send-Request "/v1/events" $invalidJson $timestamp (New-Signature $timestamp $invalidJson)) 400

$oversized = " " * 2049
Assert-Status "oversized-body-rejected" (Send-Request "/v1/events" $oversized $timestamp (New-Signature $timestamp $oversized)) 413
Assert-Status "content-type-rejected" (Send-Request "/v1/events" $safe $timestamp (New-Signature $timestamp $safe) "text/plain") 415
Assert-Status "unknown-route-rejected" (Send-Request "/unknown" $safe $timestamp (New-Signature $timestamp $safe)) 404

Write-Output "PASS / local Docker safe-log-sink contract completed. No secret, signature, or request body was printed."
