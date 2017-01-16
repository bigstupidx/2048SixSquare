using System;

public class HDUtils
{
	/// <summary>
	/// Ý sếp muốn là parse ra null là phải báo lỗi, thế ta phải làm thôi
	/// </summary>
	/// <returns>The string.</returns>
	/// <param name="data">Data.</param>
	public static string ParseString(object data) {
		if (data == null)
			throw new ArgumentNullException ("Parse string null");
		return (string)data;
	}
}

