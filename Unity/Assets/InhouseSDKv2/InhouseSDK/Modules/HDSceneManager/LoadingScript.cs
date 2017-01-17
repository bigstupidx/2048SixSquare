using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScript : MonoBehaviour {
	public Image _image;
	public Text _txtLoading;
	public Image _tank;

	void OnEnable() {
		_txtLoading.text = "Connecting";
	}

	public void setTranparent(float alpha) {
		Color color = _image.color;
		color.a = alpha;
		_image.color = color;
		color = _txtLoading.color;
		color.a = alpha;
		_txtLoading.color = color;
		color = _tank.color;
		color.a = alpha;
		_tank.color = color;
	}

	float _currentTime = 0;
	void Update() {
		float time = 0.2f;
		if (_currentTime > time) {// Connecting...
			string text = _txtLoading.text;
			if (text.Length == 13)
				text = "Connecting";
			else
				text += ".";
			_txtLoading.text = text;
			_currentTime -= time;
		} else
			_currentTime += Time.deltaTime;
	}
}
