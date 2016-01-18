using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless.Content;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Input;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;

namespace SmlSample
{
	public sealed class PainterSprite : Sprite
	{
		private List<StaticGraphicsObject> tileGraphics;
		private string[] tileTypeNames = new string[] { "SmlSample.TestTile3", "SmlSample.TestSlope1", "SmlSample.TestSlope2" };
		private string[] graphicsObjectNames = new string[] { "smw_concrete_block", "smw_grass_slope1", "smw_grass_slope2" };
		private int currentGraphicIndex;
		private Assembly currentAssembly; // laziness carries over, apparently

		public override string EditorCategory
		{
			get
			{
				return "Test Item";
			}
		}

		public PainterSprite()
		{
			tileGraphics = new List<StaticGraphicsObject>();
			Size = new Vector2(16f);
			currentAssembly = Assembly.GetExecutingAssembly();
		}

		public override void Initialize(Section owner)
		{
			for (int i = 0; i < 3; i++)
			{
				tileGraphics.Add((StaticGraphicsObject)ContentPackageManager.GetGraphicsResource(this.graphicsObjectNames[i]));
			}

			base.Initialize(owner);
		}

		public override void LoadContent()
		{
			tileGraphics.ForEach(g => g.LoadContent());
		}

		public override void Draw()
		{
			tileGraphics[currentGraphicIndex].Draw(Position, Color.White);
		}

		public override void Update()
		{
			Position = (InputManager.MousePosition + Owner.Camera.Position).FloorDivide(16f) * 16f;
			Vector2 checkPosition = Position + (Size / 2f);

			if (InputManager.IsCurrentMousePress(MouseButtons.LeftButton) && Position.GreaterThanOrEqualTo(Vector2.Zero))
			{
				if (Owner.GetTileAtPosition(checkPosition) == null)
				{
					Tile tile = (Tile)Activator.CreateInstance(currentAssembly.GetType(tileTypeNames[currentGraphicIndex]));
					tile.Position = Position;
					tile.Initialize(Owner);
					tile.LoadContent();
					Owner.AddTile(tile);
				}
			}
			else if (InputManager.IsCurrentMousePress(MouseButtons.RightButton))
			{
				Tile tile = Owner.GetTileAtPosition(checkPosition);

				if (tile != null)
				{
					Owner.RemoveTile(tile);
				}
			}
			else if (InputManager.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.C))
			{
				if (currentGraphicIndex == 2)
				{
					currentGraphicIndex = 0;
				}
				else
				{
					currentGraphicIndex++;
				}
			}
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
		}

		public override void HandleSpriteCollision(Sprite sprite, Vector2 intersect)
		{
		}

		public override void HandleTileCollision(Tile tile, Vector2 intersect)
		{
		}
	}
}
