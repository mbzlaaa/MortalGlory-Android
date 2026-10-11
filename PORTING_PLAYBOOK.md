# 手机端 AI 自动化游戏移植 Playbook（Mortal Glory 实战沉淀）

> 用途：以后用 AI 做「PC Unity 游戏 → Android」移植时，直接把本文件喂给 AI，按关键词流程走。
> 范式：**手机端指挥 + 云端 AI 反汇编/打补丁 + GitHub Actions 云端编译出包 + 手机实测反馈**，全程文本化、零本地 Gradle、零 Unity GUI。

---

## 0. 一句话方法论

> 不重编译逻辑，只搬运程序集；用 IL 补丁解决平台耦合；用 CI 出包；用手机实测闭环。

---

## 1. 开局三问（技术判定）

| 问题 | 判定关键词 | 结论 |
|---|---|---|
| 后端是 Mono 还是 IL2CPP？ | Mono: `MonoBleedingEdge/`、`mono-2.0-bdwgc.dll`、`Managed/Assembly-CSharp.dll`；IL2CPP: `GameAssembly.dll`、`global-metadata.dat` | 决定后续全路线 |
| 版本对不对？ | `ProjectSettings/ProjectVersion.txt` vs CI Unity 版本 | 必须对齐（本例 2019.1.8f1） |
| 逻辑能不能搬？ | 托管 DLL = **平台无关 CIL** | 可直接跑在 Android Mono 运行时，无需重编译 |

**关键洞见**：Mono 后端下，`Managed/*.dll` 是纯 CIL，跨平台。**不要重编译**（本例重编译后 Assembly-CSharp.dll 仅 189KB，原版 713KB，丢约 73% 代码）。

---

## 2. 数据 → 工程（反编译）

| 关键词 | 说明 |
|---|---|
| `AssetRipper` | ✅ 能重建场景/prefab，首选 |
| `AssetStudio` | ❌ 不重建场景，仅导出资源 |
| `sharedassets*.assets` / `.resS` / `.resource` | PC 数据主体 |
| `resources.assets` / `globalgamemanagers` | 全局数据 |
| `.meta` GUID | **场景靠 GUID 定位引用，改文件内容必须保留 GUID** |
| 二进制等长改名 | 破解 Unity 保留名冲突（Cecil 改名会因解析 netstandard 失败，改用二进制等长替换，零重写） |

**保留名冲突陷阱**：`Assembly-CSharp.dll` / `Assembly-CSharp-firstpass.dll` 的内部程序集名撞 Unity 保留名 → 被**静默排除**在脚本源外，是很多"改了没效果"的根因。

**直投 PC 数据证伪**：把 PC 构建数据直接塞进 Android 原生引擎 → 能起窗口但约 7 秒 SIGKILL、零 Unity 日志、报 `Unable to initialize the Unity Engine.`。**PC 数据不能直接喂 Android 引擎**（Route H 已证伪）。

---

## 3. 编译出包（CI）

| 关键词 | 说明 |
|---|---|
| GitHub Actions + game-ci | 云端出 APK，公共仓库分钟数免费无限 |
| `ks.jks`（PKCS12，别名 k）入库 | 同签名覆盖升级，**存档/进度保留** |
| `Unity_lic.ulf` 放 Secrets | 个人版授权；官方唯一支持流程=本地 Unity Hub 生成 .ulf |
| `Application.identifier` / `minSdkVersion` / `targetSdkVersion` | 包身份 |
| split APK | `Data` 全为分片属正常 |

**授权坑**：game-ci 的 `unity-request-activation-file@v2` 已废弃；现官方要求个人版必须用**本地 Unity Hub**（Preferences > Licenses > Add > Get a free personal license）生成 .ulf。
**云环境死路**：Unity Hub 登录必须有「真桌面 + 会把 `unityhub://` 交给系统的浏览器」，无真桌面的云环境（Codespace/Colab/云服务器）都会撞同一堵墙。破局=借真机/云电脑完成登录拿 .ulf。

---

## 4. 运行时修复（IL 补丁）

按现象 → 根因 → 关键词：

| 现象 | 根因 | 解决关键词 |
|---|---|---|
| 字体黑块 | shader 不兼容 | 替换 shader |
| 启动崩溃 | `AudioManager.AudioEffect` | 禁用该调用 |
| 中文显示不全 | Static SDF atlas 512、只烘 2 个字符、源字体缺失 | 生成动态字体（TTCollection Face2 = NotoSansCJKsc） |
| 本地化文本截断 | **字符数当字节数** | 改 UTF-8 字节计数 |
| 触屏弹窗点不掉 | 关闭逻辑只绑 `Input.GetMouseButtonDown(1)`（PC 右键）+ 键盘 | 见 §5 |

