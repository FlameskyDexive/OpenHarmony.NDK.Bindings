# OpenHarmony NDK Bindings (.NET 10)

本仓库面向 HarmonyOS 5.0/5.1/6.0/6.1/7 的 API15、18、20、23、26，绑定库目标框架为 .NET 10。SDK 基线清单位于 `sdk-manifests/`，实际 SDK 根目录可通过 `--sdk-root` 指定。

## 头文件清单与差分

`BindingGenerator` 会按规范化相对路径排序，并记录文件大小和 SHA-256；架构专属 include 根目录会被排除，避免 arm64/x86_64 重复声明：

```powershell
dotnet run --project tools/BindingGenerator/BindingGenerator.csproj -- `
  inventory --sdk-root C:\Users\Inxep\AppData\Local\OpenHarmony\Sdk `
  --api 26 --output artifacts/header-inventory/api26.json

dotnet run --project tools/BindingGenerator/BindingGenerator.csproj -- `
  diff --before artifacts/header-inventory/api23.json `
  --after artifacts/header-inventory/api26.json `
  --output artifacts/header-inventory/diff-23-26.json
```

API15/18/20/23/26 的 SDK manifest 已在 `sdk-manifests/` 固化，并由 `verify-sdk` 校验包版本、发布通道、manifest SHA-256、Clang、sysroot 和 CMake toolchain。ClangSharp 工具版本固定在 `.config/dotnet-tools.json`。

API26 公共 kit 覆盖清单位于 `generator-configs/public-kits.json`。coverage 命令会比较 SDK 顶层头文件根目录；C++ 专属、架构专属和内核/libc 目录必须在 `excludedRoots` 中写明原因：

```powershell
dotnet run --project tools/BindingGenerator/BindingGenerator.csproj -- `
  coverage --sysroot C:\Users\Inxep\AppData\Local\OpenHarmony\Sdk\26.0.0\native\sysroot `
  --config generator-configs/public-kits.json `
  --output artifacts/public-kit-coverage-api26.json
```

## 构建和测试

```powershell
dotnet test OpenHarmony.NDK.Bindings.sln
dotnet run --project tools/BindingGenerator/BindingGenerator.csproj -- `
  verify-sdk --sdk-root C:\Users\Inxep\AppData\Local\OpenHarmony\Sdk `
  --apis 15,18,20,23,26
```
