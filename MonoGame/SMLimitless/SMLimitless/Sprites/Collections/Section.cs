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
		private RtSlopedSides drawSlopeHitboxSides = RtSlopedSides.TopLeft;

		private Vector2 autoscrollSpeed;
		private string autoscrollPathName;
		private bool isDeserialized;
		private bool isInitialized;
		private bool isContentLoaded;

		public SectionAutoscrollSettings AutoscrollSettings { get; internal set; }
		public Background Background { get; internal set; }
		public BoundingRectangle Bounds { get; internal set; }
		public Camera2D Camera { get; private set; }
		public CameraSystem CameraSystem { get; private set; }
		public int Index { get; set; }
		public Level Owner { get; private set; }
		public string Name { get; set; }

		internal List<Tile> Tiles { get; private set; }
		internal SparseCellGrid<Sprite> Sprites { get; set; }
		internal List<Sprite> SpritesToAddOnNextFrame { get; } = new List<Sprite>();

		internal List<Layer> Layers { get; set; }
		internal Layer MainLayer { get; set; }
		internal List<Path> Paths { get; set; }
		public bool IsLoaded { get; internal set; }

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

				CameraSystem = new CameraSystem(Camera, Bounds, Sprites.First(s => s.GetType().FullName.Contains("SimplePlayer")));

				isInitialized = true;
			}
		}

		public void LoadContent()
		{
			if (!isContentLoaded)
			{
				Background.LoadContent();
				Layers.ForEach(l => l.LoadContent());
				Sprites.ForEach(s => s.LoadContent());
				isContentLoaded = true;
			}
		}

		public void Update()
		{
			System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

			AddSpritesToAddOnNextFrame();
			Tiles.ForEach(t => t.Update());
			Sprites.ForEach(s => s.Update());
			Sprites.Update();
			UpdatePhysics();
			Sprites.Update();
			Sprites.RemoveAll(s => s.RemoveOnNextFrame);
			CameraSystem.Update();
			Background.Update();
			TempUpdate();

			stopwatch.Stop();
			debugText = $"{stopwatch.ElapsedMilliseconds}";
		}

		private void UpdatePhysics()
		{
			foreach (Sprite sprite in Sprites)
			{
				// -1: up/left, 0: none, 1: down/right
				int resolutionDirection = 0;								// Stores the direction of the last resolution.
				int slopeResolutionDirection = 0;							// Stores the direction of the last slope resolution, if there was one.
				Vector2 adjacentCell = new Vector2(float.NaN);				// Stores the grid cell number of the cell adjacent to the current cell.
				float delta = GameServices.GameTime.GetElapsedSeconds();    // The number of seconds that have elapsed since the last Update call.
				Point intSpritePosition = Point.Zero;

				sprite.Position = new Vector2((sprite.Position.X + sprite.Velocity.X * delta), sprite.Position.Y);  // Move the sprite horizontally by its horizontal velocity.
				intSpritePosition = sprite.Position.ToPoint();
				foreach (Layer layer in GetLayersIntersectingRectangle(sprite.Hitbox))								// For every layer this sprite intersects...
				{
					Vector2 cellRangeTopLeft = layer.GetClampedCellNumberAtPosition(sprite.Position);                   // The leftmost and topmost grid cell the sprite's in, or {0, 0} if the sprite's top-left edge is outside the grid.
					Vector2 cellRangeBottomRight = layer.GetClampedCellNumberAtPosition(sprite.Hitbox.BottomRight);     // The rightmost and bottommost grid cell the sprite's in, or the rightmost and bottommost grid cell of the grid if the sprite's bottom-right edge is outside the grid.

					for (int y = (int)cellRangeTopLeft.Y; y <= (int)cellRangeBottomRight.Y; y++)
					{
						for (int x = (int)cellRangeTopLeft.X; x <= (int)cellRangeBottomRight.X; x++)
						{
							Tile tile = layer.GetTile(x, y);
							if (tile != null && tile.TileShape == CollidableShape.RightTriangle)
							{
								HorizontalDirection adjacentDirection = (tile.SlopedSides == RtSlopedSides.TopLeft || tile.SlopedSides == RtSlopedSides.BottomLeft) ? HorizontalDirection.Right : HorizontalDirection.Left;
								adjacentCell = new Vector2(x + (int)adjacentDirection, y);

								Vector2 resolutionDistance = tile.GetCollisionResolution(sprite);
								if (resolutionDistance.Y == 0f || resolutionDistance.IsNaN())
								{
									// Skip to the next cell. If this is a collision with the normal sides of a sloped tile, we'll get it in the horizontal/vertical collision handlers below.
									continue;
								}

								if (resolutionDirection == 0 || Math.Sign(resolutionDirection) == Math.Sign(resolutionDistance.Y))
								{
									resolutionDirection = Math.Sign(resolutionDistance.Y);
									slopeResolutionDirection = (tile.SlopedSides == RtSlopedSides.TopRight) ? -1 : 1;
									sprite.Position = new Vector2(sprite.Position.X, (sprite.Position.Y + resolutionDistance.Y));
									sprite.HandleTileCollision(tile, resolutionDistance);
								}
								else
								{
									sprite.IsEmbedded = true;
									continue;
								}
							}
						}
					}

					cellRangeTopLeft = layer.GetClampedCellNumberAtPosition(sprite.Position);
					cellRangeBottomRight = layer.GetClampedCellNumberAtPosition(sprite.Hitbox.BottomRight);

					for (int y = (int)cellRangeTopLeft.Y; y <= (int)cellRangeBottomRight.Y; y++)	// For each row of tiles in the cells intersected by the sprite...
					{
						for (int x = (int)cellRangeTopLeft.X; x <= (int)cellRangeBottomRight.X; x++)	// For each cell intersected by the sprite...
						{
							Tile tile = layer.GetTile(x, y);	// Get the tile in the cell.
							if (tile != null) // && tile.TileShape == CollidableShape.Rectangle)					// If there's a rectangular tile here...
							{
								Vector2 resolutionDistance = tile.GetCollisionResolution(sprite);																			// Get the resolution distance between this tile and the sprite.

								if (resolutionDistance.X == 0f || resolutionDistance.IsNaN() || !tile.CollisionOnSolidSide(resolutionDistance)) { continue; }												// If there is no horizontal collision, or if this side of the tile is not solid, continue to the next cell.
								HorizontalDirection horizontalResolutionDirection = (HorizontalDirection)Math.Sign(resolutionDistance.X);
								                                   // The horizontal resolution direction is equal to the sign of the resolution distance.
								
								if (resolutionDistance.X < 0f && (tile.AdjacencyFlags & TileAdjacencyFlags.SlopeOnLeft) == TileAdjacencyFlags.SlopeOnLeft) { continue; }
								else if (resolutionDistance.X > 0f && (tile.AdjacencyFlags & TileAdjacencyFlags.SlopeOnRight) == TileAdjacencyFlags.SlopeOnRight) { continue; }

								if (resolutionDirection == 0 || Math.Sign(resolutionDistance.X) == Math.Sign(resolutionDirection))	// If there has been no other horizontal resolution this frame, or if the last horizontal resolution was in the same direction as this one...
								{
									resolutionDirection = Math.Sign(resolutionDistance.X);											// The resolution direction is equal to the sign of the resolution distance.
									sprite.Position = new Vector2((sprite.Position.X + resolutionDistance.X), sprite.Position.Y);   // Move the sprite to resolve the collision.
									sprite.HandleTileCollision(tile, resolutionDistance);
								}
								else
								{
									sprite.IsEmbedded = true;	// The sprite is embedded.
									goto nextSprite;			// Continue to the next sprite.
								}
							}
						}
					}
				}

				resolutionDirection = 0;
				// Move the sprite by its Y velocity (upwards or downwards).
				sprite.Position = new Vector2(sprite.Position.X, (sprite.Position.Y + sprite.Velocity.Y * delta));  // Move the sprite vertically by (Velocity.Y * delta).

				foreach (Layer layer in GetLayersIntersectingRectangle(sprite.Hitbox))  // For every layer the sprite intersects...
				{
					Vector2 cellRangeTopLeft = layer.GetClampedCellNumberAtPosition(sprite.Position);               // The leftmost and topmost grid cell the sprite's in, or {0, 0} if the sprite's top-left edge is outside the grid.
					Vector2 cellRangeBottomRight = layer.GetClampedCellNumberAtPosition(sprite.Hitbox.BottomRight); // The rightmost and bottommost grid cell the sprite's in, or the rightmost and bottommost grid cell of the grid if the sprite's bottom-right edge is outside the grid.

					for (int y = (int)cellRangeTopLeft.Y; y <= (int)cellRangeBottomRight.Y; y++)    // For every row of cells the sprite intersects...
					{
						for (int x = (int)cellRangeTopLeft.X; x <= (int)cellRangeBottomRight.X; x++)    // For every cell the sprite intersects...
						{
							Tile tile = layer.GetTile(x, y);    // Get the tile inside the cell.
							if (tile != null)                   // If there's a tile here...
							{
								Vector2 resolutionDistance = tile.GetCollisionResolution(sprite);                           // Determine the resolution distance between this tile and the sprite.
								if (resolutionDistance.Y == 0f || resolutionDistance.IsNaN() || !tile.CollisionOnSolidSide(resolutionDistance)) continue; // If there's no vertical collision here, or if we collided on a non-solid edge, continue to the next cell.
								if ((tile.AdjacencyFlags & TileAdjacencyFlags.SlopeOnLeft) == TileAdjacencyFlags.SlopeOnLeft && sprite.Hitbox.Center.X < tile.Hitbox.Bounds.Left) continue;
								else if ((tile.AdjacencyFlags & TileAdjacencyFlags.SlopeOnRight) == TileAdjacencyFlags.SlopeOnRight && sprite.Hitbox.Center.X > tile.Hitbox.Bounds.Right) continue;

								if (resolutionDirection == 0 || Math.Sign(resolutionDirection) == Math.Sign(resolutionDistance.Y))  // If there has been no other vertical collision, or the last vertical resolution was in the same direction as this one...
								{
									resolutionDirection = Math.Sign(resolutionDistance.Y);                                      // The resolution direction is equal to the sign of the resolution distance (up = negative, down = positive).
									sprite.Position = new Vector2(sprite.Position.X, sprite.Position.Y + resolutionDistance.Y); // Move the sprite vertically by the resolution distance.
									sprite.Velocity = new Vector2(sprite.Velocity.X, 0f);                                       // Stop the sprite's vertical movement.
									sprite.HandleTileCollision(tile, resolutionDistance);
								}
								else  // ...but if the last vertical resolution was in the opposite direction...
								{
									sprite.IsEmbedded = true;   // ...the sprite is embedded (up and down resolutions in the same frame).
									goto nextSprite;            // Continue to the next sprite.
								}
							}
						}
					}
				}

				nextSprite:
				{ /* empty statement for target of label */ }

				foreach (var collidableSprite in Sprites.GetItemsNearItem(sprite))
				{
					form.AddToLogText($"Processed {collidableSprite.GetType().FullName}");
					
					Vector2 intersectA = sprite.Hitbox.GetIntersectionDepth(collidableSprite.Hitbox);
					Vector2 intersectB = collidableSprite.Hitbox.GetIntersectionDepth(sprite.Hitbox);
					
					if (sprite.Hitbox.Intersects(collidableSprite.Hitbox))
					{
						sprite.HandleSpriteCollision(collidableSprite, intersectA);
						collidableSprite.HandleSpriteCollision(sprite, intersectB);
					}
				}
			}
		}

		public void Draw()
		{
			Background.Draw();
			Tiles.ForEach(t => t.Draw());
			Sprites.ForEach(s => s.Draw());
			GameServices.DrawStringDefault(debugText);
		}

		private void TempUpdate() 
		{ 
			if (InputManager.IsNewKeyPress(Keys.OemTilde))
			{
				if (!GameServices.DebugForm.Visible) { GameServices.DebugForm.Show(); }
				else { GameServices.DebugForm.Hide(); }
			}
			else if (InputManager.IsNewKeyPress(Keys.E))
			{
				GameServices.Camera.Zoom *= 1.05f;
			}
			else if (InputManager.IsNewKeyPress(Keys.D))
			{
				GameServices.Camera.Zoom *= (1f / 1.05f);
			}
			else if (InputManager.IsNewKeyPress(Keys.F))
			{
				drawSlopeHitboxSides = (drawSlopeHitboxSides == RtSlopedSides.TopLeft) ? RtSlopedSides.TopRight : RtSlopedSides.TopLeft;
			}
			else if (InputManager.IsNewKeyPress(Keys.G))
			{
				string json = new IO.LevelSerializers.Serializer003().Serialize(Owner);
				System.IO.File.WriteAllText(@"test_003.lvl", json);
			}

			// if (Sprites.Cells[Sprites.GetCellNumberAtPosition(Sprites.First(s => s.GetType().FullName.Contains("Player")).Position)].Items.Count == 2) System.Diagnostics.Debugger.Break();
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
				if (!layer.Bounds.Intersects(position)) { continue; }
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

		public void AddSpriteOnNextFrame(Sprite sprite)
		{
			if (sprite == null)
			{
				throw new ArgumentNullException(nameof(sprite), "The sprite to add to the section was null.");
			}

			SpritesToAddOnNextFrame.Add(sprite);
		}

		private void AddSpritesToAddOnNextFrame()
		{
			if (SpritesToAddOnNextFrame.Any())
			{
				SpritesToAddOnNextFrame.ForEach(s => AddSprite(s));
				SpritesToAddOnNextFrame.Clear();
			}
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