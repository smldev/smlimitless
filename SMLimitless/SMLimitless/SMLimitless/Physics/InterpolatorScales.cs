using System;

namespace SMLimitless.Physics
{
    public static class InterpolatorScales
    {
        public static InterpolatorScaleDelegate Linear { get; private set; }
        public static InterpolatorScaleDelegate Quadratic { get; private set; }
        public static InterpolatorScaleDelegate InverseQuadratic { get; private set; }
        public static InterpolatorScaleDelegate Cubic { get; private set; }
        public static InterpolatorScaleDelegate SmoothStep { get; private set; }

        static InterpolatorScales()
        {
            Linear = LinearScale;
            Quadratic = QuadraticScale;
            InverseQuadratic = InverseQuadraticScale;
            Cubic = CubicScale;
            SmoothStep = SmoothstepScale;
        }

        private static float LinearScale(float progress)
        {
            return progress;
        }

        private static float QuadraticScale(float progress)
        {
            return progress * progress;
        }

        private static float InverseQuadraticScale(float progress)
        {
            float inverse = 1 - progress;
            return 1 - (inverse * inverse);
        }

        private static float CubicScale(float progress)
        {
            return progress * progress * progress;
        }

        private static float SmoothstepScale(float progress)
        {
            return (progress * progress * (3 - 2 * progress));
        }
    }
}
