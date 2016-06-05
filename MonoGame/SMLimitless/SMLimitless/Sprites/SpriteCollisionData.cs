using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SMLimitless.Sprites
{
	internal sealed class SpriteCollisionData
	{
		public Sprite Owner { get; }
		public Vector2 SpriteResolvedMovement { get; set; }

		public SpriteCollisionData(Sprite owner)
		{
			Owner = owner;
			SpriteResolvedMovement = Vector2.Zero;
		}
	}
}
