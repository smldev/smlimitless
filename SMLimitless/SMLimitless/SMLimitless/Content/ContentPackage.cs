//-----------------------------------------------------------------------
// <copyright file="ContentPackage.cs" company="Chris Akridge">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SMLimitless.Interfaces;
using SMLimitless.IO;

namespace SMLimitless.Content
{
    /// <summary>
    /// Stores information about a content package,
    /// a folder with a custom content assembly and
    /// a set of graphics and sounds for the custom
    /// content.
    /// </summary>
    internal sealed class ContentPackage : IName
    {
        /// <summary>
        /// A value indicating whether this content package is loaded.
        /// </summary>
        private bool isLoaded;

        /// <summary>
        /// The path to the settings file of this content package.
        /// </summary>
        private string settingsFilePath;

        /// <summary>
        /// A searcher for this content package.
        /// </summary>
        private ContentPackageResourceSearcher searcher;

        /// <summary>
        /// Gets the name of this content package.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the path to the folder of this content package.
        /// </summary>
        internal string BaseFolderPath { get; private set; }

        /// <summary>
        /// Gets the path to the custom content assembly file.
        /// </summary>
        internal string AssemblyPath { get; private set; }

        /// <summary>
        /// Gets the name of the person(s) who created this content package.
        /// </summary>
        internal string Author { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPackage"/> class.
        /// </summary>
        internal ContentPackage()
        {
            this.searcher = new ContentPackageResourceSearcher(this);
        }

        /// <summary>
        /// Loads a content package.
        /// </summary>
        /// <param name="settingsPath">The path to the settings file for this package.</param>
        internal void Load(string settingsPath)
        {
            if (!this.isLoaded)
            {
                if (!File.Exists(settingsPath))
                {
                    throw new FileNotFoundException(string.Format("ContentPackage.Load(string): The file at {0} does not exist.", settingsPath));
                }

                this.settingsFilePath = settingsPath;
                Dictionary<string, string> settings = new DataReader(settingsPath).ReadFullSection("[ContentPackage]");

                this.Name = settings["Name"];
                this.Author = settings["Author"];
                this.BaseFolderPath = string.Concat(new FileInfo(settingsPath).DirectoryName, @"\");
                this.AssemblyPath = string.Concat(this.BaseFolderPath, settings["AssemblyPath"]);

                // Sanity check - let's make sure the assembly path is right.
                if (!File.Exists(this.AssemblyPath) || !this.AssemblyPath.EndsWith(".dll"))
                {
                    throw new FileNotFoundException(string.Format("ContentPackage.ctor(string): The assembly file at {0} does not exist. Please check the settings file at {1}.", this.AssemblyPath, settingsPath));
                }

                this.isLoaded = true;
            }
        }

        /// <summary>
        /// Loads a content package given its folder path.
        /// </summary>
        /// <param name="settingsFolder">The path to the folder of the package.</param>
        internal void LoadFromFolder(string settingsFolder)
        {
            string fullPath = string.Concat(settingsFolder, @"\settings.txt");
            this.Load(fullPath);
        }

        /// <summary>
        /// Gets a file path to a resource given the resource name.
        /// </summary>
        /// <param name="resourceName">The name of the resource.</param>
        /// <returns>A path to the resource.</returns>
        internal string GetResourcePath(string resourceName)
        {
            return this.searcher.GetResourcePath(resourceName);
        }

        /// <summary>
        /// Returns a string representation of this pack's key values.
        /// </summary>
        /// <returns>A representation of the key values.</returns>
        public override string ToString()
        {
            string nameString = string.Format("Name: {0}", this.Name);
            string authorString = string.Format("Author: {0}", this.Author);
            string assemblyPathString = string.Format("Assembly Path: {0}", this.AssemblyPath);
            string baseFolderPathString = string.Format("Base Folder Path: {0}", this.BaseFolderPath);
            return string.Format("{0}, {1}, {2}, {3}, {4}", nameString, authorString, assemblyPathString, baseFolderPathString);
        }
    }
}
