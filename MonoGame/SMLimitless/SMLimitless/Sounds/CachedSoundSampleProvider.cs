using System;
using NAudio.Wave;

namespace SMLimitless.Sounds
{
	/// <summary>
	///   Provides audio samples from a <see cref="CachedSound" /> instance.
	/// </summary>
	public sealed class CachedSoundSampleProvider : ISampleProvider
	{
		// Credit to Mark Heath (author of NAudio) http://mark-dot-net.blogspot.com/2014/02/fire-and-forget-audio-playback-with.html

		private readonly CachedSound cachedSound;
		private long position;

		/// <summary>
		///   Gets the <see cref="WaveFormat" /> of this sample provider.
		/// </summary>
		public WaveFormat WaveFormat => cachedSound.WaveFormat;

		/// <summary>
		///   An event raised when playback of this sound has ended.
		/// </summary>
		public event EventHandler PlaybackEndedEvent;

		/// <summary>
		///   Initializes a new instance of the <see
		///   cref="CachedSoundSampleProvider" /> class.
		/// </summary>
		/// <param name="cachedSound">The sound to provide samples from.</param>
		public CachedSoundSampleProvider(CachedSound cachedSound)
		{
			this.cachedSound = cachedSound;
		}

		/// <summary>
		///   Reads a number of samples from the sound.
		/// </summary>
		/// <param name="buffer">A buffer to write samples into.</param>
		/// <param name="offset">How many samples in the sound to skip.</param>
		/// <param name="count">The number of samples to read.</param>
		/// <returns>The number of samples read.</returns>
		public int Read(float[] buffer, int offset, int count)
		{
			var availableSamples = cachedSound.AudioData.Length - position;
			var samplesToCopy = Math.Min(availableSamples, count);
			Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);
			position += samplesToCopy;

			if (samplesToCopy == 0) { OnPlaybackEnded(); }

			return (int)samplesToCopy;
		}

		private void OnPlaybackEnded()
		{
			if (PlaybackEndedEvent != null)
			{
				PlaybackEndedEvent(this, new EventArgs());
			}
		}
	}
}
