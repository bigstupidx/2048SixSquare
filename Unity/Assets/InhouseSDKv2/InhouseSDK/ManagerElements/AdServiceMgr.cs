using UnityEngine;
using System.Collections;
using System.IO;

public class AdServiceMgr : BaseMgr {
	public static string ADSERVICES_NUM_DISPLAY = "ADSERVICES_NUM_DISPLAY";

	public AdServiceMgr() {
	}

	public override void InitWithConfig (Hashtable data)
	{
		base.InitWithConfig (data);
		if (data == null)
			return;
		data = (Hashtable)data["Ads_service"];
		if (data == null)
			return;
		
		string imageURL = HDUtils.ParseString (data["ImageURL"]);
		int numToDisplat = TypeConvert.ToInt(data["NumberToDisplay"]);
		string url = HDUtils.ParseString (data["URL"]);
		if (imageURL == string.Empty || url == string.Empty)
			return;

		_config.AdService.ImageURL = imageURL;
		_config.AdService.NumberToDisplay = numToDisplat;
		_config.AdService.URL = url;

		_didInit = true;
	}

	public override void OnConfigLoadComplete ()
	{
		if (!_didInit)
			return;
//		if (!InhouseSDK.getInstance ().IsInGame) {
			checkAndShowAdServices ();
//		}
		_didComplete = true;
	}

	public override void OnNewConfig () {
		base.OnNewConfig ();
		if (!_didInit)
			return;
		Debug.Log ("AdService Reset");
		PlayerPrefs.SetInt (ADSERVICES_NUM_DISPLAY, 0);
		PlayerPrefs.Save ();
		string localURL = Application.persistentDataPath + "/cacheimage_" + ".png";
		File.Delete (localURL);
		Debug.Log ("Delete URL: " + localURL);
	}

	public void checkAndShowAdServices() {
		if (!_didInit)
			return;
		int num = PlayerPrefs.GetInt (ADSERVICES_NUM_DISPLAY, 0);
		Debug.Log ("AdService Num: " + num.ToString());
		if (num < _config.AdService.NumberToDisplay) {
			PopupManager.SharedManager ().LoadAndOpenPopup (_config.AdService.ImageURL, _config.AdService.URL);
			Debug.Log ("AdService Open");
		}
	}

	public void showAdServices() {
		if (!_didInit)
			return;
		PopupManager.SharedManager ().LoadAndOpenPopup (_config.AdService.ImageURL, _config.AdService.URL);
	}

	public void IncreaseNumDisplay() {
		if (!_didInit)
			return;
		int num = PlayerPrefs.GetInt (ADSERVICES_NUM_DISPLAY, 0);
		PlayerPrefs.SetInt (ADSERVICES_NUM_DISPLAY, num + 1);
		PlayerPrefs.Save ();

		Debug.Log ("Increase AdsService " + (PlayerPrefs.GetInt (ADSERVICES_NUM_DISPLAY, -1)).ToString());
	}
}
