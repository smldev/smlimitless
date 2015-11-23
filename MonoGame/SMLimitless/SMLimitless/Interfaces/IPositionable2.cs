//-----------------------------------------------------------------------
// <copyright file="IPositionable2.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SMLimitless.Interfaces
{
	/// <summary>
	/// Represents an object with a position and size in the world.
	/// </summary>
	public interface IPositionable2
	{
		/// <summary>
		/// Gets the position of this object in the world.
		/// </summary>
		Vector2 Position { get; }

		/// <summary>
		/// Gets the size of this object in the world.
		/// </summary>
		Vector2 Size { get; }

		/// <summary>
		/// Gets a value indicating whether this object has moved in the current frame.
		/// </summary>
		bool HasMoved { get; set; }
	}
}
