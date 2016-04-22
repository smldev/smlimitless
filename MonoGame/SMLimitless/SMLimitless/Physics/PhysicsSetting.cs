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

		/// <summary>
		/// Gets the type of value this setting has.
		/// </summary>
		public PhysicsSettingType Type { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PhysicsSetting{T}"/> class.
		/// </summary>
		/// <param name="name">The setting's name.</param>
		/// <param name="minimum">The lowest value the value can be.</param>
		/// <param name="maximum">The highest value the value can be.</param>
		/// <param name="initialValue">The initial value of the setting.</param>
		/// <param name="type">The type of value this setting has.</param>
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

		/// <summary>
		/// Unregisters this setting from the <see cref="PhysicsSettingsEditorForm"/>.
		/// </summary>
		public void Dispose()
		{
			// TODO: implement unregistering of this setting
		}
    }

	/// <summary>
	/// Enumerates the types a physics setting can be.
	/// </summary>
	public enum PhysicsSettingType
	{
		/// <summary>
		/// Represents a setting that is integral (a whole number).
		/// </summary>
		Integer,

		/// <summary>
		/// Represents a setting that is a floating point number.
		/// </summary>
		FloatingPoint,

		/// <summary>
		/// Represents a setting that is a Boolean true/false value.
		/// </summary>
		Boolean,

		/// <summary>
		/// Represents a setting that has another type not otherwise listed here.
		/// </summary>
		Other
	}
}
