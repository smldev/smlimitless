using System;
using Newtonsoft.Json.Linq;
using SMLimitless.Extensions;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;

namespace SMLimitless.IO.LevelSerializers
{
	/// <summary>
	///   Serializes and deserializes individual sprites without using the
	///   LevelSerializer types.
	/// </summary>
	public static class TypeSerializer
	{
		/// <summary>
		///   Deserializes a sprite, given a <see cref="JsonHelper" /> and a key
		///   for the sprite.
		/// </summary>
		/// <param name="helper">
		///   The <see cref="JsonHelper" /> that contains the sprite to deserialize
		/// </param>
		/// <param name="key">The key in which the sprite is stored.</param>
		/// <returns>A deserialized (but uninitalized) sprite.</returns>
		public static Sprite DeserializeSprite(JsonHelper helper, string key = "")
		{
			if (string.IsNullOrEmpty(key))
			{
				return DeserializeSprite((JObject)helper.Value);
			}
			else
			{
				if (helper.Value[key].Type == JTokenType.Null) { return null; }
				return DeserializeSprite((JObject)helper.Value[key]);
			}
		}

		/// <summary>
		///   Gets the objects for serialization for a given sprite.
		/// </summary>
		/// <param name="sprite">The sprite to get the objects for.</param>
		/// <returns>An anonymous object containing the objects for serialization.</returns>
		public static object GetSpriteObjects(Sprite sprite)
		{
			if (sprite == null) { return null; }

			return new
			{
				typeName = sprite.GetType().FullName,
				position = sprite.InitialPosition.Serialize(),
				isActive = sprite.IsActive,
				state = (int)sprite.InitialState,
				collision = (int)sprite.TileCollisionMode,
				name = sprite.Name,
				message = sprite.Message,
				isHostile = sprite.IsHostile,
				isMoving = sprite.IsMoving,
				direction = (int)sprite.Direction,
				customObjects = sprite.GetCustomSerializableObjects()
			};
		}

		private static Sprite DeserializeSprite(JObject obj)
		{
			string typeName = (string)obj["typeName"];
			Sprite result = AssemblyManager.GetSpriteByFullName(typeName);

			result.InitialPosition = result.Position = obj["position"].ToVector2();
			result.IsActive = (bool)obj["isActive"];
			result.InitialState = result.State = (SpriteState)(int)obj["state"];
			result.TileCollisionMode = (SpriteCollisionMode)(int)obj["collision"];
			result.Name = (string)obj["name"];
			result.Message = (string)obj["message"];
			result.IsHostile = (bool)obj["isHostile"];
			result.IsMoving = (bool)obj["isMoving"];
			result.Direction = (SpriteDirection)(int)obj["direction"];
			result.DeserializeCustomObjects(new JsonHelper(obj["customObjects"]));

			return result;
		}
	}
}
