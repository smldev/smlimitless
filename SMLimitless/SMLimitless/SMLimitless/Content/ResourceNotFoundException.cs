//-----------------------------------------------------------------------
// <copyright file="ResourceNotFoundException.cs" company="Chris Akridge">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SMLimitless.Content
{
    /// <summary>
    /// Represents an error that occurs when a resource
    /// could not be found in the loaded ContentPackages
    /// in ContentPackageManager.
    /// </summary>
    public sealed class ResourceNotFoundException : Exception
    {
        /// <summary>
        /// Gets a read-only collection of data concerning content packages
        /// loaded in ContentPackageManager.
        /// </summary>
        public ReadOnlyCollection<string> LoadedPackagesStrings { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
        /// </summary>
        public ResourceNotFoundException() : base()
        {
            this.LoadedPackagesStrings = new ReadOnlyCollection<string>(new List<string> { });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ResourceNotFoundException(string message) : base(message)
        {
            this.LoadedPackagesStrings = new ReadOnlyCollection<string>(new List<string> { });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ResourceNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
            this.LoadedPackagesStrings = new ReadOnlyCollection<string>(new List<string> { });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        /// <param name="loadedPackages">A collection of content packages to retrieve data from.</param>
        internal ResourceNotFoundException(string message, Exception innerException, IList<ContentPackage> loadedPackages) : base(message, innerException)
        {
            List<string> packageStrings = new List<string>(loadedPackages.Count);
            foreach (ContentPackage package in loadedPackages)
            {
                packageStrings.Add(package.ToString());
            }

            this.LoadedPackagesStrings = new ReadOnlyCollection<string>(packageStrings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="loadedPackages">A collection of content packages to retrieve data from.</param>
        internal ResourceNotFoundException(string message, IList<ContentPackage> loadedPackages) : this(message, null, loadedPackages)
        {
        }
    }
}
