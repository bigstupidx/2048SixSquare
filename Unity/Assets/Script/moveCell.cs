using UnityEngine;
using System.Collections;

public class moveCell : MonoBehaviour
{
		public float fSpeed = 1;
		public Sprite[] sp;
		string s;
		string s1 ;
		string s2;
		int i ;
		int j;
		float fTime = 0;
		public	bool kt = false;
		public Animator anime;
		// Use this for initialization
		void Awake ()
		{
				GetComponent<Animator> ().SetTrigger ("born");
		}
		void Start ()
		{
				anime = GetComponent<Animator> ();
				GetComponent<SpriteRenderer> ().sprite = sp [PlayerPrefs.GetInt (transform.tag + PlayerPrefs.GetInt ("Level"))];
				s = transform.tag;
				s1 = "" + s [0];
				s2 = "" + s [2];
				i = int.Parse (s1);
				j = int.Parse (s2);
				
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (transform.tag != s) {
						s = transform.tag;
						s1 = "" + s [0];
						s2 = "" + s [2];
						i = int.Parse (s1);
						j = int.Parse (s2);
				}		
				transform.position = Vector3.Lerp (transform.position, CellManager.arrPos [i, j], fSpeed * Time.deltaTime);
				if (!CellManager.isChuyen) {
						GetComponent<SpriteRenderer> ().sprite = sp [PlayerPrefs.GetInt (transform.tag + PlayerPrefs.GetInt ("Level"))];
				}
				if (kt) {
						anime.SetTrigger ("scale");
						GetComponent<SpriteRenderer> ().sprite = sp [PlayerPrefs.GetInt (transform.tag + PlayerPrefs.GetInt ("Level"))];
						kt = false;
				}
		}
}
