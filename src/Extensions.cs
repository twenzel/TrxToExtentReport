using System.Reflection;

namespace TrxToExtentReport;

internal static class Extensions
{
	/// <summary>
	/// Gets a private field of a type with a selector as Func(Of FieldInfo, Boolean)
	/// This call is recursive. If the Type does not contain a field which can found by the given Selector,
	/// all the base types will be checked, too.
	/// </summary>
	/// <param name="type">The type to get the field from</param>
	/// <param name="fieldName">The Field's name</param>
	/// <returns>A private field of the type</returns>
	/// <remarks>RECURSIVE CALL TO ALL BASETYPES IF NEEDED</remarks>
	public static FieldInfo? GetPrivateField(this Type type, string fieldName)
	{
		// select the type with the selector-func
		var fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

		// IMPORTANT:
		// this is recursive - check if we are up to the type object.
		// if so, recursion is not allowed anymore
		if (fieldInfo != null || type.Equals(typeof(object)))
			return fieldInfo;

		// try the same with the basetype
		if (type.BaseType != null)
			return type.BaseType.GetPrivateField(fieldName);

		return null;
	}
}
