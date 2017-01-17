using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.IO;

public partial class InhouseSDK : MonoBehaviour {
	private bool PLIST_DEFAULT_ENABLE = false;
	public string PLIST_ONLINE_URL = "";

	public delegate void InhouseCallback(string respone);
	public delegate void InhouseCompleteDelegate();

	public InhouseCompleteDelegate InhouseReadyCallback;
	public bool IsInGame = false;

	// Use this for initialization

	private static InhouseSDK _instance = null;
	public static InhouseSDK getInstance(){

		if (_instance == null) {
			_instance = new GameObject ().AddComponent<InhouseSDK> ();
			_instance.gameObject.name = "InhouseSDKManager";
			DontDestroyOnLoad (_instance.gameObject);
			Application.SetStackTraceLogType (LogType.Log, StackTraceLogType.None);
		}
		return _instance;
	}

	private ConfigData _config;

	private Dictionary<Type, BaseMgr> _modules = new Dictionary<Type, BaseMgr>();

	public bool IsConfigLoaded() {
		return _config != null;
	}

	public bool getIsPro() {
		return _config.IsProVersion || _config.IsBuyPro;
	}

	public void Initialize(string plistUrl, bool useDefault, params Type[] modules) {
		if (IsConfigLoaded ())
			return;

		PLIST_ONLINE_URL = plistUrl;
		PLIST_DEFAULT_ENABLE = useDefault;

		foreach (var item in modules) {
			if (!item.IsSubclassOf (typeof(BaseMgr))) {
				HDDebug.Log ("InhouseSDK: " + item.ToString () + "Module type invaid. It must be base of BaseMgr");
				continue;
			}
			BaseMgr manager = (BaseMgr)Activator.CreateInstance (item);
			_modules.Add (item, manager);
		}
		HDDebug.Log ("InhouseSDK: Initialize manager complete");
		HDDebug.Log ("InhouseSDK: Config loading");
		LoadDataConfig ();
	}

	public void LoadDataConfig() {
		// Load from local
		bool haveLocalConfig = LoadDataFromLocal();
		if (_config == null)
			_config = new ConfigData ();
		//
		foreach (var item in _modules) {
			item.Value.SetConfig (_config, haveLocalConfig);
		}
		// plist default
		if (!haveLocalConfig && PLIST_DEFAULT_ENABLE) {
			string plist_offline = Native.GetDefaultPlist();
			if (plist_offline != null) {
				Hashtable table = new Hashtable ();
				PListParser.ParsePListFileFromContent (plist_offline, ref table);
				Debug.Log ("Plist offline: " + plist_offline);
				ParseNewData (table);
			}
		}
		// OnReady with default or local plist
		#if UNITY_EDITOR_OSX
		if (haveLocalConfig || PLIST_DEFAULT_ENABLE)
			OnSDKReady();
		#endif
		#if !UNITY_EDITOR_OSX // good job, who are reading my code
		if (haveLocalConfig || PLIST_DEFAULT_ENABLE)
			OnSDKReady();
		#endif
		// get new plist
		StartCoroutine(LoadDataConfig_WWW ());
	}

	#region PARSE CONFIG
	object _checkLoi(object value, object defaultValue, string name) {
		if (value != null)
			return value;
		else {
			Debug.Log ("Plist invalid: " + name + "|" + Environment.StackTrace);
			return defaultValue;
		}
	}

	public void ParseNewData(Hashtable data) {
		_config.Version = (int)_checkLoi (data ["Version"], 0, "Version");
//		_config.AppShareText = (string)_checkLoi (data ["AppShareText"], "", "AppShareText");
		_config.AppURL = (string)_checkLoi (data ["AppURL"], "", "AppURL");
		_config.AppReviewURL = (string)_checkLoi (data ["AppReviewURL"], "", "AppReviewURL");
//		_config.ContactMailAddress = (string)_checkLoi (data ["ContactMailAddress"], "", "ContactMailAddress");
//		_config.ContactMailSubject = (string)_checkLoi (data ["ContactMailSubject"], "", "ContactMailSubject");
		_config.IsProVersion = (bool)_checkLoi (data ["IsProVersion"], false, "IsProVersion");
//		_config.FacebookURL = (string)_checkLoi (data ["FacebookURL"], "", "FacebookURL");


		HDDebug.Log("Module count: " + _modules.Count.ToString());
		foreach (var item in _modules) {
			try {
				HDDebug.Log ("InhouseSDK: Init with config " + item.Key.ToString ());
				item.Value.InitWithConfig (data);
			} catch (Exception ex) {
				ShowPopup ("Error initWithConfig", ex.ToString (), "Fix lien di", "", "", null);
			}
		}
	}

