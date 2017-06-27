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
using SMLimitless.Sprites.Components;

namespace SmlSprites.SMB.Exits
{
	public sealed class Flagpole : Sprite
	{
		// The PNG for the flagpole (16px wide) is wider than its hitbox (2px wide). As a result,
		// we need to draw the graphic 7 pixels to the left of the flagpole's Position.X value.
		private const float FlagpoleDrawXOffset = -8f;
		// How many pixels to the right of Position.X to draw the flag.
		private const float FlagXOffset = 9f;
		// How many pixels below Position.Y to draw the flag when it's at the top.
		private const float FlagYHighOffset = 9f;
		// How many pixel below Position.Y to draw the flag when it's at the bottom.
		private const float FlagYLowOffset = 143f;

		public static PhysicsSetting<int> FlagDropLength = new PhysicsSetting<int>(
			"SMB Flagpole: Flag Drop Length (frames)", 1, 300, 90, PhysicsSettingType.Integer);

		public static float FlagDropYOffsetPerFrame
		{
			get
			{
				float flagDropDistance = FlagYLowOffset - FlagYHighOffset;
				return flagDropDistance / FlagDropLength.Value;
			}
		}

		private StaticGraphicsObject flagpoleGraphics;
		private AnimatedGraphicsObject flagGraphics;
		private bool flagDropping;
		private bool exitTriggered;
		private Sprite triggeringPlayer;

		// Stores the Y-position of the flag relative to the position at the top of the flag's range
		// (9 pixels from the very top of the flagpole).
		private float flagYPosition = 0f;

		private Vector2 FlagPosition => new Vector2(Position.X + FlagXOffset + FlagpoleDrawXOffset, 
			Position.Y + FlagYHighOffset + flagYPosition);

		public override string EditorCategory => "Level Exits";

		public Flagpole()
		{
			Size = new Vector2(2f, 176f);
			flagpoleGraphics = (StaticGraphicsObject)ContentPackageManager.GetGraphicsResource("SMBFlagpole");
			flagGraphics = (AnimatedGraphicsObject)ContentPackageManager.GetGraphicsResource("SMBFlag");
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects) { }

		public override void LoadContent()
		{
			flagpoleGraphics.LoadContent();
			flagGraphics.LoadContent();
		}

		public override void Draw()
		{
			flagpoleGraphics.Draw(new Vector2(Position.X + FlagpoleDrawXOffset, Position.Y), Color.White);
			flagGraphics.Draw(FlagPosition, Color.White);

			if (triggeringPlayer != null) { triggeringPlayer.Draw(); }
		}

		public override void Draw(Rectangle cropping)
		{
			// This sprite doesn't need cropping.
			Draw();
		}

		public override void Update()
		{
			flagGraphics.Update();
			if (flagDropping)
			{
				flagYPosition += FlagDropYOffsetPerFrame;

				float playerPositionX = triggeringPlayer.Position.X;
				float playerPositionY = triggeringPlayer.Position.Y;
				if (playerPositionY < (Position.Y + FlagYLowOffset))
				{
					playerPositionY += FlagDropYOffsetPerFrame;
				}

				triggeringPlayer.Position = new Vector2(playerPositionX, playerPositionY);
				triggeringPlayer.Update();

				if (flagYPosition > FlagYLowOffset && !exitTriggered)
				{
					flagYPosition = FlagYLowOffset;
					flagDropping = false;

					// triggeringPlayer.GetComponent<TriggersLevelExitsComponent>().ExitTriggered(this);
					// trigger the section's method here
					Owner.OnLevelExit();
					exitTriggered = true;
				}
			}
		}

		public override void HandleSpriteCollision(Sprite sprite, Vector2 resolutionDistance)
		{
			var levelExitComponent = sprite.GetComponent<TriggersLevelExitsComponent>();

			if (levelExitComponent != null)
			{
				triggeringPlayer = sprite;
				Owner.RemoveSpriteOnNextFrame(sprite);
				levelExitComponent.ExitTriggered(this);
				flagDropping = true;
			}
		}

		public override object GetCustomSerializableObjects()
		{
			return new { };
		}
	}
}
