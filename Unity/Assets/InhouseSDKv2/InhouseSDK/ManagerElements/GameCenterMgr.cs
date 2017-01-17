using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class GameCenterMgr : BaseMgr {
	public GameCenterMgr() {
	}

	public override void InitWithConfig (Hashtable data) {
		base.InitWithConfig (data);
		if (data == null)
			return;
		data = (Hashtable)data["GameCenter"];
		if (data == null)
			return;

		Dictionary<string, string> list = InhouseSDK.HashtableToDictionary<string, string> (data);
		_config.GameCenter.LeaderboardIds = list;

		_didInit = true;
	}

	public override void OnConfigLoadComplete() {
		if (!_didInit)
			return;
		initGameCenter ();

		_didComplete = true;
	}

	public void initGameCenter() {
		if (!_didInit)
			return;
		Debug.Log ("Init Game Center");
		Social.localUser.Authenticate (InitGameCenterCallback);
//		initGameCenter (_config.GameCenter.LeaderboardID);
	}

	void InitGameCenterCallback(bool success) {
		if (success)
			Debug.Log ("Init GameCenter success");
		else
			Debug.Log ("Init GameCenter failure");
	}

	public void ShowLeaderBoard() {
		if (!_didInit)
			return;
		Debug.Log("GameCenter: Show Leaderboard: " + _config.GameCenter.LeaderboardIds.First().Value);
		GameCenterPlatform.ShowLeaderboardUI(_config.GameCenter.LeaderboardIds.First().Value, TimeScope.AllTime);
	}

	public void SendScoreToLeaderBoard(int score) {
		if (!_didInit)
			return;
		Debug.Log("GameCenter: Send score (" + score + ") to Leaderboard: " + _config.GameCenter.LeaderboardIds.First().Value);
		Social.ReportScore(score, _config.GameCenter.LeaderboardIds.First().Value, SendScoreCallback);
	}

	public void ShowLeaderBoard(string key) {
		if (!_didInit)
			return;
		if (!_config.GameCenter.LeaderboardIds.ContainsKey (key)) {
			HDDebug.Log ("GameCenterMgr: " + key + " not exist in plist");
			return;
		}
		Debug.Log("GameCenter: Show Leaderboard: " + _config.GameCenter.LeaderboardIds[key]);
		GameCenterPlatform.ShowLeaderboardUI(_config.GameCenter.LeaderboardIds[key], TimeScope.AllTime);
	}

	public void SendScoreToLeaderBoard(string key, int score) {
		if (!_didInit)
			return;
		if (!_config.GameCenter.LeaderboardIds.ContainsKey (key)) {
			HDDebug.Log ("GameCenterMgr: " + key + " not exist in plist");
			return;
		}
		Debug.Log("GameCenter: Send score (" + score + ") to Leaderboard: " + _config.GameCenter.LeaderboardIds[key]);
		Social.ReportScore(score, _config.GameCenter.LeaderboardIds[key], SendScoreCallback);
	}

	void SendScoreCallback(bool success) {
		Debug.Log ("GameCenter: Send score result: " + success);
	}
}
