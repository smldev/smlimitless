using System;
using SMLimitless.Physics;

namespace SMLimitless.Extensions
{
	/// <summary>
	///   Contains extension methods for certain enumerations.
	/// </summary>
	public static class EnumerationExtensions
	{
		/// <summary>
		///   Gets the horizontal direction for a <see cref="RtSlopedSides" /> instance.
		/// </summary>
		/// <param name="slopedSides">
		///   The sloped sides to get the direction for.
		/// </param>
		/// <returns>
		///   Returns Left for TopLeft/BottomLeft and Right for TopRight/BottomRight.
		/// </returns>
		public static HorizontalDirection GetHorizontalDirection(this RtSlopedSides slopedSides)
		{
			if (slopedSides == RtSlopedSides.Default) return HorizontalDirection.None;
			return (slopedSides == RtSlopedSides.TopLeft || slopedSides == RtSlopedSides.BottomLeft) ? HorizontalDirection.Left : HorizontalDirection.Right;
		}

		/// <summary>
		///   Determines if two directions are opposite.
		/// </summary>
		/// <param name="a">The first direction.</param>
		/// <param name="b">The second direction.</param>
		/// <returns>
		///   True if <paramref name="a" /> is in a different direction than
		///   <paramref name="b" />.
		/// </returns>
		/// <remarks>
		///   If either parameter is <see cref="HorizontalDirection.None" />, the
		///   result is always False.
		/// </remarks>
		public static bool IsOppositeDirection(this HorizontalDirection a, HorizontalDirection b)
		{
			if (a == HorizontalDirection.None || b == HorizontalDirection.None) return false;
			else return a != b;
		}
	}
}
