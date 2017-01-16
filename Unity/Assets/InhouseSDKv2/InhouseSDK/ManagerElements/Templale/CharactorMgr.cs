using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public class CharactorMgr : BaseMgr {
//
//	public CharactorMgr() {
//	}
//
//	public override void initWithConfig (Hashtable data) {
//		base.initWithConfig (data);
//
//		if (data == null)
//			return;
//		// need to fix
//
//		Hashtable chars = (Hashtable)data ["Characters"];
//
//		List<InhouseSDK.CharacterDescription> list = new List<InhouseSDK.CharacterDescription> ();
//		foreach (var item in chars.Keys) {
//			string name = (string)item;
//			int price = (int)chars [name];
//			InhouseSDK.CharacterDescription descript = new InhouseSDK.CharacterDescription ();
//			descript.Name = name;
//			descript.Price = price;
//			list.Add (descript);
//		}
//		_config.Character.Characters = list;
//
//
//		_didInit = true;
//	}
//
//	public override void onConfigLoadComplete() {
//		if (!_didInit)
//			return;
//
//
//	}
//
//	public int GetNewPrice(string name, int oldPrice) {
//		List<InhouseSDK.CharacterDescription> list = _config.Character.Characters;
//		foreach (var item in list) {
//			if (item.Name == name)
//				return item.Price;
//		}
//		return oldPrice;
//	}
//}
