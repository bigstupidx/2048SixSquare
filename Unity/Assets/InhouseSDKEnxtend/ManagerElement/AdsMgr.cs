using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

#if !UNITY_ADS_DISABLE
using UnityEngine.Advertisements;
#endif

#if !FACEBOOK_ADS_DISABLE
using AudienceNetwork;
#endif


public class AdsMgr : BaseMgr {
	public const string ADMOB_BANNER_LOADED = "ADMOB_BANNER_LOADED";

	public const string TAG_ADMOB 		= "Admob";
	public const string TAG_UNITYADS 	= "UnityAds";
	public const string TAG_FBADS 		= "FBAds";

	public const string TAG_ADSTYPE_BANER 		= "Banner";
	public const string TAG_ADSTYPE_INTERSITIAL = "Intersitial";
	public const string TAG_ADSTYPE_AWARDED 	= "Awarded";
	public const string TAG_ADSTYPE_NATIVE 		= "Native";

	public enum AdBannerPosition
	{
		Top, Bottom
	}

	public static AdBannerPosition BannerPosition = AdBannerPosition.Top;
	public static bool UnityAdsTestMode = false;

	Dictionary<string, BaseAds> _ads = new Dictionary<string, BaseAds> ();

	public override void InitWithConfig (Hashtable data) {
		base.InitWithConfig (data);

		if (data == null)
			return;
		data = (Hashtable)data["Ads"];
		if (data == null)
			return;

		Hashtable dataServices = (Hashtable)data["Services"];
		Hashtable dataAdsType = (Hashtable)data["AdsType"];

		Dictionary<string, string> types = new Dictionary<string, string> ();
		Dictionary<string, InhouseSDK.AdsBaseConfig> services = new Dictionary<string, InhouseSDK.AdsBaseConfig> ();

		foreach (DictionaryEntry item in dataAdsType) {
			types.Add (HDUtils.ParseString (item.Key), HDUtils.ParseString (item.Value));
		}

		{
			InhouseSDK.AdmobConfig admob = new InhouseSDK.AdmobConfig();
			Hashtable dataAdmob = (Hashtable)dataServices ["Admob"];
			admob.BannerId = HDUtils.ParseString (dataAdmob["BannerId"]);
			admob.IntersitialId = HDUtils.ParseString (dataAdmob["IntersitialId"]);
			services.Add (TAG_ADMOB, admob);
		}

		{
			InhouseSDK.UnityAdsConfig unityAd = new InhouseSDK.UnityAdsConfig();
			Hashtable dataAdmob = (Hashtable)dataServices ["UnityAds"];
			unityAd.AdsId = HDUtils.ParseString (dataAdmob["AdsId"]);
			unityAd.RewardedKey = HDUtils.ParseString (dataAdmob["RewardedKey"]);
			unityAd.SkippedKey = HDUtils.ParseString (dataAdmob["SkippedKey"]);
			services.Add (TAG_UNITYADS, unityAd);
		}

		{
			InhouseSDK.FBAdsConfig fbAd = new InhouseSDK.FBAdsConfig();
			Hashtable dataAdmob = (Hashtable)dataServices ["FBAds"];
			fbAd.BannerId = HDUtils.ParseString (dataAdmob["BannerId"]);
			fbAd.IntersitialId = HDUtils.ParseString (dataAdmob["IntersitialId"]);
			fbAd.NativeId = HDUtils.ParseString (dataAdmob["NativeId"]);
			services.Add (TAG_FBADS, fbAd);
		}

		_config.Ads.DisplayFrequencyOfInterstitial = TypeConvert.ToInt(data["DisplayFrequencyOfInterstitial"]);
		_config.Ads.Services = services;
		_config.Ads.AdsType = types;

		_didInit = true;
	}

