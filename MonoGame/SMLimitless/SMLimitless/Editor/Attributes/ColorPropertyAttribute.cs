using System;

namespace SMLimitless.Editor.Attributes
{
    /// <summary>
    /// An attribute for properties of type <see cref="Microsoft.Xna.Framework.Color"/>. 
    /// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ColorPropertyAttribute : Attribute
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
        /// cref="ColorPropertyAttribute" /> class.
        /// </summary>
        /// <param name="name">
        /// The property's name. This will be presented to the user.
        /// </param>
        /// <param name="description">
        /// The property's description. This will be presented to the user.
        /// </param>
		public ColorPropertyAttribute(string name, string description)
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
