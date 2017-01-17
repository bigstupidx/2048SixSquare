using UnityEngine;
using System.Collections;
//using Firebase.Messaging;
//
//public class FirebaseRMessageMgr : BaseMgr {
//	public FirebaseRMessageMgr() {
//		FirebaseMessaging.TokenReceived += OnTokenReceived;
//		FirebaseMessaging.MessageReceived += OnMessageReceived;
//	}
//
//	public override void InitWithConfig (Hashtable data) {
//		base.InitWithConfig (data);
//		if (data == null)
//			return;
//		data = (Hashtable)data["GameCenter"];
//		if (data == null)
//			return;
//
//		_didInit = true;
//	}
//
//	public override void OnConfigLoadComplete() {
//		if (!_didInit)
//			return;
//
//		_didComplete = true;
//	}
//
//	public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
//		UnityEngine.Debug.Log("FirebaseRemoteMessage: Received Registration Token: " + token.Token);
//	}
//
//	public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
//		UnityEngine.Debug.Log("FirebaseRemoteMessage: Received a new message from: " + e.Message.RawData);
//	}
//}
