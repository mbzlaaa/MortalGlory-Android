# MortalGlory-Android

《Mortal Glory》(Redbeak Games) 的 **安卓移植工程**。
本项目通过「反编译 Mono DLL + 资源重导入 + GitHub Actions 云端编译」的方式，
将 PC (Windows/Steam) 版 Unity 游戏移植到 Android。

> ⚠️ **版权声明**：本项目仅供**个人学习与研究**使用。游戏本体、美术、音频等
> 一切资源版权归 **Redbeak Games** 所有。请勿用于任何商业用途或二次分发。

---

## 📌 游戏身份卡（逆向分析结论）

| 项目 | 值 |
|---|---|
| 游戏名 | Mortal Glory |
| 开发商 | Redbeak Games |
| 引擎 | **Unity 2019.1.8f1** |
| 脚本后端 | **Mono**（非 IL2CPP） |
| 原始平台 | Windows x64 (Steam) |
| 类型 | 2D 精灵 · 回合制战术 / Roguelike |
| 存档系统 | Easy Save 3 (ES3) |
| 平台耦合点 | 仅 Steamworks.NET（需 Stub） |

---

## 🗂️ 工程结构

```
MortalGlory-Android/
├── .github/workflows/build.yml   # ★ 云端编译脚本 (game-ci)
├── UnityProject/                 # Unity 工程主体 (Android target)
│   ├── Assets/
│   │   ├── Scenes/               # 场景（需重建）
│   │   ├── Scripts/
│   │   │   ├── Core/             # 游戏核心逻辑（反编译回填）
│   │   │   ├── UI/               # UI 逻辑
│   │   │   ├── Data/             # 数据类
│   │   │   ├── Platform/         # ★ 平台适配（SteamStub / TouchInput）
│   │   │   └── ThirdParty/       # 第三方插件（ES3 / Odin / I2 / TMP）
│   │   ├── Sprites/              # 2D 美术资源
│   │   ├── Audio/                # 音频资源
│   │   ├── Fonts/                # 字体
│   │   ├── Resources/            # 静态资源
│   │   └── StreamingAssets/      # 流式资源
│   ├── Packages/manifest.json    # 包依赖
│   └── ProjectSettings/          # 工程设置
├── Tools/                        # 资源提取工具链 (AssetStudio 等)
├── Docs/                         # 移植文档
└── Output/                       # 构建产物
```

---

## 🚀 编译流程（GitHub Actions）

1. 将本仓库推送到 GitHub（分支 `main`）。
2. 在仓库 **Settings → Secrets and variables → Actions** 配置：
   - `UNITY_LICENSE`（Unity 个人版授权 .ulf 内容或 Pro 序列号）
   - `UNITY_EMAIL`
   - `UNITY_PASSWORD`
3. 推送到 `main` 或在 Actions 页手动 **Run workflow**。
4. 编译完成后，在 Actions 运行页下载 Artifact：`MortalGlory-Android-APK`。

---

## 📋 进度

- [x] 阶段 0：逆向分析（Unity 版本 / 脚本后端 / 资源机制 / 依赖清单）
- [x] 阶段 1：工程骨架搭建
- [x] 阶段 2：脚本回填（采用**原版托管 DLL 直投**：Mono 程序集平台无关，避免重编译丢失约 73% 代码）
- [x] 阶段 3：资源导出与重导入（AssetRipper 逆向 PC 数据 → Unity 工程）
- [x] 阶段 4：场景重建（主包 3 个场景 level0/1/2）
- [x] 阶段 5：平台适配完善（Steam P/Invoke 处理、shader/音频修复、I2 本地化修复）
- [x] 阶段 6：首次云端编译出包（GitHub Actions + Unity 2019.1.8f1 国际版）
- [x] 阶段 7：实机可玩验证（APK 可正常安装启动、进入游戏、BGM 正常）
- [ ] 阶段 8：问题修复与打磨（进行中）

### 当前状态（2026-10-10）

- ✅ CI 可稳定出包，产物 `build/Android/MortalGlory-Android.apk`（已签名、I2 本地化完整）。
- ✅ 设备实测：安装启动正常、可进入游戏、背景音乐正常。
- 🔧 近期修复中：
  - **触摸端 tooltip 秒退**：移动端触摸被 EventSystem 判为"进入随即退出"，触发 `OnPointerExit → HideTooltip → ClosingTimer(0.2s) → CloseAllTooltips`，导致点击书本/道具/人物后弹窗 0.2 秒即消失。已通过 DLL 补丁修复：①移除 `OnPointerExit` 中的 `HideTooltip()` 调用（16 处）；②在 `UIManager.Update()` 注入"点击空白处关闭弹窗"逻辑。
  - **中文显示不全**：`NotoSansCJKsc-Bold SDF` 字体资产为残缺汉化残留（Static 模式、仅烘焙 2 个字符、atlas 512×512），待修复。

---

## 📦 关键工程参数

- 包名：`com.redbeak.mortalglory`
- 最小 SDK：Android 22 (5.1)
- 目标 ABI：`ARM64` + `ARMv7`
- IL2CPP：**关闭**（保持 Mono）
