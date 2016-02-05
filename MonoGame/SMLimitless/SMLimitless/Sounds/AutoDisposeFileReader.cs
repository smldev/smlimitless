using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace SMLimitless.Sounds
{
	public sealed class AutoDisposeFileReader : ISampleProvider
	{
		// Credit to Mark Heath (author of NAudio)
		// http://mark-dot-net.blogspot.com/2014/02/fire-and-forget-audio-playback-with.html

		private readonly AudioFileReader reader;
		private bool isDisposed;

		public event EventHandler PlaybackEndedEvent;

		public AutoDisposeFileReader(AudioFileReader reader)
		{
			this.reader = reader;
			WaveFormat = reader.WaveFormat;
		}

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

		public WaveFormat WaveFormat { get; private set; }
	}
}
