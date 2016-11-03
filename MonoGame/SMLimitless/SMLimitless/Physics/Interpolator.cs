//-----------------------------------------------------------------------
// <copyright file="Interpolator.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using SMLimitless.Extensions;

namespace SMLimitless.Physics
{
	/// <summary>
	///   Represents a type that can interpolate between two values, and
	///   optionally perform an action whenever the interpolator is updated or
	///   when the interpolator completes.
	/// </summary>
	public class Interpolator : IFormattable
	{
		/// <summary>
		///   Gets the action to perform whenever the interpolation is complete.
		/// </summary>
		public Action<Interpolator> Completed { get; private set; }

		/// <summary>
		///   Gets a value indicating whether this interpolator is currently running.
		/// </summary>
		public bool Enabled { get; private set; }

		/// <summary>
		///   Gets the value at which the interpolator ends.
		/// </summary>
		public float End { get; private set; }

		/// <summary>
		///   Gets the length in seconds of how long the interpolation will take.
		/// </summary>
		public float Length { get; private set; }

		/// <summary>
		///   Gets the interpolator's progress in the range of [0, 1].
		/// </summary>
		public float Progress { get; private set; }

		/// <summary>
		///   Gets the range (end - start) of the interpolator.
		/// </summary>
		public float Range
		{
			get
			{
				return End - Start;
			}
		}

		/// <summary>
		///   Gets the scale of this interpolator.
		/// </summary>
		public InterpolatorScaleDelegate Scale { get; private set; }

		/// <summary>
		///   Gets the value at which the interpolator starts.
		/// </summary>
		public float Start { get; private set; }

		/// <summary>
		///   Gets the action to perform whenever a step occurs.
		/// </summary>
		public Action<Interpolator> Step { get; private set; }

		/// <summary>
		///   Gets the current value of the interpolator.
		/// </summary>
		public float Value { get; private set; }

		/// <summary>
		///   Initializes a new instance of the <see cref="Interpolator" /> class.
		/// </summary>
		/// <param name="endValue">The value at which the interpolator ends.</param>
		/// <param name="length">The length in seconds of the interpolation.</param>
		/// <param name="step">The action to perform on each step.</param>
		public Interpolator(float endValue, float length, Action<Interpolator> step)
			: this(0, endValue, length, step)
		{
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="Interpolator" /> class.
		/// </summary>
		/// <param name="startValue">
		///   The value at which the interpolator starts.
		/// </param>
		/// <param name="endValue">The value at which the interpolator ends.</param>
		/// <param name="length">The length in seconds of the interpolation.</param>
		/// <param name="step">The action to perform on each step.</param>
		public Interpolator(float startValue, float endValue, float length, Action<Interpolator> step)
			: this(startValue, endValue, length, step, InterpolatorScales.Linear)
		{
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="Interpolator" /> class.
		/// </summary>
		/// <param name="startValue">
		///   The value at which the interpolator starts.
		/// </param>
		/// <param name="endValue">The value at which the interpolator ends.</param>
		/// <param name="length">The length in seconds of the interpolation.</param>
		/// <param name="step">The action to perform on each step.</param>
		/// <param name="scale">The kind of interpolator scale to use.</param>
		public Interpolator(float startValue, float endValue, float length, Action<Interpolator> step, InterpolatorScaleDelegate scale) : this(startValue, endValue, length, step, null, scale)
		{
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="Interpolator" /> class.
		/// </summary>
		/// <param name="startValue">
		///   The value at which the interpolator starts.
		/// </param>
		/// <param name="endValue">The value at which the interpolator ends.</param>
		/// <param name="length">The length in seconds of the interpolation.</param>
		/// <param name="step">The action to perform on each step.</param>
		/// <param name="completed">
		///   The action to perform when the interpolator completes.
		/// </param>
		/// <param name="scale">The kind of interpolator scale to use.</param>
		public Interpolator(float startValue, float endValue, float length, Action<Interpolator> step, Action<Interpolator> completed, InterpolatorScaleDelegate scale)
		{
			Reset(startValue, endValue, length, step, completed, scale);
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="Interpolator" /> class.
		/// </summary>
		internal Interpolator()
		{
		}

		/// <summary>
		///   Forces the interpolator to finish immediately.
		/// </summary>
		public void ForceFinish()
		{
			if (Enabled)
			{
				Enabled = false;
				Progress = 1;
				float scaledProgress = Scale(Progress);
				Value = (Start + Range) * scaledProgress;

				if (Step != null)
				{
					Step(this);
				}

				if (Completed != null)
				{
					Completed(this);
				}
			}
		}

		/// <summary>
		///   Stops this interpolator.
		/// </summary>
		public void Stop()
		{
			Enabled = false;
		}

		/// <summary>
		///   Returns a string containing several useful values.
		/// </summary>
		/// <returns>A string containing several useful values.</returns>
		public override string ToString()
		{
			return string.Format("{0} to {1}, {2}, Length: {3}, Value: {4}, Progress {5}", Start, End, Enabled ? "Enabled" : "Disabled", Length, Value, Progress);
		}

		/// <summary>
		///   Returns a string containing several useful values.
		/// </summary>
		/// <param name="format">The parameter is not used.</param>
		/// <param name="formatProvider">The parameter is not used.</param>
		/// <returns>A string containing several useful values.</returns>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return ToString();
		}

		/// <summary>
		///   Updates this interpolator.
		/// </summary>
		public void Update()
		{
			if (Enabled)
			{
				// Update the progress, clamping at 1f.
				Progress = Math.Min(Progress + ((1 / Length) * GameServices.GameTime.GetElapsedSeconds()), 1f);

				// Get the scaled progress and use that to generate the value
				float scaledProgress = Scale(Progress);
				Value = (Start + Range) * scaledProgress;

				// invoke the step callback
				if (Step != null)
				{
					Step(this);
				}

				// if the progress is 1, the interpolator is done.
				if (Progress == 1f)
				{
					Enabled = false;

					// invoke the completed callback
					if (Completed != null)
					{
						Completed(this);
					}

					// free the Interpolator's resources.
					Scale = null;
					Step = null;
					Completed = null;
				}
			}
		}

		/// <summary>
		///   Resets the values of this interpolator.
		/// </summary>
		/// <param name="startValue">
		///   The value at which the interpolator starts.
		/// </param>
		/// <param name="endValue">The value at which the interpolator ends.</param>
		/// <param name="length">The length in seconds of the interpolation.</param>
		/// <param name="step">The action to perform on each step.</param>
		/// <param name="completed">
		///   The action to perform when the interpolator completes.
		/// </param>
		/// <param name="scale">The kind of interpolator scale to use.</param>
		internal void Reset(float startValue, float endValue, float length, Action<Interpolator> step, Action<Interpolator> completed, InterpolatorScaleDelegate scale)
		{
			if (length <= 0)
			{
				throw new ArgumentException("'length' must be greater than zero.", "length");
			}

			if (scale == null)
			{
				throw new ArgumentNullException("'scale' cannot be null.", "scale");
			}

			Enabled = true;
			Progress = 0;
			Value = Start;
			Start = startValue;
			End = endValue;
			Length = length;
			Completed = completed;
			Step = step;
			Scale = scale;
		}
	}

	/// <summary>
	///   A delegate used by Interpolators to scale their progress and generate
	///   their current value.
	/// </summary>
	/// <param name="progress">
	///   The current progress of the Interpolator in the range [0, 1].
	/// </param>
	/// <returns>
	///   A value representing the scaled progress used to generate the
	///   Interpolator's Value.
	/// </returns>
	public delegate float InterpolatorScaleDelegate(float progress);
}
