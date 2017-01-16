using UnityEngine;
using System.Collections;

public class BGsettingManager : MonoBehaviour
{

		public Sprite[] sp;
		// Use this for initialization
		void Awake ()
		{
				if (Setting.isSet) {
						if (PlayerPrefs.GetInt ("Style") == 1) {
								GetComponent<SpriteRenderer> ().sprite = sp [1];
						} else if (PlayerPrefs.GetInt ("Style") == 2) {
								GetComponent<SpriteRenderer> ().sprite = sp [2];
						}
				} else
						GetComponent<SpriteRenderer> ().sprite = sp [0];
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (!ktDeath.isDie) {
						if (Setting.isSet) {
								if (PlayerPrefs.GetInt ("Style") == 1) {
										GetComponent<SpriteRenderer> ().sprite = sp [1];
								} else if (PlayerPrefs.GetInt ("Style") == 2) {
										GetComponent<SpriteRenderer> ().sprite = sp [2];
								}
						} else
								GetComponent<SpriteRenderer> ().sprite = sp [0];
				} else {
						if (PlayerPrefs.GetInt ("Style") == 1) {
								GetComponent<SpriteRenderer> ().sprite = sp [1];
						} else if (PlayerPrefs.GetInt ("Style") == 2) {
								GetComponent<SpriteRenderer> ().sprite = sp [2];
						}
				}
		}
}