	private IEnumerator LoadDataConfig_WWW(){
		Debug.Log ("LOAD Config BY WWW link: " + Native.NLINKFILE());

		WWW request = new WWW (Native.NLINKFILE());
		yield return request;
		if (request.error == null && request.text != "") {
			Debug.Log ("Plist online: " + request.text);
			// Parse data
			string text_data = request.text;
			ParseData (text_data);
		} else {
			Debug.Log ("WWW Error: " + request.error);
		}
	}

	private void ParseData(string data) {
		Hashtable table = new Hashtable ();
		if (PListParser.ParsePListFileFromContent (data, ref table)) {
			int version = (int)table["Version"];
			if (version > _config.Version) {
				ParseNewData (table);
				SaveConfigToLocal ();
				OnSDKNewConfig ();
			}
		}
	}

	private void OnSDKReady() {
		if (_config.Version == -1)
			return;
		Debug.Log ("Inhouse SDK Ready");
		try {

			foreach (var item in _modules) {
				HDDebug.Log ("InhouseSDK: " + item.Key.ToString() + " onConfigLoadComplete");
				item.Value.OnConfigLoadComplete ();

			}

			if (this.InhouseReadyCallback != null)
				this.InhouseReadyCallback ();
			NotificationCenter.getInstance ().dispatchEvent (InhouseConstant.INHOUSE_READY, null);
		} catch (Exception ex) {
			ShowPopup ("Error OnSDKReady", ex.ToString (), "Fix lien di", "", "", null);
		}
		Debug.Log("Device Type: " + getDeviceType ().ToString());
	}

	private void OnSDKNewConfig() {
		if (_config.Version == -1)
			return;
		try {
			foreach (var item in _modules) {
				HDDebug.Log ("InhouseSDK: " + item.Key.ToString() + " onNewConfig");
				item.Value.OnNewConfig ();
			}
		} catch (Exception ex) {
			ShowPopup ("Error OnSDKNewConfig", ex.ToString (), "Fix lien di", "", "", null);
		}
	}

	public void SaveConfigToLocal(){
		Debug.Log ("SAVE CONFIG");
		byte[] data = ObjectToByteArray(_config);
		string base64 = Convert.ToBase64String (data);
		PlayerPrefs.SetString ("CONFIG_KEY", base64);
		PlayerPrefs.Save ();

	}
	private bool LoadDataFromLocal() {
		Debug.Log ("LOAD DATA");
		string base64 = PlayerPrefs.GetString ("CONFIG_KEY", string.Empty);
		if (base64 != string.Empty) {
			byte[] data = Convert.FromBase64String (base64);
			_config = (InhouseSDK.ConfigData) ByteArrayToObject (data);
		} else
			_config = null;
		return _config != null;
	}
	#endregion

	T _getManager<T>() where T : BaseMgr {
		if (!_modules.ContainsKey (typeof(T)))
//			_modules.Add (typeof(T), (BaseMgr) Activator.CreateInstance (typeof(T)));
			return null;
		return (T)_modules [typeof(T)];
	}

	public T GetManager<T>() where T : BaseMgr {
		return _getManager<T> ();
	}

	#region SHARE FUNCTION
	private InhouseCallback _buyProductSuccess = null;
	private InhouseCallback _buyProductFail = null;
	public void BuyProduct(string productId, InhouseCallback success, InhouseCallback failure) {
		_buyProductSuccess = success;
		_buyProductFail = failure;
		Native.buyProduct (productId);
	}
	public void BuyProductCallback(string message){
		Debug.Log ("BUY PRODUCT CALLBACK: " + message);
		if (message.Equals ("ok") && _buyProductSuccess != null)
			_buyProductSuccess (message);
		if (message.Equals ("failed") && _buyProductFail != null)
			_buyProductFail (message);
	}

	private InhouseCallback _restoreProductSuccess = null;
	private InhouseCallback _restoreProductFail = null;
	public void RestoreProduct(string productId, InhouseCallback success, InhouseCallback failure) {
		_restoreProductSuccess = success;
		_restoreProductFail = failure;
		Native.restorePurchaseInApps (productId);
	}

