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
	public sealed class SlopedTile : Tile
	{
		private IGraphicsObject graphics;

		private int widthInGridCells;
		private int heightInGridCells;

		public override string EditorCategory
		{
			get
			{
				return "Standard Tiles";
			}
		}

		public SlopedTile()
		{
			TileShape = CollidableShape.RightTriangle;
			TriSolidSides = TileTriSolidSides.Slope | TileTriSolidSides.HorizontalLeg | TileTriSolidSides.VerticalLeg;
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
			widthInGridCells = customObjects.GetInt("widthInGridCells");
			heightInGridCells = customObjects.GetInt("heightInGridCells");
			SlopedSides = (RtSlopedSides)customObjects.GetInt("slopedSides");

			Size = new Vector2(widthInGridCells * GameServices.GameObjectSize.X, heightInGridCells * GameServices.GameObjectSize.Y);
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
				widthInGridCells = this.widthInGridCells,
				heightInGridCells = this.heightInGridCells,
				slopedSides = this.SlopedSides
			};
		}

		public override void HandleCollision(Sprite sprite, Vector2 intersect)
		{
		}
	}
}