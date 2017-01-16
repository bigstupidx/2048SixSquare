using UnityEngine;
using System.Collections;

public class SoundConfig : Object
{
	public static string strSound = "Sound";
	public static void TurnOnSound(){
		PlayerPrefs.SetInt ("Sound", 0);
		PlayerPrefs.Save ();
	}

	public static void TurnOffSound(){
		PlayerPrefs.SetInt ("Sound", 1);
		PlayerPrefs.Save ();
	}

	public static bool IsOnSound(){
		return PlayerPrefs.GetInt ("Sound")==0;
	}
}

