//-----------------------------------------------------------------------
// <copyright file="AssemblyManager.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using SMLimitless.Extensions;

namespace SMLimitless.Sprites.Assemblies
{
    /// <summary>
    /// Loads and manages assemblies containing custom sprites, tiles, and screens.
    /// </summary>
    internal static class AssemblyManager
    {
        /// <summary>
        /// A list of all loaded assemblies.
        /// </summary>
        private static List<Assembly> loadedAssemblies;
        private static Dictionary<string, Assembly> typeToAssemblyDictionary;

        static AssemblyManager()
        {
            loadedAssemblies = new List<Assembly>();
            typeToAssemblyDictionary = new Dictionary<string, Assembly>();
        }

        /// <summary>
        /// Loads an assembly given a path to the assembly file.
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly file.</param>
        public static void LoadAssembly(string assemblyPath)
        {
            if (!File.Exists(assemblyPath))
            {
                throw new FileNotFoundException(string.Format("AssemblyManager.LoadAssembly(string): The file at {0} doesn't exist."), assemblyPath);
            }

            // Load the assembly.
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), assemblyPath);
            Assembly assembly = Assembly.LoadFile(fullPath);
            if (!ValidateAssembly(assembly))
            {
                throw new Exception(string.Format("AssemblyManager.LoadAssembly(string, ushort): The assembly named {0} does not meet assembly requirements.", assembly.FullName)); // TODO: get an AssemblyName here
            }

            loadedAssemblies.Add(assembly);
            AddTypesToDictionary(assembly);
        }

        public static Sprite GetSpriteByFullName(string spriteFullName)
        {
            if (!typeToAssemblyDictionary.ContainsKey(spriteFullName))
            {
                throw new Exception(string.Format("AssemblyManager.GetSpriteByFullName(string): The sprite named {0} is not present in any loaded assembly.", spriteFullName));
            }

            Assembly containingAssembly = typeToAssemblyDictionary[spriteFullName];
            return (Sprite)Activator.CreateInstance(containingAssembly.GetType(spriteFullName));
        }

        public static Tile GetTileByFullName(string tileFullName)
        {
            if (!typeToAssemblyDictionary.ContainsKey(tileFullName))
            {
                throw new Exception(string.Format("AssemblyManager.GetTileByFullName(string): The tile named {0} is not present in any loaded assembly.", tileFullName));
            }

            Assembly containingAssembly = typeToAssemblyDictionary[tileFullName];
            return (Tile)Activator.CreateInstance(containingAssembly.GetType(tileFullName));
        }

        /// <summary>
        /// Determines if a loaded assembly references SMLimitless.exe and
        /// contains a type named AssemblyMetadata.
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

        private static void AddTypesToDictionary(Assembly assembly)
        {
            var types = assembly.GetTypes().Where(t => t.InheritsFrom(typeof(Sprite)) || t.InheritsFrom(typeof(Tile)));

            foreach (Type type in types)
            {
                typeToAssemblyDictionary.Add(type.FullName, assembly);
            }
        }
    }
}
