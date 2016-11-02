using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SMLimitless.IO.LevelSerializers.Serializer003Types
{
	internal sealed class TilePositionCloud
	{
		public int TileSaveID { get; set; }
		public List<Point> CellNumbers { get; set; }
		
		public TilePositionCloud()
		{
			CellNumbers = new List<Point>();
		}
	}
}
