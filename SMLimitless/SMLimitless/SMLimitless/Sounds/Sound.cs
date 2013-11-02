//-----------------------------------------------------------------------
// <copyright file="Sound.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NAudio;
using NAudio.Wave;

namespace SMLimitless.Sounds
{
    /// <summary>
    /// Represents a sound effect or music track.
    /// </summary>
    public class Sound
    {
        /// <summary>
        /// A device that can play waveform audio.
        /// </summary>
        private IWavePlayer waveOutDevice;

        /// <summary>
        /// A stream for the sound's data.
        /// </summary>
        private WaveStream mainOutputStream;

        /// <summary>
        /// Represents a channel for the mixer.
        /// </summary>
        private WaveChannel32 volumeStream;

        /// <summary>
        /// Gets the path to the file that this sound was loaded from.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets or sets the volume of the sound (between 0.0 and 1.0).
        /// </summary>
        public float Volume
        {
            get
            {
                return this.volumeStream.Volume;
            }

            set
            {
                this.volumeStream.Volume = value;
            }
        }

        /// <summary>
        /// Gets or sets the pan of the sound, which determines how loud each speaker is (between -1.0 and 1.0).
        /// </summary>
        public float Pan
        {
            get
            {
                return this.volumeStream.Pan;
            }

            set
            {
                this.volumeStream.Pan = value;
            }
        }

        /// <summary>
        /// Gets or sets the position in samples of this sound.
        /// </summary>
        public long Position
        {
            get
            {
                return this.volumeStream.Position;
            }

            set
            {
                this.volumeStream.Position = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sound"/> class.
        /// </summary>
        /// <param name="filePath">The path to the file containing the sound.</param>
        public Sound(string filePath)
        {
            this.FilePath = filePath;
            this.waveOutDevice = new WaveOutEvent();

            if (!filePath.ToUpperInvariant().EndsWith(".MP3"))
            {
                throw new ArgumentException("MusicTrack.ctor(string, string): The given file path did not end in an .MP3 extension.", "filePath");
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(string.Format("MusicTrack.ctor(string, string): The file at \"{0}\" does not exist.", filePath));
            }
        }

        /// <summary>
        /// Initializes this sound.
        /// </summary>
        public void Initialize()
        {
            WaveChannel32 inputStream;
            WaveStream mp3Reader = new Mp3FileReader(this.FilePath);
            inputStream = new WaveChannel32(mp3Reader);

            this.volumeStream = inputStream;
            this.mainOutputStream = this.volumeStream;
        }

        /// <summary>
        /// Plays this sound.
        /// </summary>
        public void Play()
        {
            this.waveOutDevice.Init(this.mainOutputStream);
            this.waveOutDevice.Play();
        }

        /// <summary>
        /// Stops playback of this sound.
        /// </summary>
        public void Stop()
        {
            this.waveOutDevice.Stop();
        }

        /// <summary>
        /// Unloads this sound and frees resources associated with it.
        /// </summary>
        public void UnloadContent()
        {
            if (this.waveOutDevice != null)
            {
                this.waveOutDevice.Stop();
            }

            if (this.mainOutputStream != null)
            {
                // this one really closes the file and ACM conversion
                this.volumeStream.Close();
                this.volumeStream = null;

                // this one does the metering stream
                this.mainOutputStream.Close();
                this.mainOutputStream = null;
            }

            if (this.waveOutDevice != null)
            {
                this.waveOutDevice.Dispose();
                this.waveOutDevice = null;
            }
        }
    }
}
