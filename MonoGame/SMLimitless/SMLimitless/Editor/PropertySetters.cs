using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

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
				string message = $"The value {value} is too high.\r\nPlease input a number between {minValue} and {maxValue}.";
				MessageBox.Show(message + userAskMessage, "Super Mario Limitless", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return;
			}

			property.SetValue(obj, value);
		}
	}
}
