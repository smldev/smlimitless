using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLimitless.Forms;

namespace SMLimitless.Physics
{
	/// <summary>
	/// Represents a physics setting which can be changed in a <see cref="Forms.PhysicsSettingsEditorForm"/>.
	/// </summary>
	/// <typeparam name="T">The type of the value of the setting.</typeparam>
	public sealed class PhysicsSetting<T> : IDisposable where T : struct, IComparable<T>
	{
		/// <summary>
		/// Gets the setting's name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets or sets the current value of the setting.
		/// </summary>
		public T Value { get; set; }

		/// <summary>
		/// Gets the lowest value the value can be.
		/// </summary>
		public T Minimum { get;  }

		/// <summary>
		/// Gets the highest value the value can be.
		/// </summary>
		public T Maximum { get; }

		public PhysicsSettingType Type { get; }

		public PhysicsSetting(string name, T minimum, T maximum, T initialValue, PhysicsSettingType type)
		{
			Name = name;
			Minimum = minimum;
			Maximum = maximum;
			Value = initialValue;
			Type = type;

			// Register this setting with the PhysicsSettingEditorForm
			GameServices.PhysicsSettingsEditorForm.AddSetting(this);
		}

		public void Dispose()
		{
			// TODO: implement unregistering of this setting
		}
    }

	public enum PhysicsSettingType
	{
		Integer,
		FloatingPoint,
		Boolean,
		Other
	}
}
