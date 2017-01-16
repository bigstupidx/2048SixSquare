
#if !EVERYPLAY_DISABLE


using UnityEngine;
using System.Collections;
using System;



public class VideoRecordMgr : BaseMgr {

	bool _recordingReady = false;
	bool _hasLastRecord = false;
	Texture2D _texture = null;

	public Action OnUploadComplete;

	public VideoRecordMgr() {
	}

	public override void initWithConfig (Hashtable data)
	{
		base.initWithConfig (data);

		_didInit = true;
	}

	public override void onConfigLoadComplete ()
	{
		if (!_didInit)
			return;
		Everyplay.Initialize ();
		Everyplay.RecordingStarted += _onRecordVideoStart;
		Everyplay.RecordingStopped += _onRecordVideoStop;
		Everyplay.ReadyForRecording += _onReadyForRecording;
		Everyplay.ThumbnailTextureReady += _thumbnailTextureReady;
		Everyplay.UploadDidComplete += UploadDidComplete;

		Debug.Log ("Everyplay: support " + Everyplay.IsSupported().ToString());
	}
	private void UploadDidComplete(int videoId)
	{
		Debug.Log ("Da share video thanh cong!");
		if (OnUploadComplete != null) {
			OnUploadComplete ();
		}
	}

	public override void OnNewConfig () {
		if (!_didInit)
			return;
	}

	void _thumbnailTextureReady(Texture2D texture, bool portrait) {
		Debug.Log ("Everyplay: ThumbnailTextureReady");
//		_texture = texture;
	}

	void _onReadyForRecording(bool check) {
		_recordingReady = check;
		Debug.Log ("Everyplay ready: " + check.ToString());
	}

	void _onRecordVideoStart() {
		Debug.Log ("Everyplay.RecordingStarted");
	}

	void _onRecordVideoStop() {
		_hasLastRecord = true;
		Debug.Log ("Everyplay.RecordingStopped");
	}

	public void StartRecordVideo() {
		if (_recordingReady) {
			Everyplay.StartRecording ();
			Debug.Log ("VideoRecordMgr: Start Record");
		}
	}

	public void StopRecordVideo() {
		Debug.Log ("VideoRecordMgr: Stop Record"+_recordingReady);
		if (_recordingReady) {
			if (Everyplay.IsRecording ()) {
				Everyplay.StopRecording ();
			}
		}
	}

	public void PlayLastVideo() {
		Everyplay.PlayLastRecording ();
	}

	public void TakeThumbnai() {
		_texture = new Texture2D (Screen.width, Screen.height);
		_texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		_texture.Apply();
		Debug.Log ("VideoRecordMgr: Take Thumbnail");
	}

	public bool HasLastRecord() {
		return _hasLastRecord;
	}

	public void ShowVideoShare() {
		Everyplay.PlayLastRecording ();
	}

	public Texture2D getThumbnai() {
		return _texture;
	}
}

#endif