	public void RestorePurchaseInAppsCallback(string message){
		Debug.Log ("RESTORE PURCHASE IN APP CALLBACK: " + message);
		if (message.Equals ("ok") && _restoreProductSuccess != null)
			_restoreProductSuccess (message);
		if (message.Equals ("failed") && _restoreProductFail != null)
			_restoreProductFail (message);
	}

	public void ShareCallback(string data) {
		NotificationCenter.getInstance ().dispatchEvent (InhouseConstant.GAME_SHARE_COMPLETE, data == "true" ? true : false);
	}

	public string GetCurrentSystemLanguage() {
		return Native.CurrentLanguage ();
	}

	private Dictionary<string, InhouseCallback> _popupRateCallback = new Dictionary<string, InhouseCallback> ();
	public void ShowPopup(string title, string message, string ok, string cancel, string url, InhouseCallback callback){
		Guid guid = Guid.NewGuid ();
		string ident = guid.ToString ();
		if (callback != null)
			_popupRateCallback.Add (ident, callback);
		Native.showAlert (ident, title, message, ok, cancel, url);
	}

	public void ShowPopup3(string title, string message, string ok1, string ok2, string cancel, string url, InhouseCallback callback){
		Guid guid = Guid.NewGuid ();
		string ident = guid.ToString ();
		if (callback != null)
			_popupRateCallback.Add (ident, callback);
		Native.showAlert2 (ident, title, message, ok1, ok2, cancel, url);
	}

	public void RatePopupCallback(string message){
		Debug.Log ("InhouseSDK: Rate popup callback");
		string[] list = message.Split(new char[]{'|'});
		if (list.Length < 2)
			return;
		string data = list [0];
		string ident = list [1];

		if (_popupRateCallback.ContainsKey (ident)) {
			_popupRateCallback [ident] (data);
		}
	}

	public void BecomeActiveCallback(string message) {
		NotificationCenter.getInstance ().dispatchEvent (InhouseConstant.GAME_ACTIVE, null);
		// load mới plist và init Inhouse
	}

	public void BecomeDeactiveCallback(string message) {
		NotificationCenter.getInstance ().dispatchEvent (InhouseConstant.GAME_DEACTIVE, null);
	}

//	public void ShareGame ()
//	{
//		Native.shareGame ( _config.AppShareText + "\n" +_config.AppURL);
//	}
//
//	public void ShareContent(string content) {
//		Native.shareGame (content);
//	}
//
	public void Feedback ()
	{
		string email = _config.ContactMailAddress;
		string subject = WWW.EscapeURL (_config.ContactMailSubject).Replace ("+", "%20");
		string body = "";
		Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
	}

//	public void ShareGameWithImage(Texture2D texture) {
//		byte[] imageData = texture.EncodeToPNG ();
//		string dataString = Convert.ToBase64String (imageData);
//		Native.shareGameWithImage (_config.AppShareText + "\n" + _config.AppURL, dataString);
//	}

//	public void ShareGameWithGif(string gifPath) {
//		Native.shareGameWithGif (_config.AppShareText + "\n" + _config.AppURL + "\n", gifPath);
//	}
//
//	public void ShareGameScore(int type, int score, string mode) {
//		Native.shareScoreWithImage (type, score, mode, _config.AppShareText + "\n" + _config.AppURL + "\n");
//	}

	#endregion

	#region UTILS

	IEnumerator CheckConnection(Action<bool> callback)
	{
		const float timeout = 5.0f;
		float startTime = Time.timeSinceLevelLoad;
		var ping = new Ping("8.8.8.8");

		while (true)
		{
			if (ping.isDone)
			{
				callback(true);
				yield break;
			}
			if (Time.timeSinceLevelLoad - startTime > timeout)
			{
				callback(false);
				yield break;
			}

			yield return new WaitForEndOfFrame();
		}
	}

	public void CheckNetwork(Action<bool> callback) {
		StartCoroutine (CheckConnection((bool obj) => {
			if (callback != null)
				callback (obj);
		}));
	}

//	void Awake() {
//		if (Application.platform == RuntimePlatform.IPhonePlayer)
//		{
//			System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
//		} 
//	}

