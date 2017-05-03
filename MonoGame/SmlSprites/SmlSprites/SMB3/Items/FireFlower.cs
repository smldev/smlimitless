using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless.Content;
using SMLimitless.Graphics;
using SMLimitless.Physics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SmlSprites.Helpers;

namespace SmlSprites.SMB3.Items
{
	public sealed class FireFlower : Sprite
	{
		private StaticGraphicsObject graphics;
		// TODO: why does SuperMushroom hold onto playerAlreadyPoweredUpSound?
		// we're not going to have a copy of that for every sprite
		// why aren't sounds globally cached, anyway?

		private string[] NoEffectOnPlayersNamed = { "PlayerMarioFire" };

		private Dictionary<string, PowerupTransitionInfo> TypeMappings =
			new Dictionary<string, PowerupTransitionInfo>
			{
				{ "Mario", new PowerupTransitionInfo("SmlSprites.Players.PlayerMarioFire", "SMB3PlayerMarioSuperToFire") }
			};

		public override string EditorCategory => "Items";

		public FireFlower()
		{
			Size = new Vector2(16f);
			graphics = (StaticGraphicsObject)ContentPackageManager.GetGraphicsResource("SMB3FireFlower");
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects) { }

		public override void Draw()
		{
			graphics.Draw(Position, Color.White);
		}

		public override void Draw(Rectangle cropping)
		{
			graphics.Draw(Position, cropping, Color.White, 
				Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
		}

		public override void HandleSpriteCollision(Sprite sprite, Vector2 resolutionDistance)
		{
			if (!sprite.IsPlayer) { return; }
			var spriteType = sprite.GetType();
			var spriteTypeName = spriteType.Name;
			string matchingKey;
			if (!NoEffectOnPlayersNamed.Contains(spriteTypeName) &&
				KeyContainedInTypeName(spriteTypeName, out matchingKey))
			{
				// The player can be powered up by this flower.
				var transitionInfo = TypeMappings[matchingKey];
				Owner.PerformPowerupStateChange(sprite, transitionInfo.NewPlayerTypeFullName,
					transitionInfo.TransitionGraphicsObjectName, poweringUp: true);
			}
			else
			{
				// The player cannot be powered up by this flower.
				// AudioPlaybackEngine.Instance.PlaySound(playerAlreadyPoweredUpSound, (sender, e) => { });
			}
			Owner.RemoveSpriteOnNextFrame(this);
		}

		public override object GetCustomSerializableObjects()
		{
			return new { };
		}

		public override void LoadContent()
		{
			graphics.LoadContent();
			// playerAlreadyPoweredUpSound = new CachedSound(ContentPackageManager.GetAbsoluteFilePath("nsmbwiiReservedItem"));
		}

		// TODO: there's a lot of code that should be common across all powerups. The below method
		// is an example.
		private bool KeyContainedInTypeName(string typeName, out string matchingKey)
		{
			foreach (var kvp in TypeMappings)
			{
				if (typeName.Contains(kvp.Key))
				{
					matchingKey = kvp.Key;
					return true;
				}
			}
			matchingKey = null;
			return false;
		}
	}
}
