using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace SMLimitless.Sounds
{
	public sealed class CachedSound
	{
		// Credit to Mark Heath (author of NAudio)
		// http://mark-dot-net.blogspot.com/2014/02/fire-and-forget-audio-playback-with.html

		public float[] AudioData { get; private set; }
		public WaveFormat WaveFormat { get; private set; }
		public string Name { get; }

		public CachedSound(string audioFileName)
		{
			Name = audioFileName;

			using (var audioFileReader = new AudioFileReader(audioFileName))
			{
				WaveFormat = audioFileReader.WaveFormat;
				var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
				var readBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
				int samplesRead;

				while ((samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0)
				{
					wholeFile.AddRange(readBuffer.Take(samplesRead));
				}

				AudioData = wholeFile.ToArray();
			}
		}
	}
}
