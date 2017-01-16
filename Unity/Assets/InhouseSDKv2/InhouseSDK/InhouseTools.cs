#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class InhouseTools  {


	[MenuItem("Inhouse/Clear PlayerPref")]
	public static void clearPlayer() {
		PlayerPrefs.DeleteAll ();
		Debug.Log ("PlayerPref clear");
	}
}
#endif