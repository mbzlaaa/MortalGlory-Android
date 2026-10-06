# Unity 授权阻塞点 · 调查报告

> 结论先行：本项目当前唯一阻塞 = **Unity 授权**。
> 经实测，**纯手机获取 Unity Personal 授权的所有自动化/远程路径均已封死**。
> 唯一可靠解：需要**一台能运行 Unity Hub 的图形界面设备**（借用即可，10 分钟完事）。

---

## 一、已证伪路径（四条，均带一手证据）

### 1. Unity CLI（unity-cli）——无 license 能力 ❌

- 最新版本 `0.1.0-beta.3`，CDN 基址 `https://public-cdn.cloud.unity3d.com/hub/prod/cli/`
- 下载 `unity-linux-arm64`：132,385,088 字节（126MB），
  sha256 `de7c67601f0da841840e99baec0c526b694ceb294023eb32c4f6f2e57e8de944` 校验通过，
  aarch64 本机 glibc 2.39 可直接执行
- 实测 `--help` 顶层命令：`auth / changelog / editors / implode / install /
  install-modules / install-path / language / projects / open / uninstall / upgrade`
  → **无 `license` 命令**
- `auth` 仅 `login / status / logout`，其中 `login` 为 “opens browser” 的交互式 OAuth
- `latest-alpha.json` 为空；`0.1.0` / `0.1.0-beta.4` / `0.2.0` 的 `latest.json` 均返回 307；
  npm 上的 `unity-cli@0.0.7` 是无关第三方包

**结论**：CLI 无授权能力，路线作废。

### 2. 手动激活 `.alf` → `.ulf` —— 官方已停止支持 Personal ❌

`https://license.unity3d.com/manual` 页面原文（现场亲验，2026）：

> To activate a license for an offline computer, upload your license request file here.
> **Unity no longer supports manual activation of Personal licenses.**

另有官方文档三处独立印证：

| 文档 | 原文 |
|---|---|
| 6000.7 | The following procedures **don't apply to Unity Personal** … log in to the Unity Hub |
| 2020.3 | The command line procedure **only works for Plus and Pro** … use the Unity Hub to activate Personal licenses |
| 6.0 | Manual activation … **It doesn't support** … Unity Pro assigned seats, **Unity Personal**, or floating license subscriptions |

**结论**：`.alf` → `.ulf` 对 Personal 已死；game.ci 文档中残留的“变通方案”说明已过期。

### 3. 手机本地跑 Unity Hub —— 官方无 ARM64 Linux 版 ❌

- `https://public-cdn.cloud.unity3d.com/hub/prod/UnityHub.AppImage` → HTTP 200（**x86_64**）
- `https://public-cdn.cloud.unity3d.com/hub/prod/UnityHub-arm64.AppImage` → HTTP **404**
- 最新 Unity Hub 文档已**无 CLI 章节**，不存在 headless 授权入口
- （only `UnityHubSetup-arm64.exe` 存在，即 **Windows on ARM** 版 Hub）

**结论**：aarch64 proot 环境无法运行 Unity Hub，此路物理封死。

### 4. Unity Build Automation（官方云构建）—— 不支持 2019.1.8f1 ❌

官方文档明确仅支持：**Unity 2021 LTS / 2022 LTS / 2023.2 TECH**。
本项目需要 `2019.1.8f1`，不在支持列表内。

**结论**：除非把整个工程升级到 2021+（需大量 API 适配），否则不可用。

---

## 二、现存可行方案

### 方案 A（推荐，最省事）：借一台有图形界面的电脑

1. 装 Unity Hub（Windows / macOS / Linux x64 均可）
2. 用**将来 CI 要用的同一个 Unity 账号**登录
3. `Unity Hub > Preferences(偏好设置) > Licenses(许可证) > Add > Get a free personal license > Agree`
   - ⚠️ **关键**：必须点 `Add` 走完激活流程。
     只看到 Hub 里“显示有许可证”并不代表 `.ulf` 已生成。
4. 取文件：
   - Windows：`C:\ProgramData\Unity\Unity_lic.ulf`
   - macOS：`/Library/Application Support/Unity/Unity_lic.ulf`
   - Linux：`~/.local/share/unity3d/Unity/Unity_lic.ulf`
5. 内容存为仓库 secret `UNITY_LICENSE`，另配 `UNITY_EMAIL` / `UNITY_PASSWORD`

> 许可证不绑定 Unity 版本与平台，可在 Windows 上激活、在 Linux 上构建。

### 方案 B（坚持“纯手机”，但需折腾）：云端桌面 + noVNC

在 GitHub Codespaces（免费额度 120 core-hours/月）内搭建 noVNC 桌面，
运行 `UnityHub.AppImage`（先 `--appimage-extract` 规避 FUSE 限制），
用**手机浏览器**操作 Hub GUI 完成登录与激活，再从终端导出 `.ulf`。

### 方案 C（付费）：Unity Pro 订阅

拿到形如 `XX-XXXX-XXXX-XXXX-XXXX-XXXX` 的 serial 后，
可直接命令行激活，配 secret `UNITY_SERIAL`。这是唯一无需图形界面的路子。

---

## 三、拿到授权后的收尾清单

1. 配 secrets 到 `mbzlaaa/MortalGlory-Android`（Personal 用 `UNITY_LICENSE`，Pro 用 `UNITY_SERIAL`）
2. 重触发 CI 验证 `build.yml`
3. 推进**场景重建**（当前 `Assets/Scenes/` 无任何场景文件，无场景 = 空 APK）
4. 用完吊销 GitHub token
