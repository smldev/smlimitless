using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMLimitless.IO;

namespace SMLimitless.Content
{
	internal static class OverrideReader
	{
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
