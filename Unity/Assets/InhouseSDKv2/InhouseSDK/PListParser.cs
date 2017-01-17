/* Example Saving:
 
 		Hashtable playerData = new Hashtable();
		playerData.Add("Health",100);
		playerData.Add("TestObject", 1.5f);
		ArrayList guns = new ArrayList();
		guns.Add("AK-47");
		guns.Add("Pistol");
		playerData.Add("Guns", guns);
		Hashtable grenades = new Hashtable();
		grenades.Add("FragmentationCount", 1);
		grenades.Add("IncendiaryCount", 1);
		playerData.Add("Grenades", grenades);
 
		//save outside the current project (same folder as Assets and Library)
		String xmlFile = Application.dataPath + "/../ExampleSaveFile.plist";
		PListManager.SavePlistToFile(xmlFile, playerData);
 *
 */
using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Text;
using System.Globalization;
using UnityEngine;

public class PListParser {

	private const string SUPPORTED_VERSION = "1.0";

	public static bool ParsePListFileFromContent(string xmlcontent, ref Hashtable plist){
		XmlDocument xml = new XmlDocument();
		xml.XmlResolver = null; //Disable schema/DTD validation, it's not implemented for Unity.
		xml.LoadXml(xmlcontent);


		XmlNode plistNode = xml.LastChild;
		if (!plistNode.Name.Equals("plist")) {
			Debug.LogError("plist file missing <plist> nodes.");
			return false;
		}

		string plistVers = plistNode.Attributes["version"].Value;
		if (plistVers == null || !plistVers.Equals(SUPPORTED_VERSION)) {
			Debug.LogError("This is an unsupported plist version: " + plistVers + ". Required version:a " + SUPPORTED_VERSION);
			return false;
		}

		XmlNode dictNode = plistNode.FirstChild;
		if (!dictNode.Name.Equals("dict")) {
			Debug.LogError("Missing root dict from plist file: ");
			return false;
		}

		return LoadDictFromPlistNode(dictNode, ref plist);
	}

	public static bool ParsePListFile(string xmlFile, ref Hashtable plist) {
		if (!File.Exists(xmlFile)) {
			Debug.LogError("File doesn't exist: " + xmlFile);
			return false;
		}

		StreamReader sr = new StreamReader(xmlFile);
		string txt = sr.ReadToEnd();
		sr.Close();

		return ParsePListFileFromContent (txt, ref plist);
	}


	#region LOAD_PLIST_PRIVATE_METHODS
	private static bool LoadDictFromPlistNode(XmlNode node, ref Hashtable dict) {
		if (node == null) {
			Debug.LogError("Attempted to load a null plist dict node.");
			return false;
		}
		if (!node.Name.Equals("dict")) {
			Debug.LogError("Attempted to load an dict from a non-array node type: " + node + ", " + node.Name);
			return false;
		}
		if (dict == null) {
			dict = new Hashtable();
		}

		int cnodeCount = node.ChildNodes.Count;
		for (int i = 0; i+1 < cnodeCount; i+=2) {
			// Select the key and value child nodes
			XmlNode keynode = node.ChildNodes.Item(i);
			XmlNode valuenode = node.ChildNodes.Item(i+1);

			if (keynode.Name.Equals("key")) {
				string key = keynode.InnerText;
				ValueObject value = new ValueObject();

				if (LoadValueFromPlistNode(valuenode, ref value)) {
					if (!AddKeyValueToDict(ref dict, key, value)) {
						Debug.LogError("Failed to add key value to dict when loading plist from dict");
						return false;
					}
				} else {
					Debug.LogError("Did not load plist value correctly for key in node: " + key + ", " + node);
					return false;
				}
			} else {
				Debug.LogError("The plist being loaded may be corrupt.");
				return false;
			}

		} //end for

		return true;
	}

