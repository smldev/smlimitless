using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMLimitless.Sprites;
using SMLimitless.Physics;

namespace SMLimitless.IO.LevelSerializers.Serializer003Types
{
	internal sealed class TileSaveData
	{
		internal string TypeName { get; set; }
		internal int SolidSides { get; set; }	// Is an instance of TileRectSolidSides or TileTriSolidSides based on what the tile is
		internal string Name { get; set; }
		internal string GraphicsResourceName { get; set; }
		internal string InitialState { get; set; }
		internal object CustomData { get; set; }

		public int TileSaveID { get; set; }

		internal TileSaveData() { }

		public TileSaveData(Tile tile)
		{
			TypeName = tile.GetType().FullName;
			SolidSides = (tile.TileShape == CollidableShape.Rectangle) ? (int)tile.RectSolidSides : (int)tile.TriSolidSides;
			Name = tile.Name;
			GraphicsResourceName = tile.GraphicsResourceName;
			InitialState = tile.InitialState;
			CustomData = tile.GetCustomSerializableObjects();
		}

		public bool Equals(object obj)
		{
			if (!(obj is TileSaveData) || obj == null) { return false; }
			else
			{
				TileSaveData that = (TileSaveData)obj;
				return TypeName == that.TypeName && SolidSides == that.SolidSides && Name == that.Name &&
					   GraphicsResourceName == that.GraphicsResourceName && InitialState == that.InitialState && CustomData.Equals(that.CustomData);
			}
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash += TypeName.GetHashCode() * 137;
				hash += SolidSides.GetHashCode() * 137;
				hash += (Name != null) ? Name.GetHashCode() * 137 : 137;
				hash += GraphicsResourceName.GetHashCode() * 137;
				hash += (InitialState != null) ? InitialState.GetHashCode() * 137 : 137;
				hash += (CustomData != null) ? CustomData.GetHashCode() * 137 : 137;
				return hash;
			}
		}
	}
}
