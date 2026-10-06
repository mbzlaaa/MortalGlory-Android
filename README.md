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
- [ ] 阶段 1：工程骨架搭建（进行中）
- [ ] 阶段 2：反编译源码回填
- [ ] 阶段 3：资源导出与重导入
- [ ] 阶段 4：场景重建
- [ ] 阶段 5：平台适配完善
- [ ] 阶段 6：首次云端编译出包

---

## 📦 关键工程参数

- 包名：`com.redbeak.mortalglory`
- 最小 SDK：Android 22 (5.1)
- 目标 ABI：`ARM64` + `ARMv7`
- IL2CPP：**关闭**（保持 Mono）
