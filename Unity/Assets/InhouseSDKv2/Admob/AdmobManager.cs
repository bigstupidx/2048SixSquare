using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;

public class AdmobManager {
	InterstitialAd _interstitialAd;
	BannerView _bannerAd;
	RewardBasedVideoAd _rewardBasedVideo;

	public bool IsBannerShowing = false;

	static AdmobManager _instance = null;
	public static AdmobManager getInstance() {
		if (_instance == null)
			_instance = new AdmobManager ();
		return _instance;
	}

	public void ShowBanner(string key, AdPosition pos = AdPosition.Bottom) {
		if (_bannerAd == null) {
			RequestBanner (key, pos);
		} else {
			_bannerAd.Show ();
			IsBannerShowing = true;
		}
	}

	public void HideBanner() {
		if (_bannerAd != null) {
			_bannerAd.Hide ();
			IsBannerShowing = false;
		}
	}

	private void RequestBanner(string key, AdPosition pos)
	{
		string adUnitId = key;

		// Create a 320x50 banner at the top of the screen.
		if (_bannerAd == null) {
			_bannerAd = new BannerView (adUnitId, AdSize.Banner, pos);
			_bannerAd.OnAdLoaded += (object sender, System.EventArgs e) => {
				HDDebug.Log("Banner loaded");
				_bannerAd.Show ();
				IsBannerShowing = true;
				NotificationCenter.getInstance().dispatchEvent(AdsMgr.ADMOB_BANNER_LOADED, null);
			};
		}
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the banner with the request.
		_bannerAd.LoadAd(request);
	}

	public void ReloadBanner(string key, AdPosition pos) {
		string adUnitId = key;

		// Create a 320x50 banner at the top of the screen.
		HDDebug.Log("Reload banner: " + (_bannerAd != null).ToString());
		if (_bannerAd != null)
			_bannerAd.Destroy ();

		_bannerAd = new BannerView (adUnitId, AdSize.Banner, pos);

		_bannerAd.OnAdLoaded += (object sender, System.EventArgs e) => {
			HDDebug.Log("Banner loaded");
			_bannerAd.Show ();
			IsBannerShowing = true;
		};

		_bannerAd.OnAdFailedToLoad += (object sender, AdFailedToLoadEventArgs e) => {
			_bannerAd.Destroy();
			_bannerAd = null;
		};

		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the banner with the request.
		_bannerAd.LoadAd(request);
		HDDebug.Log ("Banner request");
	}

	public bool ShowInterstitial(string key = "", bool forceShow = false) {
		if (_interstitialAd != null && _intersitialLoaded) {
			Debug.Log ("Intersitial SHOW");
			_interstitialAd.Show ();
			return true;
		} else {
			if (forceShow) {
				RequestInterstitial (key, true);
			} else {
				Debug.Log ("Intersitial NOT SHOW: " + _intersitialLoaded.ToString ());
				return false;
			}
		}
		return false;
	}

	private bool _intersitialLoaded = false;
	public void RequestInterstitial(string key, bool showAfterLoaded = false)
	{
		Debug.Log("Intersitial REQUEST ENTER");
		string adUnitId = key;

		// Initialize an InterstitialAd.
		if (_interstitialAd != null) {
			if (_intersitialLoaded == true) {
				Debug.Log ("Intersitial EXIST");
				return;
			}
			_interstitialAd.Destroy ();
			_interstitialAd = null;
		}
		_interstitialAd = new InterstitialAd (adUnitId);
		_interstitialAd.OnAdLoaded += (object sender, System.EventArgs e) => {
			Debug.Log("Intersitial LOADED");
			_intersitialLoaded = true;
			if (showAfterLoaded)
				_interstitialAd.Show();
		};
		_interstitialAd.OnAdClosed += (object sender, System.EventArgs e) => {
			_intersitialLoaded = false;
			RequestInterstitial(key);
		};

		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder ().Build ();
		// Load the interstitial with the request.
		_interstitialAd.LoadAd (request);
		Debug.Log("Intersitial REQUEST");
	}
	public bool isShowReward;
	public delegate void CallbackHandler(bool isReward);

	CallbackHandler _callback;

	public void RequestRewardAds(string key, CallbackHandler callback)
	{
		isShowReward = false;

		string adUnitId = key;

		_rewardBasedVideo = RewardBasedVideoAd.Instance;



		AdRequest request = new AdRequest.Builder ().Build ();
		_rewardBasedVideo.OnAdLoaded += (object sender, System.EventArgs e) => {
			RewardBasedVideoAd testsender = sender as RewardBasedVideoAd;
			testsender.Show();
			//			_rewardBasedVideo.Show ();
			Debug.Log("Show Video");
		};

		_callback = callback;
		_rewardBasedVideo.OnAdClosed += (object sender, System.EventArgs e) => {
			Debug.Log ("OnAdClosed");
			isShowReward = true;
			if(_callback != null){
				_callback(isShowReward);
			}
			else if (_callback == null){
				Debug.Log("_callback = null");
			}
		};

		_rewardBasedVideo.LoadAd(request, adUnitId);

	}
}
