﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace SMLimitless.Sounds
{
	public sealed class CachedSoundSampleProvider : ISampleProvider
	{
		// Credit to Mark Heath (author of NAudio)
		// http://mark-dot-net.blogspot.com/2014/02/fire-and-forget-audio-playback-with.html

		private readonly CachedSound cachedSound;
		private long position;

		public event EventHandler PlaybackEndedEvent;

		public CachedSoundSampleProvider(CachedSound cachedSound)
		{
			this.cachedSound = cachedSound;
		}

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

		public WaveFormat WaveFormat => cachedSound.WaveFormat;
	}
}
