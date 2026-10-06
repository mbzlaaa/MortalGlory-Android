# Scripts/Core 目录

此目录用于存放**游戏核心逻辑源码**——由反编译 `Assembly-CSharp.dll` 得到。

## 计划回填的关键类（来自阶段 0 分析）

| 类名 | 职责 |
|---|---|
| `BattleProcessor` | 战斗处理核心 |
| `TurnBasedCombatStateMachine` | 回合制战斗状态机 |
| `SaveManager` | 存档管理（基于 ES3） |
| `MapManager` | 地图管理 |
| `Skill` | 技能数据 |
| `Item` | 物品数据 |
| `Equipment` | 装备数据 |
| `SteamManager` | Steam 集成（→ 用 SteamStub 替代） |
| `UIManager` | UI 总控 |

Assembly-CSharp.dll 共含 **413 个类型**。

## 回填方式（下一步）

在 Ubuntu 终端用反编译工具（ILSpy CLI / dnSpy 命令行版）
把 `Assembly-CSharp.dll` 导出为 `.cs` 源码，按命名空间/功能拆分到
本目录及各子目录。

> 本文件仅作占位说明，回填后删除。
