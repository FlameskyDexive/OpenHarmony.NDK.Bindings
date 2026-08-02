# OpenHarmony NDK Bindings (.NET 10)

本仓库面向 HarmonyOS 5.0/5.1/6.0/6.1/7 的 API13–24、26，绑定库目标框架为 .NET 10。SDK 基线清单位于 `sdk-manifests/`，实际 SDK 根目录可通过 `--sdk-root` 指定。API16、17、19、21、22、24 没有可安装的独立 Native SDK，manifest 会明确记录为不可编译但保留为 emulator 目标。

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

API13/14/15/18/20/23/26 的 SDK manifest 已在 `sdk-manifests/` 固化，并由 `verify-sdk` 校验包版本、发布通道、manifest SHA-256、sysroot tree SHA-256、toolchain SHA-256、header inventory SHA-256、Clang、sysroot 和 CMake toolchain。API25 明确拒绝：`API 25 is intentionally unsupported because no compatible SDK/image is available.` ClangSharp 工具版本固定在 `.config/dotnet-tools.json`。

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
  --apis 13,14,15,16,17,18,19,20,21,22,23,24,26
```
