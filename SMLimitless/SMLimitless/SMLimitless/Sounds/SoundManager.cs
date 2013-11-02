//-----------------------------------------------------------------------
// <copyright file="SoundManager.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless.Sounds
{
    /// <summary>
    /// Loads and manages sound effects and music.
    /// </summary>
    public static class SoundManager
    {
        /// <summary>
        /// A dictionary of loaded sounds. The string key is the sound's name, and the Sound value is the loaded sound.
        /// </summary>
        private static Dictionary<string, Sound> loadedSounds;

        /// <summary>
        /// Initializes static members of the <see cref="SoundManager"/> class.
        /// </summary>
        static SoundManager()
        {
            SoundManager.loadedSounds = new Dictionary<string, Sound>();
        }

        /// <summary>
        /// Loads a sound and adds it to the loaded sounds.
        /// </summary>
        /// <param name="name">The name of the sound. This will be used to access the sound.</param>
        /// <param name="filePath">The path to an MP3 file containing the sound.</param>
        public static void AddSound(string name, string filePath)
        {
            if (!SoundManager.loadedSounds.ContainsKey(name))
            {
                SoundManager.loadedSounds.Add(name, new Sound(filePath));
                SoundManager.loadedSounds[name].Initialize();
            }
            else
            {
                throw new Exception(string.Format("SoundManager.AddMusicTrack(string, string): Tried to add a music track named \"{0}\", but a track with that name was already loaded.", name));
            }
        }

        /// <summary>
        /// Unloads a sound and removes it from the list of unloaded sounds.
        /// </summary>
        /// <param name="name">The name of the sound to unload and remove.</param>
        public static void RemoveSound(string name)
        {
            if (SoundManager.loadedSounds.ContainsKey(name))
            {
                SoundManager.loadedSounds[name].UnloadContent();
                SoundManager.loadedSounds.Remove(name);
            }
            else
            {
                throw new Exception(string.Format("SoundManager.RemoveMusicTrack(string, string): Tried to remove a music track named \"{0}\", but it wasn't loaded.", name));
            }
        }

        /// <summary>
        /// Gets a sound by its name.
        /// </summary>
        /// <param name="name">The name of the sound.</param>
        /// <returns>A sound.</returns>
        public static Sound GetSound(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("SoundManager.GetSound(string): The name cannot be null.");
            }

            try
            {
                return SoundManager.loadedSounds[name];
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(string.Format("SoundManager.GetSound(string): There is no loaded sound named {0}.", name), ex);
            }
        }

        /// <summary>
        /// Plays a sound given its name.
        /// </summary>
        /// <param name="name">The name of the sound.</param>
        public static void PlaySound(string name)
        {
            SoundManager.GetSound(name).Play();
        }

        /// <summary>
        /// Stops playback of a sound given its name.
        /// </summary>
        /// <param name="name">The name of the sound.</param>
        public static void StopSound(string name)
        {
            SoundManager.GetSound(name).Stop();
        }

        /// <summary>
        /// Unloads and frees resources associated with all loaded sounds.
        /// </summary>
        public static void UnloadContent()
        {
            foreach (Sound track in SoundManager.loadedSounds.Values)
            {
                track.Stop();
                track.UnloadContent();
            }
        }
    }
}
