using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SMLimitless.Sounds
{
	/// <summary>
	/// A fire-and-forget audio player for music and sound effects.
	/// </summary>
	public sealed class AudioPlaybackEngine : IDisposable
	{
		// Credit to Mark Heath (NAudio author)
		// http://mark-dot-net.blogspot.com/2014/02/fire-and-forget-audio-playback-with.html
		private const int DesiredLatencyMs = 150;


		private readonly IWavePlayer outputDevice;
		private readonly MixingSampleProvider mixer;

		private List<string> playingSoundFileNames = new List<string>();

		/// <summary>
		/// Gets an instance of the <see cref="AudioPlaybackEngine"/> for global usage.
		/// </summary>
		public static readonly AudioPlaybackEngine Instance = new AudioPlaybackEngine(44100, 2);

		/// <summary>
		/// Initializes a new instance of the <see cref="AudioPlaybackEngine"/> class.
		/// </summary>
		/// <param name="sampleRate">How many samples should be played in a second.</param>
		/// <param name="channelCount">How many channels of sound should be played.</param>
		public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2)
		{
			outputDevice = new WaveOutEvent() { DesiredLatency = DesiredLatencyMs };
			mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
			mixer.ReadFully = true;
			outputDevice.Init(mixer);
			outputDevice.Play();
		}

		/// <summary>
		/// Plays a sound given a filename.
		/// </summary>
		/// <param name="fileName">The name of the file to play.</param>
		/// <param name="additionalOnPlaybackEndedHandler">An event handler raised when the playback ends.</param>
		public void PlaySound(string fileName, EventHandler additionalOnPlaybackEndedHandler)
		{
			if (!playingSoundFileNames.Contains(fileName))
			{
				var input = new AudioFileReader(fileName);
				var reader = new AutoDisposeFileReader(input);
				reader.PlaybackEndedEvent += (sender, e) => playingSoundFileNames.Remove(fileName);
				reader.PlaybackEndedEvent += additionalOnPlaybackEndedHandler;
				AddMixerInput(reader);
				playingSoundFileNames.Add(fileName);
			}
		}

		/// <summary>
		/// Plays a sound that fades in and/or out from a file.
		/// </summary>
		/// <param name="fileName">The name of the file to play.</param>
		/// <param name="additionalOnPlaybackEndedHandler">An event handler raised when the playback ends.</param>
		/// <param name="beginFadeInAction">An action called when the fade-in begins.</param>
		/// <param name="beginFadeOutAction">An action called when the fade-out begins.</param>
		public void PlayFadeableSound(string fileName, EventHandler additionalOnPlaybackEndedHandler, out Action<double> beginFadeInAction, out Action<double> beginFadeOutAction)
		{
			if (!playingSoundFileNames.Contains(fileName))
			{
				var input = new AudioFileReader(fileName);
				var reader = new AutoDisposeFileReader(input);
				reader.PlaybackEndedEvent += (sender, e) => playingSoundFileNames.Remove(fileName);
				reader.PlaybackEndedEvent += additionalOnPlaybackEndedHandler;

				FadeInFadeOutSampleProvider fadeSampleProvider = new FadeInFadeOutSampleProvider(reader);
				beginFadeInAction = fadeSampleProvider.BeginFadeIn;
				beginFadeOutAction = fadeSampleProvider.BeginFadeOut;

				AddMixerInput(fadeSampleProvider);
				playingSoundFileNames.Add(fileName);
			}
			else
			{
				// If the sound's already playing, we don't need to do anything,
				// but beginFadeInAction and beginFadeOutAction both need to be assigned,
				// so we give them both an empty lambda.
				beginFadeInAction = beginFadeOutAction = (value) => { };
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

		/// <summary>
		/// Plays a <see cref="CachedSound"/> instance.
		/// </summary>
		/// <param name="sound">The sound to play.</param>
		/// <param name="additionalOnPlaybackEndedHandler">An event handler raised when the playback ends.</param>
		public void PlaySound(CachedSound sound, EventHandler additionalOnPlaybackEndedHandler)
		{
			//if (!playingSoundFileNames.Contains(sound.Name))
			//{
				CachedSoundSampleProvider provider = new CachedSoundSampleProvider(sound);
				playingSoundFileNames.Add(sound.Name);
				provider.PlaybackEndedEvent += (sender, e) => playingSoundFileNames.Remove(sound.Name);
				provider.PlaybackEndedEvent += additionalOnPlaybackEndedHandler;
				AddMixerInput(provider);
			//}
		}

		/// <summary>
		/// Plays a <see cref="CachedSound"/> instance that fades in and/or out.
		/// </summary>
		/// <param name="sound">The sound to play.</param>
		/// <param name="additionalOnPlaybackEndedHandler">An event handler raised when the playback ends.</param>
		/// <param name="beginFadeInAction">An action called when the fade-in begins.</param>
		/// <param name="beginFadeOutAction">An action called when the fade-out begins.</param>
		public void PlayFadeableSound(CachedSound sound, EventHandler additionalOnPlaybackEndedHandler, out Action<double> beginFadeInAction, out Action<double> beginFadeOutAction)
		{
			if (!playingSoundFileNames.Contains(sound.Name))
			{
				CachedSoundSampleProvider provider = new CachedSoundSampleProvider(sound);
				playingSoundFileNames.Add(sound.Name);
				provider.PlaybackEndedEvent += (sender, e) => playingSoundFileNames.Remove(sound.Name);
				provider.PlaybackEndedEvent += additionalOnPlaybackEndedHandler;

				FadeInFadeOutSampleProvider fadeSampleProvider = new FadeInFadeOutSampleProvider(provider);
				beginFadeInAction = fadeSampleProvider.BeginFadeIn;
				beginFadeOutAction = fadeSampleProvider.BeginFadeOut;

				AddMixerInput(fadeSampleProvider);
			}
			else
			{
				beginFadeInAction = beginFadeOutAction = (value) => { };
			}
		}

		private void AddMixerInput(ISampleProvider input)
		{
			mixer.AddMixerInput(ConvertToRightChannelCount(input));
		}

		/// <summary>
		/// Disposes the members of this class.
		/// </summary>
		public void Dispose()
		{
			outputDevice.Dispose();
		}
	}
}
