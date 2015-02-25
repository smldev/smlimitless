using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLimitless.Sprites;

namespace SMLimitless.IO.LevelSerializers.Serializer002
{
	/// <summary>
	/// Represents the save data for a single unique tile.
	/// </summary>
	internal class TileSaveData
	{
		private string typeName;
		private TileCollisionType collisionType;
		private string name;
		private string graphicsResource;
		private string state;
		private object customData;

		public int TileSaveID { get; set; }

		public TileSaveData(Tile tile)
		{
			this.typeName = tile.GetType().FullName;
			this.collisionType = tile.Collision;
			this.name = tile.Name;
			this.graphicsResource = tile.GraphicsResourceName;
			this.state = tile.InitialState;
			this.customData = tile.GetCustomSerializableObjects();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is TileSaveData) || obj == null)
			{
				return false;
			}
			else
			{
				TileSaveData that = (TileSaveData)obj;
				return this.typeName == that.typeName && this.collisionType == that.collisionType && this.name == that.name &&
					   this.graphicsResource == that.graphicsResource && this.state == that.state && this.customData == that.customData;
			}
		}

		public override int GetHashCode()
		{
			// Multiply the hash of every value by a prime number
			unchecked
			{
				int hash = 17;
				hash += this.typeName.GetHashCode() * 137;
				hash += this.collisionType.GetHashCode() * 137;
				hash += (this.name != null) ? this.name.GetHashCode() * 137 : 0;
				hash += this.graphicsResource.GetHashCode() * 137;
				hash += (this.state != null) ? this.state.GetHashCode() * 137 : 0;
				hash += (this.customData != null) ? this.customData.GetHashCode() * 137 : 0;
				return hash;
			}
		}
	}
}
