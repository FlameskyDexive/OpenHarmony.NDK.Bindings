[CmdletBinding()]
param(
    [string]$SdkRoot = "$env:LOCALAPPDATA\OpenHarmony\Sdk",
    [int[]]$Apis = @(15, 18, 20, 23, 26),
    [string[]]$Abis = @('arm64-v8a', 'x86_64'),
    [string]$OutputRoot = 'artifacts/abi-matrix'
)

$ErrorActionPreference = 'Stop'
$probe = Join-Path $PSScriptRoot '..\tests\AbiProbe.Native\abi_probe.c'
$fullOutputRoot = [IO.Path]::GetFullPath($OutputRoot)
[IO.Directory]::CreateDirectory($fullOutputRoot) | Out-Null
$apiFolders = @{ 15='15'; 18='18'; 20='20'; 23='23'; 26='26.0.0' }
$triples = @{ 'arm64-v8a'='aarch64-linux-ohos'; 'x86_64'='x86_64-linux-ohos' }

foreach ($api in $Apis) {
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

Write-Output "Compiled ABI probe for $($Apis.Count) API level(s) and $($Abis.Count) ABI(s)."
