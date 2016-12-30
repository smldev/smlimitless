using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using SMLimitless.Editor.Attributes;
using SMLimitless.Physics;

using DrawPoint = System.Drawing.Point;

namespace SMLimitless.Editor
{
	internal static class DynamicPropertyControlGenerator
	{
		private const int ButtonSetHeight = 25;
		private const int ButtonSetWidth = 40;
		private const int ControlHeight = 40 - LabelHeightSingleLine;
		private const int DefaultSidePadding = 5;
		private const int GroupControlY = 15;
		private const int GroupHeight = 50;
		private const int LabelHeightSingleLine = 15;
		private const int MaximumGroupLabelLengthInChars = 100;

		private static Type[] validPropertyTypes = new Type[]
			{typeof(bool), typeof(int), typeof(long), typeof(float),
			 typeof(double), typeof(Vector2), typeof(Point),
			 typeof(BoundingRectangle), typeof(Rectangle), typeof(Color), typeof(string)};

		public static void GenerateControls(Panel panel, object obj)
		{
			if (panel == null)
			{
				throw new ArgumentNullException(nameof(panel), "The provided panel was null.");
			}
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj), "The provided object was null.");
			}

			Type objType = obj.GetType();
			var attribute = objType.GetCustomAttribute<HasUserEditablePropertiesAttribute>();

			if (attribute == null)
			{
				string message = $"The type {objType.Name} is not marked as containing user-editable properties.";
				throw new ArgumentException(message, nameof(obj));
			}

			var properties = objType.GetProperties();
			int newControlY = DefaultSidePadding;

			foreach (var property in properties)
			{
				if (IsUserEditablePropertyType(property))
				{
					if (property.PropertyType == typeof(bool))
					{
						if (property.GetCustomAttribute<BooleanPropertyAttribute>() != null)
						{ GenerateBooleanControls(panel, ref newControlY, obj, property); }
					}
					else if (property.PropertyType == typeof(int))
					{
						if (property.GetCustomAttribute<IntegerPropertyAttribute>() != null)
						{ GenerateIntegerControls(panel, ref newControlY, obj, property); }
					}
					else if (property.PropertyType == typeof(long))
					{
						if (property.GetCustomAttribute<LongIntegerPropertyAttribute>() != null)
						{ GenerateLongIntegerControls(panel, ref newControlY, obj, property); }
					}
				}
			}
		}

		private static void GenerateBooleanControls(Panel panel, ref int newControlY, object obj,
			PropertyInfo property)
		{
			var attribute = GetPropertyAttribute<BooleanPropertyAttribute>(obj, property);

			ThrowIfWriteOnlyProperty(property);
			bool isReadonlyProperty = IsReadOnlyProperty(property);

			GroupBox group = GenerateDefaultGroupBox(panel, newControlY);
			group.Text = RestrictStringLength(attribute.Name, MaximumGroupLabelLengthInChars);
			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(group, attribute.Description);
			panel.Controls.Add(group);

			RadioButton radioTrue = new RadioButton();
			radioTrue.AutoSize = true;
			radioTrue.Text = "True";
			radioTrue.Location = new DrawPoint(DefaultSidePadding, GroupControlY);
			radioTrue.Enabled = !isReadonlyProperty;
			radioTrue.Checked = (bool)property.GetValue(obj);
			group.Controls.Add(radioTrue);

			RadioButton radioFalse = new RadioButton();
			radioFalse.AutoSize = true;
			radioFalse.Text = "False";
			radioFalse.Location = new DrawPoint(radioTrue.Right + DefaultSidePadding, GroupControlY);
			radioFalse.Enabled = !isReadonlyProperty;
			radioFalse.Checked = !(bool)property.GetValue(obj);
			group.Controls.Add(radioFalse);

			Button buttonSet = GenerateDefaultSetButton(radioFalse.Right + DefaultSidePadding, !isReadonlyProperty);
			buttonSet.Click += (sender, e) => PropertySetters.SetBooleanProperty(radioTrue, obj, property);
			group.Controls.Add(buttonSet);

			newControlY += group.Height + DefaultSidePadding;
		}

		private static GroupBox GenerateDefaultGroupBox(Panel panel, int newControlY)
		{
			GroupBox group = new GroupBox();
			group.AutoSize = false;
			group.Location = new DrawPoint(DefaultSidePadding, newControlY);
			group.Size = new System.Drawing.Size(panel.Width - (DefaultSidePadding * 2),
				GroupHeight);

			return group;
		}

		private static Button GenerateDefaultSetButton(int locationX, bool enabled)
		{
			Button button = new Button();
			button.Text = "Set";
			button.Location = new DrawPoint(locationX, GroupControlY);
			button.Size = new System.Drawing.Size(ButtonSetWidth, ButtonSetHeight);
			button.Enabled = enabled;

			return button;
		}

		private static void GenerateIntegerControls(Panel panel, ref int newControlY, object obj,
			PropertyInfo property)
		{
			var attribute = GetPropertyAttribute<IntegerPropertyAttribute>(obj, property);

			ThrowIfWriteOnlyProperty(property);
			bool isReadonlyProperty = IsReadOnlyProperty(property);

			GroupBox group = GenerateDefaultGroupBox(panel, newControlY);
			group.Text = attribute.Name;
			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(group, attribute.Description);
			panel.Controls.Add(group);

			NumericUpDown numericUpDown = new NumericUpDown();
			numericUpDown.Minimum = attribute.MinValue;
			numericUpDown.Maximum = attribute.MaxValue;
			numericUpDown.Value = (attribute.MinValue < 0) ? 0 : attribute.MinValue;
			numericUpDown.Location = new DrawPoint(DefaultSidePadding, GroupControlY);
			numericUpDown.Enabled = !isReadonlyProperty;
			group.Controls.Add(numericUpDown);

			Button buttonSet = GenerateDefaultSetButton(numericUpDown.Right + DefaultSidePadding, !isReadonlyProperty);
			buttonSet.Click += (sender, e) => PropertySetters.SetIntegerProperty(numericUpDown, obj, property);
			group.Controls.Add(buttonSet);

			newControlY += group.Height + DefaultSidePadding;
		}

		private static void GenerateLongIntegerControls(Panel panel, ref int newControlY, object obj,
			PropertyInfo property)
		{
			var attribute = GetPropertyAttribute<LongIntegerPropertyAttribute>(obj, property);

			ThrowIfWriteOnlyProperty(property);
			bool isReadonlyProperty = IsReadOnlyProperty(property);

			GroupBox group = GenerateDefaultGroupBox(panel, newControlY);
			group.Text = attribute.Name;
			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(group, attribute.Description);
			panel.Controls.Add(group);

			TextBox textBox = new TextBox();
			textBox.Text = ((long)property.GetValue(obj)).ToString();
			textBox.Location = new DrawPoint(DefaultSidePadding, GroupControlY);
			textBox.Size = new System.Drawing.Size(200, ButtonSetHeight);
			textBox.Enabled = !isReadonlyProperty;
			group.Controls.Add(textBox);

			Button buttonSet = GenerateDefaultSetButton(textBox.Right + DefaultSidePadding, !isReadonlyProperty);
			buttonSet.Click += (sender, e) =>
			{
				PropertySetters.SetLongIntegerProperty(textBox, obj, property, attribute.MinValue, attribute.MaxValue);
			};
			group.Controls.Add(buttonSet);
		}

		private static T GetPropertyAttribute<T>(object obj, PropertyInfo property) where T : Attribute
		{
			var attribute = property.GetCustomAttribute<T>();

			if (attribute == null)
			{
				var message = $"The {obj.GetType().Name}.{property.Name} property doesn't have an attribute of type {typeof(T).Name}.";
				throw new ArgumentException(message);
			}

			return attribute;
		}

		private static bool IsReadOnlyProperty(PropertyInfo info) => (info.GetSetMethod() == null);

		private static bool IsUserEditablePropertyType(PropertyInfo property)
		{
			var propertyType = property.PropertyType;
			bool isNestedProperty = property.GetCustomAttribute<NestedPropertyAttribute>() != null;

			return validPropertyTypes.Contains(propertyType) || isNestedProperty;
		}

		private static PropertyInfo ReflectPropertyInfo(object objWithProperty, string propertyName)
		{
			Type objType = objWithProperty.GetType();
			var properties = objType.GetProperties();
			var property = properties.FirstOrDefault(p => p.Name == propertyName);

			if (property == null)
			{
				throw new ArgumentException($"The type {objType.Name} doesn't have a property named {propertyName}.");
			}

			return property;
		}

		private static string RestrictStringLength(string str, int maxLength)
		{
			if (str == null) { return ""; }
			if (str.Length < maxLength) { return str; }
			if (maxLength < 3)
			{
				throw new ArgumentException($"The maximum length of a string has to be at least 3.");
			}

			string substring = str.Substring(0, maxLength - 3);
			return substring + "...";
		}

		private static void ThrowIfWriteOnlyProperty(PropertyInfo info)
		{
			if (info.GetGetMethod() == null && info.GetSetMethod() != null)
			{
				throw new ArgumentException("User-editable properties cannot be write-only.");
			}
		}
	}
}
