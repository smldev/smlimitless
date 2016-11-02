//-----------------------------------------------------------------------
// <copyright file="ContentPackage.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using SMLimitless.Interfaces;
using SMLimitless.IO;
using SMLimitless.Sprites.Assemblies;

namespace SMLimitless.Content
{
	/// <summary>
	///   Stores information about a content package, a folder with a custom
	///   content assembly and a set of graphics and sounds for the custom content.
	/// </summary>
	internal sealed class ContentPackage : IName
	{
		/// <summary>
		///   A value indicating whether this content package is loaded.
		/// </summary>
		private bool isLoaded;

		/// <summary>
		///   A searcher for this content package.
		/// </summary>
		private ContentPackageResourceSearcher searcher;

		/// <summary>
		///   The path to the settings file of this content package.
		/// </summary>
		private string settingsFilePath;

		/// <summary>
		///   Gets the name of this content package.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		///   Gets the absolute path to the custom content assembly file.
		/// </summary>
		internal string AssemblyPath { get; private set; }

		/// <summary>
		///   Gets the name of the person(s) who created this content package.
		/// </summary>
		internal string Author { get; private set; }

		/// <summary>
		///   Gets the absolute path to the folder of this content package.
		/// </summary>
		internal string BaseFolderPath { get; private set; }

		/// <summary>
		///   Initializes a new instance of the <see cref="ContentPackage" /> class.
		/// </summary>
		internal ContentPackage()
		{
			searcher = new ContentPackageResourceSearcher(this);
		}

		/// <summary>
		///   Returns a string representation of this pack's key values.
		/// </summary>
		/// <returns>A representation of the key values.</returns>
		public override string ToString()
		{
			string nameString = string.Format("Name: {0}", Name);
			string authorString = string.Format("Author: {0}", Author);
			string assemblyPathString = string.Format("Assembly Path: {0}", AssemblyPath);
			string baseFolderPathString = string.Format("Base Folder Path: {0}", BaseFolderPath);
			return string.Format("{0}, {1}, {2}, {3}", nameString, authorString, assemblyPathString, baseFolderPathString);
		}

		/// <summary>
		///   Gets a file path to a resource given the resource name.
		/// </summary>
		/// <param name="resourceName">The name of the resource.</param>
		/// <returns>A path to the resource.</returns>
		internal string GetResourcePath(string resourceName)
		{
			return searcher.GetResourcePath(resourceName);
		}

		/// <summary>
		///   Loads a content package.
		/// </summary>
		/// <param name="settingsPath">
		///   The path to the settings file for this package.
		/// </param>
		internal void Load(string settingsPath)
		{
			if (!isLoaded)
			{
				if (!File.Exists(settingsPath))
				{
					throw new FileNotFoundException(string.Format("The settings file at {0} does not exist.", settingsPath));
				}

				settingsFilePath = settingsPath;
				Dictionary<string, string> settings = new DataReader(settingsPath).ReadFullSection("[ContentPackage]");

				Name = settings["Name"];
				Author = settings["Author"];
				BaseFolderPath = string.Concat(new FileInfo(settingsPath).DirectoryName, @"\");
				AssemblyPath = string.Concat(BaseFolderPath, settings["AssemblyPath"]);

				// Sanity check - let's make sure the assembly path is right.
				if (!File.Exists(AssemblyPath) || !AssemblyPath.EndsWith(".dll"))
				{
					throw new FileNotFoundException(string.Format("The assembly file at {0} does not exist. Please check the settings file at {1}.", AssemblyPath, settingsPath));
				}

				// Now load the assembly.
				AssemblyManager.LoadAssembly(AssemblyPath);

				isLoaded = true;
			}
		}

		/// <summary>
		///   Loads a content package given its folder path.
		/// </summary>
		/// <param name="settingsFolder">The path to the folder of the package.</param>
		internal void LoadFromFolder(string settingsFolder)
		{
			string fullPath = string.Concat(settingsFolder, @"\settings.txt");
			Load(fullPath);
		}
	}
}
