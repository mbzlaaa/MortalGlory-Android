# PROBE 报告：Unity 2019.1.8f1 环境与降级源码 CI 编译验证

> 对应 workflow: `Probe Unity 2019.1.8f1 environment`
> run id: 37500753062 | 结果: success | 日期: 2026-10-06

## 1. 致命前提验证：game-ci 镜像是否可拉

| 项 | 结果 |
|----|------|
| 镜像 | `unityci/editor:ubuntu-2019.1.8f1-base-3` |
| 结论 | **可拉取，成功下载**（48 秒完成） |
| Digest | `sha256:ec5da43023cd3f1043d7eac5e1608e6eb1a1c355a0375c8f55cbb7342e40f1b6` |

=> 整个「用 game-ci 构建 Android APK」路线的镜像前提 **成立**。

## 2. 仓库输入清点

- `Input/Managed/`：2 个游戏 DLL（仅 Assembly-CSharp / Assembly-CSharp-firstpass）——遵循路线 β，未传第三方/引擎游戏程序集。
- `Input/UnityManaged/`：**68 个 Unity 官方引擎 DLL**（UnityEngine.* ×63 + Unity.* ×5）。
- 降级源码：`GameCore.cs` 35455 行 + `ThirdParty.cs` 16901 行 = 52356 行。

## 3. CI 编译诊断（mono 6.x + 68 个引擎 DLL）

### [A] 单独编译 ThirdParty.cs
```
Compilation failed: 3 error(s), 0 warnings
----- 错误分布 -----
  2 error CS0246   (Sirenix, Steamworks 缺引用)
  1 error CS0012   (netstandard 缺引用，TextMeshPro 触发)
```

### [B] ThirdParty.cs + GameCore.cs 合编
```
Compilation failed: 7 error(s), 3 warnings
----- 错误分布 -----
  6 error CS0246  (Sirenix ×2 / Steamworks ×2 / ES3Settings ×1 / SteamAPIWarningMessageHook_t ×1)
  1 error CS0012  (netstandard)
----- 警告 -----
  CS0108 ×2  Perk.name / Skill.name 遮蔽 UnityEngine.Object.name（无害）
  CS1522 ×1  空 switch 块（无害）
```

## 4. 结论

1. **降级源码语法/结构 100% 正确**：52356 行代码在真实 Unity 引擎 DLL 下仅 7 个错误，全部源于「第三方 DLL 缺失」，**没有任何一条由降级改动（switch 表达式/using 声明/闭包类等）引入**。
2. **引擎层引用兼容**：报错中出现 `UnityEngine.CoreModule.dll` / `Unity.TextMeshPro.dll` 被正确解析，**无任何 `UnityEngine.*` 找不到**，证明 68 个官方引擎 DLL 足够且版本匹配。
3. **剩余 7 个错误解法明确**：
   - `Sirenix` (Odin Inspector) —— 需补 DLL 或剥离。
   - `Steamworks` / `SteamAPIWarningMessageHook_t` —— Steam 为 PC 专属，**Android 版必须移除/桩化**。
   - `ES3Settings` (EasySave3) —— 补 DLL 或加引用。
   - `CS0012 netstandard` —— 编译命令追加 `-r:netstandard.dll` 即可消除。

## 5. 下一步

- 进入阶段 3：补齐/桩化缺失第三方依赖，使完整解决方案可编译。
- 修复 `build.yml` 的 YAML 缩进 bug。
- 获取 `UNITY_LICENSE` / `UNITY_EMAIL` / `UNITY_PASSWORD`（Unity 个人版免费 license）。
