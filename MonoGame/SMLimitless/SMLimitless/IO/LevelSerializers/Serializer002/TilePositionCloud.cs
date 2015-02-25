using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SMLimitless.IO.LevelSerializers.Serializer002
{
	internal class TilePositionCloud
	{
		public int TileSaveID { get; set; }

		public List<Vector2> Positions { get; set; }

		public TilePositionCloud()
		{
			this.Positions = new List<Vector2>();
		}
	}
}
