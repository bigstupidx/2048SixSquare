using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GamePlayMrg : MonoBehaviour {

	public GameObject _PanelSetting;
	private SoundManager _soundMrg;
	public GameObject _btnSound;

	void Awake() {
		_soundMrg = FindObjectOfType<SoundManager> ();
	}

	public void ShowSetting(){
		_soundMrg.btnSound ();
		_PanelSetting.SetActive (true);
		Time.timeScale = 0;
	}	
	public void Share(){
		_soundMrg.btnSound ();
//		InhouseSDK.getInstance ().ShareGame();
	}

	public void Close(){
		_PanelSetting.SetActive (false);
		Time.timeScale = 1;
	}
	public void Home(){
		_soundMrg.btnSound ();
		SceneManager.LoadScene ("MenuUI");
	}
	public void Restart(){
		_soundMrg.btnSound ();
		PlayerPrefs.SetInt ("Score" + PlayerPrefs.GetInt ("Level"), 0);
		for (int i = 0; i < 5; i++) {
				for (int j = Mathf.Abs(2-i); j < 9-Mathf.Abs(2-i); j+=2) {
						PlayerPrefs.SetInt (i + "." + j + PlayerPrefs.GetInt ("Level"), 0);
				}
		}
		Close ();
		PlayerPrefs.SetInt ("Win" + PlayerPrefs.GetInt ("Level"), 0);
		Application.LoadLevel (Application.loadedLevel);
	}

	public void OnOffMusic()
	{
		if (SoundConfig.IsOnSound()) {
			SoundConfig.TurnOffSound ();
		} else {
			SoundConfig.TurnOnSound ();
			_soundMrg.btnSound ();
		}

		UpdateUiForMusicButton ();
	}

	private void UpdateUiForMusicButton(){
		if (SoundConfig.IsOnSound()) {
			
			_btnSound.GetComponent<UISprite>().spriteName = "sound";
		} else {
			_btnSound.GetComponent<UISprite>().spriteName = "sound-off";
		}
	}
}
