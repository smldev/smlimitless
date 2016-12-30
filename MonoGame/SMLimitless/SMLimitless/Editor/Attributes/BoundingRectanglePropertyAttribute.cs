using System;

namespace SMLimitless.Editor.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class BoundingRectanglePropertyAttribute : Attribute
	{
		public string Description { get; }
		public string Name { get; }

		public BoundingRectanglePropertyAttribute(string name, string description)
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
