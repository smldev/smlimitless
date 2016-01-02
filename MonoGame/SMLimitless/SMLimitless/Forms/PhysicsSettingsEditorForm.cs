﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SMLimitless.Physics;

namespace SMLimitless.Forms
{
	/// <summary>
	/// A form which contains sliders to adjust various physics properties of objects.
	/// </summary>
	public partial class PhysicsSettingsEditorForm : Form
	{
		private const int DistanceBetweenNewControls = 40;
		private const int DefaultSidePadding = 5;
		private const int LabelHeightSingleLine = 15;
		private const int ControlHeight = 40 - LabelHeightSingleLine;

		private int newControlY = DefaultSidePadding;   // Stores the position of the next control set for the next physics setting to be added.
		private bool withinControlEventHandler = false;	// This field is used to ensure that a ValueChanged event handler doesn't fire any others.
	
		/// <summary>
		/// Initializes a new instance of the <see cref="PhysicsSettingsEditorForm"/> class.
		/// </summary>
		public PhysicsSettingsEditorForm()
		{
			InitializeComponent();
		}

		internal void AddSetting<T>(PhysicsSetting<T> setting) where T : struct, IComparable<T>
		{
			if (typeof(T) == typeof(int))
			{
				AddSetting((PhysicsSetting<int>)(object)setting);	// omg what am I doing
			}
		}

		internal void AddSetting(PhysicsSetting<int> setting)
		{
			TrackBar trackBar;
			NumericUpDown numericUpDown;
			AddIntegerTrackBar(setting.Name, setting.Minimum, setting.Maximum, setting.Value, out trackBar, out numericUpDown);

			trackBar.ValueChanged += (sender, e) =>
			{
				if (!withinControlEventHandler)
				{
					withinControlEventHandler = true;
					setting.Value = trackBar.Value;
					numericUpDown.Value = trackBar.Value;
					withinControlEventHandler = false;
				}
			}; // ♥♥♥

			numericUpDown.ValueChanged += (sender, e) =>
			{
				if (!withinControlEventHandler)
				{
					withinControlEventHandler = true;
					setting.Value = (int)numericUpDown.Value;
					trackBar.Value = (int)numericUpDown.Value;
					withinControlEventHandler = false;
				}
			}; // seriously using lambdas as drop-in event handlers is so awesome
		}

		private void AddIntegerTrackBar(string name, int minimum, int maximum, int currentValue, out TrackBar trackBarControl, out NumericUpDown nudControl)
		{
			// Add a Label, TrackBar, and NumericUpDown to the panel
			// to allow a change in an integer physics setting

			Label label = new Label();
			label.AutoSize = true;
			label.Text = name;
			label.Location = new Point(DefaultSidePadding, newControlY);
			PanelSettings.Controls.Add(label);
			newControlY += LabelHeightSingleLine + DefaultSidePadding;

			TrackBar trackBar = new TrackBar();
			trackBar.Minimum = minimum;
			trackBar.Maximum = maximum;
			trackBar.Value = currentValue;
			trackBar.Location = new Point(DefaultSidePadding, newControlY);
			trackBar.Size = new Size((int)(PanelSettings.Width * 0.8f) - DefaultSidePadding, ControlHeight);
			PanelSettings.Controls.Add(trackBar);

			NumericUpDown numericUpDown = new NumericUpDown();
			numericUpDown.Minimum = minimum;
			numericUpDown.Maximum = maximum;
			numericUpDown.Value = currentValue;
			numericUpDown.Location = new Point(DefaultSidePadding + (int)(PanelSettings.Width * 0.8f) + (DefaultSidePadding * 2), newControlY + ControlHeight);
			numericUpDown.Width = 45;
			PanelSettings.Controls.Add(numericUpDown);
			newControlY += ControlHeight + DefaultSidePadding;

			trackBarControl = trackBar;
			nudControl = numericUpDown;
		}
	}
}
