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

namespace SmlSample
{
	public sealed class TestSlope2 : Tile
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

		public TestSlope2()
		{
			Size = new Vector2(16f);
			TileShape = CollidableShape.RightTriangle;
			TriSolidSides = TileTriSolidSides.Slope | TileTriSolidSides.HorizontalLeg | TileTriSolidSides.VerticalLeg;
			SlopedSides = RtSlopedSides.TopRight;
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

		public override void LoadContent()
		{
			graphics = (StaticGraphicsObject)ContentPackageManager.GetGraphicsResource("smw_grass_slope2");
			graphics.LoadContent();
		}

		public override void Update()
		{
		}

		public override object GetCustomSerializableObjects() { return null; }
	}
}
