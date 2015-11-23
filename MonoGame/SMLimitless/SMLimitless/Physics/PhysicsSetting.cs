using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless.Physics
{
	/// <summary>
	/// Represents a physics setting which can be changed in a <see cref="Forms.PhysicsSettingsEditorForm"/>.
	/// </summary>
	/// <typeparam name="T">The type of the value of the setting.</typeparam>
	public sealed class PhysicsSetting<T> where T : struct
	{
		/// <summary>
		/// Gets the setting's name.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets or sets the current value of the setting.
		/// </summary>
		public T Value { get; set; }

		/// <summary>
		/// Gets the lowest value the value can be.
		/// </summary>
		public T LowRange { get; private set; }

		/// <summary>
		/// Gets the highest value the value can be.
		/// </summary>
		public T HighRange { get; private set; }
	}
}
