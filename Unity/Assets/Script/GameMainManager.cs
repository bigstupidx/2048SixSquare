using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMainManager : MonoBehaviour {

		private int level;

		public void model1(){
//				level = 1;
//				PlayerPrefs.SetInt ("Level", level);
//				Application.LoadLevel (Application.loadedLevel);
				LoadSceneForModel ();
		}
		public void model2(){
				level = 2;
				PlayerPrefs.SetInt ("Level", level);
//				Application.LoadLevel (Application.loadedLevel);
				LoadSceneForModel ();
		}
		public void model3(){
				level = 3;
				PlayerPrefs.SetInt ("Level", level);
//				Application.LoadLevel (Application.loadedLevel);
				LoadSceneForModel ();
		}

		void LoadSceneForModel(){
				SceneManager.LoadScene ("GamePlay");
		}
}
