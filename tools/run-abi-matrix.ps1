[CmdletBinding()]
param(
    [string]$SdkRoot = "$env:LOCALAPPDATA\OpenHarmony\Sdk",
    [int[]]$Apis = @(13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 26),
    [string[]]$Abis = @('arm64-v8a', 'x86_64'),
[string]$OutputRoot = 'artifacts/abi-matrix'
)

$ErrorActionPreference = 'Stop'
$probe = Join-Path $PSScriptRoot '..\tests\AbiProbe.Native\abi_probe.c'
$fullOutputRoot = [IO.Path]::GetFullPath($OutputRoot)
[IO.Directory]::CreateDirectory($fullOutputRoot) | Out-Null
$apiFolders = @{ 13='13'; 14='14'; 15='15'; 18='18'; 20='20'; 23='23'; 26='26.0.0' }
$supportedApis = @(13..24) + 26
$nativeUnavailableApis = @(16, 17, 19, 21, 22, 24)
$triples = @{ 'arm64-v8a'='aarch64-linux-ohos'; 'x86_64'='x86_64-linux-ohos' }
$skipped = 0

foreach ($api in $Apis) {
    if ($api -eq 25) {
        throw 'API 25 is intentionally unsupported because no compatible SDK/image is available.'
    }
    if ($supportedApis -notcontains $api) {
        throw "Unsupported API $api. Supported APIs are 13 through 24 and 26."
    }
    if ($nativeUnavailableApis -contains $api) {
        Write-Output "SKIP API ${api}: no installable Native SDK package is available; emulator target only."
        $skipped++
        continue
    }
    if (-not $apiFolders.ContainsKey($api)) { throw "Unsupported API $api" }
    $sysroot = Join-Path (Join-Path $SdkRoot $apiFolders[$api]) 'native/sysroot'
    $clang = Join-Path (Join-Path $SdkRoot $apiFolders[$api]) 'native/llvm/bin/clang.exe'
    if (-not (Test-Path $clang)) { $clang = Join-Path (Join-Path $SdkRoot $apiFolders[$api]) 'native/llvm/bin/clang' }
    if (-not (Test-Path $clang)) { throw "Clang not found for API $api" }
    foreach ($abi in $Abis) {
        if (-not $triples.ContainsKey($abi)) { throw "Unsupported ABI $abi" }
        $output = Join-Path $fullOutputRoot "api$api-$abi.o"
        & $clang --target=$($triples[$abi]) --sysroot=$sysroot -x c++ -std=c++14 -c $probe -o $output
        if ($LASTEXITCODE -ne 0) { throw "ABI probe compile failed for API $api / $abi" }
    }
}

Write-Output "Compiled ABI probe for $($Apis.Count - $skipped) API level(s) and $($Abis.Count) ABI(s); skipped $skipped API level(s)."
