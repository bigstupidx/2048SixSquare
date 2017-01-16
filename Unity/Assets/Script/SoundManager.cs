using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public static SoundManager Instance;
	private AudioSource Source;
	public AudioClip _btnClip;
	public AudioClip _PlayClip;
	public AudioClip _dichuyenClip;
	public AudioClip _dichuyenNguocClip;

	void Awake() {
		PlayerPrefs.DeleteAll ();
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad (Instance);
		}
	}

	void Start()
	{
		Source = GetComponent<AudioSource> ();
		Source.volume = 1f;//1
	}

	public void btnSound()
	{
		if (!SoundConfig.IsOnSound ())
			return;
		Source.clip = _btnClip;
		Source.Play ();
	}
	public void btnPlaySound()
	{
		if (!SoundConfig.IsOnSound ())
			return;
		Source.clip = _PlayClip;
		Source.Play ();
	}
	public void soundDichuyen()
	{
		if (!SoundConfig.IsOnSound ())
			return;
		Source.clip = _dichuyenClip;
		Source.Play ();
	}
	public void soundDichuyenNguoc()
	{
		if (!SoundConfig.IsOnSound ())
			return;
		Source.clip = _dichuyenNguocClip;
		Source.Play ();
	}
}
