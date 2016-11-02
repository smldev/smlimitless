using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SMLimitless.IO;

namespace SMLimitless.Content
{
	/// <summary>
	///   Provides support for reading content override description files.
	/// </summary>
	internal static class OverrideReader
	{
		/// <summary>
		///   Reads a list of overrides from a content override description file.
		/// </summary>
		/// <param name="overridesFilePath">
		///   The path to the content override description file.
		/// </param>
		/// <returns>
		///   A dictionary mapping level filenames to lists of folder names.
		/// </returns>
		public static Dictionary<string, List<string>> GetOverridesFromFile(string overridesFilePath)
		{
			if (!File.Exists(overridesFilePath)) { throw new FileNotFoundException($"Tried to load a content overrides file, but it doesn't exist or the path is invalid. (Path: {overridesFilePath})"); }

			Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
			DataReader overrideReader = new DataReader(overridesFilePath);
			var overrideSection = overrideReader.ReadFullSection("[Overrides]");

			foreach (var overrideLevelEntry in overrideSection)
			{
				result.Add(overrideLevelEntry.Key, overrideLevelEntry.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList());
			}

			return result;
		}
	}
}
