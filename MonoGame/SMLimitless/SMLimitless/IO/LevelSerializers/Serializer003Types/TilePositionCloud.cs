using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SMLimitless.IO.LevelSerializers.Serializer003Types
{
	internal sealed class TilePositionCloud
	{
		public List<Point> CellNumbers { get; set; }
		public int TileSaveID { get; set; }

		public TilePositionCloud()
		{
			CellNumbers = new List<Point>();
		}
	}
}
