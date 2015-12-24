using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Interfaces;
using SMLimitless.Physics;

namespace SMLimitless.Extensions
{
	public static class PositionableExtensions
	{
		/// <summary>
		/// Returns a rectangle that can contain all the given positionables in an enumerable.
		/// </summary>
		/// <param name="positionables">The enumerable containing the positionables.</param>
		/// <returns></returns>
		public static BoundingRectangle GetBoundsOfPositionables(this IEnumerable<IPositionable2> positionables)
		{
			if (!positionables.Any()) { return BoundingRectangle.NaN; }

			// so there's really no elegant initial value for result, except the bounds of First()...
			var first = positionables.First();
			BoundingRectangle result = new BoundingRectangle(first.Position, first.Size + first.Position);

			foreach (var positionable in positionables.Skip(1))	// ...so we need to treat it specially
			{
				BoundingRectangle bounds = new BoundingRectangle(positionable.Position, positionable.Size + positionable.Position);
				
				if (bounds.Left < result.Left)
				{
					result.Width += (bounds.X + result.X);
					result.X = bounds.X;
				}
				else if (bounds.Right > result.Right)
				{
					result.Width = (bounds.X + bounds.Width);
				}

				if (bounds.Top < result.Top)
				{
					result.Height = (bounds.Y + result.Y);
					result.Y = bounds.Y;
				}
				else if (bounds.Bottom > result.Bottom)
				{
					result.Height = (bounds.Y + bounds.Height);
				}
			}

			return result;
		}
	}
}
