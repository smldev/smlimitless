//-----------------------------------------------------------------------
// <copyright file="ContentPackageManager.cs" company="Chris Akridge">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLimitless.Graphics;

namespace SMLimitless.Content
{
    /// <summary>
    /// Loads and manages a collection of ContentPackages.
    /// </summary>
    public static class ContentPackageManager
    {
        /// <summary>
        /// A collection of the loaded packages.
        /// </summary>
        private static List<ContentPackage> loadedPackages;

        /// <summary>
        /// Initializes static members of the <see cref="ContentPackageManager"/> class.
        /// </summary>
        static ContentPackageManager()
        {
            ContentPackageManager.loadedPackages = new List<ContentPackage>();
        }

        /// <summary>
        /// Adds a package to this manager.
        /// </summary>
        /// <param name="settingsPath">The path to the package's settings file.</param>
        public static void AddPackage(string settingsPath)
        {
            ContentPackage package = new ContentPackage();
            package.Load(settingsPath);
            loadedPackages.Add(package);
        }

        /// <summary>
        /// Adds a package to this manager.
        /// </summary>
        /// <param name="settingsFolder">The path to the package's folder.</param>
        public static void AddPackageFromFolder(string settingsFolder)
        {
            ContentPackage package = new ContentPackage();
            package.LoadFromFolder(settingsFolder);
            loadedPackages.Add(package);
        }

        /// <summary>
        /// Returns a loaded graphics object from a content package
        /// given a resource name. This method will do a file search
        /// through all loaded content packages, but only needs to do the
        /// file search once, when the package is loaded.
        /// </summary>
        /// <param name="resourceName">The name of the resource to load.</param>
        /// <returns>A loaded graphics object.</returns>
        public static IGraphicsObject GetGraphicsResource(string resourceName)
        {
            string resourcePath = "";
            int i = 0;

            while (resourcePath == "")
            {
                resourcePath = loadedPackages[i].GetResourcePath(resourceName);
                i++;
                if (i == loadedPackages.Count && resourcePath == "")
                {
                    // If we've gone over every package and we still haven't found the right file...
                    throw new ResourceNotFoundException(string.Format("ContentPackageManager.GetGraphicsResource(string): No resource named {0} exists in any loaded package.", resourceName), ContentPackageManager.loadedPackages);
                }
            }

            if (!resourcePath.EndsWith(".png"))
            {
                throw new Exception(string.Format("ContentPackageManager.GetGraphicsResource(string): File at {0} is not a PNG image file.", resourcePath));
            }

            return GraphicsManager.LoadGraphicsObject(resourcePath);
        }

        /// <summary>
        /// Testing.
        /// </summary>
        /// <returns>A value.</returns>
        public static string Testing()
        {
            StringBuilder builder = new StringBuilder();
            AddPackage(@"D:\Documents\GitHub\smlimitless\ContentPackageTest\settings.txt");

            builder.Append(loadedPackages[0].GetResourcePath("resource1"));
            builder.Append(Environment.NewLine);
            builder.Append(loadedPackages[0].GetResourcePath("resource2"));
            builder.Append(Environment.NewLine);
            builder.Append(loadedPackages[0].GetResourcePath("resource3"));

            return builder.ToString();
        }
    }
}
