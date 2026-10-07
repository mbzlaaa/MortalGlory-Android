# Mortal Glory 安卓移植 · 当前进度与结论

> 最后更新：B 路线（清空 Library 缓存重建）执行完毕并判负之后。
> 本文档记录**已证实的事实**与**已排除的假设**，供后续路线决策使用。

---

## 0. 目标与硬约束

- 目标：把 PC 版 Unity 游戏《Mortal Glory》（原版 **Unity 2019.1.8f1**，Mono，Windows x64）移植为**原生 Android APK**。
- 硬约束：全程手机端指挥 + 云端 AI 分析 + **GitHub Actions 云端编译**；
  不在手机本地跑 Gradle、不使用 Unity 图形界面、所有改动基于文本代码；
  **严禁 `set -e` / `set -o errexit`**（会导致终端会话退出卡死）。

---

## 1. 工程构成（事实）

- 仓库：`mbzlaaa/MortalGlory-Android`，主分支 `main`。
- `UnityProject/`：AssetRipper 式导出的**散装资源**（Sprite 4690 / Texture2D 2158 /
  MonoBehaviour 2060 / Material 552 / AudioClip 446 / AnimationClip 308 / Font 18 /
  GameObject 98 / Scenes 19 / Shader 4 / TextAsset 12 / Resources 138），
  `.meta` 总数 5284，`assetBundleName` 非空 = 0。
- **`*.cs` 数量 = 0**：游戏逻辑靠 `Plugins/` 里直接塞**原版 Mono DLL**（"DLL 直出"）。
- 无 `AssetBundle` / `BuildPipeline` 调用；`StreamingAssets` 仅 `.gitkeep`。
- 116 个 GUID 文件命中工程 GUID 115 个（miss 仅全零 GUID），
  目录分布 `{Resources:60, Texture2D:19, Material:16, Sprite:13, Font:5, Shader:2}`。
  → 属 Unity per-asset ResourceFile 机制产物，**排除 AssetBundle 污染**。

---

## 2. 已排除的假设（逐条证伪）

| 假设 | 结论 |
|---|---|
| 压缩方式导致越界 | 证伪（全 STORED 后报错一字未变） |
| 分片 / 未对齐导致越界 | 证伪 |
| 合并 `.split` 能修复 | 证伪 |
| 缺 `resources.assets` 是缺陷 | 证伪（CI 版构建自身自洽，引用计数 0） |
| `globalgamemanagers` / `unity default resources` 损坏 | 证伪 |
| 版本不匹配 / 文件头损坏 | 证伪 |
| 签名 / 对齐 | 证伪 |
| **Library 旧缓存污染构建产物** | **证伪（B 路线，见 §3）** |
| **`logcat -d \| grep 'Unity'` 计数可信** | **证伪（被 AI 自身 DeepseekProvider 日志污染，见 §5）** |

### A 路线（本地全 STORED + 保留 `.split`）——失败
报错**一字未变**：`sharedassets0.assets is corrupted!` +
`[Position out of bounds!]` @ `CachedReader.cpp Line: 220`（corrupted 522 次）。
→ 证明是**文件内容与自身元数据不自洽**，彻底否定"打包层（压缩/分片/签名/对齐）"假设。

### B 路线（云端清空 Library 缓存全量重建）——失败
- 改动：缓存 key `Library2-` → `Library9-`；新增构建后自动解剖（`Tools/dump_apk.py`）。
  commit `d307359`（2 files changed, 82 insertions, 4 deletions）。
- 云端冷缓存重建 run `37573088173`，`conclusion=success`（约 12 分钟）。
- **结果：产物结构性缺陷原封不动**（见 §3）。
→ **`Library` 缓存被正式排除为致因。**

---

## 3. 产物结构缺陷（唯一主嫌）

| 指标 | 原版 PC | CI 重建（含冷缓存重建） |
|---|---|---|
| `sharedassets0.assets` | 43.8 MB（元数据） | **272.6 MB（数据全内联）** |
| `sharedassets0.resS` | 251,760,728 B | **不存在（引用数 0）** |
| `sharedassets0.resS` 引用数 | 有 | **0** |
| `resources.assets` | 9,213,024 B | **缺失** |
| `resources.assets.resS` | 5,392,556 B | **缺失** |
| `globalgamemanagers.assets.resS` | 7,007,360 B | **缺失** |
| 三分片情况 | — | sharedassets0=261 / sharedassets1=8 / sharedassets2=77 |

