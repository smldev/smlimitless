using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless;
using SMLimitless.Content;
using SMLimitless.Graphics;
using SMLimitless.Physics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;

namespace SmlSample
{
	public sealed class TestTile3 : Tile
	{
		private StaticGraphicsObject graphics;

		public override string EditorCategory
		{
			get
			{
				return "Test Items";
			}
		}

		public override float SurfaceFriction => 1000f;

		public TestTile3()
		{
			Size = new Vector2(16f);
			TileShape = CollidableShape.Rectangle;
			RectSolidSides = TileRectSolidSides.Top | TileRectSolidSides.Left | TileRectSolidSides.Right | TileRectSolidSides.Bottom;
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
		}

		public override void Draw()
		{
			graphics.Draw(Position, Color.White);
		}

		public override void HandleCollision(Sprite sprite, Vector2 intersect)
		{
		}

		public override void Initialize(Section owner)
		{
			base.Initialize(owner);
		}

		public override void LoadContent()
		{
			graphics = (StaticGraphicsObject)ContentPackageManager.GetGraphicsResource("smw_concrete_block");
			graphics.LoadContent();
		}

		public override void Update()
		{
		}

		public override object GetCustomSerializableObjects() { return null; }
	}
}
