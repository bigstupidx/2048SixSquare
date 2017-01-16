using UnityEngine;
using System.Collections;

public class PopupShow : MonoBehaviour {

	public GameObject[] _list;
	public GameObject _Panel;
	Vector3 _start;
	Vector3 _startCurrent;
	void Awake(){
		_start = _Panel.transform.position;
		_startCurrent = new Vector3 (_start.x, _start.y + 10, _start.z);
		_Panel.transform.position = _startCurrent;
	}

	void Start () {
		Debug.Log (_start);
		_Panel.transform.position = Vector3.Lerp (_startCurrent, _start, Time.deltaTime);
		Debug.Log (_Panel.transform.position);
	}

	IEnumerator showAnimation(){
		yield return new WaitForSeconds (1);
		_Panel.transform.position = Vector3.Lerp (_startCurrent, _start, Time.deltaTime);
	}

}
