using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMLimitless.Editor.Attributes
{
    /// <summary>
    /// An attribute for attaching user-displayed descriptions to enumeration values.
    /// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public sealed class EnumValueAttribute : Attribute
	{
        /// <summary>
        /// The value's description, as it appears in the property's tooltip.
        /// </summary>
		public string Description { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumValueAttribute"/> class. 
        /// </summary>
        /// <param name="description">The value's description. This will be presented to the user.</param>
		public EnumValueAttribute(string description)
		{
			if (description == null) { description = ""; }

			Description = description;
		}
	}
}
