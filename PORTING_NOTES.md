# Mortal Glory → Android 移植笔记

> 状态：**暂停中（阶段性封存）**　仓库：`mbzlaaa/MortalGlory-Android`（main）
> 协作模式：手机端指挥 + 云端 AI 操作 + GitHub Actions 云端编译

---

## 1. 项目概况

| 项 | 内容 |
|---|---|
| 游戏 | **Mortal Glory**（Redbeak Games，2020） |
| 原版 | PC 版（Steam），约 510MB |
| 引擎 | **Unity 2019.1.8f1**，**Mono 后端**（非 IL2CPP） |
| 原版数据 | `/sdcard/project/Mortal Glory`（sharedassets0.assets + .resS + .resource、resources.assets、globalgamemanagers） |
| 目标 | 生成可安装、可玩的 Android APK |
| 包名 | `com.redbeak.mortalglory` |
| CI | GitHub Actions + game-ci unity-builder，Unity 2019.1.8f1（国际版） |

---

## 2. 移植技术路线（核心决策，勿动摇）

### 2.1 路线选择
- ✅ **AssetRipper 逆向 PC 数据 → 重建 Unity 工程 → CI 构建 Android 包**
- ❌ **不走** IL2CPP + Il2CppDumper（已证实游戏是 **Mono 后端**：存在 MonoBleedingEdge/、mono-2.0-bdwgc.dll、Managed/Assembly-CSharp.dll，**无** GameAssembly.dll / global-metadata.dat）
- ❌ **不走** Route H（PC 构建数据直投 Android 外壳）——已证伪：APK 可装、窗口能起，约 7 秒后 SIGKILL，报 `Unable to initialize the Unity Engine.`。PC 构建数据不能直接喂给 Android 原生 Unity 引擎。

### 2.2 关键原则
1. **脚本不重编译**：直接放原版 `Managed/*.dll`（Mono 托管程序集为平台无关 CIL，可跑在 Android Mono 运行时）。
   - 依据：CI 重编译产出的 Assembly-CSharp.dll 仅 189KB，原版 713KB，**重编译丢失约 73% 代码**（AssetRipper 导出的是签名 stub，方法体被清空）。
2. **唯一平台耦合是 Steam**（P/Invoke steam_api64），通过禁用 Steam 绕过。
3. **资源/场景问题用替换 + 补丁解决**，不重建场景（场景重建是最大瓶颈）。

### 2.3 成功率评估（立项时）
| 步骤 | 成功率 |
|---|---|
| DLL 可移植性验证 | 90% |
| AssetRipper 逆向 | 80% |
| **场景/prefab 正确重建** | **40–55%（唯一真瓶颈）** |
| CI 编译出 APK | 85% |
| 装上可玩 | 60–70% |
| 一次性干净成功 | 约 30–35% |
| 可反复迭代后最终 | 约 55–65% |

---

## 3. 已完成并验证有效（勿回退）

### 3.1 ✅ 字体黑块彻底修复
- **现象**：主菜单文字显示为一团黑色。
- **根因**：工程内 **13 个 shader 全是 DummyShaderTextExporter 假货**（11 个 TMP shader + 2 个 Custom shader）。
- **修复**：替换为**官方 TMP shader**（11 个 SDF shader）+ 官方 TMPro_Properties.cginc / TMPro.cginc / TMPro_Mobile.cginc / TMPro_Surface.cginc / SDFFunctions.hlsl。
- **结果**：用户确认「主菜单字体问题完全解决了」。
- **补充**：TMP 字体 atlas（JosefinSans-Bold/SemiBold、NotoSansCJKsc 系列）内容正常（A 通道 mean 63.72 / max 255，与原版一致）。

### 3.2 ✅ DLL stub 替换
- **现象**：工程 Assets/Plugins/ 内的 DLL 全是 **AssetRipper 导出的签名 stub 版**（类型 413、方法签名 2726 保留，但**方法体 IL 被清空**）。
- **修复**：用原版完整 DLL 替换 **17 个**（含 Assembly-CSharp.dll 189KB→713KB、Assembly-CSharp-firstpass.dll 365KB→592KB、Unity.TextMeshPro.dll 104KB→331KB 等），**md5 一致**。

