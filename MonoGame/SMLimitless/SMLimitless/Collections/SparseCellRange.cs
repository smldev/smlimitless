using System;
using Microsoft.Xna.Framework;

namespace SMLimitless.Collections
{
	/// <summary>
	///   A pair of two-dimensional, zero-based indices that represent a
	///   rectangular group of sparse cells.
	/// </summary>
	public struct SparseCellRange
	{
		/// <summary>
		///   Gets the two-dimensional, zero-based index of the bottom-right
		///   corner of the range's cells.
		/// </summary>
		public Point BottomRight { get; }

		/// <summary>
		///   Gets the two-dimensional, zero-based index of the top-left corner
		///   of the range's cells.
		/// </summary>
		public Point TopLeft { get; }

		/// <summary>
		///   Initializes a new instance of the <see cref="SparseCellRange" /> struct.
		/// </summary>
		/// <param name="topLeft">
		///   The two-dimensional, zero-based index of the top-left corner of the
		///   range's cells.
		/// </param>
		/// <param name="bottomRight">
		///   The two-dimensional, zero-based index of the bottom-right corner of
		///   the range's cells.
		/// </param>
		public SparseCellRange(Point topLeft, Point bottomRight)
		{
			// TODO: add parameter validation.

			TopLeft = topLeft;
			BottomRight = bottomRight;
		}

		/// <summary>
		///   Determines if two <see cref="SparseCellRange" /> objects are
		///   inequal to each other.
		/// </summary>
		/// <param name="lhs">The first range.</param>
		/// <param name="rhs">The second range.</param>
		/// <returns>
		///   True if all properties of both sides are not equal, false if otherwise.
		/// </returns>
		public static bool operator !=(SparseCellRange lhs, SparseCellRange rhs) => !lhs.Equals(rhs);

		/// <summary>
		///   Determines if two <see cref="SparseCellRange" /> objects are equal
		///   to each other.
		/// </summary>
		/// <param name="lhs">The first range.</param>
		/// <param name="rhs">The second range.</param>
		/// <returns>
		///   True if all properties of both sides are equal, false if otherwise.
		/// </returns>
		public static bool operator ==(SparseCellRange lhs, SparseCellRange rhs) => lhs.Equals(rhs);

		/// <summary>
		///   Determines if a given object is equal to this object.
		/// </summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>
		///   True if the object is a <see cref="SparseCellRange" /> and all
		///   properties are equal. False if otherwise.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (!(obj is SparseCellRange)) { return false; }
			else
			{
				SparseCellRange range = (SparseCellRange)obj;
				return TopLeft == range.TopLeft && BottomRight == range.BottomRight;
			}
		}

		/// <summary>
		///   Determines if a given <see cref="SparseCellRange" /> is equal to
		///   this range.
		/// </summary>
		/// <param name="range">The range to compare.</param>
		/// <returns>True if all properties are equal, false if otherwise.</returns>
		public bool Equals(SparseCellRange range)
		{
			return TopLeft == range.TopLeft && BottomRight == range.BottomRight;
		}

		/// <summary>
		///   Gets a hash code for this <see cref="SparseCellRange" />.
		/// </summary>
		/// <returns>
		///   An <see cref="int" /> containing the hash code for this range.
		/// </returns>
		public override int GetHashCode()
		{
			unchecked
			{
				return 137 * TopLeft.GetHashCode() * 17 * BottomRight.GetHashCode();
			}
		}
	}
}
