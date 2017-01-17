using System;
using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;

public class MiscellaneousMgr : BaseMgr

{
	public MiscellaneousMgr ()
	{
	}

	public override void InitWithConfig (Hashtable data) {
		base.InitWithConfig (data);



		_didInit = true;
	}

	public override void OnConfigLoadComplete() {
		if (!_didInit)
			return;
//		Texture2D image = getImageShare (99);
//		Debug.Log ("CHAUNG: " + image);
//		SpriteRenderer render = new GameObject ().AddComponent<SpriteRenderer> ();
//		render.sprite = Sprite.Create (image, new Rect (0, 0, image.width, image.height), new Vector2 (0.5f, 0.5f));
//		render.transform.position = new Vector3 (0, 0, 0);
//
//		InhouseSDK.getInstance ().ShareGameWithImage (image);
		_didComplete = true;
	}

	public Texture2D getImageShare(int score) {
		string base64Data = __getImageShare (score);
		byte[] newBytes = Convert.FromBase64String(base64Data);
		Texture2D result = new Texture2D(300, 300);
		result.LoadImage(newBytes);
		return result;
	}

	#if UNITY_IPHONE && !UNITY_EDITOR_OSX
	[DllImport ("__Internal")]
	private static extern string __getImageShare (int score);
	#endif
	#if UNITY_EDITOR_OSX
	private static string __getImageShare(int score) {
		Debug.Log ("MiscellaneousMgr: get image share");
		return string.Empty;
	}
	#endif
}

