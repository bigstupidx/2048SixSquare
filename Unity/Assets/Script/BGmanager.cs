using UnityEngine;
using System.Collections;

public class BGmanager : MonoBehaviour
{
		public Sprite[] sp;
		// Use this for initialization
		void Awake ()
		{
				
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (PlayerPrefs.GetInt ("Win" + PlayerPrefs.GetInt ("Level")) == 1 || ktDeath.isDie) {
						GetComponent<SpriteRenderer> ().sprite = sp [1];
				} else {
						GetComponent<SpriteRenderer> ().sprite = sp [0];
				}
		}
}
