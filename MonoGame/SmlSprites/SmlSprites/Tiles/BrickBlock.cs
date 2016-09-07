using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless;
using SMLimitless.Content;
using SMLimitless.Graphics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Collections;

namespace SmlSprites.Tiles
{
	public sealed class BrickBlock : ItemBlock
	{
		/// <summary>
		/// A value indicating whether this brick block is currently or has ever stored a sprite.
		/// </summary>
		private bool hadContainedSprite;

		private StaticGraphicsObject brickGraphics;
		private StaticGraphicsObject emptyGraphics;

		public override string EditorCategory => "Interactive Tiles";
		public override float SurfaceFriction => 1000f;

		public override void Initialize(Section owner)
		{
			brickGraphics = (StaticGraphicsObject)ContentPackageManager.GetGraphicsResource("SMBBrickBlock");
			emptyGraphics = (StaticGraphicsObject)ContentPackageManager.GetGraphicsResource("SMBEmptyBlock");

			SetGraphicsObjects(brickGraphics, emptyGraphics);
			base.Initialize(owner);
		}

		public override void HandleCollision(Sprite sprite, Vector2 intersect)
		{
			if (!IsEmpty && hadContainedSprite)
			{
				base.HandleCollision(sprite, intersect);
			}
			else if (IsEmpty && hadContainedSprite)
			{
				return;
			}
			else
			{
				// This brick block is just a normal brick block and thus can be broken.
				// TODO: this should only break the block iff the player is Super or higher

				if (sprite.IsPlayer)
				{
					Vector2 playerCenter = sprite.Hitbox.Center;
					float playerYVelocity = sprite.Velocity.Y;
					float thisBottom = Hitbox.Bounds.Bottom;
					float thisTop = Hitbox.Bounds.Top;
					bool isGroundPounding = sprite.HasAttribute("GroundPounding");

					if (playerCenter.Y > thisBottom)
					{
						BreakBlock();
					}
					else if (playerCenter.Y < thisTop && isGroundPounding)
					{
						BreakBlock();
					}
				}
				else if (sprite.HasAttribute("ShellSpinning"))
				{
					Vector2 spriteCenter = sprite.Hitbox.Center;
					float spriteXVelocity = sprite.Velocity.X;
					float thisLeft = Hitbox.Bounds.Left;
					float thisRight = Hitbox.Bounds.Right;

					if (spriteCenter.X < thisLeft || spriteCenter.X > thisRight)
					{
						BreakBlock();
					}
				}
			}
		}

		private void BreakBlock()
		{
			Owner.RemoveTile(this);
			Vector2[] particleVelocities = new Vector2[]
			{
				new Vector2(-10f, -100f),
				new Vector2(10f, -100f),
				new Vector2(-10f, 100f),
				new Vector2(10f, 100f)
			};

			Vector2[] particlePositions = new Vector2[]
			{
				Position,
				new Vector2(Position.X + (Hitbox.Bounds.Width / 2f), Position.Y),
				new Vector2(Position.X, Position.Y + (Hitbox.Bounds.Height / 2f)),
				new Vector2(Position.X + (Hitbox.Bounds.Width / 2f), Position.Y + (Hitbox.Bounds.Height / 2f))
			};

			for (int i = 0; i < 4; i++)
			{
				Owner.AddParticle(new Particle(Owner, "SMBBrickParticle", particlePositions[i], particleVelocities[i], true, 3f));
			}
		}

		public override bool OnEditorDrop(Sprite sprite)
		{
			hadContainedSprite = true;
			return base.OnEditorDrop(sprite);
		}
	}
}
