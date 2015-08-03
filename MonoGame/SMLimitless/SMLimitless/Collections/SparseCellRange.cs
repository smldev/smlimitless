using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SMLimitless.Collections
{
	public struct SparseCellRange
	{
		public Point TopLeft { get; }
		public Point BottomRight { get; }

		public SparseCellRange(Point topLeft, Point bottomRight)
		{
			TopLeft = topLeft;
			BottomRight = bottomRight;
		}
	}
}
