using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SMLimitless.Collections;
using SMLimitless.Extensions;
using SMLimitless.Input;
using SMLimitless.Physics;

namespace SMLimitless.Sprites.Collections
{
	public sealed class Section
	{
		private Debug.DebugForm form = new Debug.DebugForm();
		private string debugText = "";

		private Vector2 autoscrollSpeed;
		private string autoscrollPathName;
		private bool isDeserialized;
		private bool isInitialized;
		private bool isContentLoaded;

		public SectionAutoscrollSettings AutoscrollSettings { get; internal set; }
		public Background Background { get; internal set; }
		public BoundingRectangle Bounds { get; internal set; }
		public Camera2D Camera { get; private set; }
		public int Index { get; set; }
		public Level Owner { get; private set; }
		public string Name { get; set; }

		internal List<Tile> Tiles { get; private set; }
		internal SparseCellGrid<Sprite> Sprites { get; private set; }

		internal List<Layer> Layers { get; private set; }
		internal Layer MainLayer { get; set; }
		internal List<Path> Paths { get; private set; }

		public Section(Level owner)
		{
			Camera = new Camera2D();
			Owner = owner;
			Sprites = new SparseCellGrid<Sprite>(GameServices.GameObjectSize * new Vector2(4f));
			Layers = new List<Layer>();
			Tiles = new List<Tile>();
			Paths = new List<Path>();
			Background = new Background(this);

			// temporary
			GameServices.Camera = Camera;
		}

		public void Initialize()
		{
			if (!isInitialized)
			{
				Background.Initialize();
				Layers.ForEach(l => l.Initialize());
				Sprites.ForEach(s => s.Initialize(this));
				isInitialized = true;
			}
		}

		public void LoadContent()
		{
			if (!isContentLoaded)
			{
				Background.LoadContent();
				Layers.ForEach(l => l.LoadContent());

				isContentLoaded = true;
			}
		}

		public void Update()
		{
			System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

			UpdatePhysics();

			Background.Update();
			Tiles.ForEach(t => t.Update());
			Sprites.ForEach(s => s.Update());
			TempUpdate();

			stopwatch.Stop();
			Sprite player = Sprites.First(s => s.GetType().FullName.Contains("SimplePlayer"));
			debugText = $"Player cell: {MainLayer.GetCellNumberAtPosition(player.Position)}";
		}

		private void UpdatePhysics()
		{
			foreach (Sprite sprite in Sprites)
			{
				// -1: up/left, 0: none, 1: down/right
				int resolutionDirection = 0;
				int slopeResolutionDirection = 0;
				Vector2 adjacentCell = new Vector2(float.NaN);
				float delta = GameServices.GameTime.GetElapsedSeconds();

				// Move the sprite by its Y velocity (upwards or downwards).
				sprite.Position = new Vector2(sprite.Position.X, (sprite.Position.Y + sprite.Velocity.Y * delta));

				foreach (Layer layer in GetLayersIntersectingRectangle(sprite.Hitbox))
				{
					Vector2 cellRangeTopLeft = layer.GetClampedCellNumberAtPosition(sprite.Position);
					Vector2 cellRangeBottomRight = layer.GetClampedCellNumberAtPosition(sprite.Hitbox.BottomRight);

					for (int y = (int)cellRangeTopLeft.Y; y <= (int)cellRangeBottomRight.Y; y++)
					{
						for (int x = (int)cellRangeTopLeft.X; x <= (int)cellRangeBottomRight.X; x++)
						{
							Tile tile = layer.GetTile(x, y);
							if (tile != null)
							{
								Vector2 resolutionDistance = tile.GetCollisionResolution(sprite);
								if (resolutionDistance.Y == 0f || !tile.CollisionOnSolidSide(resolutionDistance)) continue;

								if (resolutionDirection == 0 || Math.Sign(resolutionDirection) == Math.Sign(resolutionDistance.Y))
								{
									if (tile.TileShape == CollidableShape.RightTriangle && (tile.SlopedSides == RtSlopedSides.TopLeft || tile.SlopedSides == RtSlopedSides.TopRight))
									{
										slopeResolutionDirection = (tile.SlopedSides == RtSlopedSides.TopRight) ? -1 : 1;
										adjacentCell = new Vector2(x + slopeResolutionDirection, y);
									}

									resolutionDirection = Math.Sign(resolutionDistance.Y);
									sprite.Position = new Vector2(sprite.Position.X, sprite.Position.Y + resolutionDistance.Y);
									sprite.Velocity = new Vector2(sprite.Velocity.X, 0f);
								}
								else
								{
									sprite.IsEmbedded = true;
									goto nextSprite;
								}
							}
						}
					}
				}

				resolutionDirection = 0;
				sprite.Position = new Vector2((sprite.Position.X + sprite.Velocity.X * delta), sprite.Position.Y);
				foreach (Layer layer in GetLayersIntersectingRectangle(sprite.Hitbox))
				{
					Vector2 cellRangeTopLeft = layer.GetClampedCellNumberAtPosition(sprite.Position);
					Vector2 cellRangeBottomRight = layer.GetClampedCellNumberAtPosition(sprite.Hitbox.BottomRight);

					for (int y = (int)cellRangeTopLeft.Y; y <= (int)cellRangeBottomRight.Y; y++)
					{
						for (int x = (int)cellRangeTopLeft.X; x <= (int)cellRangeBottomRight.X; x++)
						{
							Tile tile = layer.GetTile(x, y);
							if (tile != null)
							{
								Vector2 resolutionDistance = tile.GetCollisionResolution(sprite);
								if (resolutionDistance.X == 0f || !tile.CollisionOnSolidSide(resolutionDistance)) { continue; }
								HorizontalDirection horizontalResolutionDirection = (HorizontalDirection)Math.Sign(resolutionDistance.X);
								if (slopeResolutionDirection != 0 && adjacentCell.X == x && (int)horizontalResolutionDirection == -slopeResolutionDirection) { continue; }

								if (resolutionDirection == 0 || Math.Sign(resolutionDistance.X) == Math.Sign(resolutionDirection))
								{
									resolutionDirection = Math.Sign(resolutionDistance.X);
									sprite.Position = new Vector2((sprite.Position.X + resolutionDistance.X), sprite.Position.Y);
								}
								else
								{
									sprite.IsEmbedded = true;
									goto nextSprite;
								}
							}
						}
					}
				}

				nextSprite:
				{ /* empty statement for target of label */ }
			}
		}

