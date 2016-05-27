﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SMLimitless.Collections;
using SMLimitless.Editor;
using SMLimitless.Extensions;
using SMLimitless.Input;
using SMLimitless.Interfaces;
using SMLimitless.Physics;
using SMLimitless.Screens.Effects;
using SMLimitless.Sprites.InternalSprites;

namespace SMLimitless.Sprites.Collections
{
	/// <summary>
	/// The main area of gameplay.
	/// </summary>
	public sealed class Section
	{
		private int intersectionDepthCalls = 0;
		private int spriteCollisionCallsMade = 0;
		private int spritesNearSpriteCount = 0;

		private bool isContentLoaded;
		private bool isDeserialized;
		private bool isInitialized;

		/// <summary>
		/// Gets a value indicating whether first-stage loading (deserialization and game object initialization) has completed.
		/// </summary>
		public bool IsLoaded { get; internal set; }

		private string debugText = "";
		private Debug.DebugForm form = new Debug.DebugForm();

		/// <summary>
		/// Gets the <see cref="Sprites.Collections.Background"/> for this section.
		/// </summary>
		public Background Background { get; internal set; }

		/// <summary>
		/// Gets the bounds of this section, the rectangular area to which the camera is restricted.
		/// </summary>
		public BoundingRectangle Bounds { get; internal set; }

		/// <summary>
		/// Gets the camera viewing this section.
		/// </summary>
		public Camera2D Camera { get; private set; }

		/// <summary>
		/// Gets the <see cref="Physics.CameraSystem"/> instance tracking objects in this section.
		/// </summary>
		public CameraSystem CameraSystem { get; private set; }

		/// <summary>
		/// Gets or sets the numeric index of this section within its level.
		/// </summary>
		public int Index { get; set; }
		private IrisEffect irisEffect;

		/// <summary>
		/// Gets the <see cref="Level"/> that owns this section.
		/// </summary>
		public Level Owner { get; private set; }

		/// <summary>
		/// Gets the settings used for automatic camera scrolling for this section.
		/// </summary>
		public SectionAutoscrollSettings AutoscrollSettings { get; internal set; }

		/// <summary>
		/// Gets or sets the name of this section.
		/// </summary>
		public string Name { get; set; }

		internal EditorSelectedObject editorSelectedObject = new EditorSelectedObject();
		private EditorCameraTrackingObject editorTrackingObject = null;
		private EditorForm editorForm = null;

		/// <summary>
		/// Gets a value indicating whether the level editor is currently enabled for this section.
		/// </summary>
		public bool EditorActive { get; internal set; }

		internal Layer MainLayer { get; set; }
		internal List<Layer> Layers { get; set; }
		internal List<Path> Paths { get; set; }
		internal List<Sprite> Players { get; set; } = new List<Sprite>();
		internal List<Sprite> SpritesToAddOnNextFrame { get; } = new List<Sprite>();
		internal List<Tile> Tiles { get; private set; }
		internal SparseCellGrid<Sprite> Sprites { get; set; }

		/// <summary>
		/// Gets the current mouse position adjusted for the camera's position and zoom.
		/// </summary>
		public Vector2 MousePosition
		{
			get
			{
				Vector2 actualMousePosition = InputManager.MousePosition;
				Vector2 windowSize = GameServices.ScreenSize;
				if (actualMousePosition.X < 0f || actualMousePosition.X > windowSize.X ||
					actualMousePosition.Y < 0f || actualMousePosition.Y > windowSize.Y)
				{
					return new Vector2(float.NaN);
				}

				Vector2 mousePositionAdjustedForZoom = actualMousePosition * (1f / Camera.Zoom);
				return Camera.Position + mousePositionAdjustedForZoom;
			}
		}

		internal Sprite CollisionDebugSelectedSprite { get; set; }
		private bool isCollisionDebuggingInitialized = false;
		private CollisionDebugSelectSprite selectorSprite = new CollisionDebugSelectSprite();
		private List<Tile> collisionDebugCollidedTiles = new List<Tile>();