### 3.3 ✅ Steam 绕过
- **现象**：Android 上 steam_api64 P/Invoke 崩溃。
- **修复**：MainMenu.unity 中 SteamManager 的 `steamEnabled: 1` → **0**。

### 3.4 ✅ 网页链接按钮
- 右侧作者挂的 2 个游戏 + 4 个社交平台链接，点击可跳转浏览器（OpenURL 生效）。用户确认。

---

## 4. 当前卡点：主菜单按钮点击无效（攻坚中）

### 4.1 症状对照（用户实测）
| 能用 | 不能用 |
|---|---|
| Quit（退出） | Continue |
| 右侧 2 个游戏链接 + 4 个社交平台链接（OpenURL 跳浏览器） | New Game |
| | Hall of Fame |
| | Options |
| | Tutorial |
| | Credits |

### 4.2 ★★★ 决定性证据（最关键的技术发现）
抓到的**完整调用堆栈**：

```
at UnityEngine.UI.Button.Press ()
at UnityEngine.UI.Button.OnPointerClick (UnityEngine.EventSystems.PointerEventData eventData)
at UnityEngine.EventSystems.ExecuteEvents.Execute (IPointerClickHandler handler, ...)
at UnityEngine.EventSystems.ExecuteEvents.Execute[T] (GameObject target, ...)
...
TypeLoadException: Could not resolve type with token 01000070
  (from typeref, class/assembly UnityEngine.CoreModule.Debug, netstandard, Version=2.0.0.0, ...)
```

**结论（推翻了此前所有猜测）**：
1. ✅ **onClick 事件链路完全正常**：Button.Press → UnityEvent.Invoke → 目标方法（方法**确实被调用**）。
2. ❌ **方法执行时抛异常**：TypeLoadException —— `UnityEngine.CoreModule.Debug` 类型的 typeref **scope 错误地指向 netstandard**，导致 Debug.Log 调用无法解析。
3. **问题不在按钮绑定**，而在**方法执行**（调试注入代码 / 或方法内的类型引用）。

### 4.3 排错过程（经验）
- 曾误判为 AudioManager.AudioEffect 越界（IndexOutOfRange，at AudioManager.AudioEffect [0x01744]，位置在 `BloodDrinkSounds[Random.Range(0,Count)]` 分支）。统计场景内 20 个被索引音效列表项数**全部非空**（Clicks=4、BloodDrinkSounds=3、meleeSmallSounds=18…），对应 UI Tight 02/03/04/05.ogg 也都存在 → **排除「列表为空」假设**。
- 曾用 Mono.Cecil 将 AudioManager.AudioEffect 方法体**清空为 ret**（临时禁用音频分发），但按钮仍无效 → 说明 AudioEffect 不是主因。
- 曾注入 `Debug.Log("CALLED:xxx")` 埋点（第一版 AddLog.cs），**因 typeref scope 错误（module.TypeSystem.CoreLibrary = netstandard）本身抛异常**，反而掩盖了真相。
- 最终通过**完整异常堆栈**定位到 Button.Press 链路，证明 onClick 正常。

### 4.4 根因分析
- 按钮方法被调用 → 方法内（或注入的）Debug.Log 类型解析失败 → **方法在第一条指令就抛异常中断** → 表现为「点了没反应」。
- 为什么 Quit / 链接按钮能用：它们**不依赖 MainMenuScript 的复杂方法**（走 Application.Quit / Application.OpenURL 的简单绑定路径），不经过出问题的方法。

---

## 5. 工具链与环境

| 工具 | 说明 |
|---|---|
| Mono 6.8.0 + mcs + Mono.Cecil.dll | 修改 DLL（/tmp/cecil/lib/net40/，须与 exe 同目录运行） |
| monodis | 反汇编托管 DLL |
| UnityPy / Python3 | 资源解析 |
| GitHub Actions | game-ci unity-builder（2019.1.8f1），push 触发 |
| 部署链 | push → CI → 下载 artifact → 解出 Android.apk → /data/local/tmp → pm uninstall + pm install -r -d → am start com.redbeak.mortalglory/com.unity3d.player.UnityPlayerActivity |

