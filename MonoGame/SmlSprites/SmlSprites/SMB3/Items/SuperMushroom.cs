using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless.Content;
using SMLimitless.Graphics;
using SMLimitless.Physics;
using SMLimitless.Sounds;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;
using SMLimitless.Sprites.Components;
using SmlSprites.Helpers;
using SmlSprites.Players;

namespace SmlSprites.SMB3.Items
{
	public sealed class SuperMushroom : Sprite
	{
		public static PhysicsSetting<float> MovingSpeed =
			new PhysicsSetting<float>("Super Mushroom: Moving Speed (px/sec)", 0f, 150f, 35f,
				PhysicsSettingType.FloatingPoint);

		private StaticGraphicsObject graphics;
		private CachedSound playerAlreadyPoweredUpSound;

		private string[] NoEffectOnPlayersNamed = { "PlayerMarioSuper" };

		private Dictionary<string, PowerupTransitionInfo> TypeMappings =
			new Dictionary<string, PowerupTransitionInfo>
			{
				{ "Mario", new PowerupTransitionInfo("SmlSprites.Players.PlayerMarioSuper", "SMB3PlayerMarioSmallToSuper") }
			};

		public override string EditorCategory => "Items";

		public SuperMushroom()
		{
			Size = new Vector2(16f);
			graphics = (StaticGraphicsObject)ContentPackageManager.GetGraphicsResource("SMB3SuperMushroom");
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects) { }

		public override void Initialize(Section owner)
		{
			Components.Add(new WalkerComponent(this, SpriteDirection.FacePlayer, MovingSpeed.Value,
				turnOnSpriteCollisions: false));
			base.Initialize(owner);
		}

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
				// The player can be powered up by this mushroom.
				var transitionInfo = TypeMappings[matchingKey];
				Owner.PerformPowerupStateChange(sprite, transitionInfo.NewPlayerTypeFullName,
					transitionInfo.TransitionGraphicsObjectName, poweringUp: true);
			}
			else
			{
				// The player cannot be powered up by this mushroom.
				AudioPlaybackEngine.Instance.PlaySound(playerAlreadyPoweredUpSound, (sender, e) => { });
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
			playerAlreadyPoweredUpSound = new CachedSound(ContentPackageManager.GetAbsoluteFilePath("nsmbwiiReservedItem"));
		}

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
