using UnityEngine;
using System.Collections;
using System.IO;

public class PopupManager : MonoBehaviour {
	public Texture popupTexture = null;
	public Texture closeTexture = null;
	public Texture openTexture = null;
	
	private float sidelength = 0f;
	private float adw = 0f;
	private float adh = 0f;
	private float btnw = 0f;
	private float btnh = 0f;
	
	private bool _isshowing = false;
	private string _url;

	private static PopupManager _sharedManager = null;
	public static PopupManager SharedManager(){
		if (_sharedManager == null) {
			_sharedManager = new GameObject ().AddComponent<PopupManager> ();
			DontDestroyOnLoad(_sharedManager.gameObject);
		}
		return _sharedManager;
	}

	public void OpenPopup(Texture texture){
		Debug.Log ("OPen popup adservice");
		_isshowing = true;

		popupTexture = texture;
		openTexture = Resources.Load <Sprite>("btnOke").texture;
		closeTexture = Resources.Load <Sprite>("btnClose").texture;
		
		sidelength = Mathf.Min(Screen.width, Screen.height);

		adw = 0;
		adh = 0;
		btnw = sidelength * 0.1f;
		btnh = sidelength * 0.1f;

		Object[] objs = GameObject.FindObjectsOfType (typeof(GameObject));
		BoxCollider targetMono = null;
		foreach (GameObject obj in objs) {
			targetMono = obj.GetComponent<BoxCollider>();
			if(targetMono != null)
				targetMono.enabled = false;
		}
	}

	public void LoadAndOpenPopup(string imageUrl, string url){
		_url = url;
		StartCoroutine (LoadAdsImage (imageUrl));
	}


	private IEnumerator LoadAdsImage(string url){
		// load local first
		string localURL = Application.persistentDataPath + "/cacheimage_" + ".png";
		Debug.Log ("LOAD DATA BY WWW");
		WWW localRequest =  new WWW("file://" + localURL);
		yield return localRequest;

		if(localRequest.error != null || localRequest.texture == null){
			Debug.Log ("REQUEST NEW IMAGE " + url);

			WWW request = new WWW (url);
			yield return request;
			if (request.error == null && request.texture != null) {
				byte[] bytes = request.texture.EncodeToPNG();
				File.WriteAllBytes(localURL, bytes);
				popupTexture = request.texture;
				OpenPopup (popupTexture);
			} else {
				Debug.Log ("Failure to load image" + request.error);
			}
		}
		else {
			Debug.Log("LOAD TEXTURE FROM CACHE");
			popupTexture = localRequest.texture;
			OpenPopup (popupTexture);
		}
	}

	public void ClosePopup(){
		_isshowing = false;

		// increase number display
//		#warning need to fix
		InhouseSDK.getInstance().GetManager<AdServiceMgr>().IncreaseNumDisplay();

		Object[] objs = GameObject.FindObjectsOfType (typeof(GameObject));
		BoxCollider targetMono = null;
		foreach (GameObject obj in objs) {
			targetMono = obj.GetComponent<BoxCollider>();
			if(targetMono != null)
				targetMono.enabled = true;
		}
	}
	
	void OnGUI()
	{
		adw = Screen.width;
		adh = Screen.height;
		if (_isshowing) {
			GUI.Window (0,  new Rect (Screen.width / 2 - adw / 2, Screen.height / 2 - adh / 2, adw, adh), 
			            ShowGUI, "",  new GUIStyle ());
		}
	}
	
	void ShowGUI(int windowID)
	{
		GUIStyle testStyle = new GUIStyle();
		
		GUI.DrawTexture (new Rect (0, 0, adw, adh), popupTexture);
		
		GUI.DrawTexture (new Rect(0, 0 + btnh * 1f, btnw, btnh), closeTexture);
		if (GUI.Button(new Rect(0, 0 + btnh * 1f, btnw, btnh), "", testStyle))
		{
			Debug.Log("CLOSE POPUP");
			ClosePopup();
		}
		
		GUI.DrawTexture (new Rect (adw - btnw * 1f, 0 + btnh * 1f, btnw, btnh),openTexture);
		if (GUI.Button(new Rect(adw - btnw * 1f, 0 + btnh * 1f, btnw, btnh), "", testStyle))
		{
			Debug.Log("OPEN URL");
			ClosePopup();
			Application.OpenURL(_url);
		}
		
		if (GUI.Button(new Rect(0, 0, adw, adh), "", testStyle))
		{
			Debug.Log("OPEN URL");
			ClosePopup();
			Application.OpenURL(_url);
		}
		
	}

	void logEvent() {
		FirebaseAnalyticMgr fb = InhouseSDK.getInstance ().GetManager<FirebaseAnalyticMgr> ();
		if (fb != null)
			fb.LogEvent (InhouseSDK.FB_EVENT_ADSERVICE_CLICK);
	}
}
