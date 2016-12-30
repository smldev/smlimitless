using System;

namespace SMLimitless.Editor.Attributes
{
	/// <summary>
	///   An attribute that decorates classes or structs that have user-editable properties.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true,
		AllowMultiple = false)]
	public sealed class HasUserEditablePropertiesAttribute : Attribute
	{
	}
}
