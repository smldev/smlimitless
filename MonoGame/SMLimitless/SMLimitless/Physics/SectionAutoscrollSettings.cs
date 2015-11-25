using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Physics
{
	public struct SectionAutoscrollSettings
	{
		private Vector2 speed;
		private string pathName;

		public CameraScrollType ScrollType { get; }
		
		public Vector2 Speed
		{
			get
			{
				if (ScrollType != CameraScrollType.AutoScroll) { throw new InvalidOperationException("This section is not autoscrolling."); }

				return speed;
			}
		}

		public string PathName
		{
			get
			{
				if (ScrollType != CameraScrollType.AutoScrollAlongPath) { throw new InvalidOperationException("This section is not autoscrolling."); }

				return pathName;
			}
		}

		public SectionAutoscrollSettings(CameraScrollType scrollType, Vector2 speed, string pathName)
		{
			ScrollType = scrollType;
			this.speed = speed;
			this.pathName = pathName;
		}
	}
}
