# 移植笔记 · Mortal Glory

## 阶段 0 结论摘要

### 1. 技术路线
原计划（IL2CPP + Il2CppDumper）**不适用** —— 游戏是 Mono 后端。
改为：**反编译 Assembly-CSharp.dll → C# 源码 → 倒入 Unity 2019.1.8f1 Android 空壳工程 → 资源重导入 → Actions 编译**。

### 2. 三个决定性利好
- **无 AssetBundle / Addressables**（引用次数均为 0）→ 资源是静态资源，导入简单。
- **存档用 ES3**（内嵌于 Assembly-CSharp-firstpass.dll）→ persistentDataPath 自动适配。
- **无 Windows 专属 API** → 唯一平台耦合是 Steam（用 SteamStub 替代）。

### 3. 难度评级
| 维度 | 难度 |
|---|---|
| 代码 | ⭐⭐ |
| 资源 | ⭐⭐⭐ |
| Shader | ⭐ |
| 场景重建 | ⭐⭐⭐⭐（唯一真正瓶颈） |
| 输入 | ⭐⭐ |
| **总体** | **⭐⭐⭐ 中等偏可做** |

### 4. 外部程序集依赖
I2 Localization、Sirenix Odin、TextMeshPro、Timeline、Steamworks.NET、Easy Save 3。

---

## 待办（TODO）

- [ ] 反编译 `Assembly-CSharp.dll` 全量源码 → 回填 `Scripts/Core`、`Scripts/UI`、`Scripts/Data`
- [ ] 反编译/拷贝插件 DLL → `Scripts/ThirdParty`（或直接放 Plugins）
- [ ] 用 AssetStudio 导出 `sharedassets*.assets` 资源 → `Sprites` / `Audio` / `Fonts`
- [ ] 重建场景（`level0/1/2` → Unity 场景）
- [ ] 补齐 `ProjectSettings` 二进制配置（用 Unity 生成）
- [ ] 用户提供 Unity 授权 → 转入方案 X 真实编译
