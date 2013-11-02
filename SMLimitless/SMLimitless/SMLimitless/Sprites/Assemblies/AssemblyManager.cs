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
            Assembly assembly = Assembly.LoadFile(assemblyPath);
            if (!ValidateAssembly(assembly))
            {
                throw new Exception(string.Format("AssemblyManager.LoadAssembly(string, ushort): The assembly named {0} does not meet assembly requirements.", assembly.FullName)); // TODO: get an AssemblyName here
            }

            loadedAssemblies.Add(assembly);
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
            if (assembly.GetType("AssemblyMetadata", false) == null)
            {
                return false;
            }

            return true;
        }
    }
}