	public override void OnConfigLoadComplete() {
		if (!_didInit)
			return;
		{
			InhouseSDK.AdmobConfig admobConfig = (InhouseSDK.AdmobConfig)_config.Ads.Services[TAG_ADMOB];
			if (admobConfig == null)
				throw new UnityException ("AdsMgr: invalid config");
			AdmobAds ads = new AdmobAds (this, admobConfig);
			ads.Init ();
			_ads.Add (TAG_ADMOB, ads);
		}
		{
			InhouseSDK.UnityAdsConfig unityConfig = (InhouseSDK.UnityAdsConfig)_config.Ads.Services[TAG_UNITYADS];
			if (unityConfig == null)
				throw new UnityException ("AdsMgr: invalid config");
			UnityAds ads = new UnityAds (this, unityConfig);
			ads.Init ();
			_ads.Add (TAG_UNITYADS, ads);
		}
		{
			InhouseSDK.FBAdsConfig fbAdsConfig = (InhouseSDK.FBAdsConfig)_config.Ads.Services [TAG_FBADS];
			if (fbAdsConfig == null)
				throw new UnityException ("AdsMgr: invalid config");
			FacebookAds ads = new FacebookAds (this, fbAdsConfig);
			ads.Init ();
			_ads.Add (TAG_FBADS, ads);
		}

		ShowBanner ();

		_didComplete = true;
	}

	public void ShowBanner() {
		if (_didInit && !InhouseSDK.getInstance ().getIsPro ()) {
			foreach (KeyValuePair<string, string> item in _config.Ads.AdsType) {
				if (item.Key == TAG_ADSTYPE_BANER) {
					BaseAds ads = _ads[item.Value];
					if (ads != null)
						ads.ShowBanner ();
					else
						HDDebug.Log ("AdsMgr: " + item.Key + " not supported");
				}
			}
		}
	}

	public void HideBanner() {
		if (_didInit && !InhouseSDK.getInstance ().getIsPro ()) {
			foreach (KeyValuePair<string, string> item in _config.Ads.AdsType) {
				if (item.Key == TAG_ADSTYPE_BANER) {
					BaseAds ads = _ads [item.Value];
					if (ads != null)
						ads.HideBanner ();
					else
						HDDebug.Log ("AdsMgr: " + item.Key + " not supported");
				}
			}
		}
	}

	public bool IsBannerShowing() {
		if (_didInit && !InhouseSDK.getInstance ().getIsPro ()) {
			foreach (KeyValuePair<string, string> item in _config.Ads.AdsType) {
				if (item.Key == TAG_ADSTYPE_BANER) {
					BaseAds ads = _ads [item.Value];
					if (ads != null)
						return ads.IsBannerShowing ();
					else
						return false;
				}
			}
		}
		return false;
	}

	public void CheckAndShowIntersitial(bool forceShow = false) {
		HDDebug.Log ("ShowIntersitial: " + (_config.Ads.AdsType != null).ToString());
		if (_didInit && !InhouseSDK.getInstance ().getIsPro ()) {
			int ratio = UnityEngine.Random.Range (0, 100);
			if (forceShow || ratio < _config.Ads.DisplayFrequencyOfInterstitial) {
				foreach (KeyValuePair<string, string> item in _config.Ads.AdsType) {
					if (item.Key == TAG_ADSTYPE_INTERSITIAL) {
						BaseAds ads = _ads [item.Value];
						if (ads != null)
							ads.ShowIntersitial ();
						else
							HDDebug.Log ("AdsMgr: " + item.Key + " not supported");
					}
				}
			}
		}
	}

	public bool IsVideoAwardedAvaible() {
		HDDebug.Log ("Check ShowVideoAwarded avaible");
		if (_didInit) {
			foreach (KeyValuePair<string, string> item in _config.Ads.AdsType) {
				if (item.Key == TAG_ADSTYPE_AWARDED) {
					BaseAds ads = _ads [item.Value];
					if (ads != null)
						return ads.IsVideoAwardAvaible ();
					return false;
				}
			}
		}
		return false;
	}

	public void ShowVideoAwarded(Action<bool> callback) {
		HDDebug.Log ("ShowVideoAwarded");
		if (_didInit) {
			foreach (KeyValuePair<string, string> item in _config.Ads.AdsType) {
				if (item.Key == TAG_ADSTYPE_AWARDED) {
					BaseAds ads = _ads [item.Value];
					if (ads != null)
						ads.ShowVideoAward (callback);
					else
						HDDebug.Log ("AdsMgr: " + item.Key + " not supported");
				}
			}
		}
	}

	public FacebookAds GetFBAdService() {
		return (FacebookAds)_ads[TAG_FBADS];
	}

	public class BaseAds {
		protected AdsMgr _manager;

		public BaseAds(AdsMgr manager) {
			_manager = manager;
		}

		public virtual void Init() {
		}

		public virtual void ShowBanner() {
		}

		public virtual void HideBanner() {
		}

		public virtual bool IsBannerShowing() {
			return false;
		}

		public virtual void ShowIntersitial() {
		}

