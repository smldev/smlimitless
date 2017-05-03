using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless.Content;
using SMLimitless.Graphics;
using SMLimitless.Sounds;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites.InternalSprites
{
	/// <summary>
	/// A sprite that displays the transition between two powerup states of a player.
	/// </summary>
	public sealed class PowerupTransition : Sprite
	{
		private Sprite oldPlayer;	// The player that will be replaced after the transition.
		private Sprite newPlayer;   // The player that will be displayed after the transition.
		private ComplexGraphicsObject graphics; // The graphics of the transition to played.
		private int transitionLength;   // The number of frames that the transition lasts.
		private int currentFrame;       // The frame number the transition is currently on.
		private CachedSound sound;		// The sound played (powerup or powerdown) during the transition.

		public override string EditorCategory => "Internal Sprites";

		public PowerupTransition(Sprite oldPlayer, Sprite newPlayer, string graphicsObjectName,
			int transitionLength, bool isPoweringUp)
		{
			if (oldPlayer == null)
			{
				throw new ArgumentNullException(nameof(oldPlayer), 
					"The old player sprite must not be null.");
			}
			if (newPlayer == null)
			{
				throw new ArgumentNullException(nameof(newPlayer),
					"The new player sprite must not be null.");
			}
			if (string.IsNullOrEmpty(graphicsObjectName))
			{
				throw new ArgumentException("The name of the graphics object must not be null or empty.",
					nameof(graphicsObjectName));
			}
			if (transitionLength <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(transitionLength),
					"The transition length must be a positive number of frames.");
			}

			this.oldPlayer = oldPlayer;
			this.newPlayer = newPlayer;
			this.transitionLength = transitionLength;

			graphics = (ComplexGraphicsObject)ContentPackageManager.GetGraphicsResource(graphicsObjectName);
			string soundName = (isPoweringUp) ? "nsmbwiiPowerup" : "nsmbwiiPowerdown";
			if (!isPoweringUp) { graphics.IsReversed = true; }
			sound = new CachedSound(ContentPackageManager.GetAbsoluteFilePath(soundName));

			// Position the new player at the old player's same bottom center.
			Size = oldPlayer.Size;
			Position = oldPlayer.Position;
			FacingDirection = oldPlayer.FacingDirection;
			newPlayer.Position = new Vector2(oldPlayer.Hitbox.BottomCenter.X - (newPlayer.Hitbox.Width / 2f),
				oldPlayer.Hitbox.BottomCenter.Y - newPlayer.Hitbox.Height);
			newPlayer.FacingDirection = FacingDirection;
		}

		public override void Initialize(Section owner)
		{
			base.Initialize(owner);

			owner.RemoveSpriteOnNextFrame(oldPlayer);
			owner.Players.Remove(oldPlayer);
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
		}

		public override void Draw()
		{
			float drawPositionX = Position.X - (graphics.FrameSize.X - Size.X);
			float drawPositionY = Position.Y - (graphics.FrameSize.Y - Size.Y);
			graphics.Draw(new Vector2(drawPositionX, drawPositionY), Color.White,
				(FacingDirection == SMLimitless.Direction.Left) ?
				Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally :
				Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
		}

		public override void Draw(Rectangle cropping)
		{
			float drawPositionX = Position.X - (graphics.FrameSize.X - Size.X);
			float drawPositionY = Position.Y - (graphics.FrameSize.Y - Size.Y);
			graphics.Draw(new Vector2(drawPositionX, drawPositionY), cropping, Color.White,
				(oldPlayer.Direction == SpriteDirection.Left) ?
				Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally :
				Microsoft.Xna.Framework.Graphics.SpriteEffects.None);
		}

		public override void Update()
		{
			// Play the sound, but only on the first frame.
			if (currentFrame == 0)
			{
				AudioPlaybackEngine.Instance.PlaySound(sound, (sender, e) => { });
			}
			currentFrame++;

			if (currentFrame == transitionLength)
			{
				Owner.RemoveSpriteOnNextFrame(this);
				Owner.AddSpriteOnNextFrame(newPlayer);
				Owner.Players.Add(newPlayer);
			}
			graphics.Update();
		}

		public override object GetCustomSerializableObjects()
		{
			return null;
		}

		public override void LoadContent()
		{
			graphics.LoadContent();
		}
	}
}
