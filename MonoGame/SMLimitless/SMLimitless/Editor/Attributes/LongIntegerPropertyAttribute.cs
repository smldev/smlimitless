using System;

namespace SMLimitless.Editor.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class LongIntegerPropertyAttribute : Attribute
	{
		public string Description { get; }
		public long MaxValue { get; }
		public long MinValue { get; }
		public string Name { get; }

		public LongIntegerPropertyAttribute(string name, string description, long minValue,
			long maxValue)
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
