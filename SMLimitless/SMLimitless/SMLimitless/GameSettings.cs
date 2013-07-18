﻿//-----------------------------------------------------------------------
// <copyright file="GameSettings.cs" company="Chris Akridge">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SMLimitless.IO;

namespace SMLimitless
{
    /// <summary>
    /// Loads the settings file.
    /// </summary>
    public static class GameSettings
    {
        /// <summary>
        /// A DataReader containing the game's settings file.
        /// </summary>
        private static DataReader settingsReader;

        /// <summary>
        /// Initializes this class and loads the settings file.
        /// </summary>
        public static void Initialize()
        {
            string appDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string settingsPath;
            #if DEBUG
            settingsPath = string.Concat(appDirectory, @"\..\..\..\GameSettings.txt");
            #elif !DEBUG
            settingsPath = string.Concat(appDirectory, @"GameSettings.txt");
            #endif

            if (!File.Exists(settingsPath))
            {
                throw new FileNotFoundException(string.Format("GameSettings.Initialize(): The settings file at {0} could not be found.", settingsPath));
            }

            GameSettings.settingsReader = new DataReader(settingsPath);
        }

        /// <summary>
        /// Returns a section of the settings file.
        /// </summary>
        /// <param name="sectionName">The name of the section, optionally encased in square brackets.</param>
        /// <returns>A Dictionary{string, string} containing the keys and their values.</returns>
        public static Dictionary<string, string> GetSection(string sectionName)
        {
            return GameSettings.settingsReader.ReadFullSection(sectionName);
        }
    }
}