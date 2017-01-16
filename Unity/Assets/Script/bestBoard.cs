using UnityEngine;
using System.Collections;

public class bestBoard : MonoBehaviour
{
		public GUISkin scoreSkin;
		float x;
		// Use this for initialization
		void Start ()
		{
				x = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width - Screen.height / 7.1f + Screen.height / 16, 0, 0)).x;
				transform.position = new Vector3 (x, transform.position.y, transform.position.z);
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
		void OnGUI ()
		{
				scoreSkin.box.fontSize = Screen.height * 9 / 324;
				GUI.Box (new Rect (Screen.width - Screen.height / 7.0f, 0 + Screen.height / 15f, Screen.height / 8, Screen.height / 16), "BEST", scoreSkin.box);
		}
}
