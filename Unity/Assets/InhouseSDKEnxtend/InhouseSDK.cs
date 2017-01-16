using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.Events;

public partial class InhouseSDK : MonoBehaviour {
	public const string FB_EVENT_SCORE_25 = "FB_EVENT_SCORE_25";
	public const string FB_EVENT_SCORE_50 = "FB_EVENT_SCORE_50";
	public const string FB_EVENT_SCORE_100 = "FB_EVENT_SCORE_100";
	public const string FB_EVENT_SCORE_300 = "FB_EVENT_SCORE_300";
	public const string FB_EVENT_SCORE_300PLUS = "FB_EVENT_SCORE_300PLUS+";

	public const string FB_EVENT_ADSERVICE_CLICK = "FB_EVENT_ADSERVICE_CLICK";



	public partial class ConfigData {
		public RateReviewConfig RateReview = new RateReviewConfig ();
		public AdsConfig Ads = new AdsConfig ();
		public PromoteGameConfig PromoteGame = new PromoteGameConfig();
	}


	[Serializable]
	public class LanguageV2 {
		public Dictionary<string, string> Content = new Dictionary<string, string>();

		public void ParseLanguage(Hashtable data) {
			if (data == null)
				return;
			Content = HashtableToDictionary<string, string> (data);
		}
		public string GetContent(string key) {
			return Content [key];
		}
	}

	[Serializable]
	public class LanguagesV2 {
		public Dictionary<string, LanguageV2> languages = new Dictionary<string, LanguageV2> ();
		/*
		 * Get current language, if key not exist, get first language
		 */
		public LanguageV2 getLanguage(string key) {
			if (languages.ContainsKey (key))
				return languages [key];
			else {
				if (languages.ContainsKey ("en"))
					return languages ["en"];
				else
					return languages.First ().Value;
			}
		}

		public LanguageV2 getLanguage() {
			return getLanguage (InhouseSDK.getInstance ().GetCurrentSystemLanguage ());
		}

		public void ParseLanguage(Hashtable languagesData) {
			languages.Clear ();
			foreach (DictionaryEntry entry in languagesData) {
				LanguageV2 lang = new LanguageV2 ();
				Hashtable content = (Hashtable)entry.Value;
				lang.ParseLanguage (content);
				languages.Add ((string)entry.Key, lang);
			}
		}
	
	}

	[Serializable]
	public class RateReviewConfig {
		public bool Enable;
		public string ReviewURL;
		public string ContactMail;
		public int TimesToShow;
		public int TimesShowNext;
		public LanguagesV2 Language = new LanguagesV2();
	}

	[Serializable]
	public class PromoteGameConfig {
		public bool Enable;
		public int TimesToShow;
		public int TimesShowNext;
	}

	[Serializable]
	public class AdsBaseConfig {
		public enum AdsType
		{
			Admob,
			UnityAds,
			FacebookAds
		}

		public AdsType Type;
	}

	[Serializable]
	public class AdmobConfig : AdsBaseConfig {
		public string BannerId;
		public string IntersitialId;
	}

	[Serializable]
	public class UnityAdsConfig : AdsBaseConfig {
		public string AdsId;
		public string RewardedKey;
		public string SkippedKey;
	}

	[Serializable]
	public class FBAdsConfig : AdsBaseConfig {
		public string BannerId;
		public string IntersitialId;
		public string NativeId;
	}

	[Serializable]
	public class AdsConfig {
		public Dictionary<string, string> AdsType;
		public Dictionary<string, AdsBaseConfig> Services;
		public int DisplayFrequencyOfInterstitial;
	}
}
