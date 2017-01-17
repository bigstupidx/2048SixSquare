using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class NewVersionMgr : BaseMgr {

	public NewVersionMgr() {
	}


	public override void InitWithConfig (Hashtable data) {
		base.InitWithConfig (data);
//		if (data == null)
//			return;
//
		// alway complete
		_didInit = true;
	}

	public override void OnConfigLoadComplete() {
		if (!_didInit)
			return;
//		if (!InhouseSDK.getInstance ().IsInGame) {
			_checkNewVersion ();
//		}
		_didComplete = true;
	}

	public void _checkNewVersion() {
		checkNewVersion ();
	}
	#if UNITY_IPHONE && !UNITY_EDITOR_OSX
	[DllImport ("__Internal")]
	private static extern void checkNewVersion ();
	#endif
	#if UNITY_EDITOR_OSX
	private static void checkNewVersion() {
		Debug.Log ("NewVersionMgr: check new version");
	}
	#endif
}
