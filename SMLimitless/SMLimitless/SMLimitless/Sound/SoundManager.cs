using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace SMLimitless.Sound
{
    /// <summary>
    /// Loads and manages sound effects and music.
    /// </summary>
    public static class SoundManager
    {
        /// <summary>
        /// A collection of loaded sound effects. The string key is the name of the effect in the content package, the SoundEffect value is the sound effect.
        /// </summary>
        private static Dictionary<string, SoundEffect> loadedSoundEffects;

        /// <summary>
        /// Initializes static members of the <see cref="SoundManager"/> class.
        /// </summary>
        static SoundManager()
        {
            SoundManager.loadedSoundEffects = new Dictionary<string, SoundEffect>();
        }

        internal static SoundEffect LoadSoundFromFile(string filepath)
        {
            throw new NotImplementedException();
        }
    }
}
