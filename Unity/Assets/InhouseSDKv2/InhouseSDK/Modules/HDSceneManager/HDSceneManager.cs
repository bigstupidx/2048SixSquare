using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class HDSceneManager : MonoBehaviour {

	private static HDSceneManager _instance;
	public static HDSceneManager getInstance() {
		if (_instance == null) {
			_instance = new GameObject ().AddComponent<HDSceneManager> ();
			_instance._init ();
			DontDestroyOnLoad (_instance);
		}
		return _instance;
	}

	bool _nextSceneLoaded;
	string _nextScene;
	LoadingScript _topPanel;
	const float _timeAnimation = 0.3f;
	float _timeSimulation = 0.0f;
	Hashtable _sceneParams = new Hashtable();

	void _init() {
		_topPanel = ((GameObject)Instantiate(Resources.Load ("TopCanvas"))).GetComponent<LoadingScript>();
		_topPanel.gameObject.SetActive (false);
		DontDestroyOnLoad (_topPanel);


		SceneManager.activeSceneChanged += (Scene arg0, Scene arg1) => {
			Debug.Log ("HDSceneManager: ActiveSceneChanged");
			_nextSceneLoaded = true;
		};
		SceneManager.sceneLoaded += (Scene arg0, LoadSceneMode arg1) => {
			Debug.Log ("HDSceneManager: ScenLoaded");
		};
	}

	/// <summary>
	///	Loads the scene with fake waiting time
	/// </summary>
	/// <param name="sceneName">Scene name.</param>
	/// <param name="waitingTime">Waiting time.</param>
	public void LoadScene(string sceneName, float waitingTime = 0.0f) {
		Debug.Log ("HDSceneManager: Start LoadScene");
		// check scene exist
		_nextSceneLoaded = false;
		_nextScene = sceneName;
		_timeSimulation = waitingTime;
		StartCoroutine (_doLoadScene ());
	}

	/// <summary>
	/// Loads the scene with Loading scene
	/// </summary>
	/// <param name="sceneName">Scene name.</param>
	/// <param name="waitingTimeA">Waiting time a.</param>
	/// <param name="waitingTimeB">Waiting time b.</param>
	public void LoadScene(string sceneName, float waitingTimeA, float waitingTimeB) {
		float time = Random.Range (waitingTimeA, waitingTimeB);
		LoadScene (sceneName, time);
	}

	IEnumerator _doLoadScene() {
		float timeCheck = Time.time;
		float timeAnimation = _timeAnimation;
		_topPanel.gameObject.SetActive (true);
		while (true) { // animation show
			float time = Time.time - timeCheck;
			_topPanel.setTranparent (Mathf.Clamp01(time / timeAnimation));
			if (time > timeAnimation) {
				break;
			}
			yield return null;
		}
		SceneManager.LoadSceneAsync (_nextScene);
		while (!_nextSceneLoaded) { // wait unitl nextscene loaded
			yield return null;
		}
		timeCheck = Time.time;
		while (Time.time - timeCheck > _timeSimulation) {
			yield return null;
		}
		timeCheck = Time.time;
		while (true) { // animation hide
			float time = Time.time - timeCheck;
			_topPanel.setTranparent (1 - Mathf.Clamp01(time / timeAnimation));
			if (Time.time - timeCheck > timeAnimation) {
				break;
			}
			yield return null;
		}
		_nextScene = null;
		_nextSceneLoaded = false;
		_timeSimulation = 0;
		_topPanel.gameObject.SetActive (false);
	}

	public void LoadSceneImmedialy(string name, bool isAsync = false) {
		if (isAsync)
			SceneManager.LoadSceneAsync (name);
		else
			SceneManager.LoadScene (name);
	}

	public Hashtable getSceneParams() {
		return _sceneParams;
	}

	public object getSceneParams(string key) {
		if (_sceneParams.ContainsKey (key))
			return _sceneParams [key];
		return null;
	}

	public void setSceneParams(string key, object data) {
		_sceneParams.Add (key, data);
	}

	public void clearSceneParams() {
		_sceneParams.Clear ();
	}
}
