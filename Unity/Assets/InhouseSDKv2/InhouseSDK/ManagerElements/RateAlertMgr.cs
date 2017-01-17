using UnityEngine;
using System.Collections;

public class RateAlertMgr : BaseMgr {
	public RateAlertMgr() {
	}

	public override void InitWithConfig (Hashtable data) {
		base.InitWithConfig (data);
		if (data == null)
			return;
		data = (Hashtable)data["RateAlert"];
		if (data == null)
			return;
		
		Hashtable languages = (Hashtable)data["Languages"];
		int numDisplay = TypeConvert.ToInt(data["NumberToDisplayRatingPopup"]);
		string url = HDUtils.ParseString (data["URL"]);

		if (url == string.Empty)
			return;

		InhouseSDK.Languages outLang = new InhouseSDK.Languages();
		foreach (DictionaryEntry entry in languages) {
			InhouseSDK.Language lang = new InhouseSDK.Language ();
			Hashtable content = (Hashtable)entry.Value;
			lang.Cancel = HDUtils.ParseString (content ["Cancel"]);
			lang.Key = HDUtils.ParseString (entry.Key);
			lang.Message = HDUtils.ParseString (content ["Message"]);
			lang.OK = HDUtils.ParseString (content ["OK"]);
			lang.Title = HDUtils.ParseString (content ["Title"]);
			outLang.addLanguage (lang);
		}

		_config.RateAlert.Languages = outLang;
		_config.RateAlert.NumberToDisplayRatingPopup = numDisplay;
		_config.RateAlert.URL = url;
		Debug.Log ("Init rate ok");
		_didInit = true;
	}

	public override void OnConfigLoadComplete() {
		if (!_didInit)
			return;

		NotificationCenter.getInstance().addEvent(InhouseConstant.GAME_ACTIVE, _OnBecomeActive);

		_didComplete = true;
	}

	public override void OnNewConfig() {
		base.OnNewConfig ();
		if (!_didInit)
			return;
		PlayerPrefs.SetInt ("rated", 0);
		PlayerPrefs.SetInt ("count_rate", -1);
	}

	void _OnBecomeActive(string eventName, object data) {
		ShowRateAlert ();
	}

	public bool DidRate() {
		return PlayerPrefs.GetInt ("rated", 0) == 1;
	}

	public void ShowRateAlert() {
		if (_didInit) {
			int count_rate = PlayerPrefs.GetInt ("count_rate", -1);
			int rated = PlayerPrefs.GetInt ("rated", 0);

			if (count_rate == -1) {
				count_rate = 1;
				PlayerPrefs.SetInt ("count_rate", count_rate);
			}
			Debug.Log ("Rated = " + rated);
			Debug.Log ("Count rate: " + count_rate);
			Debug.Log ("Config rate: " + _config.RateAlert.NumberToDisplayRatingPopup);
			if (count_rate >= _config.RateAlert.NumberToDisplayRatingPopup && rated == 0) {

				Debug.Log ("Languate A: " + (_config.RateAlert.Languages != null).ToString());
				InhouseSDK.Language content = _config.RateAlert.Languages.getLanguage (InhouseSDK.getInstance ().GetCurrentSystemLanguage ());
				Debug.Log ("Languate B");
				if (content != null) {
					string title = content.Title;
					string message = content.Message;
					string ok = content.OK;
					string cancel = content.Cancel;
					InhouseSDK.getInstance().ShowPopup (title, message, ok, cancel, _config.RateAlert.URL, RateCallback);
				}
			} else
				PlayerPrefs.SetInt ("count_rate", count_rate + 1);
			PlayerPrefs.Save ();
			Debug.Log ("Rate done");
		}
	}

	public void ShowRateAlertPopup() {
		if (_didInit) {
			int rated = PlayerPrefs.GetInt ("rated", 0);
			if (rated == 0) {
				InhouseSDK.Language content = _config.RateAlert.Languages.getLanguage (InhouseSDK.getInstance ().GetCurrentSystemLanguage ());
				if (content == null)
					content = _config.RateAlert.Languages.getLanguage ("en");
				if (content != null) {
					string title = content.Title;
					string message = content.Message;
					string ok = content.OK;
					string cancel = content.Cancel;
					InhouseSDK.getInstance ().ShowPopup (title, message, ok, cancel, _config.RateAlert.URL, RateCallback);
				}
			}
		}
	}

	private void RateCallback(string message) {
		Debug.Log (message);
		if (message.Equals ("0")) {
			Application.OpenURL (_config.RateAlert.URL);
			Debug.Log ("Set rated = 1");
			PlayerPrefs.SetInt ("rated", 1);
			PlayerPrefs.Save ();
		}
	}

	public void GoToRate() {
		if (!_didInit)
			return;
		PlayerPrefs.SetInt ("rated", 1);
		Application.OpenURL (_config.AppReviewURL);
	}
	public void GoToLikeFacebook() {
//		if (!_didInit)
//			return;
		Application.OpenURL (_config.FacebookURL);
		//

	}
}