	public DeviceType getDeviceType() {
		int deviceType = Native.__getDeviceType ();
		if (deviceType == 0)
			return DeviceType.IPHONE_4_LESS;
		else if (deviceType == 1)
			return DeviceType.IPHONE_5;
		else if (deviceType == 2)
			return DeviceType.IPHONE_6;
		else
			return DeviceType.IPHONE_6P;
	}

	public void executeEnumrator(IEnumerator job) {
		StartCoroutine (_doExcute(job));
	}

	IEnumerator _doExcute(IEnumerator job) {
		yield return job;
	}

	#endregion


	#region CLASS DATA
	[Serializable]
	public class Language {
		public string Key;
		public string Cancel;
		public string Message;
		public string OK;
		public string Title;
	}

	[Serializable]
	public class Languages {
		public List<Language> languages = new List<Language>();
		/*
		 * Get current language, if key not exist, get first language
		 */
		public Language getLanguage(string key) {
			Language result = null;
			foreach (var item in languages) {
				if (item.Key == key)
					return item;
				else if (item.Key == "en")
					result = item;
			}
			if (result != null)
				return result;
			return languages[0];
		}


		public void addLanguage(Language language) {
			languages.Add (language);
		}
	}

	[Serializable]
	public class CharacterDescription {
		public string Name;
		public int Price;
	}

	[Serializable]
	public class AdService {
		public string URL;
		public int NumberToDisplay;
		public string ImageURL;
		public bool Enable;
	}

	[Serializable]
	public class GoogleAds {
		public string BannerAdUnitID;
		public bool UsingAdModInterstitial;
		public string InterstitialAdUnitID;
		public int DisplayFrequencyOfInterstitial;
		public string UnityRewardID;
		public string UnityRewardKey;
		public string UnitySkipKey;

	}

	#if !UNITY_DISABLE
	[Serializable]
	public class UnityAd {
		public string UnityRewardID;
	}
	#endif

	[Serializable]
	public class PaidAlert {
		public bool Enable;
		public float Second;
		public string URL;
		public Languages Languages;
	}

	[Serializable]
	public class RateAlert {
		public int NumberToDisplayRatingPopup;
		public string URL;
		public Languages Languages;
	}

	[Serializable]
	public class RemoveAds {
		public string ProductID;
		public string Price;
		public Languages Languages;
	}

	[Serializable]
	public class GameCenter {
		public Dictionary<string, string> LeaderboardIds;
	}

	[Serializable]
	public class OneSignalD {
		public bool Enable;
		public string IOSAppID;
		public string AndroidAppID;
	}

	[Serializable]
	public partial class ConfigData {
		public int Version = -1;
		public string AppShareText;
		public string AppURL;
		public string AppReviewURL;
		public string ContactMailAddress;
		public string ContactMailSubject;
		public bool IsProVersion;
		public bool IsBuyPro;
		public string FacebookURL;

		public AdService AdService = new AdService ();
		public GoogleAds GoogleAds = new GoogleAds ();
		public PaidAlert PaidAlert = new PaidAlert ();
		public RateAlert RateAlert = new RateAlert ();
		public RemoveAds RemoveAds = new RemoveAds ();
		public GameCenter GameCenter = new GameCenter ();
		public OneSignalD OneSignal = new OneSignalD();

		public ConfigData() {
		}

	}
	#endregion

	public enum DeviceType
	{
		IPHONE_4_LESS,
		IPHONE_5,
		IPHONE_6,
		IPHONE_6P
	}

			#region UTILS
			public static Dictionary<K,V> HashtableToDictionary<K,V> (Hashtable table)
			{
				return table
					.Cast<DictionaryEntry> ()
					.ToDictionary (kvp => (K)kvp.Key, kvp => (V)kvp.Value);
			}

			public static byte[] ObjectToByteArray(System.Object obj)
			{
				BinaryFormatter bf = new BinaryFormatter();
				using (var ms = new MemoryStream())
				{
					bf.Serialize(ms, obj);
					return ms.ToArray();
				}
			}

	public static System.Object ByteArrayToObject(byte[] arrBytes)
	{
		using (var memStream = new MemoryStream())
		{
			var binForm = new BinaryFormatter();
			memStream.Write(arrBytes, 0, arrBytes.Length);
			memStream.Seek(0, SeekOrigin.Begin);
			var obj = binForm.Deserialize(memStream);
			return obj;
		}
	}
			#endregion
}
