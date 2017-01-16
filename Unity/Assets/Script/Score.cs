using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
	public GUISkin scoreSkin;
	public float fTimeChangeColor = 0.1f;
	public Text _score;
	public Text _bestScore;
	static public int iScore;
	static public int iBest;
	int[] a = new int[3];
	// Update is called once per frame
	void Start ()
	{
		if (!PlayerPrefs.HasKey ("Win1")) {
				PlayerPrefs.SetInt ("Win1", 0);
		}
		if (!PlayerPrefs.HasKey ("Win2")) {
				PlayerPrefs.SetInt ("Win2", 0);
		}
		if (!PlayerPrefs.HasKey ("Win3")) {
				PlayerPrefs.SetInt ("Win3", 0);
		}

		iScore = PlayerPrefs.GetInt ("Score" + PlayerPrefs.GetInt ("Level"));
		iBest = PlayerPrefs.GetInt ("Best" + PlayerPrefs.GetInt ("Level"));
		_bestScore.text = iBest.ToString ();
		scoreSkin.label.fontSize = Screen.height * 13 / 324;
		a [0] = 0;
		a [1] = 255;
		a [2] = 0;
	}	
	void Update ()
	{
		if (!ktDeath.isDie) {
			scoreSkin.box.fontSize = Screen.height * 30 / 324;
			_score.text = iScore.ToString();
		}
		if (ktDeath.isDie && PlayerPrefs.GetInt ("Win" + PlayerPrefs.GetInt ("Level")) != 1) {
			if (PlayerPrefs.GetInt ("Best" + PlayerPrefs.GetInt ("Level")) < iScore) {
				PlayerPrefs.SetInt ("Best" + PlayerPrefs.GetInt ("Level"), iScore);
			}
			PlayerPrefs.SetInt ("Score" + PlayerPrefs.GetInt ("Level"), 0);
			for (int i = 0; i < 5; i++) {
				for (int j = Mathf.Abs(2-i); j < 9-Mathf.Abs(2-i); j+=2) {
					PlayerPrefs.SetInt (i + "." + j + PlayerPrefs.GetInt ("Level"), 0);
				}
			}
			PlayerPrefs.SetInt ("Win" + PlayerPrefs.GetInt ("Level"), 0);
			reload ();
		}

	}
	public GameObject _GameOverPanl;
	void OnGUI ()
	{
		scoreSkin.label.normal.textColor = new Color (a [1], a [0], a [2]);
		
		if (ktDeath.isDie) {
			_GameOverPanl.SetActive (true);
		}
		if (PlayerPrefs.GetInt ("Win" + PlayerPrefs.GetInt ("Level")) == 1) {
			// 2048 win anmation
		}
		_score.text = iScore.ToString();
		_bestScore.text = iBest.ToString ();
		scoreSkin.box.fontSize = Screen.height * 9 / 324;

		scoreSkin.button.fontSize = Screen.height * 18 / 324;
		if (ktDeath.isDie && PlayerPrefs.GetInt ("Win" + PlayerPrefs.GetInt ("Level")) != 1 && GUI.Button (new Rect (Screen.width / 2 - Screen.height / 8f, Screen.height / 2 + Screen.height / 12f, Screen.height / 4, Screen.height / 8), "Replay", scoreSkin.button)) {
			if (PlayerPrefs.GetInt ("Best" + PlayerPrefs.GetInt ("Level")) < iScore) {
				PlayerPrefs.SetInt ("Best" + PlayerPrefs.GetInt ("Level"), iScore);
			}
			PlayerPrefs.SetInt ("Score" + PlayerPrefs.GetInt ("Level"), 0);
			for (int i = 0; i < 5; i++) {
				for (int j = Mathf.Abs(2-i); j < 9-Mathf.Abs(2-i); j+=2) {
					PlayerPrefs.SetInt (i + "." + j + PlayerPrefs.GetInt ("Level"), 0);
				}
			}
			PlayerPrefs.SetInt ("Win" + PlayerPrefs.GetInt ("Level"), 0);
			reload ();
		}
	}
	void reload(){
		Application.LoadLevel (Application.loadedLevel);
	}

}
