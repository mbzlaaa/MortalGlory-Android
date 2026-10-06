using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using I2.Loc;
using Moments;
using Moments.Encoder;
using Sirenix.OdinInspector;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using ca.HenrySoftware;
using ca.HenrySoftware.Rage;
using Object = UnityEngine.Object;
using ThreadPriority = System.Threading.ThreadPriority;

namespace I2.Loc
{
	public class CallbackNotification : MonoBehaviour
	{
		public void OnModifyLocalization()
		{
			if (!string.IsNullOrEmpty(Localize.MainTranslation))
			{
				string translation = LocalizationManager.GetTranslation("Color/Red");
				Localize.MainTranslation = Localize.MainTranslation.Replace("{PLAYER_COLOR}", translation);
			}
		}
	}
	public class Example_ChangeLanguage : MonoBehaviour
	{
		public void SetLanguage_English()
		{
			SetLanguage("English");
		}

		public void SetLanguage_French()
		{
			SetLanguage("French");
		}

		public void SetLanguage_Spanish()
		{
			SetLanguage("Spanish");
		}

		public void SetLanguage(string LangName)
		{
			if (LocalizationManager.HasLanguage(LangName))
			{
				LocalizationManager.CurrentLanguage = LangName;
			}
		}
	}
	public class Example_LocalizedString : MonoBehaviour
	{
		public LocalizedString _MyLocalizedString;

		public string _NormalString;

		[TermsPopup("")]
		public string _StringWithTermPopup;

		public void Start()
		{
			Debug.Log((object)_MyLocalizedString);
			Debug.Log((object)LocalizationManager.GetTranslation(_NormalString));
			Debug.Log((object)LocalizationManager.GetTranslation(_StringWithTermPopup));
			Debug.Log((object)(string)(LocalizedString)"Term2");
			Debug.Log((object)_MyLocalizedString);
			Debug.Log((object)(LocalizedString)"Term3");
			LocalizedString localizedString = "Term3";
			localizedString.mRTL_IgnoreArabicFix = true;
			Debug.Log((object)localizedString);
			LocalizedString localizedString2 = "Term3";
			localizedString2.mRTL_ConvertNumbers = true;
			localizedString2.mRTL_MaxLineLength = 20;
			Debug.Log((object)localizedString2);
			Debug.Log((object)localizedString2);
		}
	}
	public class GlobalParametersExample : RegisterGlobalParameters
	{
		public override string GetParameterValue(string ParamName)
		{
			if (ParamName == "WINNER")
			{
				return "Javier";
			}
			if (ParamName == "NUM PLAYERS")
			{
				return 5.ToString();
			}
			return null;
		}
	}
	public class RealTimeTranslation : MonoBehaviour
	{
		private string OriginalText = "This is an example showing how to use the google translator to translate chat messages within the game.\nIt also supports multiline translations.";

		private string TranslatedText = string.Empty;

		private bool IsTranslating;

		public void OnGUI()
		{
			GUILayout.Label("Translate:", Array.Empty<GUILayoutOption>());
			OriginalText = GUILayout.TextArea(OriginalText, new GUILayoutOption[1] { GUILayout.Width((float)Screen.width) });
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("English -> Español", new GUILayoutOption[1] { GUILayout.Height(100f) }))
			{
				StartTranslating("en", "es");
			}
			if (GUILayout.Button("Español -> English", new GUILayoutOption[1] { GUILayout.Height(100f) }))
			{
				StartTranslating("es", "en");
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.TextArea("Multiple Translation with 1 call:\n'This is an example' -> en,zh\n'Hola' -> en", Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("Multi Translate", new GUILayoutOption[1] { GUILayout.ExpandHeight(true) }))
			{
				ExampleMultiTranslations_Async();
			}
			GUILayout.EndHorizontal();
			GUILayout.TextArea(TranslatedText, new GUILayoutOption[1] { GUILayout.Width((float)Screen.width) });
			GUILayout.Space(10f);
			if (IsTranslating)
			{
				GUILayout.Label("Contacting Google....", Array.Empty<GUILayoutOption>());
			}
		}

		public void StartTranslating(string fromCode, string toCode)
		{
			IsTranslating = true;
			GoogleTranslation.Translate(OriginalText, fromCode, toCode, OnTranslationReady);
		}

		private void OnTranslationReady(string Translation, string errorMsg)
		{
			IsTranslating = false;
			if (errorMsg != null)
			{
				Debug.LogError((object)errorMsg);
			}
			else
			{
				TranslatedText = Translation;
			}
		}

		public void ExampleMultiTranslations_Blocking()
		{
			Dictionary<string, TranslationQuery> dictionary = new Dictionary<string, TranslationQuery>();
			GoogleTranslation.AddQuery("This is an example", "en", "es", dictionary);
			GoogleTranslation.AddQuery("This is an example", "auto", "zh", dictionary);
			GoogleTranslation.AddQuery("Hola", "es", "en", dictionary);
			if (GoogleTranslation.ForceTranslate(dictionary))
			{
				Debug.Log((object)GoogleTranslation.GetQueryResult("This is an example", "en", dictionary));
				Debug.Log((object)GoogleTranslation.GetQueryResult("This is an example", "zh", dictionary));
				Debug.Log((object)GoogleTranslation.GetQueryResult("This is an example", "", dictionary));
				Debug.Log((object)dictionary["Hola"].Results[0]);
			}
		}

		public void ExampleMultiTranslations_Async()
		{
			IsTranslating = true;
			Dictionary<string, TranslationQuery> dictionary = new Dictionary<string, TranslationQuery>();
			GoogleTranslation.AddQuery("This is an example", "en", "es", dictionary);
			GoogleTranslation.AddQuery("This is an example", "auto", "zh", dictionary);
			GoogleTranslation.AddQuery("Hola", "es", "en", dictionary);
			GoogleTranslation.Translate(dictionary, OnMultitranslationReady);
		}

		private void OnMultitranslationReady(Dictionary<string, TranslationQuery> dict, string errorMsg)
		{
			if (!string.IsNullOrEmpty(errorMsg))
			{
				Debug.LogError((object)errorMsg);
				return;
			}
			IsTranslating = false;
			TranslatedText = "";
			TranslatedText = TranslatedText + GoogleTranslation.GetQueryResult("This is an example", "es", dict) + "\n";
			TranslatedText = TranslatedText + GoogleTranslation.GetQueryResult("This is an example", "zh", dict) + "\n";
			TranslatedText = TranslatedText + GoogleTranslation.GetQueryResult("This is an example", "", dict) + "\n";
			TranslatedText += dict["Hola"].Results[0];
		}

		public bool IsWaitingForTranslation()
		{
			return IsTranslating;
		}

		public string GetTranslatedText()
		{
			return TranslatedText;
		}

		public void SetOriginalText(string text)
		{
			OriginalText = text;
		}
	}
	public class RegisterBundlesManager : MonoBehaviour, IResourceManager_Bundles
	{
		public void OnEnable()
		{
			if (!ResourceManager.pInstance.mBundleManagers.Contains(this))
			{
				ResourceManager.pInstance.mBundleManagers.Add(this);
			}
		}

		public void OnDisable()
		{
			ResourceManager.pInstance.mBundleManagers.Remove(this);
		}

		public virtual Object LoadFromBundle(string path, Type assetType)
		{
			return null;
		}
	}
	public class ToggleLanguage : MonoBehaviour
	{
		private void Start()
		{
			((MonoBehaviour)this).Invoke("test", 3f);
		}

		private void test()
		{
			List<string> allLanguages = LocalizationManager.GetAllLanguages();
			int num = allLanguages.IndexOf(LocalizationManager.CurrentLanguage);
			if (num < 0)
			{
				num = 0;
			}
			else
			{
				num = (num + 1) % allLanguages.Count;
			}
			((MonoBehaviour)this).Invoke("test", 3f);
		}
	}
	public static class PersistentStorage
	{
		public enum eFileType
		{
			Raw,
			Persistent,
			Temporal,
			Streaming
		}

		private static I2CustomPersistentStorage mStorage;

		public static void SetSetting_String(string key, string value)
		{
			if (mStorage == null)
			{
				mStorage = new I2CustomPersistentStorage();
			}
			mStorage.SetSetting_String(key, value);
		}

		public static string GetSetting_String(string key, string defaultValue)
		{
			if (mStorage == null)
			{
				mStorage = new I2CustomPersistentStorage();
			}
			return mStorage.GetSetting_String(key, defaultValue);
		}

		public static void DeleteSetting(string key)
		{
			if (mStorage == null)
			{
				mStorage = new I2CustomPersistentStorage();
			}
			mStorage.DeleteSetting(key);
		}

		public static bool HasSetting(string key)
		{
			if (mStorage == null)
			{
				mStorage = new I2CustomPersistentStorage();
			}
			return mStorage.HasSetting(key);
		}

		public static void ForceSaveSettings()
		{
			if (mStorage == null)
			{
				mStorage = new I2CustomPersistentStorage();
			}
			mStorage.ForceSaveSettings();
		}

		public static bool CanAccessFiles()
		{
			if (mStorage == null)
			{
				mStorage = new I2CustomPersistentStorage();
			}
			return mStorage.CanAccessFiles();
		}

		public static bool SaveFile(eFileType fileType, string fileName, string data, bool logExceptions = true)
		{
			if (mStorage == null)
			{
				mStorage = new I2CustomPersistentStorage();
			}
			return mStorage.SaveFile(fileType, fileName, data, logExceptions);
		}

		public static string LoadFile(eFileType fileType, string fileName, bool logExceptions = true)
		{
			if (mStorage == null)
			{
				mStorage = new I2CustomPersistentStorage();
			}
			return mStorage.LoadFile(fileType, fileName, logExceptions);
		}

		public static bool DeleteFile(eFileType fileType, string fileName, bool logExceptions = true)
		{
			if (mStorage == null)
			{
				mStorage = new I2CustomPersistentStorage();
			}
			return mStorage.DeleteFile(fileType, fileName, logExceptions);
		}

		public static bool HasFile(eFileType fileType, string fileName, bool logExceptions = true)
		{
			if (mStorage == null)
			{
				mStorage = new I2CustomPersistentStorage();
			}
			return mStorage.HasFile(fileType, fileName, logExceptions);
		}
	}
	public abstract class I2BasePersistentStorage
	{
		public virtual void SetSetting_String(string key, string value)
		{
			try
			{
				int length = value.Length;
				int num = 8000;
				if (length <= num)
				{
					PlayerPrefs.SetString(key, value);
					return;
				}
				int num2 = Mathf.CeilToInt((float)length / (float)num);
				for (int i = 0; i < num2; i++)
				{
					int num3 = num * i;
					PlayerPrefs.SetString($"[I2split]{i}{key}", value.Substring(num3, Mathf.Min(num, length - num3)));
				}
				PlayerPrefs.SetString(key, "[$I2#@div$]" + num2);
			}
			catch (Exception)
			{
				Debug.LogError((object)("Error saving PlayerPrefs " + key));
			}
		}

		public virtual string GetSetting_String(string key, string defaultValue)
		{
			try
			{
				string text = PlayerPrefs.GetString(key, defaultValue);
				if (!string.IsNullOrEmpty(text) && text.StartsWith("[I2split]"))
				{
					int num = int.Parse(text.Substring("[I2split]".Length));
					text = "";
					for (int i = 0; i < num; i++)
					{
						text += PlayerPrefs.GetString($"[I2split]{i}{key}", "");
					}
				}
				return text;
			}
			catch (Exception)
			{
				Debug.LogError((object)("Error loading PlayerPrefs " + key));
				return defaultValue;
			}
		}

		public virtual void DeleteSetting(string key)
		{
			try
			{
				string text = PlayerPrefs.GetString(key, (string)null);
				if (!string.IsNullOrEmpty(text) && text.StartsWith("[I2split]"))
				{
					int num = int.Parse(text.Substring("[I2split]".Length));
					for (int i = 0; i < num; i++)
					{
						PlayerPrefs.DeleteKey($"[I2split]{i}{key}");
					}
				}
				PlayerPrefs.DeleteKey(key);
			}
			catch (Exception)
			{
				Debug.LogError((object)("Error deleting PlayerPrefs " + key));
			}
		}

		public virtual void ForceSaveSettings()
		{
			PlayerPrefs.Save();
		}

		public virtual bool HasSetting(string key)
		{
			return PlayerPrefs.HasKey(key);
		}

		public virtual bool CanAccessFiles()
		{
			return true;
		}

		private string UpdateFilename(PersistentStorage.eFileType fileType, string fileName)
		{
			switch (fileType)
			{
			case PersistentStorage.eFileType.Persistent:
				fileName = Application.persistentDataPath + "/" + fileName;
				break;
			case PersistentStorage.eFileType.Temporal:
				fileName = Application.temporaryCachePath + "/" + fileName;
				break;
			case PersistentStorage.eFileType.Streaming:
				fileName = Application.streamingAssetsPath + "/" + fileName;
				break;
			}
			return fileName;
		}

		public virtual bool SaveFile(PersistentStorage.eFileType fileType, string fileName, string data, bool logExceptions = true)
		{
			if (!CanAccessFiles())
			{
				return false;
			}
			try
			{
				fileName = UpdateFilename(fileType, fileName);
				File.WriteAllText(fileName, data, Encoding.UTF8);
				return true;
			}
			catch (Exception ex)
			{
				if (logExceptions)
				{
					Debug.LogError((object)("Error saving file '" + fileName + "'\n" + ex));
				}
				return false;
			}
		}

		public virtual string LoadFile(PersistentStorage.eFileType fileType, string fileName, bool logExceptions = true)
		{
			if (!CanAccessFiles())
			{
				return null;
			}
			try
			{
				fileName = UpdateFilename(fileType, fileName);
				return File.ReadAllText(fileName, Encoding.UTF8);
			}
			catch (Exception ex)
			{
				if (logExceptions)
				{
					Debug.LogError((object)("Error loading file '" + fileName + "'\n" + ex));
				}
				return null;
			}
		}

		public virtual bool DeleteFile(PersistentStorage.eFileType fileType, string fileName, bool logExceptions = true)
		{
			if (!CanAccessFiles())
			{
				return false;
			}
			try
			{
				fileName = UpdateFilename(fileType, fileName);
				File.Delete(fileName);
				return true;
			}
			catch (Exception ex)
			{
				if (logExceptions)
				{
					Debug.LogError((object)("Error deleting file '" + fileName + "'\n" + ex));
				}
				return false;
			}
		}

		public virtual bool HasFile(PersistentStorage.eFileType fileType, string fileName, bool logExceptions = true)
		{
			if (!CanAccessFiles())
			{
				return false;
			}
			try
			{
				fileName = UpdateFilename(fileType, fileName);
				return File.Exists(fileName);
			}
			catch (Exception ex)
			{
				if (logExceptions)
				{
					Debug.LogError((object)("Error requesting file '" + fileName + "'\n" + ex));
				}
				return false;
			}
		}
	}
	public class I2CustomPersistentStorage : I2BasePersistentStorage
	{
	}
	public class BaseSpecializationManager
	{
		public string[] mSpecializations;

		public Dictionary<string, string> mSpecializationsFallbacks;

		public virtual void InitializeSpecializations()
		{
			mSpecializations = new string[12]
			{
				"Any", "PC", "Touch", "Controller", "VR", "XBox", "PS4", "OculusVR", "ViveVR", "GearVR",
				"Android", "IOS"
			};
			mSpecializationsFallbacks = new Dictionary<string, string>
			{
				{ "XBox", "Controller" },
				{ "PS4", "Controller" },
				{ "OculusVR", "VR" },
				{ "ViveVR", "VR" },
				{ "GearVR", "VR" },
				{ "Android", "Touch" },
				{ "IOS", "Touch" }
			};
		}

		public virtual string GetCurrentSpecialization()
		{
			if (mSpecializations == null)
			{
				InitializeSpecializations();
			}
			return "PC";
		}

		public virtual string GetFallbackSpecialization(string specialization)
		{
			if (mSpecializationsFallbacks == null)
			{
				InitializeSpecializations();
			}
			if (mSpecializationsFallbacks.TryGetValue(specialization, out var value))
			{
				return value;
			}
			return "Any";
		}
	}
	public class SpecializationManager : BaseSpecializationManager
	{
		public static SpecializationManager Singleton = new SpecializationManager();

		private SpecializationManager()
		{
			InitializeSpecializations();
		}

		public static string GetSpecializedText(string text, string specialization = null)
		{
			int num = text.IndexOf("[i2s_");
			if (num < 0)
			{
				return text;
			}
			if (string.IsNullOrEmpty(specialization))
			{
				specialization = Singleton.GetCurrentSpecialization();
			}
			while (!string.IsNullOrEmpty(specialization) && specialization != "Any")
			{
				string text2 = "[i2s_" + specialization + "]";
				int num2 = text.IndexOf(text2);
				if (num2 < 0)
				{
					specialization = Singleton.GetFallbackSpecialization(specialization);
					continue;
				}
				num2 += text2.Length;
				int num3 = text.IndexOf("[i2s_", num2);
				if (num3 < 0)
				{
					num3 = text.Length;
				}
				return text.Substring(num2, num3 - num2);
			}
			return text.Substring(0, num);
		}

		public static string SetSpecializedText(string text, string newText, string specialization)
		{
			if (string.IsNullOrEmpty(specialization))
			{
				specialization = "Any";
			}
			if ((text == null || !text.Contains("[i2s_")) && specialization == "Any")
			{
				return newText;
			}
			Dictionary<string, string> specializations = GetSpecializations(text);
			specializations[specialization] = newText;
			return SetSpecializedText(specializations);
		}

		public static string SetSpecializedText(Dictionary<string, string> specializations)
		{
			if (!specializations.TryGetValue("Any", out var value))
			{
				value = string.Empty;
			}
			foreach (KeyValuePair<string, string> specialization in specializations)
			{
				if (specialization.Key != "Any" && !string.IsNullOrEmpty(specialization.Value))
				{
					value = value + "[i2s_" + specialization.Key + "]" + specialization.Value;
				}
			}
			return value;
		}

		public static Dictionary<string, string> GetSpecializations(string text, Dictionary<string, string> buffer = null)
		{
			if (buffer == null)
			{
				buffer = new Dictionary<string, string>();
			}
			else
			{
				buffer.Clear();
			}
			if (text == null)
			{
				buffer["Any"] = "";
				return buffer;
			}
			int num = 0;
			int num2 = text.IndexOf("[i2s_");
			if (num2 < 0)
			{
				num2 = text.Length;
			}
			buffer["Any"] = text.Substring(0, num2);
			for (num = num2; num < text.Length; num = num2)
			{
				num += "[i2s_".Length;
				int num3 = text.IndexOf(']', num);
				if (num3 < 0)
				{
					break;
				}
				string key = text.Substring(num, num3 - num);
				num = num3 + 1;
				num2 = text.IndexOf("[i2s_", num);
				if (num2 < 0)
				{
					num2 = text.Length;
				}
				string value = text.Substring(num, num2 - num);
				buffer[key] = value;
			}
			return buffer;
		}

		public static void AppendSpecializations(string text, List<string> list = null)
		{
			if (text == null)
			{
				return;
			}
			if (list == null)
			{
				list = new List<string>();
			}
			if (!list.Contains("Any"))
			{
				list.Add("Any");
			}
			int num = 0;
			while (num < text.Length)
			{
				num = text.IndexOf("[i2s_", num);
				if (num >= 0)
				{
					num += "[i2s_".Length;
					int num2 = text.IndexOf(']', num);
					if (num2 >= 0)
					{
						string item = text.Substring(num, num2 - num);
						if (!list.Contains(item))
						{
							list.Add(item);
						}
						continue;
					}
					break;
				}
				break;
			}
		}
	}
	[Serializable]
	public class EventCallback
	{
		public MonoBehaviour Target;

		public string MethodName = string.Empty;

		public void Execute(Object Sender = null)
		{
			if (HasCallback() && Application.isPlaying)
			{
				((Component)Target).gameObject.SendMessage(MethodName, (object)Sender, (SendMessageOptions)1);
			}
		}

		public bool HasCallback()
		{
			if ((Object)(object)Target != (Object)null)
			{
				return !string.IsNullOrEmpty(MethodName);
			}
			return false;
		}
	}
	public enum ePluralType
	{
		Zero,
		One,
		Two,
		Few,
		Many,
		Plural
	}
	public static class GoogleLanguages
	{
		public struct LanguageCodeDef
		{
			public string Code;

			public string GoogleCode;

			public bool HasJoinedWords;

			public int PluralRule;
		}

		public static Dictionary<string, LanguageCodeDef> mLanguageDef = new Dictionary<string, LanguageCodeDef>(StringComparer.Ordinal)
		{
			{
				"Abkhazian",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ab",
					GoogleCode = "-"
				}
			},
			{
				"Afar",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "aa",
					GoogleCode = "-"
				}
			},
			{
				"Afrikaans",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "af"
				}
			},
			{
				"Akan",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ak",
					GoogleCode = "-"
				}
			},
			{
				"Albanian",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "sq"
				}
			},
			{
				"Amharic",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "am"
				}
			},
			{
				"Arabic",
				new LanguageCodeDef
				{
					PluralRule = 11,
					Code = "ar"
				}
			},
			{
				"Arabic/Algeria",
				new LanguageCodeDef
				{
					PluralRule = 11,
					Code = "ar-DZ",
					GoogleCode = "ar"
				}
			},
			{
				"Arabic/Bahrain",
				new LanguageCodeDef
				{
					PluralRule = 11,
					Code = "ar-BH",
					GoogleCode = "ar"
				}
			},
			{
				"Arabic/Egypt",
				new LanguageCodeDef
				{
					PluralRule = 11,
					Code = "ar-EG",
					GoogleCode = "ar"
				}
			},
			{
				"Arabic/Iraq",
				new LanguageCodeDef
				{
					PluralRule = 11,
					Code = "ar-IQ",
					GoogleCode = "ar"
				}
			},
			{
				"Arabic/Jordan",
				new LanguageCodeDef
				{
					PluralRule = 11,
					Code = "ar-JO",
					GoogleCode = "ar"
				}
			},
			{
				"Arabic/Kuwait",
				new LanguageCodeDef
				{
					PluralRule = 11,
					Code = "ar-KW",
					GoogleCode = "ar"
				}
			},
			{
				"Arabic/Lebanon",
				new LanguageCodeDef
				{
					PluralRule = 11,
					Code = "ar-LB",
					GoogleCode = "ar"
				}
			},
			{
				"Arabic/Libya",
				new LanguageCodeDef
				{
					PluralRule = 11,
					Code = "ar-LY",
					GoogleCode = "ar"
				}
			},
			{
				"Arabic/Morocco",
				new LanguageCodeDef
				{
					PluralRule = 11,
					Code = "ar-MA",
					GoogleCode = "ar"
				}
			},
			{
				"Arabic/Oman",
				new LanguageCodeDef
				{
					PluralRule = 11,
					Code = "ar-OM",
					GoogleCode = "ar"
				}
			},
			{
				"Arabic/Qatar",
				new LanguageCodeDef
				{
					PluralRule = 11,
					Code = "ar-QA",
					GoogleCode = "ar"
				}
			},
			{
				"Arabic/Saudi Arabia",
				new LanguageCodeDef
				{
					PluralRule = 11,
					Code = "ar-SA",
					GoogleCode = "ar"
				}
			},
			{
				"Arabic/Syria",
				new LanguageCodeDef
				{
					PluralRule = 11,
					Code = "ar-SY",
					GoogleCode = "ar"
				}
			},
			{
				"Arabic/Tunisia",
				new LanguageCodeDef
				{
					PluralRule = 11,
					Code = "ar-TN",
					GoogleCode = "ar"
				}
			},
			{
				"Arabic/U.A.E.",
				new LanguageCodeDef
				{
					PluralRule = 11,
					Code = "ar-AE",
					GoogleCode = "ar"
				}
			},
			{
				"Arabic/Yemen",
				new LanguageCodeDef
				{
					PluralRule = 11,
					Code = "ar-YE",
					GoogleCode = "ar"
				}
			},
			{
				"Aragonese",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "an",
					GoogleCode = "-"
				}
			},
			{
				"Armenian",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "hy"
				}
			},
			{
				"Assamese",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "as",
					GoogleCode = "-"
				}
			},
			{
				"Avaric",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "av",
					GoogleCode = "-"
				}
			},
			{
				"Avestan",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ae",
					GoogleCode = "-"
				}
			},
			{
				"Aymara",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ay",
					GoogleCode = "-"
				}
			},
			{
				"Azerbaijani",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "az"
				}
			},
			{
				"Bambara",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "bm",
					GoogleCode = "-"
				}
			},
			{
				"Bashkir",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ba",
					GoogleCode = "-"
				}
			},
			{
				"Basque",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "eu"
				}
			},
			{
				"Basque/Spain",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "eu-ES",
					GoogleCode = "eu"
				}
			},
			{
				"Belarusian",
				new LanguageCodeDef
				{
					PluralRule = 6,
					Code = "be"
				}
			},
			{
				"Bengali",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "bn"
				}
			},
			{
				"Bihari",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "bh",
					GoogleCode = "-"
				}
			},
			{
				"Bislama",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "bi",
					GoogleCode = "-"
				}
			},
			{
				"Bosnian",
				new LanguageCodeDef
				{
					PluralRule = 6,
					Code = "bs"
				}
			},
			{
				"Breton",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "br",
					GoogleCode = "-"
				}
			},
			{
				"Bulgariaa",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "bg"
				}
			},
			{
				"Burmese",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "my"
				}
			},
			{
				"Catalan",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ca"
				}
			},
			{
				"Chamorro",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ch",
					GoogleCode = "-"
				}
			},
			{
				"Chechen",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ce",
					GoogleCode = "-"
				}
			},
			{
				"Chichewa",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ny"
				}
			},
			{
				"Chinese",
				new LanguageCodeDef
				{
					PluralRule = 0,
					Code = "zh",
					GoogleCode = "zh-CN",
					HasJoinedWords = true
				}
			},
			{
				"Chinese/Hong Kong",
				new LanguageCodeDef
				{
					PluralRule = 0,
					Code = "zh-HK",
					GoogleCode = "zh-TW",
					HasJoinedWords = true
				}
			},
			{
				"Chinese/Macau",
				new LanguageCodeDef
				{
					PluralRule = 0,
					Code = "zh-MO",
					GoogleCode = "zh-CN",
					HasJoinedWords = true
				}
			},
			{
				"Chinese/PRC",
				new LanguageCodeDef
				{
					PluralRule = 0,
					Code = "zh-CN",
					GoogleCode = "zh-CN",
					HasJoinedWords = true
				}
			},
			{
				"Chinese/Simplified",
				new LanguageCodeDef
				{
					PluralRule = 0,
					Code = "zh-CN",
					GoogleCode = "zh-CN",
					HasJoinedWords = true
				}
			},
			{
				"Chinese/Singapore",
				new LanguageCodeDef
				{
					PluralRule = 0,
					Code = "zh-SG",
					GoogleCode = "zh-CN",
					HasJoinedWords = true
				}
			},
			{
				"Chinese/Taiwan",
				new LanguageCodeDef
				{
					PluralRule = 0,
					Code = "zh-TW",
					GoogleCode = "zh-TW",
					HasJoinedWords = true
				}
			},
			{
				"Chinese/Traditional",
				new LanguageCodeDef
				{
					PluralRule = 0,
					Code = "zh-TW",
					GoogleCode = "zh-TW",
					HasJoinedWords = true
				}
			},
			{
				"Chuvash",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "cv",
					GoogleCode = "-"
				}
			},
			{
				"Cornish",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "kw",
					GoogleCode = "-"
				}
			},
			{
				"Corsican",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "co"
				}
			},
			{
				"Cree",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "cr",
					GoogleCode = "-"
				}
			},
			{
				"Croatian",
				new LanguageCodeDef
				{
					PluralRule = 6,
					Code = "hr"
				}
			},
			{
				"Croatian/Bosnia and Herzegovina",
				new LanguageCodeDef
				{
					PluralRule = 5,
					Code = "hr-BA",
					GoogleCode = "hr"
				}
			},
			{
				"Czech",
				new LanguageCodeDef
				{
					PluralRule = 7,
					Code = "cs"
				}
			},
			{
				"Danish",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "da"
				}
			},
			{
				"Divehi",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "dv",
					GoogleCode = "-"
				}
			},
			{
				"Dutch",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "nl"
				}
			},
			{
				"Dutch/Belgium",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "nl-BE",
					GoogleCode = "nl"
				}
			},
			{
				"Dutch/Netherlands",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "nl-NL",
					GoogleCode = "nl"
				}
			},
			{
				"Dzongkha",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "dz",
					GoogleCode = "-"
				}
			},
			{
				"English",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "en"
				}
			},
			{
				"English/Australia",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "en-AU",
					GoogleCode = "en"
				}
			},
			{
				"English/Belize",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "en-BZ",
					GoogleCode = "en"
				}
			},
			{
				"English/Canada",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "en-CA",
					GoogleCode = "en"
				}
			},
			{
				"English/Caribbean",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "en-CB",
					GoogleCode = "en"
				}
			},
			{
				"English/Ireland",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "en-IE",
					GoogleCode = "en"
				}
			},
			{
				"English/Jamaica",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "en-JM",
					GoogleCode = "en"
				}
			},
			{
				"English/New Zealand",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "en-NZ",
					GoogleCode = "en"
				}
			},
			{
				"English/Republic of the Philippines",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "en-PH",
					GoogleCode = "en"
				}
			},
			{
				"English/South Africa",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "en-ZA",
					GoogleCode = "en"
				}
			},
			{
				"English/Trinidad",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "en-TT",
					GoogleCode = "en"
				}
			},
			{
				"English/United Kingdom",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "en-GB",
					GoogleCode = "en"
				}
			},
			{
				"English/United States",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "en-US",
					GoogleCode = "en"
				}
			},
			{
				"English/Zimbabwe",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "en-ZW",
					GoogleCode = "en"
				}
			},
			{
				"Esperanto",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "eo"
				}
			},
			{
				"Estonian",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "et"
				}
			},
			{
				"Ewe",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ee",
					GoogleCode = "-"
				}
			},
			{
				"Faeroese",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "fo",
					GoogleCode = "-"
				}
			},
			{
				"Fijian",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "fj",
					GoogleCode = "-"
				}
			},
			{
				"Finnish",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "fi"
				}
			},
			{
				"French",
				new LanguageCodeDef
				{
					PluralRule = 2,
					Code = "fr"
				}
			},
			{
				"French/Belgium",
				new LanguageCodeDef
				{
					PluralRule = 2,
					Code = "fr-BE",
					GoogleCode = "fr"
				}
			},
			{
				"French/Canada",
				new LanguageCodeDef
				{
					PluralRule = 2,
					Code = "fr-CA",
					GoogleCode = "fr"
				}
			},
			{
				"French/France",
				new LanguageCodeDef
				{
					PluralRule = 2,
					Code = "fr-FR",
					GoogleCode = "fr"
				}
			},
			{
				"French/Luxembourg",
				new LanguageCodeDef
				{
					PluralRule = 2,
					Code = "fr-LU",
					GoogleCode = "fr"
				}
			},
			{
				"French/Principality of Monaco",
				new LanguageCodeDef
				{
					PluralRule = 2,
					Code = "fr-MC",
					GoogleCode = "fr"
				}
			},
			{
				"French/Switzerland",
				new LanguageCodeDef
				{
					PluralRule = 2,
					Code = "fr-CH",
					GoogleCode = "fr"
				}
			},
			{
				"Fulah",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ff",
					GoogleCode = "-"
				}
			},
			{
				"Galician",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "gl"
				}
			},
			{
				"Galician/Spain",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "gl-ES",
					GoogleCode = "gl"
				}
			},
			{
				"Georgian",
				new LanguageCodeDef
				{
					PluralRule = 0,
					Code = "ka"
				}
			},
			{
				"German",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "de"
				}
			},
			{
				"German/Austria",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "de-AT",
					GoogleCode = "de"
				}
			},
			{
				"German/Germany",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "de-DE",
					GoogleCode = "de"
				}
			},
			{
				"German/Liechtenstein",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "de-LI",
					GoogleCode = "de"
				}
			},
			{
				"German/Luxembourg",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "de-LU",
					GoogleCode = "de"
				}
			},
			{
				"German/Switzerland",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "de-CH",
					GoogleCode = "de"
				}
			},
			{
				"Greek",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "el"
				}
			},
			{
				"Guaraní",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "gn",
					GoogleCode = "-"
				}
			},
			{
				"Gujarati",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "gu"
				}
			},
			{
				"Haitian",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ht"
				}
			},
			{
				"Hausa",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ha"
				}
			},
			{
				"Hebrew",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "he",
					GoogleCode = "iw"
				}
			},
			{
				"Herero",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "hz",
					GoogleCode = "-"
				}
			},
			{
				"Hindi",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "hi"
				}
			},
			{
				"Hiri Motu",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ho",
					GoogleCode = "-"
				}
			},
			{
				"Hungarian",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "hu"
				}
			},
			{
				"Interlingua",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ia",
					GoogleCode = "-"
				}
			},
			{
				"Indonesian",
				new LanguageCodeDef
				{
					PluralRule = 0,
					Code = "id"
				}
			},
			{
				"Interlingue",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ie",
					GoogleCode = "-"
				}
			},
			{
				"Irish",
				new LanguageCodeDef
				{
					PluralRule = 10,
					Code = "ga"
				}
			},
			{
				"Igbo",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ig"
				}
			},
			{
				"Inupiaq",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ik",
					GoogleCode = "-"
				}
			},
			{
				"Ido",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "io",
					GoogleCode = "-"
				}
			},
			{
				"Icelandic",
				new LanguageCodeDef
				{
					PluralRule = 14,
					Code = "is"
				}
			},
			{
				"Italian",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "it"
				}
			},
			{
				"Italian/Italy",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "it-IT",
					GoogleCode = "it"
				}
			},
			{
				"Italian/Switzerland",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "it-CH",
					GoogleCode = "it"
				}
			},
			{
				"Inuktitut",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "iu",
					GoogleCode = "-"
				}
			},
			{
				"Japanese",
				new LanguageCodeDef
				{
					PluralRule = 0,
					Code = "ja",
					HasJoinedWords = true
				}
			},
			{
				"Javanese",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "jv"
				}
			},
			{
				"Kalaallisut",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "kl",
					GoogleCode = "-"
				}
			},
			{
				"Kannada",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "kn"
				}
			},
			{
				"Kanuri",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "kr",
					GoogleCode = "-"
				}
			},
			{
				"Kashmiri",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ks",
					GoogleCode = "-"
				}
			},
			{
				"Kazakh",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "kk"
				}
			},
			{
				"Central Khmer",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "km"
				}
			},
			{
				"Kikuyu",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ki",
					GoogleCode = "-"
				}
			},
			{
				"Kinyarwanda",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "rw",
					GoogleCode = "-"
				}
			},
			{
				"Kirghiz",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ky"
				}
			},
			{
				"Komi",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "kv",
					GoogleCode = "-"
				}
			},
			{
				"Kongo",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "kg",
					GoogleCode = "-"
				}
			},
			{
				"Korean",
				new LanguageCodeDef
				{
					PluralRule = 0,
					Code = "ko"
				}
			},
			{
				"Kurdish",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ku"
				}
			},
			{
				"Kuanyama",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "kj",
					GoogleCode = "-"
				}
			},
			{
				"Latin",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "la"
				}
			},
			{
				"Luxembourgish",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "lb"
				}
			},
			{
				"Ganda",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "lg",
					GoogleCode = "-"
				}
			},
			{
				"Limburgan",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "li",
					GoogleCode = "-"
				}
			},
			{
				"Lingala",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ln",
					GoogleCode = "-"
				}
			},
			{
				"Lao",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "lo"
				}
			},
			{
				"Latvian",
				new LanguageCodeDef
				{
					PluralRule = 5,
					Code = "lv"
				}
			},
			{
				"Luba-Katanga",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "lu",
					GoogleCode = "-"
				}
			},
			{
				"Lithuanian",
				new LanguageCodeDef
				{
					PluralRule = 5,
					Code = "lt"
				}
			},
			{
				"Manx",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "gv",
					GoogleCode = "-"
				}
			},
			{
				"Macedonian",
				new LanguageCodeDef
				{
					PluralRule = 13,
					Code = "mk"
				}
			},
			{
				"Malagasy",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "mg"
				}
			},
			{
				"Malay",
				new LanguageCodeDef
				{
					PluralRule = 0,
					Code = "ms"
				}
			},
			{
				"Malay/Brunei Darussalam",
				new LanguageCodeDef
				{
					PluralRule = 0,
					Code = "ms-BN",
					GoogleCode = "ms"
				}
			},
			{
				"Malay/Malaysia",
				new LanguageCodeDef
				{
					PluralRule = 0,
					Code = "ms-MY",
					GoogleCode = "ms"
				}
			},
			{
				"Malayalam",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ml"
				}
			},
			{
				"Maltese",
				new LanguageCodeDef
				{
					PluralRule = 12,
					Code = "mt"
				}
			},
			{
				"Maori",
				new LanguageCodeDef
				{
					PluralRule = 2,
					Code = "mi"
				}
			},
			{
				"Marathi",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "mr"
				}
			},
			{
				"Marshallese",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "mh",
					GoogleCode = "-"
				}
			},
			{
				"Mongolian",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "mn"
				}
			},
			{
				"Nauru",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "na",
					GoogleCode = "-"
				}
			},
			{
				"Navajo",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "nv",
					GoogleCode = "-"
				}
			},
			{
				"North Ndebele",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "nd",
					GoogleCode = "-"
				}
			},
			{
				"Nepali",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ne"
				}
			},
			{
				"Ndonga",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ng",
					GoogleCode = "-"
				}
			},
			{
				"Northern Sotho",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ns",
					GoogleCode = "st"
				}
			},
			{
				"Norwegian",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "nb",
					GoogleCode = "no"
				}
			},
			{
				"Norwegian/Nynorsk",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "nn",
					GoogleCode = "no"
				}
			},
			{
				"Sichuan Yi",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ii",
					GoogleCode = "-"
				}
			},
			{
				"South Ndebele",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "nr",
					GoogleCode = "-"
				}
			},
			{
				"Occitan",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "oc",
					GoogleCode = "-"
				}
			},
			{
				"Ojibwa",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "oj",
					GoogleCode = "-"
				}
			},
			{
				"Church\u00a0Slavic",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "cu",
					GoogleCode = "-"
				}
			},
			{
				"Oromo",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "om",
					GoogleCode = "-"
				}
			},
			{
				"Oriya",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "or",
					GoogleCode = "-"
				}
			},
			{
				"Ossetian",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "os",
					GoogleCode = "-"
				}
			},
			{
				"Pali",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "pi",
					GoogleCode = "-"
				}
			},
			{
				"Pashto",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ps"
				}
			},
			{
				"Persian",
				new LanguageCodeDef
				{
					PluralRule = 0,
					Code = "fa"
				}
			},
			{
				"Polish",
				new LanguageCodeDef
				{
					PluralRule = 8,
					Code = "pl"
				}
			},
			{
				"Portuguese",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "pt"
				}
			},
			{
				"Portuguese/Brazil",
				new LanguageCodeDef
				{
					PluralRule = 2,
					Code = "pt-BR",
					GoogleCode = "pt"
				}
			},
			{
				"Portuguese/Portugal",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "pt-PT",
					GoogleCode = "pt"
				}
			},
			{
				"Punjabi",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "pa"
				}
			},
			{
				"Quechua",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "qu",
					GoogleCode = "-"
				}
			},
			{
				"Quechua/Bolivia",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "qu-BO",
					GoogleCode = "-"
				}
			},
			{
				"Quechua/Ecuador",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "qu-EC",
					GoogleCode = "-"
				}
			},
			{
				"Quechua/Peru",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "qu-PE",
					GoogleCode = "-"
				}
			},
			{
				"Rhaeto-Romanic",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "rm",
					GoogleCode = "ro"
				}
			},
			{
				"Romanian",
				new LanguageCodeDef
				{
					PluralRule = 4,
					Code = "ro"
				}
			},
			{
				"Rundi",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "rn",
					GoogleCode = "-"
				}
			},
			{
				"Russian",
				new LanguageCodeDef
				{
					PluralRule = 6,
					Code = "ru"
				}
			},
			{
				"Russian/Republic of Moldova",
				new LanguageCodeDef
				{
					PluralRule = 6,
					Code = "ru-MO",
					GoogleCode = "ru"
				}
			},
			{
				"Sanskrit",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "sa",
					GoogleCode = "-"
				}
			},
			{
				"Sardinian",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "sc",
					GoogleCode = "-"
				}
			},
			{
				"Sindhi",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "sd"
				}
			},
			{
				"Northern Sami",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "se",
					GoogleCode = "-"
				}
			},
			{
				"Samoan",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "sm"
				}
			},
			{
				"Sango",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "sg",
					GoogleCode = "-"
				}
			},
			{
				"Serbian",
				new LanguageCodeDef
				{
					PluralRule = 6,
					Code = "sr"
				}
			},
			{
				"Serbian/Bosnia and Herzegovina",
				new LanguageCodeDef
				{
					PluralRule = 5,
					Code = "sr-BA",
					GoogleCode = "sr"
				}
			},
			{
				"Serbian/Serbia and Montenegro",
				new LanguageCodeDef
				{
					PluralRule = 5,
					Code = "sr-SP",
					GoogleCode = "sr"
				}
			},
			{
				"Scottish Gaelic",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "gd"
				}
			},
			{
				"Shona",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "sn"
				}
			},
			{
				"Sinhala",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "si"
				}
			},
			{
				"Slovak",
				new LanguageCodeDef
				{
					PluralRule = 7,
					Code = "sk"
				}
			},
			{
				"Slovenian",
				new LanguageCodeDef
				{
					PluralRule = 9,
					Code = "sl"
				}
			},
			{
				"Somali",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "so"
				}
			},
			{
				"Southern Sotho",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "st"
				}
			},
			{
				"Spanish",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es"
				}
			},
			{
				"Spanish/Argentina",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-AR",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/Bolivia",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-BO",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/Castilian",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-ES",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/Chile",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-CL",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/Colombia",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-CO",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/Costa Rica",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-CR",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/Dominican Republic",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-DO",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/Ecuador",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-EC",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/El Salvador",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-SV",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/Guatemala",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-GT",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/Honduras",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-HN",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/Mexico",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-MX",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/Nicaragua",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-NI",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/Panama",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-PA",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/Paraguay",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-PY",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/Peru",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-PE",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/Puerto Rico",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-PR",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/Spain",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-ES",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/Uruguay",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-UY",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/Venezuela",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-VE",
					GoogleCode = "es"
				}
			},
			{
				"Spanish/Latin Americas",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "es-US",
					GoogleCode = "es"
				}
			},
			{
				"Sundanese",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "su"
				}
			},
			{
				"Swahili",
				new LanguageCodeDef
				{
					Code = "sw"
				}
			},
			{
				"Swati",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ss",
					GoogleCode = "-"
				}
			},
			{
				"Swedish",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "sv"
				}
			},
			{
				"Swedish/Finland",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "sv-FI",
					GoogleCode = "sv"
				}
			},
			{
				"Swedish/Sweden",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "sv-SE",
					GoogleCode = "sv"
				}
			},
			{
				"Tamil",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ta"
				}
			},
			{
				"Tatar",
				new LanguageCodeDef
				{
					PluralRule = 0,
					Code = "tt",
					GoogleCode = "-"
				}
			},
			{
				"Telugu",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "te"
				}
			},
			{
				"Tajik",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "tg"
				}
			},
			{
				"Thai",
				new LanguageCodeDef
				{
					PluralRule = 0,
					Code = "th",
					HasJoinedWords = true
				}
			},
			{
				"Tigrinya",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ti",
					GoogleCode = "-"
				}
			},
			{
				"Tibetan",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "bo",
					GoogleCode = "-"
				}
			},
			{
				"Turkmen",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "tk",
					GoogleCode = "-"
				}
			},
			{
				"Tagalog",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "tl"
				}
			},
			{
				"Tswana",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "tn",
					GoogleCode = "-"
				}
			},
			{
				"Tonga",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "to",
					GoogleCode = "-"
				}
			},
			{
				"Turkish",
				new LanguageCodeDef
				{
					PluralRule = 0,
					Code = "tr"
				}
			},
			{
				"Tsonga",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ts",
					GoogleCode = "-"
				}
			},
			{
				"Twi",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "tw",
					GoogleCode = "-"
				}
			},
			{
				"Tahitian",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ty",
					GoogleCode = "-"
				}
			},
			{
				"Uighur",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ug",
					GoogleCode = "-"
				}
			},
			{
				"Ukrainian",
				new LanguageCodeDef
				{
					PluralRule = 6,
					Code = "uk"
				}
			},
			{
				"Urdu",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ur"
				}
			},
			{
				"Uzbek",
				new LanguageCodeDef
				{
					PluralRule = 2,
					Code = "uz"
				}
			},
			{
				"Venda",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "ve",
					GoogleCode = "-"
				}
			},
			{
				"Vietnamese",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "vi"
				}
			},
			{
				"Volapük",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "vo",
					GoogleCode = "-"
				}
			},
			{
				"Walloon",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "wa",
					GoogleCode = "-"
				}
			},
			{
				"Welsh",
				new LanguageCodeDef
				{
					PluralRule = 16,
					Code = "cy"
				}
			},
			{
				"Wolof",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "wo",
					GoogleCode = "-"
				}
			},
			{
				"Frisian",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "fy"
				}
			},
			{
				"Xhosa",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "xh"
				}
			},
			{
				"Yiddish",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "yi"
				}
			},
			{
				"Yoruba",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "yo"
				}
			},
			{
				"Zhuang",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "za",
					GoogleCode = "-"
				}
			},
			{
				"Zulu",
				new LanguageCodeDef
				{
					PluralRule = 1,
					Code = "zu"
				}
			}
		};

		public static string GetLanguageCode(string Filter, bool ShowWarnings = false)
		{
			if (string.IsNullOrEmpty(Filter))
			{
				return string.Empty;
			}
			string[] filters = Filter.ToLowerInvariant().Split(" /(),".ToCharArray());
			foreach (KeyValuePair<string, LanguageCodeDef> item in mLanguageDef)
			{
				if (LanguageMatchesFilter(item.Key, filters))
				{
					return item.Value.Code;
				}
			}
			if (ShowWarnings)
			{
				Debug.Log((object)$"Language '{Filter}' not recognized. Please, add the language code to GoogleTranslation.cs");
			}
			return string.Empty;
		}

		public static List<string> GetLanguagesForDropdown(string Filter, string CodesToExclude)
		{
			string[] filters = Filter.ToLowerInvariant().Split(" /(),".ToCharArray());
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, LanguageCodeDef> item in mLanguageDef)
			{
				if (string.IsNullOrEmpty(Filter) || LanguageMatchesFilter(item.Key, filters))
				{
					string text = string.Concat("[" + item.Value.Code + "]");
					if (!CodesToExclude.Contains(text))
					{
						list.Add(item.Key + " " + text);
					}
				}
			}
			for (int num = list.Count - 2; num >= 0; num--)
			{
				string text2 = list[num].Substring(0, list[num].IndexOf(" ["));
				if (list[num + 1].StartsWith(text2))
				{
					list[num] = text2 + "/" + list[num];
					list.Insert(num + 1, text2 + "/");
				}
			}
			return list;
		}

		private static bool LanguageMatchesFilter(string Language, string[] Filters)
		{
			Language = Language.ToLowerInvariant();
			int i = 0;
			for (int num = Filters.Length; i < num; i++)
			{
				if (Filters[i] != "")
				{
					if (!Language.Contains(Filters[i].ToLower()))
					{
						return false;
					}
					Language = Language.Remove(Language.IndexOf(Filters[i]), Filters[i].Length);
				}
			}
			return true;
		}

		public static string GetFormatedLanguageName(string Language)
		{
			string empty = string.Empty;
			int num = Language.IndexOf(" [");
			if (num > 0)
			{
				Language = Language.Substring(0, num);
			}
			num = Language.IndexOf('/');
			if (num > 0)
			{
				empty = Language.Substring(0, num);
				if (Language == empty + "/" + empty)
				{
					return empty;
				}
				Language = Language.Replace("/", " (") + ")";
			}
			return Language;
		}

		public static string GetCodedLanguage(string Language, string code)
		{
			string languageCode = GetLanguageCode(Language);
			if (string.Compare(code, languageCode, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return Language;
			}
			return Language + " [" + code + "]";
		}

		public static void UnPackCodeFromLanguageName(string CodedLanguage, out string Language, out string code)
		{
			if (string.IsNullOrEmpty(CodedLanguage))
			{
				Language = string.Empty;
				code = string.Empty;
				return;
			}
			int num = CodedLanguage.IndexOf("[");
			if (num < 0)
			{
				Language = CodedLanguage;
				code = GetLanguageCode(Language);
			}
			else
			{
				Language = CodedLanguage.Substring(0, num).Trim();
				code = CodedLanguage.Substring(num + 1, CodedLanguage.IndexOf("]", num) - num - 1);
			}
		}

		public static string GetGoogleLanguageCode(string InternationalCode)
		{
			foreach (KeyValuePair<string, LanguageCodeDef> item in mLanguageDef)
			{
				if (InternationalCode == item.Value.Code)
				{
					if (item.Value.GoogleCode == "-")
					{
						return null;
					}
					return (!string.IsNullOrEmpty(item.Value.GoogleCode)) ? item.Value.GoogleCode : InternationalCode;
				}
			}
			return InternationalCode;
		}

		public static string GetLanguageName(string code, bool useParenthesesForRegion = false, bool allowDiscardRegion = true)
		{
			foreach (KeyValuePair<string, LanguageCodeDef> item in mLanguageDef)
			{
				if (!(code == item.Value.Code))
				{
					continue;
				}
				string text = item.Key;
				if (useParenthesesForRegion)
				{
					int num = text.IndexOf('/');
					if (num > 0)
					{
						text = text.Substring(0, num) + " (" + text.Substring(num + 1) + ")";
					}
				}
				return text;
			}
			if (allowDiscardRegion)
			{
				int num2 = code.IndexOf("-");
				if (num2 > 0)
				{
					return GetLanguageName(code.Substring(0, num2), useParenthesesForRegion, allowDiscardRegion: false);
				}
			}
			return null;
		}

		public static List<string> GetAllInternationalCodes()
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (KeyValuePair<string, LanguageCodeDef> item in mLanguageDef)
			{
				hashSet.Add(item.Value.Code);
			}
			return new List<string>(hashSet);
		}

		public static bool LanguageCode_HasJoinedWord(string languageCode)
		{
			foreach (KeyValuePair<string, LanguageCodeDef> item in mLanguageDef)
			{
				if (languageCode == item.Value.GoogleCode || languageCode == item.Value.Code)
				{
					return item.Value.HasJoinedWords;
				}
			}
			return false;
		}

		private static int GetPluralRule(string langCode)
		{
			if (langCode.Length > 2)
			{
				langCode = langCode.Substring(0, 2);
			}
			langCode = langCode.ToLower();
			foreach (KeyValuePair<string, LanguageCodeDef> item in mLanguageDef)
			{
				if (item.Value.Code == langCode)
				{
					return item.Value.PluralRule;
				}
			}
			return 0;
		}

		public static bool LanguageHasPluralType(string langCode, string pluralType)
		{
			switch (pluralType)
			{
			case "Plural":
			case "Zero":
			case "One":
				return true;
			default:
				switch (GetPluralRule(langCode))
				{
				case 3:
					if (!(pluralType == "Two"))
					{
						return pluralType == "Few";
					}
					return true;
				case 4:
				case 5:
				case 6:
				case 7:
				case 8:
					return pluralType == "Few";
				case 9:
					if (!(pluralType == "Two"))
					{
						return pluralType == "Few";
					}
					return true;
				case 10:
				case 11:
				case 15:
				case 16:
					if (!(pluralType == "Two") && !(pluralType == "Few"))
					{
						return pluralType == "Many";
					}
					return true;
				case 12:
					if (!(pluralType == "Few"))
					{
						return pluralType == "Many";
					}
					return true;
				case 13:
					return pluralType == "Two";
				default:
					return false;
				}
			}
		}

		public static ePluralType GetPluralType(string langCode, int n)
		{
			switch (n)
			{
			case 0:
				return ePluralType.Zero;
			case 1:
				return ePluralType.One;
			default:
				switch (GetPluralRule(langCode))
				{
				case 0:
					return ePluralType.Plural;
				case 1:
					if (n != 1)
					{
						return ePluralType.Plural;
					}
					return ePluralType.One;
				case 2:
					if (n > 1)
					{
						return ePluralType.Plural;
					}
					return ePluralType.One;
				case 3:
					switch (n)
					{
					default:
						if (!inRange(n, 3, 10) && !inRange(n, 13, 19))
						{
							return ePluralType.Plural;
						}
						return ePluralType.Few;
					case 2:
					case 12:
						return ePluralType.Two;
					case 1:
					case 11:
						return ePluralType.One;
					}
				case 4:
					if (n != 1)
					{
						if (!inRange(n % 100, 1, 19))
						{
							return ePluralType.Plural;
						}
						return ePluralType.Few;
					}
					return ePluralType.One;
				case 5:
					if (n % 10 != 1 || n % 100 == 11)
					{
						if (n % 10 < 2 || (n % 100 >= 10 && n % 100 < 20))
						{
							return ePluralType.Plural;
						}
						return ePluralType.Few;
					}
					return ePluralType.One;
				case 6:
					if (n % 10 != 1 || n % 100 == 11)
					{
						if (!inRange(n % 10, 2, 4) || inRange(n % 100, 12, 14))
						{
							return ePluralType.Plural;
						}
						return ePluralType.Few;
					}
					return ePluralType.One;
				case 7:
					if (n != 1)
					{
						if (!inRange(n, 2, 4))
						{
							return ePluralType.Plural;
						}
						return ePluralType.Few;
					}
					return ePluralType.One;
				case 8:
					if (n != 1)
					{
						if (!inRange(n % 10, 2, 4) || inRange(n % 100, 12, 14))
						{
							return ePluralType.Plural;
						}
						return ePluralType.Few;
					}
					return ePluralType.One;
				case 9:
					if (n % 100 != 1)
					{
						if (n % 100 != 2)
						{
							if (!inRange(n % 100, 3, 4))
							{
								return ePluralType.Plural;
							}
							return ePluralType.Few;
						}
						return ePluralType.Two;
					}
					return ePluralType.One;
				case 10:
					switch (n)
					{
					default:
						if (!inRange(n, 3, 6))
						{
							if (!inRange(n, 7, 10))
							{
								return ePluralType.Plural;
							}
							return ePluralType.Many;
						}
						return ePluralType.Few;
					case 2:
						return ePluralType.Two;
					case 1:
						return ePluralType.One;
					}
				case 11:
					switch (n)
					{
					default:
						if (!inRange(n % 100, 3, 10))
						{
							if (n % 100 < 11)
							{
								return ePluralType.Plural;
							}
							return ePluralType.Many;
						}
						return ePluralType.Few;
					case 2:
						return ePluralType.Two;
					case 1:
						return ePluralType.One;
					case 0:
						return ePluralType.Zero;
					}
				case 12:
					if (n != 1)
					{
						if (!inRange(n % 100, 1, 10))
						{
							if (!inRange(n % 100, 11, 19))
							{
								return ePluralType.Plural;
							}
							return ePluralType.Many;
						}
						return ePluralType.Few;
					}
					return ePluralType.One;
				case 13:
					if (n % 10 != 1)
					{
						if (n % 10 != 2)
						{
							return ePluralType.Plural;
						}
						return ePluralType.Two;
					}
					return ePluralType.One;
				case 14:
					if (n % 10 != 1 || n % 100 == 11)
					{
						return ePluralType.Plural;
					}
					return ePluralType.One;
				case 15:
					if (n % 10 != 1 || n % 100 == 11 || n % 100 == 71 || n % 100 == 91)
					{
						if (n % 10 != 2 || n % 100 == 12 || n % 100 == 72 || n % 100 == 92)
						{
							if ((n % 10 != 3 && n % 10 != 4 && n % 10 != 9) || n % 100 == 13 || n % 100 == 14 || n % 100 == 19 || n % 100 == 73 || n % 100 == 74 || n % 100 == 79 || n % 100 == 93 || n % 100 == 94 || n % 100 == 99)
							{
								if (n % 1000000 != 0)
								{
									return ePluralType.Plural;
								}
								return ePluralType.Many;
							}
							return ePluralType.Few;
						}
						return ePluralType.Two;
					}
					return ePluralType.One;
				case 16:
					switch (n)
					{
						case 6:
							return ePluralType.Many;
						case 3:
							return ePluralType.Few;
						case 2:
							return ePluralType.Two;
						case 1:
							return ePluralType.One;
						case 0:
							return ePluralType.Zero;
						default:
							return ePluralType.Plural;
					}
				default:
					return ePluralType.Plural;
				}
			}
		}

		public static int GetPluralTestNumber(string langCode, ePluralType pluralType)
		{
			switch (pluralType)
			{
			case ePluralType.Zero:
				return 0;
			case ePluralType.One:
				return 1;
			case ePluralType.Few:
				return 3;
			case ePluralType.Many:
				switch (GetPluralRule(langCode))
				{
				case 10:
					return 8;
				case 11:
				case 12:
					return 13;
				case 15:
					return 1000000;
				default:
					return 6;
				}
			default:
				return 936;
			}
		}

		private static bool inRange(int amount, int min, int max)
		{
			if (amount >= min)
			{
				return amount <= max;
			}
			return false;
		}
	}
	public static class GoogleTranslation
	{
		public delegate void fnOnTranslated(string Translation, string Error);

		public delegate void fnOnTranslationReady(Dictionary<string, TranslationQuery> dict, string error);

		private static List<UnityWebRequest> mCurrentTranslations = new List<UnityWebRequest>();

		private static List<TranslationJob> mTranslationJobs = new List<TranslationJob>();

		public static bool CanTranslate()
		{
			if (LocalizationManager.Sources.Count > 0)
			{
				return !string.IsNullOrEmpty(LocalizationManager.GetWebServiceURL());
			}
			return false;
		}

		public static void Translate(string text, string LanguageCodeFrom, string LanguageCodeTo, fnOnTranslated OnTranslationReady)
		{
			LocalizationManager.InitializeIfNeeded();
			if (!CanTranslate())
			{
				OnTranslationReady(null, "WebService is not set correctly or needs to be reinstalled");
				return;
			}
			if (LanguageCodeTo == LanguageCodeFrom)
			{
				OnTranslationReady(text, null);
				return;
			}
			Dictionary<string, TranslationQuery> queries = new Dictionary<string, TranslationQuery>();
			if (string.IsNullOrEmpty(LanguageCodeTo))
			{
				OnTranslationReady(string.Empty, null);
				return;
			}
			CreateQueries(text, LanguageCodeFrom, LanguageCodeTo, queries);
			Translate(queries, (Dictionary<string, TranslationQuery> results, string error) =>
			{
				if (!string.IsNullOrEmpty(error) || results.Count == 0)
				{
					OnTranslationReady(null, error);
				}
				else
				{
					string translation = RebuildTranslation(text, queries, LanguageCodeTo);
					OnTranslationReady(translation, null);
				}
			});
		}

		public static string ForceTranslate(string text, string LanguageCodeFrom, string LanguageCodeTo)
		{
			Dictionary<string, TranslationQuery> dictionary = new Dictionary<string, TranslationQuery>();
			AddQuery(text, LanguageCodeFrom, LanguageCodeTo, dictionary);
			TranslationJob_Main translationJob_Main = new TranslationJob_Main(dictionary, null);
			while (true)
			{
				switch (translationJob_Main.GetState())
				{
				case TranslationJob.eJobState.Running:
					break;
				case TranslationJob.eJobState.Failed:
					return null;
				default:
					return GetQueryResult(text, "", dictionary);
				}
			}
		}

		public static void Translate(Dictionary<string, TranslationQuery> requests, fnOnTranslationReady OnTranslationReady, bool usePOST = true)
		{
			AddTranslationJob(new TranslationJob_Main(requests, OnTranslationReady));
		}

		public static bool ForceTranslate(Dictionary<string, TranslationQuery> requests, bool usePOST = true)
		{
			TranslationJob_Main translationJob_Main = new TranslationJob_Main(requests, null);
			while (true)
			{
				switch (translationJob_Main.GetState())
				{
				case TranslationJob.eJobState.Running:
					break;
				case TranslationJob.eJobState.Failed:
					return false;
				default:
					return true;
				}
			}
		}

		public static List<string> ConvertTranslationRequest(Dictionary<string, TranslationQuery> requests, bool encodeGET)
		{
			List<string> list = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, TranslationQuery> request in requests)
			{
				TranslationQuery value = request.Value;
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("<I2Loc>");
				}
				stringBuilder.Append(GoogleLanguages.GetGoogleLanguageCode(value.LanguageCode));
				stringBuilder.Append(":");
				for (int i = 0; i < value.TargetLanguagesCode.Length; i++)
				{
					if (i != 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(GoogleLanguages.GetGoogleLanguageCode(value.TargetLanguagesCode[i]));
				}
				stringBuilder.Append("=");
				string text = ((TitleCase(value.Text) == value.Text) ? value.Text.ToLowerInvariant() : value.Text);
				if (!encodeGET)
				{
					stringBuilder.Append(text);
					continue;
				}
				stringBuilder.Append(Uri.EscapeDataString(text));
				if (stringBuilder.Length > 4000)
				{
					list.Add(stringBuilder.ToString());
					stringBuilder.Length = 0;
				}
			}
			list.Add(stringBuilder.ToString());
			return list;
		}

		private static void AddTranslationJob(TranslationJob job)
		{
			mTranslationJobs.Add(job);
			if (mTranslationJobs.Count == 1)
			{
				CoroutineManager.Start(WaitForTranslations());
			}
		}

		private static IEnumerator WaitForTranslations()
		{
			while (mTranslationJobs.Count > 0)
			{
				TranslationJob[] array = mTranslationJobs.ToArray();
				foreach (TranslationJob translationJob in array)
				{
					if (translationJob.GetState() != TranslationJob.eJobState.Running)
					{
						mTranslationJobs.Remove(translationJob);
					}
				}
				yield return null;
			}
		}

		public static string ParseTranslationResult(string html, Dictionary<string, TranslationQuery> requests)
		{
			if (html.StartsWith("<!DOCTYPE html>") || html.StartsWith("<HTML>"))
			{
				if (html.Contains("The script completed but did not return anything"))
				{
					return "The current Google WebService is not supported.\nPlease, delete the WebService from the Google Drive and Install the latest version.";
				}
				if (html.Contains("Service invoked too many times in a short time"))
				{
					return "";
				}
				return "There was a problem contacting the WebService. Please try again later\n" + html;
			}
			string[] array = html.Split(new string[1] { "<I2Loc>" }, StringSplitOptions.None);
			string[] separator = new string[1] { "<i2>" };
			int num = 0;
			string[] array2 = requests.Keys.ToArray();
			foreach (string text in array2)
			{
				TranslationQuery value = FindQueryFromOrigText(text, requests);
				string text2 = array[num++];
				if (value.Tags != null)
				{
					for (int num2 = value.Tags.Length - 1; num2 >= 0; num2--)
					{
						text2 = text2.Replace(GetGoogleNoTranslateTag(num2), value.Tags[num2]);
					}
				}
				value.Results = text2.Split(separator, StringSplitOptions.None);
				if (TitleCase(text) == text)
				{
					for (int j = 0; j < value.Results.Length; j++)
					{
						value.Results[j] = TitleCase(value.Results[j]);
					}
				}
				requests[value.OrigText] = value;
			}
			return null;
		}

		public static bool IsTranslating()
		{
			if (mCurrentTranslations.Count <= 0)
			{
				return mTranslationJobs.Count > 0;
			}
			return true;
		}

		public static void CancelCurrentGoogleTranslations()
		{
			mCurrentTranslations.Clear();
			foreach (TranslationJob mTranslationJob in mTranslationJobs)
			{
				mTranslationJob.Dispose();
			}
			mTranslationJobs.Clear();
		}

		public static void CreateQueries(string text, string LanguageCodeFrom, string LanguageCodeTo, Dictionary<string, TranslationQuery> dict)
		{
			if (!text.Contains("[i2s_"))
			{
				CreateQueries_Plurals(text, LanguageCodeFrom, LanguageCodeTo, dict);
				return;
			}
			foreach (KeyValuePair<string, string> specialization in SpecializationManager.GetSpecializations(text))
			{
				CreateQueries_Plurals(specialization.Value, LanguageCodeFrom, LanguageCodeTo, dict);
			}
		}

		private static void CreateQueries_Plurals(string text, string LanguageCodeFrom, string LanguageCodeTo, Dictionary<string, TranslationQuery> dict)
		{
			bool flag = text.Contains("{[#");
			bool flag2 = text.Contains("[i2p_");
			if (!HasParameters(text) || (!flag && !flag2))
			{
				AddQuery(text, LanguageCodeFrom, LanguageCodeTo, dict);
				return;
			}
			bool forceTag = flag;
			for (ePluralType ePluralType2 = ePluralType.Zero; ePluralType2 <= ePluralType.Plural; ePluralType2++)
			{
				string pluralType = ePluralType2.ToString();
				if (GoogleLanguages.LanguageHasPluralType(LanguageCodeTo, pluralType))
				{
					string text2 = GetPluralText(text, pluralType);
					int pluralTestNumber = GoogleLanguages.GetPluralTestNumber(LanguageCodeTo, ePluralType2);
					string pluralParameter = GetPluralParameter(text2, forceTag);
					if (!string.IsNullOrEmpty(pluralParameter))
					{
						text2 = text2.Replace(pluralParameter, pluralTestNumber.ToString());
					}
					AddQuery(text2, LanguageCodeFrom, LanguageCodeTo, dict);
				}
			}
		}

		public static void AddQuery(string text, string LanguageCodeFrom, string LanguageCodeTo, Dictionary<string, TranslationQuery> dict)
		{
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			if (!dict.ContainsKey(text))
			{
				TranslationQuery query = new TranslationQuery
				{
					OrigText = text,
					LanguageCode = LanguageCodeFrom,
					TargetLanguagesCode = new string[1] { LanguageCodeTo }
				};
				query.Text = text;
				ParseNonTranslatableElements(ref query);
				dict[text] = query;
			}
			else
			{
				TranslationQuery value = dict[text];
				if (Array.IndexOf(value.TargetLanguagesCode, LanguageCodeTo) < 0)
				{
					value.TargetLanguagesCode = value.TargetLanguagesCode.Concat(new string[1] { LanguageCodeTo }).Distinct().ToArray();
				}
				dict[text] = value;
			}
		}

		private static string GetTranslation(string text, string LanguageCodeTo, Dictionary<string, TranslationQuery> dict)
		{
			if (!dict.ContainsKey(text))
			{
				return null;
			}
			TranslationQuery translationQuery = dict[text];
			int num = Array.IndexOf(translationQuery.TargetLanguagesCode, LanguageCodeTo);
			if (num < 0)
			{
				return "";
			}
			if (translationQuery.Results == null)
			{
				return "";
			}
			return translationQuery.Results[num];
		}

		private static TranslationQuery FindQueryFromOrigText(string origText, Dictionary<string, TranslationQuery> dict)
		{
			foreach (KeyValuePair<string, TranslationQuery> item in dict)
			{
				if (item.Value.OrigText == origText)
				{
					return item.Value;
				}
			}
			return default;
		}

		public static bool HasParameters(string text)
		{
			int num = text.IndexOf("{[");
			if (num < 0)
			{
				return false;
			}
			return text.IndexOf("]}", num) > 0;
		}

		public static string GetPluralParameter(string text, bool forceTag)
		{
			int num = text.IndexOf("{[#");
			if (num < 0)
			{
				if (forceTag)
				{
					return null;
				}
				num = text.IndexOf("{[");
			}
			if (num < 0)
			{
				return null;
			}
			int num2 = text.IndexOf("]}", num + 2);
			if (num2 < 0)
			{
				return null;
			}
			return text.Substring(num, num2 - num + 2);
		}

		public static string GetPluralText(string text, string pluralType)
		{
			pluralType = "[i2p_" + pluralType + "]";
			int num = text.IndexOf(pluralType);
			if (num >= 0)
			{
				num += pluralType.Length;
				int num2 = text.IndexOf("[i2p_", num);
				if (num2 < 0)
				{
					num2 = text.Length;
				}
				return text.Substring(num, num2 - num);
			}
			num = text.IndexOf("[i2p_");
			if (num < 0)
			{
				return text;
			}
			if (num > 0)
			{
				return text.Substring(0, num);
			}
			num = text.IndexOf("]");
			if (num < 0)
			{
				return text;
			}
			num++;
			int num3 = text.IndexOf("[i2p_", num);
			if (num3 < 0)
			{
				num3 = text.Length;
			}
			return text.Substring(num, num3 - num);
		}

		private static int FindClosingTag(string tag, MatchCollection matches, int startIndex)
		{
			int i = startIndex;
			for (int count = matches.Count; i < count; i++)
			{
				string captureMatch = I2Utils.GetCaptureMatch(matches[i]);
				if (captureMatch[0] == '/' && tag.StartsWith(captureMatch.Substring(1)))
				{
					return i;
				}
			}
			return -1;
		}

		private static string GetGoogleNoTranslateTag(int tagNumber)
		{
			if (tagNumber < 70)
			{
				return "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++".Substring(0, tagNumber + 1);
			}
			string text = "";
			for (int i = -1; i < tagNumber; i++)
			{
				text += "+";
			}
			return text;
		}

		private static void ParseNonTranslatableElements(ref TranslationQuery query)
		{
			MatchCollection matchCollection = Regex.Matches(query.Text, "\\{\\[(.*?)]}|\\[(.*?)]|\\<(.*?)>");
			if (matchCollection == null || matchCollection.Count == 0)
			{
				return;
			}
			string text = query.Text;
			List<string> list = new List<string>();
			int i = 0;
			for (int count = matchCollection.Count; i < count; i++)
			{
				string captureMatch = I2Utils.GetCaptureMatch(matchCollection[i]);
				int num = FindClosingTag(captureMatch, matchCollection, i);
				if (num < 0)
				{
					string text2 = matchCollection[i].ToString();
					if (text2.StartsWith("{[") && text2.EndsWith("]}"))
					{
						text = text.Replace(text2, GetGoogleNoTranslateTag(list.Count) + " ");
						list.Add(text2);
					}
				}
				else if (captureMatch == "i2nt")
				{
					string text3 = query.Text.Substring(matchCollection[i].Index, matchCollection[num].Index - matchCollection[i].Index + matchCollection[num].Length);
					text = text.Replace(text3, GetGoogleNoTranslateTag(list.Count) + " ");
					list.Add(text3);
				}
				else
				{
					string text4 = matchCollection[i].ToString();
					text = text.Replace(text4, GetGoogleNoTranslateTag(list.Count) + " ");
					list.Add(text4);
					string text5 = matchCollection[num].ToString();
					text = text.Replace(text5, GetGoogleNoTranslateTag(list.Count) + " ");
					list.Add(text5);
				}
			}
			query.Text = text;
			query.Tags = list.ToArray();
		}

		public static string GetQueryResult(string text, string LanguageCodeTo, Dictionary<string, TranslationQuery> dict)
		{
			if (!dict.ContainsKey(text))
			{
				return null;
			}
			TranslationQuery translationQuery = dict[text];
			if (translationQuery.Results == null || translationQuery.Results.Length < 0)
			{
				return null;
			}
			if (string.IsNullOrEmpty(LanguageCodeTo))
			{
				return translationQuery.Results[0];
			}
			int num = Array.IndexOf(translationQuery.TargetLanguagesCode, LanguageCodeTo);
			if (num < 0)
			{
				return null;
			}
			return translationQuery.Results[num];
		}

		public static string RebuildTranslation(string text, Dictionary<string, TranslationQuery> dict, string LanguageCodeTo)
		{
			if (!text.Contains("[i2s_"))
			{
				return RebuildTranslation_Plural(text, dict, LanguageCodeTo);
			}
			Dictionary<string, string> specializations = SpecializationManager.GetSpecializations(text);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> item in specializations)
			{
				dictionary[item.Key] = RebuildTranslation_Plural(item.Value, dict, LanguageCodeTo);
			}
			return SpecializationManager.SetSpecializedText(dictionary);
		}

		private static string RebuildTranslation_Plural(string text, Dictionary<string, TranslationQuery> dict, string LanguageCodeTo)
		{
			bool flag = text.Contains("{[#");
			bool flag2 = text.Contains("[i2p_");
			if (!HasParameters(text) || (!flag && !flag2))
			{
				return GetTranslation(text, LanguageCodeTo, dict);
			}
			StringBuilder stringBuilder = new StringBuilder();
			string text2 = null;
			bool forceTag = flag;
			for (ePluralType ePluralType2 = ePluralType.Plural; ePluralType2 >= ePluralType.Zero; ePluralType2--)
			{
				string text3 = ePluralType2.ToString();
				if (!GoogleLanguages.LanguageHasPluralType(LanguageCodeTo, text3))
				{
					continue;
				}
				string text4 = GetPluralText(text, text3);
				int pluralTestNumber = GoogleLanguages.GetPluralTestNumber(LanguageCodeTo, ePluralType2);
				string pluralParameter = GetPluralParameter(text4, forceTag);
				if (!string.IsNullOrEmpty(pluralParameter))
				{
					text4 = text4.Replace(pluralParameter, pluralTestNumber.ToString());
				}
				string text5 = GetTranslation(text4, LanguageCodeTo, dict);
				if (!string.IsNullOrEmpty(pluralParameter))
				{
					text5 = text5.Replace(pluralTestNumber.ToString(), pluralParameter);
				}
				if (ePluralType2 == ePluralType.Plural)
				{
					text2 = text5;
				}
				else
				{
					if (text5 == text2)
					{
						continue;
					}
					stringBuilder.AppendFormat("[i2p_{0}]", text3);
				}
				stringBuilder.Append(text5);
			}
			return stringBuilder.ToString();
		}

		public static string UppercaseFirst(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			char[] array = s.ToLower().ToCharArray();
			array[0] = char.ToUpper(array[0]);
			return new string(array);
		}

		public static string TitleCase(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s);
		}
	}
	public struct TranslationQuery
	{
		public string OrigText;

		public string Text;

		public string LanguageCode;

		public string[] TargetLanguagesCode;

		public string[] Results;

		public string[] Tags;
	}
	public class TranslationJob : IDisposable
	{
		public enum eJobState
		{
			Running,
			Succeeded,
			Failed
		}

		public eJobState mJobState;

		public virtual eJobState GetState()
		{
			return mJobState;
		}

		public virtual void Dispose()
		{
		}
	}
	public class TranslationJob_WWW : TranslationJob
	{
		public UnityWebRequest www;

		public override void Dispose()
		{
			if (www != null)
			{
				www.Dispose();
			}
			www = null;
		}
	}
	public class TranslationJob_GET : TranslationJob_WWW
	{
		private Dictionary<string, TranslationQuery> _requests;

		private GoogleTranslation.fnOnTranslationReady _OnTranslationReady;

		private List<string> mQueries;

		public string mErrorMessage;

		public TranslationJob_GET(Dictionary<string, TranslationQuery> requests, GoogleTranslation.fnOnTranslationReady OnTranslationReady)
		{
			_requests = requests;
			_OnTranslationReady = OnTranslationReady;
			mQueries = GoogleTranslation.ConvertTranslationRequest(requests, encodeGET: true);
			GetState();
		}

		private void ExecuteNextQuery()
		{
			if (mQueries.Count == 0)
			{
				mJobState = eJobState.Succeeded;
				return;
			}
			int index = mQueries.Count - 1;
			string arg = mQueries[index];
			mQueries.RemoveAt(index);
			string text = $"{LocalizationManager.GetWebServiceURL()}?action=Translate&list={arg}";
			www = UnityWebRequest.Get(text);
			I2Utils.SendWebRequest(www);
		}

		public override eJobState GetState()
		{
			if (www != null && www.isDone)
			{
				ProcessResult(www.downloadHandler.data, www.error);
				www.Dispose();
				www = null;
			}
			if (www == null)
			{
				ExecuteNextQuery();
			}
			return mJobState;
		}

		public void ProcessResult(byte[] bytes, string errorMsg)
		{
			if (string.IsNullOrEmpty(errorMsg))
			{
				errorMsg = GoogleTranslation.ParseTranslationResult(Encoding.UTF8.GetString(bytes, 0, bytes.Length), _requests);
				if (string.IsNullOrEmpty(errorMsg))
				{
					if (_OnTranslationReady != null)
					{
						_OnTranslationReady(_requests, null);
					}
					return;
				}
			}
			mJobState = eJobState.Failed;
			mErrorMessage = errorMsg;
		}
	}
	public class TranslationJob_Main : TranslationJob
	{
		private TranslationJob_WEB mWeb;

		private TranslationJob_POST mPost;

		private TranslationJob_GET mGet;

		private Dictionary<string, TranslationQuery> _requests;

		private GoogleTranslation.fnOnTranslationReady _OnTranslationReady;

		public string mErrorMessage;

		public TranslationJob_Main(Dictionary<string, TranslationQuery> requests, GoogleTranslation.fnOnTranslationReady OnTranslationReady)
		{
			_requests = requests;
			_OnTranslationReady = OnTranslationReady;
			mPost = new TranslationJob_POST(requests, OnTranslationReady);
		}

		public override eJobState GetState()
		{
			if (mWeb != null)
			{
				switch (mWeb.GetState())
				{
				case eJobState.Running:
					return eJobState.Running;
				case eJobState.Succeeded:
					mJobState = eJobState.Succeeded;
					break;
				case eJobState.Failed:
					mWeb.Dispose();
					mWeb = null;
					mPost = new TranslationJob_POST(_requests, _OnTranslationReady);
					break;
				}
			}
			if (mPost != null)
			{
				switch (mPost.GetState())
				{
				case eJobState.Running:
					return eJobState.Running;
				case eJobState.Succeeded:
					mJobState = eJobState.Succeeded;
					break;
				case eJobState.Failed:
					mPost.Dispose();
					mPost = null;
					mGet = new TranslationJob_GET(_requests, _OnTranslationReady);
					break;
				}
			}
			if (mGet != null)
			{
				switch (mGet.GetState())
				{
				case eJobState.Running:
					return eJobState.Running;
				case eJobState.Succeeded:
					mJobState = eJobState.Succeeded;
					break;
				case eJobState.Failed:
					mErrorMessage = mGet.mErrorMessage;
					if (_OnTranslationReady != null)
					{
						_OnTranslationReady(_requests, mErrorMessage);
					}
					mGet.Dispose();
					mGet = null;
					break;
				}
			}
			return mJobState;
		}

		public override void Dispose()
		{
			if (mPost != null)
			{
				mPost.Dispose();
			}
			if (mGet != null)
			{
				mGet.Dispose();
			}
			mPost = null;
			mGet = null;
		}
	}
	public class TranslationJob_POST : TranslationJob_WWW
	{
		private Dictionary<string, TranslationQuery> _requests;

		private GoogleTranslation.fnOnTranslationReady _OnTranslationReady;

		public TranslationJob_POST(Dictionary<string, TranslationQuery> requests, GoogleTranslation.fnOnTranslationReady OnTranslationReady)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected Obj, but got Unknown
			_requests = requests;
			_OnTranslationReady = OnTranslationReady;
			List<string> list = GoogleTranslation.ConvertTranslationRequest(requests, encodeGET: false);
			WWWForm val = new WWWForm();
			val.AddField("action", "Translate");
			val.AddField("list", list[0]);
			www = UnityWebRequest.Post(LocalizationManager.GetWebServiceURL(), val);
			I2Utils.SendWebRequest(www);
		}

		public override eJobState GetState()
		{
			if (www != null && www.isDone)
			{
				ProcessResult(www.downloadHandler.data, www.error);
				www.Dispose();
				www = null;
			}
			return mJobState;
		}

		public void ProcessResult(byte[] bytes, string errorMsg)
		{
			if (!string.IsNullOrEmpty(errorMsg))
			{
				mJobState = eJobState.Failed;
				return;
			}
			errorMsg = GoogleTranslation.ParseTranslationResult(Encoding.UTF8.GetString(bytes, 0, bytes.Length), _requests);
			if (_OnTranslationReady != null)
			{
				_OnTranslationReady(_requests, errorMsg);
			}
			mJobState = eJobState.Succeeded;
		}
	}
	public class TranslationJob_WEB : TranslationJob_WWW
	{
		private Dictionary<string, TranslationQuery> _requests;

		private GoogleTranslation.fnOnTranslationReady _OnTranslationReady;

		public string mErrorMessage;

		private string mCurrentBatch_ToLanguageCode;

		private string mCurrentBatch_FromLanguageCode;

		private List<string> mCurrentBatch_Text;

		private List<KeyValuePair<string, string>> mQueries;

		public TranslationJob_WEB(Dictionary<string, TranslationQuery> requests, GoogleTranslation.fnOnTranslationReady OnTranslationReady)
		{
			_requests = requests;
			_OnTranslationReady = OnTranslationReady;
			FindAllQueries();
			ExecuteNextBatch();
		}

		private void FindAllQueries()
		{
			mQueries = new List<KeyValuePair<string, string>>();
			foreach (KeyValuePair<string, TranslationQuery> request in _requests)
			{
				string[] targetLanguagesCode = request.Value.TargetLanguagesCode;
				foreach (string text in targetLanguagesCode)
				{
					mQueries.Add(new KeyValuePair<string, string>(request.Value.OrigText, request.Value.LanguageCode + ":" + text));
				}
			}
			mQueries.Sort((KeyValuePair<string, string> a, KeyValuePair<string, string> b) => a.Value.CompareTo(b.Value));
		}

		private void ExecuteNextBatch()
		{
			if (mQueries.Count == 0)
			{
				mJobState = eJobState.Succeeded;
				return;
			}
			mCurrentBatch_Text = new List<string>();
			string text = null;
			int num = 200;
			StringBuilder stringBuilder = new StringBuilder();
			int i;
			for (i = 0; i < mQueries.Count; i++)
			{
				string key = mQueries[i].Key;
				string value = mQueries[i].Value;
				if (text == null || value == text)
				{
					if (i != 0)
					{
						stringBuilder.Append("|||");
					}
					stringBuilder.Append(key);
					mCurrentBatch_Text.Add(key);
					text = value;
				}
				if (stringBuilder.Length > num)
				{
					break;
				}
			}
			mQueries.RemoveRange(0, i);
			string[] array = text.Split(new char[1] { ':' });
			mCurrentBatch_FromLanguageCode = array[0];
			mCurrentBatch_ToLanguageCode = array[1];
			string text2 = $"http://www.google.com/translate_t?hl=en&vi=c&ie=UTF8&oe=UTF8&submit=Translate&langpair={mCurrentBatch_FromLanguageCode}|{mCurrentBatch_ToLanguageCode}&text={Uri.EscapeUriString(stringBuilder.ToString())}";
			Debug.Log((object)text2);
			www = UnityWebRequest.Get(text2);
			I2Utils.SendWebRequest(www);
		}

		public override eJobState GetState()
		{
			if (www != null && www.isDone)
			{
				ProcessResult(www.downloadHandler.data, www.error);
				www.Dispose();
				www = null;
			}
			if (www == null)
			{
				ExecuteNextBatch();
			}
			return mJobState;
		}

		public void ProcessResult(byte[] bytes, string errorMsg)
		{
			if (string.IsNullOrEmpty(errorMsg))
			{
				string html = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
				Debug.Log((object)ParseTranslationResult(html, "aab"));
				if (string.IsNullOrEmpty(errorMsg))
				{
					if (_OnTranslationReady != null)
					{
						_OnTranslationReady(_requests, null);
					}
					return;
				}
			}
			mJobState = eJobState.Failed;
			mErrorMessage = errorMsg;
		}

		private string ParseTranslationResult(string html, string OriginalText)
		{
			try
			{
				int num = html.IndexOf("TRANSLATED_TEXT='") + "TRANSLATED_TEXT='".Length;
				int num2 = html.IndexOf("';var", num);
				string input = html.Substring(num, num2 - num);
				input = Regex.Replace(input, "\\\\x([a-fA-F0-9]{2})", (Match match) => char.ConvertFromUtf32(int.Parse(match.Groups[1].Value, NumberStyles.HexNumber)));
				input = Regex.Replace(input, "&#(\\d+);", (Match match) => char.ConvertFromUtf32(int.Parse(match.Groups[1].Value)));
				input = input.Replace("<br>", "\n");
				if (OriginalText.ToUpper() == OriginalText)
				{
					input = input.ToUpper();
				}
				else if (GoogleTranslation.UppercaseFirst(OriginalText) == OriginalText)
				{
					input = GoogleTranslation.UppercaseFirst(input);
				}
				else if (GoogleTranslation.TitleCase(OriginalText) == OriginalText)
				{
					input = GoogleTranslation.TitleCase(input);
				}
				return input;
			}
			catch (Exception ex)
			{
				Debug.LogError((object)ex.Message);
				return string.Empty;
			}
		}
	}
	public enum eLanguageDataFlags
	{
		DISABLED = 1,
		KEEP_LOADED = 2,
		NOT_LOADED = 4
	}
	[Serializable]
	public class LanguageData
	{
		public string Name;

		public string Code;

		public byte Flags;

		[NonSerialized]
		public bool Compressed;

		public bool IsEnabled()
		{
			return (Flags & 1) == 0;
		}

		public void SetEnabled(bool bEnabled)
		{
			if (bEnabled)
			{
				Flags = (byte)(Flags & -2);
			}
			else
			{
				Flags |= 1;
			}
		}

		public bool IsLoaded()
		{
			return (Flags & 4) == 0;
		}

		public bool CanBeUnloaded()
		{
			return (Flags & 2) == 0;
		}

		public void SetLoaded(bool loaded)
		{
			if (loaded)
			{
				Flags = (byte)(Flags & -5);
			}
			else
			{
				Flags |= 4;
			}
		}

		public void SetCanBeUnLoaded(bool allowUnloading)
		{
			if (allowUnloading)
			{
				Flags = (byte)(Flags & -3);
			}
			else
			{
				Flags |= 2;
			}
		}
	}
	[AddComponentMenu("I2/Localization/Source")]
	[ExecuteInEditMode]
	public class LanguageSource : MonoBehaviour, ISerializationCallbackReceiver, ILanguageSource
	{
		public delegate void fnOnSourceUpdated(LanguageSourceData source, bool ReceivedNewData, string errorMsg);

		public LanguageSourceData mSource = new LanguageSourceData();

		public int version;

		public bool NeverDestroy;

		public bool UserAgreesToHaveItOnTheScene;

		public bool UserAgreesToHaveItInsideThePluginsFolder;

		public bool GoogleLiveSyncIsUptoDate = true;

		public List<Object> Assets = new List<Object>();

		public string Google_WebServiceURL;

		public string Google_SpreadsheetKey;

		public string Google_SpreadsheetName;

		public string Google_LastUpdatedVersion;

		public LanguageSourceData.eGoogleUpdateFrequency GoogleUpdateFrequency = LanguageSourceData.eGoogleUpdateFrequency.Weekly;

		public float GoogleUpdateDelay = 5f;

		public List<LanguageData> mLanguages = new List<LanguageData>();

		public bool IgnoreDeviceLanguage;

		public LanguageSourceData.eAllowUnloadLanguages _AllowUnloadingLanguages;

		public List<TermData> mTerms = new List<TermData>();

		public bool CaseInsensitiveTerms;

		public LanguageSourceData.MissingTranslationAction OnMissingTranslation = LanguageSourceData.MissingTranslationAction.Fallback;

		public string mTerm_AppName;

		public LanguageSourceData SourceData
		{
			get
			{
				return mSource;
			}
			set
			{
				mSource = value;
			}
		}

		public event fnOnSourceUpdated Event_OnSourceUpdateFromGoogle;

		private void Awake()
		{
			mSource.owner = this;
			mSource.Awake();
		}

		private void OnDestroy()
		{
			NeverDestroy = false;
			if (!NeverDestroy)
			{
				mSource.OnDestroy();
			}
		}

		public string GetSourceName()
		{
			string text = ((Object)((Component)this).gameObject).name;
			Transform parent = ((Component)this).transform.parent;
			while (Object.op_Implicit((Object)(object)parent))
			{
				text = ((Object)parent).name + "_" + text;
				parent = parent.parent;
			}
			return text;
		}

		public void OnBeforeSerialize()
		{
			version = 1;
		}

		public void OnAfterDeserialize()
		{
			if (version != 0 && mSource != null)
			{
				return;
			}
			mSource = new LanguageSourceData();
			mSource.owner = this;
			mSource.UserAgreesToHaveItOnTheScene = UserAgreesToHaveItOnTheScene;
			mSource.UserAgreesToHaveItInsideThePluginsFolder = UserAgreesToHaveItInsideThePluginsFolder;
			mSource.IgnoreDeviceLanguage = IgnoreDeviceLanguage;
			mSource._AllowUnloadingLanguages = _AllowUnloadingLanguages;
			mSource.CaseInsensitiveTerms = CaseInsensitiveTerms;
			mSource.OnMissingTranslation = OnMissingTranslation;
			mSource.mTerm_AppName = mTerm_AppName;
			mSource.GoogleLiveSyncIsUptoDate = GoogleLiveSyncIsUptoDate;
			mSource.Google_WebServiceURL = Google_WebServiceURL;
			mSource.Google_SpreadsheetKey = Google_SpreadsheetKey;
			mSource.Google_SpreadsheetName = Google_SpreadsheetName;
			mSource.Google_LastUpdatedVersion = Google_LastUpdatedVersion;
			mSource.GoogleUpdateFrequency = GoogleUpdateFrequency;
			mSource.GoogleUpdateDelay = GoogleUpdateDelay;
			mSource.Event_OnSourceUpdateFromGoogle += Event_OnSourceUpdateFromGoogle;
			if (mLanguages != null && mLanguages.Count > 0)
			{
				mSource.mLanguages.Clear();
				mSource.mLanguages.AddRange(mLanguages);
				mLanguages.Clear();
			}
			if (Assets != null && Assets.Count > 0)
			{
				mSource.Assets.Clear();
				mSource.Assets.AddRange(Assets);
				Assets.Clear();
			}
			if (mTerms != null && mTerms.Count > 0)
			{
				mSource.mTerms.Clear();
				for (int i = 0; i < mTerms.Count; i++)
				{
					mSource.mTerms.Add(mTerms[i]);
				}
				mTerms.Clear();
			}
			version = 1;
			Event_OnSourceUpdateFromGoogle = null;
		}
	}
	[CreateAssetMenu(fileName = "I2Languages", menuName = "I2 Localization/LanguageSource", order = 1)]
	public class LanguageSourceAsset : ScriptableObject, ILanguageSource
	{
		public LanguageSourceData mSource = new LanguageSourceData();

		public LanguageSourceData SourceData
		{
			get
			{
				return mSource;
			}
			set
			{
				mSource = value;
			}
		}
	}
	public interface ILanguageSource
	{
		LanguageSourceData SourceData { get; set; }
	}
	[Serializable]
	[ExecuteInEditMode]
	public class LanguageSourceData
	{
		public enum MissingTranslationAction
		{
			Empty,
			Fallback,
			ShowWarning,
			ShowTerm
		}

		public enum eAllowUnloadLanguages
		{
			Never,
			OnlyInDevice,
			EditorAndDevice
		}

		public enum eGoogleUpdateFrequency
		{
			Always,
			Never,
			Daily,
			Weekly,
			Monthly,
			OnlyOnce,
			EveryOtherDay
		}

		public enum eGoogleUpdateSynchronization
		{
			Manual,
			OnSceneLoaded,
			AsSoonAsDownloaded
		}

		[NonSerialized]
		public ILanguageSource owner;

		public bool UserAgreesToHaveItOnTheScene;

		public bool UserAgreesToHaveItInsideThePluginsFolder;

		public bool GoogleLiveSyncIsUptoDate = true;

		[NonSerialized]
		public bool mIsGlobalSource;

		public List<TermData> mTerms = new List<TermData>();

		public bool CaseInsensitiveTerms;

		[NonSerialized]
		public Dictionary<string, TermData> mDictionary = new Dictionary<string, TermData>(StringComparer.Ordinal);

		public MissingTranslationAction OnMissingTranslation = MissingTranslationAction.Fallback;

		public string mTerm_AppName;

		public List<LanguageData> mLanguages = new List<LanguageData>();

		public bool IgnoreDeviceLanguage;

		public eAllowUnloadLanguages _AllowUnloadingLanguages;

		public string Google_WebServiceURL;

		public string Google_SpreadsheetKey;

		public string Google_SpreadsheetName;

		public string Google_LastUpdatedVersion;

		public eGoogleUpdateFrequency GoogleUpdateFrequency = eGoogleUpdateFrequency.Weekly;

		public eGoogleUpdateFrequency GoogleInEditorCheckFrequency = eGoogleUpdateFrequency.Daily;

		public eGoogleUpdateSynchronization GoogleUpdateSynchronization = eGoogleUpdateSynchronization.OnSceneLoaded;

		public float GoogleUpdateDelay;

		public List<Object> Assets = new List<Object>();

		[NonSerialized]
		public Dictionary<string, Object> mAssetDictionary = new Dictionary<string, Object>(StringComparer.Ordinal);

		private string mDelayedGoogleData;

		public static string EmptyCategory = "Default";

		public static char[] CategorySeparators = "/\\".ToCharArray();

		public Object ownerObject
		{
			get
			{
				ILanguageSource languageSource = owner;
				return (Object)((languageSource is Object) ? languageSource : null);
			}
		}

		public event LanguageSource.fnOnSourceUpdated Event_OnSourceUpdateFromGoogle;

		public void Awake()
		{
			LocalizationManager.AddSource(this);
			UpdateDictionary();
			UpdateAssetDictionary();
			LocalizationManager.LocalizeAll(Force: true);
		}

		public void OnDestroy()
		{
			LocalizationManager.RemoveSource(this);
		}

		public bool IsEqualTo(LanguageSourceData Source)
		{
			if (Source.mLanguages.Count != mLanguages.Count)
			{
				return false;
			}
			int i = 0;
			for (int count = mLanguages.Count; i < count; i++)
			{
				if (Source.GetLanguageIndex(mLanguages[i].Name) < 0)
				{
					return false;
				}
			}
			if (Source.mTerms.Count != mTerms.Count)
			{
				return false;
			}
			for (int j = 0; j < mTerms.Count; j++)
			{
				if (Source.GetTermData(mTerms[j].Term) == null)
				{
					return false;
				}
			}
			return true;
		}

		internal bool ManagerHasASimilarSource()
		{
			int i = 0;
			for (int count = LocalizationManager.Sources.Count; i < count; i++)
			{
				LanguageSourceData languageSourceData = LocalizationManager.Sources[i];
				if (languageSourceData != null && languageSourceData.IsEqualTo(this) && languageSourceData != this)
				{
					return true;
				}
			}
			return false;
		}

		public void ClearAllData()
		{
			mTerms.Clear();
			mLanguages.Clear();
			mDictionary.Clear();
			mAssetDictionary.Clear();
		}

		public bool IsGlobalSource()
		{
			return mIsGlobalSource;
		}

		public void Editor_SetDirty()
		{
		}

		public void UpdateAssetDictionary()
		{
			Assets.RemoveAll((Object x) => x == (Object)null);
			mAssetDictionary = (from o in Assets.Distinct()
				group o by o.name).ToDictionary((IGrouping<string, Object> g) => g.Key, (IGrouping<string, Object> g) => g.First());
		}

		public Object FindAsset(string Name)
		{
			if (Assets != null)
			{
				if (mAssetDictionary == null || mAssetDictionary.Count != Assets.Count)
				{
					UpdateAssetDictionary();
				}
				if (mAssetDictionary.TryGetValue(Name, out var value))
				{
					return value;
				}
			}
			return null;
		}

		public bool HasAsset(Object Obj)
		{
			return Assets.Contains(Obj);
		}

		public void AddAsset(Object Obj)
		{
			if (!Assets.Contains(Obj))
			{
				Assets.Add(Obj);
				UpdateAssetDictionary();
			}
		}

		public string Export_I2CSV(string Category, char Separator = ',', bool specializationsAsRows = true)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Key[*]Type[*]Desc");
			foreach (LanguageData mLanguage in mLanguages)
			{
				stringBuilder.Append("[*]");
				if (!mLanguage.IsEnabled())
				{
					stringBuilder.Append('$');
				}
				stringBuilder.Append(GoogleLanguages.GetCodedLanguage(mLanguage.Name, mLanguage.Code));
			}
			stringBuilder.Append("[ln]");
			mTerms.Sort((TermData a, TermData b) => string.CompareOrdinal(a.Term, b.Term));
			int count = mLanguages.Count;
			bool flag = true;
			foreach (TermData mTerm in mTerms)
			{
				string term;
				if (string.IsNullOrEmpty(Category) || (Category == EmptyCategory && mTerm.Term.IndexOfAny(CategorySeparators) < 0))
				{
					term = mTerm.Term;
				}
				else
				{
					if (!mTerm.Term.StartsWith(Category + "/") || !(Category != mTerm.Term))
					{
						continue;
					}
					term = mTerm.Term.Substring(Category.Length + 1);
				}
				if (!flag)
				{
					stringBuilder.Append("[ln]");
				}
				flag = false;
				if (!specializationsAsRows)
				{
					AppendI2Term(stringBuilder, count, term, mTerm, Separator, null);
					continue;
				}
				List<string> allSpecializations = mTerm.GetAllSpecializations();
				for (int num = 0; num < allSpecializations.Count; num++)
				{
					if (num != 0)
					{
						stringBuilder.Append("[ln]");
					}
					string forceSpecialization = allSpecializations[num];
					AppendI2Term(stringBuilder, count, term, mTerm, Separator, forceSpecialization);
				}
			}
			return stringBuilder.ToString();
		}

		private static void AppendI2Term(StringBuilder Builder, int nLanguages, string Term, TermData termData, char Separator, string forceSpecialization)
		{
			AppendI2Text(Builder, Term);
			if (!string.IsNullOrEmpty(forceSpecialization) && forceSpecialization != "Any")
			{
				Builder.Append("[");
				Builder.Append(forceSpecialization);
				Builder.Append("]");
			}
			Builder.Append("[*]");
			Builder.Append(termData.TermType.ToString());
			Builder.Append("[*]");
			Builder.Append(termData.Description);
			for (int i = 0; i < Mathf.Min(nLanguages, termData.Languages.Length); i++)
			{
				Builder.Append("[*]");
				string text = termData.Languages[i];
				if (!string.IsNullOrEmpty(forceSpecialization))
				{
					text = termData.GetTranslation(i, forceSpecialization);
				}
				AppendI2Text(Builder, text);
			}
		}

		private static void AppendI2Text(StringBuilder Builder, string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				if (text.StartsWith("'") || text.StartsWith("="))
				{
					Builder.Append('\'');
				}
				Builder.Append(text);
			}
		}

		private string Export_Language_to_Cache(int langIndex, bool fillTermWithFallback)
		{
			if (!mLanguages[langIndex].IsLoaded())
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < mTerms.Count; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append("[i2t]");
				}
				TermData termData = mTerms[i];
				stringBuilder.Append(termData.Term);
				stringBuilder.Append("=");
				string Translation = termData.Languages[langIndex];
				if (OnMissingTranslation == MissingTranslationAction.Fallback && string.IsNullOrEmpty(Translation) && TryGetFallbackTranslation(termData, out Translation, langIndex, null, skipDisabled: true))
				{
					stringBuilder.Append("[i2fb]");
					if (fillTermWithFallback)
					{
						termData.Languages[langIndex] = Translation;
					}
				}
				if (!string.IsNullOrEmpty(Translation))
				{
					stringBuilder.Append(Translation);
				}
			}
			return stringBuilder.ToString();
		}

		public string Export_CSV(string Category, char Separator = ',', bool specializationsAsRows = true)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int count = mLanguages.Count;
			stringBuilder.AppendFormat("Key{0}Type{0}Desc", Separator);
			foreach (LanguageData mLanguage in mLanguages)
			{
				stringBuilder.Append(Separator);
				if (!mLanguage.IsEnabled())
				{
					stringBuilder.Append('$');
				}
				AppendString(stringBuilder, GoogleLanguages.GetCodedLanguage(mLanguage.Name, mLanguage.Code), Separator);
			}
			stringBuilder.Append("\n");
			mTerms.Sort((TermData a, TermData b) => string.CompareOrdinal(a.Term, b.Term));
			foreach (TermData mTerm in mTerms)
			{
				string term;
				if (string.IsNullOrEmpty(Category) || (Category == EmptyCategory && mTerm.Term.IndexOfAny(CategorySeparators) < 0))
				{
					term = mTerm.Term;
				}
				else
				{
					if (!mTerm.Term.StartsWith(Category + "/") || !(Category != mTerm.Term))
					{
						continue;
					}
					term = mTerm.Term.Substring(Category.Length + 1);
				}
				if (specializationsAsRows)
				{
					foreach (string allSpecialization in mTerm.GetAllSpecializations())
					{
						AppendTerm(stringBuilder, count, term, mTerm, allSpecialization, Separator);
					}
				}
				else
				{
					AppendTerm(stringBuilder, count, term, mTerm, null, Separator);
				}
			}
			return stringBuilder.ToString();
		}

		private static void AppendTerm(StringBuilder Builder, int nLanguages, string Term, TermData termData, string specialization, char Separator)
		{
			AppendString(Builder, Term, Separator);
			if (!string.IsNullOrEmpty(specialization) && specialization != "Any")
			{
				Builder.AppendFormat("[{0}]", specialization);
			}
			Builder.Append(Separator);
			Builder.Append(termData.TermType.ToString());
			Builder.Append(Separator);
			AppendString(Builder, termData.Description, Separator);
			for (int i = 0; i < Mathf.Min(nLanguages, termData.Languages.Length); i++)
			{
				Builder.Append(Separator);
				string text = termData.Languages[i];
				if (!string.IsNullOrEmpty(specialization))
				{
					text = termData.GetTranslation(i, specialization);
				}
				AppendTranslation(Builder, text, Separator, null);
			}
			Builder.Append("\n");
		}

		private static void AppendString(StringBuilder Builder, string Text, char Separator)
		{
			if (!string.IsNullOrEmpty(Text))
			{
				Text = Text.Replace("\\n", "\n");
				if (Text.IndexOfAny((Separator + "\n\"").ToCharArray()) >= 0)
				{
					Text = Text.Replace("\"", "\"\"");
					Builder.AppendFormat("\"{0}\"", Text);
				}
				else
				{
					Builder.Append(Text);
				}
			}
		}

		private static void AppendTranslation(StringBuilder Builder, string Text, char Separator, string tags)
		{
			if (!string.IsNullOrEmpty(Text))
			{
				Text = Text.Replace("\\n", "\n");
				if (Text.IndexOfAny((Separator + "\n\"").ToCharArray()) >= 0)
				{
					Text = Text.Replace("\"", "\"\"");
					Builder.AppendFormat("\"{0}{1}\"", tags, Text);
				}
				else
				{
					Builder.Append(tags);
					Builder.Append(Text);
				}
			}
		}

		public UnityWebRequest Export_Google_CreateWWWcall(eSpreadsheetUpdateMode UpdateMode = eSpreadsheetUpdateMode.Replace)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected Obj, but got Unknown
			string text = Export_Google_CreateData();
			WWWForm val = new WWWForm();
			val.AddField("key", Google_SpreadsheetKey);
			val.AddField("action", "SetLanguageSource");
			val.AddField("data", text);
			val.AddField("updateMode", UpdateMode.ToString());
			UnityWebRequest val2 = UnityWebRequest.Post(LocalizationManager.GetWebServiceURL(this), val);
			I2Utils.SendWebRequest(val2);
			return val2;
		}

		private string Export_Google_CreateData()
		{
			List<string> categories = GetCategories(OnlyMainCategory: true);
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (string item in categories)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append("<I2Loc>");
				}
				bool specializationsAsRows = true;
				string value = Export_I2CSV(item, ',', specializationsAsRows);
				stringBuilder.Append(item);
				stringBuilder.Append("<I2Loc>");
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		public string Import_CSV(string Category, string CSVstring, eSpreadsheetUpdateMode UpdateMode = eSpreadsheetUpdateMode.Replace, char Separator = ',')
		{
			List<string[]> cSV = LocalizationReader.ReadCSV(CSVstring, Separator);
			return Import_CSV(Category, cSV, UpdateMode);
		}

		public string Import_I2CSV(string Category, string I2CSVstring, eSpreadsheetUpdateMode UpdateMode = eSpreadsheetUpdateMode.Replace)
		{
			List<string[]> cSV = LocalizationReader.ReadI2CSV(I2CSVstring);
			return Import_CSV(Category, cSV, UpdateMode);
		}

		public string Import_CSV(string Category, List<string[]> CSV, eSpreadsheetUpdateMode UpdateMode = eSpreadsheetUpdateMode.Replace)
		{
			string[] array = CSV[0];
			int num = 1;
			int num2 = -1;
			int num3 = -1;
			string[] texts = new string[1] { "Key" };
			string[] texts2 = new string[1] { "Type" };
			string[] texts3 = new string[2] { "Desc", "Description" };
			if (array.Length > 1 && ArrayContains(array[0], texts))
			{
				if (UpdateMode == eSpreadsheetUpdateMode.Replace)
				{
					ClearAllData();
				}
				if (array.Length > 2)
				{
					if (ArrayContains(array[1], texts2))
					{
						num2 = 1;
						num = 2;
					}
					if (ArrayContains(array[1], texts3))
					{
						num3 = 1;
						num = 2;
					}
				}
				if (array.Length > 3)
				{
					if (ArrayContains(array[2], texts2))
					{
						num2 = 2;
						num = 3;
					}
					if (ArrayContains(array[2], texts3))
					{
						num3 = 2;
						num = 3;
					}
				}
				int num4 = Mathf.Max(array.Length - num, 0);
				int[] array2 = new int[num4];
				for (int i = 0; i < num4; i++)
				{
					if (string.IsNullOrEmpty(array[i + num]))
					{
						array2[i] = -1;
						continue;
					}
					string text = array[i + num];
					bool flag = true;
					if (text.StartsWith("$"))
					{
						flag = false;
						text = text.Substring(1);
					}
					GoogleLanguages.UnPackCodeFromLanguageName(text, out var Language, out var code);
					int num5 = -1;
					num5 = (string.IsNullOrEmpty(code) ? GetLanguageIndex(Language, AllowDiscartingRegion: true, SkipDisabled: false) : GetLanguageIndexFromCode(code));
					if (num5 < 0)
					{
						LanguageData languageData = new LanguageData();
						languageData.Name = Language;
						languageData.Code = code;
						languageData.Flags = (byte)(0u | ((!flag) ? 1u : 0u));
						mLanguages.Add(languageData);
						num5 = mLanguages.Count - 1;
					}
					array2[i] = num5;
				}
				num4 = mLanguages.Count;
				int j = 0;
				for (int count = mTerms.Count; j < count; j++)
				{
					TermData termData = mTerms[j];
					if (termData.Languages.Length < num4)
					{
						Array.Resize(ref termData.Languages, num4);
						Array.Resize(ref termData.Flags, num4);
					}
				}
				int k = 1;
				for (int count2 = CSV.Count; k < count2; k++)
				{
					array = CSV[k];
					string Term = (string.IsNullOrEmpty(Category) ? array[0] : (Category + "/" + array[0]));
					string text2 = null;
					if (Term.EndsWith("]"))
					{
						int num6 = Term.LastIndexOf('[');
						if (num6 > 0)
						{
							text2 = Term.Substring(num6 + 1, Term.Length - num6 - 2);
							if (text2 == "touch")
							{
								text2 = "Touch";
							}
							Term = Term.Remove(num6);
						}
					}
					ValidateFullTerm(ref Term);
					if (string.IsNullOrEmpty(Term))
					{
						continue;
					}
					TermData termData2 = GetTermData(Term);
					if (termData2 == null)
					{
						termData2 = new TermData();
						termData2.Term = Term;
						termData2.Languages = new string[mLanguages.Count];
						termData2.Flags = new byte[mLanguages.Count];
						for (int l = 0; l < mLanguages.Count; l++)
						{
							termData2.Languages[l] = string.Empty;
						}
						mTerms.Add(termData2);
						mDictionary.Add(Term, termData2);
					}
					else if (UpdateMode == eSpreadsheetUpdateMode.AddNewTerms)
					{
						continue;
					}
					if (num2 > 0)
					{
						termData2.TermType = GetTermType(array[num2]);
					}
					if (num3 > 0)
					{
						termData2.Description = array[num3];
					}
					for (int m = 0; m < array2.Length && m < array.Length - num; m++)
					{
						if (string.IsNullOrEmpty(array[m + num]))
						{
							continue;
						}
						int num7 = array2[m];
						if (num7 >= 0)
						{
							string text3 = array[m + num];
							if (text3 == "-")
							{
								text3 = string.Empty;
							}
							else if (text3 == "")
							{
								text3 = null;
							}
							termData2.SetTranslation(num7, text3, text2);
						}
					}
				}
				if (Application.isPlaying)
				{
					SaveLanguages(HasUnloadedLanguages());
				}
				return string.Empty;
			}
			return "Bad Spreadsheet Format.\nFirst columns should be 'Key', 'Type' and 'Desc'";
		}

		private bool ArrayContains(string MainText, params string[] texts)
		{
			int i = 0;
			for (int num = texts.Length; i < num; i++)
			{
				if (MainText.IndexOf(texts[i], StringComparison.OrdinalIgnoreCase) >= 0)
				{
					return true;
				}
			}
			return false;
		}

		public static eTermType GetTermType(string type)
		{
			int i = 0;
			for (int num = 10; i <= num; i++)
			{
				eTermType eTermType2 = (eTermType)i;
				if (string.Equals(eTermType2.ToString(), type, StringComparison.OrdinalIgnoreCase))
				{
					return (eTermType)i;
				}
			}
			return eTermType.Text;
		}

		private void Import_Language_from_Cache(int langIndex, string langData, bool useFallback, bool onlyCurrentSpecialization)
		{
			int num = 0;
			while (num < langData.Length)
			{
				int num2 = langData.IndexOf("[i2t]", num);
				if (num2 < 0)
				{
					num2 = langData.Length;
				}
				int num3 = langData.IndexOf("=", num);
				if (num3 >= num2)
				{
					break;
				}
				string term = langData.Substring(num, num3 - num);
				num = num3 + 1;
				TermData termData = GetTermData(term);
				if (termData != null)
				{
					string text = null;
					if (num != num2)
					{
						text = langData.Substring(num, num2 - num);
						if (text.StartsWith("[i2fb]"))
						{
							text = (useFallback ? text.Substring(6) : null);
						}
						if (onlyCurrentSpecialization && text != null)
						{
							text = SpecializationManager.GetSpecializedText(text);
						}
					}
					termData.Languages[langIndex] = text;
				}
				num = num2 + 5;
			}
		}

		public static void FreeUnusedLanguages()
		{
			LanguageSourceData languageSourceData = LocalizationManager.Sources[0];
			int languageIndex = languageSourceData.GetLanguageIndex(LocalizationManager.CurrentLanguage);
			for (int i = 0; i < languageSourceData.mTerms.Count; i++)
			{
				TermData termData = languageSourceData.mTerms[i];
				for (int j = 0; j < termData.Languages.Length; j++)
				{
					if (j != languageIndex)
					{
						termData.Languages[j] = null;
					}
				}
			}
		}

		public void Import_Google_FromCache()
		{
			if (GoogleUpdateFrequency == eGoogleUpdateFrequency.Never || !I2Utils.IsPlaying())
			{
				return;
			}
			string sourcePlayerPrefName = GetSourcePlayerPrefName();
			string text = PersistentStorage.LoadFile(PersistentStorage.eFileType.Persistent, "I2Source_" + sourcePlayerPrefName + ".loc", logExceptions: false);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			if (text.StartsWith("[i2e]", StringComparison.Ordinal))
			{
				text = StringObfucator.Decode(text.Substring(5, text.Length - 5));
			}
			bool flag = false;
			string text2 = Google_LastUpdatedVersion;
			if (PersistentStorage.HasSetting("I2SourceVersion_" + sourcePlayerPrefName))
			{
				text2 = PersistentStorage.GetSetting_String("I2SourceVersion_" + sourcePlayerPrefName, Google_LastUpdatedVersion);
				flag = IsNewerVersion(Google_LastUpdatedVersion, text2);
			}
			if (!flag)
			{
				PersistentStorage.DeleteFile(PersistentStorage.eFileType.Persistent, "I2Source_" + sourcePlayerPrefName + ".loc", logExceptions: false);
				PersistentStorage.DeleteSetting("I2SourceVersion_" + sourcePlayerPrefName);
				return;
			}
			if (text2.Length > 19)
			{
				text2 = string.Empty;
			}
			Google_LastUpdatedVersion = text2;
			Import_Google_Result(text, eSpreadsheetUpdateMode.Replace);
		}

		private bool IsNewerVersion(string currentVersion, string newVersion)
		{
			if (string.IsNullOrEmpty(newVersion))
			{
				return false;
			}
			if (string.IsNullOrEmpty(currentVersion))
			{
				return true;
			}
			if (!long.TryParse(newVersion, out var result) || !long.TryParse(currentVersion, out var result2))
			{
				return true;
			}
			return result > result2;
		}

		public void Import_Google(bool ForceUpdate, bool justCheck)
		{
			if ((!ForceUpdate && GoogleUpdateFrequency == eGoogleUpdateFrequency.Never) || !I2Utils.IsPlaying())
			{
				return;
			}
			eGoogleUpdateFrequency googleUpdateFrequency = GoogleUpdateFrequency;
			string sourcePlayerPrefName = GetSourcePlayerPrefName();
			if (!ForceUpdate && googleUpdateFrequency != eGoogleUpdateFrequency.Always)
			{
				string setting_String = PersistentStorage.GetSetting_String("LastGoogleUpdate_" + sourcePlayerPrefName, "");
				try
				{
					if (DateTime.TryParse(setting_String, out var result))
					{
						double totalDays = (DateTime.Now - result).TotalDays;
						switch (googleUpdateFrequency)
						{
						case eGoogleUpdateFrequency.Daily:
							if (totalDays < 1.0)
							{
								return;
							}
							break;
						case eGoogleUpdateFrequency.Weekly:
							if (totalDays < 8.0)
							{
								return;
							}
							break;
						case eGoogleUpdateFrequency.Monthly:
							if (totalDays < 31.0)
							{
								return;
							}
							break;
						case eGoogleUpdateFrequency.OnlyOnce:
							return;
						case eGoogleUpdateFrequency.EveryOtherDay:
							if (totalDays < 2.0)
							{
								return;
							}
							break;
						}
					}
				}
				catch (Exception)
				{
				}
			}
			PersistentStorage.SetSetting_String("LastGoogleUpdate_" + sourcePlayerPrefName, DateTime.Now.ToString());
			CoroutineManager.Start(Import_Google_Coroutine(justCheck));
		}

		private string GetSourcePlayerPrefName()
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			if (owner == null)
			{
				return null;
			}
			ILanguageSource languageSource = owner;
			string text = ((Object)((languageSource is Object) ? languageSource : null)).name;
			if (!string.IsNullOrEmpty(Google_SpreadsheetKey))
			{
				text += Google_SpreadsheetKey;
			}
			string[] globalSources = LocalizationManager.GlobalSources;
			ILanguageSource languageSource2 = owner;
			if (Array.IndexOf(globalSources, ((Object)((languageSource2 is Object) ? languageSource2 : null)).name) >= 0)
			{
				return text;
			}
			Scene activeScene = SceneManager.GetActiveScene();
			return activeScene.name + "_" + text;
		}

		private IEnumerator Import_Google_Coroutine(bool JustCheck)
		{
			UnityWebRequest www = Import_Google_CreateWWWcall(ForceUpdate: false, JustCheck);
			if (www == null)
			{
				yield break;
			}
			while (!www.isDone)
			{
				yield return null;
			}
			if (string.IsNullOrEmpty(www.error))
			{
				byte[] data = www.downloadHandler.data;
				string text = Encoding.UTF8.GetString(data, 0, data.Length);
				bool flag = string.IsNullOrEmpty(text) || text == "\"\"";
				if (JustCheck)
				{
					if (!flag)
					{
						Debug.LogWarning((object)"Spreadsheet is not up-to-date and Google Live Synchronization is enabled\nWhen playing in the device the Spreadsheet will be downloaded and translations may not behave as what you see in the editor.\nTo fix this, Import or Export replace to Google");
						GoogleLiveSyncIsUptoDate = false;
					}
					yield break;
				}
				if (!flag)
				{
					mDelayedGoogleData = text;
					switch (GoogleUpdateSynchronization)
					{
					case eGoogleUpdateSynchronization.AsSoonAsDownloaded:
						ApplyDownloadedDataFromGoogle();
						break;
					case eGoogleUpdateSynchronization.OnSceneLoaded:
						SceneManager.sceneLoaded += ApplyDownloadedDataOnSceneLoaded;
						break;
					}
					yield break;
				}
			}
			if (Event_OnSourceUpdateFromGoogle != null)
			{
				Event_OnSourceUpdateFromGoogle(this, ReceivedNewData: false, www.error);
			}
			Debug.Log((object)"Language Source was up-to-date with Google Spreadsheet");
		}

		private void ApplyDownloadedDataOnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			SceneManager.sceneLoaded -= ApplyDownloadedDataOnSceneLoaded;
			ApplyDownloadedDataFromGoogle();
		}

		public void ApplyDownloadedDataFromGoogle()
		{
			if (string.IsNullOrEmpty(mDelayedGoogleData))
			{
				return;
			}
			if (string.IsNullOrEmpty(Import_Google_Result(mDelayedGoogleData, eSpreadsheetUpdateMode.Replace, saveInPlayerPrefs: true)))
			{
				if (Event_OnSourceUpdateFromGoogle != null)
				{
					Event_OnSourceUpdateFromGoogle(this, ReceivedNewData: true, "");
				}
				LocalizationManager.LocalizeAll(Force: true);
				Debug.Log((object)"Done Google Sync");
			}
			else
			{
				if (Event_OnSourceUpdateFromGoogle != null)
				{
					Event_OnSourceUpdateFromGoogle(this, ReceivedNewData: false, "");
				}
				Debug.Log((object)"Done Google Sync: source was up-to-date");
			}
		}

		public UnityWebRequest Import_Google_CreateWWWcall(bool ForceUpdate, bool justCheck)
		{
			if (!HasGoogleSpreadsheet())
			{
				return null;
			}
			string text = PersistentStorage.GetSetting_String("I2SourceVersion_" + GetSourcePlayerPrefName(), Google_LastUpdatedVersion);
			if (text.Length > 19)
			{
				text = string.Empty;
			}
			if (IsNewerVersion(text, Google_LastUpdatedVersion))
			{
				Google_LastUpdatedVersion = text;
			}
			UnityWebRequest val = UnityWebRequest.Get(string.Format("{0}?key={1}&action=GetLanguageSource&version={2}", LocalizationManager.GetWebServiceURL(this), Google_SpreadsheetKey, ForceUpdate ? "0" : Google_LastUpdatedVersion));
			I2Utils.SendWebRequest(val);
			return val;
		}

		public bool HasGoogleSpreadsheet()
		{
			if (!string.IsNullOrEmpty(Google_WebServiceURL) && !string.IsNullOrEmpty(Google_SpreadsheetKey))
			{
				return !string.IsNullOrEmpty(LocalizationManager.GetWebServiceURL(this));
			}
			return false;
		}

		public string Import_Google_Result(string JsonString, eSpreadsheetUpdateMode UpdateMode, bool saveInPlayerPrefs = false)
		{
			try
			{
				string empty = string.Empty;
				if (string.IsNullOrEmpty(JsonString) || JsonString == "\"\"")
				{
					return empty;
				}
				int num = JsonString.IndexOf("version=", StringComparison.Ordinal);
				int num2 = JsonString.IndexOf("script_version=", StringComparison.Ordinal);
				if (num < 0 || num2 < 0)
				{
					return "Invalid Response from Google, Most likely the WebService needs to be updated";
				}
				num += "version=".Length;
				num2 += "script_version=".Length;
				string text = JsonString.Substring(num, JsonString.IndexOf(",", num, StringComparison.Ordinal) - num);
				int num3 = int.Parse(JsonString.Substring(num2, JsonString.IndexOf(",", num2, StringComparison.Ordinal) - num2));
				if (text.Length > 19)
				{
					text = string.Empty;
				}
				if (num3 != LocalizationManager.GetRequiredWebServiceVersion())
				{
					return "The current Google WebService is not supported.\nPlease, delete the WebService from the Google Drive and Install the latest version.";
				}
				if (saveInPlayerPrefs && !IsNewerVersion(Google_LastUpdatedVersion, text))
				{
					return "LanguageSource is up-to-date";
				}
				if (saveInPlayerPrefs)
				{
					string sourcePlayerPrefName = GetSourcePlayerPrefName();
					PersistentStorage.SaveFile(PersistentStorage.eFileType.Persistent, "I2Source_" + sourcePlayerPrefName + ".loc", "[i2e]" + StringObfucator.Encode(JsonString));
					PersistentStorage.SetSetting_String("I2SourceVersion_" + sourcePlayerPrefName, text);
					PersistentStorage.ForceSaveSettings();
				}
				Google_LastUpdatedVersion = text;
				if (UpdateMode == eSpreadsheetUpdateMode.Replace)
				{
					ClearAllData();
				}
				int num4 = JsonString.IndexOf("[i2category]", StringComparison.Ordinal);
				while (num4 > 0)
				{
					num4 += "[i2category]".Length;
					int num5 = JsonString.IndexOf("[/i2category]", num4, StringComparison.Ordinal);
					string category = JsonString.Substring(num4, num5 - num4);
					num5 += "[/i2category]".Length;
					int num6 = JsonString.IndexOf("[/i2csv]", num5, StringComparison.Ordinal);
					string i2CSVstring = JsonString.Substring(num5, num6 - num5);
					num4 = JsonString.IndexOf("[i2category]", num6, StringComparison.Ordinal);
					Import_I2CSV(category, i2CSVstring, UpdateMode);
					if (UpdateMode == eSpreadsheetUpdateMode.Replace)
					{
						UpdateMode = eSpreadsheetUpdateMode.Merge;
					}
				}
				GoogleLiveSyncIsUptoDate = true;
				if (I2Utils.IsPlaying())
				{
					SaveLanguages(unloadAll: true);
				}
				if (!string.IsNullOrEmpty(empty))
				{
					Editor_SetDirty();
				}
				return empty;
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)ex);
				return ex.ToString();
			}
		}

		public int GetLanguageIndex(string language, bool AllowDiscartingRegion = true, bool SkipDisabled = true)
		{
			int i = 0;
			for (int count = mLanguages.Count; i < count; i++)
			{
				if ((!SkipDisabled || mLanguages[i].IsEnabled()) && string.Compare(mLanguages[i].Name, language, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return i;
				}
			}
			if (AllowDiscartingRegion)
			{
				int num = -1;
				int num2 = 0;
				int j = 0;
				for (int count2 = mLanguages.Count; j < count2; j++)
				{
					if (!SkipDisabled || mLanguages[j].IsEnabled())
					{
						int commonWordInLanguageNames = GetCommonWordInLanguageNames(mLanguages[j].Name, language);
						if (commonWordInLanguageNames > num2)
						{
							num2 = commonWordInLanguageNames;
							num = j;
						}
					}
				}
				if (num >= 0)
				{
					return num;
				}
			}
			return -1;
		}

		public LanguageData GetLanguageData(string language, bool AllowDiscartingRegion = true)
		{
			int languageIndex = GetLanguageIndex(language, AllowDiscartingRegion, SkipDisabled: false);
			if (languageIndex >= 0)
			{
				return mLanguages[languageIndex];
			}
			return null;
		}

		public bool IsCurrentLanguage(int languageIndex)
		{
			return LocalizationManager.CurrentLanguage == mLanguages[languageIndex].Name;
		}

		public int GetLanguageIndexFromCode(string Code, bool exactMatch = true, bool ignoreDisabled = false)
		{
			int i = 0;
			for (int count = mLanguages.Count; i < count; i++)
			{
				if ((!ignoreDisabled || mLanguages[i].IsEnabled()) && string.Compare(mLanguages[i].Code, Code, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return i;
				}
			}
			if (!exactMatch)
			{
				int j = 0;
				for (int count2 = mLanguages.Count; j < count2; j++)
				{
					if ((!ignoreDisabled || mLanguages[j].IsEnabled()) && string.Compare(mLanguages[j].Code, 0, Code, 0, 2, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return j;
					}
				}
			}
			return -1;
		}

		public static int GetCommonWordInLanguageNames(string Language1, string Language2)
		{
			if (string.IsNullOrEmpty(Language1) || string.IsNullOrEmpty(Language2))
			{
				return 0;
			}
			char[] separator = "( )-/\\".ToCharArray();
			string[] array = Language1.ToLower().Split(separator);
			string[] array2 = Language2.ToLower().Split(separator);
			int num = 0;
			string[] array3 = array;
			foreach (string value in array3)
			{
				if (!string.IsNullOrEmpty(value) && Enumerable.Contains(array2, value))
				{
					num++;
				}
			}
			array3 = array2;
			foreach (string value2 in array3)
			{
				if (!string.IsNullOrEmpty(value2) && Enumerable.Contains(array, value2))
				{
					num++;
				}
			}
			return num;
		}

		public static bool AreTheSameLanguage(string Language1, string Language2)
		{
			Language1 = GetLanguageWithoutRegion(Language1);
			Language2 = GetLanguageWithoutRegion(Language2);
			return string.Compare(Language1, Language2, StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static string GetLanguageWithoutRegion(string Language)
		{
			int num = Language.IndexOfAny("(/\\[,{".ToCharArray());
			if (num < 0)
			{
				return Language;
			}
			return Language.Substring(0, num).Trim();
		}

		public void AddLanguage(string LanguageName)
		{
			AddLanguage(LanguageName, GoogleLanguages.GetLanguageCode(LanguageName));
		}

		public void AddLanguage(string LanguageName, string LanguageCode)
		{
			if (GetLanguageIndex(LanguageName, AllowDiscartingRegion: false) < 0)
			{
				LanguageData languageData = new LanguageData();
				languageData.Name = LanguageName;
				languageData.Code = LanguageCode;
				mLanguages.Add(languageData);
				int count = mLanguages.Count;
				int i = 0;
				for (int count2 = mTerms.Count; i < count2; i++)
				{
					Array.Resize(ref mTerms[i].Languages, count);
					Array.Resize(ref mTerms[i].Flags, count);
				}
				Editor_SetDirty();
			}
		}

		public void RemoveLanguage(string LanguageName)
		{
			int languageIndex = GetLanguageIndex(LanguageName, AllowDiscartingRegion: false, SkipDisabled: false);
			if (languageIndex < 0)
			{
				return;
			}
			int count = mLanguages.Count;
			int i = 0;
			for (int count2 = mTerms.Count; i < count2; i++)
			{
				for (int j = languageIndex + 1; j < count; j++)
				{
					mTerms[i].Languages[j - 1] = mTerms[i].Languages[j];
					mTerms[i].Flags[j - 1] = mTerms[i].Flags[j];
				}
				Array.Resize(ref mTerms[i].Languages, count - 1);
				Array.Resize(ref mTerms[i].Flags, count - 1);
			}
			mLanguages.RemoveAt(languageIndex);
			Editor_SetDirty();
		}

		public List<string> GetLanguages(bool skipDisabled = true)
		{
			List<string> list = new List<string>();
			int i = 0;
			for (int count = mLanguages.Count; i < count; i++)
			{
				if (!skipDisabled || mLanguages[i].IsEnabled())
				{
					list.Add(mLanguages[i].Name);
				}
			}
			return list;
		}

		public List<string> GetLanguagesCode(bool allowRegions = true, bool skipDisabled = true)
		{
			List<string> list = new List<string>();
			int i = 0;
			for (int count = mLanguages.Count; i < count; i++)
			{
				if (!skipDisabled || mLanguages[i].IsEnabled())
				{
					string text = mLanguages[i].Code;
					if (!allowRegions && text != null && text.Length > 2)
					{
						text = text.Substring(0, 2);
					}
					if (!string.IsNullOrEmpty(text) && !list.Contains(text))
					{
						list.Add(text);
					}
				}
			}
			return list;
		}

		public bool IsLanguageEnabled(string Language)
		{
			int languageIndex = GetLanguageIndex(Language, AllowDiscartingRegion: false);
			if (languageIndex >= 0)
			{
				return mLanguages[languageIndex].IsEnabled();
			}
			return false;
		}

		public void EnableLanguage(string Language, bool bEnabled)
		{
			int languageIndex = GetLanguageIndex(Language, AllowDiscartingRegion: false, SkipDisabled: false);
			if (languageIndex >= 0)
			{
				mLanguages[languageIndex].SetEnabled(bEnabled);
			}
		}

		public bool AllowUnloadingLanguages()
		{
			return _AllowUnloadingLanguages != eAllowUnloadLanguages.Never;
		}

		private string GetSavedLanguageFileName(int languageIndex)
		{
			if (languageIndex < 0)
			{
				return null;
			}
			return "LangSource_" + GetSourcePlayerPrefName() + "_" + mLanguages[languageIndex].Name + ".loc";
		}

		public void LoadLanguage(int languageIndex, bool UnloadOtherLanguages, bool useFallback, bool onlyCurrentSpecialization, bool forceLoad)
		{
			if (!AllowUnloadingLanguages() || !PersistentStorage.CanAccessFiles())
			{
				return;
			}
			if (languageIndex >= 0 && (forceLoad || !mLanguages[languageIndex].IsLoaded()))
			{
				string savedLanguageFileName = GetSavedLanguageFileName(languageIndex);
				string text = PersistentStorage.LoadFile(PersistentStorage.eFileType.Temporal, savedLanguageFileName, logExceptions: false);
				if (!string.IsNullOrEmpty(text))
				{
					Import_Language_from_Cache(languageIndex, text, useFallback, onlyCurrentSpecialization);
					mLanguages[languageIndex].SetLoaded(loaded: true);
				}
			}
			if (!UnloadOtherLanguages || !I2Utils.IsPlaying())
			{
				return;
			}
			for (int i = 0; i < mLanguages.Count; i++)
			{
				if (i != languageIndex)
				{
					UnloadLanguage(i);
				}
			}
		}

		public void LoadAllLanguages(bool forceLoad = false)
		{
			for (int i = 0; i < mLanguages.Count; i++)
			{
				LoadLanguage(i, UnloadOtherLanguages: false, useFallback: false, onlyCurrentSpecialization: false, forceLoad);
			}
		}

		public void UnloadLanguage(int languageIndex)
		{
			if (!AllowUnloadingLanguages() || !PersistentStorage.CanAccessFiles() || !I2Utils.IsPlaying() || !mLanguages[languageIndex].IsLoaded() || !mLanguages[languageIndex].CanBeUnloaded() || IsCurrentLanguage(languageIndex) || !PersistentStorage.HasFile(PersistentStorage.eFileType.Temporal, GetSavedLanguageFileName(languageIndex)))
			{
				return;
			}
			foreach (TermData mTerm in mTerms)
			{
				mTerm.Languages[languageIndex] = null;
			}
			mLanguages[languageIndex].SetLoaded(loaded: false);
		}

		public void SaveLanguages(bool unloadAll, PersistentStorage.eFileType fileLocation = PersistentStorage.eFileType.Temporal)
		{
			if (!AllowUnloadingLanguages() || !PersistentStorage.CanAccessFiles())
			{
				return;
			}
			for (int i = 0; i < mLanguages.Count; i++)
			{
				string text = Export_Language_to_Cache(i, IsCurrentLanguage(i));
				if (!string.IsNullOrEmpty(text))
				{
					PersistentStorage.SaveFile(PersistentStorage.eFileType.Temporal, GetSavedLanguageFileName(i), text);
				}
			}
			if (!unloadAll)
			{
				return;
			}
			for (int j = 0; j < mLanguages.Count; j++)
			{
				if (unloadAll && !IsCurrentLanguage(j))
				{
					UnloadLanguage(j);
				}
			}
		}

		public bool HasUnloadedLanguages()
		{
			for (int i = 0; i < mLanguages.Count; i++)
			{
				if (!mLanguages[i].IsLoaded())
				{
					return true;
				}
			}
			return false;
		}

		public List<string> GetCategories(bool OnlyMainCategory = false, List<string> Categories = null)
		{
			if (Categories == null)
			{
				Categories = new List<string>();
			}
			foreach (TermData mTerm in mTerms)
			{
				string categoryFromFullTerm = GetCategoryFromFullTerm(mTerm.Term, OnlyMainCategory);
				if (!Categories.Contains(categoryFromFullTerm))
				{
					Categories.Add(categoryFromFullTerm);
				}
			}
			Categories.Sort();
			return Categories;
		}

		public static string GetKeyFromFullTerm(string FullTerm, bool OnlyMainCategory = false)
		{
			int num = (OnlyMainCategory ? FullTerm.IndexOfAny(CategorySeparators) : FullTerm.LastIndexOfAny(CategorySeparators));
			if (num >= 0)
			{
				return FullTerm.Substring(num + 1);
			}
			return FullTerm;
		}

		public static string GetCategoryFromFullTerm(string FullTerm, bool OnlyMainCategory = false)
		{
			int num = (OnlyMainCategory ? FullTerm.IndexOfAny(CategorySeparators) : FullTerm.LastIndexOfAny(CategorySeparators));
			if (num >= 0)
			{
				return FullTerm.Substring(0, num);
			}
			return EmptyCategory;
		}

		public static void DeserializeFullTerm(string FullTerm, out string Key, out string Category, bool OnlyMainCategory = false)
		{
			int num = (OnlyMainCategory ? FullTerm.IndexOfAny(CategorySeparators) : FullTerm.LastIndexOfAny(CategorySeparators));
			if (num < 0)
			{
				Category = EmptyCategory;
				Key = FullTerm;
			}
			else
			{
				Category = FullTerm.Substring(0, num);
				Key = FullTerm.Substring(num + 1);
			}
		}

		public void UpdateDictionary(bool force = false)
		{
			if (force || mDictionary == null || mDictionary.Count != mTerms.Count)
			{
				StringComparer stringComparer = (CaseInsensitiveTerms ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
				if (mDictionary.Comparer != stringComparer)
				{
					mDictionary = new Dictionary<string, TermData>(stringComparer);
				}
				else
				{
					mDictionary.Clear();
				}
				int i = 0;
				for (int count = mTerms.Count; i < count; i++)
				{
					TermData termData = mTerms[i];
					ValidateFullTerm(ref termData.Term);
					mDictionary[termData.Term] = mTerms[i];
					mTerms[i].Validate();
				}
				if (I2Utils.IsPlaying())
				{
					SaveLanguages(unloadAll: true);
				}
			}
		}

		public string GetTranslation(string term, string overrideLanguage = null, string overrideSpecialization = null, bool skipDisabled = false, bool allowCategoryMistmatch = false)
		{
			if (TryGetTranslation(term, out var Translation, overrideLanguage, overrideSpecialization, skipDisabled, allowCategoryMistmatch))
			{
				return Translation;
			}
			return string.Empty;
		}

		public bool TryGetTranslation(string term, out string Translation, string overrideLanguage = null, string overrideSpecialization = null, bool skipDisabled = false, bool allowCategoryMistmatch = false)
		{
			int languageIndex = GetLanguageIndex((overrideLanguage == null) ? LocalizationManager.CurrentLanguage : overrideLanguage, AllowDiscartingRegion: true, SkipDisabled: false);
			if (languageIndex >= 0 && (!skipDisabled || mLanguages[languageIndex].IsEnabled()))
			{
				TermData termData = GetTermData(term, allowCategoryMistmatch);
				if (termData != null)
				{
					Translation = termData.GetTranslation(languageIndex, overrideSpecialization, editMode: true);
					if (Translation == "---")
					{
						Translation = string.Empty;
						return true;
					}
					if (!string.IsNullOrEmpty(Translation))
					{
						return true;
					}
					Translation = null;
				}
				if (OnMissingTranslation == MissingTranslationAction.ShowWarning)
				{
					Translation = $"<!-Missing Translation [{term}]-!>";
					return true;
				}
				if (OnMissingTranslation == MissingTranslationAction.Fallback && termData != null)
				{
					return TryGetFallbackTranslation(termData, out Translation, languageIndex, overrideSpecialization, skipDisabled);
				}
				if (OnMissingTranslation == MissingTranslationAction.Empty)
				{
					Translation = string.Empty;
					return true;
				}
				if (OnMissingTranslation == MissingTranslationAction.ShowTerm)
				{
					Translation = term;
					return true;
				}
			}
			Translation = null;
			return false;
		}

		private bool TryGetFallbackTranslation(TermData termData, out string Translation, int langIndex, string overrideSpecialization = null, bool skipDisabled = false)
		{
			string text = mLanguages[langIndex].Code;
			if (!string.IsNullOrEmpty(text))
			{
				if (Enumerable.Contains(text, '-'))
				{
					text = text.Substring(0, text.IndexOf('-'));
				}
				for (int i = 0; i < mLanguages.Count; i++)
				{
					if (i != langIndex && mLanguages[i].Code.StartsWith(text) && (!skipDisabled || mLanguages[i].IsEnabled()))
					{
						Translation = termData.GetTranslation(i, overrideSpecialization, editMode: true);
						if (!string.IsNullOrEmpty(Translation))
						{
							return true;
						}
					}
				}
			}
			for (int j = 0; j < mLanguages.Count; j++)
			{
				if (j != langIndex && (!skipDisabled || mLanguages[j].IsEnabled()) && (text == null || !mLanguages[j].Code.StartsWith(text)))
				{
					Translation = termData.GetTranslation(j, overrideSpecialization, editMode: true);
					if (!string.IsNullOrEmpty(Translation))
					{
						return true;
					}
				}
			}
			Translation = null;
			return false;
		}

		public TermData AddTerm(string term)
		{
			return AddTerm(term, eTermType.Text);
		}

		public TermData GetTermData(string term, bool allowCategoryMistmatch = false)
		{
			if (string.IsNullOrEmpty(term))
			{
				return null;
			}
			if (mDictionary.Count == 0)
			{
				UpdateDictionary();
			}
			if (mDictionary.TryGetValue(term, out var value))
			{
				return value;
			}
			TermData termData = null;
			if (allowCategoryMistmatch)
			{
				string keyFromFullTerm = GetKeyFromFullTerm(term);
				foreach (KeyValuePair<string, TermData> item in mDictionary)
				{
					if (item.Value.IsTerm(keyFromFullTerm, allowCategoryMistmatch: true))
					{
						if (termData != null)
						{
							return null;
						}
						termData = item.Value;
					}
				}
			}
			return termData;
		}

		public bool ContainsTerm(string term)
		{
			return GetTermData(term) != null;
		}

		public List<string> GetTermsList(string Category = null)
		{
			if (mDictionary.Count != mTerms.Count)
			{
				UpdateDictionary();
			}
			if (string.IsNullOrEmpty(Category))
			{
				return new List<string>(mDictionary.Keys);
			}
			List<string> list = new List<string>();
			for (int i = 0; i < mTerms.Count; i++)
			{
				TermData termData = mTerms[i];
				if (GetCategoryFromFullTerm(termData.Term) == Category)
				{
					list.Add(termData.Term);
				}
			}
			return list;
		}

		public TermData AddTerm(string NewTerm, eTermType termType, bool SaveSource = true)
		{
			ValidateFullTerm(ref NewTerm);
			NewTerm = NewTerm.Trim();
			if (mLanguages.Count == 0)
			{
				AddLanguage("English", "en");
			}
			TermData termData = GetTermData(NewTerm);
			if (termData == null)
			{
				termData = new TermData();
				termData.Term = NewTerm;
				termData.TermType = termType;
				termData.Languages = new string[mLanguages.Count];
				termData.Flags = new byte[mLanguages.Count];
				mTerms.Add(termData);
				mDictionary.Add(NewTerm, termData);
			}
			return termData;
		}

		public void RemoveTerm(string term)
		{
			int i = 0;
			for (int count = mTerms.Count; i < count; i++)
			{
				if (mTerms[i].Term == term)
				{
					mTerms.RemoveAt(i);
					mDictionary.Remove(term);
					break;
				}
			}
		}

		public static void ValidateFullTerm(ref string Term)
		{
			Term = Term.Replace('\\', '/');
			Term = Term.Trim();
			if (Term.StartsWith(EmptyCategory, StringComparison.Ordinal) && Term.Length > EmptyCategory.Length && Term[EmptyCategory.Length] == '/')
			{
				Term = Term.Substring(EmptyCategory.Length + 1);
			}
			Term = I2Utils.GetValidTermName(Term, allowCategory: true);
		}
	}
	public enum eSpreadsheetUpdateMode
	{
		None,
		Replace,
		Merge,
		AddNewTerms
	}
	public class LocalizationReader
	{
		public static Dictionary<string, string> ReadTextAsset(TextAsset asset)
		{
			StringReader stringReader = new StringReader(Encoding.UTF8.GetString(asset.bytes, 0, asset.bytes.Length).Replace("\r\n", "\n").Replace("\r", "\n"));
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.Ordinal);
			string line;
			while ((line = stringReader.ReadLine()) != null)
			{
				if (TextAsset_ReadLine(line, out var key, out var value, out var __d1, out var __d2, out var __d3) && !string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
				{
					dictionary[key] = value;
				}
			}
			return dictionary;
		}

		public static bool TextAsset_ReadLine(string line, out string key, out string value, out string category, out string comment, out string termType)
		{
			key = string.Empty;
			category = string.Empty;
			comment = string.Empty;
			termType = string.Empty;
			value = string.Empty;
			int num = line.LastIndexOf("//");
			if (num >= 0)
			{
				comment = line.Substring(num + 2).Trim();
				comment = DecodeString(comment);
				line = line.Substring(0, num);
			}
			int num2 = line.IndexOf("=");
			if (num2 < 0)
			{
				return false;
			}
			key = line.Substring(0, num2).Trim();
			value = line.Substring(num2 + 1).Trim();
			value = value.Replace("\r\n", "\n").Replace("\n", "\\n");
			value = DecodeString(value);
			if (key.Length > 2 && key[0] == '[')
			{
				int num3 = key.IndexOf(']');
				if (num3 >= 0)
				{
					termType = key.Substring(1, num3 - 1);
					key = key.Substring(num3 + 1);
				}
			}
			ValidateFullTerm(ref key);
			return true;
		}

		public static string ReadCSVfile(string Path, Encoding encoding)
		{
			string text = string.Empty;
			using (StreamReader streamReader = new StreamReader(Path, encoding))
			{
				text = streamReader.ReadToEnd();
			}
			text = text.Replace("\r\n", "\n");
			return text.Replace("\r", "\n");
		}

		public static List<string[]> ReadCSV(string Text, char Separator = ',')
		{
			int iStart = 0;
			List<string[]> list = new List<string[]>();
			while (iStart < Text.Length)
			{
				string[] array = ParseCSVline(Text, ref iStart, Separator);
				if (array == null)
				{
					break;
				}
				list.Add(array);
			}
			return list;
		}

		private static string[] ParseCSVline(string Line, ref int iStart, char Separator)
		{
			List<string> list = new List<string>();
			int length = Line.Length;
			int iWordStart = iStart;
			bool flag = false;
			while (iStart < length)
			{
				char c = Line[iStart];
				if (flag)
				{
					if (c == '"')
					{
						if (iStart + 1 >= length || Line[iStart + 1] != '"')
						{
							flag = false;
						}
						else if (iStart + 2 < length && Line[iStart + 2] == '"')
						{
							flag = false;
							iStart += 2;
						}
						else
						{
							iStart++;
						}
					}
				}
				else if (c == '\n' || c == Separator)
				{
					AddCSVtoken(ref list, ref Line, iStart, ref iWordStart);
					if (c == '\n')
					{
						iStart++;
						break;
					}
				}
				else if (c == '"')
				{
					flag = true;
				}
				iStart++;
			}
			if (iStart > iWordStart)
			{
				AddCSVtoken(ref list, ref Line, iStart, ref iWordStart);
			}
			return list.ToArray();
		}

		private static void AddCSVtoken(ref List<string> list, ref string Line, int iEnd, ref int iWordStart)
		{
			string text = Line.Substring(iWordStart, iEnd - iWordStart);
			iWordStart = iEnd + 1;
			text = text.Replace("\"\"", "\"");
			if (text.Length > 1 && text[0] == '"' && text[text.Length - 1] == '"')
			{
				text = text.Substring(1, text.Length - 2);
			}
			list.Add(text);
		}

		public static List<string[]> ReadI2CSV(string Text)
		{
			string[] separator = new string[1] { "[*]" };
			string[] separator2 = new string[1] { "[ln]" };
			List<string[]> list = new List<string[]>();
			string[] array = Text.Split(separator2, StringSplitOptions.None);
			foreach (string text in array)
			{
				list.Add(text.Split(separator, StringSplitOptions.None));
			}
			return list;
		}

		public static void ValidateFullTerm(ref string Term)
		{
			Term = Term.Replace('\\', '/');
			int num = Term.IndexOf('/');
			if (num >= 0)
			{
				int startIndex;
				while ((startIndex = Term.LastIndexOf('/')) != num)
				{
					Term = Term.Remove(startIndex, 1);
				}
			}
		}

		public static string EncodeString(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			return str.Replace("\r\n", "<\\n>").Replace("\r", "<\\n>").Replace("\n", "<\\n>");
		}

		public static string DecodeString(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			return str.Replace("<\\n>", "\r\n");
		}
	}
	[AddComponentMenu("I2/Localization/I2 Localize")]
	public class Localize : MonoBehaviour
	{
		public enum TermModification
		{
			DontModify,
			ToUpper,
			ToLower,
			ToUpperFirst,
			ToTitle
		}

		public string mTerm = string.Empty;

		public string mTermSecondary = string.Empty;

		[NonSerialized]
		public string FinalTerm;

		[NonSerialized]
		public string FinalSecondaryTerm;

		public TermModification PrimaryTermModifier;

		public TermModification SecondaryTermModifier;

		public string TermPrefix;

		public string TermSuffix;

		public bool LocalizeOnAwake = true;

		private string LastLocalizedLanguage;

		public bool IgnoreRTL;

		public int MaxCharactersInRTL;

		public bool IgnoreNumbersInRTL = true;

		public bool CorrectAlignmentForRTL = true;

		public bool AddSpacesToJoinedLanguages;

		public bool AllowLocalizedParameters = true;

		public List<Object> TranslatedObjects = new List<Object>();

		[NonSerialized]
		public Dictionary<string, Object> mAssetDictionary = new Dictionary<string, Object>(StringComparer.Ordinal);

		public UnityEvent LocalizeEvent = new UnityEvent();

		public static string MainTranslation;

		public static string SecondaryTranslation;

		public static string CallBackTerm;

		public static string CallBackSecondaryTerm;

		public static Localize CurrentLocalizeComponent;

		public bool AlwaysForceLocalize;

		[SerializeField]
		public EventCallback LocalizeCallBack = new EventCallback();

		public bool mGUI_ShowReferences;

		public bool mGUI_ShowTems = true;

		public bool mGUI_ShowCallback;

		public ILocalizeTarget mLocalizeTarget;

		public string mLocalizeTargetName;

		public string Term
		{
			get
			{
				return mTerm;
			}
			set
			{
				SetTerm(value);
			}
		}

		public string SecondaryTerm
		{
			get
			{
				return mTermSecondary;
			}
			set
			{
				SetTerm(null, value);
			}
		}

		private void Awake()
		{
			UpdateAssetDictionary();
			FindTarget();
			if (LocalizeOnAwake)
			{
				OnLocalize();
			}
		}

		private void OnEnable()
		{
			OnLocalize();
		}

		public bool HasCallback()
		{
			if (LocalizeCallBack.HasCallback())
			{
				return true;
			}
			return ((UnityEventBase)LocalizeEvent).GetPersistentEventCount() > 0;
		}

		public void OnLocalize(bool Force = false)
		{
			if ((!Force && (!((Behaviour)this).enabled || (Object)(object)((Component)this).gameObject == (Object)null || !((Component)this).gameObject.activeInHierarchy)) || string.IsNullOrEmpty(LocalizationManager.CurrentLanguage) || (!AlwaysForceLocalize && !Force && !HasCallback() && LastLocalizedLanguage == LocalizationManager.CurrentLanguage))
			{
				return;
			}
			LastLocalizedLanguage = LocalizationManager.CurrentLanguage;
			if (string.IsNullOrEmpty(FinalTerm) || string.IsNullOrEmpty(FinalSecondaryTerm))
			{
				GetFinalTerms(out FinalTerm, out FinalSecondaryTerm);
			}
			bool flag = I2Utils.IsPlaying() && HasCallback();
			if (!flag && string.IsNullOrEmpty(FinalTerm) && string.IsNullOrEmpty(FinalSecondaryTerm))
			{
				return;
			}
			CallBackTerm = FinalTerm;
			CallBackSecondaryTerm = FinalSecondaryTerm;
			MainTranslation = ((string.IsNullOrEmpty(FinalTerm) || FinalTerm == "-") ? null : LocalizationManager.GetTranslation(FinalTerm, FixForRTL: false));
			SecondaryTranslation = ((string.IsNullOrEmpty(FinalSecondaryTerm) || FinalSecondaryTerm == "-") ? null : LocalizationManager.GetTranslation(FinalSecondaryTerm, FixForRTL: false));
			if (!flag && string.IsNullOrEmpty(FinalTerm) && string.IsNullOrEmpty(SecondaryTranslation))
			{
				return;
			}
			CurrentLocalizeComponent = this;
			LocalizeCallBack.Execute((Object)(object)this);
			LocalizeEvent.Invoke();
			LocalizationManager.ApplyLocalizationParams(ref MainTranslation, ((Component)this).gameObject, AllowLocalizedParameters);
			if (!FindTarget())
			{
				return;
			}
			bool flag2 = LocalizationManager.IsRight2Left && !IgnoreRTL;
			if (MainTranslation != null)
			{
				switch (PrimaryTermModifier)
				{
				case TermModification.ToUpper:
					MainTranslation = MainTranslation.ToUpper();
					break;
				case TermModification.ToLower:
					MainTranslation = MainTranslation.ToLower();
					break;
				case TermModification.ToUpperFirst:
					MainTranslation = GoogleTranslation.UppercaseFirst(MainTranslation);
					break;
				case TermModification.ToTitle:
					MainTranslation = GoogleTranslation.TitleCase(MainTranslation);
					break;
				}
				if (!string.IsNullOrEmpty(TermPrefix))
				{
					MainTranslation = (flag2 ? (MainTranslation + TermPrefix) : (TermPrefix + MainTranslation));
				}
				if (!string.IsNullOrEmpty(TermSuffix))
				{
					MainTranslation = (flag2 ? (TermSuffix + MainTranslation) : (MainTranslation + TermSuffix));
				}
				if (AddSpacesToJoinedLanguages && LocalizationManager.HasJoinedWords && !string.IsNullOrEmpty(MainTranslation))
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(MainTranslation[0]);
					int i = 1;
					for (int length = MainTranslation.Length; i < length; i++)
					{
						stringBuilder.Append(' ');
						stringBuilder.Append(MainTranslation[i]);
					}
					MainTranslation = stringBuilder.ToString();
				}
				if (flag2 && mLocalizeTarget.AllowMainTermToBeRTL() && !string.IsNullOrEmpty(MainTranslation))
				{
					MainTranslation = LocalizationManager.ApplyRTLfix(MainTranslation, MaxCharactersInRTL, IgnoreNumbersInRTL);
				}
			}
			if (SecondaryTranslation != null)
			{
				switch (SecondaryTermModifier)
				{
				case TermModification.ToUpper:
					SecondaryTranslation = SecondaryTranslation.ToUpper();
					break;
				case TermModification.ToLower:
					SecondaryTranslation = SecondaryTranslation.ToLower();
					break;
				case TermModification.ToUpperFirst:
					SecondaryTranslation = GoogleTranslation.UppercaseFirst(SecondaryTranslation);
					break;
				case TermModification.ToTitle:
					SecondaryTranslation = GoogleTranslation.TitleCase(SecondaryTranslation);
					break;
				}
				if (flag2 && mLocalizeTarget.AllowSecondTermToBeRTL() && !string.IsNullOrEmpty(SecondaryTranslation))
				{
					SecondaryTranslation = LocalizationManager.ApplyRTLfix(SecondaryTranslation);
				}
			}
			if (LocalizationManager.HighlightLocalizedTargets)
			{
				MainTranslation = "LOC:" + FinalTerm;
			}
			mLocalizeTarget.DoLocalize(this, MainTranslation, SecondaryTranslation);
			CurrentLocalizeComponent = null;
		}

		public bool FindTarget()
		{
			if ((Object)(object)mLocalizeTarget != (Object)null && mLocalizeTarget.IsValid(this))
			{
				return true;
			}
			if ((Object)(object)mLocalizeTarget != (Object)null)
			{
				Object.DestroyImmediate((Object)(object)mLocalizeTarget);
				mLocalizeTarget = null;
				mLocalizeTargetName = null;
			}
			if (!string.IsNullOrEmpty(mLocalizeTargetName))
			{
				foreach (ILocalizeTargetDescriptor mLocalizeTarget in LocalizationManager.mLocalizeTargets)
				{
					if (mLocalizeTargetName == mLocalizeTarget.GetTargetType().ToString())
					{
						if (mLocalizeTarget.CanLocalize(this))
						{
							this.mLocalizeTarget = mLocalizeTarget.CreateTarget(this);
						}
						if ((Object)(object)this.mLocalizeTarget != (Object)null)
						{
							return true;
						}
					}
				}
			}
			foreach (ILocalizeTargetDescriptor mLocalizeTarget2 in LocalizationManager.mLocalizeTargets)
			{
				if (mLocalizeTarget2.CanLocalize(this))
				{
					this.mLocalizeTarget = mLocalizeTarget2.CreateTarget(this);
					mLocalizeTargetName = mLocalizeTarget2.GetTargetType().ToString();
					if ((Object)(object)this.mLocalizeTarget != (Object)null)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void GetFinalTerms(out string primaryTerm, out string secondaryTerm)
		{
			primaryTerm = string.Empty;
			secondaryTerm = string.Empty;
			if (FindTarget())
			{
				if ((Object)(object)mLocalizeTarget != (Object)null)
				{
					mLocalizeTarget.GetFinalTerms(this, mTerm, mTermSecondary, out primaryTerm, out secondaryTerm);
					primaryTerm = I2Utils.GetValidTermName(primaryTerm);
				}
				if (!string.IsNullOrEmpty(mTerm))
				{
					primaryTerm = mTerm;
				}
				if (!string.IsNullOrEmpty(mTermSecondary))
				{
					secondaryTerm = mTermSecondary;
				}
				if (primaryTerm != null)
				{
					primaryTerm = primaryTerm.Trim();
				}
				if (secondaryTerm != null)
				{
					secondaryTerm = secondaryTerm.Trim();
				}
			}
		}

		public string GetMainTargetsText()
		{
			string primaryTerm = null;
			string secondaryTerm = null;
			if ((Object)(object)mLocalizeTarget != (Object)null)
			{
				mLocalizeTarget.GetFinalTerms(this, null, null, out primaryTerm, out secondaryTerm);
			}
			if (!string.IsNullOrEmpty(primaryTerm))
			{
				return primaryTerm;
			}
			return mTerm;
		}

		public void SetFinalTerms(string Main, string Secondary, out string primaryTerm, out string secondaryTerm, bool RemoveNonASCII)
		{
			primaryTerm = (RemoveNonASCII ? I2Utils.GetValidTermName(Main) : Main);
			secondaryTerm = Secondary;
		}

		public void SetTerm(string primary)
		{
			if (!string.IsNullOrEmpty(primary))
			{
				FinalTerm = (mTerm = primary);
			}
			OnLocalize(Force: true);
		}

		public void SetTerm(string primary, string secondary)
		{
			if (!string.IsNullOrEmpty(primary))
			{
				FinalTerm = (mTerm = primary);
			}
			FinalSecondaryTerm = (mTermSecondary = secondary);
			OnLocalize(Force: true);
		}

		internal T GetSecondaryTranslatedObj<T>(ref string mainTranslation, ref string secondaryTranslation) where T : Object
		{
			DeserializeTranslation(mainTranslation, out var value, out var secondary);
			T val = default;
			if (!string.IsNullOrEmpty(secondary))
			{
				val = GetObject<T>(secondary);
				if ((Object)(object)val != (Object)null)
				{
					mainTranslation = value;
					secondaryTranslation = secondary;
				}
			}
			if ((Object)(object)val == (Object)null)
			{
				val = GetObject<T>(secondaryTranslation);
			}
			return val;
		}

		public void UpdateAssetDictionary()
		{
			TranslatedObjects.RemoveAll((Object x) => x == (Object)null);
			mAssetDictionary = (from o in TranslatedObjects.Distinct()
				group o by o.name).ToDictionary((IGrouping<string, Object> g) => g.Key, (IGrouping<string, Object> g) => g.First());
		}

		internal T GetObject<T>(string Translation) where T : Object
		{
			if (string.IsNullOrEmpty(Translation))
			{
				return default;
			}
			return GetTranslatedObject<T>(Translation);
		}

		private T GetTranslatedObject<T>(string Translation) where T : Object
		{
			return FindTranslatedObject<T>(Translation);
		}

		private void DeserializeTranslation(string translation, out string value, out string secondary)
		{
			if (!string.IsNullOrEmpty(translation) && translation.Length > 1 && translation[0] == '[')
			{
				int num = translation.IndexOf(']');
				if (num > 0)
				{
					secondary = translation.Substring(1, num - 1);
					value = translation.Substring(num + 1);
					return;
				}
			}
			value = translation;
			secondary = string.Empty;
		}

		public T FindTranslatedObject<T>(string value) where T : Object
		{
			if (string.IsNullOrEmpty(value))
			{
				return default;
			}
			if (mAssetDictionary == null || mAssetDictionary.Count != TranslatedObjects.Count)
			{
				UpdateAssetDictionary();
			}
			foreach (KeyValuePair<string, Object> item in mAssetDictionary)
			{
				if (item.Value is T && value.EndsWith(item.Key, StringComparison.OrdinalIgnoreCase) && string.Compare(value, item.Key, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return (T)(object)item.Value;
				}
			}
			Object val = LocalizationManager.FindAsset(value);
			T val2 = (T)(object)((val is T) ? val : null);
			if (Object.op_Implicit((Object)(object)val2))
			{
				return val2;
			}
			return ResourceManager.pInstance.GetAsset<T>(value);
		}

		public bool HasTranslatedObject(Object Obj)
		{
			if (TranslatedObjects.Contains(Obj))
			{
				return true;
			}
			return ResourceManager.pInstance.HasAsset(Obj);
		}

		public void AddTranslatedObject(Object Obj)
		{
			if (!TranslatedObjects.Contains(Obj))
			{
				TranslatedObjects.Add(Obj);
				UpdateAssetDictionary();
			}
		}

		public void SetGlobalLanguage(string Language)
		{
			LocalizationManager.CurrentLanguage = Language;
		}

		public Localize()
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected Obj, but got Unknown
		}
	}
	[AddComponentMenu("I2/Localization/Localize Dropdown")]
	public class LocalizeDropdown : MonoBehaviour
	{
		public List<string> _Terms = new List<string>();

		public void Start()
		{
			LocalizationManager.OnLocalizeEvent += OnLocalize;
			OnLocalize();
		}

		public void OnDestroy()
		{
			LocalizationManager.OnLocalizeEvent -= OnLocalize;
		}

		private void OnEnable()
		{
			if (_Terms.Count == 0)
			{
				FillValues();
			}
			OnLocalize();
		}

		public void OnLocalize()
		{
			if (((Behaviour)this).enabled && !((Object)(object)((Component)this).gameObject == (Object)null) && ((Component)this).gameObject.activeInHierarchy && !string.IsNullOrEmpty(LocalizationManager.CurrentLanguage))
			{
				UpdateLocalization();
			}
		}

		private void FillValues()
		{
			Dropdown component = ((Component)this).GetComponent<Dropdown>();
			if ((Object)(object)component == (Object)null && I2Utils.IsPlaying())
			{
				FillValuesTMPro();
				return;
			}
			foreach (OptionData option in component.options)
			{
				_Terms.Add(option.text);
			}
		}

		public void UpdateLocalization()
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected Obj, but got Unknown
			Dropdown component = ((Component)this).GetComponent<Dropdown>();
			if ((Object)(object)component == (Object)null)
			{
				UpdateLocalizationTMPro();
				return;
			}
			component.options.Clear();
			foreach (string term in _Terms)
			{
				string translation = LocalizationManager.GetTranslation(term);
				component.options.Add(new OptionData(translation));
			}
			component.RefreshShownValue();
		}

		public void UpdateLocalizationTMPro()
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Expected Obj, but got Unknown
			TMP_Dropdown component = ((Component)this).GetComponent<TMP_Dropdown>();
			if ((Object)(object)component == (Object)null)
			{
				return;
			}
			component.options.Clear();
			foreach (string term in _Terms)
			{
				string translation = LocalizationManager.GetTranslation(term);
				component.options.Add(new OptionData(translation));
			}
			component.RefreshShownValue();
		}

		private void FillValuesTMPro()
		{
			TMP_Dropdown component = ((Component)this).GetComponent<TMP_Dropdown>();
			if ((Object)(object)component == (Object)null)
			{
				return;
			}
			foreach (OptionData option in component.options)
			{
				_Terms.Add(option.text);
			}
		}
	}
	public static class LocalizationManager
	{
		public delegate object _GetParam(string param);

		public delegate void OnLocalizeCallback();

		private static string mCurrentLanguage;

		private static string mLanguageCode;

		private static CultureInfo mCurrentCulture;

		private static bool mChangeCultureInfo = false;

		public static bool IsRight2Left = false;

		public static bool HasJoinedWords = false;

		public static List<ILocalizationParamsManager> ParamManagers = new List<ILocalizationParamsManager>();

		private static string[] LanguagesRTL = new string[21]
		{
			"ar-DZ", "ar", "ar-BH", "ar-EG", "ar-IQ", "ar-JO", "ar-KW", "ar-LB", "ar-LY", "ar-MA",
			"ar-OM", "ar-QA", "ar-SA", "ar-SY", "ar-TN", "ar-AE", "ar-YE", "fa", "he", "ur",
			"ji"
		};

		public static List<LanguageSourceData> Sources = new List<LanguageSourceData>();

		public static string[] GlobalSources = new string[1] { "I2Languages" };

		private static string mCurrentDeviceLanguage;

		public static List<ILocalizeTargetDescriptor> mLocalizeTargets = new List<ILocalizeTargetDescriptor>();

		private static bool mLocalizeIsScheduled = false;

		private static bool mLocalizeIsScheduledWithForcedValue = false;

		public static bool HighlightLocalizedTargets = false;

		public static string CurrentLanguage
		{
			get
			{
				InitializeIfNeeded();
				return mCurrentLanguage;
			}
			set
			{
				InitializeIfNeeded();
				string supportedLanguage = GetSupportedLanguage(value);
				if (!string.IsNullOrEmpty(supportedLanguage) && mCurrentLanguage != supportedLanguage)
				{
					SetLanguageAndCode(supportedLanguage, GetLanguageCode(supportedLanguage));
				}
			}
		}

		public static string CurrentLanguageCode
		{
			get
			{
				InitializeIfNeeded();
				return mLanguageCode;
			}
			set
			{
				InitializeIfNeeded();
				if (mLanguageCode != value)
				{
					string languageFromCode = GetLanguageFromCode(value);
					if (!string.IsNullOrEmpty(languageFromCode))
					{
						SetLanguageAndCode(languageFromCode, value);
					}
				}
			}
		}

		public static string CurrentRegion
		{
			get
			{
				string currentLanguage = CurrentLanguage;
				int num = currentLanguage.IndexOfAny("/\\".ToCharArray());
				if (num > 0)
				{
					return currentLanguage.Substring(num + 1);
				}
				num = currentLanguage.IndexOfAny("[(".ToCharArray());
				int num2 = currentLanguage.LastIndexOfAny("])".ToCharArray());
				if (num > 0 && num != num2)
				{
					return currentLanguage.Substring(num + 1, num2 - num - 1);
				}
				return string.Empty;
			}
			set
			{
				string text = CurrentLanguage;
				int num = text.IndexOfAny("/\\".ToCharArray());
				if (num > 0)
				{
					CurrentLanguage = text.Substring(num + 1) + value;
					return;
				}
				num = text.IndexOfAny("[(".ToCharArray());
				int num2 = text.LastIndexOfAny("])".ToCharArray());
				if (num > 0 && num != num2)
				{
					text = text.Substring(num);
				}
				CurrentLanguage = text + "(" + value + ")";
			}
		}

		public static string CurrentRegionCode
		{
			get
			{
				string currentLanguageCode = CurrentLanguageCode;
				int num = currentLanguageCode.IndexOfAny(" -_/\\".ToCharArray());
				if (num >= 0)
				{
					return currentLanguageCode.Substring(num + 1);
				}
				return string.Empty;
			}
			set
			{
				string text = CurrentLanguageCode;
				int num = text.IndexOfAny(" -_/\\".ToCharArray());
				if (num > 0)
				{
					text = text.Substring(0, num);
				}
				CurrentLanguageCode = text + "-" + value;
			}
		}

		public static CultureInfo CurrentCulture => mCurrentCulture;

		public static event OnLocalizeCallback OnLocalizeEvent;

		public static void InitializeIfNeeded()
		{
			if (string.IsNullOrEmpty(mCurrentLanguage) || Sources.Count == 0)
			{
				AutoLoadGlobalParamManagers();
				UpdateSources();
				SelectStartupLanguage();
			}
		}

		public static string GetVersion()
		{
			return "2.8.13 f2";
		}

		public static int GetRequiredWebServiceVersion()
		{
			return 5;
		}

		public static string GetWebServiceURL(LanguageSourceData source = null)
		{
			if (source != null && !string.IsNullOrEmpty(source.Google_WebServiceURL))
			{
				return source.Google_WebServiceURL;
			}
			InitializeIfNeeded();
			for (int i = 0; i < Sources.Count; i++)
			{
				if (Sources[i] != null && !string.IsNullOrEmpty(Sources[i].Google_WebServiceURL))
				{
					return Sources[i].Google_WebServiceURL;
				}
			}
			return string.Empty;
		}

		public static void SetLanguageAndCode(string LanguageName, string LanguageCode, bool RememberLanguage = true, bool Force = false)
		{
			if ((mCurrentLanguage != LanguageName || mLanguageCode != LanguageCode) | Force)
			{
				if (RememberLanguage)
				{
					PersistentStorage.SetSetting_String("I2 Language", LanguageName);
				}
				mCurrentLanguage = LanguageName;
				mLanguageCode = LanguageCode;
				mCurrentCulture = CreateCultureForCode(LanguageCode);
				if (mChangeCultureInfo)
				{
					SetCurrentCultureInfo();
				}
				IsRight2Left = IsRTL(mLanguageCode);
				HasJoinedWords = GoogleLanguages.LanguageCode_HasJoinedWord(mLanguageCode);
				LocalizeAll(Force);
			}
		}

		private static CultureInfo CreateCultureForCode(string code)
		{
			try
			{
				return CultureInfo.CreateSpecificCulture(code);
			}
			catch (Exception)
			{
				return CultureInfo.InvariantCulture;
			}
		}

		public static void EnableChangingCultureInfo(bool bEnable)
		{
			if (!mChangeCultureInfo & bEnable)
			{
				SetCurrentCultureInfo();
			}
			mChangeCultureInfo = bEnable;
		}

		private static void SetCurrentCultureInfo()
		{
			Thread.CurrentThread.CurrentCulture = mCurrentCulture;
		}

		private static void SelectStartupLanguage()
		{
			if (Sources.Count == 0)
			{
				return;
			}
			string setting_String = PersistentStorage.GetSetting_String("I2 Language", string.Empty);
			string currentDeviceLanguage = GetCurrentDeviceLanguage();
			if (!string.IsNullOrEmpty(setting_String) && HasLanguage(setting_String, AllowDiscartingRegion: true, Initialize: false))
			{
				SetLanguageAndCode(setting_String, GetLanguageCode(setting_String));
				return;
			}
			if (!Sources[0].IgnoreDeviceLanguage)
			{
				string supportedLanguage = GetSupportedLanguage(currentDeviceLanguage, ignoreDisabled: true);
				if (!string.IsNullOrEmpty(supportedLanguage))
				{
					SetLanguageAndCode(supportedLanguage, GetLanguageCode(supportedLanguage), RememberLanguage: false);
					return;
				}
			}
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				if (Sources[i].mLanguages.Count <= 0)
				{
					continue;
				}
				for (int j = 0; j < Sources[i].mLanguages.Count; j++)
				{
					if (Sources[i].mLanguages[j].IsEnabled())
					{
						SetLanguageAndCode(Sources[i].mLanguages[j].Name, Sources[i].mLanguages[j].Code, RememberLanguage: false);
						return;
					}
				}
			}
		}

		public static bool HasLanguage(string Language, bool AllowDiscartingRegion = true, bool Initialize = true, bool SkipDisabled = true)
		{
			if (Initialize)
			{
				InitializeIfNeeded();
			}
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				if (Sources[i].GetLanguageIndex(Language, AllowDiscartingRegion: false, SkipDisabled) >= 0)
				{
					return true;
				}
			}
			if (AllowDiscartingRegion)
			{
				int j = 0;
				for (int count2 = Sources.Count; j < count2; j++)
				{
					if (Sources[j].GetLanguageIndex(Language, AllowDiscartingRegion: true, SkipDisabled) >= 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static string GetSupportedLanguage(string Language, bool ignoreDisabled = false)
		{
			string languageCode = GoogleLanguages.GetLanguageCode(Language);
			if (!string.IsNullOrEmpty(languageCode))
			{
				int i = 0;
				for (int count = Sources.Count; i < count; i++)
				{
					int languageIndexFromCode = Sources[i].GetLanguageIndexFromCode(languageCode, exactMatch: true, ignoreDisabled);
					if (languageIndexFromCode >= 0)
					{
						return Sources[i].mLanguages[languageIndexFromCode].Name;
					}
				}
				int j = 0;
				for (int count2 = Sources.Count; j < count2; j++)
				{
					int languageIndexFromCode2 = Sources[j].GetLanguageIndexFromCode(languageCode, exactMatch: false, ignoreDisabled);
					if (languageIndexFromCode2 >= 0)
					{
						return Sources[j].mLanguages[languageIndexFromCode2].Name;
					}
				}
			}
			int k = 0;
			for (int count3 = Sources.Count; k < count3; k++)
			{
				int languageIndex = Sources[k].GetLanguageIndex(Language, AllowDiscartingRegion: false, ignoreDisabled);
				if (languageIndex >= 0)
				{
					return Sources[k].mLanguages[languageIndex].Name;
				}
			}
			int l = 0;
			for (int count4 = Sources.Count; l < count4; l++)
			{
				int languageIndex2 = Sources[l].GetLanguageIndex(Language, AllowDiscartingRegion: true, ignoreDisabled);
				if (languageIndex2 >= 0)
				{
					return Sources[l].mLanguages[languageIndex2].Name;
				}
			}
			return string.Empty;
		}

		public static string GetLanguageCode(string Language)
		{
			if (Sources.Count == 0)
			{
				UpdateSources();
			}
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				int languageIndex = Sources[i].GetLanguageIndex(Language);
				if (languageIndex >= 0)
				{
					return Sources[i].mLanguages[languageIndex].Code;
				}
			}
			return string.Empty;
		}

		public static string GetLanguageFromCode(string Code, bool exactMatch = true)
		{
			if (Sources.Count == 0)
			{
				UpdateSources();
			}
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				int languageIndexFromCode = Sources[i].GetLanguageIndexFromCode(Code, exactMatch);
				if (languageIndexFromCode >= 0)
				{
					return Sources[i].mLanguages[languageIndexFromCode].Name;
				}
			}
			return string.Empty;
		}

		public static List<string> GetAllLanguages(bool SkipDisabled = true)
		{
			if (Sources.Count == 0)
			{
				UpdateSources();
			}
			List<string> Languages = new List<string>();
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				Languages.AddRange(from x in Sources[i].GetLanguages(SkipDisabled)
					where !Languages.Contains(x)
					select x);
			}
			return Languages;
		}

		public static List<string> GetAllLanguagesCode(bool allowRegions = true, bool SkipDisabled = true)
		{
			List<string> Languages = new List<string>();
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				Languages.AddRange(from x in Sources[i].GetLanguagesCode(allowRegions, SkipDisabled)
					where !Languages.Contains(x)
					select x);
			}
			return Languages;
		}

		public static bool IsLanguageEnabled(string Language)
		{
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				if (!Sources[i].IsLanguageEnabled(Language))
				{
					return false;
				}
			}
			return true;
		}

		private static void LoadCurrentLanguage()
		{
			for (int i = 0; i < Sources.Count; i++)
			{
				int languageIndex = Sources[i].GetLanguageIndex(mCurrentLanguage, AllowDiscartingRegion: true, SkipDisabled: false);
				Sources[i].LoadLanguage(languageIndex, UnloadOtherLanguages: true, useFallback: true, onlyCurrentSpecialization: true, forceLoad: false);
			}
		}

		public static void PreviewLanguage(string NewLanguage)
		{
			mCurrentLanguage = NewLanguage;
			mLanguageCode = GetLanguageCode(mCurrentLanguage);
			IsRight2Left = IsRTL(mLanguageCode);
			HasJoinedWords = GoogleLanguages.LanguageCode_HasJoinedWord(mLanguageCode);
		}

		public static void AutoLoadGlobalParamManagers()
		{
			LocalizationParamsManager[] array = Object.FindObjectsOfType<LocalizationParamsManager>();
			foreach (LocalizationParamsManager localizationParamsManager in array)
			{
				if (localizationParamsManager._IsGlobalManager && !ParamManagers.Contains(localizationParamsManager))
				{
					ParamManagers.Add(localizationParamsManager);
				}
			}
		}

		public static void ApplyLocalizationParams(ref string translation, bool allowLocalizedParameters = true)
		{
			ApplyLocalizationParams(ref translation, (_GetParam)((string p) => GetLocalizationParam(p, null)), allowLocalizedParameters);
		}

		public static void ApplyLocalizationParams(ref string translation, GameObject root, bool allowLocalizedParameters = true)
		{
			ApplyLocalizationParams(ref translation, (_GetParam)((string p) => GetLocalizationParam(p, root)), allowLocalizedParameters);
		}

		public static void ApplyLocalizationParams(ref string translation, Dictionary<string, object> parameters, bool allowLocalizedParameters = true)
		{
			ApplyLocalizationParams(ref translation, (string p) =>
			{
				object value = null;
				return parameters.TryGetValue(p, out value) ? value : null;
			}, allowLocalizedParameters);
		}

		public static void ApplyLocalizationParams(ref string translation, _GetParam getParam, bool allowLocalizedParameters = true)
		{
			if (translation == null)
			{
				return;
			}
			string text = null;
			int num = 0;
			int length = translation.Length;
			int num2 = 0;
			while (num2 >= 0 && num2 < translation.Length)
			{
				int num3 = translation.IndexOf("{[", num2);
				if (num3 < 0)
				{
					break;
				}
				int num4 = translation.IndexOf("]}", num3);
				if (num4 < 0)
				{
					break;
				}
				int num5 = translation.IndexOf("{[", num3 + 1);
				if (num5 > 0 && num5 < num4)
				{
					num2 = num5;
					continue;
				}
				int num6 = ((translation[num3 + 2] == '#') ? 3 : 2);
				string param = translation.Substring(num3 + num6, num4 - num3 - num6);
				string text2 = (string)getParam(param);
				if ((text2 != null) & allowLocalizedParameters)
				{
					TermData termData = GetTermData(text2, out var source);
					if (termData != null)
					{
						int languageIndex = source.GetLanguageIndex(CurrentLanguage);
						if (languageIndex >= 0)
						{
							text2 = termData.GetTranslation(languageIndex);
						}
					}
					string oldValue = translation.Substring(num3, num4 - num3 + 2);
					translation = translation.Replace(oldValue, text2);
					int result = 0;
					if (int.TryParse(text2, out result))
					{
						text = GoogleLanguages.GetPluralType(CurrentLanguageCode, result).ToString();
					}
					num2 = num3 + text2.Length;
				}
				else
				{
					num2 = num4 + 2;
				}
			}
			if (text != null)
			{
				string text3 = "[i2p_" + text + "]";
				num = translation.IndexOf(text3, StringComparison.OrdinalIgnoreCase);
				num = ((num >= 0) ? (num + text3.Length) : 0);
				length = translation.IndexOf("[i2p_", num + 1, StringComparison.OrdinalIgnoreCase);
				if (length < 0)
				{
					length = translation.Length;
				}
				translation = translation.Substring(num, length - num);
			}
		}

		internal static string GetLocalizationParam(string ParamName, GameObject root)
		{
			string text = null;
			if (Object.op_Implicit((Object)(object)root))
			{
				MonoBehaviour[] components = root.GetComponents<MonoBehaviour>();
				int i = 0;
				for (int num = components.Length; i < num; i++)
				{
					if (components[i] is ILocalizationParamsManager localizationParamsManager && ((Behaviour)components[i]).enabled)
					{
						text = localizationParamsManager.GetParameterValue(ParamName);
						if (text != null)
						{
							return text;
						}
					}
				}
			}
			int j = 0;
			for (int count = ParamManagers.Count; j < count; j++)
			{
				text = ParamManagers[j].GetParameterValue(ParamName);
				if (text != null)
				{
					return text;
				}
			}
			return null;
		}

		private static string GetPluralType(MatchCollection matches, string langCode, _GetParam getParam)
		{
			int i = 0;
			for (int count = matches.Count; i < count; i++)
			{
				Match match = matches[i];
				string value = match.Groups[match.Groups.Count - 1].Value;
				string text = (string)getParam(value);
				if (text != null)
				{
					int result = 0;
					if (int.TryParse(text, out result))
					{
						return GoogleLanguages.GetPluralType(langCode, result).ToString();
					}
				}
			}
			return null;
		}

		public static string ApplyRTLfix(string line)
		{
			return ApplyRTLfix(line, 0, ignoreNumbers: true);
		}

		public static string ApplyRTLfix(string line, int maxCharacters, bool ignoreNumbers)
		{
			if (string.IsNullOrEmpty(line))
			{
				return line;
			}
			char c = line[0];
			if (c == '!' || c == '.' || c == '?')
			{
				line = line.Substring(1) + c;
			}
			int tagStart = -1;
			int num = 0;
			int num2 = 40000;
			num = 0;
			List<string> list = new List<string>();
			while (I2Utils.FindNextTag(line, num, out tagStart, out num))
			{
				string text = "@@" + (char)(num2 + list.Count) + "@@";
				list.Add(line.Substring(tagStart, num - tagStart + 1));
				line = line.Substring(0, tagStart) + text + line.Substring(num + 1);
				num = tagStart + 5;
			}
			line = line.Replace("\r\n", "\n");
			line = I2Utils.SplitLine(line, maxCharacters);
			line = RTLFixer.Fix(line, showTashkeel: true, !ignoreNumbers);
			for (int i = 0; i < list.Count; i++)
			{
				int length = line.Length;
				for (int j = 0; j < length; j++)
				{
					if (line[j] == '@' && line[j + 1] == '@' && line[j + 2] >= num2 && line[j + 3] == '@' && line[j + 4] == '@')
					{
						int num3 = line[j + 2] - num2;
						num3 = ((num3 % 2 != 0) ? (num3 - 1) : (num3 + 1));
						if (num3 >= list.Count)
						{
							num3 = list.Count - 1;
						}
						line = line.Substring(0, j) + list[num3] + line.Substring(j + 5);
						break;
					}
				}
			}
			return line;
		}

		public static string FixRTL_IfNeeded(string text, int maxCharacters = 0, bool ignoreNumber = false)
		{
			if (IsRight2Left)
			{
				return ApplyRTLfix(text, maxCharacters, ignoreNumber);
			}
			return text;
		}

		public static bool IsRTL(string Code)
		{
			return Array.IndexOf(LanguagesRTL, Code) >= 0;
		}

		public static bool UpdateSources()
		{
			UnregisterDeletededSources();
			RegisterSourceInResources();
			RegisterSceneSources();
			return Sources.Count > 0;
		}

		private static void UnregisterDeletededSources()
		{
			for (int num = Sources.Count - 1; num >= 0; num--)
			{
				if (Sources[num] == null)
				{
					RemoveSource(Sources[num]);
				}
			}
		}

		private static void RegisterSceneSources()
		{
			LanguageSource[] array = (LanguageSource[])(object)Resources.FindObjectsOfTypeAll(typeof(LanguageSource));
			foreach (LanguageSource languageSource in array)
			{
				if (!Sources.Contains(languageSource.mSource))
				{
					if (languageSource.mSource.owner == null)
					{
						languageSource.mSource.owner = languageSource;
					}
					AddSource(languageSource.mSource);
				}
			}
		}

		private static void RegisterSourceInResources()
		{
			string[] globalSources = GlobalSources;
			foreach (string name in globalSources)
			{
				LanguageSourceAsset asset = ResourceManager.pInstance.GetAsset<LanguageSourceAsset>(name);
				if (Object.op_Implicit((Object)(object)asset) && !Sources.Contains(asset.mSource))
				{
					if (!asset.mSource.mIsGlobalSource)
					{
						asset.mSource.mIsGlobalSource = true;
					}
					asset.mSource.owner = asset;
					AddSource(asset.mSource);
				}
			}
		}

		internal static void AddSource(LanguageSourceData Source)
		{
			if (Sources.Contains(Source))
			{
				return;
			}
			Sources.Add(Source);
			if (Source.HasGoogleSpreadsheet() && Source.GoogleUpdateFrequency != LanguageSourceData.eGoogleUpdateFrequency.Never)
			{
				Source.Import_Google_FromCache();
				bool justCheck = false;
				if (Source.GoogleUpdateDelay > 0f)
				{
					CoroutineManager.Start(Delayed_Import_Google(Source, Source.GoogleUpdateDelay, justCheck));
				}
				else
				{
					Source.Import_Google(ForceUpdate: false, justCheck);
				}
			}
			for (int i = 0; i < Source.mLanguages.Count(); i++)
			{
				Source.mLanguages[i].SetLoaded(loaded: true);
			}
			if (Source.mDictionary.Count == 0)
			{
				Source.UpdateDictionary(force: true);
			}
		}

		private static IEnumerator Delayed_Import_Google(LanguageSourceData source, float delay, bool justCheck)
		{
			yield return (object)new WaitForSeconds(delay);
			source?.Import_Google(ForceUpdate: false, justCheck);
		}

		internal static void RemoveSource(LanguageSourceData Source)
		{
			Sources.Remove(Source);
		}

		public static bool IsGlobalSource(string SourceName)
		{
			return Array.IndexOf(GlobalSources, SourceName) >= 0;
		}

		public static LanguageSourceData GetSourceContaining(string term, bool fallbackToFirst = true)
		{
			if (!string.IsNullOrEmpty(term))
			{
				int i = 0;
				for (int count = Sources.Count; i < count; i++)
				{
					if (Sources[i].GetTermData(term) != null)
					{
						return Sources[i];
					}
				}
			}
			if (!fallbackToFirst || Sources.Count <= 0)
			{
				return null;
			}
			return Sources[0];
		}

		public static Object FindAsset(string value)
		{
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				Object val = Sources[i].FindAsset(value);
				if (Object.op_Implicit(val))
				{
					return val;
				}
			}
			return null;
		}

		public static void ApplyDownloadedDataFromGoogle()
		{
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				Sources[i].ApplyDownloadedDataFromGoogle();
			}
		}

		public static string GetCurrentDeviceLanguage(bool force = false)
		{
			if (force || string.IsNullOrEmpty(mCurrentDeviceLanguage))
			{
				DetectDeviceLanguage();
			}
			return mCurrentDeviceLanguage;
		}

		private static void DetectDeviceLanguage()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			mCurrentDeviceLanguage = ((object)Application.systemLanguage/*cast due to constrained. prefix*/).ToString();
			if (mCurrentDeviceLanguage == "ChineseSimplified")
			{
				mCurrentDeviceLanguage = "Chinese (Simplified)";
			}
			if (mCurrentDeviceLanguage == "ChineseTraditional")
			{
				mCurrentDeviceLanguage = "Chinese (Traditional)";
			}
		}

		public static void RegisterTarget(ILocalizeTargetDescriptor desc)
		{
			if (mLocalizeTargets.FindIndex((ILocalizeTargetDescriptor x) => x.Name == desc.Name) != -1)
			{
				return;
			}
			for (int num = 0; num < mLocalizeTargets.Count; num++)
			{
				if (mLocalizeTargets[num].Priority > desc.Priority)
				{
					mLocalizeTargets.Insert(num, desc);
					return;
				}
			}
			mLocalizeTargets.Add(desc);
		}

		public static string GetTranslation(string Term, bool FixForRTL = true, int maxLineLengthForRTL = 0, bool ignoreRTLnumbers = true, bool applyParameters = false, GameObject localParametersRoot = null, string overrideLanguage = null)
		{
			string Translation = null;
			TryGetTranslation(Term, out Translation, FixForRTL, maxLineLengthForRTL, ignoreRTLnumbers, applyParameters, localParametersRoot, overrideLanguage);
			return Translation;
		}

		public static string GetTermTranslation(string Term, bool FixForRTL = true, int maxLineLengthForRTL = 0, bool ignoreRTLnumbers = true, bool applyParameters = false, GameObject localParametersRoot = null, string overrideLanguage = null)
		{
			return GetTranslation(Term, FixForRTL, maxLineLengthForRTL, ignoreRTLnumbers, applyParameters, localParametersRoot, overrideLanguage);
		}

		public static bool TryGetTranslation(string Term, out string Translation, bool FixForRTL = true, int maxLineLengthForRTL = 0, bool ignoreRTLnumbers = true, bool applyParameters = false, GameObject localParametersRoot = null, string overrideLanguage = null)
		{
			Translation = null;
			if (string.IsNullOrEmpty(Term))
			{
				return false;
			}
			InitializeIfNeeded();
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				if (Sources[i].TryGetTranslation(Term, out Translation, overrideLanguage))
				{
					if (applyParameters)
					{
						ApplyLocalizationParams(ref Translation, localParametersRoot);
					}
					if (IsRight2Left & FixForRTL)
					{
						Translation = ApplyRTLfix(Translation, maxLineLengthForRTL, ignoreRTLnumbers);
					}
					return true;
				}
			}
			return false;
		}

		public static T GetTranslatedObject<T>(string AssetName, Localize optionalLocComp = null) where T : Object
		{
			if ((Object)(object)optionalLocComp != (Object)null)
			{
				return optionalLocComp.FindTranslatedObject<T>(AssetName);
			}
			Object val = FindAsset(AssetName);
			T val2 = (T)(object)((val is T) ? val : null);
			if (Object.op_Implicit((Object)(object)val2))
			{
				return val2;
			}
			return ResourceManager.pInstance.GetAsset<T>(AssetName);
		}

		public static T GetTranslatedObjectByTermName<T>(string Term, Localize optionalLocComp = null) where T : Object
		{
			return GetTranslatedObject<T>(GetTranslation(Term, FixForRTL: false));
		}

		public static string GetAppName(string languageCode)
		{
			if (!string.IsNullOrEmpty(languageCode))
			{
				for (int i = 0; i < Sources.Count; i++)
				{
					if (string.IsNullOrEmpty(Sources[i].mTerm_AppName))
					{
						continue;
					}
					int languageIndexFromCode = Sources[i].GetLanguageIndexFromCode(languageCode, exactMatch: false);
					if (languageIndexFromCode < 0)
					{
						continue;
					}
					TermData termData = Sources[i].GetTermData(Sources[i].mTerm_AppName);
					if (termData != null)
					{
						string translation = termData.GetTranslation(languageIndexFromCode);
						if (!string.IsNullOrEmpty(translation))
						{
							return translation;
						}
					}
				}
			}
			return Application.productName;
		}

		public static void LocalizeAll(bool Force = false)
		{
			LoadCurrentLanguage();
			if (!Application.isPlaying)
			{
				DoLocalizeAll(Force);
				return;
			}
			mLocalizeIsScheduledWithForcedValue |= Force;
			if (!mLocalizeIsScheduled)
			{
				CoroutineManager.Start(Coroutine_LocalizeAll());
			}
		}

		private static IEnumerator Coroutine_LocalizeAll()
		{
			mLocalizeIsScheduled = true;
			yield return null;
			mLocalizeIsScheduled = false;
			bool force = mLocalizeIsScheduledWithForcedValue;
			mLocalizeIsScheduledWithForcedValue = false;
			DoLocalizeAll(force);
		}

		private static void DoLocalizeAll(bool Force = false)
		{
			Localize[] array = (Localize[])(object)Resources.FindObjectsOfTypeAll(typeof(Localize));
			int i = 0;
			for (int num = array.Length; i < num; i++)
			{
				array[i].OnLocalize(Force);
			}
			if (OnLocalizeEvent != null)
			{
				OnLocalizeEvent();
			}
		}

		public static List<string> GetCategories()
		{
			List<string> list = new List<string>();
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				Sources[i].GetCategories(OnlyMainCategory: false, list);
			}
			return list;
		}

		public static List<string> GetTermsList(string Category = null)
		{
			if (Sources.Count == 0)
			{
				UpdateSources();
			}
			if (Sources.Count == 1)
			{
				return Sources[0].GetTermsList(Category);
			}
			HashSet<string> hashSet = new HashSet<string>();
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				hashSet.UnionWith(Sources[i].GetTermsList(Category));
			}
			return new List<string>(hashSet);
		}

		public static TermData GetTermData(string term)
		{
			InitializeIfNeeded();
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				TermData termData = Sources[i].GetTermData(term);
				if (termData != null)
				{
					return termData;
				}
			}
			return null;
		}

		public static TermData GetTermData(string term, out LanguageSourceData source)
		{
			InitializeIfNeeded();
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				TermData termData = Sources[i].GetTermData(term);
				if (termData != null)
				{
					source = Sources[i];
					return termData;
				}
			}
			source = null;
			return null;
		}
	}
	public abstract class ILocalizeTarget : ScriptableObject
	{
		public abstract bool IsValid(Localize cmp);

		public abstract void GetFinalTerms(Localize cmp, string Main, string Secondary, out string primaryTerm, out string secondaryTerm);

		public abstract void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation);

		public abstract bool CanUseSecondaryTerm();

		public abstract bool AllowMainTermToBeRTL();

		public abstract bool AllowSecondTermToBeRTL();

		public abstract eTermType GetPrimaryTermType(Localize cmp);

		public abstract eTermType GetSecondaryTermType(Localize cmp);
	}
	public abstract class LocalizeTarget<T> : ILocalizeTarget where T : Object
	{
		public T mTarget;

		public override bool IsValid(Localize cmp)
		{
			if ((Object)(object)mTarget != (Object)null)
			{
				object obj = mTarget;
				Component val = (Component)((obj is Component) ? obj : null);
				if ((Object)(object)val != (Object)null && (Object)(object)val.gameObject != (Object)(object)((Component)cmp).gameObject)
				{
					mTarget = default;
				}
			}
			if ((Object)(object)mTarget == (Object)null)
			{
				mTarget = ((Component)cmp).GetComponent<T>();
			}
			return (Object)(object)mTarget != (Object)null;
		}
		public abstract override void GetFinalTerms(Localize cmp, string Main, string Secondary, out string primaryTerm, out string secondaryTerm);
		public abstract override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation);
		public abstract override bool CanUseSecondaryTerm();
		public abstract override bool AllowMainTermToBeRTL();
		public abstract override bool AllowSecondTermToBeRTL();
		public abstract override eTermType GetPrimaryTermType(Localize cmp);
		public abstract override eTermType GetSecondaryTermType(Localize cmp);
	}
	public abstract class ILocalizeTargetDescriptor
	{
		public string Name;

		public int Priority;

		public abstract bool CanLocalize(Localize cmp);

		public abstract ILocalizeTarget CreateTarget(Localize cmp);

		public abstract Type GetTargetType();
	}
	public abstract class LocalizeTargetDesc<T> : ILocalizeTargetDescriptor where T : ILocalizeTarget
	{
		public override ILocalizeTarget CreateTarget(Localize cmp)
		{
			return ScriptableObject.CreateInstance<T>();
		}

		public override Type GetTargetType()
		{
			return typeof(T);
		}
	}
	public class LocalizeTargetDesc_Type<T, G> : LocalizeTargetDesc<G> where T : Object where G : LocalizeTarget<T>
	{
		public override bool CanLocalize(Localize cmp)
		{
			return (Object)(object)((Component)cmp).GetComponent<T>() != (Object)null;
		}

		public override ILocalizeTarget CreateTarget(Localize cmp)
		{
			T component = ((Component)cmp).GetComponent<T>();
			if ((Object)(object)component == (Object)null)
			{
				return null;
			}
			G val = ScriptableObject.CreateInstance<G>();
			val.mTarget = component;
			return val;
		}
	}
	public class LocalizeTarget_TextMeshPro_Label : LocalizeTarget<TextMeshPro>
	{
		private TextAlignmentOptions mAlignment_RTL = (TextAlignmentOptions)516;

		private TextAlignmentOptions mAlignment_LTR = (TextAlignmentOptions)513;

		private bool mAlignmentWasRTL;

		private bool mInitializeAlignment = true;

		static LocalizeTarget_TextMeshPro_Label()
		{
			AutoRegister();
		}

		[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
		private static void AutoRegister()
		{
			LocalizationManager.RegisterTarget(new LocalizeTargetDesc_Type<TextMeshPro, LocalizeTarget_TextMeshPro_Label>
			{
				Name = "TextMeshPro Label",
				Priority = 100
			});
		}

		public override eTermType GetPrimaryTermType(Localize cmp)
		{
			return eTermType.Text;
		}

		public override eTermType GetSecondaryTermType(Localize cmp)
		{
			return eTermType.Font;
		}

		public override bool CanUseSecondaryTerm()
		{
			return true;
		}

		public override bool AllowMainTermToBeRTL()
		{
			return true;
		}

		public override bool AllowSecondTermToBeRTL()
		{
			return false;
		}

		public override void GetFinalTerms(Localize cmp, string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			primaryTerm = (Object.op_Implicit((Object)(object)mTarget) ? ((TMP_Text)mTarget).text : null);
			secondaryTerm = (((Object)(object)((TMP_Text)mTarget).font != (Object)null) ? ((Object)((TMP_Text)mTarget).font).name : string.Empty);
		}

		public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
		{
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			TMP_FontAsset secondaryTranslatedObj = cmp.GetSecondaryTranslatedObj<TMP_FontAsset>(ref mainTranslation, ref secondaryTranslation);
			if ((Object)(object)secondaryTranslatedObj != (Object)null)
			{
				SetFont((TMP_Text)(object)mTarget, secondaryTranslatedObj);
			}
			else
			{
				Material secondaryTranslatedObj2 = cmp.GetSecondaryTranslatedObj<Material>(ref mainTranslation, ref secondaryTranslation);
				if ((Object)(object)secondaryTranslatedObj2 != (Object)null && (Object)(object)((TMP_Text)mTarget).fontMaterial != (Object)(object)secondaryTranslatedObj2)
				{
					if (!((Object)secondaryTranslatedObj2).name.StartsWith(((Object)((TMP_Text)mTarget).font).name, StringComparison.Ordinal))
					{
						secondaryTranslatedObj = GetTMPFontFromMaterial(cmp, secondaryTranslation.EndsWith(((Object)secondaryTranslatedObj2).name, StringComparison.Ordinal) ? secondaryTranslation : ((Object)secondaryTranslatedObj2).name);
						if ((Object)(object)secondaryTranslatedObj != (Object)null)
						{
							SetFont((TMP_Text)(object)mTarget, secondaryTranslatedObj);
						}
					}
					SetMaterial((TMP_Text)(object)mTarget, secondaryTranslatedObj2);
				}
			}
			if (mInitializeAlignment)
			{
				mInitializeAlignment = false;
				mAlignmentWasRTL = LocalizationManager.IsRight2Left;
				InitAlignment_TMPro(mAlignmentWasRTL, ((TMP_Text)mTarget).alignment, out mAlignment_LTR, out mAlignment_RTL);
			}
			else
			{
				InitAlignment_TMPro(mAlignmentWasRTL, ((TMP_Text)mTarget).alignment, out var alignLTR, out var alignRTL);
				if ((mAlignmentWasRTL && mAlignment_RTL != alignRTL) || (!mAlignmentWasRTL && mAlignment_LTR != alignLTR))
				{
					mAlignment_LTR = alignLTR;
					mAlignment_RTL = alignRTL;
				}
				mAlignmentWasRTL = LocalizationManager.IsRight2Left;
			}
			if (mainTranslation == null || !(((TMP_Text)mTarget).text != mainTranslation))
			{
				return;
			}
			if (mainTranslation != null && cmp.CorrectAlignmentForRTL)
			{
				((TMP_Text)mTarget).alignment = (LocalizationManager.IsRight2Left ? mAlignment_RTL : mAlignment_LTR);
				((TMP_Text)mTarget).isRightToLeftText = LocalizationManager.IsRight2Left;
				if (LocalizationManager.IsRight2Left)
				{
					mainTranslation = I2Utils.ReverseText(mainTranslation);
				}
			}
			((TMP_Text)mTarget).text = mainTranslation;
		}

		internal static TMP_FontAsset GetTMPFontFromMaterial(Localize cmp, string matName)
		{
			string text = " .\\/-[]()";
			int num = matName.Length - 1;
			while (num > 0)
			{
				while (num > 0 && text.IndexOf(matName[num]) >= 0)
				{
					num--;
				}
				if (num <= 0)
				{
					break;
				}
				string translation = matName.Substring(0, num + 1);
				TMP_FontAsset val = cmp.GetObject<TMP_FontAsset>(translation);
				if ((Object)(object)val != (Object)null)
				{
					return val;
				}
				while (num > 0 && text.IndexOf(matName[num]) < 0)
				{
					num--;
				}
			}
			return null;
		}

		internal static void InitAlignment_TMPro(bool isRTL, TextAlignmentOptions alignment, out TextAlignmentOptions alignLTR, out TextAlignmentOptions alignRTL)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected I4, but got Unknown
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected I4, but got Unknown
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Invalid comparison between Unknown and I4
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Invalid comparison between Unknown and I4
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Invalid comparison between Unknown and I4
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Invalid comparison between Unknown and I4
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Invalid comparison between Unknown and I4
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Invalid comparison between Unknown and I4
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Invalid comparison between Unknown and I4
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Invalid comparison between Unknown and I4
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Invalid comparison between Unknown and I4
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Invalid comparison between Unknown and I4
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Invalid comparison between Unknown and I4
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Invalid comparison between Unknown and I4
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Invalid comparison between Unknown and I4
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Invalid comparison between Unknown and I4
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Invalid comparison between Unknown and I4
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Invalid comparison between Unknown and I4
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Invalid comparison between Unknown and I4
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Invalid comparison between Unknown and I4
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Invalid comparison between Unknown and I4
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Invalid comparison between Unknown and I4
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Invalid comparison between Unknown and I4
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Invalid comparison between Unknown and I4
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Invalid comparison between Unknown and I4
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Invalid comparison between Unknown and I4
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Invalid comparison between Unknown and I4
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Invalid comparison between Unknown and I4
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Invalid comparison between Unknown and I4
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Invalid comparison between Unknown and I4
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Invalid comparison between Unknown and I4
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Invalid comparison between Unknown and I4
			TextAlignmentOptions val;
			alignRTL = (TextAlignmentOptions)(int)(val = alignment);
			alignLTR = (TextAlignmentOptions)(int)val;
			if (isRTL)
			{
				if ((int)alignment <= 1028)
				{
					if ((int)alignment <= 513)
					{
						if ((int)alignment != 257)
						{
							if ((int)alignment != 260)
							{
								if ((int)alignment == 513)
								{
									alignLTR = (TextAlignmentOptions)516;
								}
							}
							else
							{
								alignLTR = (TextAlignmentOptions)257;
							}
						}
						else
						{
							alignLTR = (TextAlignmentOptions)260;
						}
					}
					else if ((int)alignment != 516)
					{
						if ((int)alignment != 1025)
						{
							if ((int)alignment == 1028)
							{
								alignLTR = (TextAlignmentOptions)1025;
							}
						}
						else
						{
							alignLTR = (TextAlignmentOptions)1028;
						}
					}
					else
					{
						alignLTR = (TextAlignmentOptions)513;
					}
				}
				else if ((int)alignment <= 4097)
				{
					if ((int)alignment != 2049)
					{
						if ((int)alignment != 2052)
						{
							if ((int)alignment == 4097)
							{
								alignLTR = (TextAlignmentOptions)4100;
							}
						}
						else
						{
							alignLTR = (TextAlignmentOptions)2049;
						}
					}
					else
					{
						alignLTR = (TextAlignmentOptions)2052;
					}
				}
				else if ((int)alignment != 4100)
				{
					if ((int)alignment != 8193)
					{
						if ((int)alignment == 8196)
						{
							alignLTR = (TextAlignmentOptions)8193;
						}
					}
					else
					{
						alignLTR = (TextAlignmentOptions)8196;
					}
				}
				else
				{
					alignLTR = (TextAlignmentOptions)4097;
				}
			}
			else if ((int)alignment <= 1028)
			{
				if ((int)alignment <= 513)
				{
					if ((int)alignment != 257)
					{
						if ((int)alignment != 260)
						{
							if ((int)alignment == 513)
							{
								alignRTL = (TextAlignmentOptions)516;
							}
						}
						else
						{
							alignRTL = (TextAlignmentOptions)257;
						}
					}
					else
					{
						alignRTL = (TextAlignmentOptions)260;
					}
				}
				else if ((int)alignment != 516)
				{
					if ((int)alignment != 1025)
					{
						if ((int)alignment == 1028)
						{
							alignRTL = (TextAlignmentOptions)1025;
						}
					}
					else
					{
						alignRTL = (TextAlignmentOptions)1028;
					}
				}
				else
				{
					alignRTL = (TextAlignmentOptions)513;
				}
			}
			else if ((int)alignment <= 4097)
			{
				if ((int)alignment != 2049)
				{
					if ((int)alignment != 2052)
					{
						if ((int)alignment == 4097)
						{
							alignRTL = (TextAlignmentOptions)4100;
						}
					}
					else
					{
						alignRTL = (TextAlignmentOptions)2049;
					}
				}
				else
				{
					alignRTL = (TextAlignmentOptions)2052;
				}
			}
			else if ((int)alignment != 4100)
			{
				if ((int)alignment != 8193)
				{
					if ((int)alignment == 8196)
					{
						alignRTL = (TextAlignmentOptions)8193;
					}
				}
				else
				{
					alignRTL = (TextAlignmentOptions)8196;
				}
			}
			else
			{
				alignRTL = (TextAlignmentOptions)4097;
			}
		}

		internal static void SetFont(TMP_Text label, TMP_FontAsset newFont)
		{
			if ((Object)(object)label.font != (Object)(object)newFont)
			{
				label.font = newFont;
			}
			if ((Object)(object)label.linkedTextComponent != (Object)null)
			{
				SetFont(label.linkedTextComponent, newFont);
			}
		}

		internal static void SetMaterial(TMP_Text label, Material newMat)
		{
			if ((Object)(object)label.fontSharedMaterial != (Object)(object)newMat)
			{
				label.fontSharedMaterial = newMat;
			}
			if ((Object)(object)label.linkedTextComponent != (Object)null)
			{
				SetMaterial(label.linkedTextComponent, newMat);
			}
		}

		public LocalizeTarget_TextMeshPro_Label()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		}
	}
	public class LocalizeTarget_TextMeshPro_UGUI : LocalizeTarget<TextMeshProUGUI>
	{
		public TextAlignmentOptions mAlignment_RTL = (TextAlignmentOptions)516;

		public TextAlignmentOptions mAlignment_LTR = (TextAlignmentOptions)513;

		public bool mAlignmentWasRTL;

		public bool mInitializeAlignment = true;

		static LocalizeTarget_TextMeshPro_UGUI()
		{
			AutoRegister();
		}

		[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
		private static void AutoRegister()
		{
			LocalizationManager.RegisterTarget(new LocalizeTargetDesc_Type<TextMeshProUGUI, LocalizeTarget_TextMeshPro_UGUI>
			{
				Name = "TextMeshPro UGUI",
				Priority = 100
			});
		}

		public override eTermType GetPrimaryTermType(Localize cmp)
		{
			return eTermType.Text;
		}

		public override eTermType GetSecondaryTermType(Localize cmp)
		{
			return eTermType.TextMeshPFont;
		}

		public override bool CanUseSecondaryTerm()
		{
			return true;
		}

		public override bool AllowMainTermToBeRTL()
		{
			return true;
		}

		public override bool AllowSecondTermToBeRTL()
		{
			return false;
		}

		public override void GetFinalTerms(Localize cmp, string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			primaryTerm = (Object.op_Implicit((Object)(object)mTarget) ? ((TMP_Text)mTarget).text : null);
			secondaryTerm = (((Object)(object)((TMP_Text)mTarget).font != (Object)null) ? ((Object)((TMP_Text)mTarget).font).name : string.Empty);
		}

		public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
		{
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			TMP_FontAsset secondaryTranslatedObj = cmp.GetSecondaryTranslatedObj<TMP_FontAsset>(ref mainTranslation, ref secondaryTranslation);
			if ((Object)(object)secondaryTranslatedObj != (Object)null)
			{
				LocalizeTarget_TextMeshPro_Label.SetFont((TMP_Text)(object)mTarget, secondaryTranslatedObj);
			}
			else
			{
				Material secondaryTranslatedObj2 = cmp.GetSecondaryTranslatedObj<Material>(ref mainTranslation, ref secondaryTranslation);
				if ((Object)(object)secondaryTranslatedObj2 != (Object)null && (Object)(object)((TMP_Text)mTarget).fontMaterial != (Object)(object)secondaryTranslatedObj2)
				{
					if (!((Object)secondaryTranslatedObj2).name.StartsWith(((Object)((TMP_Text)mTarget).font).name, StringComparison.Ordinal))
					{
						secondaryTranslatedObj = LocalizeTarget_TextMeshPro_Label.GetTMPFontFromMaterial(cmp, secondaryTranslation.EndsWith(((Object)secondaryTranslatedObj2).name, StringComparison.Ordinal) ? secondaryTranslation : ((Object)secondaryTranslatedObj2).name);
						if ((Object)(object)secondaryTranslatedObj != (Object)null)
						{
							LocalizeTarget_TextMeshPro_Label.SetFont((TMP_Text)(object)mTarget, secondaryTranslatedObj);
						}
					}
					LocalizeTarget_TextMeshPro_Label.SetMaterial((TMP_Text)(object)mTarget, secondaryTranslatedObj2);
				}
			}
			if (mInitializeAlignment)
			{
				mInitializeAlignment = false;
				mAlignmentWasRTL = LocalizationManager.IsRight2Left;
				LocalizeTarget_TextMeshPro_Label.InitAlignment_TMPro(mAlignmentWasRTL, ((TMP_Text)mTarget).alignment, out mAlignment_LTR, out mAlignment_RTL);
			}
			else
			{
				LocalizeTarget_TextMeshPro_Label.InitAlignment_TMPro(mAlignmentWasRTL, ((TMP_Text)mTarget).alignment, out var alignLTR, out var alignRTL);
				if ((mAlignmentWasRTL && mAlignment_RTL != alignRTL) || (!mAlignmentWasRTL && mAlignment_LTR != alignLTR))
				{
					mAlignment_LTR = alignLTR;
					mAlignment_RTL = alignRTL;
				}
				mAlignmentWasRTL = LocalizationManager.IsRight2Left;
			}
			if (mainTranslation == null || !(((TMP_Text)mTarget).text != mainTranslation))
			{
				return;
			}
			if (mainTranslation != null && cmp.CorrectAlignmentForRTL)
			{
				((TMP_Text)mTarget).alignment = (LocalizationManager.IsRight2Left ? mAlignment_RTL : mAlignment_LTR);
				((TMP_Text)mTarget).isRightToLeftText = LocalizationManager.IsRight2Left;
				if (LocalizationManager.IsRight2Left)
				{
					mainTranslation = I2Utils.ReverseText(mainTranslation);
				}
			}
			((TMP_Text)mTarget).text = mainTranslation;
		}

		public LocalizeTarget_TextMeshPro_UGUI()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		}
	}
	public class LocalizeTarget_UnityStandard_AudioSource : LocalizeTarget<AudioSource>
	{
		static LocalizeTarget_UnityStandard_AudioSource()
		{
			AutoRegister();
		}

		[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
		private static void AutoRegister()
		{
			LocalizationManager.RegisterTarget(new LocalizeTargetDesc_Type<AudioSource, LocalizeTarget_UnityStandard_AudioSource>
			{
				Name = "AudioSource",
				Priority = 100
			});
		}

		public override eTermType GetPrimaryTermType(Localize cmp)
		{
			return eTermType.AudioClip;
		}

		public override eTermType GetSecondaryTermType(Localize cmp)
		{
			return eTermType.Text;
		}

		public override bool CanUseSecondaryTerm()
		{
			return false;
		}

		public override bool AllowMainTermToBeRTL()
		{
			return false;
		}

		public override bool AllowSecondTermToBeRTL()
		{
			return false;
		}

		public override void GetFinalTerms(Localize cmp, string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			primaryTerm = (Object.op_Implicit((Object)(object)mTarget.clip) ? ((Object)mTarget.clip).name : string.Empty);
			secondaryTerm = null;
		}

		public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
		{
			bool num = (mTarget.isPlaying || mTarget.loop) && Application.isPlaying;
			AudioClip clip = mTarget.clip;
			AudioClip val = cmp.FindTranslatedObject<AudioClip>(mainTranslation);
			if ((Object)(object)clip != (Object)(object)val)
			{
				mTarget.clip = val;
			}
			if (num && Object.op_Implicit((Object)(object)mTarget.clip))
			{
				mTarget.Play();
			}
		}
	}
	public class LocalizeTargetDesc_Child : LocalizeTargetDesc<LocalizeTarget_UnityStandard_Child>
	{
		public override bool CanLocalize(Localize cmp)
		{
			return ((Component)cmp).transform.childCount > 1;
		}
	}
	public class LocalizeTarget_UnityStandard_Child : LocalizeTarget<GameObject>
	{
		static LocalizeTarget_UnityStandard_Child()
		{
			AutoRegister();
		}

		[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
		private static void AutoRegister()
		{
			LocalizationManager.RegisterTarget(new LocalizeTargetDesc_Child
			{
				Name = "Child",
				Priority = 200
			});
		}

		public override bool IsValid(Localize cmp)
		{
			return ((Component)cmp).transform.childCount > 1;
		}

		public override eTermType GetPrimaryTermType(Localize cmp)
		{
			return eTermType.GameObject;
		}

		public override eTermType GetSecondaryTermType(Localize cmp)
		{
			return eTermType.Text;
		}

		public override bool CanUseSecondaryTerm()
		{
			return false;
		}

		public override bool AllowMainTermToBeRTL()
		{
			return false;
		}

		public override bool AllowSecondTermToBeRTL()
		{
			return false;
		}

		public override void GetFinalTerms(Localize cmp, string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			primaryTerm = ((Object)cmp).name;
			secondaryTerm = null;
		}

		public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
		{
			if (!string.IsNullOrEmpty(mainTranslation))
			{
				Transform transform = ((Component)cmp).transform;
				string text = mainTranslation;
				int num = mainTranslation.LastIndexOfAny(LanguageSourceData.CategorySeparators);
				if (num >= 0)
				{
					text = text.Substring(num + 1);
				}
				for (int i = 0; i < transform.childCount; i++)
				{
					Transform child = transform.GetChild(i);
					((Component)child).gameObject.SetActive(((Object)child).name == text);
				}
			}
		}
	}
	public class LocalizeTarget_UnityStandard_MeshRenderer : LocalizeTarget<MeshRenderer>
	{
		static LocalizeTarget_UnityStandard_MeshRenderer()
		{
			AutoRegister();
		}

		[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
		private static void AutoRegister()
		{
			LocalizationManager.RegisterTarget(new LocalizeTargetDesc_Type<MeshRenderer, LocalizeTarget_UnityStandard_MeshRenderer>
			{
				Name = "MeshRenderer",
				Priority = 800
			});
		}

		public override eTermType GetPrimaryTermType(Localize cmp)
		{
			return eTermType.Mesh;
		}

		public override eTermType GetSecondaryTermType(Localize cmp)
		{
			return eTermType.Material;
		}

		public override bool CanUseSecondaryTerm()
		{
			return true;
		}

		public override bool AllowMainTermToBeRTL()
		{
			return false;
		}

		public override bool AllowSecondTermToBeRTL()
		{
			return false;
		}

		public override void GetFinalTerms(Localize cmp, string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			if ((Object)(object)mTarget == (Object)null)
			{
				primaryTerm = (secondaryTerm = null);
			}
			else
			{
				MeshFilter component = ((Component)mTarget).GetComponent<MeshFilter>();
				if ((Object)(object)component == (Object)null || (Object)(object)component.sharedMesh == (Object)null)
				{
					primaryTerm = null;
				}
				else
				{
					primaryTerm = ((Object)component.sharedMesh).name;
				}
			}
			if ((Object)(object)mTarget == (Object)null || (Object)(object)((Renderer)mTarget).sharedMaterial == (Object)null)
			{
				secondaryTerm = null;
			}
			else
			{
				secondaryTerm = ((Object)((Renderer)mTarget).sharedMaterial).name;
			}
		}

		public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
		{
			Material secondaryTranslatedObj = cmp.GetSecondaryTranslatedObj<Material>(ref mainTranslation, ref secondaryTranslation);
			if ((Object)(object)secondaryTranslatedObj != (Object)null && (Object)(object)((Renderer)mTarget).sharedMaterial != (Object)(object)secondaryTranslatedObj)
			{
				((Renderer)mTarget).material = secondaryTranslatedObj;
			}
			Mesh val = cmp.FindTranslatedObject<Mesh>(mainTranslation);
			MeshFilter component = ((Component)mTarget).GetComponent<MeshFilter>();
			if ((Object)(object)val != (Object)null && (Object)(object)component.sharedMesh != (Object)(object)val)
			{
				component.mesh = val;
			}
		}
	}
	public class LocalizeTargetDesc_Prefab : LocalizeTargetDesc<LocalizeTarget_UnityStandard_Prefab>
	{
		public override bool CanLocalize(Localize cmp)
		{
			return true;
		}
	}
	public class LocalizeTarget_UnityStandard_Prefab : LocalizeTarget<GameObject>
	{
		static LocalizeTarget_UnityStandard_Prefab()
		{
			AutoRegister();
		}

		[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
		private static void AutoRegister()
		{
			LocalizationManager.RegisterTarget(new LocalizeTargetDesc_Prefab
			{
				Name = "Prefab",
				Priority = 250
			});
		}

		public override bool IsValid(Localize cmp)
		{
			return true;
		}

		public override eTermType GetPrimaryTermType(Localize cmp)
		{
			return eTermType.GameObject;
		}

		public override eTermType GetSecondaryTermType(Localize cmp)
		{
			return eTermType.Text;
		}

		public override bool CanUseSecondaryTerm()
		{
			return false;
		}

		public override bool AllowMainTermToBeRTL()
		{
			return false;
		}

		public override bool AllowSecondTermToBeRTL()
		{
			return false;
		}

		public override void GetFinalTerms(Localize cmp, string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			primaryTerm = ((Object)cmp).name;
			secondaryTerm = null;
		}

		public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
		{
			if (string.IsNullOrEmpty(mainTranslation) || (Object.op_Implicit((Object)(object)mTarget) && ((Object)mTarget).name == mainTranslation))
			{
				return;
			}
			Transform transform = ((Component)cmp).transform;
			string text = mainTranslation;
			int num = mainTranslation.LastIndexOfAny(LanguageSourceData.CategorySeparators);
			if (num >= 0)
			{
				text = text.Substring(num + 1);
			}
			Transform val = InstantiateNewPrefab(cmp, mainTranslation);
			if ((Object)(object)val == (Object)null)
			{
				return;
			}
			((Object)val).name = text;
			for (int num2 = transform.childCount - 1; num2 >= 0; num2--)
			{
				Transform child = transform.GetChild(num2);
				if ((Object)(object)child != (Object)(object)val)
				{
					Object.Destroy((Object)(object)((Component)child).gameObject);
				}
			}
		}

		private Transform InstantiateNewPrefab(Localize cmp, string mainTranslation)
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = cmp.FindTranslatedObject<GameObject>(mainTranslation);
			if ((Object)(object)val == (Object)null)
			{
				return null;
			}
			GameObject val2 = mTarget;
			mTarget = Object.Instantiate<GameObject>(val);
			if ((Object)(object)mTarget == (Object)null)
			{
				return null;
			}
			Transform transform = ((Component)cmp).transform;
			Transform transform2 = mTarget.transform;
			transform2.SetParent(transform);
			Transform val3 = (Object.op_Implicit((Object)(object)val2) ? val2.transform : transform);
			transform2.rotation = val3.rotation;
			transform2.position = val3.position;
			return transform2;
		}
	}
	public class LocalizeTarget_UnityStandard_SpriteRenderer : LocalizeTarget<SpriteRenderer>
	{
		static LocalizeTarget_UnityStandard_SpriteRenderer()
		{
			AutoRegister();
		}

		[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
		private static void AutoRegister()
		{
			LocalizationManager.RegisterTarget(new LocalizeTargetDesc_Type<SpriteRenderer, LocalizeTarget_UnityStandard_SpriteRenderer>
			{
				Name = "SpriteRenderer",
				Priority = 100
			});
		}

		public override eTermType GetPrimaryTermType(Localize cmp)
		{
			return eTermType.Sprite;
		}

		public override eTermType GetSecondaryTermType(Localize cmp)
		{
			return eTermType.Text;
		}

		public override bool CanUseSecondaryTerm()
		{
			return false;
		}

		public override bool AllowMainTermToBeRTL()
		{
			return false;
		}

		public override bool AllowSecondTermToBeRTL()
		{
			return false;
		}

		public override void GetFinalTerms(Localize cmp, string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			primaryTerm = (((Object)(object)mTarget.sprite != (Object)null) ? ((Object)mTarget.sprite).name : string.Empty);
			secondaryTerm = null;
		}

		public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
		{
			Sprite sprite = mTarget.sprite;
			if ((Object)(object)sprite == (Object)null || ((Object)sprite).name != mainTranslation)
			{
				mTarget.sprite = cmp.FindTranslatedObject<Sprite>(mainTranslation);
			}
		}
	}
	public class LocalizeTarget_UnityStandard_TextMesh : LocalizeTarget<TextMesh>
	{
		private TextAlignment mAlignment_RTL = (TextAlignment)2;

		private TextAlignment mAlignment_LTR;

		private bool mAlignmentWasRTL;

		private bool mInitializeAlignment = true;

		static LocalizeTarget_UnityStandard_TextMesh()
		{
			AutoRegister();
		}

		[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
		private static void AutoRegister()
		{
			LocalizationManager.RegisterTarget(new LocalizeTargetDesc_Type<TextMesh, LocalizeTarget_UnityStandard_TextMesh>
			{
				Name = "TextMesh",
				Priority = 100
			});
		}

		public override eTermType GetPrimaryTermType(Localize cmp)
		{
			return eTermType.Text;
		}

		public override eTermType GetSecondaryTermType(Localize cmp)
		{
			return eTermType.Font;
		}

		public override bool CanUseSecondaryTerm()
		{
			return true;
		}

		public override bool AllowMainTermToBeRTL()
		{
			return true;
		}

		public override bool AllowSecondTermToBeRTL()
		{
			return false;
		}

		public override void GetFinalTerms(Localize cmp, string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			primaryTerm = (Object.op_Implicit((Object)(object)mTarget) ? mTarget.text : null);
			secondaryTerm = ((string.IsNullOrEmpty(Secondary) && (Object)(object)mTarget.font != (Object)null) ? ((Object)mTarget.font).name : null);
		}

		public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Invalid comparison between Unknown and I4
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Invalid comparison between Unknown and I4
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			Font secondaryTranslatedObj = cmp.GetSecondaryTranslatedObj<Font>(ref mainTranslation, ref secondaryTranslation);
			if ((Object)(object)secondaryTranslatedObj != (Object)null && (Object)(object)mTarget.font != (Object)(object)secondaryTranslatedObj)
			{
				mTarget.font = secondaryTranslatedObj;
			}
			if (mInitializeAlignment)
			{
				mInitializeAlignment = false;
				mAlignment_LTR = (mAlignment_RTL = mTarget.alignment);
				if (LocalizationManager.IsRight2Left && (int)mAlignment_RTL == 2)
				{
					mAlignment_LTR = (TextAlignment)0;
				}
				if (!LocalizationManager.IsRight2Left && (int)mAlignment_LTR == 0)
				{
					mAlignment_RTL = (TextAlignment)2;
				}
			}
			if (mainTranslation != null && mTarget.text != mainTranslation)
			{
				if (cmp.CorrectAlignmentForRTL && (int)mTarget.alignment != 1)
				{
					mTarget.alignment = (LocalizationManager.IsRight2Left ? mAlignment_RTL : mAlignment_LTR);
				}
				mTarget.font.RequestCharactersInTexture(mainTranslation);
				mTarget.text = mainTranslation;
			}
		}

		public LocalizeTarget_UnityStandard_TextMesh()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		}
	}
	public class LocalizeTarget_UnityUI_Image : LocalizeTarget<Image>
	{
		static LocalizeTarget_UnityUI_Image()
		{
			AutoRegister();
		}

		[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
		private static void AutoRegister()
		{
			LocalizationManager.RegisterTarget(new LocalizeTargetDesc_Type<Image, LocalizeTarget_UnityUI_Image>
			{
				Name = "Image",
				Priority = 100
			});
		}

		public override bool CanUseSecondaryTerm()
		{
			return false;
		}

		public override bool AllowMainTermToBeRTL()
		{
			return false;
		}

		public override bool AllowSecondTermToBeRTL()
		{
			return false;
		}

		public override eTermType GetPrimaryTermType(Localize cmp)
		{
			if (!((Object)(object)mTarget.sprite == (Object)null))
			{
				return eTermType.Sprite;
			}
			return eTermType.Texture;
		}

		public override eTermType GetSecondaryTermType(Localize cmp)
		{
			return eTermType.Text;
		}

		public override void GetFinalTerms(Localize cmp, string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			primaryTerm = (Object.op_Implicit((Object)(object)((Graphic)mTarget).mainTexture) ? ((Object)((Graphic)mTarget).mainTexture).name : "");
			if ((Object)(object)mTarget.sprite != (Object)null && ((Object)mTarget.sprite).name != primaryTerm)
			{
				primaryTerm = primaryTerm + "." + ((Object)mTarget.sprite).name;
			}
			secondaryTerm = null;
		}

		public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
		{
			Sprite sprite = mTarget.sprite;
			if ((Object)(object)sprite == (Object)null || ((Object)sprite).name != mainTranslation)
			{
				mTarget.sprite = cmp.FindTranslatedObject<Sprite>(mainTranslation);
			}
		}
	}
	public class LocalizeTarget_UnityUI_RawImage : LocalizeTarget<RawImage>
	{
		static LocalizeTarget_UnityUI_RawImage()
		{
			AutoRegister();
		}

		[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
		private static void AutoRegister()
		{
			LocalizationManager.RegisterTarget(new LocalizeTargetDesc_Type<RawImage, LocalizeTarget_UnityUI_RawImage>
			{
				Name = "RawImage",
				Priority = 100
			});
		}

		public override eTermType GetPrimaryTermType(Localize cmp)
		{
			return eTermType.Texture;
		}

		public override eTermType GetSecondaryTermType(Localize cmp)
		{
			return eTermType.Text;
		}

		public override bool CanUseSecondaryTerm()
		{
			return false;
		}

		public override bool AllowMainTermToBeRTL()
		{
			return false;
		}

		public override bool AllowSecondTermToBeRTL()
		{
			return false;
		}

		public override void GetFinalTerms(Localize cmp, string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			primaryTerm = (Object.op_Implicit((Object)(object)((Graphic)mTarget).mainTexture) ? ((Object)((Graphic)mTarget).mainTexture).name : "");
			secondaryTerm = null;
		}

		public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
		{
			Texture texture = mTarget.texture;
			if ((Object)(object)texture == (Object)null || ((Object)texture).name != mainTranslation)
			{
				mTarget.texture = cmp.FindTranslatedObject<Texture>(mainTranslation);
			}
		}
	}
	public class LocalizeTarget_UnityUI_Text : LocalizeTarget<Text>
	{
		private TextAnchor mAlignment_RTL = (TextAnchor)2;

		private TextAnchor mAlignment_LTR;

		private bool mAlignmentWasRTL;

		private bool mInitializeAlignment = true;

		static LocalizeTarget_UnityUI_Text()
		{
			AutoRegister();
		}

		[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
		private static void AutoRegister()
		{
			LocalizationManager.RegisterTarget(new LocalizeTargetDesc_Type<Text, LocalizeTarget_UnityUI_Text>
			{
				Name = "Text",
				Priority = 100
			});
		}

		public override eTermType GetPrimaryTermType(Localize cmp)
		{
			return eTermType.Text;
		}

		public override eTermType GetSecondaryTermType(Localize cmp)
		{
			return eTermType.Font;
		}

		public override bool CanUseSecondaryTerm()
		{
			return true;
		}

		public override bool AllowMainTermToBeRTL()
		{
			return true;
		}

		public override bool AllowSecondTermToBeRTL()
		{
			return false;
		}

		public override void GetFinalTerms(Localize cmp, string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
		{
			primaryTerm = (Object.op_Implicit((Object)(object)mTarget) ? mTarget.text : null);
			secondaryTerm = (((Object)(object)mTarget.font != (Object)null) ? ((Object)mTarget.font).name : string.Empty);
		}

		public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
		{
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			Font secondaryTranslatedObj = cmp.GetSecondaryTranslatedObj<Font>(ref mainTranslation, ref secondaryTranslation);
			if ((Object)(object)secondaryTranslatedObj != (Object)null && (Object)(object)secondaryTranslatedObj != (Object)(object)mTarget.font)
			{
				mTarget.font = secondaryTranslatedObj;
			}
			if (mInitializeAlignment)
			{
				mInitializeAlignment = false;
				mAlignmentWasRTL = LocalizationManager.IsRight2Left;
				InitAlignment(mAlignmentWasRTL, mTarget.alignment, out mAlignment_LTR, out mAlignment_RTL);
			}
			else
			{
				InitAlignment(mAlignmentWasRTL, mTarget.alignment, out var alignLTR, out var alignRTL);
				if ((mAlignmentWasRTL && mAlignment_RTL != alignRTL) || (!mAlignmentWasRTL && mAlignment_LTR != alignLTR))
				{
					mAlignment_LTR = alignLTR;
					mAlignment_RTL = alignRTL;
				}
				mAlignmentWasRTL = LocalizationManager.IsRight2Left;
			}
			if (mainTranslation != null && mTarget.text != mainTranslation)
			{
				if (cmp.CorrectAlignmentForRTL)
				{
					mTarget.alignment = (LocalizationManager.IsRight2Left ? mAlignment_RTL : mAlignment_LTR);
				}
				mTarget.text = mainTranslation;
				((Graphic)mTarget).SetVerticesDirty();
			}
		}

		private void InitAlignment(bool isRTL, TextAnchor alignment, out TextAnchor alignLTR, out TextAnchor alignRTL)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected I4, but got Unknown
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Expected I4, but got Unknown
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Expected I4, but got Unknown
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected I4, but got Unknown
			TextAnchor val;
			alignRTL = (TextAnchor)(int)(val = alignment);
			alignLTR = (TextAnchor)(int)val;
			if (isRTL)
			{
				switch ((int)alignment)
				{
				case 2:
					alignLTR = (TextAnchor)0;
					break;
				case 5:
					alignLTR = (TextAnchor)3;
					break;
				case 8:
					alignLTR = (TextAnchor)6;
					break;
				case 0:
					alignLTR = (TextAnchor)2;
					break;
				case 3:
					alignLTR = (TextAnchor)5;
					break;
				case 6:
					alignLTR = (TextAnchor)8;
					break;
				case 1:
				case 4:
				case 7:
					break;
				}
			}
			else
			{
				switch ((int)alignment)
				{
				case 2:
					alignRTL = (TextAnchor)0;
					break;
				case 5:
					alignRTL = (TextAnchor)3;
					break;
				case 8:
					alignRTL = (TextAnchor)6;
					break;
				case 0:
					alignRTL = (TextAnchor)2;
					break;
				case 3:
					alignRTL = (TextAnchor)5;
					break;
				case 6:
					alignRTL = (TextAnchor)8;
					break;
				case 1:
				case 4:
				case 7:
					break;
				}
			}
		}

		public LocalizeTarget_UnityUI_Text()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		}
	}
	public enum eTermType
	{
		Text,
		Font,
		Texture,
		AudioClip,
		GameObject,
		Sprite,
		Material,
		Child,
		Mesh,
		TextMeshPFont,
		Object
	}
	public enum TranslationFlag : byte
	{
		Normal = 1,
		AutoTranslated
	}
	[Serializable]
	public class TermData
	{
		public string Term = string.Empty;

		public eTermType TermType;

		[NonSerialized]
		public string Description;

		public string[] Languages = new string[0];

		public byte[] Flags = new byte[0];

		[SerializeField]
		private string[] Languages_Touch;

		public string GetTranslation(int idx, string specialization = null, bool editMode = false)
		{
			string text = Languages[idx];
			if (text != null)
			{
				text = SpecializationManager.GetSpecializedText(text, specialization);
				if (!editMode)
				{
					text = text.Replace("[i2nt]", "").Replace("[/i2nt]", "");
				}
			}
			return text;
		}

		public void SetTranslation(int idx, string translation, string specialization = null)
		{
			Languages[idx] = SpecializationManager.SetSpecializedText(Languages[idx], translation, specialization);
		}

		public void RemoveSpecialization(string specialization)
		{
			for (int i = 0; i < Languages.Length; i++)
			{
				RemoveSpecialization(i, specialization);
			}
		}

		public void RemoveSpecialization(int idx, string specialization)
		{
			string text = Languages[idx];
			if (!(specialization == "Any") && text.Contains("[i2s_" + specialization + "]"))
			{
				Dictionary<string, string> specializations = SpecializationManager.GetSpecializations(text);
				specializations.Remove(specialization);
				Languages[idx] = SpecializationManager.SetSpecializedText(specializations);
			}
		}

		public bool IsAutoTranslated(int idx, bool IsTouch)
		{
			return (Flags[idx] & 2) > 0;
		}

		public void Validate()
		{
			int num = Mathf.Max(Languages.Length, Flags.Length);
			if (Languages.Length != num)
			{
				Array.Resize(ref Languages, num);
			}
			if (Flags.Length != num)
			{
				Array.Resize(ref Flags, num);
			}
			if (Languages_Touch == null)
			{
				return;
			}
			for (int i = 0; i < Mathf.Min(Languages_Touch.Length, num); i++)
			{
				if (string.IsNullOrEmpty(Languages[i]) && !string.IsNullOrEmpty(Languages_Touch[i]))
				{
					Languages[i] = Languages_Touch[i];
					Languages_Touch[i] = null;
				}
			}
			Languages_Touch = null;
		}

		public bool IsTerm(string name, bool allowCategoryMistmatch)
		{
			if (!allowCategoryMistmatch)
			{
				return name == Term;
			}
			return name == LanguageSourceData.GetKeyFromFullTerm(Term);
		}

		public bool HasSpecializations()
		{
			for (int i = 0; i < Languages.Length; i++)
			{
				if (!string.IsNullOrEmpty(Languages[i]) && Languages[i].Contains("[i2s_"))
				{
					return true;
				}
			}
			return false;
		}

		public List<string> GetAllSpecializations()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < Languages.Length; i++)
			{
				SpecializationManager.AppendSpecializations(Languages[i], list);
			}
			return list;
		}
	}
	public class TermsPopup : PropertyAttribute
	{
		public string Filter { get; private set; }

		public TermsPopup(string filter = "")
		{
			Filter = filter;
		}
	}
	public class AutoChangeCultureInfo : MonoBehaviour
	{
		public void Start()
		{
			LocalizationManager.EnableChangingCultureInfo(bEnable: true);
		}
	}
	public class CoroutineManager : MonoBehaviour
	{
		private static CoroutineManager mInstance;

		private static CoroutineManager pInstance
		{
			get
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Expected Obj, but got Unknown
				if ((Object)(object)mInstance == (Object)null)
				{
					GameObject val = new GameObject("_Coroutiner");
					((Object)val).hideFlags = (HideFlags)61;
					mInstance = val.AddComponent<CoroutineManager>();
					if (Application.isPlaying)
					{
						Object.DontDestroyOnLoad((Object)(object)val);
					}
				}
				return mInstance;
			}
		}

		private void Awake()
		{
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
			}
		}

		public static Coroutine Start(IEnumerator coroutine)
		{
			return ((MonoBehaviour)pInstance).StartCoroutine(coroutine);
		}
	}
	[AddComponentMenu("I2/Localization/I2 Localize Callback")]
	public class CustomLocalizeCallback : MonoBehaviour
	{
		public UnityEvent _OnLocalize = new UnityEvent();

		public void OnEnable()
		{
			LocalizationManager.OnLocalizeEvent -= OnLocalize;
			LocalizationManager.OnLocalizeEvent += OnLocalize;
		}

		public void OnDisable()
		{
			LocalizationManager.OnLocalizeEvent -= OnLocalize;
		}

		public void OnLocalize()
		{
			_OnLocalize.Invoke();
		}

		public CustomLocalizeCallback()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected Obj, but got Unknown
		}
	}
	public class HindiFixer
	{
		internal static string Fix(string text)
		{
			char[] array = text.ToCharArray();
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == '\u093f' && !char.IsWhiteSpace(array[i - 1]) && array[i - 1] != '\0')
				{
					array[i] = array[i - 1];
					array[i - 1] = '\u093f';
					flag = true;
				}
				if (i != array.Length - 1)
				{
					if (array[i] == 'इ' && array[i + 1] == '\u093c')
					{
						array[i] = 'ऌ';
						array[i + 1] = '\0';
						flag = true;
					}
					if (array[i] == '\u0943' && array[i + 1] == '\u093c')
					{
						array[i] = '\u0944';
						array[i + 1] = '\0';
						flag = true;
					}
					if (array[i] == '\u0901' && array[i + 1] == '\u093c')
					{
						array[i] = 'ॐ';
						array[i + 1] = '\0';
						flag = true;
					}
					if (array[i] == 'ऋ' && array[i + 1] == '\u093c')
					{
						array[i] = 'ॠ';
						array[i + 1] = '\0';
						flag = true;
					}
					if (array[i] == 'ई' && array[i + 1] == '\u093c')
					{
						array[i] = 'ॡ';
						array[i + 1] = '\0';
						flag = true;
					}
					if (array[i] == '\u093f' && array[i + 1] == '\u093c')
					{
						array[i] = '\u0962';
						array[i + 1] = '\0';
						flag = true;
					}
					if (array[i] == '\u0940' && array[i + 1] == '\u093c')
					{
						array[i] = '\u0963';
						array[i + 1] = '\0';
						flag = true;
					}
					if (array[i] == '।' && array[i + 1] == '\u093c')
					{
						array[i] = 'ऽ';
						array[i + 1] = '\0';
						flag = true;
					}
				}
			}
			if (!flag)
			{
				return text;
			}
			string text2 = new string(array.Where((char x) => x != '\0').ToArray());
			if (text2 == text)
			{
				return text2;
			}
			text = text2;
			return text;
		}
	}
	public static class I2Utils
	{
		public const string ValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";

		public const string NumberChars = "0123456789";

		public const string ValidNameSymbols = ".-_$#@*()[]{}+:?!&',^=<>~`";

		public static string ReverseText(string source)
		{
			int length = source.Length;
			char[] array = new char[length];
			for (int i = 0; i < length; i++)
			{
				array[length - 1 - i] = source[i];
			}
			return new string(array);
		}

		public static string RemoveNonASCII(string text, bool allowCategory = false)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			int num = 0;
			char[] array = new char[text.Length];
			bool flag = false;
			char[] array2 = text.Trim().ToCharArray();
			foreach (char c in array2)
			{
				char c2 = ' ';
				if ((allowCategory && (c == '\\' || c == '"' || c == '/')) || char.IsLetterOrDigit(c) || ".-_$#@*()[]{}+:?!&',^=<>~`".IndexOf(c) >= 0)
				{
					c2 = c;
				}
				if (char.IsWhiteSpace(c2))
				{
					if (!flag)
					{
						if (num > 0)
						{
							array[num++] = ' ';
						}
						flag = true;
					}
				}
				else
				{
					flag = false;
					array[num++] = c2;
				}
			}
			return new string(array, 0, num);
		}

		public static string GetValidTermName(string text, bool allowCategory = false)
		{
			if (text == null)
			{
				return null;
			}
			text = RemoveTags(text);
			return RemoveNonASCII(text, allowCategory);
		}

		public static string SplitLine(string line, int maxCharacters)
		{
			if (maxCharacters <= 0 || line.Length < maxCharacters)
			{
				return line;
			}
			char[] array = line.ToCharArray();
			bool flag = true;
			bool flag2 = false;
			int i = 0;
			int num = 0;
			for (; i < array.Length; i++)
			{
				if (flag)
				{
					num++;
					if (array[i] == '\n')
					{
						num = 0;
					}
					if (num >= maxCharacters && char.IsWhiteSpace(array[i]))
					{
						array[i] = '\n';
						flag = false;
						flag2 = false;
					}
				}
				else if (!char.IsWhiteSpace(array[i]))
				{
					flag = true;
					num = 0;
				}
				else if (array[i] != '\n')
				{
					array[i] = '\0';
				}
				else
				{
					if (!flag2)
					{
						array[i] = '\0';
					}
					flag2 = true;
				}
			}
			return new string(array.Where((char c) => c != '\0').ToArray());
		}

		public static bool FindNextTag(string line, int iStart, out int tagStart, out int tagEnd)
		{
			tagStart = -1;
			tagEnd = -1;
			int length = line.Length;
			tagStart = iStart;
			while (tagStart < length && line[tagStart] != '[' && line[tagStart] != '(' && line[tagStart] != '{' && line[tagStart] != '<')
			{
				tagStart++;
			}
			if (tagStart == length)
			{
				return false;
			}
			bool flag = false;
			for (tagEnd = tagStart + 1; tagEnd < length; tagEnd++)
			{
				char c = line[tagEnd];
				if (c == ']' || c == ')' || c == '}' || c == '>')
				{
					if (flag)
					{
						return FindNextTag(line, tagEnd + 1, out tagStart, out tagEnd);
					}
					return true;
				}
				if (c > 'ÿ')
				{
					flag = true;
				}
			}
			return false;
		}

		public static string RemoveTags(string text)
		{
			return Regex.Replace(text, "\\{\\[(.*?)]}|\\[(.*?)]|\\<(.*?)>", "");
		}

		public static bool RemoveResourcesPath(ref string sPath)
		{
			int num = sPath.IndexOf("\\Resources\\");
			int num2 = sPath.IndexOf("\\Resources/");
			int num3 = sPath.IndexOf("/Resources\\");
			int num4 = sPath.IndexOf("/Resources/");
			int num5 = Mathf.Max(new int[4] { num, num2, num3, num4 });
			bool result = false;
			if (num5 >= 0)
			{
				sPath = sPath.Substring(num5 + 11);
				result = true;
			}
			else
			{
				num5 = sPath.LastIndexOfAny(LanguageSourceData.CategorySeparators);
				if (num5 > 0)
				{
					sPath = sPath.Substring(num5 + 1);
				}
			}
			string extension = Path.GetExtension(sPath);
			if (!string.IsNullOrEmpty(extension))
			{
				sPath = sPath.Substring(0, sPath.Length - extension.Length);
			}
			return result;
		}

		public static bool IsPlaying()
		{
			if (Application.isPlaying)
			{
				return true;
			}
			return false;
		}

		public static string GetPath(this Transform tr)
		{
			Transform parent = tr.parent;
			if ((Object)(object)tr == (Object)null)
			{
				return ((Object)tr).name;
			}
			return parent.GetPath() + "/" + ((Object)tr).name;
		}

		public static Transform FindObject(string objectPath)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return FindObject(SceneManager.GetActiveScene(), objectPath);
		}

		public static Transform FindObject(Scene scene, string objectPath)
		{
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				Transform transform = rootGameObjects[i].transform;
				if (((Object)transform).name == objectPath)
				{
					return transform;
				}
				if (objectPath.StartsWith(((Object)transform).name + "/"))
				{
					return FindObject(transform, objectPath.Substring(((Object)transform).name.Length + 1));
				}
			}
			return null;
		}

		public static Transform FindObject(Transform root, string objectPath)
		{
			for (int i = 0; i < root.childCount; i++)
			{
				Transform child = root.GetChild(i);
				if (((Object)child).name == objectPath)
				{
					return child;
				}
				if (objectPath.StartsWith(((Object)child).name + "/"))
				{
					return FindObject(child, objectPath.Substring(((Object)child).name.Length + 1));
				}
			}
			return null;
		}

		public static H FindInParents<H>(Transform tr) where H : Component
		{
			if (!Object.op_Implicit((Object)(object)tr))
			{
				return default;
			}
			H component = ((Component)tr).GetComponent<H>();
			while (!Object.op_Implicit((Object)(object)component) && Object.op_Implicit((Object)(object)tr))
			{
				component = ((Component)tr).GetComponent<H>();
				tr = tr.parent;
			}
			return component;
		}

		public static string GetCaptureMatch(Match match)
		{
			for (int num = match.Groups.Count - 1; num >= 0; num--)
			{
				if (match.Groups[num].Success)
				{
					return match.Groups[num].ToString();
				}
			}
			return match.ToString();
		}

		public static void SendWebRequest(UnityWebRequest www)
		{
			www.SendWebRequest();
		}
	}
	public interface ILocalizationParamsManager
	{
		string GetParameterValue(string Param);
	}
	public class LocalizationParamsManager : MonoBehaviour, ILocalizationParamsManager
	{
		[Serializable]
		public struct ParamValue
		{
			public string Name;

			public string Value;
		}

		[SerializeField]
		public List<ParamValue> _Params = new List<ParamValue>();

		public bool _IsGlobalManager;

		public string GetParameterValue(string ParamName)
		{
			if (_Params != null)
			{
				int i = 0;
				for (int count = _Params.Count; i < count; i++)
				{
					if (_Params[i].Name == ParamName)
					{
						return _Params[i].Value;
					}
				}
			}
			return null;
		}

		public void SetParameterValue(string ParamName, string ParamValue, bool localize = true)
		{
			bool flag = false;
			int i = 0;
			for (int count = _Params.Count; i < count; i++)
			{
				if (_Params[i].Name == ParamName)
				{
					ParamValue value = _Params[i];
					value.Value = ParamValue;
					_Params[i] = value;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				_Params.Add(new ParamValue
				{
					Name = ParamName,
					Value = ParamValue
				});
			}
			if (localize)
			{
				OnLocalize();
			}
		}

		public void OnLocalize()
		{
			Localize component = ((Component)this).GetComponent<Localize>();
			if ((Object)(object)component != (Object)null)
			{
				component.OnLocalize(Force: true);
			}
		}

		public virtual void OnEnable()
		{
			if (_IsGlobalManager)
			{
				DoAutoRegister();
			}
		}

		public void DoAutoRegister()
		{
			if (!LocalizationManager.ParamManagers.Contains(this))
			{
				LocalizationManager.ParamManagers.Add(this);
				LocalizationManager.LocalizeAll(Force: true);
			}
		}

		public void OnDisable()
		{
			LocalizationManager.ParamManagers.Remove(this);
		}
	}
	[Serializable]
	public struct LocalizedString
	{
		public string mTerm;

		public bool mRTL_IgnoreArabicFix;

		public int mRTL_MaxLineLength;

		public bool mRTL_ConvertNumbers;

		public bool m_DontLocalizeParameters;

		public static implicit operator string(LocalizedString s)
		{
			return s.ToString();
		}

		public static implicit operator LocalizedString(string term)
		{
			return new LocalizedString
			{
				mTerm = term
			};
		}

		public LocalizedString(LocalizedString str)
		{
			mTerm = str.mTerm;
			mRTL_IgnoreArabicFix = str.mRTL_IgnoreArabicFix;
			mRTL_MaxLineLength = str.mRTL_MaxLineLength;
			mRTL_ConvertNumbers = str.mRTL_ConvertNumbers;
			m_DontLocalizeParameters = str.m_DontLocalizeParameters;
		}

		public override string ToString()
		{
			string translation = LocalizationManager.GetTranslation(mTerm, !mRTL_IgnoreArabicFix, mRTL_MaxLineLength, !mRTL_ConvertNumbers, applyParameters: true);
			LocalizationManager.ApplyLocalizationParams(ref translation, !m_DontLocalizeParameters);
			return translation;
		}
	}
	public class RegisterGlobalParameters : MonoBehaviour, ILocalizationParamsManager
	{
		public virtual void OnEnable()
		{
			if (!LocalizationManager.ParamManagers.Contains(this))
			{
				LocalizationManager.ParamManagers.Add(this);
				LocalizationManager.LocalizeAll(Force: true);
			}
		}

		public virtual void OnDisable()
		{
			LocalizationManager.ParamManagers.Remove(this);
		}

		public virtual string GetParameterValue(string ParamName)
		{
			return null;
		}
	}
	public interface IResourceManager_Bundles
	{
		Object LoadFromBundle(string path, Type assetType);
	}
	public class ResourceManager : MonoBehaviour
	{
		private static ResourceManager mInstance;

		public List<IResourceManager_Bundles> mBundleManagers = new List<IResourceManager_Bundles>();

		public Object[] Assets;

		private readonly Dictionary<string, Object> mResourcesCache = new Dictionary<string, Object>(StringComparer.Ordinal);

		public static ResourceManager pInstance
		{
			get
			{
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				bool flag = (Object)(object)mInstance == (Object)null;
				if ((Object)(object)mInstance == (Object)null)
				{
					mInstance = (ResourceManager)(object)Object.FindObjectOfType(typeof(ResourceManager));
				}
				if ((Object)(object)mInstance == (Object)null)
				{
					GameObject val = new GameObject("I2ResourceManager", new Type[1] { typeof(ResourceManager) });
					((Object)val).hideFlags = (HideFlags)(((Object)val).hideFlags | 0x3D);
					mInstance = val.GetComponent<ResourceManager>();
					SceneManager.sceneLoaded += MyOnLevelWasLoaded;
				}
				if (flag && Application.isPlaying)
				{
					Object.DontDestroyOnLoad((Object)(object)((Component)mInstance).gameObject);
				}
				return mInstance;
			}
		}

		public static void MyOnLevelWasLoaded(Scene scene, LoadSceneMode mode)
		{
			pInstance.CleanResourceCache();
			LocalizationManager.UpdateSources();
		}

		public T GetAsset<T>(string Name) where T : Object
		{
			Object val = FindAsset(Name);
			T val2 = (T)(object)((val is T) ? val : null);
			if ((Object)(object)val2 != (Object)null)
			{
				return val2;
			}
			return LoadFromResources<T>(Name);
		}

		private Object FindAsset(string Name)
		{
			if (Assets != null)
			{
				int i = 0;
				for (int num = Assets.Length; i < num; i++)
				{
					if (Assets[i] != (Object)null && Assets[i].name == Name)
					{
						return Assets[i];
					}
				}
			}
			return null;
		}

		public bool HasAsset(Object Obj)
		{
			if (Assets == null)
			{
				return false;
			}
			return Array.IndexOf(Assets, Obj) >= 0;
		}

		public T LoadFromResources<T>(string Path) where T : Object
		{
			try
			{
				if (string.IsNullOrEmpty(Path))
				{
					return default;
				}
				if (mResourcesCache.TryGetValue(Path, out var value) && value != (Object)null)
				{
					return (T)(object)((value is T) ? value : null);
				}
				T val = default;
				if (Path.EndsWith("]", StringComparison.OrdinalIgnoreCase))
				{
					int num = Path.LastIndexOf("[", StringComparison.OrdinalIgnoreCase);
					int length = Path.Length - num - 2;
					string value2 = Path.Substring(num + 1, length);
					Path = Path.Substring(0, num);
					T[] array = Resources.LoadAll<T>(Path);
					int i = 0;
					for (int num2 = array.Length; i < num2; i++)
					{
						if (((Object)array[i]).name.Equals(value2))
						{
							val = array[i];
							break;
						}
					}
				}
				else
				{
					Object val2 = Resources.Load(Path, typeof(T));
					val = (T)(object)((val2 is T) ? val2 : null);
				}
				if ((Object)(object)val == (Object)null)
				{
					val = LoadFromBundle<T>(Path);
				}
				if ((Object)(object)val != (Object)null)
				{
					mResourcesCache[Path] = (Object)(object)val;
				}
				return val;
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Unable to load {0} '{1}'\nERROR: {2}", new object[3]
				{
					typeof(T),
					Path,
					ex.ToString()
				});
				return default;
			}
		}

		public T LoadFromBundle<T>(string path) where T : Object
		{
			int i = 0;
			for (int count = mBundleManagers.Count; i < count; i++)
			{
				if (mBundleManagers[i] != null)
				{
					Object val = mBundleManagers[i].LoadFromBundle(path, typeof(T));
					T val2 = (T)(object)((val is T) ? val : null);
					if ((Object)(object)val2 != (Object)null)
					{
						return val2;
					}
				}
			}
			return default;
		}

		public void CleanResourceCache()
		{
			mResourcesCache.Clear();
			Resources.UnloadUnusedAssets();
			((MonoBehaviour)this).CancelInvoke();
		}
	}
	public class RTLFixer
	{
		public static string Fix(string str)
		{
			return Fix(str, showTashkeel: false, useHinduNumbers: true);
		}

		public static string Fix(string str, bool rtl)
		{
			if (rtl)
			{
				return Fix(str);
			}
			string[] array = str.Split(new char[1] { ' ' });
			string text = "";
			string text2 = "";
			string[] array2 = array;
			foreach (string text3 in array2)
			{
				if (char.IsLower(text3.ToLower()[text3.Length / 2]))
				{
					text = text + Fix(text2) + text3 + " ";
					text2 = "";
				}
				else
				{
					text2 = text2 + text3 + " ";
				}
			}
			if (text2 != "")
			{
				text += Fix(text2);
			}
			return text;
		}

		public static string Fix(string str, bool showTashkeel, bool useHinduNumbers)
		{
			string text = HindiFixer.Fix(str);
			if (text != str)
			{
				return text;
			}
			RTLFixerTool.showTashkeel = showTashkeel;
			RTLFixerTool.useHinduNumbers = useHinduNumbers;
			if (str.Contains("\n"))
			{
				str = str.Replace("\n", Environment.NewLine);
			}
			if (str.Contains(Environment.NewLine))
			{
				string[] separator = new string[1] { Environment.NewLine };
				string[] array = str.Split(separator, StringSplitOptions.None);
				if (array.Length == 0)
				{
					return RTLFixerTool.FixLine(str);
				}
				if (array.Length == 1)
				{
					return RTLFixerTool.FixLine(str);
				}
				string text2 = RTLFixerTool.FixLine(array[0]);
				int i = 1;
				if (array.Length > 1)
				{
					for (; i < array.Length; i++)
					{
						text2 = text2 + Environment.NewLine + RTLFixerTool.FixLine(array[i]);
					}
				}
				return text2;
			}
			return RTLFixerTool.FixLine(str);
		}
	}
	internal enum IsolatedArabicLetters
	{
		Hamza = 65152,
		Alef = 65165,
		AlefHamza = 65155,
		WawHamza = 65157,
		AlefMaksoor = 65159,
		AlefMaksora = 64508,
		HamzaNabera = 65161,
		Ba = 65167,
		Ta = 65173,
		Tha2 = 65177,
		Jeem = 65181,
		H7aa = 65185,
		Khaa2 = 65189,
		Dal = 65193,
		Thal = 65195,
		Ra2 = 65197,
		Zeen = 65199,
		Seen = 65201,
		Sheen = 65205,
		S9a = 65209,
		Dha = 65213,
		T6a = 65217,
		T6ha = 65221,
		Ain = 65225,
		Gain = 65229,
		Fa = 65233,
		Gaf = 65237,
		Kaf = 65241,
		Lam = 65245,
		Meem = 65249,
		Noon = 65253,
		Ha = 65257,
		Waw = 65261,
		Ya = 65265,
		AlefMad = 65153,
		TaMarboota = 65171,
		PersianPe = 64342,
		PersianChe = 64378,
		PersianZe = 64394,
		PersianGaf = 64402,
		PersianGaf2 = 64398
	}
	internal enum GeneralArabicLetters
	{
		Hamza = 1569,
		Alef = 1575,
		AlefHamza = 1571,
		WawHamza = 1572,
		AlefMaksoor = 1573,
		AlefMagsora = 1609,
		HamzaNabera = 1574,
		Ba = 1576,
		Ta = 1578,
		Tha2 = 1579,
		Jeem = 1580,
		H7aa = 1581,
		Khaa2 = 1582,
		Dal = 1583,
		Thal = 1584,
		Ra2 = 1585,
		Zeen = 1586,
		Seen = 1587,
		Sheen = 1588,
		S9a = 1589,
		Dha = 1590,
		T6a = 1591,
		T6ha = 1592,
		Ain = 1593,
		Gain = 1594,
		Fa = 1601,
		Gaf = 1602,
		Kaf = 1603,
		Lam = 1604,
		Meem = 1605,
		Noon = 1606,
		Ha = 1607,
		Waw = 1608,
		Ya = 1610,
		AlefMad = 1570,
		TaMarboota = 1577,
		PersianPe = 1662,
		PersianChe = 1670,
		PersianZe = 1688,
		PersianGaf = 1711,
		PersianGaf2 = 1705
	}
	internal class ArabicMapping
	{
		public int from;

		public int to;

		public ArabicMapping(int from, int to)
		{
			this.from = from;
			this.to = to;
		}
	}
	internal class ArabicTable
	{
		private static List<ArabicMapping> mapList;

		private static ArabicTable arabicMapper;

		internal static ArabicTable ArabicMapper
		{
			get
			{
				if (arabicMapper == null)
				{
					arabicMapper = new ArabicTable();
				}
				return arabicMapper;
			}
		}

		private ArabicTable()
		{
			mapList = new List<ArabicMapping>();
			mapList.Add(new ArabicMapping(1569, 65152));
			mapList.Add(new ArabicMapping(1575, 65165));
			mapList.Add(new ArabicMapping(1571, 65155));
			mapList.Add(new ArabicMapping(1572, 65157));
			mapList.Add(new ArabicMapping(1573, 65159));
			mapList.Add(new ArabicMapping(1609, 64508));
			mapList.Add(new ArabicMapping(1574, 65161));
			mapList.Add(new ArabicMapping(1576, 65167));
			mapList.Add(new ArabicMapping(1578, 65173));
			mapList.Add(new ArabicMapping(1579, 65177));
			mapList.Add(new ArabicMapping(1580, 65181));
			mapList.Add(new ArabicMapping(1581, 65185));
			mapList.Add(new ArabicMapping(1582, 65189));
			mapList.Add(new ArabicMapping(1583, 65193));
			mapList.Add(new ArabicMapping(1584, 65195));
			mapList.Add(new ArabicMapping(1585, 65197));
			mapList.Add(new ArabicMapping(1586, 65199));
			mapList.Add(new ArabicMapping(1587, 65201));
			mapList.Add(new ArabicMapping(1588, 65205));
			mapList.Add(new ArabicMapping(1589, 65209));
			mapList.Add(new ArabicMapping(1590, 65213));
			mapList.Add(new ArabicMapping(1591, 65217));
			mapList.Add(new ArabicMapping(1592, 65221));
			mapList.Add(new ArabicMapping(1593, 65225));
			mapList.Add(new ArabicMapping(1594, 65229));
			mapList.Add(new ArabicMapping(1601, 65233));
			mapList.Add(new ArabicMapping(1602, 65237));
			mapList.Add(new ArabicMapping(1603, 65241));
			mapList.Add(new ArabicMapping(1604, 65245));
			mapList.Add(new ArabicMapping(1605, 65249));
			mapList.Add(new ArabicMapping(1606, 65253));
			mapList.Add(new ArabicMapping(1607, 65257));
			mapList.Add(new ArabicMapping(1608, 65261));
			mapList.Add(new ArabicMapping(1610, 65265));
			mapList.Add(new ArabicMapping(1570, 65153));
			mapList.Add(new ArabicMapping(1577, 65171));
			mapList.Add(new ArabicMapping(1662, 64342));
			mapList.Add(new ArabicMapping(1670, 64378));
			mapList.Add(new ArabicMapping(1688, 64394));
			mapList.Add(new ArabicMapping(1711, 64402));
			mapList.Add(new ArabicMapping(1705, 64398));
		}

		internal int Convert(int toBeConverted)
		{
			foreach (ArabicMapping map in mapList)
			{
				if (map.from == toBeConverted)
				{
					return map.to;
				}
			}
			return toBeConverted;
		}
	}
	internal class TashkeelLocation
	{
		public char tashkeel;

		public int position;

		public TashkeelLocation(char tashkeel, int position)
		{
			this.tashkeel = tashkeel;
			this.position = position;
		}
	}
	internal class RTLFixerTool
	{
		internal static bool showTashkeel = true;

		internal static bool useHinduNumbers = false;

		internal static string RemoveTashkeel(string str, out List<TashkeelLocation> tashkeelLocation)
		{
			tashkeelLocation = new List<TashkeelLocation>();
			char[] array = str.ToCharArray();
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == '\u064b')
				{
					tashkeelLocation.Add(new TashkeelLocation('\u064b', i));
					num++;
				}
				else if (array[i] == '\u064c')
				{
					tashkeelLocation.Add(new TashkeelLocation('\u064c', i));
					num++;
				}
				else if (array[i] == '\u064d')
				{
					tashkeelLocation.Add(new TashkeelLocation('\u064d', i));
					num++;
				}
				else if (array[i] == '\u064e')
				{
					if (num > 0 && tashkeelLocation[num - 1].tashkeel == '\u0651')
					{
						tashkeelLocation[num - 1].tashkeel = 'ﱠ';
						continue;
					}
					tashkeelLocation.Add(new TashkeelLocation('\u064e', i));
					num++;
				}
				else if (array[i] == '\u064f')
				{
					if (num > 0 && tashkeelLocation[num - 1].tashkeel == '\u0651')
					{
						tashkeelLocation[num - 1].tashkeel = 'ﱡ';
						continue;
					}
					tashkeelLocation.Add(new TashkeelLocation('\u064f', i));
					num++;
				}
				else if (array[i] == '\u0650')
				{
					if (num > 0 && tashkeelLocation[num - 1].tashkeel == '\u0651')
					{
						tashkeelLocation[num - 1].tashkeel = 'ﱢ';
						continue;
					}
					tashkeelLocation.Add(new TashkeelLocation('\u0650', i));
					num++;
				}
				else if (array[i] == '\u0651')
				{
					if (num > 0)
					{
						if (tashkeelLocation[num - 1].tashkeel == '\u064e')
						{
							tashkeelLocation[num - 1].tashkeel = 'ﱠ';
							continue;
						}
						if (tashkeelLocation[num - 1].tashkeel == '\u064f')
						{
							tashkeelLocation[num - 1].tashkeel = 'ﱡ';
							continue;
						}
						if (tashkeelLocation[num - 1].tashkeel == '\u0650')
						{
							tashkeelLocation[num - 1].tashkeel = 'ﱢ';
							continue;
						}
					}
					tashkeelLocation.Add(new TashkeelLocation('\u0651', i));
					num++;
				}
				else if (array[i] == '\u0652')
				{
					tashkeelLocation.Add(new TashkeelLocation('\u0652', i));
					num++;
				}
				else if (array[i] == '\u0653')
				{
					tashkeelLocation.Add(new TashkeelLocation('\u0653', i));
					num++;
				}
			}
			string[] array2 = str.Split('\u064b', '\u064c', '\u064d', '\u064e', '\u064f', '\u0650', '\u0651', '\u0652', '\u0653', 'ﱠ', 'ﱡ', 'ﱢ');
			str = "";
			string[] array3 = array2;
			foreach (string text in array3)
			{
				str += text;
			}
			return str;
		}

		internal static char[] ReturnTashkeel(char[] letters, List<TashkeelLocation> tashkeelLocation)
		{
			char[] array = new char[letters.Length + tashkeelLocation.Count];
			int num = 0;
			for (int i = 0; i < letters.Length; i++)
			{
				array[num] = letters[i];
				num++;
				foreach (TashkeelLocation item in tashkeelLocation)
				{
					if (item.position == num)
					{
						array[num] = item.tashkeel;
						num++;
					}
				}
			}
			return array;
		}

		internal static string FixLine(string str)
		{
			string text = "";
			string text2 = RemoveTashkeel(str, out var tashkeelLocation);
			char[] array = text2.ToCharArray();
			char[] array2 = text2.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (char)ArabicTable.ArabicMapper.Convert(array[i]);
			}
			for (int j = 0; j < array.Length; j++)
			{
				bool flag = false;
				if (array[j] == 'ﻝ' && j < array.Length - 1)
				{
					if (array[j + 1] == 'ﺇ')
					{
						array[j] = 'ﻷ';
						array2[j + 1] = '\uffff';
						flag = true;
					}
					else if (array[j + 1] == 'ﺍ')
					{
						array[j] = 'ﻹ';
						array2[j + 1] = '\uffff';
						flag = true;
					}
					else if (array[j + 1] == 'ﺃ')
					{
						array[j] = 'ﻵ';
						array2[j + 1] = '\uffff';
						flag = true;
					}
					else if (array[j + 1] == 'ﺁ')
					{
						array[j] = 'ﻳ';
						array2[j + 1] = '\uffff';
						flag = true;
					}
				}
				if (!IsIgnoredCharacter(array[j]))
				{
					if (IsMiddleLetter(array, j))
					{
						array2[j] = (char)(array[j] + 3);
					}
					else if (IsFinishingLetter(array, j))
					{
						array2[j] = (char)(array[j] + 1);
					}
					else if (IsLeadingLetter(array, j))
					{
						array2[j] = (char)(array[j] + 2);
					}
				}
				text = text + Convert.ToString(array[j], 16) + " ";
				if (flag)
				{
					j++;
				}
				if (useHinduNumbers)
				{
					if (array[j] == '0')
					{
						array2[j] = '٠';
					}
					else if (array[j] == '1')
					{
						array2[j] = '١';
					}
					else if (array[j] == '2')
					{
						array2[j] = '٢';
					}
					else if (array[j] == '3')
					{
						array2[j] = '٣';
					}
					else if (array[j] == '4')
					{
						array2[j] = '٤';
					}
					else if (array[j] == '5')
					{
						array2[j] = '٥';
					}
					else if (array[j] == '6')
					{
						array2[j] = '٦';
					}
					else if (array[j] == '7')
					{
						array2[j] = '٧';
					}
					else if (array[j] == '8')
					{
						array2[j] = '٨';
					}
					else if (array[j] == '9')
					{
						array2[j] = '٩';
					}
				}
			}
			if (showTashkeel)
			{
				array2 = ReturnTashkeel(array2, tashkeelLocation);
			}
			List<char> list = new List<char>();
			List<char> list2 = new List<char>();
			for (int num = array2.Length - 1; num >= 0; num--)
			{
				if (char.IsPunctuation(array2[num]) && num > 0 && num < array2.Length - 1 && (char.IsPunctuation(array2[num - 1]) || char.IsPunctuation(array2[num + 1])))
				{
					if (array2[num] == '(')
					{
						list.Add(')');
					}
					else if (array2[num] == ')')
					{
						list.Add('(');
					}
					else if (array2[num] == '<')
					{
						list.Add('>');
					}
					else if (array2[num] == '>')
					{
						list.Add('<');
					}
					else if (array2[num] == '[')
					{
						list.Add(']');
					}
					else if (array2[num] == ']')
					{
						list.Add('[');
					}
					else if (array2[num] != '\uffff')
					{
						list.Add(array2[num]);
					}
				}
				else if (array2[num] == ' ' && num > 0 && num < array2.Length - 1 && (char.IsLower(array2[num - 1]) || char.IsUpper(array2[num - 1]) || char.IsNumber(array2[num - 1])) && (char.IsLower(array2[num + 1]) || char.IsUpper(array2[num + 1]) || char.IsNumber(array2[num + 1])))
				{
					list2.Add(array2[num]);
				}
				else if (char.IsNumber(array2[num]) || char.IsLower(array2[num]) || char.IsUpper(array2[num]) || char.IsSymbol(array2[num]) || char.IsPunctuation(array2[num]))
				{
					if (array2[num] == '(')
					{
						list2.Add(')');
					}
					else if (array2[num] == ')')
					{
						list2.Add('(');
					}
					else if (array2[num] == '<')
					{
						list2.Add('>');
					}
					else if (array2[num] == '>')
					{
						list2.Add('<');
					}
					else if (array2[num] == '[')
					{
						list.Add(']');
					}
					else if (array2[num] == ']')
					{
						list.Add('[');
					}
					else
					{
						list2.Add(array2[num]);
					}
				}
				else if ((array2[num] >= '\ud800' && array2[num] <= '\udbff') || (array2[num] >= '\udc00' && array2[num] <= '\udfff'))
				{
					list2.Add(array2[num]);
				}
				else
				{
					if (list2.Count > 0)
					{
						for (int k = 0; k < list2.Count; k++)
						{
							list.Add(list2[list2.Count - 1 - k]);
						}
						list2.Clear();
					}
					if (array2[num] != '\uffff')
					{
						list.Add(array2[num]);
					}
				}
			}
			if (list2.Count > 0)
			{
				for (int l = 0; l < list2.Count; l++)
				{
					list.Add(list2[list2.Count - 1 - l]);
				}
				list2.Clear();
			}
			array2 = new char[list.Count];
			for (int m = 0; m < array2.Length; m++)
			{
				array2[m] = list[m];
			}
			str = new string(array2);
			return str;
		}

		internal static bool IsIgnoredCharacter(char ch)
		{
			bool flag = char.IsPunctuation(ch);
			bool flag2 = char.IsNumber(ch);
			bool flag3 = char.IsLower(ch);
			bool flag4 = char.IsUpper(ch);
			bool flag5 = char.IsSymbol(ch);
			bool flag6 = ch == 'ﭖ' || ch == 'ﭺ' || ch == 'ﮊ' || ch == 'ﮒ' || ch == 'ﮎ';
			bool flag7 = ((ch <= '\ufeff' && ch >= 'ﹰ') | flag6) || ch == 'ﯼ';
			if (!(flag | flag2 | flag3 | flag4 | flag5) && flag7 && ch != 'a' && ch != '>' && ch != '<')
			{
				return ch == '؛';
			}
			return true;
		}

		internal static bool IsLeadingLetter(char[] letters, int index)
		{
			bool num = index == 0 || letters[index - 1] == ' ' || letters[index - 1] == '*' || letters[index - 1] == 'A' || char.IsPunctuation(letters[index - 1]) || letters[index - 1] == '>' || letters[index - 1] == '<' || letters[index - 1] == 'ﺍ' || letters[index - 1] == 'ﺩ' || letters[index - 1] == 'ﺫ' || letters[index - 1] == 'ﺭ' || letters[index - 1] == 'ﺯ' || letters[index - 1] == 'ﮊ' || letters[index - 1] == 'ﻭ' || letters[index - 1] == 'ﺁ' || letters[index - 1] == 'ﺃ' || letters[index - 1] == 'ﺇ' || letters[index - 1] == 'ﺅ';
			bool flag = letters[index] != ' ' && letters[index] != 'ﺩ' && letters[index] != 'ﺫ' && letters[index] != 'ﺭ' && letters[index] != 'ﺯ' && letters[index] != 'ﮊ' && letters[index] != 'ﺍ' && letters[index] != 'ﺃ' && letters[index] != 'ﺇ' && letters[index] != 'ﺁ' && letters[index] != 'ﺅ' && letters[index] != 'ﻭ' && letters[index] != 'ﺀ';
			bool flag2 = index < letters.Length - 1 && letters[index + 1] != ' ' && !char.IsPunctuation(letters[index + 1]) && !char.IsNumber(letters[index + 1]) && !char.IsSymbol(letters[index + 1]) && !char.IsLower(letters[index + 1]) && !char.IsUpper(letters[index + 1]) && letters[index + 1] != 'ﺀ';
			if (num & flag & flag2)
			{
				return true;
			}
			return false;
		}

		internal static bool IsFinishingLetter(char[] letters, int index)
		{
			bool num = index != 0 && letters[index - 1] != ' ' && letters[index - 1] != 'ﺩ' && letters[index - 1] != 'ﺫ' && letters[index - 1] != 'ﺭ' && letters[index - 1] != 'ﺯ' && letters[index - 1] != 'ﮊ' && letters[index - 1] != 'ﻭ' && letters[index - 1] != 'ﺍ' && letters[index - 1] != 'ﺁ' && letters[index - 1] != 'ﺃ' && letters[index - 1] != 'ﺇ' && letters[index - 1] != 'ﺅ' && letters[index - 1] != 'ﺀ' && !char.IsPunctuation(letters[index - 1]) && letters[index - 1] != '>' && letters[index - 1] != '<';
			bool flag = letters[index] != ' ' && letters[index] != 'ﺀ';
			if (num & flag)
			{
				return true;
			}
			return false;
		}

		internal static bool IsMiddleLetter(char[] letters, int index)
		{
			bool flag = index != 0 && letters[index] != 'ﺍ' && letters[index] != 'ﺩ' && letters[index] != 'ﺫ' && letters[index] != 'ﺭ' && letters[index] != 'ﺯ' && letters[index] != 'ﮊ' && letters[index] != 'ﻭ' && letters[index] != 'ﺁ' && letters[index] != 'ﺃ' && letters[index] != 'ﺇ' && letters[index] != 'ﺅ' && letters[index] != 'ﺀ';
			bool flag2 = index != 0 && letters[index - 1] != 'ﺍ' && letters[index - 1] != 'ﺩ' && letters[index - 1] != 'ﺫ' && letters[index - 1] != 'ﺭ' && letters[index - 1] != 'ﺯ' && letters[index - 1] != 'ﮊ' && letters[index - 1] != 'ﻭ' && letters[index - 1] != 'ﺁ' && letters[index - 1] != 'ﺃ' && letters[index - 1] != 'ﺇ' && letters[index - 1] != 'ﺅ' && letters[index - 1] != 'ﺀ' && !char.IsPunctuation(letters[index - 1]) && letters[index - 1] != '>' && letters[index - 1] != '<' && letters[index - 1] != ' ' && letters[index - 1] != '*';
			if ((index < letters.Length - 1 && letters[index + 1] != ' ' && letters[index + 1] != '\r' && letters[index + 1] != 'ﺀ' && !char.IsNumber(letters[index + 1]) && !char.IsSymbol(letters[index + 1]) && !char.IsPunctuation(letters[index + 1])) & flag2 & flag)
			{
				try
				{
					if (char.IsPunctuation(letters[index + 1]))
					{
						return false;
					}
					return true;
				}
				catch
				{
					return false;
				}
			}
			return false;
		}
	}
	[AddComponentMenu("I2/Localization/SetLanguage Button")]
	public class SetLanguage : MonoBehaviour
	{
		public string _Language;

		private void OnClick()
		{
			ApplyLanguage();
		}

		public void ApplyLanguage()
		{
			if (LocalizationManager.HasLanguage(_Language))
			{
				LocalizationManager.CurrentLanguage = _Language;
			}
		}
	}
	[AddComponentMenu("I2/Localization/SetLanguage Dropdown")]
	public class SetLanguageDropdown : MonoBehaviour
	{
		private void OnEnable()
		{
			Dropdown component = ((Component)this).GetComponent<Dropdown>();
			if (!((Object)(object)component == (Object)null))
			{
				string currentLanguage = LocalizationManager.CurrentLanguage;
				if (LocalizationManager.Sources.Count == 0)
				{
					LocalizationManager.UpdateSources();
				}
				List<string> allLanguages = LocalizationManager.GetAllLanguages();
				component.ClearOptions();
				component.AddOptions(allLanguages);
				component.value = allLanguages.IndexOf(currentLanguage);
				((UnityEvent<int>)(object)component.onValueChanged).RemoveListener((UnityAction<int>)OnValueChanged);
				((UnityEvent<int>)(object)component.onValueChanged).AddListener((UnityAction<int>)OnValueChanged);
			}
		}

		private void OnValueChanged(int index)
		{
			Dropdown component = ((Component)this).GetComponent<Dropdown>();
			if (index < 0)
			{
				index = 0;
				component.value = index;
			}
			LocalizationManager.CurrentLanguage = component.options[index].text;
		}
	}
	public class StringObfucator
	{
		public static char[] StringObfuscatorPassword = "ÝúbUu\u00b8CÁÂ§*4PÚ©-á©¾@T6Dl±ÒWâuzÅm4GÐóØ$=Íg,¥Që®iKEßr¡×60Ít4öÃ~^«y:Èd1<QÛÝúbUu\u00b8CÁÂ§*4PÚ©-á©¾@T6Dl±ÒWâuzÅm4GÐóØ$=Íg,¥Që®iKEßr¡×60Ít4öÃ~^«y:Èd".ToCharArray();

		public static string Encode(string NormalString)
		{
			try
			{
				return ToBase64(XoREncode(NormalString));
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static string Decode(string ObfucatedString)
		{
			try
			{
				return XoREncode(FromBase64(ObfucatedString));
			}
			catch (Exception)
			{
				return null;
			}
		}

		private static string ToBase64(string regularString)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(regularString));
		}

		private static string FromBase64(string base64string)
		{
			byte[] array = Convert.FromBase64String(base64string);
			return Encoding.UTF8.GetString(array, 0, array.Length);
		}

		private static string XoREncode(string NormalString)
		{
			try
			{
				char[] stringObfuscatorPassword = StringObfuscatorPassword;
				char[] array = NormalString.ToCharArray();
				int num = stringObfuscatorPassword.Length;
				int i = 0;
				for (int num2 = array.Length; i < num2; i++)
				{
					array[i] = (char)(array[i] ^ stringObfuscatorPassword[i % num] ^ (byte)((i % 2 == 0) ? (i * 23) : (-i * 51)));
				}
				return new string(array);
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
namespace I2.Loc.SimpleJSON
{
	public enum JSONBinaryTag
	{
		Array = 1,
		Class,
		Value,
		IntValue,
		DoubleValue,
		BoolValue,
		FloatValue
	}
	public class JSONNode
	{
		public virtual JSONNode this[int aIndex]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual JSONNode this[string aKey]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual string Value
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public virtual int Count => 0;

		public virtual IEnumerable<JSONNode> Childs
		{
			get
			{
				yield break;
			}
		}

		public IEnumerable<JSONNode> DeepChilds
		{
			get
			{
				foreach (JSONNode child in Childs)
				{
					foreach (JSONNode deepChild in child.DeepChilds)
					{
						yield return deepChild;
					}
				}
			}
		}

		public virtual int AsInt
		{
			get
			{
				int result = 0;
				if (int.TryParse(Value, out result))
				{
					return result;
				}
				return 0;
			}
			set
			{
				Value = value.ToString();
			}
		}

		public virtual float AsFloat
		{
			get
			{
				float result = 0f;
				if (float.TryParse(Value, out result))
				{
					return result;
				}
				return 0f;
			}
			set
			{
				Value = value.ToString();
			}
		}

		public virtual double AsDouble
		{
			get
			{
				double result = 0.0;
				if (double.TryParse(Value, out result))
				{
					return result;
				}
				return 0.0;
			}
			set
			{
				Value = value.ToString();
			}
		}

		public virtual bool AsBool
		{
			get
			{
				bool result = false;
				if (bool.TryParse(Value, out result))
				{
					return result;
				}
				return !string.IsNullOrEmpty(Value);
			}
			set
			{
				Value = (value ? "true" : "false");
			}
		}

		public virtual JSONArray AsArray => this as JSONArray;

		public virtual JSONClass AsObject => this as JSONClass;

		public virtual void Add(string aKey, JSONNode aItem)
		{
		}

		public virtual void Add(JSONNode aItem)
		{
			Add("", aItem);
		}

		public virtual JSONNode Remove(string aKey)
		{
			return null;
		}

		public virtual JSONNode Remove(int aIndex)
		{
			return null;
		}

		public virtual JSONNode Remove(JSONNode aNode)
		{
			return aNode;
		}

		public override string ToString()
		{
			return "JSONNode";
		}

		public virtual string ToString(string aPrefix)
		{
			return "JSONNode";
		}

		public static implicit operator JSONNode(string s)
		{
			return new JSONData(s);
		}

		public static implicit operator string(JSONNode d)
		{
			if (!(d == null))
			{
				return d.Value;
			}
			return null;
		}

		public static bool operator ==(JSONNode a, object b)
		{
			if (b == null && a is JSONLazyCreator)
			{
				return true;
			}
			return (object)a == b;
		}

		public static bool operator !=(JSONNode a, object b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return (object)this == obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		internal static string Escape(string aText)
		{
			string text = "";
			for (int i = 0; i < aText.Length; i++)
			{
				char c = aText[i];
				text;
				switch (c)
				{
					case '\\':
						text = text + "\\\\";
						break;
					case '"':
						text = text + "\\\"";
						break;
					case '\n':
						text = text + "\\n";
						break;
					case '\r':
						text = text + "\\r";
						break;
					case '\t':
						text = text + "\\t";
						break;
					case '\b':
						text = text + "\\b";
						break;
					case '\f':
						text = text + "\\f";
						break;
					default:
						text = text + c;
						break;
				}
			}
			return text;
		}

		public static JSONNode Parse(string aJSON)
		{
			Stack<JSONNode> stack = new Stack<JSONNode>();
			JSONNode jSONNode = null;
			int i = 0;
			string text = "";
			string text2 = "";
			bool flag = false;
			for (; i < aJSON.Length; i++)
			{
				switch (aJSON[i])
				{
				case '{':
					if (flag)
					{
						text += aJSON[i];
						break;
					}
					stack.Push(new JSONClass());
					if (jSONNode != null)
					{
						text2 = text2.Trim();
						if (jSONNode is JSONArray)
						{
							jSONNode.Add(stack.Peek());
						}
						else if (text2 != "")
						{
							jSONNode.Add(text2, stack.Peek());
						}
					}
					text2 = "";
					text = "";
					jSONNode = stack.Peek();
					break;
				case '[':
					if (flag)
					{
						text += aJSON[i];
						break;
					}
					stack.Push(new JSONArray());
					if (jSONNode != null)
					{
						text2 = text2.Trim();
						if (jSONNode is JSONArray)
						{
							jSONNode.Add(stack.Peek());
						}
						else if (text2 != "")
						{
							jSONNode.Add(text2, stack.Peek());
						}
					}
					text2 = "";
					text = "";
					jSONNode = stack.Peek();
					break;
				case ']':
				case '}':
					if (flag)
					{
						text += aJSON[i];
						break;
					}
					if (stack.Count == 0)
					{
						throw new Exception("JSON Parse: Too many closing brackets");
					}
					stack.Pop();
					if (text != "")
					{
						text2 = text2.Trim();
						if (jSONNode is JSONArray)
						{
							jSONNode.Add(text);
						}
						else if (text2 != "")
						{
							jSONNode.Add(text2, text);
						}
					}
					text2 = "";
					text = "";
					if (stack.Count > 0)
					{
						jSONNode = stack.Peek();
					}
					break;
				case ':':
					if (flag)
					{
						text += aJSON[i];
						break;
					}
					text2 = text;
					text = "";
					break;
				case '"':
					flag = !flag;
					break;
				case ',':
					if (flag)
					{
						text += aJSON[i];
						break;
					}
					if (text != "")
					{
						if (jSONNode is JSONArray)
						{
							jSONNode.Add(text);
						}
						else if (text2 != "")
						{
							jSONNode.Add(text2, text);
						}
					}
					text2 = "";
					text = "";
					break;
				case '\t':
				case ' ':
					if (flag)
					{
						text += aJSON[i];
					}
					break;
				case '\\':
					i++;
					if (flag)
					{
						char c = aJSON[i];
						switch (c)
						{
						case 't':
							text += "\t";
							break;
						case 'r':
							text += "\r";
							break;
						case 'n':
							text += "\n";
							break;
						case 'b':
							text += "\b";
							break;
						case 'f':
							text += "\f";
							break;
						case 'u':
						{
							string s = aJSON.Substring(i + 1, 4);
							text += (char)int.Parse(s, NumberStyles.AllowHexSpecifier);
							i += 4;
							break;
						}
						default:
							text += c;
							break;
						}
					}
					break;
				default:
					text += aJSON[i];
					break;
				case '\n':
				case '\r':
					break;
				}
			}
			if (flag)
			{
				throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
			}
			return jSONNode;
		}

		public virtual void Serialize(BinaryWriter aWriter)
		{
		}

		public void SaveToStream(Stream aData)
		{
			BinaryWriter aWriter = new BinaryWriter(aData);
			Serialize(aWriter);
		}

		public void SaveToCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public void SaveToCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public string SaveToCompressedBase64()
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public void SaveToFile(string aFileName)
		{
			Directory.CreateDirectory(new FileInfo(aFileName).Directory.FullName);
			using (FileStream aData = File.OpenWrite(aFileName))
			{
				SaveToStream(aData);
			}
		}

		public string SaveToBase64()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				SaveToStream(memoryStream);
				memoryStream.Position = 0L;
				return Convert.ToBase64String(memoryStream.ToArray());
			}
		}

		public static JSONNode Deserialize(BinaryReader aReader)
		{
			JSONBinaryTag jSONBinaryTag = (JSONBinaryTag)aReader.ReadByte();
			switch (jSONBinaryTag)
			{
			case JSONBinaryTag.Array:
			{
				int num2 = aReader.ReadInt32();
				JSONArray jSONArray = new JSONArray();
				for (int j = 0; j < num2; j++)
				{
					jSONArray.Add(Deserialize(aReader));
				}
				return jSONArray;
			}
			case JSONBinaryTag.Class:
			{
				int num = aReader.ReadInt32();
				JSONClass jSONClass = new JSONClass();
				for (int i = 0; i < num; i++)
				{
					string aKey = aReader.ReadString();
					JSONNode aItem = Deserialize(aReader);
					jSONClass.Add(aKey, aItem);
				}
				return jSONClass;
			}
			case JSONBinaryTag.Value:
				return new JSONData(aReader.ReadString());
			case JSONBinaryTag.IntValue:
				return new JSONData(aReader.ReadInt32());
			case JSONBinaryTag.DoubleValue:
				return new JSONData(aReader.ReadDouble());
			case JSONBinaryTag.BoolValue:
				return new JSONData(aReader.ReadBoolean());
			case JSONBinaryTag.FloatValue:
				return new JSONData(aReader.ReadSingle());
			default:
				throw new Exception("Error deserializing JSON. Unknown tag: " + jSONBinaryTag);
			}
		}

		public static JSONNode LoadFromCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static JSONNode LoadFromCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static JSONNode LoadFromCompressedBase64(string aBase64)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static JSONNode LoadFromStream(Stream aData)
		{
			using (BinaryReader aReader = new BinaryReader(aData))
			{
				return Deserialize(aReader);
			}
		}

		public static JSONNode LoadFromFile(string aFileName)
		{
			using (FileStream aData = File.OpenRead(aFileName))
			{
				return LoadFromStream(aData);
			}
		}

		public static JSONNode LoadFromBase64(string aBase64)
		{
			return LoadFromStream(new MemoryStream(Convert.FromBase64String(aBase64))
			{
				Position = 0L
			});
		}
	}
	public class JSONArray : JSONNode, IEnumerable
	{
		private List<JSONNode> m_List = new List<JSONNode>();

		public override JSONNode this[int aIndex]
		{
			get
			{
				if (aIndex < 0 || aIndex >= m_List.Count)
				{
					return new JSONLazyCreator(this);
				}
				return m_List[aIndex];
			}
			set
			{
				if (aIndex < 0 || aIndex >= m_List.Count)
				{
					m_List.Add(value);
				}
				else
				{
					m_List[aIndex] = value;
				}
			}
		}

		public override JSONNode this[string aKey]
		{
			get
			{
				return new JSONLazyCreator(this);
			}
			set
			{
				m_List.Add(value);
			}
		}

		public override int Count => m_List.Count;

		public override IEnumerable<JSONNode> Childs
		{
			get
			{
				foreach (JSONNode item in m_List)
				{
					yield return item;
				}
			}
		}

		public override void Add(string aKey, JSONNode aItem)
		{
			m_List.Add(aItem);
		}

		public override JSONNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= m_List.Count)
			{
				return null;
			}
			JSONNode result = m_List[aIndex];
			m_List.RemoveAt(aIndex);
			return result;
		}

		public override JSONNode Remove(JSONNode aNode)
		{
			m_List.Remove(aNode);
			return aNode;
		}

		public IEnumerator GetEnumerator()
		{
			foreach (JSONNode item in m_List)
			{
				yield return item;
			}
		}

		public override string ToString()
		{
			string text = "[ ";
			foreach (JSONNode item in m_List)
			{
				if (text.Length > 2)
				{
					text += ", ";
				}
				text += item.ToString();
			}
			return text + " ]";
		}

		public override string ToString(string aPrefix)
		{
			string text = "[ ";
			foreach (JSONNode item in m_List)
			{
				if (text.Length > 3)
				{
					text += ", ";
				}
				text = text + "\n" + aPrefix + "   ";
				text += item.ToString(aPrefix + "   ");
			}
			return text + "\n" + aPrefix + "]";
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write((byte)1);
			aWriter.Write(m_List.Count);
			for (int i = 0; i < m_List.Count; i++)
			{
				m_List[i].Serialize(aWriter);
			}
		}
	}
	public class JSONClass : JSONNode, IEnumerable
	{
		private Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>(StringComparer.Ordinal);

		public override JSONNode this[string aKey]
		{
			get
			{
				if (m_Dict.ContainsKey(aKey))
				{
					return m_Dict[aKey];
				}
				return new JSONLazyCreator(this, aKey);
			}
			set
			{
				if (m_Dict.ContainsKey(aKey))
				{
					m_Dict[aKey] = value;
				}
				else
				{
					m_Dict.Add(aKey, value);
				}
			}
		}

		public override JSONNode this[int aIndex]
		{
			get
			{
				if (aIndex < 0 || aIndex >= m_Dict.Count)
				{
					return null;
				}
				return m_Dict.ElementAt(aIndex).Value;
			}
			set
			{
				if (aIndex >= 0 && aIndex < m_Dict.Count)
				{
					string key = m_Dict.ElementAt(aIndex).Key;
					m_Dict[key] = value;
				}
			}
		}

		public override int Count => m_Dict.Count;

		public override IEnumerable<JSONNode> Childs
		{
			get
			{
				foreach (KeyValuePair<string, JSONNode> item in m_Dict)
				{
					yield return item.Value;
				}
			}
		}

		public override void Add(string aKey, JSONNode aItem)
		{
			if (!string.IsNullOrEmpty(aKey))
			{
				if (m_Dict.ContainsKey(aKey))
				{
					m_Dict[aKey] = aItem;
				}
				else
				{
					m_Dict.Add(aKey, aItem);
				}
			}
			else
			{
				m_Dict.Add(Guid.NewGuid().ToString(), aItem);
			}
		}

		public override JSONNode Remove(string aKey)
		{
			if (!m_Dict.ContainsKey(aKey))
			{
				return null;
			}
			JSONNode result = m_Dict[aKey];
			m_Dict.Remove(aKey);
			return result;
		}

		public override JSONNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= m_Dict.Count)
			{
				return null;
			}
			KeyValuePair<string, JSONNode> keyValuePair = m_Dict.ElementAt(aIndex);
			m_Dict.Remove(keyValuePair.Key);
			return keyValuePair.Value;
		}

		public override JSONNode Remove(JSONNode aNode)
		{
			try
			{
				KeyValuePair<string, JSONNode> keyValuePair = m_Dict.Where((KeyValuePair<string, JSONNode> k) => k.Value == aNode).First();
				m_Dict.Remove(keyValuePair.Key);
				return aNode;
			}
			catch
			{
				return null;
			}
		}

		public IEnumerator GetEnumerator()
		{
			foreach (KeyValuePair<string, JSONNode> item in m_Dict)
			{
				yield return item;
			}
		}

		public override string ToString()
		{
			string text = "{";
			foreach (KeyValuePair<string, JSONNode> item in m_Dict)
			{
				if (text.Length > 2)
				{
					text += ", ";
				}
				text = text + "\"" + JSONNode.Escape(item.Key) + "\":" + item.Value.ToString();
			}
			return text + "}";
		}

		public override string ToString(string aPrefix)
		{
			string text = "{ ";
			foreach (KeyValuePair<string, JSONNode> item in m_Dict)
			{
				if (text.Length > 3)
				{
					text += ", ";
				}
				text = text + "\n" + aPrefix + "   ";
				text = text + "\"" + JSONNode.Escape(item.Key) + "\" : " + item.Value.ToString(aPrefix + "   ");
			}
			return text + "\n" + aPrefix + "}";
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write((byte)2);
			aWriter.Write(m_Dict.Count);
			foreach (string key in m_Dict.Keys)
			{
				aWriter.Write(key);
				m_Dict[key].Serialize(aWriter);
			}
		}
	}
	public class JSONData : JSONNode
	{
		private string m_Data;

		public override string Value
		{
			get
			{
				return m_Data;
			}
			set
			{
				m_Data = value;
			}
		}

		public JSONData(string aData)
		{
			m_Data = aData;
		}

		public JSONData(float aData)
		{
			AsFloat = aData;
		}

		public JSONData(double aData)
		{
			AsDouble = aData;
		}

		public JSONData(bool aData)
		{
			AsBool = aData;
		}

		public JSONData(int aData)
		{
			AsInt = aData;
		}

		public override string ToString()
		{
			return "\"" + JSONNode.Escape(m_Data) + "\"";
		}

		public override string ToString(string aPrefix)
		{
			return "\"" + JSONNode.Escape(m_Data) + "\"";
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			JSONData jSONData = new JSONData("");
			jSONData.AsInt = AsInt;
			if (jSONData.m_Data == m_Data)
			{
				aWriter.Write((byte)4);
				aWriter.Write(AsInt);
				return;
			}
			jSONData.AsFloat = AsFloat;
			if (jSONData.m_Data == m_Data)
			{
				aWriter.Write((byte)7);
				aWriter.Write(AsFloat);
				return;
			}
			jSONData.AsDouble = AsDouble;
			if (jSONData.m_Data == m_Data)
			{
				aWriter.Write((byte)5);
				aWriter.Write(AsDouble);
				return;
			}
			jSONData.AsBool = AsBool;
			if (jSONData.m_Data == m_Data)
			{
				aWriter.Write((byte)6);
				aWriter.Write(AsBool);
			}
			else
			{
				aWriter.Write((byte)3);
				aWriter.Write(m_Data);
			}
		}
	}
	internal class JSONLazyCreator : JSONNode
	{
		private JSONNode m_Node;

		private string m_Key;

		public override JSONNode this[int aIndex]
		{
			get
			{
				return new JSONLazyCreator(this);
			}
			set
			{
				JSONArray jSONArray = new JSONArray();
				jSONArray.Add(value);
				Set(jSONArray);
			}
		}

		public override JSONNode this[string aKey]
		{
			get
			{
				return new JSONLazyCreator(this, aKey);
			}
			set
			{
				JSONClass jSONClass = new JSONClass();
				jSONClass.Add(aKey, value);
				Set(jSONClass);
			}
		}

		public override int AsInt
		{
			get
			{
				JSONData aVal = new JSONData(0);
				Set(aVal);
				return 0;
			}
			set
			{
				JSONData aVal = new JSONData(value);
				Set(aVal);
			}
		}

		public override float AsFloat
		{
			get
			{
				JSONData aVal = new JSONData(0f);
				Set(aVal);
				return 0f;
			}
			set
			{
				JSONData aVal = new JSONData(value);
				Set(aVal);
			}
		}

		public override double AsDouble
		{
			get
			{
				JSONData aVal = new JSONData(0.0);
				Set(aVal);
				return 0.0;
			}
			set
			{
				JSONData aVal = new JSONData(value);
				Set(aVal);
			}
		}

		public override bool AsBool
		{
			get
			{
				JSONData aVal = new JSONData(aData: false);
				Set(aVal);
				return false;
			}
			set
			{
				JSONData aVal = new JSONData(value);
				Set(aVal);
			}
		}

		public override JSONArray AsArray
		{
			get
			{
				JSONArray jSONArray = new JSONArray();
				Set(jSONArray);
				return jSONArray;
			}
		}

		public override JSONClass AsObject
		{
			get
			{
				JSONClass jSONClass = new JSONClass();
				Set(jSONClass);
				return jSONClass;
			}
		}

		public JSONLazyCreator(JSONNode aNode)
		{
			m_Node = aNode;
			m_Key = null;
		}

		public JSONLazyCreator(JSONNode aNode, string aKey)
		{
			m_Node = aNode;
			m_Key = aKey;
		}

		private void Set(JSONNode aVal)
		{
			if (m_Key == null)
			{
				m_Node.Add(aVal);
			}
			else
			{
				m_Node.Add(m_Key, aVal);
			}
			m_Node = null;
		}

		public override void Add(JSONNode aItem)
		{
			JSONArray jSONArray = new JSONArray();
			jSONArray.Add(aItem);
			Set(jSONArray);
		}

		public override void Add(string aKey, JSONNode aItem)
		{
			JSONClass jSONClass = new JSONClass();
			jSONClass.Add(aKey, aItem);
			Set(jSONClass);
		}

		public static bool operator ==(JSONLazyCreator a, object b)
		{
			if (b == null)
			{
				return true;
			}
			return (object)a == b;
		}

		public static bool operator !=(JSONLazyCreator a, object b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return true;
			}
			return (object)this == obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return "";
		}

		public override string ToString(string aPrefix)
		{
			return "";
		}
	}
	public static class JSON
	{
		public static JSONNode Parse(string aJSON)
		{
			return JSONNode.Parse(aJSON);
		}
	}
}
namespace Moments
{
	internal sealed class Worker
	{
		private static int workerId = 1;

		private Thread m_Thread;

		private int m_Id;

		internal List<GifFrame> m_Frames;

		internal GifEncoder m_Encoder;

		internal string m_FilePath;

		internal Action<int, string> m_OnFileSaved;

		internal Action<int, float> m_OnFileSaveProgress;

		internal Worker(ThreadPriority priority)
		{
			m_Id = workerId++;
			m_Thread = new Thread(Run);
			m_Thread.Priority = priority;
		}

		internal void Start()
		{
			m_Thread.Start();
		}

		private void Run()
		{
			m_Encoder.Start(m_FilePath);
			for (int i = 0; i < m_Frames.Count; i++)
			{
				if (m_OnFileSaveProgress != null)
				{
					float arg = (float)i / (float)m_Frames.Count;
					m_OnFileSaveProgress(m_Id, arg);
				}
				GifFrame frame = m_Frames[i];
				m_Encoder.AddFrame(frame);
			}
			m_Encoder.Finish();
			if (m_OnFileSaved != null)
			{
				m_OnFileSaved(m_Id, m_FilePath);
			}
		}
	}
}
namespace Moments.Encoder
{
	public class GifEncoder
	{
		protected int m_Width;

		protected int m_Height;

		protected int m_Repeat = -1;

		protected int m_FrameDelay;

		protected bool m_HasStarted;

		protected FileStream m_FileStream;

		protected GifFrame m_CurrentFrame;

		protected byte[] m_Pixels;

		protected byte[] m_IndexedPixels;

		protected int m_ColorDepth;

		protected byte[] m_ColorTab;

		protected bool[] m_UsedEntry = new bool[256];

		protected int m_PaletteSize = 7;

		protected int m_DisposalCode = -1;

		protected bool m_ShouldCloseStream;

		protected bool m_IsFirstFrame = true;

		protected bool m_IsSizeSet;

		protected int m_SampleInterval = 10;

		public GifEncoder()
			: this(-1, 10)
		{
		}

		public GifEncoder(int repeat, int quality)
		{
			if (repeat >= 0)
			{
				m_Repeat = repeat;
			}
			m_SampleInterval = Mathf.Clamp(quality, 1, 100);
		}

		public void SetDelay(int ms)
		{
			m_FrameDelay = Mathf.RoundToInt((float)ms / 10f);
		}

		public void SetFrameRate(float fps)
		{
			if (fps > 0f)
			{
				m_FrameDelay = Mathf.RoundToInt(100f / fps);
			}
		}

		public void AddFrame(GifFrame frame)
		{
			if (frame == null)
			{
				throw new ArgumentNullException("Can't add a null frame to the gif.");
			}
			if (!m_HasStarted)
			{
				throw new InvalidOperationException("Call Start() before adding frames to the gif.");
			}
			if (!m_IsSizeSet)
			{
				SetSize(frame.Width, frame.Height);
			}
			m_CurrentFrame = frame;
			GetImagePixels();
			AnalyzePixels();
			if (m_IsFirstFrame)
			{
				WriteLSD();
				WritePalette();
				if (m_Repeat >= 0)
				{
					WriteNetscapeExt();
				}
			}
			WriteGraphicCtrlExt();
			WriteImageDesc();
			if (!m_IsFirstFrame)
			{
				WritePalette();
			}
			WritePixels();
			m_IsFirstFrame = false;
		}

		public void Start(FileStream os)
		{
			if (os == null)
			{
				throw new ArgumentNullException("Stream is null.");
			}
			m_ShouldCloseStream = false;
			m_FileStream = os;
			try
			{
				WriteString("GIF89a");
			}
			catch (IOException ex)
			{
				throw ex;
			}
			m_HasStarted = true;
		}

		public void Start(string file)
		{
			try
			{
				m_FileStream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
				Start(m_FileStream);
				m_ShouldCloseStream = true;
			}
			catch (IOException ex)
			{
				throw ex;
			}
		}

		public void Finish()
		{
			if (!m_HasStarted)
			{
				throw new InvalidOperationException("Can't finish a non-started gif.");
			}
			m_HasStarted = false;
			try
			{
				m_FileStream.WriteByte(59);
				m_FileStream.Flush();
				if (m_ShouldCloseStream)
				{
					m_FileStream.Close();
				}
			}
			catch (IOException ex)
			{
				throw ex;
			}
			m_FileStream = null;
			m_CurrentFrame = null;
			m_Pixels = null;
			m_IndexedPixels = null;
			m_ColorTab = null;
			m_ShouldCloseStream = false;
			m_IsFirstFrame = true;
		}

		protected void SetSize(int w, int h)
		{
			m_Width = w;
			m_Height = h;
			m_IsSizeSet = true;
		}

		protected void GetImagePixels()
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			m_Pixels = new byte[3 * m_CurrentFrame.Width * m_CurrentFrame.Height];
			Color32[] data = m_CurrentFrame.Data;
			int num = 0;
			for (int num2 = m_CurrentFrame.Height - 1; num2 >= 0; num2--)
			{
				for (int i = 0; i < m_CurrentFrame.Width; i++)
				{
					Color32 val = data[num2 * m_CurrentFrame.Width + i];
					m_Pixels[num] = val.r;
					num++;
					m_Pixels[num] = val.g;
					num++;
					m_Pixels[num] = val.b;
					num++;
				}
			}
		}

		protected void AnalyzePixels()
		{
			int num = m_Pixels.Length;
			int num2 = num / 3;
			m_IndexedPixels = new byte[num2];
			NeuQuant neuQuant = new NeuQuant(m_Pixels, num, m_SampleInterval);
			m_ColorTab = neuQuant.Process();
			int num3 = 0;
			for (int i = 0; i < num2; i++)
			{
				int num4 = neuQuant.Map(m_Pixels[num3++] & 0xFF, m_Pixels[num3++] & 0xFF, m_Pixels[num3++] & 0xFF);
				m_UsedEntry[num4] = true;
				m_IndexedPixels[i] = (byte)num4;
			}
			m_Pixels = null;
			m_ColorDepth = 8;
			m_PaletteSize = 7;
		}

		protected void WriteGraphicCtrlExt()
		{
			m_FileStream.WriteByte(33);
			m_FileStream.WriteByte(249);
			m_FileStream.WriteByte(4);
			m_FileStream.WriteByte(Convert.ToByte(0));
			WriteShort(m_FrameDelay);
			m_FileStream.WriteByte(Convert.ToByte(0));
			m_FileStream.WriteByte(0);
		}

		protected void WriteImageDesc()
		{
			m_FileStream.WriteByte(44);
			WriteShort(0);
			WriteShort(0);
			WriteShort(m_Width);
			WriteShort(m_Height);
			if (m_IsFirstFrame)
			{
				m_FileStream.WriteByte(0);
			}
			else
			{
				m_FileStream.WriteByte(Convert.ToByte(0x80 | m_PaletteSize));
			}
		}

		protected void WriteLSD()
		{
			WriteShort(m_Width);
			WriteShort(m_Height);
			m_FileStream.WriteByte(Convert.ToByte(0xF0 | m_PaletteSize));
			m_FileStream.WriteByte(0);
			m_FileStream.WriteByte(0);
		}

		protected void WriteNetscapeExt()
		{
			m_FileStream.WriteByte(33);
			m_FileStream.WriteByte(byte.MaxValue);
			m_FileStream.WriteByte(11);
			WriteString("NETSCAPE2.0");
			m_FileStream.WriteByte(3);
			m_FileStream.WriteByte(1);
			WriteShort(m_Repeat);
			m_FileStream.WriteByte(0);
		}

		protected void WritePalette()
		{
			m_FileStream.Write(m_ColorTab, 0, m_ColorTab.Length);
			int num = 768 - m_ColorTab.Length;
			for (int i = 0; i < num; i++)
			{
				m_FileStream.WriteByte(0);
			}
		}

		protected void WritePixels()
		{
			new LzwEncoder(m_Width, m_Height, m_IndexedPixels, m_ColorDepth).Encode(m_FileStream);
		}

		protected void WriteShort(int value)
		{
			m_FileStream.WriteByte(Convert.ToByte(value & 0xFF));
			m_FileStream.WriteByte(Convert.ToByte((value >> 8) & 0xFF));
		}

		protected void WriteString(string s)
		{
			char[] array = s.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				m_FileStream.WriteByte((byte)array[i]);
			}
		}
	}
	public class GifFrame
	{
		public int Width;

		public int Height;

		public Color32[] Data;
	}
	public class LzwEncoder
	{
		private static readonly int EOF = -1;

		private byte[] pixAry;

		private int initCodeSize;

		private int curPixel;

		private static readonly int BITS = 12;

		private static readonly int HSIZE = 5003;

		private int n_bits;

		private int maxbits = BITS;

		private int maxcode;

		private int maxmaxcode = 1 << BITS;

		private int[] htab = new int[HSIZE];

		private int[] codetab = new int[HSIZE];

		private int hsize = HSIZE;

		private int free_ent;

		private bool clear_flg;

		private int g_init_bits;

		private int ClearCode;

		private int EOFCode;

		private int cur_accum;

		private int cur_bits;

		private int[] masks = new int[17]
		{
			0, 1, 3, 7, 15, 31, 63, 127, 255, 511,
			1023, 2047, 4095, 8191, 16383, 32767, 65535
		};

		private int a_count;

		private byte[] accum = new byte[256];

		public LzwEncoder(int width, int height, byte[] pixels, int color_depth)
		{
			pixAry = pixels;
			initCodeSize = Math.Max(2, color_depth);
		}

		private void Add(byte c, Stream outs)
		{
			accum[a_count++] = c;
			if (a_count >= 254)
			{
				Flush(outs);
			}
		}

		private void ClearTable(Stream outs)
		{
			ResetCodeTable(hsize);
			free_ent = ClearCode + 2;
			clear_flg = true;
			Output(ClearCode, outs);
		}

		private void ResetCodeTable(int hsize)
		{
			for (int i = 0; i < hsize; i++)
			{
				htab[i] = -1;
			}
		}

		private void Compress(int init_bits, Stream outs)
		{
			g_init_bits = init_bits;
			clear_flg = false;
			n_bits = g_init_bits;
			maxcode = MaxCode(n_bits);
			ClearCode = 1 << init_bits - 1;
			EOFCode = ClearCode + 1;
			free_ent = ClearCode + 2;
			a_count = 0;
			int num = NextPixel();
			int num2 = 0;
			for (int num3 = hsize; num3 < 65536; num3 *= 2)
			{
				num2++;
			}
			num2 = 8 - num2;
			int num4 = hsize;
			ResetCodeTable(num4);
			Output(ClearCode, outs);
			int num5;
			while ((num5 = NextPixel()) != EOF)
			{
				int num3 = (num5 << maxbits) + num;
				int num6 = (num5 << num2) ^ num;
				if (htab[num6] == num3)
				{
					num = codetab[num6];
					continue;
				}
				if (htab[num6] >= 0)
				{
					int num7 = num4 - num6;
					if (num6 == 0)
					{
						num7 = 1;
					}
					while (true)
					{
						if ((num6 -= num7) < 0)
						{
							num6 += num4;
						}
						if (htab[num6] == num3)
						{
							break;
						}
						if (htab[num6] >= 0)
						{
							continue;
						}
						goto IL_0121;
					}
					num = codetab[num6];
					continue;
				}
				goto IL_0121;
				IL_0121:
				Output(num, outs);
				num = num5;
				if (free_ent < maxmaxcode)
				{
					codetab[num6] = free_ent++;
					htab[num6] = num3;
				}
				else
				{
					ClearTable(outs);
				}
			}
			Output(num, outs);
			Output(EOFCode, outs);
		}

		public void Encode(Stream os)
		{
			os.WriteByte(Convert.ToByte(initCodeSize));
			curPixel = 0;
			Compress(initCodeSize + 1, os);
			os.WriteByte(0);
		}

		private void Flush(Stream outs)
		{
			if (a_count > 0)
			{
				outs.WriteByte(Convert.ToByte(a_count));
				outs.Write(accum, 0, a_count);
				a_count = 0;
			}
		}

		private int MaxCode(int n_bits)
		{
			return (1 << n_bits) - 1;
		}

		private int NextPixel()
		{
			if (curPixel == pixAry.Length)
			{
				return EOF;
			}
			curPixel++;
			return pixAry[curPixel - 1] & 0xFF;
		}

		private void Output(int code, Stream outs)
		{
			cur_accum &= masks[cur_bits];
			if (cur_bits > 0)
			{
				cur_accum |= code << cur_bits;
			}
			else
			{
				cur_accum = code;
			}
			cur_bits += n_bits;
			while (cur_bits >= 8)
			{
				Add((byte)(cur_accum & 0xFF), outs);
				cur_accum >>= 8;
				cur_bits -= 8;
			}
			if (free_ent > maxcode || clear_flg)
			{
				if (clear_flg)
				{
					maxcode = MaxCode(n_bits = g_init_bits);
					clear_flg = false;
				}
				else
				{
					n_bits++;
					if (n_bits == maxbits)
					{
						maxcode = maxmaxcode;
					}
					else
					{
						maxcode = MaxCode(n_bits);
					}
				}
			}
			if (code == EOFCode)
			{
				while (cur_bits > 0)
				{
					Add((byte)(cur_accum & 0xFF), outs);
					cur_accum >>= 8;
					cur_bits -= 8;
				}
				Flush(outs);
			}
		}
	}
	public class NeuQuant
	{
		protected static readonly int netsize = 256;

		protected static readonly int prime1 = 499;

		protected static readonly int prime2 = 491;

		protected static readonly int prime3 = 487;

		protected static readonly int prime4 = 503;

		protected static readonly int minpicturebytes = 3 * prime4;

		protected static readonly int maxnetpos = netsize - 1;

		protected static readonly int netbiasshift = 4;

		protected static readonly int ncycles = 100;

		protected static readonly int intbiasshift = 16;

		protected static readonly int intbias = 1 << intbiasshift;

		protected static readonly int gammashift = 10;

		protected static readonly int gamma = 1 << gammashift;

		protected static readonly int betashift = 10;

		protected static readonly int beta = intbias >> betashift;

		protected static readonly int betagamma = intbias << gammashift - betashift;

		protected static readonly int initrad = netsize >> 3;

		protected static readonly int radiusbiasshift = 6;

		protected static readonly int radiusbias = 1 << radiusbiasshift;

		protected static readonly int initradius = initrad * radiusbias;

		protected static readonly int radiusdec = 30;

		protected static readonly int alphabiasshift = 10;

		protected static readonly int initalpha = 1 << alphabiasshift;

		protected int alphadec;

		protected static readonly int radbiasshift = 8;

		protected static readonly int radbias = 1 << radbiasshift;

		protected static readonly int alpharadbshift = alphabiasshift + radbiasshift;

		protected static readonly int alpharadbias = 1 << alpharadbshift;

		protected byte[] thepicture;

		protected int lengthcount;

		protected int samplefac;

		protected int[][] network;

		protected int[] netindex = new int[256];

		protected int[] bias = new int[netsize];

		protected int[] freq = new int[netsize];

		protected int[] radpower = new int[initrad];

		public NeuQuant(byte[] thepic, int len, int sample)
		{
			thepicture = thepic;
			lengthcount = len;
			samplefac = sample;
			network = new int[netsize][];
			for (int i = 0; i < netsize; i++)
			{
				network[i] = new int[4];
				int[] array = network[i];
				array[0] = (array[1] = (array[2] = (i << netbiasshift + 8) / netsize));
				freq[i] = intbias / netsize;
				bias[i] = 0;
			}
		}

		public byte[] ColorMap()
		{
			byte[] array = new byte[3 * netsize];
			int[] array2 = new int[netsize];
			for (int i = 0; i < netsize; i++)
			{
				array2[network[i][3]] = i;
			}
			int num = 0;
			for (int j = 0; j < netsize; j++)
			{
				int num2 = array2[j];
				array[num++] = (byte)network[num2][0];
				array[num++] = (byte)network[num2][1];
				array[num++] = (byte)network[num2][2];
			}
			return array;
		}

		public void Inxbuild()
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < netsize; i++)
			{
				int[] array = network[i];
				int num3 = i;
				int num4 = array[1];
				int[] array2;
				for (int j = i + 1; j < netsize; j++)
				{
					array2 = network[j];
					if (array2[1] < num4)
					{
						num3 = j;
						num4 = array2[1];
					}
				}
				array2 = network[num3];
				if (i != num3)
				{
					int j = array2[0];
					array2[0] = array[0];
					array[0] = j;
					j = array2[1];
					array2[1] = array[1];
					array[1] = j;
					j = array2[2];
					array2[2] = array[2];
					array[2] = j;
					j = array2[3];
					array2[3] = array[3];
					array[3] = j;
				}
				if (num4 != num)
				{
					netindex[num] = num2 + i >> 1;
					for (int j = num + 1; j < num4; j++)
					{
						netindex[j] = i;
					}
					num = num4;
					num2 = i;
				}
			}
			netindex[num] = num2 + maxnetpos >> 1;
			for (int j = num + 1; j < 256; j++)
			{
				netindex[j] = maxnetpos;
			}
		}

		public void Learn()
		{
			if (lengthcount < minpicturebytes)
			{
				samplefac = 1;
			}
			alphadec = 30 + (samplefac - 1) / 3;
			byte[] array = thepicture;
			int num = 0;
			int num2 = lengthcount;
			int num3 = lengthcount / (3 * samplefac);
			int num4 = num3 / ncycles;
			int num5 = initalpha;
			int num6 = initradius;
			int num7 = num6 >> radiusbiasshift;
			if (num7 <= 1)
			{
				num7 = 0;
			}
			int i;
			for (i = 0; i < num7; i++)
			{
				radpower[i] = num5 * ((num7 * num7 - i * i) * radbias / (num7 * num7));
			}
			int num8;
			if (lengthcount < minpicturebytes)
			{
				num8 = 3;
			}
			else if (lengthcount % prime1 != 0)
			{
				num8 = 3 * prime1;
			}
			else if (lengthcount % prime2 != 0)
			{
				num8 = 3 * prime2;
			}
			else
			{
				num8 = ((lengthcount % prime3 == 0) ? (3 * prime4) : (3 * prime3));
			}
			i = 0;
			while (i < num3)
			{
				int b = (array[num] & 0xFF) << netbiasshift;
				int g = (array[num + 1] & 0xFF) << netbiasshift;
				int r = (array[num + 2] & 0xFF) << netbiasshift;
				int i2 = Contest(b, g, r);
				Altersingle(num5, i2, b, g, r);
				if (num7 != 0)
				{
					Alterneigh(num7, i2, b, g, r);
				}
				num += num8;
				if (num >= num2)
				{
					num -= lengthcount;
				}
				i++;
				if (num4 == 0)
				{
					num4 = 1;
				}
				if (i % num4 == 0)
				{
					num5 -= num5 / alphadec;
					num6 -= num6 / radiusdec;
					num7 = num6 >> radiusbiasshift;
					if (num7 <= 1)
					{
						num7 = 0;
					}
					for (i2 = 0; i2 < num7; i2++)
					{
						radpower[i2] = num5 * ((num7 * num7 - i2 * i2) * radbias / (num7 * num7));
					}
				}
			}
		}

		public int Map(int b, int g, int r)
		{
			int num = 1000;
			int result = -1;
			int num2 = netindex[g];
			int num3 = num2 - 1;
			while (num2 < netsize || num3 >= 0)
			{
				int[] array;
				int num5;
				int num4;
				if (num2 < netsize)
				{
					array = network[num2];
					num4 = array[1] - g;
					if (num4 >= num)
					{
						num2 = netsize;
					}
					else
					{
						num2++;
						if (num4 < 0)
						{
							num4 = -num4;
						}
						num5 = array[0] - b;
						if (num5 < 0)
						{
							num5 = -num5;
						}
						num4 += num5;
						if (num4 < num)
						{
							num5 = array[2] - r;
							if (num5 < 0)
							{
								num5 = -num5;
							}
							num4 += num5;
							if (num4 < num)
							{
								num = num4;
								result = array[3];
							}
						}
					}
				}
				if (num3 < 0)
				{
					continue;
				}
				array = network[num3];
				num4 = g - array[1];
				if (num4 >= num)
				{
					num3 = -1;
					continue;
				}
				num3--;
				if (num4 < 0)
				{
					num4 = -num4;
				}
				num5 = array[0] - b;
				if (num5 < 0)
				{
					num5 = -num5;
				}
				num4 += num5;
				if (num4 < num)
				{
					num5 = array[2] - r;
					if (num5 < 0)
					{
						num5 = -num5;
					}
					num4 += num5;
					if (num4 < num)
					{
						num = num4;
						result = array[3];
					}
				}
			}
			return result;
		}

		public byte[] Process()
		{
			Learn();
			Unbiasnet();
			Inxbuild();
			return ColorMap();
		}

		public void Unbiasnet()
		{
			for (int i = 0; i < netsize; i++)
			{
				network[i][0] >>= netbiasshift;
				network[i][1] >>= netbiasshift;
				network[i][2] >>= netbiasshift;
				network[i][3] = i;
			}
		}

		protected void Alterneigh(int rad, int i, int b, int g, int r)
		{
			int num = i - rad;
			if (num < -1)
			{
				num = -1;
			}
			int num2 = i + rad;
			if (num2 > netsize)
			{
				num2 = netsize;
			}
			int num3 = i + 1;
			int num4 = i - 1;
			int num5 = 1;
			while (num3 < num2 || num4 > num)
			{
				int num6 = radpower[num5++];
				if (num3 < num2)
				{
					int[] array = network[num3++];
					array[0] -= num6 * (array[0] - b) / alpharadbias;
					array[1] -= num6 * (array[1] - g) / alpharadbias;
					array[2] -= num6 * (array[2] - r) / alpharadbias;
				}
				if (num4 > num)
				{
					int[] array = network[num4--];
					array[0] -= num6 * (array[0] - b) / alpharadbias;
					array[1] -= num6 * (array[1] - g) / alpharadbias;
					array[2] -= num6 * (array[2] - r) / alpharadbias;
				}
			}
		}

		protected void Altersingle(int alpha, int i, int b, int g, int r)
		{
			int[] array = network[i];
			array[0] -= alpha * (array[0] - b) / initalpha;
			array[1] -= alpha * (array[1] - g) / initalpha;
			array[2] -= alpha * (array[2] - r) / initalpha;
		}

		protected int Contest(int b, int g, int r)
		{
			int num = int.MaxValue;
			int num2 = num;
			int num3 = -1;
			int result = num3;
			for (int i = 0; i < netsize; i++)
			{
				int[] obj = network[i];
				int num4 = obj[0] - b;
				if (num4 < 0)
				{
					num4 = -num4;
				}
				int num5 = obj[1] - g;
				if (num5 < 0)
				{
					num5 = -num5;
				}
				num4 += num5;
				num5 = obj[2] - r;
				if (num5 < 0)
				{
					num5 = -num5;
				}
				num4 += num5;
				if (num4 < num)
				{
					num = num4;
					num3 = i;
				}
				int num6 = num4 - (bias[i] >> intbiasshift - netbiasshift);
				if (num6 < num2)
				{
					num2 = num6;
					result = i;
				}
				int num7 = freq[i] >> betashift;
				freq[i] -= num7;
				bias[i] += num7 << gammashift;
			}
			freq[num3] += beta;
			bias[num3] -= betagamma;
			return result;
		}
	}
}
namespace PygmyMonkey.GifCreator
{
	public class CubeBar : MonoBehaviour
	{
		private int randomFrameColorChange;

		private Vector3 randomLocalScale;

		private float randomSpeed;

		private void Awake()
		{
			udpateRandoms();
		}

		private void Update()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			if (Time.frameCount % randomFrameColorChange == 0)
			{
				udpateRandoms();
			}
			((Component)this).transform.localScale = Vector3.Lerp(((Component)this).transform.localScale, randomLocalScale, randomSpeed * Time.deltaTime);
		}

		private void udpateRandoms()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			randomFrameColorChange = Random.Range(50, 300);
			randomLocalScale = new Vector3(0.5f, Random.Range(0.2f, 5.5f), 0.5f);
			randomSpeed = Random.Range(0.5f, 2f);
			((Component)this).GetComponent<Renderer>().material.color = getRandomColor();
		}

		private Color getRandomColor()
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
		}
	}
	[ExecuteInEditMode]
	public class CustomRectPreview : MonoBehaviour
	{
		public static CustomRectPreview Singleton;

		private Material lineMaterial;

		private RecorderParameters parameters;

		private float startX;

		private float startY;

		private float endX;

		private float endY;

		private static MethodInfo getMainGameViewMethod;

		private bool show;

		private void Awake()
		{
			initGameViewMethod();
		}

		public void OnEnable()
		{
			Singleton = this;
		}

		private void createLineMaterial()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected Obj, but got Unknown
			lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
			((Object)lineMaterial).hideFlags = (HideFlags)61;
		}

		private void initGameViewMethod()
		{
		}

		public void Show(RecorderParameters parameters)
		{
			this.parameters = parameters;
			show = true;
			repaintGameView();
		}

		public void Hide()
		{
			show = false;
			repaintGameView();
		}

		private void OnRenderObject()
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			if (show)
			{
				if ((Object)(object)lineMaterial == (Object)null)
				{
					createLineMaterial();
				}
				if (parameters.section == RecorderParameters.Section.CustomRectAbsolute)
				{
					startX = (float)parameters.absolutePosX / parameters.getMainGameViewSize().x;
					startY = (float)parameters.absolutePosY / parameters.getMainGameViewSize().y;
					endX = (float)(parameters.absolutePosX + parameters.absoluteWidth) / parameters.getMainGameViewSize().x;
					endY = (float)(parameters.absolutePosY + parameters.absoluteHeight) / parameters.getMainGameViewSize().y;
				}
				else if (parameters.section == RecorderParameters.Section.CustomRectRelative)
				{
					startX = parameters.relativePosX;
					startY = parameters.relativePosY;
					endX = parameters.relativePosX + parameters.relativeWidth;
					endY = parameters.relativePosY + parameters.relativeHeight;
				}
				GL.PushMatrix();
				lineMaterial.SetPass(0);
				GL.LoadOrtho();
				GL.Begin(7);
				GL.Color(parameters.customRectPreviewColor);
				GL.Vertex3(startX, startY, 0f);
				GL.Vertex3(startX, endY, 0f);
				GL.Vertex3(endX, endY, 0f);
				GL.Vertex3(endX, startY, 0f);
				GL.End();
				GL.PopMatrix();
			}
		}

		private void repaintGameView()
		{
		}
	}
	public enum RecorderState
	{
		None,
		Recording,
		Paused,
		PreProcessing,
		Saving
	}
	public class NTexture : ITexture
	{
		private Texture2D texture2D;

		public NTexture(Texture2D texture2D)
		{
			this.texture2D = texture2D;
		}

		public void Flush()
		{
			if ((Object)(object)texture2D != (Object)null)
			{
				Object.DestroyImmediate((Object)(object)texture2D);
			}
		}

		public void Convert(RecorderParameters parameters)
		{
		}

		public Texture2D GetTexture2D()
		{
			return texture2D;
		}
	}
	public class RTexture : ITexture
	{
		private RenderTexture renderTexture;

		private Texture2D texture2D;

		public RTexture(RenderTexture renderTexture)
		{
			this.renderTexture = renderTexture;
		}

		public void Flush()
		{
			if ((Object)(object)renderTexture != (Object)null)
			{
				Object.DestroyImmediate((Object)(object)renderTexture);
			}
			if ((Object)(object)texture2D != (Object)null)
			{
				Object.DestroyImmediate((Object)(object)texture2D);
			}
		}

		public void Convert(RecorderParameters parameters)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected Obj, but got Unknown
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			texture2D = new Texture2D(((Texture)renderTexture).width, ((Texture)renderTexture).height, (TextureFormat)3, false);
			((Object)texture2D).hideFlags = (HideFlags)61;
			((Texture)texture2D).wrapMode = (TextureWrapMode)1;
			((Texture)texture2D).filterMode = (FilterMode)1;
			((Texture)texture2D).anisoLevel = 0;
			RenderTexture.active = renderTexture;
			texture2D.ReadPixels(new Rect(0f, 0f, (float)((Texture)renderTexture).width, (float)((Texture)renderTexture).height), 0, 0);
			texture2D.Apply();
			RenderTexture.active = null;
			Object.DestroyImmediate((Object)(object)renderTexture);
		}

		public Texture2D GetTexture2D()
		{
			return texture2D;
		}
	}
	public interface ITexture
	{
		void Flush();

		void Convert(RecorderParameters parameters);

		Texture2D GetTexture2D();
	}
	[ExecuteInEditMode]
	public sealed class Recorder : MonoBehaviour
	{
		public static Recorder Singleton;

		private RecorderParameters parameters;

		public Action<float> OnRecordingProgress;

		public Action OnRecordingDone;

		public Action OnPreProcessingDone;

		public Action<int, float> OnFileSaveProgress;

		public Action<int, string> OnFileSaved;

		private Material customBlitMaterial;

		private int m_MaxFrameCount;

		private float m_Time;

		public RecorderState State { get; private set; }

		public List<ITexture> FrameList { get; private set; }

		public void Reset()
		{
			State = RecorderState.None;
			FlushMemory();
		}

		public void Init(RecorderParameters parameters)
		{
			this.parameters = parameters;
			Reset();
			m_MaxFrameCount = Mathf.RoundToInt(parameters.duration * (float)parameters.framePerSecond);
			m_Time = 0f;
		}

		public void Pause()
		{
			if (State == RecorderState.PreProcessing)
			{
				Debug.LogWarning((object)"Attempting to pause recording during the pre-processing step. The recorder is automatically paused when pre-processing.");
			}
			else
			{
				State = RecorderState.Paused;
			}
		}

		public void Record()
		{
			if (State == RecorderState.PreProcessing)
			{
				Debug.LogWarning((object)"Attempting to resume recording during the pre-processing step.");
			}
			else
			{
				State = RecorderState.Recording;
			}
		}

		public void FlushMemory()
		{
			if (State == RecorderState.PreProcessing)
			{
				Debug.LogWarning((object)"Attempting to flush memory during the pre-processing step.");
			}
			else
			{
				if (FrameList == null)
				{
					return;
				}
				foreach (ITexture frame in FrameList)
				{
					frame.Flush();
				}
				FrameList.Clear();
			}
		}

		public void Save(int frameStart, int frameEnd)
		{
			Save(GenerateFileName(), frameStart, frameEnd);
		}

		public void Save(string filename, int frameStart, int frameEnd)
		{
			if (State == RecorderState.PreProcessing)
			{
				Debug.LogWarning((object)"Attempting to save during the pre-processing step.");
				return;
			}
			if (FrameList.Count == 0)
			{
				Debug.LogWarning((object)"Nothing to save. Maybe you forgot to start the recorder ?");
				return;
			}
			State = RecorderState.PreProcessing;
			if (string.IsNullOrEmpty(filename))
			{
				filename = GenerateFileName();
			}
			((MonoBehaviour)this).StartCoroutine(PreProcess(filename, frameStart, frameEnd));
		}

		public void StopRecording()
		{
			State = RecorderState.Paused;
			foreach (ITexture frame in FrameList)
			{
				frame.Convert(parameters);
			}
			if (OnRecordingDone != null)
			{
				OnRecordingDone();
			}
		}

		private void Awake()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected Obj, but got Unknown
			FrameList = new List<ITexture>();
			customBlitMaterial = new Material(Shader.Find("Hidden/BlitCopy"));
			((Object)customBlitMaterial).hideFlags = (HideFlags)61;
			State = RecorderState.None;
		}

		public void OnEnable()
		{
			Singleton = this;
		}

		private void OnDestroy()
		{
			FlushMemory();
		}

		private void OnGUI()
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Invalid comparison between Unknown and I4
			if (parameters != null && parameters.recordMode.Equals(RecorderParameters.RecordMode.EVERYTHING) && State == RecorderState.Recording && (int)Event.current.type == 7)
			{
				recordFrame(() =>
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_0020: Unknown result type (might be due to invalid IL or missing references)
					//IL_0025: Unknown result type (might be due to invalid IL or missing references)
					//IL_0030: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Expected Obj, but got Unknown
					//IL_0044: Unknown result type (might be due to invalid IL or missing references)
					RenderTexture.active = null;
					Rect rectSection = parameters.getRectSection();
					int num = (int)rectSection.width;
					rectSection = parameters.getRectSection();
					Texture2D val = new Texture2D(num, (int)rectSection.height, (TextureFormat)5, false);
					((Texture)val).filterMode = (FilterMode)1;
					val.ReadPixels(parameters.getRectSection(), 0, 0, false);
					val.Apply();
					resize(val, parameters.getRecordWidth(), parameters.getRecordHeight());
					FrameList.Add(new NTexture(val));
				});
			}
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (parameters == null || !parameters.recordMode.Equals(RecorderParameters.RecordMode.SINGLE_CAMERA))
			{
				Graphics.Blit((Texture)(object)source, destination);
				return;
			}
			if (State != RecorderState.Recording)
			{
				Graphics.Blit((Texture)(object)source, destination);
				return;
			}
			recordFrame(() =>
			{
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Expected Obj, but got Unknown
				RenderTexture val = new RenderTexture(parameters.getRecordWidth(), parameters.getRecordHeight(), 0, (RenderTextureFormat)0);
				((Texture)val).wrapMode = (TextureWrapMode)1;
				((Texture)val).filterMode = (FilterMode)1;
				((Texture)val).anisoLevel = 0;
				CustomGraphicsBlit(source, val);
				FrameList.Add(new RTexture(val));
			});
			Graphics.Blit((Texture)(object)source, destination);
		}

		private void CustomGraphicsBlit(RenderTexture source, RenderTexture dest)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			RenderTexture.active = dest;
			customBlitMaterial.SetTexture("_MainTex", (Texture)(object)source);
			GL.PushMatrix();
			GL.LoadOrtho();
			customBlitMaterial.SetPass(0);
			GL.Begin(7);
			Rect rectSection = parameters.getRectSection();
			float num = rectSection.y / (float)((Texture)source).height;
			float num2 = (rectSection.y + rectSection.height) / (float)((Texture)source).height;
			float num3 = rectSection.x / (float)((Texture)source).width;
			float num4 = (rectSection.x + rectSection.width) / (float)((Texture)source).width;
			float num5 = 1f;
			float num6 = 0f;
			if (parameters.recordUpsideDown)
			{
				num5 = 0f;
				num6 = 1f;
			}
			GL.MultiTexCoord2(0, num3, num);
			GL.Vertex3(0f, num5, 0f);
			GL.MultiTexCoord2(0, num4, num);
			GL.Vertex3(1f, num5, 0f);
			GL.MultiTexCoord2(0, num4, num2);
			GL.Vertex3(1f, num6, 0f);
			GL.MultiTexCoord2(0, num3, num2);
			GL.Vertex3(0f, num6, 0f);
			GL.End();
			GL.PopMatrix();
		}

		private void recordFrame(Action onRecord)
		{
			m_Time += Time.unscaledDeltaTime;
			if (!(m_Time >= parameters.getTimePerFrame()))
			{
				return;
			}
			switch (parameters.recordType)
			{
			case RecorderParameters.RecordType.Duration:
				if (FrameList.Count >= m_MaxFrameCount)
				{
					StopRecording();
					return;
				}
				break;
			case RecorderParameters.RecordType.LastSeconds:
				if (FrameList.Count >= m_MaxFrameCount)
				{
					FrameList[0].Flush();
					FrameList.RemoveAt(0);
				}
				break;
			}
			m_Time -= parameters.getTimePerFrame();
			onRecord();
			if (parameters.recordType == RecorderParameters.RecordType.Duration && OnRecordingProgress != null)
			{
				OnRecordingProgress((float)FrameList.Count / (float)m_MaxFrameCount);
			}
		}

		private void Flush(Texture texture)
		{
			Object.DestroyImmediate((Object)(object)texture);
		}

		private string GenerateFileName()
		{
			string text = DateTime.Now.ToString("yyyyMMddHHmmssffff");
			return "GifCapture-" + text;
		}

		private IEnumerator PreProcess(string filename, int frameStart, int frameEnd)
		{
			List<GifFrame> frames = new List<GifFrame>(frameEnd - frameStart);
			for (int i = frameStart; i <= frameEnd; i++)
			{
				Texture2D val = new Texture2D(((Texture)FrameList[i].GetTexture2D()).width, ((Texture)FrameList[i].GetTexture2D()).height, FrameList[i].GetTexture2D().format, false);
				val.SetPixels32(FrameList[i].GetTexture2D().GetPixels32());
				val.Apply();
				resize(val, parameters.getOutputWidth(), parameters.getOutputHeight());
				GifFrame item = new GifFrame
				{
					Width = ((Texture)val).width,
					Height = ((Texture)val).height,
					Data = val.GetPixels32()
				};
				frames.Add(item);
				yield return null;
			}
			if (OnPreProcessingDone != null)
			{
				OnPreProcessingDone();
			}
			State = RecorderState.Saving;
			if (!Directory.Exists(parameters.getDestinationFolder()))
			{
				Directory.CreateDirectory(parameters.getDestinationFolder());
			}
			GifEncoder gifEncoder = new GifEncoder(parameters.repeat, 100 - parameters.quality + 1);
			gifEncoder.SetDelay(Mathf.RoundToInt(parameters.getTimePerFrame() * 1000f));
			Worker worker = new Worker(parameters.workerPriority);
			worker.m_Encoder = gifEncoder;
			worker.m_Frames = frames;
			worker.m_FilePath = parameters.getDestinationFolder() + "/" + filename + ".gif";
			worker.m_OnFileSaved = OnFileSaved;
			worker.m_OnFileSaveProgress = OnFileSaveProgress;
			worker.Start();
		}

		private static void resize(Texture2D texture, int width, int height)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			if (((Texture)texture).width != width || ((Texture)texture).height != height)
			{
				Color[] array = new Color[width * height];
				float num = 1f / (float)width;
				float num2 = 1f / (float)height;
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = texture.GetPixelBilinear(num * ((float)i % (float)width), num2 * Mathf.Floor((float)(i / width)));
				}
				texture.Resize(width, height);
				texture.SetPixels(array, 0);
				texture.Apply();
			}
		}
	}
	[Serializable]
	public class RecorderParameters
	{
		public enum RecordMode
		{
			SINGLE_CAMERA,
			EVERYTHING
		}

		public enum Section
		{
			Fullscreen,
			CustomRectAbsolute,
			CustomRectRelative
		}

		public enum RecordType
		{
			Duration,
			LastSeconds
		}

		public Camera camera;

		public RecordMode recordMode;

		public Section section;

		public float relativePosX;

		public float relativePosY;

		public float relativeWidth = 0.5f;

		public float relativeHeight = 0.5f;

		public int absolutePosX;

		public int absolutePosY;

		public int absoluteWidth = 320;

		public int absoluteHeight = 200;

		public Color customRectPreviewColor = new Color(1f, 0f, 0f, 0.2f);

		public bool showCustomRectPreview;

		public bool recordUpsideDown;

		public bool pauseAfterRecord;

		public RecordType recordType;

		public int framePerSecond = 15;

		public int repeat;

		public int quality = 85;

		public float duration = 3f;

		public int recordWidth = 320;

		public ThreadPriority workerPriority = ThreadPriority.BelowNormal;

		public string destinationFolder;

		public bool openFolderAfterSave;

		public int outputWidth = 320;

		private MethodInfo getSizeOfMainGameViewMethod;

		private Vector2 gameViewSize;

		public RecorderParameters()
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			Type type = Type.GetType("UnityEditor.GameView,UnityEditor");
			getSizeOfMainGameViewMethod = type.GetMethod("GetMainGameViewTargetSize", BindingFlags.Static | BindingFlags.NonPublic);
			UpdateGameViewSize();
		}

		public void init()
		{
		}

		public float getTimePerFrame()
		{
			return 1f / (float)framePerSecond;
		}

		public Rect getRectSection()
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			switch (section)
			{
				case Section.CustomRectAbsolute:
					return new Rect((float)absolutePosX, (float)absolutePosY, (float)absoluteWidth, (float)absoluteHeight);
				case Section.CustomRectRelative:
					return new Rect(relativePosX * getMainGameViewSize().x, relativePosY * getMainGameViewSize().y, relativeWidth * getMainGameViewSize().x, relativeHeight * getMainGameViewSize().y);
				default:
					return new Rect(0f, 0f, getMainGameViewSize().x, getMainGameViewSize().y);
			}
		}

		public float getEstimatedMemoryUse()
		{
			return (float)framePerSecond * duration * (float)(getRecordWidth() * getRecordHeight() * 4) / 1048576f;
		}

		public string getEstimatedMemoryUseDescription()
		{
			return framePerSecond + "fps * " + duration + " seconds * " + getRecordWidth() + " (width) * " + getRecordHeight() + " (height) * 4 (RGBA) / 1024 (to KB) / 1024 (to MB)";
		}

		public int getRecordWidth()
		{
			return recordWidth;
		}

		public int getRecordHeight()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			float num = recordWidth;
			Rect rectSection = getRectSection();
			float num2 = num * rectSection.height;
			rectSection = getRectSection();
			return (int)(num2 / rectSection.width);
		}

		public int getOutputWidth()
		{
			return outputWidth;
		}

		public int getOutputHeight()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			float num = outputWidth;
			Rect rectSection = getRectSection();
			float num2 = num * rectSection.height;
			rectSection = getRectSection();
			return (int)(num2 / rectSection.width);
		}

		public string getDestinationFolder()
		{
			if (string.IsNullOrEmpty(destinationFolder))
			{
				return Application.dataPath;
			}
			return destinationFolder;
		}

		public void UpdateGameViewSize()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			object obj = getSizeOfMainGameViewMethod.Invoke(null, null);
			gameViewSize = (Vector2)obj;
		}

		public Vector2 getMainGameViewSize()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return gameViewSize;
		}
	}
}
namespace ca.HenrySoftware
{
	[CreateAssetMenu]
	public class ModelEffectAnimation : ScriptableObject
	{
		public RuntimeAnimatorController Controller;

		public bool Blend;

		public Vector2 Offset = Vector2.zero;

		public Vector2 BackOffset = Vector2.zero;

		public Vector2 ForeOffset = Vector2.zero;

		public ModelEffectAnimation()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		}
	}
	public class StateFlip : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			((Component)animator).gameObject.transform.localScale = new Vector3((Random.value > 0.5f) ? 1f : (-1f), 1f, 1f);
		}
	}
	public class StateRandom : StateMachineBehaviour
	{
		private static int AnimatorRandom = Animator.StringToHash("Random");

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.SetFloat(AnimatorRandom, Random.Range(0f, 1f));
		}
	}
	public class StateRemove : StateMachineBehaviour
	{
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			Pool componentInParent = ((Component)animator).gameObject.GetComponentInParent<Pool>();
			if ((Object)(object)componentInParent != (Object)null)
			{
				((Component)animator).gameObject.SetActive(false);
				animator.runtimeAnimatorController = null;
				componentInParent.Exit(((Component)animator).gameObject);
			}
		}
	}
}
namespace ca.HenrySoftware.Rage
{
	public enum EaseType
	{
		Linear,
		SineIn,
		SineOut,
		SineInOut,
		QuadIn,
		QuadOut,
		QuadInOut,
		CubicIn,
		CubicOut,
		CubicInOut,
		QuartIn,
		QuartOut,
		QuartInOut,
		QuintIn,
		QuintOut,
		QuintInOut,
		ExpoIn,
		ExpoOut,
		ExpoInOut,
		CircIn,
		CircOut,
		CircInOut,
		BackIn,
		BackOut,
		BackInOut,
		ElasticIn,
		ElasticOut,
		ElasticInOut,
		BounceIn,
		BounceOut,
		BounceInOut,
		Spring
	}
	public static class Ease
	{
		private static readonly Dictionary<EaseType, Func<float, float, float, float>> Types = new Dictionary<EaseType, Func<float, float, float, float>>
		{
			{
				EaseType.Linear,
				(Func<float, float, float, float>)Mathf.Lerp
			},
			{
				EaseType.SineIn,
				SineIn
			},
			{
				EaseType.SineOut,
				SineOut
			},
			{
				EaseType.SineInOut,
				SineInOut
			},
			{
				EaseType.QuadIn,
				QuadIn
			},
			{
				EaseType.QuadOut,
				QuadOut
			},
			{
				EaseType.QuadInOut,
				QuadInOut
			},
			{
				EaseType.CubicIn,
				CubicIn
			},
			{
				EaseType.CubicOut,
				CubicOut
			},
			{
				EaseType.CubicInOut,
				CubicInOut
			},
			{
				EaseType.QuartIn,
				QuartIn
			},
			{
				EaseType.QuartOut,
				QuartOut
			},
			{
				EaseType.QuartInOut,
				QuartInOut
			},
			{
				EaseType.QuintIn,
				QuintIn
			},
			{
				EaseType.QuintOut,
				QuintOut
			},
			{
				EaseType.QuintInOut,
				QuintInOut
			},
			{
				EaseType.ExpoIn,
				ExpoIn
			},
			{
				EaseType.ExpoOut,
				ExpoOut
			},
			{
				EaseType.ExpoInOut,
				ExpoInOut
			},
			{
				EaseType.CircIn,
				CircIn
			},
			{
				EaseType.CircOut,
				CircOut
			},
			{
				EaseType.CircInOut,
				CircInOut
			},
			{
				EaseType.BackIn,
				BackIn
			},
			{
				EaseType.BackOut,
				BackOut
			},
			{
				EaseType.BackInOut,
				BackInOut
			},
			{
				EaseType.ElasticIn,
				ElasticIn
			},
			{
				EaseType.ElasticOut,
				ElasticOut
			},
			{
				EaseType.ElasticInOut,
				ElasticInOut
			},
			{
				EaseType.BounceIn,
				BounceIn
			},
			{
				EaseType.BounceOut,
				BounceOut
			},
			{
				EaseType.BounceInOut,
				BounceInOut
			},
			{
				EaseType.Spring,
				Spring
			}
		};

		private const float HalfPi = MathF.PI / 2f;

		private const float DoublePi = MathF.PI * 2f;

		public static IEnumerator Go(MonoBehaviour m, float from, float to, float time, Action<float> update, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			IEnumerator enumerator = GoCoroutine(m, from, to, time, update, complete, type, delay, repeat, pingPong, realTime);
			m.StartCoroutine(enumerator);
			return enumerator;
		}

		private static IEnumerator GoCoroutine(MonoBehaviour m, float from, float to, float time, Action<float> update, Action complete, EaseType type, float delay, int repeat, bool pingPong, bool realTime)
		{
			int counter = repeat;
			while (repeat == 0 || repeat == -1 || counter > 0)
			{
				if (delay > 0f)
				{
					if (realTime)
					{
						yield return new WaitForSecondsRealtime(delay);
					}
					else
					{
						yield return (object)new WaitForSeconds(delay);
					}
				}
				float t = 0f;
				while (t <= 1f)
				{
					t += (realTime ? Time.unscaledDeltaTime : Time.deltaTime) / time;
					update(Types[type](from, to, Mathf.Clamp01(t)));
					yield return null;
				}
				if (pingPong)
				{
					if (delay > 0f)
					{
						if (realTime)
						{
							yield return new WaitForSecondsRealtime(delay);
						}
						else
						{
							yield return (object)new WaitForSeconds(delay);
						}
					}
					t = 0f;
					while (t <= 1f)
					{
						t += (realTime ? Time.unscaledDeltaTime : Time.deltaTime) / time;
						update(Types[type](to, from, Mathf.Clamp01(t)));
						yield return null;
					}
				}
				if (repeat != 0)
				{
					counter--;
				}
				if (repeat == 0 || repeat == -1)
				{
					complete?.Invoke();
				}
			}
			if (repeat != 0)
			{
				complete?.Invoke();
			}
		}

		private static float GetAlpha(Component m)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			CanvasGroup component = m.GetComponent<CanvasGroup>();
			if ((Object)(object)component != (Object)null)
			{
				return component.alpha;
			}
			Image component2 = m.GetComponent<Image>();
			if ((Object)(object)component2 != (Object)null)
			{
				return ((Graphic)component2).color.a;
			}
			return Camera.main.backgroundColor.a;
		}

		public static IEnumerator GoAlphaTo(MonoBehaviour m, float to, float time, Action<float> update = null, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			return GoAlpha(m, GetAlpha((Component)(object)m), to, time, update, complete, type, delay, repeat, pingPong, realTime);
		}

		public static IEnumerator GoAlphaBy(MonoBehaviour m, float by, float time, Action<float> update = null, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			float alpha = GetAlpha((Component)(object)m);
			return GoAlpha(m, alpha, alpha + by, time, update, complete, type, delay, repeat, pingPong, realTime);
		}

		public static IEnumerator GoAlpha(MonoBehaviour m, float from, float to, float time, Action<float> update, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			IEnumerator enumerator = GoAlphaCoroutine(m, from, to, time, update, complete, type, delay, repeat, pingPong, realTime);
			m.StartCoroutine(enumerator);
			return enumerator;
		}

		private static IEnumerator GoAlphaCoroutine(MonoBehaviour m, float from, float to, float time, Action<float> update, Action complete, EaseType type, float delay, int repeat, bool pingPong, bool realTime)
		{
			Image image = ((Component)m).GetComponent<Image>();
			CanvasGroup canvasGroup = ((Component)m).GetComponent<CanvasGroup>();
			Action<float> setAlpha = (float value) =>
			{
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)canvasGroup != (Object)null)
				{
					canvasGroup.alpha = value;
				}
				else if ((Object)(object)image != (Object)null)
				{
					((Graphic)image).color = ((Graphic)image).color.SetAlpha(value);
				}
				else
				{
					Camera.main.backgroundColor = Camera.main.backgroundColor.SetAlpha(value);
				}
			};
			int counter = repeat;
			while (repeat == 0 || repeat == -1 || counter > 0)
			{
				if (delay > 0f)
				{
					if (realTime)
					{
						yield return new WaitForSecondsRealtime(delay);
					}
					else
					{
						yield return (object)new WaitForSeconds(delay);
					}
				}
				float t = 0f;
				while (t <= 1f)
				{
					t += (realTime ? Time.unscaledDeltaTime : Time.deltaTime) / time;
					float obj = Types[type](from, to, Mathf.Clamp01(t));
					setAlpha(obj);
					update?.Invoke(obj);
					yield return null;
				}
				setAlpha(to);
				if (pingPong)
				{
					if (delay > 0f)
					{
						if (realTime)
						{
							yield return new WaitForSecondsRealtime(delay);
						}
						else
						{
							yield return (object)new WaitForSeconds(delay);
						}
					}
					t = 0f;
					while (t <= 1f)
					{
						t += (realTime ? Time.unscaledDeltaTime : Time.deltaTime) / time;
						float obj2 = Types[type](to, from, Mathf.Clamp01(t));
						setAlpha(obj2);
						update?.Invoke(obj2);
						yield return null;
					}
					setAlpha(from);
				}
				if (repeat != 0)
				{
					counter--;
				}
				if (repeat == 0 || repeat == -1)
				{
					complete?.Invoke();
				}
			}
			if (repeat != 0)
			{
				complete?.Invoke();
			}
		}

		public static float SineIn(float from, float to, float time)
		{
			return Mathf.Lerp(from, to, 1f - Mathf.Cos(time * (MathF.PI / 2f)));
		}

		public static float SineOut(float from, float to, float time)
		{
			return Mathf.Lerp(from, to, Mathf.Sin(time * (MathF.PI / 2f)));
		}

		public static float SineInOut(float from, float to, float time)
		{
			return Mathf.Lerp(from, to, 0.5f * (1f - Mathf.Cos(MathF.PI * time)));
		}

		public static float QuadIn(float from, float to, float time)
		{
			return Mathf.Lerp(from, to, time * time);
		}

		public static float QuadOut(float from, float to, float time)
		{
			return Mathf.Lerp(from, to, (0f - time) * (time - 2f));
		}

		public static float QuadInOut(float from, float to, float time)
		{
			if ((time /= 0.5f) < 1f)
			{
				return Mathf.Lerp(from, to, 0.5f * time * time);
			}
			return Mathf.Lerp(from, to, -0.5f * (--time * (time - 2f) - 1f));
		}

		public static float CubicIn(float from, float to, float time)
		{
			return Mathf.Lerp(from, to, time * time * time);
		}

		public static float CubicOut(float from, float to, float time)
		{
			return Mathf.Lerp(from, to, --time * time * time + 1f);
		}

		public static float CubicInOut(float from, float to, float time)
		{
			if ((time /= 0.5f) < 1f)
			{
				return Mathf.Lerp(from, to, 0.5f * time * time * time);
			}
			return Mathf.Lerp(from, to, 0.5f * ((time -= 2f) * time * time + 2f));
		}

		public static float QuartIn(float from, float to, float time)
		{
			return Mathf.Lerp(from, to, time * time * time * time);
		}

		public static float QuartOut(float from, float to, float time)
		{
			return Mathf.Lerp(from, to, 0f - (--time * time * time * time - 1f));
		}

		public static float QuartInOut(float from, float to, float time)
		{
			if ((time /= 0.5f) < 1f)
			{
				return Mathf.Lerp(from, to, 0.5f * time * time * time * time);
			}
			return Mathf.Lerp(from, to, -0.5f * ((time -= 2f) * time * time * time - 2f));
		}

		public static float QuintIn(float from, float to, float time)
		{
			return Mathf.Lerp(from, to, time * time * time * time * time);
		}

		public static float QuintOut(float from, float to, float time)
		{
			return Mathf.Lerp(from, to, --time * time * time * time * time + 1f);
		}

		public static float QuintInOut(float from, float to, float time)
		{
			if ((time /= 0.5f) < 1f)
			{
				return Mathf.Lerp(from, to, 0.5f * time * time * time * time * time);
			}
			return Mathf.Lerp(from, to, 0.5f * ((time -= 2f) * time * time * time * time + 2f));
		}

		public static float ExpoIn(float from, float to, float time)
		{
			return Mathf.Lerp(from, to, Mathf.Pow(2f, 10f * (time - 1f)));
		}

		public static float ExpoOut(float from, float to, float time)
		{
			return Mathf.Lerp(from, to, 0f - Mathf.Pow(2f, -10f * time) + 1f);
		}

		public static float ExpoInOut(float from, float to, float time)
		{
			if ((time /= 0.5f) < 1f)
			{
				return Mathf.Lerp(from, to, 0.5f * Mathf.Pow(2f, 10f * (time - 1f)));
			}
			return Mathf.Lerp(from, to, 0.5f * (0f - Mathf.Pow(2f, -10f * --time) + 2f));
		}

		public static float CircIn(float from, float to, float time)
		{
			return Mathf.Lerp(from, to, 0f - (Mathf.Sqrt(1f - time * time) - 1f));
		}

		public static float CircOut(float from, float to, float time)
		{
			return Mathf.Lerp(from, to, Mathf.Sqrt(1f - --time * time));
		}

		public static float CircInOut(float from, float to, float time)
		{
			if ((time /= 0.5f) < 1f)
			{
				return Mathf.Lerp(from, to, -0.5f * (Mathf.Sqrt(1f - time * time) - 1f));
			}
			return Mathf.Lerp(from, to, 0.5f * (Mathf.Sqrt(1f - (time -= 2f) * time) + 1f));
		}

		public static float BackIn(float from, float to, float time)
		{
			to -= from;
			return to * time * time * (2.70158f * time - 1.70158f) + from;
		}

		public static float BackOut(float from, float to, float time)
		{
			to -= from;
			return to * (--time * time * (2.70158f * time + 1.70158f) + 1f) + from;
		}

		public static float BackInOut(float from, float to, float time)
		{
			to -= from;
			if ((time /= 0.5f) < 1f)
			{
				return to * 0.5f * (time * time * (3.5949094f * time - 2.5949094f)) + from;
			}
			return to * 0.5f * ((time -= 2f) * time * (3.5949094f * time + 2.5949094f) + 2f) + from;
		}

		public static float ElasticIn(float from, float to, float time)
		{
			to -= from;
			return to * (0f - Mathf.Pow(2f, 10f * --time) * Mathf.Sin((time - 0.075f) * (MathF.PI * 2f) / 0.3f)) + from;
		}

		public static float ElasticOut(float from, float to, float time)
		{
			to -= from;
			return to * Mathf.Pow(2f, -10f * time) * Mathf.Sin((time - 0.075f) * (MathF.PI * 2f) / 0.3f) + to + from;
		}

		public static float ElasticInOut(float from, float to, float time)
		{
			to -= from;
			if ((time /= 0.5f) < 1f)
			{
				return -0.5f * (to * Mathf.Pow(2f, 10f * --time) * Mathf.Sin((time - 0.112500004f) * (MathF.PI * 2f) / 0.45000002f)) + from;
			}
			return to * Mathf.Pow(2f, -10f * --time) * Mathf.Sin((time - 0.112500004f) * (MathF.PI * 2f) / 0.45000002f) * 0.5f + to + from;
		}

		public static float BounceIn(float from, float to, float time)
		{
			to -= from;
			return to - BounceOut(0f, to, 1f - time) + from;
		}

		public static float BounceOut(float from, float to, float time)
		{
			to -= from;
			if (time < 372f / 1023f)
			{
				return to * (7.5625f * time * time) + from;
			}
			if (time < 744f / 1023f)
			{
				return to * (7.5625f * (time -= 558f / 1023f) * time + 0.75f) + from;
			}
			if (time < 930f / 1023f)
			{
				return to * (7.5625f * (time -= 837f / 1023f) * time + 0.9375f) + from;
			}
			return to * (7.5625f * (time -= 21f / 22f) * time + 63f / 64f) + from;
		}

		public static float BounceInOut(float from, float to, float time)
		{
			to -= from;
			if (time < 0.5f)
			{
				return BounceIn(0f, to, time * 2f) * 0.5f + from;
			}
			return BounceOut(0f, to, time * 2f - 1f) * 0.5f + to * 0.5f + from;
		}

		public static float Spring(float from, float to, float time)
		{
			time = Mathf.Clamp01(time);
			time = (Mathf.Sin(time * MathF.PI * (0.2f + 2.5f * time * time * time)) * Mathf.Pow(1f - time, 2.2f) + time) * (1f + 1.2f * (1f - time));
			return from + (to - from) * time;
		}
	}
	public static class Ease3
	{
		private static readonly Dictionary<EaseType, Func<Vector3, Vector3, float, Vector3>> Types = new Dictionary<EaseType, Func<Vector3, Vector3, float, Vector3>>
		{
			{
				EaseType.Linear,
				(Func<Vector3, Vector3, float, Vector3>)Vector3.Lerp
			},
			{
				EaseType.SineIn,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.SineIn(from.x, to.x, time), Ease.SineIn(from.y, to.y, time), Ease.SineIn(from.z, to.z, time));
				}
			},
			{
				EaseType.SineOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.SineOut(from.x, to.x, time), Ease.SineOut(from.y, to.y, time), Ease.SineOut(from.z, to.z, time));
				}
			},
			{
				EaseType.SineInOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.SineInOut(from.x, to.x, time), Ease.SineInOut(from.y, to.y, time), Ease.SineInOut(from.z, to.z, time));
				}
			},
			{
				EaseType.QuadIn,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.QuadIn(from.x, to.x, time), Ease.QuadIn(from.y, to.y, time), Ease.QuadIn(from.z, to.z, time));
				}
			},
			{
				EaseType.QuadOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.QuadOut(from.x, to.x, time), Ease.QuadOut(from.y, to.y, time), Ease.QuadOut(from.z, to.z, time));
				}
			},
			{
				EaseType.QuadInOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.QuadInOut(from.x, to.x, time), Ease.QuadInOut(from.y, to.y, time), Ease.QuadInOut(from.z, to.z, time));
				}
			},
			{
				EaseType.CubicIn,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.CubicIn(from.x, to.x, time), Ease.CubicIn(from.y, to.y, time), Ease.CubicIn(from.z, to.z, time));
				}
			},
			{
				EaseType.CubicOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.CubicOut(from.x, to.x, time), Ease.CubicOut(from.y, to.y, time), Ease.CubicOut(from.z, to.z, time));
				}
			},
			{
				EaseType.CubicInOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.CubicInOut(from.x, to.x, time), Ease.CubicInOut(from.y, to.y, time), Ease.CubicInOut(from.z, to.z, time));
				}
			},
			{
				EaseType.QuartIn,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.QuartIn(from.x, to.x, time), Ease.QuartIn(from.y, to.y, time), Ease.QuartIn(from.z, to.z, time));
				}
			},
			{
				EaseType.QuartOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.QuartOut(from.x, to.x, time), Ease.QuartOut(from.y, to.y, time), Ease.QuartOut(from.z, to.z, time));
				}
			},
			{
				EaseType.QuartInOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.QuartInOut(from.x, to.x, time), Ease.QuartInOut(from.y, to.y, time), Ease.QuartInOut(from.z, to.z, time));
				}
			},
			{
				EaseType.QuintIn,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.QuintIn(from.x, to.x, time), Ease.QuintIn(from.y, to.y, time), Ease.QuintIn(from.z, to.z, time));
				}
			},
			{
				EaseType.QuintOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.QuintOut(from.x, to.x, time), Ease.QuintOut(from.y, to.y, time), Ease.QuintOut(from.z, to.z, time));
				}
			},
			{
				EaseType.QuintInOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.QuintInOut(from.x, to.x, time), Ease.QuintInOut(from.y, to.y, time), Ease.QuintInOut(from.z, to.z, time));
				}
			},
			{
				EaseType.ExpoIn,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.ExpoIn(from.x, to.x, time), Ease.ExpoIn(from.y, to.y, time), Ease.ExpoIn(from.z, to.z, time));
				}
			},
			{
				EaseType.ExpoOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.ExpoOut(from.x, to.x, time), Ease.ExpoOut(from.y, to.y, time), Ease.ExpoOut(from.z, to.z, time));
				}
			},
			{
				EaseType.ExpoInOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.ExpoInOut(from.x, to.x, time), Ease.ExpoInOut(from.y, to.y, time), Ease.ExpoInOut(from.z, to.z, time));
				}
			},
			{
				EaseType.CircIn,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.CircIn(from.x, to.x, time), Ease.CircIn(from.y, to.y, time), Ease.CircIn(from.z, to.z, time));
				}
			},
			{
				EaseType.CircOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.CircOut(from.x, to.x, time), Ease.CircOut(from.y, to.y, time), Ease.CircOut(from.z, to.z, time));
				}
			},
			{
				EaseType.CircInOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.CircInOut(from.x, to.x, time), Ease.CircInOut(from.y, to.y, time), Ease.CircInOut(from.z, to.z, time));
				}
			},
			{
				EaseType.BackIn,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.BackIn(from.x, to.x, time), Ease.BackIn(from.y, to.y, time), Ease.BackIn(from.z, to.z, time));
				}
			},
			{
				EaseType.BackOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.BackOut(from.x, to.x, time), Ease.BackOut(from.y, to.y, time), Ease.BackOut(from.z, to.z, time));
				}
			},
			{
				EaseType.BackInOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.BackInOut(from.x, to.x, time), Ease.BackInOut(from.y, to.y, time), Ease.BackInOut(from.z, to.z, time));
				}
			},
			{
				EaseType.ElasticIn,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.ElasticIn(from.x, to.x, time), Ease.ElasticIn(from.y, to.y, time), Ease.ElasticIn(from.z, to.z, time));
				}
			},
			{
				EaseType.ElasticOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.ElasticOut(from.x, to.x, time), Ease.ElasticOut(from.y, to.y, time), Ease.ElasticOut(from.z, to.z, time));
				}
			},
			{
				EaseType.ElasticInOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.ElasticInOut(from.x, to.x, time), Ease.ElasticInOut(from.y, to.y, time), Ease.ElasticInOut(from.z, to.z, time));
				}
			},
			{
				EaseType.BounceIn,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.BounceIn(from.x, to.x, time), Ease.BounceIn(from.y, to.y, time), Ease.BounceIn(from.z, to.z, time));
				}
			},
			{
				EaseType.BounceOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.BounceOut(from.x, to.x, time), Ease.BounceOut(from.y, to.y, time), Ease.BounceOut(from.z, to.z, time));
				}
			},
			{
				EaseType.BounceInOut,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.BounceInOut(from.x, to.x, time), Ease.BounceInOut(from.y, to.y, time), Ease.BounceInOut(from.z, to.z, time));
				}
			},
			{
				EaseType.Spring,
				(Vector3 from, Vector3 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					return new Vector3(Ease.Spring(from.x, to.x, time), Ease.Spring(from.y, to.y, time), Ease.Spring(from.z, to.z, time));
				}
			}
		};

		public static IEnumerator Go(MonoBehaviour m, Vector3 from, Vector3 to, float time, Action<Vector3> update, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			IEnumerator enumerator = GoCoroutine(m, from, to, time, update, complete, type, delay, repeat, pingPong, realTime);
			m.StartCoroutine(enumerator);
			return enumerator;
		}

		private static IEnumerator GoCoroutine(MonoBehaviour m, Vector3 from, Vector3 to, float time, Action<Vector3> update, Action complete, EaseType type, float delay, int repeat, bool pingPong, bool realTime)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			int counter = repeat;
			while (repeat == 0 || repeat == -1 || counter > 0)
			{
				if (delay > 0f)
				{
					if (realTime)
					{
						yield return new WaitForSecondsRealtime(delay);
					}
					else
					{
						yield return (object)new WaitForSeconds(delay);
					}
				}
				float t = 0f;
				while (t <= 1f)
				{
					t += (realTime ? Time.unscaledDeltaTime : Time.deltaTime) / time;
					update(Types[type](from, to, Mathf.Clamp01(t)));
					yield return null;
				}
				if (pingPong)
				{
					if (delay > 0f)
					{
						if (realTime)
						{
							yield return new WaitForSecondsRealtime(delay);
						}
						else
						{
							yield return (object)new WaitForSeconds(delay);
						}
					}
					t = 0f;
					while (t <= 1f)
					{
						t += (realTime ? Time.unscaledDeltaTime : Time.deltaTime) / time;
						update(Types[type](to, from, Mathf.Clamp01(t)));
						yield return null;
					}
				}
				if (repeat != 0)
				{
					counter--;
				}
				if (repeat == 0 || repeat == -1)
				{
					complete?.Invoke();
				}
			}
			if (repeat != 0)
			{
				complete?.Invoke();
			}
		}

		public static IEnumerator GoPositionTo(MonoBehaviour m, Vector3 to, float time, Action<Vector3> update = null, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return GoPosition(m, ((Component)m).transform.localPosition, to, time, update, complete, type, delay, repeat, pingPong, realTime);
		}

		public static IEnumerator GoPositionBy(MonoBehaviour m, Vector3 by, float time, Action<Vector3> update = null, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			Vector3 localPosition = ((Component)m).transform.localPosition;
			return GoPosition(m, localPosition, localPosition + by, time, update, complete, type, delay, repeat, pingPong, realTime);
		}

		public static IEnumerator GoPosition(MonoBehaviour m, Vector3 from, Vector3 to, float time, Action<Vector3> update = null, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			IEnumerator enumerator = GoPositionCoroutine(m, from, to, time, update, complete, type, delay, repeat, pingPong, realTime);
			m.StartCoroutine(enumerator);
			return enumerator;
		}

		private static IEnumerator GoPositionCoroutine(MonoBehaviour m, Vector3 from, Vector3 to, float time, Action<Vector3> update, Action complete, EaseType type, float delay, int repeat, bool pingPong, bool realTime)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			int counter = repeat;
			while (repeat == 0 || repeat == -1 || counter > 0)
			{
				if (delay > 0f)
				{
					if (realTime)
					{
						yield return new WaitForSecondsRealtime(delay);
					}
					else
					{
						yield return (object)new WaitForSeconds(delay);
					}
				}
				float t = 0f;
				while (t <= 1f)
				{
					t += (realTime ? Time.unscaledDeltaTime : Time.deltaTime) / time;
					Vector3 val = Types[type](from, to, Mathf.Clamp01(t));
					((Component)m).transform.localPosition = val;
					update?.Invoke(val);
					yield return null;
				}
				((Component)m).transform.localPosition = to;
				if (pingPong)
				{
					if (delay > 0f)
					{
						if (realTime)
						{
							yield return new WaitForSecondsRealtime(delay);
						}
						else
						{
							yield return (object)new WaitForSeconds(delay);
						}
					}
					t = 0f;
					while (t <= 1f)
					{
						t += (realTime ? Time.unscaledDeltaTime : Time.deltaTime) / time;
						Vector3 val2 = Types[type](to, from, Mathf.Clamp01(t));
						((Component)m).transform.localPosition = val2;
						update?.Invoke(val2);
						yield return null;
					}
					((Component)m).transform.localPosition = from;
				}
				if (repeat != 0)
				{
					counter--;
				}
				if (repeat == 0 || repeat == -1)
				{
					complete?.Invoke();
				}
			}
			if (repeat != 0)
			{
				complete?.Invoke();
			}
		}

		public static IEnumerator GoRotationTo(MonoBehaviour m, Vector3 to, float time, Action<Vector3> update = null, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return GoRotation(m, ((Component)m).transform.localEulerAngles, to, time, update, complete, type, delay, repeat, pingPong, realTime);
		}

		public static IEnumerator GoRotationBy(MonoBehaviour m, Vector3 by, float time, Action<Vector3> update = null, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			Vector3 localEulerAngles = ((Component)m).transform.localEulerAngles;
			return GoRotation(m, localEulerAngles, localEulerAngles + by, time, update, complete, type, delay, repeat, pingPong, realTime);
		}

		public static IEnumerator GoRotation(MonoBehaviour m, Vector3 from, Vector3 to, float time, Action<Vector3> update = null, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			IEnumerator enumerator = GoRotationCoroutine(m, from, to, time, update, complete, type, delay, repeat, pingPong, realTime);
			m.StartCoroutine(enumerator);
			return enumerator;
		}

		private static IEnumerator GoRotationCoroutine(MonoBehaviour m, Vector3 from, Vector3 to, float time, Action<Vector3> update, Action complete, EaseType type, float delay, int repeat, bool pingPong, bool realTime)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			int counter = repeat;
			while (repeat == 0 || repeat == -1 || counter > 0)
			{
				if (delay > 0f)
				{
					if (realTime)
					{
						yield return new WaitForSecondsRealtime(delay);
					}
					else
					{
						yield return (object)new WaitForSeconds(delay);
					}
				}
				float t = 0f;
				while (t <= 1f)
				{
					t += (realTime ? Time.unscaledDeltaTime : Time.deltaTime) / time;
					Vector3 val = Types[type](from, to, Mathf.Clamp01(t));
					((Component)m).transform.localEulerAngles = val;
					update?.Invoke(val);
					yield return null;
				}
				((Component)m).transform.localEulerAngles = to;
				if (pingPong)
				{
					if (delay > 0f)
					{
						if (realTime)
						{
							yield return new WaitForSecondsRealtime(delay);
						}
						else
						{
							yield return (object)new WaitForSeconds(delay);
						}
					}
					t = 0f;
					while (t <= 1f)
					{
						t += (realTime ? Time.unscaledDeltaTime : Time.deltaTime) / time;
						Vector3 val2 = Types[type](to, from, Mathf.Clamp01(t));
						((Component)m).transform.localEulerAngles = val2;
						update?.Invoke(val2);
						yield return null;
					}
					((Component)m).transform.localEulerAngles = from;
				}
				if (repeat != 0)
				{
					counter--;
				}
				if (repeat == 0 || repeat == -1)
				{
					complete?.Invoke();
				}
			}
			if (repeat != 0)
			{
				complete?.Invoke();
			}
		}

		public static IEnumerator GoScaleTo(MonoBehaviour m, Vector3 to, float time, Action<Vector3> update = null, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return GoScale(m, ((Component)m).transform.localScale, to, time, update, complete, type, delay, repeat, pingPong, realTime);
		}

		public static IEnumerator GoScaleBy(MonoBehaviour m, Vector3 by, float time, Action<Vector3> update = null, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			Vector3 localScale = ((Component)m).transform.localScale;
			return GoScale(m, localScale, localScale + by, time, update, complete, type, delay, repeat, pingPong, realTime);
		}

		public static IEnumerator GoScale(MonoBehaviour m, Vector3 from, Vector3 to, float time, Action<Vector3> update = null, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			IEnumerator enumerator = GoScaleCoroutine(m, from, to, time, update, complete, type, delay, repeat, pingPong, realTime);
			m.StartCoroutine(enumerator);
			return enumerator;
		}

		private static IEnumerator GoScaleCoroutine(MonoBehaviour m, Vector3 from, Vector3 to, float time, Action<Vector3> update, Action complete, EaseType type, float delay, int repeat, bool pingPong, bool realTime)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			float last = Time.unscaledTime;
			Func<float> deltaTime = () =>
			{
				float unscaledTime = Time.unscaledTime;
				float result = unscaledTime - last;
				last = unscaledTime;
				return result;
			};
			int counter = repeat;
			while (repeat == 0 || repeat == -1 || counter > 0)
			{
				if (delay > 0f)
				{
					if (realTime)
					{
						yield return new WaitForSecondsRealtime(delay);
					}
					else
					{
						yield return (object)new WaitForSeconds(delay);
					}
				}
				float t = 0f;
				while (t <= 1f)
				{
					t += (realTime ? deltaTime() : Time.deltaTime) / time;
					Vector3 val = Types[type](from, to, Mathf.Clamp01(t));
					((Component)m).transform.localScale = val;
					update?.Invoke(val);
					yield return null;
				}
				((Component)m).transform.localScale = to;
				if (pingPong)
				{
					if (delay > 0f)
					{
						if (realTime)
						{
							yield return new WaitForSecondsRealtime(delay);
						}
						else
						{
							yield return (object)new WaitForSeconds(delay);
						}
					}
					t = 0f;
					while (t <= 1f)
					{
						t += (realTime ? deltaTime() : Time.deltaTime) / time;
						Vector3 val2 = Types[type](to, from, Mathf.Clamp01(t));
						((Component)m).transform.localScale = val2;
						update?.Invoke(val2);
						yield return null;
					}
					((Component)m).transform.localScale = from;
				}
				if (repeat != 0)
				{
					counter--;
				}
				if (repeat == 0 || repeat == -1)
				{
					complete?.Invoke();
				}
			}
			if (repeat != 0)
			{
				complete?.Invoke();
			}
		}

		private static Color GetColor(MonoBehaviour m)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			Image component = ((Component)m).GetComponent<Image>();
			if (!((Object)(object)component == (Object)null))
			{
				return ((Graphic)component).color;
			}
			return Camera.main.backgroundColor;
		}

		public static IEnumerator GoColorTo(MonoBehaviour m, Vector3 to, float time, Action<Vector3> update = null, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return GoColor(m, GetColor(m).GetVector3(), to, time, update, complete, type, delay, repeat, pingPong, realTime);
		}

		public static IEnumerator GoColorBy(MonoBehaviour m, Vector3 by, float time, Action<Vector3> update = null, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			Vector3 vector = GetColor(m).GetVector3();
			return GoColor(m, vector, vector + by, time, update, complete, type, delay, repeat, pingPong, realTime);
		}

		public static IEnumerator GoColor(MonoBehaviour m, Vector3 from, Vector3 to, float time, Action<Vector3> update = null, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			IEnumerator enumerator = GoColorCoroutine(m, from, to, time, update, complete, type, delay, repeat, pingPong, realTime);
			m.StartCoroutine(enumerator);
			return enumerator;
		}

		private static IEnumerator GoColorCoroutine(MonoBehaviour m, Vector3 from, Vector3 to, float time, Action<Vector3> update, Action complete, EaseType type, float delay, int repeat, bool pingPong, bool realTime)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			Image image = ((Component)m).GetComponent<Image>();
			Camera camera = Camera.main;
			Action<Vector3> setColor = (Vector3 value) =>
			{
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)image == (Object)null)
				{
					camera.backgroundColor = value.GetColor().SetAlpha(camera.backgroundColor.a);
				}
				else
				{
					((Graphic)image).color = value.GetColor().SetAlpha(((Graphic)image).color.a);
				}
			};
			int counter = repeat;
			while (repeat == 0 || repeat == -1 || counter > 0)
			{
				if (delay > 0f)
				{
					if (realTime)
					{
						yield return new WaitForSecondsRealtime(delay);
					}
					else
					{
						yield return (object)new WaitForSeconds(delay);
					}
				}
				float t = 0f;
				while (t <= 1f)
				{
					t += (realTime ? Time.unscaledDeltaTime : Time.deltaTime) / time;
					Vector3 obj = Types[type](from, to, Mathf.Clamp01(t));
					setColor(obj);
					update?.Invoke(obj);
					yield return null;
				}
				setColor(to);
				if (pingPong)
				{
					if (delay > 0f)
					{
						if (realTime)
						{
							yield return new WaitForSecondsRealtime(delay);
						}
						else
						{
							yield return (object)new WaitForSeconds(delay);
						}
					}
					t = 0f;
					while (t <= 1f)
					{
						t += (realTime ? Time.unscaledDeltaTime : Time.deltaTime) / time;
						Vector3 obj2 = Types[type](to, from, Mathf.Clamp01(t));
						setColor(obj2);
						update?.Invoke(obj2);
						yield return null;
					}
					setColor(from);
				}
				if (repeat != 0)
				{
					counter--;
				}
				if (repeat == 0 || repeat == -1)
				{
					complete?.Invoke();
				}
			}
			if (repeat != 0)
			{
				complete?.Invoke();
			}
		}
	}
	public static class Ease4
	{
		private static readonly Dictionary<EaseType, Func<Vector4, Vector4, float, Vector4>> Types = new Dictionary<EaseType, Func<Vector4, Vector4, float, Vector4>>
		{
			{
				EaseType.Linear,
				(Func<Vector4, Vector4, float, Vector4>)Vector4.Lerp
			},
			{
				EaseType.SineIn,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.SineIn(from.x, to.x, time), Ease.SineIn(from.y, to.y, time), Ease.SineIn(from.z, to.z, time), Ease.SineIn(from.w, to.w, time));
				}
			},
			{
				EaseType.SineOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.SineOut(from.x, to.x, time), Ease.SineOut(from.y, to.y, time), Ease.SineOut(from.z, to.z, time), Ease.SineOut(from.w, to.w, time));
				}
			},
			{
				EaseType.SineInOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.SineInOut(from.x, to.x, time), Ease.SineInOut(from.y, to.y, time), Ease.SineInOut(from.z, to.z, time), Ease.SineInOut(from.w, to.w, time));
				}
			},
			{
				EaseType.QuadIn,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.QuadIn(from.x, to.x, time), Ease.QuadIn(from.y, to.y, time), Ease.QuadIn(from.z, to.z, time), Ease.QuadIn(from.w, to.w, time));
				}
			},
			{
				EaseType.QuadOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.QuadOut(from.x, to.x, time), Ease.QuadOut(from.y, to.y, time), Ease.QuadOut(from.z, to.z, time), Ease.QuadOut(from.w, to.w, time));
				}
			},
			{
				EaseType.QuadInOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.QuadInOut(from.x, to.x, time), Ease.QuadInOut(from.y, to.y, time), Ease.QuadInOut(from.z, to.z, time), Ease.QuadInOut(from.w, to.w, time));
				}
			},
			{
				EaseType.CubicIn,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.CubicIn(from.x, to.x, time), Ease.CubicIn(from.y, to.y, time), Ease.CubicIn(from.z, to.z, time), Ease.CubicIn(from.w, to.w, time));
				}
			},
			{
				EaseType.CubicOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.CubicOut(from.x, to.x, time), Ease.CubicOut(from.y, to.y, time), Ease.CubicOut(from.z, to.z, time), Ease.CubicOut(from.w, to.w, time));
				}
			},
			{
				EaseType.CubicInOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.CubicInOut(from.x, to.x, time), Ease.CubicInOut(from.y, to.y, time), Ease.CubicInOut(from.z, to.z, time), Ease.CubicInOut(from.w, to.w, time));
				}
			},
			{
				EaseType.QuartIn,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.QuartIn(from.x, to.x, time), Ease.QuartIn(from.y, to.y, time), Ease.QuartIn(from.z, to.z, time), Ease.QuartIn(from.w, to.w, time));
				}
			},
			{
				EaseType.QuartOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.QuartOut(from.x, to.x, time), Ease.QuartOut(from.y, to.y, time), Ease.QuartOut(from.z, to.z, time), Ease.QuartOut(from.w, to.w, time));
				}
			},
			{
				EaseType.QuartInOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.QuartInOut(from.x, to.x, time), Ease.QuartInOut(from.y, to.y, time), Ease.QuartInOut(from.z, to.z, time), Ease.QuartInOut(from.w, to.w, time));
				}
			},
			{
				EaseType.QuintIn,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.QuintIn(from.x, to.x, time), Ease.QuintIn(from.y, to.y, time), Ease.QuintIn(from.z, to.z, time), Ease.QuintIn(from.w, to.w, time));
				}
			},
			{
				EaseType.QuintOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.QuintOut(from.x, to.x, time), Ease.QuintOut(from.y, to.y, time), Ease.QuintOut(from.z, to.z, time), Ease.QuintOut(from.w, to.w, time));
				}
			},
			{
				EaseType.QuintInOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.QuintInOut(from.x, to.x, time), Ease.QuintInOut(from.y, to.y, time), Ease.QuintInOut(from.z, to.z, time), Ease.QuintInOut(from.w, to.w, time));
				}
			},
			{
				EaseType.ExpoIn,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.ExpoIn(from.x, to.x, time), Ease.ExpoIn(from.y, to.y, time), Ease.ExpoIn(from.z, to.z, time), Ease.ExpoIn(from.w, to.w, time));
				}
			},
			{
				EaseType.ExpoOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.ExpoOut(from.x, to.x, time), Ease.ExpoOut(from.y, to.y, time), Ease.ExpoOut(from.z, to.z, time), Ease.ExpoOut(from.w, to.w, time));
				}
			},
			{
				EaseType.ExpoInOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.ExpoInOut(from.x, to.x, time), Ease.ExpoInOut(from.y, to.y, time), Ease.ExpoInOut(from.z, to.z, time), Ease.ExpoInOut(from.w, to.w, time));
				}
			},
			{
				EaseType.CircIn,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.CircIn(from.x, to.x, time), Ease.CircIn(from.y, to.y, time), Ease.CircIn(from.z, to.z, time), Ease.CircIn(from.w, to.w, time));
				}
			},
			{
				EaseType.CircOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.CircOut(from.x, to.x, time), Ease.CircOut(from.y, to.y, time), Ease.CircOut(from.z, to.z, time), Ease.CircOut(from.w, to.w, time));
				}
			},
			{
				EaseType.CircInOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.CircInOut(from.x, to.x, time), Ease.CircInOut(from.y, to.y, time), Ease.CircInOut(from.z, to.z, time), Ease.CircInOut(from.w, to.w, time));
				}
			},
			{
				EaseType.BackIn,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.BackIn(from.x, to.x, time), Ease.BackIn(from.y, to.y, time), Ease.BackIn(from.z, to.z, time), Ease.BackIn(from.w, to.w, time));
				}
			},
			{
				EaseType.BackOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.BackOut(from.x, to.x, time), Ease.BackOut(from.y, to.y, time), Ease.BackOut(from.z, to.z, time), Ease.BackOut(from.w, to.w, time));
				}
			},
			{
				EaseType.BackInOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.BackInOut(from.x, to.x, time), Ease.BackInOut(from.y, to.y, time), Ease.BackInOut(from.z, to.z, time), Ease.BackInOut(from.w, to.w, time));
				}
			},
			{
				EaseType.ElasticIn,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.ElasticIn(from.x, to.x, time), Ease.ElasticIn(from.y, to.y, time), Ease.ElasticIn(from.z, to.z, time), Ease.ElasticIn(from.w, to.w, time));
				}
			},
			{
				EaseType.ElasticOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.ElasticOut(from.x, to.x, time), Ease.ElasticOut(from.y, to.y, time), Ease.ElasticOut(from.z, to.z, time), Ease.ElasticOut(from.w, to.w, time));
				}
			},
			{
				EaseType.ElasticInOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.ElasticInOut(from.x, to.x, time), Ease.ElasticInOut(from.y, to.y, time), Ease.ElasticInOut(from.z, to.z, time), Ease.ElasticInOut(from.w, to.w, time));
				}
			},
			{
				EaseType.BounceIn,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.BounceIn(from.x, to.x, time), Ease.BounceIn(from.y, to.y, time), Ease.BounceIn(from.z, to.z, time), Ease.BounceIn(from.w, to.w, time));
				}
			},
			{
				EaseType.BounceOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.BounceOut(from.x, to.x, time), Ease.BounceOut(from.y, to.y, time), Ease.BounceOut(from.z, to.z, time), Ease.BounceOut(from.w, to.w, time));
				}
			},
			{
				EaseType.BounceInOut,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.BounceInOut(from.x, to.x, time), Ease.BounceInOut(from.y, to.y, time), Ease.BounceInOut(from.z, to.z, time), Ease.BounceInOut(from.w, to.w, time));
				}
			},
			{
				EaseType.Spring,
				(Vector4 from, Vector4 to, float time) =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					return new Vector4(Ease.Spring(from.x, to.x, time), Ease.Spring(from.y, to.y, time), Ease.Spring(from.z, to.z, time), Ease.Spring(from.w, to.w, time));
				}
			}
		};

		public static IEnumerator Go(MonoBehaviour m, Vector4 from, Vector4 to, float time, Action<Vector4> update, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			IEnumerator enumerator = GoCoroutine(m, from, to, time, update, complete, type, delay, repeat, pingPong, realTime);
			m.StartCoroutine(enumerator);
			return enumerator;
		}

		private static IEnumerator GoCoroutine(MonoBehaviour m, Vector4 from, Vector4 to, float time, Action<Vector4> update, Action complete, EaseType type, float delay, int repeat, bool pingPong, bool realTime)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			int counter = repeat;
			while (repeat == 0 || repeat == -1 || counter > 0)
			{
				if (delay > 0f)
				{
					if (realTime)
					{
						yield return new WaitForSecondsRealtime(delay);
					}
					else
					{
						yield return (object)new WaitForSeconds(delay);
					}
				}
				float t = 0f;
				while (t <= 1f)
				{
					t += (realTime ? Time.unscaledDeltaTime : Time.deltaTime) / time;
					update(Types[type](from, to, Mathf.Clamp01(t)));
					yield return null;
				}
				if (pingPong)
				{
					if (delay > 0f)
					{
						if (realTime)
						{
							yield return new WaitForSecondsRealtime(delay);
						}
						else
						{
							yield return (object)new WaitForSeconds(delay);
						}
					}
					t = 0f;
					while (t <= 1f)
					{
						t += (realTime ? Time.unscaledDeltaTime : Time.deltaTime) / time;
						update(Types[type](to, from, Mathf.Clamp01(t)));
						yield return null;
					}
				}
				if (repeat != 0)
				{
					counter--;
				}
				if (repeat == 0 || repeat == -1)
				{
					complete?.Invoke();
				}
			}
			if (repeat != 0)
			{
				complete?.Invoke();
			}
		}

		private static Color GetColor(MonoBehaviour m)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			Image component = ((Component)m).GetComponent<Image>();
			if (!((Object)(object)component == (Object)null))
			{
				return ((Graphic)component).color;
			}
			return Camera.main.backgroundColor;
		}

		public static IEnumerator GoColorTo(MonoBehaviour m, Vector4 to, float time, Action<Vector4> update = null, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return GoColor(m, GetColor(m).GetVector4(), to, time, update, complete, type, delay, repeat, pingPong, realTime);
		}

		public static IEnumerator GoColorBy(MonoBehaviour m, Vector4 by, float time, Action<Vector4> update = null, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			Vector4 vector = GetColor(m).GetVector4();
			return GoColor(m, vector, vector + by, time, update, complete, type, delay, repeat, pingPong, realTime);
		}

		public static IEnumerator GoColor(MonoBehaviour m, Vector4 from, Vector4 to, float time, Action<Vector4> update = null, Action complete = null, EaseType type = EaseType.Linear, float delay = 0f, int repeat = 1, bool pingPong = false, bool realTime = false)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			IEnumerator enumerator = GoColorCoroutine(m, from, to, time, update, complete, type, delay, repeat, pingPong, realTime);
			m.StartCoroutine(enumerator);
			return enumerator;
		}

		private static IEnumerator GoColorCoroutine(MonoBehaviour m, Vector4 from, Vector4 to, float time, Action<Vector4> update, Action complete, EaseType type, float delay, int repeat, bool pingPong, bool realTime)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			Image image = ((Component)m).GetComponent<Image>();
			Camera camera = Camera.main;
			Action<Vector4> setColor = (Vector4 value) =>
			{
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)image == (Object)null)
				{
					camera.backgroundColor = value.GetColor();
				}
				else
				{
					((Graphic)image).color = value.GetColor();
				}
			};
			int counter = repeat;
			while (repeat == 0 || repeat == -1 || counter > 0)
			{
				if (delay > 0f)
				{
					if (realTime)
					{
						yield return new WaitForSecondsRealtime(delay);
					}
					else
					{
						yield return (object)new WaitForSeconds(delay);
					}
				}
				float t = 0f;
				while (t <= 1f)
				{
					t += (realTime ? Time.unscaledDeltaTime : Time.deltaTime) / time;
					Vector4 obj = Types[type](from, to, Mathf.Clamp01(t));
					setColor(obj);
					update?.Invoke(obj);
					yield return null;
				}
				setColor(to);
				if (pingPong)
				{
					if (delay > 0f)
					{
						if (realTime)
						{
							yield return new WaitForSecondsRealtime(delay);
						}
						else
						{
							yield return (object)new WaitForSeconds(delay);
						}
					}
					t = 0f;
					while (t <= 1f)
					{
						t += (realTime ? Time.unscaledDeltaTime : Time.deltaTime) / time;
						Vector4 obj2 = Types[type](to, from, Mathf.Clamp01(t));
						setColor(obj2);
						update?.Invoke(obj2);
						yield return null;
					}
					setColor(from);
				}
				if (repeat != 0)
				{
					counter--;
				}
				if (repeat == 0 || repeat == -1)
				{
					complete?.Invoke();
				}
			}
			if (repeat != 0)
			{
				complete?.Invoke();
			}
		}
	}
	public static class VectorExtensions
	{
		public static Color GetColor(this Vector3 v)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			return new Color(Mathf.Clamp01(v.x), Mathf.Clamp01(v.y), Mathf.Clamp01(v.z));
		}

		public static Color GetColor(this Vector4 v)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			return new Color(Mathf.Clamp01(v.x), Mathf.Clamp01(v.y), Mathf.Clamp01(v.z), Mathf.Clamp01(v.w));
		}
	}
	public static class ColorExtensions
	{
		public static Vector3 GetVector3(this Color c)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(c.r, c.g, c.b);
		}

		public static Vector4 GetVector4(this Color c)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			return new Vector4(c.r, c.g, c.b, c.a);
		}

		public static Color SetAlpha(this Color c, float a)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			return new Color(c.r, c.g, c.b, a);
		}
	}
	public class WaitForSecondsRealtime : CustomYieldInstruction
	{
		private float _time;

		public override bool keepWaiting => Time.unscaledTime < _time;

		public WaitForSecondsRealtime(float time)
		{
			_time = Time.unscaledTime + time;
		}
	}
	public class Pool : MonoBehaviour
	{
		[SerializeField]
		private int _count = 12;

		[SerializeField]
		private GameObject _prefab;

		private List<GameObject> _pool;

		private const string _name = "Pool";

		private Transform _t;

		private void Awake()
		{
			_t = ((Component)this).transform;
			_pool = new List<GameObject>(_count);
			for (int i = 0; i < _count; i++)
			{
				_pool.Add(New());
			}
		}

		private GameObject New()
		{
			GameObject val = Object.Instantiate<GameObject>(_prefab);
			if ((Object)(object)val != (Object)null)
			{
				val.transform.SetParent(_t, false);
				((Object)val).name = "Pool";
				val.SetActive(false);
			}
			return val;
		}

		public GameObject Enter()
		{
			GameObject val = null;
			for (int i = 0; i < _pool.Count; i++)
			{
				if (!((Object)(object)val == (Object)null))
				{
					break;
				}
				if (!_pool[i].activeInHierarchy)
				{
					val = _pool[i];
				}
			}
			if ((Object)(object)val == (Object)null)
			{
				val = New();
				_pool.Add(val);
			}
			val.SetActive(true);
			return val;
		}

		public void Exit(GameObject o)
		{
			if (!((Object)(object)o == (Object)null))
			{
				((Object)o).name = "Pool";
				o.SetActive(false);
			}
		}
	}
}
namespace GGEZ
{
	public enum ZoomDemoMode
	{
		Bubbling,
		Shimmering,
		Marching
	}
	public class ZoomDemo : MonoBehaviour
	{
		public Camera MainCamera;

		public AnimationCurve TimeToOrthographicSize;

		public Text TitleText;

		public Text BodyText;

		public Text SwitchModesButtonText;

		private ZoomDemoMode mode = ZoomDemoMode.Marching;

		public void SwitchModes()
		{
			switch (mode)
			{
			case ZoomDemoMode.Shimmering:
				mode = ZoomDemoMode.Bubbling;
				SwitchModesButtonText.text = "Switch to Marching";
				break;
			case ZoomDemoMode.Bubbling:
				mode = ZoomDemoMode.Marching;
				SwitchModesButtonText.text = "Switch to Shimmering";
				break;
			case ZoomDemoMode.Marching:
				mode = ZoomDemoMode.Shimmering;
				SwitchModesButtonText.text = "Switch to Bubbling";
				break;
			}
			TitleText.text = mode.ToString();
		}

		private void Start()
		{
			SwitchModes();
		}

		private void Update()
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			PerfectPixelCamera perfectPixelCamera = ((Component)MainCamera).GetComponent(typeof(PerfectPixelCamera)) as PerfectPixelCamera;
			bool flag = (Object)(object)perfectPixelCamera != (Object)null && ((Behaviour)perfectPixelCamera).isActiveAndEnabled;
			float orthographicSize = MainCamera.orthographicSize;
			float num = orthographicSize;
			if (flag)
			{
				Rect pixelRect = MainCamera.pixelRect;
				float num2 = Mathf.Max(1f, Mathf.Ceil(1f * pixelRect.height / (MainCamera.orthographicSize * 2f * (float)perfectPixelCamera.TexturePixelsPerWorldUnit)));
				pixelRect = MainCamera.pixelRect;
				num = 1f * pixelRect.height / (num2 * 2f * (float)perfectPixelCamera.TexturePixelsPerWorldUnit);
			}
			switch (mode)
			{
			case ZoomDemoMode.Shimmering:
				MainCamera.orthographicSize = TimeToOrthographicSize.Evaluate(Time.time);
				((Component)MainCamera).transform.position = Vector3.back * 10f;
				if (flag)
				{
					BodyText.text = "The Perfect Pixel Camera snaps the orthographicSize of the camera component behind the scenes to fix zooming issues (" + orthographicSize.ToString("0.0") + " snapped to " + num.ToString("0.0") + ")";
				}
				else
				{
					BodyText.text = "A miscalibrated camera shows a shimmer or wave pattern as it zooms. Add the PerfectPixelCamera component to the MainCamera object to check out the fix, or tap the button to see other errors.";
				}
				break;
			case ZoomDemoMode.Bubbling:
				((Component)MainCamera).transform.position = new Vector3(Mathf.Cos(Time.time * 0.5f) * 5f, Mathf.Sin(Time.time * 0.5f) * 5f, -10f);
				if (flag)
				{
					BodyText.text = "With the PerfectPixelCamera enabled, panning doesn't jiggle anymore!";
				}
				else
				{
					BodyText.text = "When panning, the image appears to jiggle and bubble. Add the PerfectPixelCamera component to the MainCamera object to check out the fix, or tap the button to see other errors.";
				}
				break;
			case ZoomDemoMode.Marching:
				MainCamera.orthographicSize = (float)MainCamera.pixelHeight / 32f * 0.5f;
				((Component)MainCamera).transform.position = new Vector3((Mathf.PingPong(Time.time * 0.1f, 2f) - 1f) * 0.1f, (Mathf.PingPong(Time.time * 0.1f, 2f) - 1f) * 3f, -10f);
				if (flag)
				{
					BodyText.text = "With the PerfectPixelCamera enabled, the camera's projection matrix is automatically offset by less than a pixel and marching is fixed.";
				}
				else
				{
					BodyText.text = "Even with orthographicSize set correctly to " + MainCamera.orthographicSize.ToString("0.0") + ", an alignment issue is easily visible at the border at max zoom. Use the scale slider to zoom in. Add the PerfectPixelCamera component to the MainCamera object to check out the fix, or tap the button to see other errors.";
				}
				break;
			}
		}
	}
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[HelpURL("http://ggez.org/posts/perfect-pixel-camera/")]
	[DisallowMultipleComponent]
	[AddComponentMenu("GGEZ/Camera/Perfect Pixel Camera")]
	public class PerfectPixelCamera : MonoBehaviour
	{
		[Tooltip("The number of texture pixels that fit in 1.0 world units. Common values are 8, 16, 32 and 64. If you're making a tile-based game, this is your tile size.")]
		[Range(1f, 64f)]
		public int TexturePixelsPerWorldUnit = 16;

		private Camera cameraComponent;

		private const float halfPixelOffsetIfNeededForD3D = 0f;

		public float SnapSizeWorldUnits { get; private set; }

		private void OnEnable()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected Obj, but got Unknown
			cameraComponent = (Camera)((Component)this).GetComponent(typeof(Camera));
			LateUpdate();
		}

		private void OnDisable()
		{
			if (!((Object)(object)cameraComponent == (Object)null))
			{
				cameraComponent.ResetProjectionMatrix();
				cameraComponent = null;
			}
		}

		private void LateUpdate()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			Camera val = cameraComponent;
			val.transparencySortMode = (TransparencySortMode)2;
			val.orthographic = true;
			((Component)val).transform.rotation = Quaternion.identity;
			val.orthographicSize = Mathf.Max(val.orthographicSize, 1E-05f);
			Rect pixelRect = val.pixelRect;
			float num = TexturePixelsPerWorldUnit;
			float num2 = Mathf.Max(1f, Mathf.Ceil(1f * pixelRect.height / (val.orthographicSize * 2f * num)));
			float num3 = 1f * pixelRect.width / (num2 * 2f * num);
			float num4 = 1f * pixelRect.height / (num2 * 2f * num);
			float num5 = 1f / (num2 * num);
			float num6 = 0f * num5;
			float num7 = num6 - Mathf.Repeat(num5 + Mathf.Repeat(((Component)val).transform.position.x, num5), num5);
			float num8 = num6 - Mathf.Repeat(num5 + Mathf.Repeat(((Component)val).transform.position.y, num5), num5);
			SnapSizeWorldUnits = num5;
			val.projectionMatrix = Matrix4x4.Ortho(0f - num3 + num7, num3 + num7, 0f - num4 + num8, num4 + num8, val.nearClipPlane, val.farClipPlane);
		}
	}
}
