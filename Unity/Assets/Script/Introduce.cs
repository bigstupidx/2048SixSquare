using UnityEngine;
using System.Collections;

public class Introduce : MonoBehaviour
{
		public GUISkin scoreSkin;
	
		public Sprite[] sp;
		// Use this for initialization
		void Awake ()
		{
				if (PlayerPrefs.GetInt ("Level") == 1) {
						GetComponent<SpriteRenderer> ().sprite = sp [0];
				} else if (PlayerPrefs.GetInt ("Level") == 2) {
						GetComponent<SpriteRenderer> ().sprite = sp [1];
				} else
						GetComponent<SpriteRenderer> ().sprite = sp [2];
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (PlayerPrefs.GetInt ("Level") == 1) {
						GetComponent<SpriteRenderer> ().sprite = sp [0];
				} else if (PlayerPrefs.GetInt ("Level") == 2) {
						GetComponent<SpriteRenderer> ().sprite = sp [1];
				} else
						GetComponent<SpriteRenderer> ().sprite = sp [2];
		}
		void OnGUI ()
		{
				if (PlayerPrefs.GetInt ("Level") == 1) {
						scoreSkin.box.fontSize = Screen.height * 17 / 324;
						GUI.Box (new Rect (Screen.width / 2 - Screen.height / 6, 0 + Screen.height / 6.2f, Screen.height / 3f, Screen.height / 12), "Easy", scoreSkin.box);
				} else if (PlayerPrefs.GetInt ("Level") == 2) {
						scoreSkin.box.fontSize = Screen.height * 17 / 324;
						GUI.Box (new Rect (Screen.width / 2 - Screen.height / 6, 0 + Screen.height / 6.2f, Screen.height / 3f, Screen.height / 12), "Professional", scoreSkin.box);
				} else {
						scoreSkin.box.fontSize = Screen.height * 17 / 324;
						GUI.Box (new Rect (Screen.width / 2 - Screen.height / 6, 0 + Screen.height / 6.2f, Screen.height / 3f, Screen.height / 12), "Guru", scoreSkin.box);
				}
		}
}