	private static bool LoadValueFromPlistNode(XmlNode node, ref ValueObject value) {
		if (node == null) {
			Debug.LogError("Attempted to load a null plist value node.");
			return false;
		}
		if (node.Name.Equals("string")) { value.val = node.InnerText; }
		else if (node.Name.Equals("integer")) { value.val = int.Parse(node.InnerText); }
		else if (node.Name.Equals("real")) { value.val = float.Parse(node.InnerText); }
		else if (node.Name.Equals("date")) { value.val = DateTime.Parse(node.InnerText, null, DateTimeStyles.None); } // Date objects are in ISO 8601 format
		else if (node.Name.Equals("data")) { value.val = node.InnerText; } // Data objects are just loaded as a string
		else if (node.Name.Equals("true")) { value.val = true; } // Boollean values are empty objects, simply identified with a name being "true" or "false"
		else if (node.Name.Equals("false")) { value.val = false; }
		// The value can be an array or dict type.  In this case, we need to recursively call the appropriate loader functions for dict and arrays.
		// These functions will in turn return a boolean value for their success, so we can just return that.
		// The val value also has to be instantiated, since it's being passed by reference.
		else if (node.Name.Equals("dict")) {
			value.val = new Hashtable();
			Hashtable htRef = (Hashtable)value.val;
			return LoadDictFromPlistNode(node, ref htRef);
		}
		else if (node.Name.Equals("array")) {
			value.val = new ArrayList();
			ArrayList alRef = (ArrayList)value.val;
			return LoadArrayFromPlistNode(node, ref alRef);
		} else {
			Debug.LogError("Attempted to load a value from a non value type node: " + node + ", " + node.Name);
			return false;
		}

		return true;
	}

	private static bool LoadArrayFromPlistNode(XmlNode node, ref ArrayList array ) {
		if (node == null) {
			Debug.LogError("Attempted to load a null plist array node.");
			return false;
		}
		if (!node.Name.Equals("array")) {
			Debug.LogError("Attempted to load an array from a non-array node type: " + node + ", " + node.Name);
			return false;
		}

		if (array == null) { array = new ArrayList(); }

		int nodeCount = node.ChildNodes.Count;
		for (int i = 0; i < nodeCount; i++) {
			XmlNode cnode = node.ChildNodes.Item(i);
			ValueObject element = new ValueObject();
			if (LoadValueFromPlistNode(cnode, ref element)) {
				array.Add(element.val);
			} else {
				return false;
			}
		}

		return true;
	}

	private static bool AddKeyValueToDict(ref Hashtable dict, string key, ValueObject value) {
		if (dict == null || key == null || key.Length < 1 || value == null) {
			Debug.LogError("Attempted to AddKeyValueToDict() with null objects.");
			return false;
		}
		if (!dict.ContainsKey(key)) {
			dict.Add(key, value.val);
			return true;
		}
		if (value.val.GetType() != dict[key].GetType()) {
			Debug.LogWarning("Value type mismatch for overlapping key (will replace old value with new one): " + value.val + ", " + dict[key] + ", " + key);
			dict[key] = value.val;
		}
		else if (value.val.GetType() == typeof(Hashtable)) {
			Hashtable htTmp = (Hashtable)value.val;
			foreach (object element in htTmp) {
				Hashtable htRef = (Hashtable)dict[key];
				if (!AddKeyValueToDict(ref htRef, (string)element, new ValueObject(htTmp[element]))) {
					Debug.LogError("Failed to add key value to dict: " + element + ", " + htTmp[element] + ", " + dict[key]);
					return false;
				}
			}
		}

		else if (value.val.GetType() == typeof(ArrayList)) {
			ArrayList alTmp = (ArrayList)value.val;
			ArrayList alAddTmp = (ArrayList)dict[key];
			foreach (object element in alTmp) {
				alAddTmp.Add(element);
			}
		}

		else {
			dict[key] = value.val;
		}

		return true;
	}
	#endregion


