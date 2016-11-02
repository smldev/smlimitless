//-----------------------------------------------------------------------
// <copyright file="ContentPackageManager.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SMLimitless.Graphics;
using SMLimitless.Sounds;

namespace SMLimitless.Content
{
	/// <summary>
	///   Loads and manages a collection of ContentPackages.
	/// </summary>
	public static class ContentPackageManager
	{
		// Key: string - The name of a level file.
		// Value: string[] - An array of folder names that override the content
		//        for the level.
		/// <summary>
		///   A collection of folder names for folders containing content overrides.
		/// </summary>
		private static Dictionary<string, string[]> contentOverrides;

		/// <summary>
		///   A collection of the loaded packages.
		/// </summary>
		private static List<ContentPackage> loadedPackages;

		/// <summary>
		///   Initializes static members of the <see cref="ContentPackageManager"
		///   /> class.
		/// </summary>
		static ContentPackageManager()
		{
			loadedPackages = new List<ContentPackage>();
			contentOverrides = new Dictionary<string, string[]>();
		}

		/// <summary>
		///   Adds an override folder to the content override folders.
		/// </summary>
		/// <param name="absoluteFolderPath">The absolute path to the folder.</param>
		public static void AddOverrideFolder(string absoluteFolderPath)
		{
			if (!Directory.Exists(absoluteFolderPath)) { throw new IOException($"Tried to add an override folder to the content package manager, but the folder doesn't exist or the path is invalid (Path: {absoluteFolderPath})"); }

			string[] filePathsInFolder = Directory.GetFiles(absoluteFolderPath, "*", SearchOption.AllDirectories).Where(p => !p.ToLowerInvariant().EndsWith(".txt")).ToArray();
			contentOverrides.Add(absoluteFolderPath, filePathsInFolder);
		}

		/// <summary>
		///   Adds a package to this manager.
		/// </summary>
		/// <param name="settingsPath">
		///   The path to the package's settings file.
		/// </param>
		public static void AddPackage(string settingsPath)
		{
			ContentPackage package = new ContentPackage();
			package.Load(settingsPath);
			loadedPackages.Add(package);
		}

		/// <summary>
		///   Adds a package to this manager.
		/// </summary>
		/// <param name="settingsFolder">
		///   The absolute path to the package's folder.
		/// </param>
		public static void AddPackageFromFolder(string settingsFolder)
		{
			ContentPackage package = new ContentPackage();
			package.LoadFromFolder(settingsFolder);
			loadedPackages.Add(package);
		}

		/// <summary>
		///   Clears all loaded content override folders.
		/// </summary>
		public static void ClearOverrideFolders()
		{
			contentOverrides.Clear();
		}

		/// <summary>
		///   Gets the absolute file path of a resource, given the resource's name.
		/// </summary>
		/// <param name="resourceName">The name of the resource.</param>
		/// <returns>The resource's absolute file path.</returns>
		public static string GetAbsoluteFilePath(string resourceName)
		{
			string resourcePath = "";
			int i = 0;

			// TODO: add content override support

			while (resourcePath == "")
			{
				resourcePath = loadedPackages[i].GetResourcePath(resourceName);
				i++;
				if (i == loadedPackages.Count && resourcePath == "")
				{
					throw new ResourceNotFoundException(string.Format("No resource named {0} exists in any loaded package.", resourceName), ContentPackageManager.loadedPackages);
				}
			}

			return resourcePath;
		}

		/// <summary>
		///   Returns a loaded graphics object from a content package given a
		///   resource name. This method will do a file search through all loaded
		///   content packages, but only needs to do the file search once, when
		///   the package is loaded.
		/// </summary>
		/// <param name="resourceName">The name of the resource to load.</param>
		/// <returns>A loaded graphics object.</returns>
		public static IGraphicsObject GetGraphicsResource(string resourceName)
		{
			string resourcePath = "";
			int i = 0;

			// First, search for the resource in all loaded overrides, in reverse
			// order. The enumeration is in reversed order because content
			// overrides for a level, for instance, must override content
			// overrides for a world, and so forth.
			foreach (KeyValuePair<string, string[]> contentOverride in contentOverrides.Reverse())
			{
				resourcePath = SearchOverrideForResource(contentOverride.Key, resourceName);
				if (resourcePath != "") { break; }
			}

			// If there are no content overrides, or if the resource wasn't in
			// the overrides, check the content package itself.
			while (resourcePath == "")
			{
				resourcePath = loadedPackages[i].GetResourcePath(resourceName);
				i++;
				if (i == loadedPackages.Count && resourcePath == "")
				{
					// If we've gone over every package and we still haven't
					// found the right file...
					throw new ResourceNotFoundException(string.Format("No resource named {0} exists in any loaded package.", resourceName), ContentPackageManager.loadedPackages);
				}
			}

			if (!resourcePath.EndsWith(".png"))
			{
				throw new ArgumentException(string.Format("File at {0} is not a PNG image file.", resourcePath));
			}

			return GraphicsManager.LoadGraphicsObject(resourcePath);
		}

		/// <summary>
		///   Gets a loaded sound resource given its name.
		/// </summary>
		/// <param name="resourceName">The name of the sound resource.</param>
		/// <returns>A sound resource.</returns>
		public static Sound GetSoundResource(string resourceName)
		{
			string resourcePath = "";
			int i = 0;

			// TODO: Add content override support

			while (resourcePath == "")
			{
				resourcePath = loadedPackages[i].GetResourcePath(resourceName);
				i++;
				if (i == loadedPackages.Count && resourceName == "")
				{
					throw new ResourceNotFoundException(string.Format("No resource named {0} exists in any loaded package.", resourceName), ContentPackageManager.loadedPackages);
				}
			}

			if (!resourcePath.EndsWith(".mp3"))
			{
				throw new ArgumentException(string.Format("File at {0} is not an MP3 sound file.", resourcePath));
			}

			SoundManager.AddSound(resourceName, resourcePath);
			return SoundManager.GetSound(resourceName);
		}

		private static string SearchOverrideForResource(string overrideFolderPath, string resourceName)
		{
			string[] filePaths = contentOverrides[overrideFolderPath];

			Func<string, bool> searchFunc = (s) =>
			{
				string fileName = Path.GetFileNameWithoutExtension(s);
				return fileName.ToLowerInvariant().Equals(resourceName.ToLowerInvariant(), StringComparison.InvariantCultureIgnoreCase);
			};

			var matchingFilePaths = filePaths.Where(searchFunc);
			int matchingFileCount = matchingFilePaths.Count();
			if (matchingFileCount > 1) { throw new ArgumentException($"Multiple files in a content override named {resourceName}."); }
			else if (matchingFileCount == 0) { return ""; }
			else { return matchingFilePaths.First(); }
		}
	}
}
