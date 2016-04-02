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

namespace SmlSprites.Tiles
{
	public sealed class SolidTile : Tile
	{
		private IGraphicsObject graphics;

		private int widthInGridCells;
		private int heightInGridCells;

		private static PhysicsSetting<float> SurfaceFrictionSetting = new PhysicsSetting<float>("Solid Tile: Surface Friction", 0f, 5000f, 12f, PhysicsSettingType.FloatingPoint);

		public override string EditorCategory
		{
			get
			{
				return "Standard Tiles";
			}
		}

		public override float SurfaceFriction
		{
			get
			{
				return SurfaceFrictionSetting.Value;
			}
		}

		public SolidTile()
		{
			TileShape = CollidableShape.Rectangle;
			RectSolidSides = TileRectSolidSides.Top | TileRectSolidSides.Left | TileRectSolidSides.Right | TileRectSolidSides.Bottom;
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
			widthInGridCells = customObjects.GetInt("widthInGridCells");
			heightInGridCells = customObjects.GetInt("heightInGridCells");

			Size = new Vector2(widthInGridCells * GameServices.GameObjectSize.X, GameServices.GameObjectSize.Y);
		}

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
			return new
			{
				heightInGridCells = this.heightInGridCells,
				widthInGridCells = this.widthInGridCells
			};
		}

		public override void HandleCollision(Sprite sprite, Vector2 intersect)
		{
		}
	}
}
