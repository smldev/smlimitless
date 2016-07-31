using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SMLimitless.Extensions;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;

namespace SMLimitless.IO.LevelSerializers
{
	public static class TypeSerializer
	{
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
	}
}
