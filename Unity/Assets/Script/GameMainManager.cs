using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameMainManager : MonoBehaviour {

	public UILabel _bestscore;
	public GameObject _PanelSetting;
	public UIButton _btnRemoveAds;
	public UIButton _btnRestore;
	public GameObject _btnSound;

	private SoundManager _soundMrg;
	public void PlayGame(){
		_soundMrg.btnPlaySound ();
		PlayerPrefs.SetInt ("Level", 1);
		SceneManager.LoadScene ("GamePlay");
	}

	void Awake() {
		_soundMrg = FindObjectOfType<SoundManager> ();
//		string urlPlist = "https://www.dropbox.com/s/h01v3s1kdgep8pr/config.plist?dl=1";
		InhouseSDK.getInstance ().Initialize ("https://www.dropbox.com/s/hu9lslj6lbz9len/config.plist?dl=1", true, typeof(AdsMgr), typeof(GameCenterMgr));
	}

	void Start(){
		_bestscore.text = PlayerPrefs.GetInt ("Best" + 1).ToString();
		hideBtn ();
	}

	public void ShowLB(){
		_soundMrg.btnSound ();
//		InhouseSDK.getInstance ().GetManager<GameCenterMgr> ().ShowLeaderBoard();
	}

	public void ShowSetting(){
		_soundMrg.btnSound ();
		_PanelSetting.SetActive (true);
	}	
	public void Share(){
		_soundMrg.btnSound ();
//		InhouseSDK.getInstance ().ShareGame();
	}
	public void Rate(){
		_soundMrg.btnSound ();
//		InhouseSDK.getInstance ().GetManager<RateAlertMgr> ().GoToRate ();
	}
	public void Close(){
		_soundMrg.btnSound ();
		_PanelSetting.SetActive (false);
	}
	public void RemoveAds(){
		_soundMrg.btnSound ();
//		InhouseSDK.getInstance ().GetManager<RemoveAdsMgr> ().RemoveAds((bool obj) =>{
//			if(obj){
//				hideBtn();
//			}
//		} );
	}
	public void RestorePurchase(){
		_soundMrg.btnSound ();
//		InhouseSDK.getInstance ().GetManager<RemoveAdsMgr> ().RestoreAds((bool obj) =>{
//			if(obj){
//				hideBtn();
//			}
//		} );
	}
	void hideBtn(){
		_btnRemoveAds.isEnabled = false;
		_btnRestore.isEnabled = false;
	}

	//Music BG
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
