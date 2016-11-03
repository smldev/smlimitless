using System;
using SMLimitless.Physics;
using SMLimitless.Sprites;

namespace SMLimitless.IO.LevelSerializers.Serializer003Types
{
	internal sealed class TileSaveData
	{
		public int TileSaveID { get; set; }
		internal object CustomData { get; set; }
		internal string GraphicsResourceName { get; set; }
		internal string InitialState { get; set; }
		internal string Name { get; set; }
		internal int SolidSides { get; set; }
		internal string TypeName { get; set; }

		public TileSaveData(Tile tile)
		{
			TypeName = tile.GetType().FullName;
			SolidSides = (tile.TileShape == CollidableShape.Rectangle) ? (int)tile.RectSolidSides : (int)tile.TriSolidSides;
			Name = tile.Name;
			GraphicsResourceName = tile.GraphicsResourceName;
			InitialState = tile.InitialState;
			CustomData = tile.GetCustomSerializableObjects();
		}

		// Is an instance of TileRectSolidSides or TileTriSolidSides based on
		// what the tile is
		internal TileSaveData() { }

		public override bool Equals(object obj)
		{
			if (!(obj is TileSaveData) || obj == null) { return false; }
			else
			{
				TileSaveData that = (TileSaveData)obj;

				return TypeName == that.TypeName && SolidSides == that.SolidSides && Name == that.Name &&
					   GraphicsResourceName == that.GraphicsResourceName && InitialState == that.InitialState &&
					   (CustomData != null) ? (CustomData.Equals(that.CustomData)) : (that.CustomData == null);
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
				hash += (GraphicsResourceName != null) ? GraphicsResourceName.GetHashCode() * 137 : 137;
				hash += (InitialState != null) ? InitialState.GetHashCode() * 137 : 137;
				hash += (CustomData != null) ? CustomData.GetHashCode() * 137 : 137;
				return hash;
			}
		}
	}
}
