using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {

	public Text _best;
	public Text _score;

	void Start(){
		int iScore = PlayerPrefs.GetInt ("Score" + PlayerPrefs.GetInt ("Level"));
		int iBest = PlayerPrefs.GetInt ("Best" + PlayerPrefs.GetInt ("Level"));
		_best.text = iBest.ToString();
		_score.text = iScore.ToString ();
	}
}
