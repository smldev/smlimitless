//-----------------------------------------------------------------------
// <copyright file="ContentPackageResourceSearcher.cs" company="Chris Akridge">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SMLimitless.Content
{
    /// <summary>
    /// Searches for files in a content package given
    /// the file name of the resource. Also caches search
    /// results in order to reduce processing time.
    /// </summary>
    internal sealed class ContentPackageResourceSearcher
    {
        /// <summary>
        /// The ContentPackage that owns this searcher.
        /// </summary>
        private ContentPackage owner;

        /// <summary>
        /// A dictionary that maps resource names (keys)
        /// to file paths (values).
        /// </summary>
        private Dictionary<string, string> filePaths;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPackageResourceSearcher"/> class.
        /// </summary>
        /// <param name="owner">The ContentPackage that owns this searcher.</param>
        internal ContentPackageResourceSearcher(ContentPackage owner)
        {
            this.owner = owner;
            this.filePaths = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets a file path to a resource given the resource name.
        /// </summary>
        /// <param name="resourceName">The name of the resource.</param>
        /// <returns>A path to the resource.</returns>
        internal string GetResourcePath(string resourceName)
        {
            if (!this.filePaths.ContainsKey(resourceName))
            {
                string packagePath = this.owner.BaseFolderPath;
                string[] matchingFilePaths = Directory.GetFiles(packagePath, string.Concat("*", resourceName, "*.*"), SearchOption.AllDirectories);

                if (matchingFilePaths.Length > 1)
                {
                    throw new Exception(string.Format("ContentPackageResourceSearcher.GetPathToResource(string): Multiple files with resource name {0}.", resourceName));
                }
                else if (matchingFilePaths.Length == 0)
                {
                    return "";
                }

                this.filePaths.Add(resourceName, matchingFilePaths[0]);
            }

            return this.filePaths[resourceName];
        }
    }
}
