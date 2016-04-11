using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Physics
{
	/// <summary>
	/// A structure containing data about how a section should automatically scroll.
	/// </summary>
	public struct SectionAutoscrollSettings
	{
		private Vector2 speed;
		private string pathName;

		/// <summary>
		/// Gets a value indicating how the section automatically scrolls.
		/// </summary>
		public CameraScrollType ScrollType { get; internal set; }
		
		/// <summary>
		/// Gets a value indicating how fast the section automatically scrolls.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown in the getter and setter if this section is not automatically scrolling.</exception>
		public Vector2 Speed
		{
			get
			{
				if (ScrollType != CameraScrollType.AutoScroll) { throw new InvalidOperationException("This section is not autoscrolling."); }

				return speed;
			}
			internal set
			{
				if (ScrollType != CameraScrollType.AutoScroll) { throw new InvalidOperationException("This section is not autoscrolling."); }

				speed = value;
			}
		}

		/// <summary>
		/// Gets the name of the path that this section is automatically scrolling along.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown in the getter and setter if this section is not automatically scrolling along a path.</exception>
		public string PathName
		{
			get
			{
				if (ScrollType != CameraScrollType.AutoScrollAlongPath) { throw new InvalidOperationException("This section is not autoscrolling."); }

				return pathName;
			}
			internal set
			{
				if (ScrollType != CameraScrollType.AutoScrollAlongPath) { throw new InvalidOperationException("This section is not autoscrolling."); }

				pathName = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SectionAutoscrollSettings"/> struct.
		/// </summary>
		/// <param name="scrollType">A value indicating how the section automatically scrolls.</param>
		/// <param name="speed">A value indicating how fast the section automatically scrolls.</param>
		/// <param name="pathName">The name of the path that this section is automatically scrolling along.</param>
		public SectionAutoscrollSettings(CameraScrollType scrollType, Vector2 speed, string pathName)
		{
			ScrollType = scrollType;
			this.speed = speed;
			this.pathName = pathName;
		}
	}
}
