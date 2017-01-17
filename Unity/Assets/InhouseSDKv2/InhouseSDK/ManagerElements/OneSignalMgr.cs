#if !ONESIGNAL_DISABLE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OneSignalMgr : BaseMgr {

	public OneSignalMgr() {
	}

	public override void InitWithConfig (Hashtable data) {
		base.InitWithConfig (data);

		if (data == null)
			return;
		data = (Hashtable)data["OneSignal"];
		if (data == null)
			return;

		_config.OneSignal.Enable = (bool)data["Enable"];
		_config.OneSignal.IOSAppID = HDUtils.ParseString (data ["IOSKey"]);
		_config.OneSignal.AndroidAppID = HDUtils.ParseString (data["AndroidKey"]);

		Debug.Log ("Init OneSignal complete");
		_didInit = true;
	}

	public override void OnConfigLoadComplete() {
		if (!_didInit)
			return;

		if (_config.OneSignal.Enable)
			OneSignal.Init (_config.OneSignal.IOSAppID != "" ? _config.OneSignal.IOSAppID : null,
				_config.OneSignal.AndroidAppID != "" ? _config.OneSignal.AndroidAppID : null, HandleNotificationReceived);
		_didComplete = true;
	}

	void HandleNotificationReceived (string message, Dictionary<string, object> additionalData, bool isActive)
	{
		Debug.Log ("OneSignal: HandleNotificationReceived");
		Debug.Log ("OneSignal: Additional data: " + additionalData.ToString());
	}
				
}

#endif