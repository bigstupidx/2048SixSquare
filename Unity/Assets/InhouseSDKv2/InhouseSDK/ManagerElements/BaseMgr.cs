using UnityEngine;
using System.Collections;

public class BaseMgr {
	protected bool _didInit = false;
	protected bool _didComplete = false;
	protected InhouseSDK.ConfigData _config;

	//// <summary>
	/// Chạy lúc khởi tạo InhouseSDK
	/// </summary>
	/// <param name="config">Config.</param>
	public BaseMgr() {
	}

	/// <summary>
	/// Parse plist từ source vô config
	/// Source có thể là:
	/// 	- Offline plist
	/// 	- Local data saved
	/// 	- Online plist
	/// </summary>
	/// <param name="data">Data.</param>
	public virtual void InitWithConfig(Hashtable data) {
		_didInit = false;
	}

	// chạy sau khi load plist về
	public virtual void OnConfigLoadComplete() {
		_didComplete = false;
	}

	/// <summary>
	/// Khi plist online có version lớn hơn plist trên máy
	/// </summary>
	public virtual void OnNewConfig() {
		if (!_didComplete)
			OnConfigLoadComplete ();
	}

	public void SetConfig(InhouseSDK.ConfigData config, bool inited) {
		_config = config;
		_didInit = inited;
	}
}
