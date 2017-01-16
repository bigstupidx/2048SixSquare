using System;

/// <summary>
/// Dành cho người sẽ đọc code này,
/// Tại sao có class này
/// Vì việc cast kiểu từ plist sang kiểu C# có một số vấn đề, sinh lỗi, nên parse kiểu này
/// sẽ an toàn
/// </summary>
public class TypeConvert
{
	/// <summary>
	/// Convert các kiểu sang string
	/// </summary>
	/// <returns>The int.</returns>
	/// <param name="obj">Object.</param>
	public static int ToInt(object obj) {
		return Int32.Parse(obj.ToString());
	}
}

