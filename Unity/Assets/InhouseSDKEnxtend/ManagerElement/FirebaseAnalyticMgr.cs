using UnityEngine;
using System.Collections;
using Firebase.Analytics;


public class FirebaseAnalyticMgr : BaseMgr {
	public const string FB_EVENT_LAUNCH = "FB_EVENT_LAUNCH";

	public override void InitWithConfig (Hashtable data) {
		base.InitWithConfig (data);
//		if (data == null)
//			return;
//
		_didInit = true;
	}

	public override void OnConfigLoadComplete() {
		if (!_didInit)
			return;

		// Log event login
		FirebaseAnalytics.LogEvent (FB_EVENT_LAUNCH);

		_didComplete = true;
	}

	public void LogEvent(string eventName) {
		FirebaseAnalytics.LogEvent (eventName);
	}
}
