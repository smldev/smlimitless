using System;

namespace SMLimitless.Debug
{
	/// <summary>
	///   Signifies that a member is used for debugging purposes.
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public sealed class DebugAttribute : Attribute
	{
	}
}
