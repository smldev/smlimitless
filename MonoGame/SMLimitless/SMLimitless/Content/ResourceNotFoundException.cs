//-----------------------------------------------------------------------
// <copyright file="ResourceNotFoundException.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SMLimitless.Content
{
	/// <summary>
	///   Represents an error that occurs when a resource could not be found in
	///   the loaded ContentPackages in ContentPackageManager.
	/// </summary>
	[Serializable]
	public class ResourceNotFoundException : Exception, ISerializable
	{
		/// <summary>
		///   Gets a read-only collection of data concerning content packages
		///   loaded in ContentPackageManager.
		/// </summary>
		public ReadOnlyCollection<string> LoadedPackagesStrings { get; private set; }

		/// <summary>
		///   Initializes a new instance of the <see
		///   cref="ResourceNotFoundException" /> class.
		/// </summary>
		public ResourceNotFoundException() : base()
		{
			LoadedPackagesStrings = new ReadOnlyCollection<string>(new List<string> { });
		}

		/// <summary>
		///   Initializes a new instance of the <see
		///   cref="ResourceNotFoundException" /> class.
		/// </summary>
		/// <param name="message">
		///   The error message that explains the reason for the exception.
		/// </param>
		public ResourceNotFoundException(string message) : base(message)
		{
			LoadedPackagesStrings = new ReadOnlyCollection<string>(new List<string> { });
		}

		/// <summary>
		///   Initializes a new instance of the <see
		///   cref="ResourceNotFoundException" /> class.
		/// </summary>
		/// <param name="message">
		///   The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		///   The exception that is the cause of the current exception, or a null
		///   reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		public ResourceNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
			LoadedPackagesStrings = new ReadOnlyCollection<string>(new List<string> { });
		}

		/// <summary>
		///   Initializes a new instance of the <see
		///   cref="ResourceNotFoundException" /> class.
		/// </summary>
		/// <param name="message">
		///   The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		///   The exception that is the cause of the current exception, or a null
		///   reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		/// <param name="loadedPackages">
		///   A collection of content packages to retrieve data from.
		/// </param>
		internal ResourceNotFoundException(string message, Exception innerException, IList<ContentPackage> loadedPackages) : base(message, innerException)
		{
			List<string> packageStrings = new List<string>(loadedPackages.Count);
			foreach (ContentPackage package in loadedPackages)
			{
				packageStrings.Add(package.ToString());
			}

			LoadedPackagesStrings = new ReadOnlyCollection<string>(packageStrings);
		}

		/// <summary>
		///   Initializes a new instance of the <see
		///   cref="ResourceNotFoundException" /> class.
		/// </summary>
		/// <param name="message">
		///   The error message that explains the reason for the exception.
		/// </param>
		/// <param name="loadedPackages">
		///   A collection of content packages to retrieve data from.
		/// </param>
		internal ResourceNotFoundException(string message, IList<ContentPackage> loadedPackages) : this(message, null, loadedPackages)
		{
		}

        /// <summary>
        /// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the target object. 
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization. </param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Message", Message);

            int i = 0;
            foreach (var package in LoadedPackagesStrings)
            {
                info.AddValue($"Package{i}", package);
                i++;
            }

            base.GetObjectData(info, context);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) { throw new ArgumentNullException("info"); }

            GetObjectData(info, context);
        }
	}
}
