using UnityEngine;
using System.Collections;

public class guide : MonoBehaviour
{
		public GUISkin guideSkin;
		// Use this for initialization
		
		void OnGUI ()
		{
				GUI.Box (new Rect (0, Screen.height - Screen.height / 15, Screen.height / 3.5f, Screen.height / 13), "", guideSkin.box);
		}
}
