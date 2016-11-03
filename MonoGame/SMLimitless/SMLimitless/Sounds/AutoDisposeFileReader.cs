using System;
using NAudio.Wave;

namespace SMLimitless.Sounds
{
	/// <summary>
	///   An automatically-disposing file reader for audio files.
	/// </summary>
	public sealed class AutoDisposeFileReader : ISampleProvider
	{
		// Credit to Mark Heath (author of NAudio) http://mark-dot-net.blogspot.com/2014/02/fire-and-forget-audio-playback-with.html

		private readonly AudioFileReader reader;
		private bool isDisposed;

		/// <summary>
		///   Gets a value indicating the <see cref="WaveFormat" /> of this reader.
		/// </summary>
		public WaveFormat WaveFormat { get; private set; }

		/// <summary>
		///   An event raised when the playback of this file has ended.
		/// </summary>
		public event EventHandler PlaybackEndedEvent;

		/// <summary>
		///   Initializes a new instance of the <see cref="AutoDisposeFileReader"
		///   /> class.
		/// </summary>
		/// <param name="reader">A reader for an audio file.</param>
		public AutoDisposeFileReader(AudioFileReader reader)
		{
			this.reader = reader;
			WaveFormat = reader.WaveFormat;
		}

		/// <summary>
		///   Reads a number of samples from the file into a buffer.
		/// </summary>
		/// <param name="buffer">A buffer of <see cref="float" /> samples.</param>
		/// <param name="offset">How many samples from the beginning to skip.</param>
		/// <param name="count">How many samples to read.</param>
		/// <returns>The number of samples read.</returns>
		public int Read(float[] buffer, int offset, int count)
		{
			if (isDisposed) { return 0; }

			int read = reader.Read(buffer, offset, count);
			if (read == 0)
			{
				OnPlaybackEnded();
				reader.Dispose();
				isDisposed = true;
			}
			return read;
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