**IL 补丁工具链关键词**：`Mono.Cecil`、`MethodDefinition`、`InsertBefore`、`TypeReference(跨程序集)`、`MethodReference(get_activeSelf, HasThis=true)`、`ildump` 反汇编复核、`InitLocals/MaxStackSize`。

---

## 5. 案例：安卓触屏弹窗无法关闭（Patch1–8 演进）

| 版本 | 方案 | 结果 |
|---|---|---|
| 基线 | 原版 `UIManager.Update()` 仅 `KeyAction.15 或鼠标右键 → CloseAllTooltips(3)` | 安卓无右键 → 死锁 |
| Patch1 | `IsPointerOverGameObject()` 无参 | ❌ 触屏永不成立 |
| Patch2 | `touchCount>0` | ❌ 每帧为真 |
| Patch3 | 3 秒自动关 | 不推送 |
| Patch4 | `TouchPhase.Began` + `IsPointerOverGameObject(fingerId)` | ❌ 实测无效 |
| Patch5 | toggle 状态切换 | ❌ |
| Patch6 | 按下武装 + 抬起关闭 + 保护学习链 | ❌ |
| Patch7 | 同位置 60px 内二次点击切换 | ❌ |
| **Patch8** | **拦截"返回按钮"，有弹窗先关弹窗再返回** | ✅ |

**Patch8 机制**：`TownManager` 的 `CloseShop/CloseRecruitScreen/CloseGuildhouse/CloseLobby/CloseInfo`（场景 Button 的 onClick 绑定，代码中无调用者）入口注入 `if (UIManager.__mg_BackCloseTooltip()) return;`；判定 7 个弹窗 GameObject 的 `activeSelf`。

**关键教训**：**先看反汇编确认"谁在调用/如何绑定"，再动手**；把"点空白/二次点击"这类猜测定为证伪路线，不再重走。

---

## 6. 自动化流水线（可复用 Stage）

```
P0  设备勘探   : 定位游戏数据、ProjectVersion、Managed DLL、后端类型
P1  工程重建   : AssetRipper 逆向 → Unity 工程；原版 Managed/*.dll 直投；保留 GUID
P2  适配改造   : 修 shader/字体/本地化/平台耦合(Steam P/Invoke) 等
P3  IL 补丁    : ildump 反汇编 → PatchX.cs(Cecil) → 编译 → 复核 IL
P4  提交推送   : git commit --no-verify → push origin main
P5  CI 出包    : 轮询 run → success → 取 artifact id
P6  后台下载   : nohup curl -C - 循环续传至尺寸达标（前台易超时）
P7  哈希核验   : 解 APK → 比对内部 DLL md5 与补丁 DLL 逐字节一致
P8  装机实测   : cp 到 /data/local/tmp → pm install -r → dumpsys 核验版本 → monkey 启动 → 清理
```

---

## 7. 环境/工程纪律（坑）

| 坑 | 纪律 |
|---|---|
| `pm install` 用 /sdcard 路径 | ❌ system_server 读不了 FUSE → 必须 `cp` 到 `/data/local/tmp/` |
| Shizuku 掉线 | `pidof shizuku_server` 检查，掉了请用户重启 |
| 终端 `set -e` | ❌ 会话直接退出卡死；**严禁** |
| Android shell 是 `sh -e` | 任何子命令非 0 会中断整串 → 用 `\|\| true` 兜底 |
| 前台 `curl` 下大包 | 常超时中断 → 后台 `nohup` 续传 |
| OCR 截图糊 | 命令改成只输出**短数字/短词**（窗口号、字节数、类型A/B） |
| `unzip` 不可用 | 改 `python3 zipfile` |
| `monodis` 大 DLL | 段错误 → 改 Cecil / `strings` |
| 带空格路径 | 必须加引号 |
| 磁盘满 | 及时清 `/tmp`、旧 artifact；勿在核验 hash 前删除 zip/apk |
| 明文 PAT 入库 | 立即吊销 GitHub Token，`git remote set-url` 去明文 |

---

## 8. 验证闭环（结论可信度）

- 每个 Patch：编译输出 `PATCHED N` + IL 复核（dump 文本逐条比对）
- 出包：CI `conclusion=success` + artifact 尺寸
- 装机：`versionName` 递增 + `lastUpdateTime` 吻合
- 内容：APK 内 DLL md5 与补丁 DLL **逐字节一致**
- 体验：用户手机实测（唯一权威验收）

---

## 9. 一句话交接给下一个 AI

> 本项目为 **Mono 后端 Unity 2019.1.8f1** 游戏，采用「原版托管 DLL 直投 + Cecil IL 补丁 + GitHub Actions 出包」；改逻辑走 `ildump → PatchX.cs → 复核 → commit → CI → 核验 md5 → 装机`；别重编译脚本，别猜交互，先反汇编。
