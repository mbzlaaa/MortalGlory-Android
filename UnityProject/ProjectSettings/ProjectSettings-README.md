# ProjectSettings 目录说明

Unity 的 ProjectSettings 目录包含工程全局配置。其中：

- **可文本编辑**（本阶段已提供或可手写）：
  - `ProjectVersion.txt`（已创建 —— 锁定 Unity 2019.1.8f1）
  - `EditorBuildSettings.asset`（场景列表，YAML）
  - `TagManager.asset`、`InputManager.asset`（YAML，可手写）
  - `GraphicsSettings.asset`、`QualitySettings.asset`（YAML）

- **Unity 自动生成 / 需谨慎**（建议转方案 X 时由 Unity 生成后提交）：
  - `ProjectSettings.asset`（含包名、版本号等，YAML 但字段多）
  - 各类 `.asset` 的二进制部分

> 说明：方案 Y（当前阶段）先建立「人类可读」的文本配置骨架；
> 等转入方案 X（真实 Unity 环境）时，用 Unity 编辑器打开工程，
> 它会自动补全所有缺失的默认配置后再提交，避免手工写错。

关键参数（待 ProjectSettings.asset 写入）：
- bundleIdentifier: `com.redbeak.mortalglory`
- minSdkVersion: 22
- targetSdkVersion: 34
- scriptingBackend: Mono
- targetArchitectures: ARMv7 + ARM64