- 旧 `sa0.bin` = 272,631,912 B；新包拼回 = **272,636,040 B**（SHA256 不同，字节级有差异）。
- 三包 + 新包的 Data 压缩分布硬证据：
  - `mg21` 571 条 `{DEFLATE:569, STORED:2}`
  - `mg22` 570 条 `{DEFLATE:568, STORED:2}`
  - `allstored` 227 条 `{STORED:227}`
  - `pure_stored` 570 条 `{STORED:570}`
  - **新构建（冷缓存）571 条 `{DEFLATE:569, STORED:2}`**
- 共同点：`.resS` 引用均 **0**、`.resource` 引用均 223、`resources.assets` 均 **MISSING**。

**结论：病根不在构建缓存，而在"工程/资源重建方式"本身——
重建过程把本该写入 `.resS` 的流式资源**全部内联**进 serialized file，
且该文件在运行时被自身播放器判为 corrupt（`Position out of bounds`）。**

### `sharedassets0.assets` 头部（重建产物）
- 头：`meta=72589 fsize=272631912 ver=19 doff=72624`；off20 起 `2019.1.8f1`。
- off31~35 = `0d 00 00 00 00`；off36 = `fb 00 00 00` → 类型数 **251**；
  类型表自 off40 每条 23 字节（classID 4 + isStripped 1 + scriptTypeIndex 2 + oldTypeHash 16）。
- 读出的 classID 为标准序列（150 / 21 Material / 28 Texture2D / 48 Shader / 49 TextAsset /
  74 AnimationClip / 83 AudioClip / 91 AnimatorController / 128 Font / 213 Sprite /
  1 GameObject / 4 Transform），但**后段错乱**；对象表扫描未命中（`CAND_COUNT 0`）。

---

## 4. 上机复验（新包）

- 新包：artifact `11461569436` → `Android.apk` 91,061,122 B →
  `zipalign` + 重签 → `new_signed.apk` **91,062,960 B**（设备上 base.apk 同尺寸）。
- 装机：`pm uninstall`(Success) + `pm install -r`(Success)。
- 运行：进程存活，启动 `com.unity3d.player.UnityPlayerActivity`，
  日志到 **GL 扩展初始化**（`SystemInfo CPU = ARMv7 VFPv3 NEON`、`Version '2019.1.8f1 (7938dd008a75)'`、
  `Scripting Backend 'mono', CPU 'armeabi-v7a', Stripping 'Disabled'`），
  version `0.0.25`（旧包 `0.0.24`）；无 ANR / FATAL / 崩溃。
- **用户现场反馈**：能启动渲染 Splash，但**文字不显示、按钮点击无反应、不能正常游玩**。
- 若按 PID 过滤后，真实 Unity tag 行极少甚至为 0；抓取窗口内**未见** `corrupted`
  报错（与旧包"25 秒内即报错"不同，可能因抓取过早 / 启动更慢）。

---

## 5. 取证规范（重要，必须遵守）

1. **禁止**用 `logcat -d | grep 'Unity'` 计数——AI 自身的 `DeepseekProvider` 会把整段
   对话摘要（含 "Unity" / "corrupted" / "CachedReader.cpp:220" 字样）打进 logcat，
   造成**假计数**（曾误得 `CORRUPTED=27`，实为 0）。
2. 推荐：`logcat -d > file` 全量落盘后，用 `grep -v DeepseekProvider` 排除污染，
   再按 **PID** 过滤：`logcat -d --pid=$(pidof <pkg>)`。
3. 干净取证法：`logcat -c` → `am force-stop` / `monkey` 启动 → 固定 `sleep` → 按 PID/精确 tag 抓取。
4. 装机：先 `pm uninstall` → 再 `pm install -r`；APK 先 `cp` 到 `/data/local/tmp/` 并 `chmod 666`。

---

## 6. 下一步候选路线（原生 APK 方向）

- **E. 二进制度量诊断（本地，零 CI 成本）**：用自研 Python serialized-file 解析器，
  逐对象对比"原版 PC `sharedassets0.assets`"与"重建产物 `sharedassets0.assets`"，
  读出每个对象的 `m_StreamData{path,offset,size}`，定位**是哪些对象被内联、offset 为何越界**。
  → 给出精确修复点。
- **F. "空壳 + 数据外科手术"**：用 2019.1.8f1 构建**空 Android 工程**得到正确外壳
  （libunity.so / globalgamemanagers 模板 / Data 目录结构），再用工具把原版 PC 数据
  **按 Android 结构重排**（`.assets` 元数据 + `.resS` 资源），仅对 **Shader** 用 GLES 变体、
  **Texture** 用 ETC2/ASTC 替换。
- **G. 混合封装**：把资源在 Android 工程内重新打成 **AssetBundle**，由空壳在运行时加载，
  绕开 `sharedassets` 的序列化路径。

---

## 7. 凭据提醒

- 仓库远端曾内嵌明文 PAT；**任务收尾必须吊销该 PAT**。本文档**不含任何密钥**。
