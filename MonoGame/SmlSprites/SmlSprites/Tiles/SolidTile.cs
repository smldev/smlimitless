﻿using System;
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
		private bool graphicsLoaded;
		private IGraphicsObject _graphics;
		private IGraphicsObject graphics
		{
			get { return _graphics; }
			set { graphicsLoaded = true; _graphics = value; }
		}

		private int widthInGridCells;
		private int heightInGridCells;

		private static PhysicsSetting<float> SurfaceFrictionSetting = new PhysicsSetting<float>("Solid Tile: Surface Friction", 0f, 5000f, 4500f, PhysicsSettingType.FloatingPoint);

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
			//if (graphics == null) { return; }
			graphics.Draw(Position, Color.White);
		}

		public override void Update()
		{
		}

		public override object GetCustomSerializableObjects()
		{
			return new
			{
				widthInGridCells = (int)(this.Size.X / GameServices.GameObjectSize.X),
				heightInGridCells = (int)(this.Size.Y / GameServices.GameObjectSize.Y)
			};
		}

		public override void HandleCollision(Sprite sprite, Vector2 intersect)
		{
		}
	}
}
