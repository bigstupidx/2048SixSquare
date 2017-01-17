using UnityEngine;
using System.Collections;

public class PromoteGameMgr : BaseMgr {
	const string TAG_TIMESRATE = "TAG_PROMOTE_TIMESRATE";
	const string TAG_LASTRATE = "TAG_PROMOTE_LASTRATE";

	int _timesRate;
	int _timesLastRate;

	public PromoteGameMgr ()
	{
		_timesRate = PlayerPrefs.GetInt(TAG_TIMESRATE, 0);
		_timesLastRate = PlayerPrefs.GetInt (TAG_TIMESRATE, 0);
	}

	public override void InitWithConfig (Hashtable data) {
		base.InitWithConfig (data);
		if (data == null)
			return;
		data = (Hashtable)data["PromoteGame"];
		if (data == null)
			return;

		_config.PromoteGame.Enable = (bool)data["Enable"];
		_config.PromoteGame.TimesToShow = TypeConvert.ToInt(data["TimesToShow"]);
		_config.PromoteGame.TimesShowNext = TypeConvert.ToInt(data["TimeShowNext"]);

		_didInit = true;
	}

	public override void OnConfigLoadComplete() {
		if (!_didInit)
			return;

		// Log event login

		_didComplete = true;
	}

	public bool CanPromoteGame() {
		if (!_config.PromoteGame.Enable)
			return false;
		if (_timesRate < _config.PromoteGame.TimesToShow)
			return false;
		if (_timesLastRate == 0 && _timesRate >= _config.PromoteGame.TimesToShow)
			return true;
		if (_timesRate - _timesLastRate >= _config.PromoteGame.TimesShowNext)
			return true;
		return false;
	}

	public void OnGameOver() {
		if (!_config.PromoteGame.Enable)
			return;
		_timesRate++;
		PlayerPrefs.SetInt (TAG_TIMESRATE, _timesRate);
		PlayerPrefs.Save ();
	}

	public void MarkAsShowPromote() {
		_timesLastRate = _timesRate;
		PlayerPrefs.SetInt (TAG_LASTRATE, _timesLastRate);
	}
}
