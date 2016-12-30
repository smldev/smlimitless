using System;

namespace SMLimitless.Editor.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class FloatingPointPropertyAttribute : Attribute
	{
		public string Description { get; }
		public float MaxValue { get; }
		public float MinValue { get; }
		public string Name { get; }

		/// <summary>
		///   Initializes a new instance of the <see
		///   cref="FloatingPointPropertyAttribute" /> class.
		/// </summary>
		/// <param name="name">
		///   The display name of the property as it appears to the user.
		/// </param>
		/// <param name="description">
		///   A description of the property as it appears to the user.
		/// </param>
		/// <param name="minValue">
		///   The lowest legal value for this property. Set to <see
		///   cref="float.NegativeInfinity" /> for no minimum value.
		/// </param>
		/// <param name="maxValue">
		///   The highest legal value for this property. Set to <see
		///   cref="float.PositiveInfinity" /> for no maximum value.
		/// </param>
		public FloatingPointPropertyAttribute(string name, string description,
			float minValue, float maxValue)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException($"User-editable property was not given a name.");
			}
			if (description == null) { description = ""; }
			if (float.IsNaN(minValue) || float.IsNaN(maxValue))
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
