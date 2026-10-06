# Decompiled Sources (Downgraded to C# 7.3)

本目录存放从原版 PC 游戏 `Assembly-CSharp.dll` 反编译、并降级到 C# 7.3 的源代码，
以匹配目标引擎 Unity 2019.1.8f1 的脚本编译上限。

## 文件

| 文件 | 行数 | 内容 |
|------|------|------|
| GameCore.cs | 35455 | 游戏自有类（无 namespace，顶部含 27 个 using） |
| ThirdParty.cs | 16901 | 内嵌第三方命名空间：I2.Loc / Moments / Sirenix / Steamworks / ca.HenrySoftware 等 |

> 来源：Assembly-CSharp.decompiled.cs（52258 行）按命名空间边界切分为两部分。
> I2.Loc 等并非独立 DLL，而是内嵌在 Assembly-CSharp.dll 中。

## 降级改动摘要

见 Docs/DOWNGRADE_REPORT.md。核心几何：

- switch 表达式 -> switch 语句（7 处）
- using 声明 -> using 块 / 补齐缺失 using 头（ThirdParty 原有 0 个 using，补 27 个）
- default 字面量 -> default(T)
- 闭包类 <>c 及非法方法名重命名
- 抽象中继声明补齐（LocalizeTarget<T>）
- 歧义消解别名：using Object = UnityEngine.Object; / using ThreadPriority = System.Threading.ThreadPriority;

## 验证状态（第三次编译前）
第二次云端编译（run 37537087696）失败的唯一根因是 **534 条 C# 编译错误**，已在本轮修复：
- CS0111×342 / CS0101×72 / CS0579×33：`Scripts/Core/` 手写占位桩与 `GameCore.cs` 重复定义 → 已 `rm -rf Core`
- CS0616×60 / CS0246×18：Odin `[Button]` / `[InlineEditor]` / `using Sirenix` → 已移除
- CS0103×6：`MathF` 在旧版 .NET 不存在 → 已改 `Mathf`
- CS0012×1（CI 真实仅这一条）：`SteamManager` 的 `SteamAPIWarningMessageHook_t` 委托
  引用 `System.MulticastDelegate`（netstandard）→ 已将 `SteamAchievements`/`SteamManager`
  整块替换为 `Scripts/Platform/SteamAndroidStubs.cs` 安卓安全桩

本地 mcs（-langversion:latest）离线预演结果：
- `MulticastDelegate` CS0012 已消失
- 仅剩 `ICloneable`（ES3Settings）/ `Enum`（TextAlignmentOptions）两条 CS0012，
  均为本地 mcs 未引用 netstandard 的假阳性，CI（Unity 2019.1.8f1）从未报出这两条。
最终权威验证将在 GitHub Actions 上用 Unity 2019.1.8f1 真实编译完成。
