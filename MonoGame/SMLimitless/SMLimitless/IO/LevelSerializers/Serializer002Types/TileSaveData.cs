using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLimitless.Sprites;

namespace SMLimitless.IO.LevelSerializers.Serializer002Types
{
	/// <summary>
	/// Represents the save data for a single unique tile.
	/// </summary>
	internal class TileSaveData
	{
		internal string TypeName { get; set; }
		internal TileCollisionType CollisionType { get; set; }
		internal string Name { get; set; }
		internal string GraphicsResource { get; set; }
		internal string State { get; set; }
		internal object CustomData { get; set; }

		public int TileSaveID { get; set; }

		internal TileSaveData()
		{
			// empty constructor for deserializer
		}

		public TileSaveData(Tile tile)
		{
			this.TypeName = tile.GetType().FullName;
			this.CollisionType = tile.Collision;
			this.Name = tile.Name;
			this.GraphicsResource = tile.GraphicsResourceName;
			this.State = tile.InitialState;
			this.CustomData = tile.GetCustomSerializableObjects();
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
				return this.TypeName == that.TypeName && this.CollisionType == that.CollisionType && this.Name == that.Name &&
					   this.GraphicsResource == that.GraphicsResource && this.State == that.State && this.CustomData == that.CustomData;
			}
		}

		public override int GetHashCode()
		{
			// Multiply the hash of every value by a prime number
			unchecked
			{
				int hash = 17;
				hash += this.TypeName.GetHashCode() * 137;
				hash += this.CollisionType.GetHashCode() * 137;
				hash += (this.Name != null) ? this.Name.GetHashCode() * 137 : 0;
				hash += this.GraphicsResource.GetHashCode() * 137;
				hash += (this.State != null) ? this.State.GetHashCode() * 137 : 0;
				hash += (this.CustomData != null) ? this.CustomData.GetHashCode() * 137 : 0;
				return hash;
			}
		}
	}
}
