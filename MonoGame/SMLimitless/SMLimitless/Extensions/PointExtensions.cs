using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SMLimitless.Extensions
{
	public static class PointExtensions
	{
		public static string SerializeCompact(this IEnumerable<Point> points)
		{
			StringBuilder resultBuilder = new StringBuilder();
			
			foreach (Point point in points)
			{
				resultBuilder.Append($"{point.X},{point.Y};");
			}

			resultBuilder.Remove(resultBuilder.Length - 2, 1);  // remove trailing semicolon
			return resultBuilder.ToString();
		}

		public static List<Point> DeserializeCompact(this string pointsString)
		{
			string[] points = pointsString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			List<Point> result = new List<Point>(points.Length);

			foreach (var point in points)
			{
				string[] components = point.Split(',');
				Point resultPoint = new Point(int.Parse(components[0]), int.Parse(components[1]));
				result.Add(resultPoint);
			}

			return result;
		}
	}
}
