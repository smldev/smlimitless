using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SMLimitless.Collections
{
	/// <summary>
	/// A pair of two-dimensional, zero-based indices that represent a rectangular group of sparse cells.
	/// </summary>
	public struct SparseCellRange
	{
		/// <summary>
		/// Gets the two-dimensional, zero-based index of the top-left corner of the range's cells.
		/// </summary>
		public Point TopLeft { get; }

		/// <summary>
		/// Gets the two-dimensional, zero-based index of the bottom-right corner of the range's cells.
		/// </summary>
		public Point BottomRight { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SparseCellRange"/> struct.
		/// </summary>
		/// <param name="topLeft">The two-dimensional, zero-based index of the top-left corner of the range's cells.</param>
		/// <param name="bottomRight">The two-dimensional, zero-based index of the bottom-right corner of the range's cells.</param>
		public SparseCellRange(Point topLeft, Point bottomRight)
		{
			// TODO: add parameter validation

			TopLeft = topLeft;
			BottomRight = bottomRight;
		}
	}
}
