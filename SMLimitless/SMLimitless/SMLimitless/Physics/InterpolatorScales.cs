//-----------------------------------------------------------------------
// <copyright file="InterpolatorScales.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace SMLimitless.Physics
{
    /// <summary>
    /// Defines several different scales
    /// for interpolators to use.
    /// </summary>
    public static class InterpolatorScales
    {
        /// <summary>
        /// Initializes static members of the <see cref="InterpolatorScales"/> class.
        /// </summary>
        static InterpolatorScales()
        {
            Linear = LinearScale;
            Quadratic = QuadraticScale;
            InverseQuadratic = InverseQuadraticScale;
            Cubic = CubicScale;
            SmoothStep = SmoothstepScale;
        }

        /// <summary>
        /// Gets a scale that linearly interpolates between two values.
        /// The change in the value on every step is equal.
        /// </summary>
        public static InterpolatorScaleDelegate Linear { get; private set; }

        /// <summary>
        /// Gets a quadratic scale to interpolate between two values.
        /// The value starts increasing slowly, and gains more speed as it approaches completion.
        /// </summary>
        public static InterpolatorScaleDelegate Quadratic { get; private set; }

        /// <summary>
        /// Gets an inverse quadratic scale to interpolate between two values.
        /// The value starts increasing quickly, and loses speed as it approaches completion.
        /// </summary>
        public static InterpolatorScaleDelegate InverseQuadratic { get; private set; }

        /// <summary>
        /// Gets a cubic scale to interpolate between two values.
        /// This scale is similar to the quadratic scale, although it seems
        /// to accelerate more slowly.
        /// </summary>
        public static InterpolatorScaleDelegate Cubic { get; private set; }

        /// <summary>
        /// Gets a smooth step scale to interpolate between two values.
        /// The value will start increasing slowly, accelerate, and then slow
        /// down as it approaches completion.
        /// </summary>
        public static InterpolatorScaleDelegate SmoothStep { get; private set; }

        /// <summary>
        /// Applies the linear scale to a certain amount of progress.
        /// </summary>
        /// <param name="progress">The original progress.</param>
        /// <returns>The progress itself (it remains unchanged).</returns>
        private static float LinearScale(float progress)
        {
            return progress;
        }

        /// <summary>
        /// Applies the quadratic scale to a certain amount of progress.
        /// Only use for progress values less than 1.
        /// </summary>
        /// <param name="progress">The original progress.</param>
        /// <returns>Returns the progress multiplied by itself.</returns>
        private static float QuadraticScale(float progress)
        {
            return progress * progress;
        }

        /// <summary>
        /// Applies the inverse quadratic scale to a certain amount of progress.
        /// Only use for progress values less than 1.
        /// </summary>
        /// <param name="progress">The original progress.</param>
        /// <returns>Return 1 minus the inverse (1 minus the progress) squared.</returns>
        private static float InverseQuadraticScale(float progress)
        {
            float inverse = 1 - progress;
            return 1 - (inverse * inverse);
        }

        /// <summary>
        /// Applies the cubic scale to a certain amount of progress.
        /// Only use for progress values less than 1.
        /// </summary>
        /// <param name="progress">The original progress.</param>
        /// <returns>The progress multiplied by itself thrice.</returns>
        private static float CubicScale(float progress)
        {
            return progress * progress * progress;
        }

        /// <summary>
        /// Applies the smooth step scale to a certain amount of progress.
        /// Only use for progress values less than 1.
        /// </summary>
        /// <param name="progress">The original progress.</param>
        /// <returns>Returns the progress, multiplied by itself, multiplied by 3 less than twice the progress.</returns>
        private static float SmoothstepScale(float progress)
        {
            return progress * progress * (3 - (2 * progress));
        }
    }
}
