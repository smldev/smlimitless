//-----------------------------------------------------------------------
// <copyright file="IName.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless.Interfaces
{
    /// <summary>
    /// Defines an object with a name.
    /// </summary>
    public interface IName
    {
        /// <summary>
        /// Gets the name of this object.
        /// </summary>
        string Name { get; }
    }
}
