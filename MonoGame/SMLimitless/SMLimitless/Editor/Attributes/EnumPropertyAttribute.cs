using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMLimitless.Editor.Attributes
{
    /// <summary>
    /// An attribute for enumerated properties.
    /// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class EnumPropertyAttribute : Attribute
	{
        /// <summary>
        /// The property's description, as it appears in the property's tooltip.
        /// </summary>
		public string Description { get; }

        /// <summary>
        /// The property's name, as it appears in the property panel.
        /// </summary>
		public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="EnumPropertyAttribute" /> class.
        /// </summary>
        /// <param name="name">
        /// The property's name. This will be presented to the user.
        /// </param>
        /// <param name="description">
        /// The property's description. This will be presented to the user.
        /// </param>
		public EnumPropertyAttribute(string name, string description)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException($"User-editable property was not given a name.");
			}
			if (description == null) { description = ""; }

			Name = name;
			Description = description;
		}
	}
}
