using System;
using NAudio.Wave;

namespace SMLimitless.Sounds
{
	/// <summary>
	///   Provides samples for a sound that fades in or fades out.
	/// </summary>
	public sealed class FadeInFadeOutSampleProvider : ISampleProvider
	{
		// Credit to Mark Heath (http://stackoverflow.com/a/9471208/2709212)

		private enum FadeState
		{
			Silence,
			FadingIn,
			FullVolume,
			FadingOut
		}

		private readonly object lockObject = new object();
		private readonly ISampleProvider source;
		private int fadeSampleCount;
		private int fadeSamplePosition;
		private FadeState fadeState;

		/// <summary>
		///   Gets the <see cref="WaveFormat" /> for this provider.
		/// </summary>
		public WaveFormat WaveFormat
		{
			get { return source.WaveFormat; }
		}

		/// <summary>
		///   Initializes a new instance of the <see
		///   cref="FadeInFadeOutSampleProvider" /> class.
		/// </summary>
		/// <param name="source">A sample provider.</param>
		public FadeInFadeOutSampleProvider(ISampleProvider source)
		{
			this.source = source;
			fadeState = FadeState.FullVolume;
		}

		/// <summary>
		///   Begins a fade-in.
		/// </summary>
		/// <param name="fadeDurationInMilliseconds">
		///   How long the fade should last, in milliseconds.
		/// </param>
		public void BeginFadeIn(double fadeDurationInMilliseconds)
		{
			lock (lockObject)
			{
				fadeSamplePosition = 0;
				fadeSampleCount = (int)((fadeDurationInMilliseconds * source.WaveFormat.SampleRate) / 1000);
				fadeState = FadeState.FadingIn;
			}
		}

		/// <summary>
		///   Begins a fade-out.
		/// </summary>
		/// <param name="fadeDurationInMilliseconds">
		///   How long the fade should last, in milliseconds.
		/// </param>
		public void BeginFadeOut(double fadeDurationInMilliseconds)
		{
			lock (lockObject)
			{
				fadeSamplePosition = 0;
				fadeSampleCount = (int)((fadeDurationInMilliseconds * source.WaveFormat.SampleRate) / 1000);
				fadeState = FadeState.FadingOut;
			}
		}

		/// <summary>
		///   Reads samples from this provider into a buffer.
		/// </summary>
		/// <param name="buffer">A buffer to write samples into.</param>
		/// <param name="offset">How many samples in the sound to skip.</param>
		/// <param name="count">The number of samples to read.</param>
		/// <returns>The number of samples read.</returns>
		public int Read(float[] buffer, int offset, int count)
		{
			int sourceSamplesRead = source.Read(buffer, offset, count);
			lock (lockObject)
			{
				if (fadeState == FadeState.FadingIn)
				{
					FadeIn(buffer, offset, sourceSamplesRead);
				}
				else if (fadeState == FadeState.FadingOut)
				{
					FadeOut(buffer, offset, sourceSamplesRead);
				}
				else if (fadeState == FadeState.Silence)
				{
					ClearBuffer(buffer, offset, count);
				}
			}

			return sourceSamplesRead;
		}

		private static void ClearBuffer(float[] buffer, int offset, int count)
		{
			for (int n = 0; n < count; n++)
			{
				buffer[n + offset] = 0f;
			}
		}

		private void FadeIn(float[] buffer, int offset, int sourceSamplesRead)
		{
			int sample = 0;
			while (sample < sourceSamplesRead)
			{
				float multiplier = (fadeSamplePosition / (float)fadeSampleCount);
				for (int ch = 0; ch < source.WaveFormat.Channels; ch++)
				{
					buffer[offset + sample++] *= multiplier;
				}
				fadeSamplePosition++;
				if (fadeSamplePosition > fadeSampleCount)
				{
					fadeState = FadeState.FullVolume;
					break;
				}
			}
		}

		private void FadeOut(float[] buffer, int offset, int sourceSamplesRead)
		{
			int sample = 0;
			while (sample < sourceSamplesRead)
			{
				float multiplier = 1.0f - (fadeSamplePosition / (float)fadeSampleCount);
				for (int ch = 0; ch < source.WaveFormat.Channels; ch++)
				{
					buffer[offset + sample++] *= multiplier;
				}
				fadeSamplePosition++;
				if (fadeSamplePosition > fadeSampleCount)
				{
					fadeState = FadeState.Silence;
					ClearBuffer(buffer, sample + offset, sourceSamplesRead - sample);
					break;
				}
			}
		}
	}
}
