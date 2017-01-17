using UnityEngine;
using System.Collections;
using System;

public class RemoveAdsMgr : BaseMgr {
	

	public RemoveAdsMgr() {
	}

	public override void InitWithConfig (Hashtable data) {
		base.InitWithConfig (data);
		if (data == null)
			return;
		data = (Hashtable)data["RemoveAds"];
		if (data == null)
			return;

		Hashtable languages = (Hashtable)data["Languages"];
		string productId = HDUtils.ParseString (data["ProductID"]);
		string price = HDUtils.ParseString (data["Price"]);

		if (productId == string.Empty)
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

		_config.RemoveAds.Languages = outLang;
		_config.RemoveAds.ProductID = productId;
		_config.RemoveAds.Price = price;

		_didInit = true;
	}

	public override void OnConfigLoadComplete() {
		if (!_didInit)
			return;
		_didComplete = true;
	}

	public void RemoveAds(Action<bool> callback) {
		if (_didInit && !InhouseSDK.getInstance().getIsPro()) {
			_callback = callback;
			InhouseSDK.getInstance ().BuyProduct (_config.RemoveAds.ProductID, OnRemoveAdsSuccess, OnRemoveAdsFaild);
		}
	}

	private void OnRemoveAdsSuccess(string message) {
		_config.IsBuyPro = true;
		InhouseSDK.getInstance ().SaveConfigToLocal ();

		InhouseSDK.getInstance ().GetManager<AdsMgr>().HideBanner ();
		if (_callback != null) {
			_callback (true);
			_callback = null;
		}

	}



	private void OnRemoveAdsFaild(string message) {
		if (_callback != null) {
			_callback (false);
			_callback = null;
		}
	}

	private Action<bool> _callback;

	public void checkAndShowRemoveAd(Action<bool> callback = null) {
		if (_didInit) {
			if (!InhouseSDK.getInstance().getIsPro()) {
				InhouseSDK.Language content = _config.RemoveAds.Languages.getLanguage (InhouseSDK.getInstance ().GetCurrentSystemLanguage ());
				if (content == null)
					content = _config.RemoveAds.Languages.getLanguage ("en");
				if (content != null) {
					string title = content.Title;
					string message = content.Message;
					string ok = content.OK;
					string cancel = content.Cancel;
					_callback = callback;
					InhouseSDK.getInstance().ShowPopup (title, message, ok, cancel, "", (string respone) => {
						if (respone == "0") {
							RemoveAds(callback);
						}
					});
				}
			}
		}
	}
	private void OnRemoveAdCallback(string respone) {
//		if (respone == "0") {
//			RemoveAds ();
//		}
	}

	public void RestoreAds(Action<bool> callback = null) {
		if (_didInit && !InhouseSDK.getInstance().getIsPro()) {
			_callback = callback;
			InhouseSDK.getInstance ().RestoreProduct (_config.RemoveAds.ProductID, OnRestoreSuccess, OnRemoveAdsFaild);
		}
	}
	private void OnRestoreSuccess(string message) {
		_config.IsBuyPro = true;
		InhouseSDK.getInstance ().SaveConfigToLocal ();

		InhouseSDK.getInstance ().GetManager<AdsMgr>().HideBanner ();
		InhouseSDK.getInstance ().ShowPopup ("","Restore purchase successfully","OK","","",_ClosePopup);
		if (_callback != null) {
			_callback (true);
			_callback = null;
		}

	}
	void _ClosePopup (string respone)
	{
		Debug.Log ("Dong popup");
	}
	public string getPrice() {
		return _config.RemoveAds.Price;
	}
}
