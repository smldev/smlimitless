using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;
using SMLimitless.Interfaces;

namespace SMLimitless.Sounds
{
	/// <summary>
	///   A sound, consisting of a buffer of <see cref="float" /> samples.
	/// </summary>
	public sealed class CachedSound : IName
	{
		// Credit to Mark Heath (author of NAudio) http://mark-dot-net.blogspot.com/2014/02/fire-and-forget-audio-playback-with.html

		/// <summary>
		///   The samples of this sound.
		/// </summary>
		public float[] AudioData { get; private set; }

		/// <summary>
		///   The name of this sound.
		/// </summary>
		public string Name { get; }

		/// <summary>
		///   The <see cref="WaveFormat" /> of this sound.
		/// </summary>
		public WaveFormat WaveFormat { get; private set; }

		/// <summary>
		///   Initializes a new instance of the <see cref="CachedSound" /> class.
		/// </summary>
		/// <param name="audioFileName">
		///   The name of the file containing the sound.
		/// </param>
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
