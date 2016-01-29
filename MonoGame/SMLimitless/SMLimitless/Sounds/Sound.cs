//-----------------------------------------------------------------------
// <copyright file="Sound.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
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
		private CachedSound soundData;
		
        /// <summary>
        /// Gets the path to the file that this sound was loaded from.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sound"/> class.
        /// </summary>
        /// <param name="filePath">The path to the file containing the sound.</param>
        public Sound(string filePath)
        {
			FilePath = filePath;

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
			soundData = new CachedSound(FilePath);
		}

		/// <summary>
		/// Plays this sound.
		/// </summary>
		public void Play()
        {
			AudioPlaybackEngine.Instance.PlaySound(soundData);
        }

        /// <summary>
        /// Stops playback of this sound.
        /// </summary>
        public void Stop()
        {

        }

        /// <summary>
        /// Unloads this sound and frees resources associated with it.
        /// </summary>
        public void UnloadContent()
        {
        }
    }
}
