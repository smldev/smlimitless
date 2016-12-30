using System;

namespace SMLimitless.Editor.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class IntegerPropertyAttribute : Attribute
	{
		public string Description { get; }
		public int MaxValue { get; }
		public int MinValue { get; }
		public string Name { get; }

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
