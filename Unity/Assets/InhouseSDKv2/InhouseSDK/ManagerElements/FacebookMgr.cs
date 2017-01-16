
#if !FACEBOOK_DISABLE

using UnityEngine;
using System;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

public class FacebookMgr : BaseMgr
{
	bool _didLogin = false;
	AccessToken _accessToken;
	IDictionary<string, object> _loginData;

	public FacebookMgr() {
	}
	public override void InitWithConfig (Hashtable data) {
		base.InitWithConfig (data);

		if (data == null)
			return;
		data = (Hashtable)data["Facebook"];
		if (data == null)
			return;

//		_config.Facebook.FacebookLink = HDUtils.ParseString (data["LinkApp"]);

		_didInit = true;
	}

	public override void OnConfigLoadComplete() {
		if (!_didInit)
			return;
		Debug.Log("FacebookMgr: Init");
		#if !UNITY_EDITOR
		FB.Init (() => {
			Debug.Log("FacebookMgr: Init success");
//			_lazyLogin(null);
		}, (bool isUnityShown) => {
			Debug.Log("FacebookMgr: Hide Unity");
		});
		#endif
		_didComplete = true;
	}

	void _lazyLogin(Action<bool> job) {
		if (!FB.IsLoggedIn) {
			FB.LogInWithPublishPermissions (new string[] { "publish_actions" }, (ILoginResult result) => {
				if (result.AccessToken != null) {
					_accessToken = result.AccessToken;
					_loginData = result.ResultDictionary;

					if (_accessToken != null)
					{
						Debug.Log("FacebookMgr: Login success - " + _accessToken);
						if (job != null) {
							Debug.Log("FacebookMgr: call");
							job (true);
						}
					} else {
						Debug.Log("FacebookMgr: Login false - " + result.Error);
						if (job != null)
							job(false);
					}
				}
			});
		} else {
			if (job != null)
				job (true);
		}
	}

	public void TestVideo() {
		string path = __getGifPath();
		Debug.Log("Gif path: " + path);
		Debug.Log("File exist: " + File.Exists (path).ToString());
//		path = "/Users/giangchau92/Desktop/haha.gif";
//		InhouseSDK.getInstance().executeEnumrator(_doUpload(path));
	}

	public void UpdateGifAndShare(Action<IShareResult> callback) {
		_lazyLogin ((bool obj) => {
			if (!obj)
				return;
			string path = __getGifPath();
			Debug.Log("Gif path: " + path);
			Debug.Log("File exist: " + File.Exists (path).ToString());

			InhouseSDK.getInstance().executeEnumrator(_doUpload(path, (string id) => {
				string link = new System.Text.StringBuilder().AppendFormat("https://media.giphy.com/media/{0}/giphy.gif", id).ToString();
				Debug.Log("Facebok link: " + link);
				FB.ShareLink(new Uri(link), "Tank Online", "Let play", new Uri(link), (IShareResult result) => {
					Debug.Log("Share result: " + result.ToString());
					if (callback != null) {
						if (result.Error != null)
							callback(result);
						else
							callback(null);
					}
				});
			}));
		});

	}

	IEnumerator _doUpload(string path, Action<string> callback) {
		if (!File.Exists (path))
			yield break;
		byte[] data = File.ReadAllBytes(path);
		WWWForm upload = new WWWForm();
		upload.AddField("username", "yourusername");
		upload.AddBinaryData ("file", data);
		upload.AddField("api_key", "dc6zaTOxFJmzC");
		upload.AddField("tags", "tag1,tag2,tag3");
		upload.AddField("source_post_url", "http://your.source-url.tld");

		WWW www = new WWW("http://upload.giphy.com/v1/gifs", upload);
		yield return www;

		Debug.Log ("Response: " + www.text);
		string temp = www.text;
		string[] array = temp.Split(new string[] {"\"id\":\"","\"}"}, StringSplitOptions.None);
		temp = array[1];
		if (temp != "" && callback != null)
			callback (temp);
	}

	public void ShareGifLink(Action<IShareResult> callback) {
		_lazyLogin ((bool obj) => {
			if (!obj)
				return;
			string link;
			if(InhouseSDK.getInstance().IsConfigLoaded()){
				link = "http://giphy.com/gifs/tank-online-71KCD6dDFFV60";//link = InhouseSDK.getInstance().getLinkShareFacebook();
			} else {
				link = "http://giphy.com/gifs/tank-online-71KCD6dDFFV60";
			}
			FB.ShareLink(new Uri(link), "Tank Online", "Let play", new Uri(link), (IShareResult result) => {
				Debug.Log("Share result: === " + result.ToString());
				if (result.Cancelled || !String.IsNullOrEmpty(result.Error)) {
					Debug.Log("ShareLink Error: "+result.Error);
					callback(null);
				} else if (!String.IsNullOrEmpty(result.PostId)) {
					// Print post identifier of the shared content
					Debug.Log(result.PostId);
					callback(result);
				} else {
					// Share succeeded without postID
					Debug.Log("ShareLink success!");
					callback(result);
				}
			});
		});

	}


	public void InviteFriend(Action<IAppInviteResult> callback) {
		_lazyLogin ((bool logged) => {
			if (!logged)
				return;
			
			Debug.Log("Facebook: Invite");//
			InhouseSDK.getInstance().executeEnumrator(_doInvite(callback));

		});
	}

	public IEnumerator _doInvite(Action<IAppInviteResult> callback) {
		yield return new WaitForSeconds(1.0f);
//		HDDebug.Log ("fb link: "+_config.Facebook.FacebookLink);
//		FB.Mobile.AppInvite(new System.Uri(_config.Facebook.FacebookLink), new System.Uri("https://lh3.googleusercontent.com/eBvlc0ajeNc9OMjYTth5IIl5serJ50n_jmKqP5CYBO6siT5I-_5daE0VwZIJYFusejU=w300-rw"), (IAppInviteResult result) => {
//			if (result.Cancelled || !String.IsNullOrEmpty(result.Error)) {
//				Debug.Log("ShareLink Error: "+result.Error);
//				callback(null);
//			} else {
//				// Share succeeded without postID
//				Debug.Log("ShareLink success!");
//				callback(result);
//			}
//		});
	}


	#if UNITY_IPHONE && !UNITY_EDITOR_OSX
	[DllImport ("__Internal")]
	public static extern string __getGifPath ();
	#endif

	#if UNITY_EDITOR_OSX
	public static string __getGifPath () {
		Debug.Log ("__getGifPath call");
		return "";
	}
	#endif
}

#endif