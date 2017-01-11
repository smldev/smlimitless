using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using SMLimitless.Editor.Attributes;

namespace SMLimitless.Editor
{
	internal static class PropertySetters
	{
		public static void SetBooleanProperty(RadioButton radioTrue, object obj, PropertyInfo property)
		{
			property.SetValue(obj, radioTrue.Checked);
		}

		public static void SetIntegerProperty(NumericUpDown numericUpDown, object obj, PropertyInfo property)
		{
			int value = (int)numericUpDown.Value;
			property.SetValue(obj, value);
		}

		public static void SetLongIntegerProperty(TextBox textBox, object obj, PropertyInfo property, long minValue,
			long maxValue)
		{
			long value = 0L;
			var numberStyles = NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands;
			bool parseSucceeded = long.TryParse(textBox.Text, numberStyles, CultureInfo.CurrentCulture, out value);
			string userAskMessage = $"Please input a number between {minValue} and {maxValue}.";

			if (!parseSucceeded)
			{
				string message = $"The value \"{textBox.Text}\" is not valid.\r\n\r\n";
				MessageBox.Show(message + userAskMessage, "Super Mario Limitless", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return;
			}

			if (value < minValue)
			{
				string message = $"The value {value} is too low.\r\n\r\n";
				MessageBox.Show(message + userAskMessage, "Super Mario Limitless", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return;
			}
			else if (value > maxValue)
			{
				string message = $"The value {value} is too high.\r\n\r\n";
				MessageBox.Show(message + userAskMessage, "Super Mario Limitless", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return;
			}

			property.SetValue(obj, value);
		}

		public static void SetFloatingPointProperty(TextBox textBox, object obj, PropertyInfo property,
			FloatingPointPropertyAttribute attribute)
		{
			float value = 0f;
			var numberStyles =  NumberStyles.Float;
			bool parseSucceeded = float.TryParse(textBox.Text, numberStyles, CultureInfo.CurrentCulture, out value);
			string userAskMessage = $"Please input a number between {attribute.MinValue} and {attribute.MaxValue}.";

			if (!parseSucceeded)
			{
				string message = $"The value \"{textBox.Text}\" is not valid.\r\n\r\n";
				MessageBox.Show(message + userAskMessage, "Super Mario Limitless", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return;
			}

			if (value < attribute.MinValue)
			{
				string message = $"The value {value} is too low.\r\n\r\n";
				MessageBox.Show(message + userAskMessage, "Super Mario Limitless", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return;
			}
			else if (value > attribute.MaxValue)
			{
				string message = $"The value {value} is too high.\r\n\r\n";
				MessageBox.Show(message + userAskMessage, "Super Mario Limitless", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return;
			}

			property.SetValue(obj, value);
		}

		public static void SetDoubleProperty(TextBox textBox, object obj, PropertyInfo property,
			DoublePropertyAttribute attribute)
		{
			double value = 0d;
			var numberStyles = NumberStyles.Float;
			bool parseSucceeded = double.TryParse(textBox.Text, numberStyles, CultureInfo.CurrentCulture, out value);
			string userAskMessage = $"Please input a number between {attribute.MinValue} and {attribute.MaxValue}.";

			if (!parseSucceeded)
			{
				string message = $"The value \"{textBox.Text}\" is not valid.\r\n\r\n";
				MessageBox.Show(message + userAskMessage, "Super Mario Limitless", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return;
			}

			if (value < attribute.MinValue)
			{
				string message = $"The value {value} is too low.\r\n\r\n";
				MessageBox.Show(message + userAskMessage, "Super Mario Limitless", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return;
			}
			else if (value > attribute.MaxValue)
			{
				string message = $"The value {value} is too high.\r\n\r\n";
				MessageBox.Show(message + userAskMessage, "Super Mario Limitless", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return;
			}

			property.SetValue(obj, value);
		}

		public static void SetVector2Property(TextBox textX, TextBox textY, object obj, PropertyInfo property)
		{
			float x = 0f;
			float y = 0f;
			var numberStyles = NumberStyles.Float;
			bool parseXSucceeded = float.TryParse(textX.Text, numberStyles, CultureInfo.CurrentCulture, out x);
			bool parseYSucceeded = float.TryParse(textY.Text, numberStyles, CultureInfo.CurrentCulture, out y);

			if (!parseXSucceeded || !parseYSucceeded)
			{
				string message = $"The values \"{textX.Text}\" and/or \"{textY.Text}\" are not valid.\r\nPlease input two numbers.";
				MessageBox.Show(message, "Super Mario Limitless", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			property.SetValue(obj, new Vector2(x, y));
		}

		public static void SetPointProperty(TextBox textX, TextBox textY, object obj, PropertyInfo property)
		{
			int x = 0;
			int y = 0;
			var numberStyles = NumberStyles.Integer;
			bool parseXSucceeded = int.TryParse(textX.Text, numberStyles, CultureInfo.CurrentCulture, out x);
			bool parseYSucceeded = int.TryParse(textY.Text, numberStyles, CultureInfo.CurrentCulture, out y);

			if (!parseXSucceeded || !parseYSucceeded)
			{
				string message = $"The values \"{textX.Text}\" and/or \"{textY.Text}\" are not valid.\r\nPlease input two numbers.";
				MessageBox.Show(message, "Super Mario Limitless", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			property.SetValue(obj, new Point(x, y));
		}
	}
}
