using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless.Debug
{
	/// <summary>
	/// Signifies that a member is used for debugging purposes.
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public sealed class DebugAttribute : Attribute
	{
	}
}