### 自定义 Mono.Cecil 工具（位于 /sdcard/Download/）
- `FixAudio.cs`：清空指定方法体（用法 `mono FixAudio.exe in.dll out.dll MethodName`）。
- `AddLog.cs`：注入 Debug.Log 埋点（**旧版有 bug**，typeref scope 错误）。
- `AddLog2.cs`：修复版（用 UnityEngine.CoreModule assembly reference 作 scope）——但**实测仍未生效**，注入后运行时仍报 token 01000070。

---

## 6. 硬约束 / 环境坑（务必遵守）

1. **全程手机端指挥 + 云端 AI 分析 + GitHub Actions 云端编译**；不在手机本地跑 Gradle、不用 Unity 图形界面、所有改动基于文本代码。
2. **严禁**在命令中使用 `set -e` / `set -o errexit`（会导致终端会话退出卡死）。
3. 手机 Android shell（super_admin:shell）执行器为 **sh -e**：任何子命令非 0 返回会**中断整串命令**（如 pidof 查不到进程即返回 1）→ 必须用 `|| true` 兜底。
4. shell **不能向 /sdcard 写文件**，日志/临时文件落 /data/local/tmp/。
5. **heredoc / 多行命令易被截断**；含 `!` 的命令会触发 bash 历史扩展报错（!u! event not found）→ 脚本一律落地为文件（create_file）再执行。
6. 单次工具输出过长会触发「调用被截断作废」→ 命令要精简、分批。
7. **前台抢占坑**：微信小程序浮窗（AppBrandUI00）会顽固占据前台，导致 input tap 点到别的应用。自动点击前须确保游戏真正在前台（dumpsys window | grep mCurrentFocus）。
8. CI artifact 下载需带 Authorization: token 头，URL 先从 artifacts API 取出。
9. unzip 不存在，改用 Python zipfile。

---

## 7. 待办 & 下一步方向

### 立即待办
1. **测试干净版**（commit e215ff0，仅禁用 AudioEffect、**无埋点**）——验证「移除埋点后 New Game 是否可点开」。
2. 若干净版仍失败 → 说明方法内还有别的类型引用/异常，需继续定位。
3. **恢复 AudioManager.AudioEffect**：当前被清空为 ret 只是**临时绕过**，导致音效/背景音乐全失效。应改为**健壮性修复**（列表越界保护），而非永久禁用。
4. **背景音乐缺失**（用户已报）——待音频系统恢复后排查。

### 方向性建议
- 埋点调试**不要再用会抛异常的 Debug.Log**；若必须用，改用**已存在的 MethodReference**（从 DLL 内已有的 Debug 调用复制）或 System.Console，避免 typeref scope 问题。
- 优先用**异常堆栈**定位，而非猜测。
- 场景/prefab 重建仍是最大风险点，若后续出现场景级问题，考虑有真机 Unity 编辑器辅助。

---

## 8. 安全待办（重要）

- ⚠️ 仓库/命令历史中曾**明文出现过 GitHub PAT**，**必须尽快吊销并更换**，git remote 换成无 token 地址（改用凭据/SSH）。
- 本文档及仓库内**不得写入任何 token/密钥**。

---

## 9. 变更历史（关键 commit）

| commit | 说明 |
|---|---|
| `dc1334b` | fix: 禁用 AudioManager.AudioEffect 避免越界中断按钮逻辑 |
| `d31ea05` | debug: 注入日志到 MainMenuScript 按钮方法 |
| `2285196` | fix: 修正埋点的 UE.CoreModule 引用 |
| `e215ff0` | clean: 禁用 AudioEffect，移除日志（**待测版本**） |

---

## 10. 结论

- **已攻克**：字体渲染、DLL stub、Steam 耦合、DLL 完整性。
- **已定位**：按钮「点了没反应」的根因是**方法执行时抛 TypeLoadException**，而**不是** onClick 绑定问题（调用堆栈已证）。
- **暂停点**：干净版（e215ff0）待上机验证；音频系统需恢复。
- **总体判断**：路线正确、障碍逐个在收敛，属于「多根因逐层剥离」的攻坚阶段。
