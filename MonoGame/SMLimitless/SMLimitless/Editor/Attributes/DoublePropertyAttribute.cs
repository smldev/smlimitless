using System;

namespace SMLimitless.Editor.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class DoublePropertyAttribute : Attribute
	{
		public string Description { get; }
		public double MaxValue { get; }
		public double MinValue { get; }
		public string Name { get; }

		/// <summary>
		///   Initializes a new instance of the <see
		///   cref="DoublePropertyAttribute" /> class.
		/// </summary>
		/// <param name="name">
		///   The display name of the property as it appears to the user.
		/// </param>
		/// <param name="description">
		///   A description of the property as it appears to the user.
		/// </param>
		/// <param name="minValue">
		///   The lowest legal value for this property. Set to <see
		///   cref="double.NegativeInfinity" /> for no minimum value.
		/// </param>
		/// <param name="maxValue">
		///   The highest legal value for this property. Set to <see
		///   cref="double.PositiveInfinity" /> for no maximum value.
		/// </param>
		public DoublePropertyAttribute(string name, string description,
			double minValue, double maxValue)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException($"User-editable property was not given a name.");
			}
			if (description == null) { description = ""; }
			if (double.IsNaN(minValue) || double.IsNaN(maxValue))
			{
				throw new ArgumentException("The minimum or maximum value was not a number.");
			}

			Name = name;
			Description = description;
			MinValue = minValue;
			MaxValue = maxValue;
		}
	}
}
