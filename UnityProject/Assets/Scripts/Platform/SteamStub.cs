// ============================================================
//  SteamStub.cs —— Steam 平台依赖的「空实现」桩
// ------------------------------------------------------------
//  原游戏使用 Steamworks.NET 处理：成就、云存档、富状态。
//  安卓没有 Steam，这里提供同名/同签名的空方法，
//  保证游戏调用时不崩溃，只是功能静默失效。
//
//  ⚠️ 这是一个「占位骨架」。等反编译出真正的 SteamManager /
//     SteamAchievements 后，我们需要用这里的桩去替换其实现，
//     或让原代码调用本桩。方法签名需要和原类对齐。
// ============================================================

using UnityEngine;

namespace MortalGlory.Platform
{
    /// <summary>
    /// Steam 功能桩：所有平台相关调用在此被安全地“吞掉”。
    /// </summary>
    public static class SteamStub
    {
        public static bool IsInitialized => false;
        public static bool Enabled => false;

        public static void Init()
        {
            Debug.Log("[SteamStub] Steam 在安卓平台禁用，使用空实现。");
        }

        public static void Shutdown() { }

        // ---- 成就相关（空操作）----
        public static void UnlockAchievement(string id)
        {
            Debug.Log($"[SteamStub] (忽略) 解锁成就: {id}");
        }

        public static void SetStat(string name, int value) { }

        // ---- 云存档相关（直接返回失败，走本地存档）----
        public static bool CloudSave(string fileName, byte[] data) => false;
        public static byte[] CloudLoad(string fileName) => null;

        // ---- 富状态 ----
        public static void SetRichPresence(string key, string value) { }
    }
}
