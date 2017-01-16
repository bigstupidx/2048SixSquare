using UnityEngine;
using System.Collections;

public class Setting : MonoBehaviour
{
		public GUISkin choiseSkin;
		static public bool isSet;
		public float fSince = 2;

		private int level;
		private int restart;
		private int style;
		private int speed;
		
		private string sRes;
		private string sTyl;
		private string sSpe;
		 
		
		// Use this for initialization
		void Start ()
		{
				level = PlayerPrefs.GetInt ("Level");
		}
	
		void Update ()
		{

				if (Input.GetKeyDown (KeyCode.Escape)) {
						if (isSet == true) {
								isSet = false;
						} else {
								Application.Quit ();
						}
				}	
		}

		void OnGUI ()
		{
//				settingSkin.button.fontSize = Screen.height * 9 / 324;
//				if (GUI.Button (new Rect (Screen.width - Screen.height / 14, 0 + Screen.height / 14 - Screen.height / 15, Screen.height / 15, Screen.height / 15), "", settingSkin.button)) {
//						level++;
//						if (level == 4) {
//								level = 1;
//						}
//						if (level == 1) {
//								PlayerPrefs.SetInt ("Level", level);
//						} else if (level == 2) {
//								PlayerPrefs.SetInt ("Level", level);
//						} else if (level == 3) {
//								PlayerPrefs.SetInt ("Level", level);
//						}
//						Application.LoadLevel (Application.loadedLevel);
//				}
				if (GUI.Button (new Rect (Screen.width / 2 - Screen.height / 30, 0 + Screen.height / 14 - Screen.height / 15, Screen.height / 15, Screen.height / 15), "", choiseSkin.button)) {
						if (PlayerPrefs.GetInt ("Best" + PlayerPrefs.GetInt ("Level")) < Score.iScore) {
								PlayerPrefs.SetInt ("Best" + PlayerPrefs.GetInt ("Level"), Score.iScore);
						}
						PlayerPrefs.SetInt ("Score" + PlayerPrefs.GetInt ("Level"), 0);
						for (int i = 0; i < 5; i++) {
								for (int j = Mathf.Abs(2-i); j < 9-Mathf.Abs(2-i); j+=2) {
										PlayerPrefs.SetInt (i + "." + j + PlayerPrefs.GetInt ("Level"), 0);
								}
						}
						PlayerPrefs.SetInt ("Win" + PlayerPrefs.GetInt ("Level"), 0);
						Application.LoadLevel (Application.loadedLevel);
				}
		}
}
