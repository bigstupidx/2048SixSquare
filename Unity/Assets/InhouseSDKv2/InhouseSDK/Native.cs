using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class Native {
	#if UNITY_IPHONE && !UNITY_EDITOR_OSX
	[DllImport ("__Internal")]
	public static extern void buyProduct (string productID);
	[DllImport ("__Internal")]
	public static extern void restorePurchaseInApps(string productID);
	[DllImport ("__Internal")]
	public static extern string CurrentLanguage();
	[DllImport ("__Internal")]
	public static extern void showAlert(string ident, string title, string message, string cancel, string ok, string url);
	[DllImport ("__Internal")]
	public static extern void showAlert2(string ident, string title, string message, string choose1, string choose2, string cancel, string url);
	[DllImport ("__Internal")]
	public static extern string NLINKFILE ();
	[DllImport ("__Internal")]
	public static extern string GetDefaultPlist ();
	[DllImport ("__Internal")]
//	public static extern void shareGame (string url);
//	[DllImport ("__Internal")]
//	public static extern void shareGameWithImage (string url, string imageString);
//	[DllImport ("__Internal")]
//	public static extern void shareGameWithGif (string url, string path);
//	[DllImport ("__Internal")]
//	public static extern void shareScoreWithImage (int type, int score, string mode, string text);
//	[DllImport ("__Internal")]
	public static extern int __getDeviceType ();

	#endif

	#if UNITY_EDITOR_OSX
	public static void buyProduct (string productID) {
		Debug.Log ("Buy Product " + productID);
	}
	public static void restorePurchaseInApps(string productID) {
		Debug.Log ("Restore Product " + productID);
	}
	public static string CurrentLanguage() {
		return "en";
	}
	public static void showAlert(string ident, string title, string message, string cancel, string ok, string url) {
		Debug.Log ("Show MessageBox in Unityeditor");
	}
	public static void showAlert2(string ident, string title, string message, string choose1, string choose2, string cancel, string url) {
		Debug.Log ("Show MessageBox in Unityeditor");
	}
	public static string NLINKFILE () {
		return InhouseSDK.getInstance().PLIST_ONLINE_URL;
	}

	public static  string GetDefaultPlist (){
		Debug.Log ("Get defaut plist");
		return null;
	}
//	public static void shareGame (string url) {
//		Debug.Log ("Share game: "+ url);
//	}

//	public static void shareGameWithImage (string url, string imageString) {
//		Debug.Log ("Share game with imageString: "+ url + " imageString: " + imageString != "");
//	}
//
//	public static void shareGameWithGif (string text, string url) {
//		Debug.Log ("Share game with gif in Editor");
//	}
//
//	public static void shareScoreWithImage (int type, int score, string mode, string text) {
//		Debug.Log ("Share game with imageString: "+ type + "|" + score + "|" + mode);
//	}

	public static int __getDeviceType () {
		return 0;
	}
	#endif
}
