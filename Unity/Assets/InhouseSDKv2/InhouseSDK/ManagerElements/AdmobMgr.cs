using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using System;

#if UNITY_EDITOR
using UnityEditor.Advertisements;
using UnityEditor.Callbacks;
using UnityEditor;

#endif

public class AdmobMgr : BaseMgr {
	public const string ADMOB_BANNER_LOADED = "ADMOB_BANNER_LOADED";

	public static GoogleMobileAds.Api.AdPosition BannerPosition = GoogleMobileAds.Api.AdPosition.Bottom;

	public AdmobMgr() {
		NotificationCenter.getInstance ().addEvent (InhouseConstant.GAME_ACTIVE, OnBecomeActive);
	}


	public override void InitWithConfig (Hashtable data) {
		base.InitWithConfig (data);

		if (data == null)
			return;
		data = (Hashtable)data["Google_Ads"];
		if (data == null)
			return;
		
		string bannerId = HDUtils.ParseString (data ["BannerAdUnitID"]);
		int frequency = TypeConvert.ToInt(data["DisplayFrequencyOfInterstitial"]);
		string intersitialId = HDUtils.ParseString (data["InterstitialAdUnitID"]);

		string UnityRewardID = HDUtils.ParseString (data ["UnityRewardID"]);
		string UnityRewardKey = HDUtils.ParseString (data["UnityRewardKey"]);
		string UnitySkipKey = HDUtils.ParseString (data["UnitySkipKey"]);
		bool usingGoogleIntertisial = (bool)data ["UsingAdmod"];

		if (bannerId == string.Empty || intersitialId == string.Empty)
			return;

		_config.GoogleAds.BannerAdUnitID = bannerId;
		_config.GoogleAds.DisplayFrequencyOfInterstitial = frequency;
		_config.GoogleAds.InterstitialAdUnitID = intersitialId;
		_config.GoogleAds.UsingAdModInterstitial = usingGoogleIntertisial;

		_config.GoogleAds.UnityRewardID = UnityRewardID;
		_config.GoogleAds.UnityRewardKey = UnityRewardKey;
		_config.GoogleAds.UnitySkipKey = UnitySkipKey;

		_didInit = true;
	}

	public override void OnConfigLoadComplete() {
		if (!_didInit)
			return;
		Debug.Log ("AdmobMgr onConfigLoadComplete");
		CheckAndShowBannerIfNeed ();
		checkAndRequestIntersitial ();


		Debug.Log ("Init UnityAds: " + _config.GoogleAds.UnityRewardID);
		#if !UNITY_EDITOR
		Advertisement.Initialize (_config.GoogleAds.UnityRewardID, false);
		#endif

		_didComplete = true;
	}

	public void CheckAndShowBannerIfNeed(){
		if (!InhouseSDK.getInstance ().IsInGame) {
			showBannerAd ();
		}
	}

	public void ReloadBanner(){
		Debug.Log ("Is in game: " + InhouseSDK.getInstance ().IsInGame.ToString());
		if (InhouseSDK.getInstance ().IsInGame) {
			if (_didInit && !InhouseSDK.getInstance ().getIsPro ()) {
				Debug.Log ("Show banner");
				AdmobManager.getInstance ().ReloadBanner (_config.GoogleAds.BannerAdUnitID, BannerPosition);
			} else
				Debug.Log ("Can not show banner");
		}
	}

	public void showBannerAd() {
		if (_didInit && !InhouseSDK.getInstance ().getIsPro ()) {
			AdmobManager.getInstance ().ShowBanner (_config.GoogleAds.BannerAdUnitID, BannerPosition);
			Debug.Log ("Show banner");
		} else
			Debug.Log ("Can not show banner");
	}

	public void hideBannerAd() {
		if (_didInit)
			AdmobManager.getInstance ().HideBanner ();
	}


	public void showIntersitialAd() {
		if (_didInit && !InhouseSDK.getInstance().getIsPro())
			AdmobManager.getInstance ().ShowInterstitial ();
	}

