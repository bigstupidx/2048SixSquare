using UnityEngine;
using System;
using System.Collections;

//public class CoinBonusMgr : BaseMgr
//{
//	bool _didLike = false;
//	bool _didShare = false;
//
//	public CoinBonusMgr() {
//	}
//
//	public override void initWithConfig (Hashtable data) {
//		base.initWithConfig (data);
//
//		if (data == null)
//			return;
//		// need to fix
//		
//		_config.CoinBonusConfig.LikeBonus = (int)data["LikeBonus"];
//		_config.CoinBonusConfig.RateBonus = (int)data["RateBonus"];
//		_config.CoinBonusConfig.ShareBonus = (int)data["ShareBonus"];
//
//		Hashtable temp = (Hashtable)data["BuyBonus"];
//		InhouseSDK.BuyBonus buyBonus = new InhouseSDK.BuyBonus ();
//		buyBonus.CoinBonus = (int)temp["CoinBonus"];
//		buyBonus.ProductId = (string)temp["ProductId"];
//
//		Hashtable languages = (Hashtable)temp["Languages"];
//		InhouseSDK.Languages outLang = new InhouseSDK.Languages();
//		foreach (DictionaryEntry entry in languages) {
//			InhouseSDK.Language lang = new InhouseSDK.Language ();
//			Hashtable content = (Hashtable)entry.Value;
//			lang.Cancel = (string)content ["Cancel"];
//			lang.Key = (string)entry.Key;
//			lang.Message = (string)content ["Message"];
//			lang.OK = (string)content ["OK"];
//			lang.Title = (string)content ["Title"];
//			outLang.addLanguage (lang);
//		}
//		buyBonus.Languages = outLang;
//		_config.CoinBonusConfig.BuyBonus = buyBonus;
//		_didLike = PlayerPrefs.GetInt ("did_like", 0) == 1;
//		_didShare = PlayerPrefs.GetInt ("did_share", 0) == 1;
//
//		_didInit = true;
//	}
//
//	public override void onConfigLoadComplete() {
//		if (!_didInit)
//			return;
//	}
//
//	public int GetLikeCoinBonus() {
//		return _config.CoinBonusConfig.LikeBonus;
//	}
//
//	public void SetLikeCoin() {
//		_didLike = true;
//		PlayerPrefs.SetInt ("did_like", 1);
//		PlayerPrefs.Save ();
//	}
//
//	public bool DidLikeCoin() {
//		return _didLike;
//	}
//
//	public int GetRateCoinBonus() {
//		return _config.CoinBonusConfig.RateBonus;
//	}
//
//	public int GetShareCoinBonus() {
//		return _config.CoinBonusConfig.ShareBonus;
//	}
//
//	public void SetShareCoin() {
//		_didShare = true;
//		PlayerPrefs.SetInt ("did_share", 1);
//		PlayerPrefs.Save ();
//	}
//
//	public bool DidShareCoin() {
//		return _didShare;
//	}
//
//	public int GetBuyCoinBonus() {
//		return _config.CoinBonusConfig.BuyBonus.CoinBonus;
//	}
//
//	public void BuyBonusCoin() {
////		InhouseSDK.Language language = _config.CoinBonusConfig.BuyBonus.Languages.getLanguage (
////			                               InhouseSDK.getInstance ().GetCurrentSystemLanguage ());
////		InhouseSDK.getInstance ().ShowPopup (language.Title, language.Message, language.OK, language.Cancel, "", (string respone) => {
////			InhouseSDK.getInstance().BuyProduct(_config.CoinBonusConfig.BuyBonus.ProductId, (string responeA) => {
////				int medal = UserManager.getInstance().getMedal();
////				UserManager.getInstance().setMedal(medal + _config.CoinBonusConfig.BuyBonus.CoinBonus);
////			}, (string responeB) => {
////				Debug.Log("Buy bonus coin failure");
////			});
////		});
//	}
//}
//
