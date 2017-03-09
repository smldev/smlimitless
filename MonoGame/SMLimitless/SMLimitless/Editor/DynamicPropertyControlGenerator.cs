using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using SMLimitless.Editor.Attributes;
using SMLimitless.Physics;

using DrawPoint = System.Drawing.Point;
using DrawSize = System.Drawing.Size;

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
			 typeof(BoundingRectangle), typeof(Rectangle), typeof(Color),
			 typeof(string), typeof(Enum)};

		private static Dictionary<Type, IDictionary<object, string>> allEnumDescriptions = 
			new Dictionary<Type, IDictionary<object, string>>();

		public static void GenerateControls(Panel panel, object obj)
		{
			if (panel == null)
			{
				throw new ArgumentNullException(nameof(panel), "The provided panel was null.");
			}
			if (obj == null)
			{
				// Sometimes the EditorSelectedObject hasn't selected anything,
				// so we need to just show nothing here
				return;
			}

			panel.Controls.Clear();

			Type objType = obj.GetType();
			var attribute = objType.GetCustomAttribute<HasUserEditablePropertiesAttribute>();

			if (attribute == null)
			{
                return;
			}

			var properties = objType.GetProperties();
			int newControlY = DefaultSidePadding;

			Label labelTypeName = new Label();
			labelTypeName.AutoSize = true;
			labelTypeName.Text = objType.Name;
			labelTypeName.Location = new DrawPoint(DefaultSidePadding, newControlY);
			panel.Controls.Add(labelTypeName);
			newControlY += labelTypeName.Height + DefaultSidePadding;

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
					else if (property.PropertyType == typeof(float))
					{
						if (property.GetCustomAttribute<FloatingPointPropertyAttribute>() != null)
						{ GenerateFloatingPointControls(panel, ref newControlY, obj, property); }
					}
					else if (property.PropertyType == typeof(double))
					{
						if (property.GetCustomAttribute<DoublePropertyAttribute>() != null)
						{ GenerateDoubleControls(panel, ref newControlY, obj, property); }
					}
					else if (property.PropertyType == typeof(Vector2))
					{
						if (property.GetCustomAttribute<Vector2PropertyAttribute>() != null)
						{ GenerateVector2Controls(panel, ref newControlY, obj, property); }
					}
					else if (property.PropertyType == typeof(Point))
					{
						if (property.GetCustomAttribute<PointPropertyAttribute>() != null)
						{ GeneratePointControls(panel, ref newControlY, obj, property); }
					}
					else if (property.PropertyType == typeof(BoundingRectangle))
					{
						if (property.GetCustomAttribute<BoundingRectanglePropertyAttribute>() != null)
						{ GenerateBoudingRectangleControls(panel, ref newControlY, obj, property); }
					}
					else if (property.PropertyType == typeof(Rectangle))
					{
						if (property.GetCustomAttribute<RectanglePropertyAttribute>() != null)
						{ GenerateRectangleControls(panel, ref newControlY, obj, property); }
					}
					else if (property.PropertyType == typeof(Color))
					{
						if (property.GetCustomAttribute<ColorPropertyAttribute>() != null)
						{ GenerateColorControls(panel, ref newControlY, obj, property); }
					}
					else if (property.PropertyType == typeof(string))
					{
						if (property.GetCustomAttribute<StringPropertyAttribute>() != null)
						{ GenerateStringControls(panel, ref newControlY, obj, property); }
					}
					else if (property.PropertyType.IsEnum)
					{
						if (property.GetCustomAttribute<EnumPropertyAttribute>() != null)
						{ GenerateEnumControls(panel, ref newControlY, obj, property); }
					}
					else if (property.GetCustomAttribute<NestedPropertyAttribute>() != null)
					{
						GeneratedNestedPropertyControls(panel, ref newControlY, obj, property);
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
			group.Size = new DrawSize(panel.Width - (DefaultSidePadding * 2),
				GroupHeight);

			return group;
		}

		private static Button GenerateDefaultSetButton(int locationX, bool enabled)
		{
			Button button = new Button();
			button.Text = "Set";
			button.Location = new DrawPoint(locationX, GroupControlY);
			button.Size = new DrawSize(ButtonSetWidth, ButtonSetHeight);
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
			textBox.Size = new DrawSize(200, ButtonSetHeight);
			textBox.Enabled = !isReadonlyProperty;
			group.Controls.Add(textBox);

			Button buttonSet = GenerateDefaultSetButton(textBox.Right + DefaultSidePadding, !isReadonlyProperty);
			buttonSet.Click += (sender, e) =>
			{
				PropertySetters.SetLongIntegerProperty(textBox, obj, property, attribute.MinValue, attribute.MaxValue);
			};
			group.Controls.Add(buttonSet);

			newControlY += group.Height + DefaultSidePadding;
		}

		private static void GenerateFloatingPointControls(Panel panel, ref int newControlY, object obj,
			PropertyInfo property)
		{
			var attribute = GetPropertyAttribute<FloatingPointPropertyAttribute>(obj, property);

			ThrowIfWriteOnlyProperty(property);
			bool isReadonlyProperty = IsReadOnlyProperty(property);

			GroupBox group = GenerateDefaultGroupBox(panel, newControlY);
			group.Text = attribute.Name;
			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(group, attribute.Description);
			panel.Controls.Add(group);

			TextBox textBox = new TextBox();
			textBox.Text = ((float)property.GetValue(obj)).ToString();
			textBox.Location = new DrawPoint(DefaultSidePadding, GroupControlY);
			textBox.Size = new DrawSize(200, ButtonSetHeight);
			textBox.Enabled = !isReadonlyProperty;
			group.Controls.Add(textBox);

			Button buttonSet = GenerateDefaultSetButton(textBox.Right + DefaultSidePadding, !isReadonlyProperty);
			buttonSet.Click += (sender, e) =>
			{
				PropertySetters.SetFloatingPointProperty(textBox, obj, property, attribute);
			};
			group.Controls.Add(buttonSet);

			newControlY += group.Height + DefaultSidePadding;
		}

		private static void GenerateDoubleControls(Panel panel, ref int newControlY, object obj, PropertyInfo property)
		{
			var attribute = GetPropertyAttribute<DoublePropertyAttribute>(obj, property);

			ThrowIfWriteOnlyProperty(property);
			bool isReadonlyProperty = IsReadOnlyProperty(property);

			GroupBox group = GenerateDefaultGroupBox(panel, newControlY);
			group.Text = attribute.Name;
			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(group, attribute.Description);
			panel.Controls.Add(group);

			TextBox textBox = new TextBox();
			textBox.Text = ((double)property.GetValue(obj)).ToString();
			textBox.Location = new DrawPoint(DefaultSidePadding, GroupControlY);
			textBox.Size = new DrawSize(200, ButtonSetHeight);
			textBox.Enabled = !isReadonlyProperty;
			group.Controls.Add(textBox);

			Button buttonSet = GenerateDefaultSetButton(textBox.Right + DefaultSidePadding, !isReadonlyProperty);
			buttonSet.Click += (sender, e) =>
			{
				PropertySetters.SetDoubleProperty(textBox, obj, property, attribute);
			};
			group.Controls.Add(buttonSet);

			newControlY += group.Height + DefaultSidePadding;
		}

		private static void GenerateVector2Controls(Panel panel, ref int newControlY, object obj, 
			PropertyInfo property)
		{
			var attribute = GetPropertyAttribute<Vector2PropertyAttribute>(obj, property);

			ThrowIfWriteOnlyProperty(property);
			bool isReadonlyProperty = IsReadOnlyProperty(property);

			GroupBox group = GenerateDefaultGroupBox(panel, newControlY);
			group.Text = attribute.Name;
			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(group, attribute.Description);
			panel.Controls.Add(group);

			int controlX = DefaultSidePadding;

			Label labelX = new Label();
			labelX.Text = "X";
			labelX.AutoSize = true;
			labelX.Location = new DrawPoint(controlX, GroupControlY);
			labelX.Enabled = !isReadonlyProperty;
			group.Controls.Add(labelX);
			controlX += labelX.Width + DefaultSidePadding;

			TextBox textX = new TextBox();
			textX.Text = (((Vector2)property.GetValue(obj)).X).ToString();
			textX.Location = new DrawPoint(controlX, GroupControlY);
			textX.Size = new DrawSize(80, ButtonSetHeight);
			textX.Enabled = !isReadonlyProperty;
			group.Controls.Add(textX);
			controlX += textX.Width + DefaultSidePadding;

			Label labelY = new Label();
			labelY.Text = "Y";
			labelY.AutoSize = true;
			labelY.Location = new DrawPoint(controlX, GroupControlY);
			labelY.Enabled = !isReadonlyProperty;
			group.Controls.Add(labelY);
			controlX += labelY.Width + DefaultSidePadding;

			TextBox textY = new TextBox();
			textY.Text = (((Vector2)property.GetValue(obj)).Y).ToString();
			textY.Location = new DrawPoint(controlX, GroupControlY);
			textY.Size = new DrawSize(80, ButtonSetHeight);
			textY.Enabled = !isReadonlyProperty;
			group.Controls.Add(textY);
			controlX += textY.Width + DefaultSidePadding;

			Button buttonSet = GenerateDefaultSetButton(controlX, !isReadonlyProperty);
			buttonSet.Click += (sender, e) =>
			{
				PropertySetters.SetVector2Property(textX, textY, obj, property);
			};
			group.Controls.Add(buttonSet);

			newControlY += group.Height + DefaultSidePadding;
		}

		private static void GeneratePointControls(Panel panel, ref int newControlY, object obj,
			PropertyInfo property)
		{
			var attribute = GetPropertyAttribute<PointPropertyAttribute>(obj, property);

			ThrowIfWriteOnlyProperty(property);
			bool isReadonlyProperty = IsReadOnlyProperty(property);

			GroupBox group = GenerateDefaultGroupBox(panel, newControlY);
			group.Text = attribute.Name;
			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(group, attribute.Description);
			panel.Controls.Add(group);

			int controlX = DefaultSidePadding;

			Label labelX = new Label();
			labelX.Text = "X";
			labelX.AutoSize = true;
			labelX.Location = new DrawPoint(controlX, GroupControlY);
			labelX.Enabled = !isReadonlyProperty;
			group.Controls.Add(labelX);
			controlX += labelX.Width + DefaultSidePadding;

			TextBox textX = new TextBox();
			textX.Text = (((Vector2)property.GetValue(obj)).X).ToString();
			textX.Location = new DrawPoint(controlX, GroupControlY);
			textX.Size = new DrawSize(80, ButtonSetHeight);
			textX.Enabled = !isReadonlyProperty;
			group.Controls.Add(textX);
			controlX += textX.Width + DefaultSidePadding;

			Label labelY = new Label();
			labelY.Text = "Y";
			labelY.AutoSize = true;
			labelY.Location = new DrawPoint(controlX, GroupControlY);
			labelY.Enabled = !isReadonlyProperty;
			group.Controls.Add(labelY);
			controlX += labelY.Width + DefaultSidePadding;

			TextBox textY = new TextBox();
			textY.Text = (((Vector2)property.GetValue(obj)).Y).ToString();
			textY.Location = new DrawPoint(controlX, GroupControlY);
			textY.Size = new DrawSize(80, ButtonSetHeight);
			textY.Enabled = !isReadonlyProperty;
			group.Controls.Add(textY);
			controlX += textY.Width + DefaultSidePadding;

			Button buttonSet = GenerateDefaultSetButton(controlX, !isReadonlyProperty);
			buttonSet.Click += (sender, e) =>
			{
				PropertySetters.SetPointProperty(textX, textY, obj, property);
			};
			group.Controls.Add(buttonSet);

			newControlY += group.Height + DefaultSidePadding;
		}

		private static void GenerateBoudingRectangleControls(Panel panel, ref int newControlY, object obj,
			PropertyInfo property)
		{
			var attribute = GetPropertyAttribute<BoundingRectanglePropertyAttribute>(obj, property);

			ThrowIfWriteOnlyProperty(property);
			bool isReadonlyProperty = IsReadOnlyProperty(property);

			GroupBox group = GenerateDefaultGroupBox(panel, newControlY);
			group.Text = attribute.Name;
			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(group, attribute.Description);

			int controlX = DefaultSidePadding;

			Label labelX, labelY, labelWidth, labelHeight;
			labelX = new Label();
			labelY = new Label();
			labelWidth = new Label();
			labelHeight = new Label();

			labelX.AutoSize = labelY.AutoSize = labelWidth.AutoSize = labelHeight.AutoSize = true;
			labelX.Enabled = labelY.Enabled = labelWidth.Enabled = labelHeight.Enabled = !isReadonlyProperty;

			labelX.Text = "X";
			labelY.Text = "Y";
			labelWidth.Text = "W";
			labelHeight.Text = "H";

			group.Controls.AddRange(new Label[] { labelX, labelY, labelWidth, labelHeight });

			TextBox textX, textY, textWidth, textHeight;
			textX = new TextBox();
			textY = new TextBox();
			textWidth = new TextBox();
			textHeight = new TextBox();

			BoundingRectangle value = (BoundingRectangle)property.GetValue(obj);
			textX.Text = value.X.ToString();
			textY.Text = value.Y.ToString();
			textWidth.Text = value.Width.ToString();
			textHeight.Text = value.Height.ToString();

			group.Controls.AddRange(new TextBox[] { textX, textY, textWidth, textHeight });

			textX.Enabled = textY.Enabled = textWidth.Enabled = textHeight.Enabled = !isReadonlyProperty;
			textX.Size = textY.Size = textWidth.Size = textHeight.Size = new DrawSize(80, ButtonSetHeight);

			// There are three rows of controls for rectangles:
			// Row 1: labelX, textX, labelY, textY
			// Row 2: labelWidth, textWidth, labelHeight, textHeight
			// Row 3: buttonSet
			labelX.Location = new DrawPoint(controlX, GroupControlY);
			textX.Location = new DrawPoint(labelX.Right + DefaultSidePadding, GroupControlY);
			labelY.Location = new DrawPoint(textX.Right + DefaultSidePadding, GroupControlY);
			textY.Location = new DrawPoint(labelY.Right + DefaultSidePadding, GroupControlY);

			int row2Y = textX.Bottom + DefaultSidePadding;
			labelWidth.Location = new DrawPoint(controlX, row2Y);
			textWidth.Location = new DrawPoint(labelWidth.Right + DefaultSidePadding, row2Y);
			labelHeight.Location = new DrawPoint(textWidth.Right + DefaultSidePadding, row2Y);
			textHeight.Location = new DrawPoint(labelHeight.Right + DefaultSidePadding, row2Y);

			int row3Y = textWidth.Bottom + DefaultSidePadding;
			Button buttonSet = GenerateDefaultSetButton(controlX, !isReadonlyProperty);
			buttonSet.Size = new DrawSize(labelX.Width + (DefaultSidePadding * 3) + textX.Width + labelY.Width 
				+ textY.Width, ButtonSetHeight);
			buttonSet.Location = new DrawPoint(DefaultSidePadding, row3Y);
			buttonSet.Click += (sender, e) =>
			{
				PropertySetters.SetBoundingRectangleProperty(textX, textY, textWidth, textHeight, obj, property);
			};

			group.Controls.Add(buttonSet);
			group.Size = new DrawSize(buttonSet.Right + DefaultSidePadding, buttonSet.Bottom + DefaultSidePadding);
			panel.Controls.Add(group);

			newControlY += group.Height + DefaultSidePadding;
		}

		private static void GenerateRectangleControls(Panel panel, ref int newControlY, object obj,
			PropertyInfo property)
		{
			var attribute = GetPropertyAttribute<RectanglePropertyAttribute>(obj, property);

			ThrowIfWriteOnlyProperty(property);
			bool isReadonlyProperty = IsReadOnlyProperty(property);

			GroupBox group = GenerateDefaultGroupBox(panel, newControlY);
			group.Text = attribute.Name;
			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(group, attribute.Description);

			int controlX = DefaultSidePadding;

			Label labelX, labelY, labelWidth, labelHeight;
			labelX = new Label();
			labelY = new Label();
			labelWidth = new Label();
			labelHeight = new Label();

			labelX.AutoSize = labelY.AutoSize = labelWidth.AutoSize = labelHeight.AutoSize = true;
			labelX.Enabled = labelY.Enabled = labelWidth.Enabled = labelHeight.Enabled = !isReadonlyProperty;

			labelX.Text = "X";
			labelY.Text = "Y";
			labelWidth.Text = "W";
			labelHeight.Text = "H";

			group.Controls.AddRange(new Label[] { labelX, labelY, labelWidth, labelHeight });

			TextBox textX, textY, textWidth, textHeight;
			textX = new TextBox();
			textY = new TextBox();
			textWidth = new TextBox();
			textHeight = new TextBox();

			Rectangle value = (Rectangle)property.GetValue(obj);
			textX.Text = value.X.ToString();
			textY.Text = value.Y.ToString();
			textWidth.Text = value.Width.ToString();
			textHeight.Text = value.Height.ToString();

			group.Controls.AddRange(new TextBox[] { textX, textY, textWidth, textHeight });

			textX.Enabled = textY.Enabled = textWidth.Enabled = textHeight.Enabled = !isReadonlyProperty;
			textX.Size = textY.Size = textWidth.Size = textHeight.Size = new DrawSize(80, ButtonSetHeight);

			// There are three rows of controls for rectangles:
			// Row 1: labelX, textX, labelY, textY
			// Row 2: labelWidth, textWidth, labelHeight, textHeight
			// Row 3: buttonSet
			labelX.Location = new DrawPoint(controlX, GroupControlY);
			textX.Location = new DrawPoint(labelX.Right + DefaultSidePadding, GroupControlY);
			labelY.Location = new DrawPoint(textX.Right + DefaultSidePadding, GroupControlY);
			textY.Location = new DrawPoint(labelY.Right + DefaultSidePadding, GroupControlY);

			int row2Y = textX.Bottom + DefaultSidePadding;
			labelWidth.Location = new DrawPoint(controlX, row2Y);
			textWidth.Location = new DrawPoint(labelWidth.Right + DefaultSidePadding, row2Y);
			labelHeight.Location = new DrawPoint(textWidth.Right + DefaultSidePadding, row2Y);
			textHeight.Location = new DrawPoint(labelHeight.Right + DefaultSidePadding, row2Y);

			int row3Y = textWidth.Bottom + DefaultSidePadding;
			Button buttonSet = GenerateDefaultSetButton(controlX, !isReadonlyProperty);
			buttonSet.Size = new DrawSize(labelX.Width + (DefaultSidePadding * 3) + textX.Width + labelY.Width
				+ textY.Width, ButtonSetHeight);
			buttonSet.Location = new DrawPoint(DefaultSidePadding, row3Y);
			buttonSet.Click += (sender, e) =>
			{
				PropertySetters.SetRectangleProperty(textX, textY, textWidth, textHeight, obj, property);
			};

			group.Controls.Add(buttonSet);
			group.Size = new DrawSize(buttonSet.Right + DefaultSidePadding, buttonSet.Bottom + DefaultSidePadding);
			panel.Controls.Add(group);

			newControlY += group.Height + DefaultSidePadding;
		}

		private static void GenerateColorControls(Panel panel, ref int newControlY, object obj,
			PropertyInfo property)
		{
			var attribute = property.GetCustomAttribute<ColorPropertyAttribute>();

			ThrowIfWriteOnlyProperty(property);
			bool isReadonlyProperty = IsReadOnlyProperty(property);

			GroupBox group = GenerateDefaultGroupBox(panel, newControlY);
			group.Text = attribute.Name;
			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(group, attribute.Description);
			panel.Controls.Add(group);

			Label labelR, labelG, labelB, labelA;
			labelR = new Label();
			labelG = new Label();
			labelB = new Label();
			labelA = new Label();

			labelR.AutoSize = labelG.AutoSize = labelB.AutoSize = labelA.AutoSize = true;
			labelR.Enabled = labelG.Enabled = labelB.Enabled = labelA.Enabled = !isReadonlyProperty;

			labelR.Text = "R";
			labelG.Text = "G";
			labelB.Text = "B";
			labelA.Text = "A";

			NumericUpDown nudR, nudG, nudB, nudA;
			nudR = new NumericUpDown();
			nudG = new NumericUpDown();
			nudB = new NumericUpDown();
			nudA = new NumericUpDown();

			nudR.Enabled = nudG.Enabled = nudB.Enabled = nudA.Enabled = !isReadonlyProperty;
			nudR.Minimum = nudG.Minimum = nudB.Minimum = nudA.Minimum = 0;
			nudR.Maximum = nudG.Maximum = nudB.Maximum = nudA.Maximum = 255;

			Color value = (Color)property.GetValue(obj);
			nudR.Value = value.R;
			nudG.Value = value.G;
			nudB.Value = value.B;
			nudA.Value = value.A;

			Panel panelColorPreview = new Panel();
			panelColorPreview.BorderStyle = BorderStyle.Fixed3D;
			panelColorPreview.BackColor = System.Drawing.Color.FromArgb(value.A, value.R, value.G, value.B);

			Button buttonSet = GenerateDefaultSetButton(DefaultSidePadding, !isReadonlyProperty);
			buttonSet.Click += (sender, e) =>
			{
				var color = PropertySetters.SetColorProperty(nudR, nudG, nudB, nudA, obj, property);
				panelColorPreview.BackColor = color;
			};

			group.Controls.AddRange(new Control[] { labelR, labelG, labelB, labelA, nudR, nudG, nudB, nudA, buttonSet,
			panelColorPreview});

			// Row 1: labelR, nudR, labelG, nudG, labelB, nudB, labelA, nudA
			// Row 2: buttonSet, panelColorPreview
			var nudSize = new DrawSize(45, ButtonSetHeight);
			nudR.Size = nudG.Size = nudB.Size = nudA.Size = nudSize;

			labelR.Location = new DrawPoint(DefaultSidePadding, GroupControlY);
			nudR.Location = new DrawPoint(labelR.Right + DefaultSidePadding, GroupControlY);
			labelG.Location = new DrawPoint(nudR.Right + DefaultSidePadding, GroupControlY);
			nudG.Location = new DrawPoint(labelG.Right + DefaultSidePadding, GroupControlY);
			labelB.Location = new DrawPoint(nudG.Right + DefaultSidePadding, GroupControlY);
			nudB.Location = new DrawPoint(labelB.Right + DefaultSidePadding, GroupControlY);
			labelA.Location = new DrawPoint(nudB.Right + DefaultSidePadding, GroupControlY);
			nudA.Location = new DrawPoint(labelA.Right + DefaultSidePadding, GroupControlY);

			int row2Y = nudA.Bottom + DefaultSidePadding;
			buttonSet.Size = new DrawSize(nudB.Right - labelR.Left, ButtonSetHeight);
			buttonSet.Location = new DrawPoint(DefaultSidePadding, row2Y);

			panelColorPreview.Size = new DrawSize(nudA.Right - labelA.Left, ButtonSetHeight);
			panelColorPreview.Location = new DrawPoint(buttonSet.Right + DefaultSidePadding, row2Y);

			group.Size = new DrawSize(nudA.Right + DefaultSidePadding, buttonSet.Bottom + DefaultSidePadding);
			newControlY += group.Height + DefaultSidePadding;
		}

		private static void GenerateStringControls(Panel panel, ref int newControlY, object obj,
			PropertyInfo property)
		{
			var attribute = GetPropertyAttribute<StringPropertyAttribute>(obj, property);

			ThrowIfWriteOnlyProperty(property);
			bool isReadonlyProperty = IsReadOnlyProperty(property);

			GroupBox group = GenerateDefaultGroupBox(panel, newControlY);
			group.Text = attribute.Name;
			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(group, attribute.Description);
			panel.Controls.Add(group);

			Button buttonSet = GenerateDefaultSetButton(0, !isReadonlyProperty);
			buttonSet.Location = new DrawPoint(group.Width - buttonSet.Width - DefaultSidePadding, GroupControlY);
			group.Controls.Add(buttonSet);

			TextBox textString = new TextBox();
			textString.Location = new DrawPoint(DefaultSidePadding, GroupControlY);
			textString.Size = new DrawSize(buttonSet.Left - DefaultSidePadding, ButtonSetHeight);
			textString.Text = (string)property.GetValue(obj);
			group.Controls.Add(textString);

			buttonSet.Click += (sender, e) =>
			{
				PropertySetters.SetStringProperty(textString, obj, property);
			};

			newControlY += group.Height + DefaultSidePadding;
		}

		private static void GenerateEnumControls(Panel panel, ref int newControlY, object obj,
			PropertyInfo property)
		{
			var attribute = GetPropertyAttribute<EnumPropertyAttribute>(obj, property);
			var enumType = property.PropertyType;

			if (enumType.GetCustomAttribute<FlagsAttribute>() != null)
			{
				// We'll add this later. With checkboxes!
				throw new NotImplementedException();
			}

			ThrowIfWriteOnlyProperty(property);
			bool isReadonlyProperty = IsReadOnlyProperty(property);

			if (!allEnumDescriptions.ContainsKey(enumType))
			{
				var enumDescriptions = GetEnumValueDescriptions(enumType);
				allEnumDescriptions.Add(enumType, enumDescriptions);
			}

			GroupBox group = GenerateDefaultGroupBox(panel, newControlY);
			group.Text = attribute.Name;
			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(group, attribute.Description);
			panel.Controls.Add(group);

			// Get all the names of the enum.
			var enumNames = Enum.GetNames(enumType);

			ComboBox comboEnumNames = new ComboBox();
			comboEnumNames.Items.AddRange(enumNames);
			int indexOfPropertyValue = comboEnumNames.Items.IndexOf(property.GetValue(obj).ToString());
			comboEnumNames.SelectedIndex = indexOfPropertyValue;
			comboEnumNames.Location = new DrawPoint(DefaultSidePadding, GroupControlY);
			comboEnumNames.Size = new DrawSize(200, ButtonSetHeight);
			comboEnumNames.Enabled = !isReadonlyProperty;
			group.Controls.Add(comboEnumNames);


			// Create a set button.
			Button buttonSet = GenerateDefaultSetButton(comboEnumNames.Right + DefaultSidePadding, 
				!isReadonlyProperty);
			group.Controls.Add(buttonSet);

			// Create the enum value description label.
			Label labelDescription = new Label();
			labelDescription.Location = new DrawPoint(DefaultSidePadding, comboEnumNames.Bottom + DefaultSidePadding);
			labelDescription.Size = new DrawSize(group.Width - (DefaultSidePadding * 2), ButtonSetHeight * 2);
			group.Controls.Add(labelDescription);
			group.Size = new DrawSize(group.Width, labelDescription.Bottom + DefaultSidePadding);

			// Create a handler for the combo box selected index change event. This updates the
			// description label.
			comboEnumNames.SelectedIndexChanged += (sender, e) =>
			{
				string selectedItem = (string)comboEnumNames.SelectedItem;
				UpdateEnumDescriptionLabel(enumType, selectedItem, labelDescription);
			};
			
			// Create the set button handler.
			buttonSet.Click += (sender, e) =>
			{
				PropertySetters.SetEnumProperty(comboEnumNames, obj, property, enumType);
			};

			newControlY += group.Height + DefaultSidePadding;
		}

		private static void GeneratedNestedPropertyControls(Panel panel, ref int newControlY,
			object obj, PropertyInfo property)
		{
			var attribute = GetPropertyAttribute<NestedPropertyAttribute>(obj, property);

			ThrowIfWriteOnlyProperty(property);
			bool isReadonlyProperty = IsReadOnlyProperty(property);

			GroupBox group = GenerateDefaultGroupBox(panel, newControlY);
			group.Text = attribute.Name;
			ToolTip toolTip = new ToolTip();
			toolTip.SetToolTip(group, attribute.Description);
			panel.Controls.Add(group);

			Button buttonOpenPropertyEditor = new Button();
			buttonOpenPropertyEditor.Location = new DrawPoint(DefaultSidePadding, GroupControlY);
			buttonOpenPropertyEditor.Size = new DrawSize(group.Width - (DefaultSidePadding * 2),
				group.Height - DefaultSidePadding - GroupControlY);
			buttonOpenPropertyEditor.Text = "Edit Properties...";
			buttonOpenPropertyEditor.Enabled = !isReadonlyProperty;
			buttonOpenPropertyEditor.Click += (sender, e) =>
			{
				var propForm = new PropertyForm(property.GetValue(obj), true);
				propForm.Show();
			};
			group.Controls.Add(buttonOpenPropertyEditor);

			newControlY += group.Height + DefaultSidePadding;
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

		private static IDictionary<object, string> GetEnumValueDescriptions(Type enumType)
		{
			Dictionary<object, string> result = new Dictionary<object, string>();

			foreach (var value in Enum.GetValues(enumType))
			{
				var attribute = GetEnumValueAttribute(enumType, value);
				if (attribute != null) { result.Add(value, attribute.Description); }
				else { result.Add(value, ""); }
			}

			return result;
		}

		private static EnumValueAttribute GetEnumValueAttribute(Type enumType, object enumValue)
		{
			// http://stackoverflow.com/a/1799401/2709212
			var member = enumType.GetMember(enumValue.ToString());
			var attribute = member[0].GetCustomAttribute(typeof(EnumValueAttribute));

			return (EnumValueAttribute)attribute;
		}

		private static void UpdateEnumDescriptionLabel(Type enumType, string enumValueName, Label labelDescription)
		{
			var descriptions = allEnumDescriptions[enumType];
			var value = Enum.Parse(enumType, enumValueName);
			labelDescription.Text = descriptions[value];
		}

		private static bool IsReadOnlyProperty(PropertyInfo info) => (info.GetSetMethod() == null);

		private static bool IsUserEditablePropertyType(PropertyInfo property)
		{
			var propertyType = property.PropertyType;
			if (propertyType.IsEnum) { return true; }

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
