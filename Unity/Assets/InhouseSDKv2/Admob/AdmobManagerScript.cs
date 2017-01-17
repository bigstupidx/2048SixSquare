using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;


public class AdmobManagerScript : MonoBehaviour {
	public string admobInterstitialAds;
	public string admobBannerAds;
	public AdPosition admobBannerPos;
	public bool showBanner = true;

	// Use this for initialization
	InterstitialAd interstitial;
	BannerView bannerView;
	void Start () {
		if (showBanner)
			AdmobManager.getInstance ().ShowBanner (admobBannerAds, admobBannerPos);
	}
}
