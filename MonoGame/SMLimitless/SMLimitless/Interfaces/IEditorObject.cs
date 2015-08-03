//-----------------------------------------------------------------------
// <copyright file="IEditorObject.cs" company="The Limitless Development Team">
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
    /// Defines an object used in the level or world editor. Incomplete.
    /// </summary>
    public interface IEditorObject
    {
        /// <summary>
        /// Gets the label for the editor to use for this object.
        /// </summary>
        string EditorLabel { get; }
    }
}
