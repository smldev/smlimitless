//-----------------------------------------------------------------------
// <copyright file="AssemblyManager.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SMLimitless.Extensions;
using SMLimitless.IO;

namespace SMLimitless.Sprites.Assemblies
{
	/// <summary>
	///   Loads and manages assemblies containing custom sprites, tiles, and screens.
	/// </summary>
	internal static class AssemblyManager
	{
		/// <summary>
		///   A list of all loaded assemblies.
		/// </summary>
		private static List<Assembly> loadedAssemblies;

		/// <summary>
		///   A dictionary that links sprite and tile type full names to assembly references.
		/// </summary>
		private static Dictionary<string, Assembly> typeToAssemblyDictionary;

		/// <summary>
		///   Initializes static members of the <see cref="AssemblyManager" /> class.
		/// </summary>
		static AssemblyManager()
		{
			loadedAssemblies = new List<Assembly>();
			typeToAssemblyDictionary = new Dictionary<string, Assembly>();
		}

		public static List<EditorObjectData> GetAllObjectData()
		{
			List<EditorObjectData> result = new List<EditorObjectData>();

			foreach (var loadedAssembly in loadedAssemblies)
			{
				EditorObjectData objectData = new EditorObjectData();
				string objectDataJSONBasePath = Path.GetDirectoryName(loadedAssembly.Location);

				Type assemblyMetadataType = loadedAssembly.GetType($"{loadedAssembly.GetName().Name}.AssemblyMetadata");
				var objectDataJSONPathProperty = assemblyMetadataType.GetProperty("ObjectDataJSONPath");
				string path = Path.Combine(objectDataJSONBasePath, (string)objectDataJSONPathProperty.GetValue(null));

				objectData.ReadData(path);
				result.Add(objectData);
			}

			return result;
		}

		/// <summary>
		///   Returns a fully constructed sprite instances given a sprite's full name.
		/// </summary>
		/// <param name="spriteFullName">
		///   The full name of the sprite, containing namespaces but not assembly data.
		/// </param>
		/// <returns>A fully constructed sprite instance.</returns>
		public static Sprite GetSpriteByFullName(string spriteFullName)
		{
			if (!typeToAssemblyDictionary.ContainsKey(spriteFullName))
			{
				throw new ArgumentException(string.Format("AssemblyManager.GetSpriteByFullName(string): The sprite named {0} is not present in any loaded assembly.", spriteFullName));
			}

			Assembly containingAssembly = typeToAssemblyDictionary[spriteFullName];
			return (Sprite)Activator.CreateInstance(containingAssembly.GetType(spriteFullName));
		}

		/// <summary>
		///   Returns a fully constructed Tile instance, given the full name of
		///   the type.
		/// </summary>
		/// <param name="tileFullName">
		///   The full name of the tile, containing namespaces but not assembly data.
		/// </param>
		/// <returns>A fully constructed Tile instance.</returns>
		public static Tile GetTileByFullName(string tileFullName)
		{
			if (!typeToAssemblyDictionary.ContainsKey(tileFullName))
			{
				throw new ArgumentException(string.Format("AssemblyManager.GetTileByFullName(string): The tile named {0} is not present in any loaded assembly.", tileFullName));
			}

			Assembly containingAssembly = typeToAssemblyDictionary[tileFullName];
			return (Tile)Activator.CreateInstance(containingAssembly.GetType(tileFullName));
		}

		/// <summary>
		///   Loads an assembly given a path to the assembly file.
		/// </summary>
		/// <param name="assemblyPath">The path to the assembly file.</param>
		public static void LoadAssembly(string assemblyPath)
		{
			if (!File.Exists(assemblyPath))
			{
				throw new FileNotFoundException(string.Format("AssemblyManager.LoadAssembly(string): The file at {0} doesn't exist.", assemblyPath), assemblyPath);
			}

			// Load the assembly.
			string fullPath = Path.Combine(Directory.GetCurrentDirectory(), assemblyPath);
			Assembly assembly = Assembly.LoadFile(fullPath);
			if (!ValidateAssembly(assembly))
			{
				throw new InvalidOperationException(string.Format("AssemblyManager.LoadAssembly(string, ushort): The assembly named {0} does not meet assembly requirements.", assembly.FullName));
			}

			if (loadedAssemblies.Any(a => a.FullName == assembly.FullName)) { return; }

			loadedAssemblies.Add(assembly);
			AddTypesToDictionary(assembly);
		}

		/// <summary>
		///   Adds all the sprite and tile types of a given assembly to the
		///   type/assembly dictionary.
		/// </summary>
		/// <param name="assembly">The assembly from which to add the types.</param>
		private static void AddTypesToDictionary(Assembly assembly)
		{
			var types = assembly.GetTypes().Where(t => t.InheritsFrom(typeof(Sprite)) || t.InheritsFrom(typeof(Tile)));

			foreach (Type type in types)
			{
				typeToAssemblyDictionary.Add(type.FullName, assembly);
			}
		}

		/// <summary>
		///   Determines if a loaded assembly references SMLimitless.exe and
		///   contains a type named AssemblyMetadata.
		/// </summary>
		/// <param name="assembly">The assembly to check for validity.</param>
		/// <returns>True if the assembly meets both of the conditions.</returns>
		private static bool ValidateAssembly(Assembly assembly)
		{
			if (assembly == null)
			{
				return false;
			}

			// First requirement: The assembly references SMLimitless.exe
			var references = assembly.GetReferencedAssemblies();
			if (!references.Any(r => r.Name == "SMLimitless"))
			{
				return false;
			}

			// Second requirement: The assembly contains a type named AssemblyMetadata
			if (!assembly.GetTypes().Where(t => t.Name.Contains("AssemblyMetadata")).Any())
			{
				return false;
			}

			return true;
		}
	}
}
