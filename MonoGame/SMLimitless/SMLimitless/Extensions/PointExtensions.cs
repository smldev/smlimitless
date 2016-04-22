using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SMLimitless.Extensions
{
	/// <summary>
	/// Contains extension methods for the <see cref="Point"/> struct.
	/// </summary>
	public static class PointExtensions
	{
		/// <summary>
		/// Converts an enumerable containing <see cref="Point"/> instances into a <see cref="string"/>
		/// of form "x,y;x,y" with no trailing semicolon.
		/// </summary>
		/// <param name="points">An enumerable containing the points to be serialized.</param>
		/// <returns>A string containing the point values.</returns>
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

		/// <summary>
		/// Converts a <see cref="string"/> with the form "x,y;x,y" into a <see cref="List{T}"/> of <see cref="Point"/> instances.
		/// </summary>
		/// <param name="pointsString">A string of points.</param>
		/// <returns>A list containing the points deserialized from the string.</returns>
		/// <remarks>Handles empty entries ("x,y;;x,y") and trailing semicolons.</remarks>
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