	public static bool SavePlistToFile (String xmlFile, Hashtable plist) {
		if (plist == null) {
			Debug.LogError("Passed a null plist hashtable to SavePlistToFile.");
			return false;
		}

		XmlDocument xml = new XmlDocument();
		xml.XmlResolver = null; 

		XmlDeclaration xmldecl = xml.CreateXmlDeclaration("1.0", "UTF-8", null);
		xml.PrependChild(xmldecl);

		XmlDocumentType doctype = xml.CreateDocumentType("plist", "-//Apple//DTD PLIST 1.0//EN", "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null);
		xml.AppendChild(doctype);

		XmlNode plistNode = xml.CreateNode(XmlNodeType.Element, "plist", null);
		XmlAttribute plistVers = (XmlAttribute)xml.CreateNode(XmlNodeType.Attribute, "version", null);
		plistVers.Value = "1.0";
		plistNode.Attributes.Append(plistVers);
		xml.AppendChild(plistNode);

		if (!SaveDictToPlistNode(plistNode, plist)) {
			Debug.LogError("Failed to save plist data to root dict node: " + plist);
			return false;
		} else { // We were successful
			StreamWriter sw = new StreamWriter(xmlFile, false, System.Text.Encoding.UTF8);
			xml.Save(sw);
			sw.Close();
		}

		return true;
	}

	#region SAVE_PLIST_PRIVATE_METHODS

	private static bool SaveDictToPlistNode(XmlNode node, Hashtable dict) {
		if (node == null) {
			Debug.LogError("Attempted to save a null plist dict node.");
			return false;
		}

		XmlNode dictNode = node.OwnerDocument.CreateNode(XmlNodeType.Element, "dict", null);
		node.AppendChild(dictNode);

		if (dict == null) {
			Debug.LogWarning("Attemped to save a null dict: " + dict);
			return true;
		}

		foreach (object key in dict.Keys) {
			XmlElement keyNode = node.OwnerDocument.CreateElement("key");
			keyNode.InnerText = (string)key;
			dictNode.AppendChild(keyNode);

			if (!SaveValueToPlistNode(dictNode, dict[key])) {
				Debug.LogError("Failed to save value to plist node: " + key);
				return false;
			}
		}

		// If we got this far then all is well.  Return true to indicate success.
		return true;
	}

	private static bool SaveValueToPlistNode(XmlNode node, object value) {
		XmlNode valNode;
		System.Type type = value.GetType();
		if (type == typeof(String)) {
			valNode = node.OwnerDocument.CreateElement("string");
		}
		else if (type == typeof(Int16) ||type == typeof(Int32)  ||type == typeof(Int64)) { 
			valNode = node.OwnerDocument.CreateElement("integer"); 
		}
		else if (type == typeof(Single)   ||type == typeof(Double)  ||type == typeof(Decimal)) { 
			valNode = node.OwnerDocument.CreateElement("real"); 
		}
		else if (type == typeof(DateTime)) {
			valNode = node.OwnerDocument.CreateElement("date");
			DateTime dt = (DateTime)value;
			valNode.InnerText = dt.ToUniversalTime().ToString("o");
			node.AppendChild(valNode);
			return true;
		}
		else if (type == typeof(bool)) {

			if ((bool)value == true) { valNode = node.OwnerDocument.CreateElement("true"); }
			else { valNode = node.OwnerDocument.CreateElement("false"); }
			node.AppendChild(valNode);
			return true;
		}

		else if (type == typeof(Hashtable))	{
			return SaveDictToPlistNode(node, (Hashtable)value);
		}
		else if (type == typeof(ArrayList)) { return SaveArrayToPlistNode(node, (ArrayList)value); }

		else {
			valNode = node.OwnerDocument.CreateElement("data");
		}

		if (valNode != null) valNode.InnerText = value.ToString();
		node.AppendChild(valNode);
		return true;
	}

	private static bool SaveArrayToPlistNode (XmlNode node, ArrayList array) {
		// Create the value node as an "array" element.
		XmlElement arrayNode = node.OwnerDocument.CreateElement("array");
		node.AppendChild(arrayNode);
		foreach (object element in array) {
			if (!SaveValueToPlistNode(arrayNode, element)) { return false; }
		}
		return true;
	}

	#endregion

} //end PListManager class

class ValueObject {
	public object val;
	public ValueObject() {}
	public ValueObject(object aVal) { val = aVal; }
}
