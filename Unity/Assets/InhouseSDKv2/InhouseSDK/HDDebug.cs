using UnityEngine;
using System.Collections;

public class HDDebug : MonoBehaviour {
	const bool IsDebug = true;
	public static void Log(string message) {
		if (IsDebug)
			UnityEngine.Debug.Log (message);
	}
}
