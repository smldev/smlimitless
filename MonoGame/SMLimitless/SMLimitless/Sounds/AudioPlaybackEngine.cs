using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SMLimitless.Sounds
{
	public sealed class AudioPlaybackEngine : IDisposable
	{
		// Credit to Mark Heath (NAudio author)
		// http://mark-dot-net.blogspot.com/2014/02/fire-and-forget-audio-playback-with.html

		private readonly IWavePlayer outputDevice;
		private readonly MixingSampleProvider mixer;

		private List<string> playingSoundFileNames = new List<string>();

		public static readonly AudioPlaybackEngine Instance = new AudioPlaybackEngine(44100, 2);

		public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2)
		{
			outputDevice = new WaveOutEvent();
			mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
			mixer.ReadFully = true;
			outputDevice.Init(mixer);
			outputDevice.Play();
		}

		public void PlaySound(string fileName)
		{
			if (!playingSoundFileNames.Contains(fileName))
			{
				var input = new AudioFileReader(fileName);
				var reader = new AutoDisposeFileReader(input);
				reader.PlaybackEndedEvent += (sender, e) => playingSoundFileNames.Remove(fileName);
				AddMixerInput(reader);
				playingSoundFileNames.Add(fileName);
			}
		}

		private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
		{
			if (input.WaveFormat.Channels == mixer.WaveFormat.Channels)
			{
				return input;
			}

			if (input.WaveFormat.Channels == 1 && mixer.WaveFormat.Channels == 2)
			{
				return new MonoToStereoSampleProvider(input);
			}

			throw new NotImplementedException($"Cannot convert a sound from {input.WaveFormat.Channels} channel(s) to {mixer.WaveFormat.Channels} channel(s).");
		}

		public void PlaySound(CachedSound sound)
		{
			if (!playingSoundFileNames.Contains(sound.Name))
			{
				CachedSoundSampleProvider provider = new CachedSoundSampleProvider(sound);
				playingSoundFileNames.Add(sound.Name);
				provider.PlaybackEndedEvent += (sender, e) => playingSoundFileNames.Remove(sound.Name);
				AddMixerInput(provider);
			}
		}

		private void AddMixerInput(ISampleProvider input)
		{
			mixer.AddMixerInput(ConvertToRightChannelCount(input));
		}

		public void Dispose()
		{
			outputDevice.Dispose();
		}
	}
}
