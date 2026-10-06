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

## 验证状态

本地 mcs（C# 7.0 内核，-langversion:latest）编译：
- 语法错误零
- CS0246 / CS0115 / CS0104 零
- 仅剩 3 个 CS0012（TileBase / SteamAPIWarningMessageHook_t / TextAlignmentOptions），
  均为类型转发到 netstandard 的本地 mono 引用环境限制，非源码问题。

最终权威验证将在 GitHub Actions 上用 Unity 2019.1.8f1 真实编译完成。