		public virtual bool IsVideoAwardAvaible() {
			return false;
		}

		public virtual void ShowVideoAward(Action<bool> callback) {
		}
	}

	public class AdmobAds : BaseAds {
		InhouseSDK.AdmobConfig _config;
		public AdmobAds(AdsMgr manager, InhouseSDK.AdmobConfig config) : base(manager) {
			_config = config;
		}

		public override void Init() {
			if (_manager._config.Ads.AdsType[TAG_ADSTYPE_INTERSITIAL] == TAG_ADMOB)
				AdmobManager.getInstance ().RequestInterstitial (_config.IntersitialId);
		}

		public override void ShowBanner() {
			if (!_manager._didInit || InhouseSDK.getInstance().getIsPro())
				return;
			AdmobManager.getInstance ().ShowBanner (_config.BannerId,
				AdsMgr.BannerPosition == AdBannerPosition.Bottom ? GoogleMobileAds.Api.AdPosition.Bottom : GoogleMobileAds.Api.AdPosition.Top);
		}

		public override void HideBanner() {
			if (!_manager._didInit || InhouseSDK.getInstance().getIsPro())
				return;
			AdmobManager.getInstance ().HideBanner ();
		}

		public override bool IsBannerShowing ()
		{
			#if UNITY_EDITOR
			return false;
			#endif
			return AdmobManager.getInstance ().IsBannerShowing;
		}

		public override void ShowIntersitial ()
		{
			Debug.Log ("AdmobMgr: ShowIntersitial1");
			if (_manager._didInit && !InhouseSDK.getInstance ().getIsPro ()) {
				Debug.Log ("AdmobMgr: ShowIntersitial2");
				AdmobManager.getInstance ().ShowInterstitial (_config.IntersitialId, true);
			}
		}

		public override void ShowVideoAward (Action<bool> callback)
		{
			HDDebug.Log ("AdmobAds: Not support Video Rewarded");
		}
	}

	public class UnityAds : BaseAds {
		InhouseSDK.UnityAdsConfig _config;
		public UnityAds(AdsMgr manager, InhouseSDK.UnityAdsConfig config) : base(manager) {
			_config = config;
		}

		public override void Init() {
			Debug.Log ("Init UnityAds: " + _config.AdsId);
			#if !UNITY_EDITOR
			Advertisement.Initialize (_config.AdsId, AdsMgr.UnityAdsTestMode);
			#endif
		}

		public override void ShowBanner() {
			HDDebug.Log ("UnityAds: not support Banner Ads");
		}

		public override void HideBanner() {
			HDDebug.Log ("UnityAds: not support Banner Ads");
		}

		public override void ShowIntersitial ()
		{
			if (_manager._didInit && !InhouseSDK.getInstance ().getIsPro ()) {
				Advertisement.Show (_config.SkippedKey);
			}
		}

		public override bool IsVideoAwardAvaible ()
		{
			return Advertisement.IsReady (_config.RewardedKey);
		}

		public override void ShowVideoAward (Action<bool> callback)
		{
			if (_manager._didInit) {
				if (!Advertisement.IsReady (_config.RewardedKey)) {
					HDDebug.Log ("AdmobMgr: " + _config.RewardedKey + " not ready");
					if (callback != null)
						callback (false);
					return;
				}
				Advertisement.Show (_config.RewardedKey, new ShowOptions { resultCallback = (ShowResult obj) => {
						switch (obj) {
						case ShowResult.Finished:
							if (callback != null)
								callback (true);
							break;
						}
					}
				});
			}
		}
	}

	public class FacebookAds : BaseAds {
		InhouseSDK.FBAdsConfig _config;

		AdView _adBanner;
		InterstitialAd _adIntersitial;
		private bool isLoaded = false;
		bool _isBannerShowing = false;

		public FacebookAds(AdsMgr manager, InhouseSDK.FBAdsConfig config) : base(manager) {
			_config = config;
		}

		public override void Init() {
			Debug.Log ("Init Facebook Ads");
			if (_manager._config.Ads.AdsType[TAG_ADSTYPE_INTERSITIAL] == TAG_FBADS)
				LoadInterstitial ();
		}

