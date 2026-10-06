// =====================================================================
// 安卓安全桩：替换原 Steamworks.NET 的 SteamAchievements / SteamManager
// 目的：移除 SteamAPIWarningMessageHook_t 委托字段。其基类 System.MulticastDelegate
//       定义在 netstandard 程序集，在 Unity 2019.1.8f1 默认脚本环境下无法解析，
//       导致 CI 编译报 CS0012（第二次编译唯一剩下的错误）。
// 保留游戏其余代码所依赖的全部公开 API：
//   SteamAchievements.instance / UnlockSteamAchievement(string) / IsDLCActivated()
//   SteamManager.GOGVersion / SteamManager.Initialized
// 行为对齐“非 Steam 环境”下的原版默认值（成就忽略、DLC 未激活）。
// =====================================================================
using System;
using System.Text;
using UnityEngine;

public class SteamAchievements : MonoBehaviour
{
	public static SteamAchievements instance;

	private void Awake()
	{
		if (instance != null)
		{
			UnityEngine.Object.Destroy(gameObject);
			return;
		}
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}

	public void UnlockSteamAchievement(string ID)
	{
		Debug.Log("[SteamStub] Achievement ignored on Android: " + ID);
	}

	public void CheckAchievementStatus(string ID)
	{
		Debug.Log("[SteamStub] CheckAchievementStatus: " + ID);
	}

	public bool CheckIfAchievementIsUnlocked(string ID)
	{
		return false;
	}

	public void DEBUG_ClearAchievement(string ID)
	{
	}

	public void DEBUG_TestConnection()
	{
	}

	public bool IsDLCActivated()
	{
		return false;
	}
}

[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
	public bool steamEnabled;
	public bool GOGVersionEnabled;
	public static bool GOGVersion;
	protected static SteamManager s_instance;
	protected static bool s_EverInitialized;
	protected bool m_bInitialized;

	protected static SteamManager Instance
	{
		get
		{
			if (s_instance == null)
			{
				return new GameObject("SteamManager").AddComponent<SteamManager>();
			}
			return s_instance;
		}
	}

	public static bool Initialized => false;

	protected virtual void Awake()
	{
		if (steamEnabled)
		{
			if (s_instance != null)
			{
				UnityEngine.Object.Destroy(gameObject);
				return;
			}
			s_instance = this;
			s_EverInitialized = true;
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
		else
		{
			gameObject.SetActive(false);
		}
	}

	protected virtual void OnEnable()
	{
		if (s_instance == null)
		{
			s_instance = this;
		}
	}

	protected virtual void OnDestroy()
	{
		if (s_instance == this)
		{
			s_instance = null;
		}
	}

	protected virtual void Update()
	{
	}
}
