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
	public interface IPositionable2
	{
		Vector2 Position { get; }
		Vector2 Size { get; }
		bool HasMoved { get; set; }
	}
}