	public void checkAndRequestIntersitial() {
		if (!_config.GoogleAds.UsingAdModInterstitial) {
			return;
		}
		if (_didInit && !InhouseSDK.getInstance ().getIsPro ()) {
			int ratio = UnityEngine.Random.Range (0, 100);
			if (ratio < _config.GoogleAds.DisplayFrequencyOfInterstitial)
				AdmobManager.getInstance ().RequestInterstitial (_config.GoogleAds.InterstitialAdUnitID);
		} else
			Debug.Log ("AdmobMgr: Dont request");
	}

	public void requestIntersitial() {
		if (_didInit && !InhouseSDK.getInstance ().getIsPro () && _config.GoogleAds.UsingAdModInterstitial) {
			int ratio = UnityEngine.Random.Range (0, 100);
			if (ratio < _config.GoogleAds.DisplayFrequencyOfInterstitial) {
				Debug.Log ("AdmobMgr: request intersitial");
				AdmobManager.getInstance ().RequestInterstitial (_config.GoogleAds.InterstitialAdUnitID);
			}
		}
	}

	public void checkAndShowIntersitial() {
		if (!_config.GoogleAds.UsingAdModInterstitial) {
			if (_didInit && !InhouseSDK.getInstance ().getIsPro ()) {
				ShowSkipVideo (null);
			}
			return;
		} else {
			if (_didInit && !InhouseSDK.getInstance ().getIsPro ()) {
				AdmobManager.getInstance ().ShowInterstitial ();
				checkAndRequestIntersitial ();
			}
		}
	}

	public AdmobManager.CallbackHandler rewardCallback;

	public string GetRewardKey(){
		return _config.GoogleAds.UnityRewardKey;
	}

	public void ShowRewardedVideo(Action<bool> callback) {
		if (!Advertisement.IsReady ()) {
			HDDebug.Log ("AdmobMgr: " + _config.GoogleAds.UnitySkipKey + " not ready");
			if (callback != null)
				callback (false);
			return;
		}
		Advertisement.Show (_config.GoogleAds.UnityRewardKey, new ShowOptions { resultCallback = (ShowResult obj) => {
				switch (obj) {
				case ShowResult.Finished:
					if (callback != null)
						callback (true);
					break;
				}
			}
		});
	}

	public void ShowSkipVideo(Action<bool> callback) {
		if (!Advertisement.IsReady ()) {
			HDDebug.Log ("AdmobMgr: " + _config.GoogleAds.UnitySkipKey + " not ready");
			if (callback != null)
				callback (false);
			return;
		}
		int ratio = UnityEngine.Random.Range (0, 100);
		if (ratio < _config.GoogleAds.DisplayFrequencyOfInterstitial) {
			Debug.Log ("Show Skip video");
			Advertisement.Show (_config.GoogleAds.UnitySkipKey, new ShowOptions { resultCallback = (ShowResult obj) => {
					switch (obj) {
					case ShowResult.Finished:
						if (callback != null)
							callback (true);
						break;
					case ShowResult.Skipped:
						if (callback != null)
							callback (true);
						break;
					default:
						if (callback != null)
							callback (false);
						break;
					}
				}
			});
		}
	}

	public bool StateVideoRewardID()
	{
		Debug.Log ("UnityAds: " + _config.GoogleAds.UnityRewardKey + " is ready: " + Advertisement.IsReady (_config.GoogleAds.UnityRewardKey));
//		int ratio = UnityEngine.Random.Range (0, 100);
//		if (ratio < _config.GoogleAds.DisplayFrequencyOfInterstitial) {
//			return Advertisement.IsReady (_config.GoogleAds.UnityRewardKey);
//		} else
//			return false;
		return Advertisement.IsReady (_config.GoogleAds.UnityRewardKey);

	}

	private void OnBecomeActive(string eventName, object data) {
		if (!_didInit)
			return;
		Debug.Log ("AdmobMgr Reset");
//		OnConfigLoadComplete ();
		ReloadBanner ();
		checkAndRequestIntersitial ();


		Debug.Log ("Init UnityAds: " + _config.GoogleAds.UnityRewardID);
		#if !UNITY_EDITOR
		Advertisement.Initialize (_config.GoogleAds.UnityRewardID, false);
		#endif
	}
}
