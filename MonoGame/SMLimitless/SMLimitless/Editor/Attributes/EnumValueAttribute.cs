using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMLimitless.Editor.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public sealed class EnumValueAttribute : Attribute
	{
		public string Description { get; }

		public EnumValueAttribute(string description)
		{
			if (description == null) { description = ""; }

			Description = description;
		}
	}
}