		/// <summary>
		/// Initializes a new instance of the <see cref="Section"/> class.
		/// </summary>
		/// <param name="owner">The <see cref="Level"/> that owns this section.</param>
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

		/// <summary>
		/// Initializes the game objects for this section.
		/// </summary>
		public void Initialize()
		{
			if (!isInitialized)
			{
				Background.Initialize();
				Layers.ForEach(l => l.Initialize());
				Sprites.ForEach(s => s.Initialize(this));
				editorSelectedObject.Initialize(this);
				selectorSprite.Initialize(this);

				CameraSystem = new CameraSystem(Camera, Bounds);
				irisEffect = new IrisEffect(Camera.Viewport.Center);

				isInitialized = true;
			}
		}

		/// <summary>
		/// Loads the content for the game objects in this section.
		/// </summary>
		public void LoadContent()
		{
			if (!isContentLoaded)
			{
				Background.LoadContent();
				Layers.ForEach(l => l.LoadContent());
				Sprites.ForEach(s => s.LoadContent());
				editorSelectedObject.LoadContent();

				irisEffect.LoadContent();
				irisEffect.Start(90, EffectDirection.Forward, Vector2.Zero, Color.Black);

				// TEMPORARY: Code to add a test player; remove when player support is a bit better
				//Vector2 createPlayerAt = Tiles.First(t => GetTileAtPosition(new Vector2(t.Position.X, t.Position.Y - 8f)) == null).Position;
				//createPlayerAt.Y -= 16f;
				//Sprite playerSprite = Assemblies.AssemblyManager.GetSpriteByFullName("SmlSprites.Players.PlayerMario");
				//playerSprite.Initialize(this);
				//playerSprite.LoadContent();
				//playerSprite.Position = createPlayerAt;
				//Sprites.Add(playerSprite);
				//CameraSystem.TrackingObjects.Add(playerSprite);

				isContentLoaded = true;
			}
		}

		/// <summary>
		/// Updates this section.
		/// </summary>
		public void Update()
		{
			System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

			irisEffect.Update();
			AddSpritesToAddOnNextFrame();

			if (!EditorActive)
			{
				if (GameServices.CollisionDebuggerActive && !isCollisionDebuggingInitialized) { InitializeCollisionDebugging(); }
				else if (!GameServices.CollisionDebuggerActive && isCollisionDebuggingInitialized) { UninitializeCollisionDebugging(); }

				Tiles.ForEach(t => t.Update());
				Sprites.ForEach(s => s.Update());
				Sprites.Update();
				UpdatePhysics();
				Sprites.ForEach(s => s.SpritesCollidedWithThisFrame.Clear());
				Sprites.Update();
			}
			else
			{
				editorTrackingObject.Update();
				editorSelectedObject.Update();
			}

			Sprites.RemoveAllWhere(s => s.RemoveOnNextFrame);
			CameraSystem.Update();
			Background.Update();
			TempUpdate();

			stopwatch.Stop();
		}

