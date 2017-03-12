using System;

namespace SMLimitless.Editor.Attributes
{
    /// <summary>
    /// An attribute for properties of type <see cref="int"/>. 
    /// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class IntegerPropertyAttribute : Attribute
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
        /// The maximum allowable value for the property, inclusive.
        /// </summary>
		public int MaxValue { get; }

        /// <summary>
        /// The minimum allowable value for the property, inclusive.
        /// </summary>
		public int MinValue { get; }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="IntegerPropertyAttribute" /> class.
        /// </summary>
        /// <param name="name">
        /// The property's name. This will be presented to the user.
        /// </param>
        /// <param name="description">
        /// The property's description. This will be presented to the user.
        /// </param>
        /// <param name="maxValue">
        /// The maximum allowable value for the property, inclusive.
        /// </param>
        /// <param name="minValue">
        /// The minimum allowable value for the property, inclusive.
        /// </param>
		public IntegerPropertyAttribute(string name, string description, int minValue, int maxValue)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException($"User-editable property was not given a name.");
			}
			if (description == null) { description = ""; }

			Name = name;
			Description = description;
			MinValue = minValue;
			MaxValue = maxValue;
		}
	}
}
