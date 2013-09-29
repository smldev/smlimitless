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
        private static Dictionary<string, Sound> loadedSounds;

        static SoundManager()
        {
            SoundManager.loadedSounds = new Dictionary<string, Sound>();
        }

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

        public static void RemoveSound(string name)
        {
            if (SoundManager.loadedSounds.ContainsKey(name))
            {
                SoundManager.loadedSounds.Remove(name);
            }
            else
            {
                throw new Exception(string.Format("SoundManager.RemoveMusicTrack(string, string): Tried to remove a music track named \"{0}\", but it wasn't loaded.", name));
            }
        }

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

        public static void PlaySound(string name)
        {
            SoundManager.GetSound(name).Play();
        }

        public static void StopSound(string name)
        {
            SoundManager.GetSound(name).Stop();
        }

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
