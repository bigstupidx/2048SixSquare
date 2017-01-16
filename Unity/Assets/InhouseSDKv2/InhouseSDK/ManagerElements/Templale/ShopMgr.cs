using System;
using System.Collections;
using System.Collections.Generic;

//public class ShopMgr : BaseMgr
//{
//	public ShopMgr() {
//	}
//	public override void initWithConfig (Hashtable data) {
//		base.initWithConfig (data);
//
//		if (data == null)
//			return;
//		// need to fix
//
//		List<InhouseSDK.ShopItemData> list = new List<InhouseSDK.ShopItemData> ();
//		foreach (var id in data.Keys) {
//			InhouseSDK.ShopItemData item = new InhouseSDK.ShopItemData ();
//			Hashtable dataItem = (Hashtable)data [id];
//			item.Id = (string)id;
//			item.Name = (string)dataItem["Name"];
//			item.Image = (string)dataItem["Image"];
//			item.Description = (string)dataItem["Description"];
//			item.Price = (int)dataItem["Price"];
//			item.Group = (string)dataItem["Group"];
//			list.Add (item);
//		}
//		_config.ShopConfig.Items = list;
//
//		_didInit = true;
//	}
//
//	public override void onConfigLoadComplete() {
//		if (!_didInit)
//			return;
//	}
//
//	public InhouseSDK.ShopItemData getShopConfigData(string id) {
//		for (int i = 0; i < _config.ShopConfig.Items.Count; i++) {
//			InhouseSDK.ShopItemData item = _config.ShopConfig.Items [i];
//			if (item.Id == id)
//				return item;
//		}
//		return null;
//	}
//}

