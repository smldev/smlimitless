using System;
using System.Diagnostics;

using Microsoft.Xna.Framework;

using SMLimitless.Extensions;

namespace SMLimitless.Physics
{
    /// <summary>
    /// A delegate used by Interpolators to scale their progress and generate their current value.
    /// </summary>
    /// <param name="progress">The current progress of the Interpolator in the range [0, 1].</param>
    /// <returns>A value representing the scaled progress used to generate the Interpolator's Value.</returns>
    public delegate float InterpolatorScaleDelegate(float progress);

    public class Interpolator : IFormattable
    {
        public bool Enabled { get; private set; }

        public float Start { get; private set; }
        public float End { get; private set; }

        public float Range { get { return End - Start; } }

        /// <summary>
        /// Gets the interpolator's progress in the range of [0, 1].
        /// </summary>
        public float Progress { get; private set; }
        public float Value { get; private set; }

        public float Length { get; private set; }

        public InterpolatorScaleDelegate Scale { get; private set; }
        public Action<Interpolator> Step { get; private set; }
        public Action<Interpolator> Completed { get; private set; }

        internal Interpolator() { }

        public Interpolator(float endValue, float length, Action<Interpolator> step)
            : this(0, endValue, length, step) { }

        public Interpolator(float startValue, float endValue, float length, Action<Interpolator> step)
            : this(startValue, endValue, length, step, InterpolatorScales.Linear) { }

        public Interpolator(float startValue, float endValue, float length,
            Action<Interpolator> step, InterpolatorScaleDelegate scale)
            : this(startValue, endValue, length, step, null, scale) { }

        public Interpolator(float startValue, float endValue, float length,
            Action<Interpolator> step, Action<Interpolator> completed, InterpolatorScaleDelegate scale)
        {
            Reset(startValue, endValue, length, step, completed, scale);
        }

        internal void Reset(float startValue, float endValue, float length,
            Action<Interpolator> step, Action<Interpolator> completed, InterpolatorScaleDelegate scale)
        {
            if (length <= 0)
                throw new ArgumentException("'length' must be greater than zero.", "length");

            if (scale == null)
                throw new ArgumentNullException("'scale' cannot be null.", "scale");

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

        public void Stop()
        {
            Enabled = false;
        }

        public void ForceFinish()
        {
            if (Enabled)
            {
                Enabled = false;
                Progress = 1;
                float scaledProgress = Scale(Progress);
                Value = Start + Range * scaledProgress;

                if (Step != null)
                    Step(this);

                if (Completed != null)
                    Completed(this);
            }
        }

        public void Update()
        {
            if (Enabled)
            {
                // Update the progress, clamping at 1f.
                Progress = Math.Min(Progress + 1 / Length * GameServices.GameTime.GetElapsedSeconds(), 1f);

                // Get the scaled progress and use that to generate the value
                float scaledProgress = Scale(Progress);
                Value = Start + Range * scaledProgress;

                // invoke the step callback
                if (Step != null)
                    Step(this);

                // if the progress is 1, the interpolator is done.
                if (Progress == 1f)
                {
                    Enabled = false;

                    // invoke the completed callback
                    if (Completed != null)
                        Completed(this);

                    // free the Interpolator's resources.
                    Scale = null;
                    Step = null;
                    Completed = null;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0} to {1}, {2}, Length: {3}, Value: {4}, Progress {5}", Start, End, Enabled ? "Enabled" : "Disabled", Length, Value, Progress);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ToString();
        }
    }
}