		/// <summary>
		/// Moves sprites according to their velocity and performs collision detection and resolution.
		/// </summary>
		private void UpdatePhysics()
		{
			collisionDebugCollidedTiles.Clear();
			float delta = GameServices.GameTime.GetElapsedSeconds();    // The number of seconds that have elapsed since the last Update call.

			foreach (Sprite sprite in Sprites)
			{
				if (sprite.CollisionMode == SpriteCollisionMode.NoCollision) { continue; }

				// -1: up/left, 0: none, 1: down/right
				int resolutionDirection = 0;                                // Stores the direction of the last resolution.
				int slopeResolutionDirection = 0;                           // Stores the direction of the last slope resolution, if there was one.
				Vector2 adjacentCell = new Vector2(float.NaN);              // Stores the grid cell number of the cell adjacent to the current cell.
				Vector2 spritePositionWithoutResolutions = sprite.Position;
				sprite.IsOnGround = false;

				int numberOfCollidingTiles = 0;
				bool slopeCollisionOccurred = false;

				// Resolution with the sloped side of a sloped tile
				sprite.Position = new Vector2((sprite.Position.X + sprite.Velocity.X * delta), sprite.Position.Y);  // Move the sprite horizontally by its horizontal velocity.
				spritePositionWithoutResolutions.X += (sprite.Velocity.X * delta);
				foreach (Layer layer in GetLayersIntersectingRectangle(sprite.Hitbox))                              // For every layer this sprite intersects...
				{
					Vector2 cellRangeTopLeft = layer.GetClampedCellNumberAtPosition(sprite.Position);                   // The leftmost and topmost grid cell the sprite's in, or {0, 0} if the sprite's top-left edge is outside the grid.
					Vector2 cellRangeBottomRight = layer.GetClampedCellNumberAtPosition(sprite.Hitbox.BottomRight);     // The rightmost and bottommost grid cell the sprite's in, or the rightmost and bottommost grid cell of the grid if the sprite's bottom-right edge is outside the grid.

					for (int y = (int)cellRangeTopLeft.Y; y <= (int)cellRangeBottomRight.Y; y++)
					{
						for (int x = (int)cellRangeTopLeft.X; x <= (int)cellRangeBottomRight.X; x++)
						{
							Tile tile = layer.SafeGetTile(new Vector2(x, y));
							if (tile != null && tile.TileShape == CollidableShape.RightTriangle)
							{
								HorizontalDirection tileSlopeDirection = tile.SlopedSides.GetHorizontalDirection();
								HorizontalDirection adjacentDirection = (tileSlopeDirection == HorizontalDirection.Left) ? HorizontalDirection.Right : HorizontalDirection.Left;
								adjacentCell = new Vector2(x + (int)adjacentDirection, y);
								var adjacentTile = layer.SafeGetTile(adjacentCell);

								if (adjacentTile != null)
								{
									if (adjacentTile.TileShape == CollidableShape.RightTriangle && adjacentTile.SlopedSides.GetHorizontalDirection().IsOppositeDirection(tileSlopeDirection))
									{
										continue;
									}
								}

								Vector2 resolutionDistance = tile.GetCollisionResolution(sprite);
								if (resolutionDistance.Y == 0f || resolutionDistance.IsNaN())
								{
									// Skip to the next cell. If this is a collision with the normal sides of a sloped tile, we'll get it in the horizontal/vertical collision handlers below.
									continue;
								}
								else
								{
									if (tile.BreakOnCollision && sprite.BreakOnCollision) { System.Diagnostics.Debugger.Break(); }
								}

								if (resolutionDirection == 0 || Math.Sign(resolutionDirection) == Math.Sign(resolutionDistance.Y))
								{
									resolutionDirection = Math.Sign(resolutionDistance.Y);
									slopeResolutionDirection = (tile.SlopedSides == RtSlopedSides.TopRight) ? -1 : 1;
									sprite.Position = new Vector2(sprite.Position.X, (sprite.Position.Y + resolutionDistance.Y));
									sprite.IsOnGround = (resolutionDistance.Y < 0f);
									sprite.HandleTileCollision(tile, resolutionDistance);

									numberOfCollidingTiles++;
									collisionDebugCollidedTiles.Add(tile);
									slopeCollisionOccurred = true;
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

					// Horizontal resolution
					for (int y = (int)cellRangeTopLeft.Y; y <= (int)cellRangeBottomRight.Y; y++)    // For each row of tiles in the cells intersected by the sprite...
					{
						for (int x = (int)cellRangeTopLeft.X; x <= (int)cellRangeBottomRight.X; x++)    // For each cell intersected by the sprite...
						{
							Tile tile = layer.SafeGetTile(new Vector2(x, y));   // Get the tile in the cell.
							if (tile != null) // && tile.TileShape == CollidableShape.Rectangle)					// If there's a rectangular tile here...
							{
								Vector2 resolutionDistance = tile.GetCollisionResolution(sprite);                                                                           // Get the resolution distance between this tile and the sprite.

								if (resolutionDistance.X == 0f || resolutionDistance.IsNaN() || !tile.CollisionOnSolidSide(resolutionDistance)) { continue; }                                              // If there is no horizontal collision, or if this side of the tile is not solid, continue to the next cell.
								else
								{
									if (tile.BreakOnCollision && sprite.BreakOnCollision) { System.Diagnostics.Debugger.Break(); }
								}
								HorizontalDirection horizontalResolutionDirection = (HorizontalDirection)Math.Sign(resolutionDistance.X);
								// The horizontal resolution direction is equal to the sign of the resolution distance.

								if (resolutionDistance.X < 0f && (tile.AdjacencyFlags & TileAdjacencyFlags.SlopeOnLeft) == TileAdjacencyFlags.SlopeOnLeft) { continue; }
								else if (resolutionDistance.X > 0f && (tile.AdjacencyFlags & TileAdjacencyFlags.SlopeOnRight) == TileAdjacencyFlags.SlopeOnRight) { continue; }

								if (resolutionDirection == 0 || Math.Sign(resolutionDistance.X) == Math.Sign(resolutionDirection))  // If there has been no other horizontal resolution this frame, or if the last horizontal resolution was in the same direction as this one...
								{
									resolutionDirection = Math.Sign(resolutionDistance.X);                                          // The resolution direction is equal to the sign of the resolution distance.
									sprite.Position = new Vector2((sprite.Position.X + resolutionDistance.X), sprite.Position.Y);   // Move the sprite to resolve the collision.
									sprite.Velocity = new Vector2(0f, sprite.Velocity.Y);
									sprite.Acceleration = new Vector2(0f, sprite.Acceleration.Y);
									sprite.HandleTileCollision(tile, resolutionDistance);

									numberOfCollidingTiles++;
									collisionDebugCollidedTiles.Add(tile);
								}
								else
								{
									sprite.IsEmbedded = true;   // The sprite is embedded.
									goto nextSprite;            // Continue to the next sprite.
								}
							}
						}
					}
				}

				// Vertical resolution
				resolutionDirection = 0;
				// Move the sprite by its Y velocity (upwards or downwards).
				sprite.Position = new Vector2(sprite.Position.X, (sprite.Position.Y + sprite.Velocity.Y * delta));  // Move the sprite vertically by (Velocity.Y * delta).
				spritePositionWithoutResolutions.Y += sprite.Velocity.Y * delta;
				foreach (Layer layer in GetLayersIntersectingRectangle(sprite.Hitbox))  // For every layer the sprite intersects...
				{
					Vector2 cellRangeTopLeft = layer.GetClampedCellNumberAtPosition(sprite.Position);               // The leftmost and topmost grid cell the sprite's in, or {0, 0} if the sprite's top-left edge is outside the grid.
					Vector2 cellRangeBottomRight = layer.GetClampedCellNumberAtPosition(sprite.Hitbox.BottomRight); // The rightmost and bottommost grid cell the sprite's in, or the rightmost and bottommost grid cell of the grid if the sprite's bottom-right edge is outside the grid.

					for (int y = (int)cellRangeTopLeft.Y; y <= (int)cellRangeBottomRight.Y; y++)    // For every row of cells the sprite intersects...
					{
						for (int x = (int)cellRangeTopLeft.X; x <= (int)cellRangeBottomRight.X; x++)    // For every cell the sprite intersects...
						{
							Tile tile = layer.SafeGetTile(new Vector2(x, y));    // Get the tile inside the cell.
							if (tile != null)                   // If there's a tile here...
							{
								Vector2 resolutionDistance = tile.GetCollisionResolution(sprite);                           // Determine the resolution distance between this tile and the sprite.
								if (resolutionDistance.Y == 0f || resolutionDistance.IsNaN() || !tile.CollisionOnSolidSide(resolutionDistance)) continue; // If there's no vertical collision here, or if we collided on a non-solid edge, continue to the next cell.
								else
								{
									if (tile.BreakOnCollision && sprite.BreakOnCollision) { System.Diagnostics.Debugger.Break(); }
								}
								if ((tile.AdjacencyFlags & TileAdjacencyFlags.SlopeOnLeft) == TileAdjacencyFlags.SlopeOnLeft && sprite.Hitbox.Center.X < tile.Hitbox.Bounds.Left) continue;
								else if ((tile.AdjacencyFlags & TileAdjacencyFlags.SlopeOnRight) == TileAdjacencyFlags.SlopeOnRight && sprite.Hitbox.Center.X > tile.Hitbox.Bounds.Right)
								{
									continue;
								}


								if (resolutionDirection == 0 || Math.Sign(resolutionDirection) == Math.Sign(resolutionDistance.Y))  // If there has been no other vertical collision, or the last vertical resolution was in the same direction as this one...
								{
									resolutionDirection = Math.Sign(resolutionDistance.Y);                                      // The resolution direction is equal to the sign of the resolution distance (up = negative, down = positive).
									sprite.Position = new Vector2(sprite.Position.X, sprite.Position.Y + resolutionDistance.Y); // Move the sprite vertically by the resolution distance.
									sprite.IsOnGround = (resolutionDistance.Y < 0f);
									sprite.Velocity = new Vector2(sprite.Velocity.X, 0f);                                       // Stop the sprite's vertical movement.
									sprite.HandleTileCollision(tile, resolutionDistance);

									numberOfCollidingTiles++;
									collisionDebugCollidedTiles.Add(tile);
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
				if (sprite == CollisionDebugSelectedSprite && GameServices.CollisionDebuggerActive)
				{
					Vector2 totalOffset = sprite.Position - spritePositionWithoutResolutions;
					GameServices.CollisionDebuggerForm.Update(numberOfCollidingTiles, slopeCollisionOccurred, totalOffset);
				}

				var spritesNear = Sprites.GetItemsNearItem(sprite).ToList();
				spritesNearSpriteCount += spritesNear.Count;
				foreach (var collidableSprite in spritesNear)
				{
					if (Object.ReferenceEquals(collidableSprite, sprite)) { continue; }
					
					//form.AddToLogText($"Processed {collidableSprite.GetType().FullName}");

					BoundingRectangle hitboxA = sprite.Hitbox;
					BoundingRectangle hitboxB = collidableSprite.Hitbox;

					Vector2 intersectA = hitboxA.GetIntersectionDepth(hitboxB);
					Vector2 intersectB = hitboxB.GetIntersectionDepth(hitboxA);
					intersectionDepthCalls += 2;

					if (!intersectA.IsNaN() && !intersectB.IsNaN() && !sprite.SpritesCollidedWithThisFrame.Contains(collidableSprite) && !collidableSprite.SpritesCollidedWithThisFrame.Contains(sprite))
					{
						sprite.HandleSpriteCollision(collidableSprite, intersectA);
						collidableSprite.HandleSpriteCollision(sprite, intersectB);
						spriteCollisionCallsMade += 2;

						sprite.SpritesCollidedWithThisFrame.Add(collidableSprite);
						collidableSprite.SpritesCollidedWithThisFrame.Add(sprite);
					}
				}

			}
			Sprite playerSprite = Sprites.First(s => s.GetType().FullName.Contains("PlayerMario"));

			debugText = $"Intersect Calls: {intersectionDepthCalls}, Sprite Collision Calls: {spriteCollisionCallsMade}, Near: {spritesNearSpriteCount}, Sprites: {Sprites.Count()}";
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
				ToggleEditor();
			}
			else if (InputManager.IsNewKeyPress(Keys.G))
			{
				string json = new IO.LevelSerializers.Serializer003().Serialize(Owner);
				System.IO.File.WriteAllText(@"test_003.lvl", json);
			}
			else if (InputManager.IsNewKeyPress(Keys.H))
			{
				irisEffect.Start(60, Interfaces.EffectDirection.Forward, Vector2.Zero, Color.Black);
			}
			else if (InputManager.IsNewKeyPress(Keys.K))
			{
				irisEffect.Start(60, Interfaces.EffectDirection.Backward, Vector2.Zero, Color.Black);
			}

			// debugText = $"{MousePosition.X}, {MousePosition.Y}";
			Tile tileUnderCursor = (!MousePosition.IsNaN()) ? GetTileAtPosition(MousePosition) : null;
			if (GameServices.CollisionDebuggerActive) { GameServices.CollisionDebuggerForm.SetTileInfo(tileUnderCursor); }
		}

		/// <summary>
		/// Draws the game objects in this section.
		/// </summary>
		public void Draw()
		{
			Background.Draw();
			foreach (Tile tile in Tiles)
			{
				tile.Draw();
				if (GameServices.CollisionDebuggerActive && tile.BreakOnCollision) { GameServices.SpriteBatch.DrawRectangle(tile.Hitbox.Bounds.ToRectangle(), Color.Red); }
			}
			foreach (Sprite sprite in Sprites)
			{
				sprite.Draw();
				if (GameServices.CollisionDebuggerActive && sprite.BreakOnCollision) { GameServices.SpriteBatch.DrawRectangleEdges(sprite.Hitbox.ToRectangle(), Color.DarkRed); }
			}

			editorSelectedObject.Draw();
			CameraSystem.Draw(debug: false);
			//Sprites.DrawCells();
			GameServices.DebugFont.DrawString(debugText, new Vector2(120f, 16f), 1f);
			intersectionDepthCalls = spriteCollisionCallsMade = spritesNearSpriteCount = 0;
			DrawCollisionDebug();
			irisEffect.Draw();
		}

		/// <summary>
		/// Gets the tile at a given position on the topmost layer.
		/// </summary>
		/// <param name="position">The position to get the tile at.</param>
		/// <returns>A <see cref="Tile"/> instance, or null if there is no tile at <paramref name="position"/>.</returns>
		public Tile GetTileAtPosition(Vector2 position)
		{
			// Gets a tile from the topmost layer at the given position.
			// One layer is above another if its index within the Layers list is higher.
			// The MainLayer is always the lowest layer.

			int highestIndex = Layers.Count - 1;
			for (int i = highestIndex; i >= 0; i--)
			{
				Layer layer = Layers[i];
				if (!layer.Bounds.IntersectsIncludingEdges(position)) { continue; }
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

		/// <summary>
		/// Adds a <see cref="Tile"/> to this section and to the main layer.
		/// </summary>
		/// <param name="tile">The tile to add.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="tile"/> reference is null.</exception>
		public void AddTile(Tile tile)
		{
			if (tile == null)
			{
				throw new ArgumentNullException(nameof(tile), "The tile to add to the section was null.");
			}

			if (!tile.Position.IsNaN() && MainLayer.GetTile(MainLayer.GetCellNumberAtPosition(tile.Position)) == null)
			{
				Tiles.Add(tile);
				if (!MainLayer.Tiles.Contains(tile)) { MainLayer.AddTile(tile); }
			}
		}

		/// <summary>
		/// Adds a <see cref="Sprite"/> to this section.
		/// </summary>
		/// <param name="sprite">The sprite to add.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="sprite"/> reference is null.</exception>
		public void AddSprite(Sprite sprite)
		{
			if (sprite == null)
			{
				throw new ArgumentNullException(nameof(sprite), "The sprite to add to the section was null.");
			}

			Sprites.Add(sprite);
			MainLayer.AddSprite(sprite);
		}

		/// <summary>
		/// Sets a sprite to be added to the section on the next frame.
		/// </summary>
		/// <param name="sprite">The sprite to add.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="sprite"/> reference is null.</exception>
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

		/// <summary>
		/// Removes a tile from this section and all layers it may be in.
		/// </summary>
		/// <param name="tile">The tile to remove.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="tile"/> reference is null.</exception>
		public void RemoveTile(Tile tile)
		{
			if (tile == null) { throw new ArgumentNullException(nameof(tile), "The tile to remove from the section was null."); }

			Tiles.Remove(tile);
			Layers.ForEach(l => l.RemoveTile(tile));
		}

		/// <summary>
		/// Removes a sprite from this section.
		/// </summary>
		/// <param name="sprite">The sprite to remove.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="sprite"/> reference is null.</exception>
		public void RemoveSprite(Sprite sprite)
		{
			if (sprite == null) { throw new ArgumentNullException(nameof(sprite), "The sprite to remove from the section was not null."); }

			Sprites.Remove(sprite);
		}

		private void ToggleEditor()
		{
			if (!EditorActive)
			{
				// Enable the level editor.
				EditorActive = true;

				editorTrackingObject = new EditorCameraTrackingObject();
				editorTrackingObject.Initialize(this);
				editorTrackingObject.Position = Camera.Viewport.Center;

				CameraSystem.StayInBounds = false;
				CameraSystem.TrackingObjects.Clear();
				CameraSystem.TrackingObjects.Add(editorTrackingObject);

				editorForm = new EditorForm(Owner, this, editorSelectedObject);
				editorForm.Show();
			}
			else
			{
				// Disable the level editor.
				EditorActive = false;

				editorTrackingObject = null;

				CameraSystem.StayInBounds = true;
				CameraSystem.TrackingObjects.Clear();
				// TODO: call whatever gets the right tracking objects here

				editorForm.Close();
				editorForm.Dispose();
				editorForm = null;
			}
		}

		private void InitializeCollisionDebugging()
		{
			GameServices.CollisionDebuggerForm.Section = this;
			Sprites.Add(selectorSprite);
			isCollisionDebuggingInitialized = true;
		}

		private void UninitializeCollisionDebugging()
		{
			selectorSprite.RemoveOnNextFrame = true;
			isCollisionDebuggingInitialized = false;
		}

		private void UpdateCollisionDebug()
		{
			if (GameServices.CollisionDebuggerActive)
			{
				
			}
		}

		private void DrawCollisionDebug()
		{
			if (GameServices.CollisionDebuggerActive && CollisionDebugSelectedSprite != null)
			{
				// Draw a rectangle outline around the selected sprite.
				GameServices.SpriteBatch.DrawRectangleEdges(CollisionDebugSelectedSprite.Hitbox.ToRectangle(), Color.Red);

				// Draw the hitbox of every collided tile.
				foreach (Tile tile in collisionDebugCollidedTiles)
				{
					if (tile.TileShape == CollidableShape.Rectangle)
					{
						GameServices.SpriteBatch.DrawRectangleEdges(((BoundingRectangle)tile.Hitbox).ToRectangle(), Color.LimeGreen);
					}
					else if (tile.TileShape == CollidableShape.RightTriangle)
					{
						((RightTriangle)tile.Hitbox).Draw(false);
					}
				}
			}
		}

		internal void CollisionDebugSelectSprite(Sprite sprite)
		{
			if (sprite == null) { throw new ArgumentNullException(nameof(sprite), "The sprite to select for collision debugging was null."); }

			CollisionDebugSelectedSprite = sprite;
			CameraSystem.TrackingObjects.Clear();
			CameraSystem.TrackingObjects.Add(sprite);
			GameServices.CollisionDebuggerForm.SelectedSprite = sprite;
		}
	}
}