		public override void ShowBanner() {
			if (_manager._didInit && !InhouseSDK.getInstance ().getIsPro ()) {

				AdView adView = new AdView (_config.BannerId, AdSize.BANNER_HEIGHT_50);
				this._adBanner = adView;
				this._adBanner.Register (InhouseSDK.getInstance ().gameObject);

				// Set delegates to get notified on changes or when the user interacts with the ad.
				this._adBanner.AdViewDidLoad = (delegate() {
					Debug.Log ("Ad view loaded.");
					double y = 0;
					double height = AudienceNetwork.Utility.AdUtility.convert (Screen.height);
					y = (AdsMgr.BannerPosition == AdsMgr.AdBannerPosition.Bottom) ? height - 50 : 0;
					this._adBanner.Show (y);
					_isBannerShowing = true;
				});
				adView.AdViewDidFailWithError = (delegate(string error) {
					Debug.Log ("Ad view failed to load with error: " + error);
				});
				adView.AdViewWillLogImpression = (delegate() {
					Debug.Log ("Ad view logged impression.");
				});
				adView.AdViewDidClick = (delegate() {
					Debug.Log ("Ad view clicked.");
				});

				// Initiate a request to load an ad.
				adView.LoadAd ();
			}
		}

		public override void HideBanner() {
			if (this._adBanner) {
				this._adBanner.Dispose ();
				_isBannerShowing = false;
				Debug.Log ("Ad view close.");
			}
		}

		public override bool IsBannerShowing ()
		{
			return _isBannerShowing;
		}

		public void LoadInterstitial (bool forceShow = false) {
			if (_adIntersitial != null)
				return;
			InterstitialAd interstitialAd = new InterstitialAd (_config.IntersitialId);
			this._adIntersitial = interstitialAd;
			this._adIntersitial.Register (InhouseSDK.getInstance().gameObject);

			// Set delegates to get notified on changes or when the user interacts with the ad.
			this._adIntersitial.InterstitialAdDidLoad = (delegate() {
				Debug.Log("Interstitial ad loaded.");
				this.isLoaded = true;
				if (forceShow)
					_adIntersitial.Show();
			});
			_adIntersitial.InterstitialAdDidFailWithError = (delegate(string error) {
				Debug.Log("Interstitial ad failed to load with error: " + error);
			});
			_adIntersitial.InterstitialAdWillLogImpression = (delegate() {
				Debug.Log("Interstitial ad logged impression.");
			});
			_adIntersitial.InterstitialAdDidClick = (delegate() {
				Debug.Log("Interstitial ad clicked.");
			});
			_adIntersitial.InterstitialAdDidClose = (delegate() {
				Debug.Log("Interstitial ad close.");
				if (_adIntersitial != null)
					_adIntersitial.Dispose();
				_adIntersitial = null;
				Debug.Log("Interstitial reload ads.");
				LoadInterstitial();
			});

			// Initiate the request to load the ad.
			this._adIntersitial.LoadAd ();
		}

		public override void ShowIntersitial ()
		{
			if (_manager._didInit && !InhouseSDK.getInstance ().getIsPro ()) {
				Debug.Log ("Loaded: " + isLoaded.ToString());
				if (this.isLoaded) {
					this._adIntersitial.Show ();
				} else {
					HDDebug.Log ("FBAds intersitial not loaded");
					if (_adIntersitial == null)
						LoadInterstitial (true);
				}
			}
		}

		public override void ShowVideoAward (Action<bool> callback)
		{
			HDDebug.Log ("Facebook ads: not support VideosReward Ads");
		}

		/// 
		/// NATIVE ADS
		/// 
		/// 
		public void GetNativeAds (GameObject root, Button[] actionButtons, Action<NativeAd> complete)
		{
			HDDebug.Log ("FBAds: Request native ads: " + _config.NativeId);
			NativeAd nativeAd = new AudienceNetwork.NativeAd (_config.NativeId);

			nativeAd.RegisterGameObjectForImpression (root, actionButtons);

			// Set delegates to get notified on changes or when the user interacts with the ad.
			nativeAd.NativeAdDidLoad = (delegate() {
				Debug.Log("FBNative ad loaded.");
				if (complete != null)
					complete(nativeAd);
			});
			nativeAd.NativeAdDidFailWithError = (delegate(string error) {
				Debug.Log("Native ad failed to load with error: " + error);
				if (complete != null)
					complete(null);
			});
			nativeAd. NativeAdWillLogImpression = (delegate() {
				Debug.Log("Native ad logged impression.");
			});
			nativeAd.NativeAdDidClick = (delegate() {
				Debug.Log("Native ad clicked.");
			});
			// Initiate a request to load an ad.
			nativeAd.LoadAd ();
		}
	}

}
