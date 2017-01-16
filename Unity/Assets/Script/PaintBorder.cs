using UnityEngine;
using System.Collections;

public class PaintBorder : MonoBehaviour
{
		public Transform cell;
		public Transform cellBG;
		public float fSince;
		private float fWidCell;
		private float fHeiCell;
		// Use this for initialization
		void Start ()
		{
				fWidCell = cell.GetComponent<SpriteRenderer> ().sprite.bounds.size.x;
				fHeiCell = cell.GetComponent<SpriteRenderer> ().sprite.bounds.size.y;
				for (int i = 0; i < 5; i++) {
						for (int j = Mathf.Abs(2-i); j < 9-Mathf.Abs(2-i); j+=2) {
								PlayerPrefs.SetInt (i + "." + j, 0);
								Vector3 pos = new Vector3 (0 - fWidCell / 4.6f * (4 - j), fSince - fHeiCell / 3.1f * (i - 2), 0);
								Instantiate (cellBG, pos, Quaternion.identity);
						}
				}
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
}
