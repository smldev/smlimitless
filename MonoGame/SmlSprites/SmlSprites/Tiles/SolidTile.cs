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

namespace SmlSprites.Tiles
{
	public sealed class SolidTile : Tile
	{
		private IGraphicsObject graphics;

		public override string EditorCategory
		{
			get
			{
				return "Standard Tiles";
			}
		}

		public SolidTile()
		{
			Size = new Vector2(16f);
			TileShape = CollidableShape.Rectangle;
			RectSolidSides = TileRectSolidSides.Top | TileRectSolidSides.Left | TileRectSolidSides.Right | TileRectSolidSides.Bottom;
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects) { }

		public override void LoadContent()
		{
			graphics = ContentPackageManager.GetGraphicsResource(GraphicsResourceName);
			graphics.LoadContent();
		}

		public override void Draw()
		{
			graphics.Draw(Position, Color.White);
		}

		public override void Update()
		{
		}

		public override object GetCustomSerializableObjects()
		{
			return null;
		}

		public override void HandleCollision(Sprite sprite, Vector2 intersect)
		{
		}
	}
}
