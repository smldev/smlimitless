using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMLimitless.Physics;

namespace SMLimitless.Extensions
{
	public static class EnumerationExtensions
	{
		public static HorizontalDirection GetHorizontalDirection(this RtSlopedSides slopedSides)
		{
			if (slopedSides == RtSlopedSides.Default) return HorizontalDirection.None;
			return (slopedSides == RtSlopedSides.TopLeft || slopedSides == RtSlopedSides.BottomLeft) ? HorizontalDirection.Left : HorizontalDirection.Right;
		}

		public static bool IsOppositeDirection(this HorizontalDirection a, HorizontalDirection b)
		{
			if (a == HorizontalDirection.None || b == HorizontalDirection.None) return false;
			else return a != b;
		}
	}
}
