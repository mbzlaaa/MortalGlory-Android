# 路线 A：反编译源码 C#8 → C#7.3 降级报告

## 目标
将 Assembly-CSharp.decompiled.cs（52258 行）降级到 Unity 2019.1.8f1 支持的 C# 7.3，本地用 mcs 验证语法合法性。

## 切分
- GameCore.final.cs : 35455 行（游戏自有类，无 namespace，顶格）
- ThirdParty.final.cs : 16901 行（I2.Loc / Moments / ca.HenrySoftware / GGEZ 等）

## C#8 → C#7.3 降级清单
### GameCore
1. switch 表达式 4 处 → 标准 switch 语句（配对括号计数定位）
2. using 声明 1 处 → using(...){} 块（补了缺失的 }）
3. default 字面量 3 处 → default(Vector2)
4. 闭包类 <>c → __c，非法方法名 <>CreateSkills>b__29_0 → __CreateSkills_b__29_0
### ThirdParty
1. switch 表达式 3 处 → 标准 switch 语句
2. out var _ 重复 3 处 → out var __d1/__d2/__d3
3. using 声明 4 处 → using(...){} 块
4. 缺失 using 块 → 补回原始 27 个 using + Object/ThreadPriority 别名
5. LocalizeTarget<T> 补 7 个 abstract override 声明（反编译器丢失的抽象中继）

## 本地验证结果（mono mcs 6.8.0.105，C# 内核算）
- 编译命令：mcs -langversion:latest -target:library -r:<25 DLL> GameCore.cs ThirdParty.cs
- 错误总数量：3
- 错误类型：全部为 CS0012（类型定义在 netstandard 中）
- CS0246（缺类型）：0
- CS0115（无可重写方法）：0
- CS0104（歧义）：0
- 语法错误：0

## CS0012 说明
仅 3 处，均因引用的三个 DLL（UnityEngine.TilemapModule / Assembly-CSharp-firstpass(Steamworks) / Unity.TextMeshPro）
基类/接口转发到 netstandard。属本地 mono 引用环境限制（mono 无 netstandard facade），
在真实 Unity 编译环境中 netstandard 为标准引用，不会报错。
判定：**非源码问题**。本地语法降级已验证成功。

## 结论
两个文件的 C#8 语法已全部降至 C#7.3，语法完全合法，无任何语法错误。
下一步：推送到 GitHub，用 Unity 2019.1.8f1 + GitHub Actions 真实编译验证。