		public void Draw()
		{
			Background.Draw();
			Tiles.ForEach(t => t.Draw());
			Sprites.ForEach(s => s.Draw());

			GameServices.DrawStringDefault(string.Join(" ", debugText, ""));
		}

		private void TempUpdate() 
		{ 
			if (InputManager.IsNewKeyPress(Keys.OemTilde))
			{
				if (!GameServices.DebugForm.Visible) { GameServices.DebugForm.Show(); }
				else { GameServices.DebugForm.Hide(); }
			}
		}

		public Tile GetTileAtPosition(Vector2 position)
		{
			// Gets a tile from the topmost layer at the given position.
			// One layer is above another if its index within the Layers list is higher.
			// The MainLayer is always the lowest layer.

			int highestIndex = Layers.Count - 1;
			for (int i = highestIndex; i >= 0; i--)
			{
				Layer layer = Layers[i];
				Tile tile = layer.GetTile(layer.GetCellNumberAtPosition(position));
				if (tile != null) { return tile; }
			}

			return null;
		}

		private IEnumerable<Layer> GetLayersIntersectingRectangle(BoundingRectangle rectangle)
		{
			foreach (Layer layer in Layers)
			{
				if (rectangle.IntersectsIncludingEdges(layer.Bounds))
				{
					yield return layer;
				}
			}
		}

		public void AddTile(Tile tile)
		{
			if (tile == null)
			{
				throw new ArgumentNullException(nameof(tile), "The tile to add to the section was null.");
			}

			Tiles.Add(tile);
			MainLayer.AddTile(tile);
		}

		public void AddSprite(Sprite sprite)
		{
			if (sprite == null)
			{
				throw new ArgumentNullException(nameof(sprite), "The sprite to add to the section was null.");
			}

			Sprites.Add(sprite);
			MainLayer.AddSprite(sprite);
		}

		public void RemoveTile(Tile tile)
		{
			if (tile == null) { throw new ArgumentNullException(nameof(tile), "The tile to remove from the section was not null."); }

			Tiles.Remove(tile);
			Layers.ForEach(l => l.RemoveTile(tile));
		}

		public void RemoveSprite(Sprite sprite)
		{
			if (sprite == null) { throw new ArgumentNullException(nameof(sprite), "The sprite to remove from the section was not null."); }

			Sprites.Remove(sprite);
		}
	}
}