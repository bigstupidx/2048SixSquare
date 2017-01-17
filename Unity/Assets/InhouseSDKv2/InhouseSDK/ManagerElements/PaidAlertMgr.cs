using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PaidAlertMgr : BaseMgr {

	public PaidAlertMgr() {
	}

	public override void InitWithConfig (Hashtable data) {
		base.InitWithConfig (data);
		if (data == null)
			return;
		data = (Hashtable)data ["PaidAlert"];
		if (data == null)
			return;
		
		bool enable = (bool)data["Enable"];
		Hashtable languages = (Hashtable)data["Languages"];
		int second = TypeConvert.ToInt(data["Second"]);
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

		_config.PaidAlert.Enable = enable;
		_config.PaidAlert.Languages = outLang;
		_config.PaidAlert.Second = second;
		_config.PaidAlert.URL = url;

		HDDebug.Log ("PaidAlertMgr: initWithConfig success");
		_didInit = true;
	}

	public override void OnConfigLoadComplete() {
		if (!_didInit)
			return;
//		if (!InhouseSDK.getInstance ().IsInGame) {
			ShowPaidAlert ();
//		}
		_didComplete = true;
	}

	public override void OnNewConfig() {
		base.OnNewConfig ();
		if (!_didInit)
			return;
		PlayerPrefs.SetInt ("paid", 0);
	}

	public void ShowPaidAlert() {
		if (_didInit) {
			int rated = PlayerPrefs.GetInt ("paid", 0);
			if (_config.PaidAlert.Enable && rated == 0) {
				InhouseSDK.Language content = _config.PaidAlert.Languages.getLanguage (InhouseSDK.getInstance ().GetCurrentSystemLanguage ());
				if (content != null) {
					float delayTime = _config.PaidAlert.Second;
					string title = (string)content.Title;
					string message = (string)content.Message;
					string ok = (string)content.OK;
					string cancel = (string)content.Cancel;

					InhouseSDK.getInstance().StartCoroutine(ShowMessageWithDelay(delayTime, title, message, ok, cancel, _config.PaidAlert.URL, PaidCall));
				}
			}
		}
	}

	IEnumerator ShowMessageWithDelay(float delay, string title, string message, string ok, string cancel, string urk, InhouseSDK.InhouseCallback callback) {
		yield return new WaitForSeconds (delay);
		InhouseSDK.getInstance().ShowPopup (title, message, ok, cancel, urk, callback);
	}

	private void PaidCall(string message) { // ) mean cancel, 1 mean ok
		Debug.Log (message);
		if (message == "0") {
			Application.OpenURL (_config.PaidAlert.URL);
			PlayerPrefs.SetInt ("paid", 1);
		}
	}
}
