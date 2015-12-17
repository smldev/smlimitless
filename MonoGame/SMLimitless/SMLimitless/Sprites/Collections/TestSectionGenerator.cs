using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless.Sprites.Assemblies;

namespace SMLimitless.Sprites.Collections
{
	public static class TestSectionGenerator
	{
		public static Section GenerateSection(Level owner)
		{
			// Create a section of randomly placed stretches of stone tiles.

			Section result = new Section(owner);

			result.Bounds = new Physics.BoundingRectangle(0, 0, 3200, 1920);
			result.AutoscrollSettings = new Physics.SectionAutoscrollSettings(CameraScrollType.FreelyMoving, Vector2.Zero, null);
			result.Background = GenerateBackground(result);

			Layer mainLayer = new Layer(result, false);
			result.Layers.Add(mainLayer);
			mainLayer.SetMainLayer();

			Random random = new Random();
			float tilePlacerX = 0f;
			float tilePlacerY = 200f;

			// really temporary
			AssemblyManager.LoadAssembly(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "TestPackage", "SmlSample.dll"));

			Tile testTile3 = AssemblyManager.GetTileByFullName("SmlSample.TestTile3");
			testTile3.GraphicsResourceName = "smw_concrete_block";

			while (tilePlacerX < 1008f)
            {
				int runDistance = random.Next(3, 8);
				tilePlacerY += (random.Next(1, 3) * 16f) * ((random.Next(0, 2) != 0) ? 1 : -1);
				tilePlacerY = MathHelper.Clamp(tilePlacerY, 0, 1920);

				for (int i = 0; i < runDistance; i++)
				{
					Tile newTile = testTile3.Clone();
					newTile.Position = new Vector2(tilePlacerX, tilePlacerY);
					tilePlacerX += 16f;
				}
			}

			Sprite player = AssemblyManager.GetSpriteByFullName("SmlSample.SimplePlayer");
			player.Position = Vector2.Zero;
			player.IsActive = true;
			player.State = (SpriteState)0;
			player.CollisionMode = SpriteCollisionMode.OffsetNotify;
			result.AddSprite(player);

			return result;
		}

		private static Background GenerateBackground(Section owner)
		{
			Background result = new Background(owner);
			result.TopColor = result.BottomColor = new Color(0, 0, 0, 0);

			BackgroundLayer layer = new BackgroundLayer(GameServices.Camera, owner.Bounds);
			layer.BackgroundTextureResourceName = "Background";
			layer.ScrollDirection = (BackgroundScrollDirection)1;
			layer.ScrollRate = 0.5f;

			result.AddLayerToFront(layer);
			return result;
		}
	}
}
