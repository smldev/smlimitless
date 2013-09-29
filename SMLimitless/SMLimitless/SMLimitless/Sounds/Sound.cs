using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NAudio;
using NAudio.Wave;

namespace SMLimitless.Sounds
{
    public class Sound
    {
        private IWavePlayer waveOutDevice;
        private WaveStream mainOutputStream;
        private WaveChannel32 volumeStream;
        public string FilePath { get; private set; }

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

            // TODO: move everything below to Initialize()

        }

        public void Initialize()
        {
            WaveChannel32 inputStream;
            WaveStream mp3Reader = new Mp3FileReader(this.FilePath);
            inputStream = new WaveChannel32(mp3Reader);

            this.volumeStream = inputStream;
            this.mainOutputStream = volumeStream;
        }

        public void Play()
        {
            this.waveOutDevice.Init(this.mainOutputStream);
            this.waveOutDevice.Play();
        }

        public void Stop()
        {
            this.waveOutDevice.Stop();
        }

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
