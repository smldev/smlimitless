using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using SMLimitless.Collections;
using SMLimitless.Components;
using SMLimitless.Editor.Attributes;
using SMLimitless.Extensions;
using SMLimitless.Input;
using SMLimitless.Interfaces;
using SMLimitless.Physics;
using SMLimitless.Screens.Effects;
using SMLimitless.Sprites.InternalSprites;

namespace SMLimitless.Sprites.Collections
{
	/// <summary>
	///   The main area of gameplay.
	/// </summary>
	[HasUserEditableProperties]
	public sealed class Section : IDisposable
	{
		internal static PhysicsSetting<int> MaximumParticles = new PhysicsSetting<int>("Section: Maximum Particles", 1, 1000, 200, PhysicsSettingType.FloatingPoint);
		private List<Tile> collisionDebugCollidedTiles = new List<Tile>();
		private string debugText = "";
		private Debug.DebugForm form = new Debug.DebugForm();
		private IrisEffect irisEffect;
		private bool isCollisionDebuggingInitialized = false;
		private bool isContentLoaded;
		private bool isDeserialized;
		private bool isInitialized;
		private List<Particle> particlesToRemoveOnNextFrame = new List<Particle>();
		private CollisionDebugSelectSprite selectorSprite = new CollisionDebugSelectSprite();

		/// <summary>
		///   Gets the settings used for automatic camera scrolling for this section.
		/// </summary>
		public SectionAutoscrollSettings AutoscrollSettings { get; internal set; }

		/// <summary>
		///   Gets the <see cref="Sprites.Collections.Background" /> for this section.
		/// </summary>
		public Background Background { get; internal set; }

		/// <summary>
		///   Gets the bounds of this section, the rectangular area to which the
		///   camera is restricted.
		/// </summary>
		[BoundingRectangleProperty("Bounds", "The rectangle within which all the elements of the section exist.")]
		public BoundingRectangle Bounds { get; internal set; }

		/// <summary>
		///   Gets the camera viewing this section.
		/// </summary>
		[NestedProperty("Camera", "camera")]
		public Camera2D Camera { get; private set; }

		/// <summary>
		///   Gets the <see cref="Physics.CameraSystem" /> instance tracking
		///   objects in this section.
		/// </summary>
		public CameraSystem CameraSystem { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the level editor is currently
		///   enabled for this section.
		/// </summary>
		public bool EditorActive { get; internal set; }

		/// <summary>
		///   Gets the <see cref="SMLimitless.Components.HUDInfo" /> for this section.
		/// </summary>
		public HUDInfo HUDInfo { get; private set; }

		/// <summary>
		///   Gets or sets the numeric index of this section within its level.
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		///   Gets a value indicating whether this section is active. Inactive
		///   sections will be drawn but not updated.
		/// </summary>
		public bool IsActive { get; internal set; } = true;

		/// <summary>
		///   Gets a value indicating whether first-stage loading
		///   (deserialization and game object initialization) has completed.
		/// </summary>
		public bool IsLoaded { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this section is the first loaded when a level begins.
        /// </summary>
		public bool IsStartSection { get; internal set; } = false;

		/// <summary>
		///   Gets the current mouse position adjusted for the camera's position
		///   and zoom.
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

		/// <summary>
		///   Gets or sets the name of this section.
		/// </summary>
		[StringProperty("Name", "The name of this section.")]
		public string Name { get; set; }

		/// <summary>
		///   Gets the <see cref="Level" /> that owns this section.
		/// </summary>
		public Level Owner { get; private set; }

		/// <summary>
		///   Gets a read-only list of players in the section.
		/// </summary>
		public IReadOnlyList<Sprite> PlayerList { get { return Players.AsReadOnly(); } }

		/// <summary>
		///   Gets a read-only list of sprites in this section.
		/// </summary>
		public IReadOnlyList<Sprite> SpriteList { get { return Sprites.AsReadOnly(); } }

		internal Sprite CollisionDebugSelectedSprite { get; set; }

		/// <summary>
		///   Gets a section exit that a player has entered before other players have.
		/// </summary>
		internal SectionExit ExitLock { get; set; } = null;

		internal Vector2 LastEditorCameraPosition { get; set; } = Vector2.Zero;
		internal List<Layer> Layers { get; set; }

		internal Layer MainLayer { get; set; }

		internal List<Particle> Particles { get; private set; } = new List<Particle>();
		internal List<Path> Paths { get; set; }

		internal List<Sprite> Players { get; set; } = new List<Sprite>();

		internal List<SectionExit> SectionExits { get; private set; } = new List<SectionExit>();
		internal List<Sprite> Sprites { get; set; } = new List<Sprite>();
		internal SparseCellGrid<Sprite> SpritesGrid { get; set; }

		internal List<Sprite> SpritesToAddOnNextFrame { get; } = new List<Sprite>();
		internal List<Sprite> SpritesToRemoveOnNextFrame { get; } = new List<Sprite>();

		internal List<Tile> Tiles { get; private set; }

		/// <summary>
		///   Initializes a new instance of the <see cref="Section" /> class.
		/// </summary>
		/// <param name="owner">The <see cref="Level" /> that owns this section.</param>
		public Section(Level owner)
		{
			Camera = new Camera2D();
			Owner = owner;
			SpritesGrid = new SparseCellGrid<Sprite>(GameServices.GameObjectSize * new Vector2(4f));
			Layers = new List<Layer>();
			Tiles = new List<Tile>();
			Paths = new List<Path>();
			Background = new Background(this);
			HUDInfo = new HUDInfo(100);

			// temporary
			GameServices.Camera = Camera;
		}

		/// <summary>
		///   Adds a particle to this section.
		/// </summary>
		/// <param name="particle">The particle to add.</param>
		public void AddParticle(Particle particle)
		{
			if (particle == null) { throw new ArgumentNullException("The provided particle was null."); }
			particle.LoadContent();

			if (Particles.Count == MaximumParticles.Value)
			{
				Particles.RemoveAt(0);
			}

			Particles.Add(particle);
		}

		/// <summary>
		///   Adds a <see cref="Sprite" /> to this section.
		/// </summary>
		/// <param name="sprite">The sprite to add.</param>
		/// <exception cref="ArgumentNullException">
		///   Thrown if the <paramref name="sprite" /> reference is null.
		/// </exception>
		public void AddSprite(Sprite sprite)
		{
			if (sprite == null)
			{
				throw new ArgumentNullException(nameof(sprite), "The sprite to add to the section was null.");
			}

			if (sprite.IsPlayer) { Players.Add(sprite); }

			Sprites.Add(sprite);
			SpritesGrid.Add(sprite);
			MainLayer.AddSprite(sprite);

			Debug.Logger.LogInfo($"Sprite {sprite.GetType().Name} spawn at {sprite.Position}");
		}

		/// <summary>
		///   Sets a sprite to be added to the section on the next frame.
		/// </summary>
		/// <param name="sprite">The sprite to add.</param>
		/// <exception cref="ArgumentNullException">
		///   Thrown if the <paramref name="sprite" /> reference is null.
		/// </exception>
		public void AddSpriteOnNextFrame(Sprite sprite)
		{
			if (sprite == null)
			{
				throw new ArgumentNullException(nameof(sprite), "The sprite to add to the section was null.");
			}

			SpritesToAddOnNextFrame.Add(sprite);
		}

		/// <summary>
		///   Adds a <see cref="Tile" /> to this section and to the main layer.
		/// </summary>
		/// <param name="tile">The tile to add.</param>
		/// <exception cref="ArgumentNullException">
		///   Thrown if the <paramref name="tile" /> reference is null.
		/// </exception>
		public void AddTile(Tile tile)
		{
			if (tile == null)
			{
				throw new ArgumentNullException(nameof(tile), "The tile to add to the section was null.");
			}

			if (!tile.Position.IsNaN() && MainLayer.GetTile(MainLayer.GetCellNumberAtPosition(tile.Position)) == null)
			{
				if (!MainLayer.Tiles.Contains(tile)) { MainLayer.AddTile(tile); }
				else { Tiles.Add(tile); }
			}
		}

		/// <summary>
		///   Draws the game objects in this section.
		/// </summary>
		public void Draw()
		{
			Background.Draw();

			foreach (SectionExit exit in SectionExits)
			{
				exit.Draw();
			}

			foreach (Tile tile in Tiles)
			{
				tile.Draw();
				if (GameServices.CollisionDebuggerActive && tile.BreakOnCollision) { GameServices.SpriteBatch.DrawRectangle(tile.Hitbox.Bounds.ToRectangle(), Color.Red); }
			}
			foreach (Sprite sprite in SpritesGrid)
			{
				sprite.Draw();
				if (GameServices.CollisionDebuggerActive && sprite.BreakOnCollision) { GameServices.SpriteBatch.DrawRectangleEdges(sprite.Hitbox.ToRectangle(), Color.DarkRed); }
			}

			foreach (Particle particle in Particles)
			{
				particle.Draw();
			}

			foreach (SectionExit exit in SectionExits)
			{
				exit.DebugDraw();
			}

			CameraSystem.Draw(debug: false);
			GameServices.DebugFont.DrawString($"{HUDInfo.Score:D9}", new Vector2(120f, 16f) + Camera.Position, 1f);
			GameServices.DebugFont.DrawString($"{HUDInfo.Coins:D2}", new Vector2(600f, 16f) + Camera.Position, 1f);
			GameServices.DebugFont.DrawString(debugText, new Vector2(120f, 48f) + Camera.Position, 1f);
			DrawCollisionDebug();
			irisEffect.Draw();
		}

		/// <summary>
		///   Gets the tile at a given position on the topmost layer.
		/// </summary>
		/// <param name="position">The position to get the tile at.</param>
		/// <returns>
		///   A <see cref="Tile" /> instance, or null if there is no tile at
		///   <paramref name="position" />.
		/// </returns>
		public Tile GetTileAtPosition(Vector2 position)
		{
			// Gets a tile from the topmost layer at the given position. One
			// layer is above another if its index within the Layers list is
			// higher. The MainLayer is always the lowest layer.

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

		/// <summary>
		///   Initializes the game objects for this section.
		/// </summary>
		public void Initialize()
		{
			if (!isInitialized)
			{
				Background.Initialize();
				Layers.ForEach(l => l.Initialize());
				SpritesGrid.ForEach(s => s.Initialize(this));
				selectorSprite.Initialize(this);

				CameraSystem = new CameraSystem(Camera, Bounds);
				irisEffect = new IrisEffect(Camera.Viewport.Center);

				CameraSystem.TrackingObjects.AddRange(Players);

				isInitialized = true;
			}
		}

		/// <summary>
		///   Plays an iris-in effect, turning the screen black.
		/// </summary>
		/// <param name="length">The number of frames the effect should last.</param>
		/// <param name="position">The position the iris should close on.</param>
		/// <param name="onEffectCompleted">
		///   An action ran when the iris has closed.
		/// </param>
		public void IrisIn(int length, Vector2 position, Action<object, EffectCompletedEventArgs> onEffectCompleted)
		{
			EffectCompletedEventHandler handler = null;

			handler = (sender, args) =>
			{
				onEffectCompleted(sender, args);
				irisEffect.EffectCompletedEvent -= handler;
			};

			irisEffect.EffectCompletedEvent += handler;
			irisEffect.Start(length, EffectDirection.Backward, position, Color.Black);
		}

		/// <summary>
		///   Plays an iris-out effect, bringing the screen back.
		/// </summary>
		/// <param name="length">The number of frames the effect should last.</param>
		/// <param name="position">The position the iris should open from.</param>
		/// <param name="onEffectCompleted">
		///   An action ran when the iris has opened.
		/// </param>
		public void IrisOut(int length, Vector2 position, Action<object, EffectCompletedEventArgs> onEffectCompleted)
		{
			EffectCompletedEventHandler handler = null;

			handler = (sender, args) =>
			{
				onEffectCompleted(sender, args);
				irisEffect.EffectCompletedEvent -= handler;
			};

			irisEffect.EffectCompletedEvent += handler;
			irisEffect.Start(length, EffectDirection.Forward, position, Color.Black);
		}

		/// <summary>
		///   Loads the content for the game objects in this section.
		/// </summary>
		public void LoadContent()
		{
			if (!isContentLoaded)
			{
				Background.LoadContent();
				Layers.ForEach(l => l.LoadContent());
				SpritesGrid.ForEach(s => s.LoadContent());

				irisEffect.LoadContent();
				irisEffect.Start(90, EffectDirection.Forward, Vector2.Zero, Color.Black);

				isContentLoaded = true;
			}
		}

		/// <summary>
		///   Handles the death of a player.
		/// </summary>
		/// <param name="player">The player that died.</param>
		public void PlayerKilled(Sprite player)
		{
			// TODO: add code to check if all players are dead

			CameraSystem.TrackingObjects.Remove(player);
			Vector2 createPlayerAt = Tiles.First(t => GetTileAtPosition(new Vector2(t.Position.X, t.Position.Y - 8f)) == null).Position;
			createPlayerAt.Y -= 16f;
			Sprite playerSprite = Assemblies.AssemblyManager.GetSpriteByFullName("SmlSprites.Players.PlayerMario");
			playerSprite.Initialize(this);
			playerSprite.LoadContent();
			playerSprite.Position = createPlayerAt;
			SpritesToAddOnNextFrame.Add(playerSprite);
			CameraSystem.TrackingObjects.Add(playerSprite);
		}

		/// <summary>
		///   Sets a particle to be removed from the section on the next frame.
		/// </summary>
		/// <param name="particle">The particle to remove.</param>
		public void RemoveParticleOnNextFrame(Particle particle)
		{
			if (particle == null) { throw new ArgumentNullException("The provided particle was null."); }
			particlesToRemoveOnNextFrame.Add(particle);
		}

		/// <summary>
		///   Removes a sprite from this section.
		/// </summary>
		/// <param name="sprite">The sprite to remove.</param>
		/// <exception cref="ArgumentNullException">
		///   Thrown if the <paramref name="sprite" /> reference is null.
		/// </exception>
		public void RemoveSprite(Sprite sprite)
		{
			if (sprite == null) { throw new ArgumentNullException(nameof(sprite), "The sprite to remove from the section was not null."); }

			Sprites.Remove(sprite);
			SpritesGrid.Remove(sprite);
			if (sprite.IsPlayer) { Players.Remove(sprite); }

			Debug.Logger.LogInfo($"Removed sprite {sprite.GetType().Name} from {sprite.Position}");
		}

		/// <summary>
		///   Sets a sprite to be removed on the next frame.
		/// </summary>
		/// <param name="sprite">The sprite to remove.</param>
		public void RemoveSpriteOnNextFrame(Sprite sprite)
		{
			if (sprite == null) { throw new ArgumentNullException(nameof(sprite), "The sprite to remove from the section was null."); }

			SpritesToRemoveOnNextFrame.Add(sprite);
		}

		/// <summary>
		///   Removes a tile from this section and all layers it may be in.
		/// </summary>
		/// <param name="tile">The tile to remove.</param>
		/// <exception cref="ArgumentNullException">
		///   Thrown if the <paramref name="tile" /> reference is null.
		/// </exception>
		public void RemoveTile(Tile tile)
		{
			if (tile == null) { throw new ArgumentNullException(nameof(tile), "The tile to remove from the section was null."); }

			Tiles.Remove(tile);
			Layers.ForEach(l => l.RemoveTile(tile));
		}

		/// <summary>
		///   Sets the state of this iris effect.
		/// </summary>
		/// <param name="closed">
		///   If this parameter is true, the iris will be closed and the screen
		///   will be black. If this parameter is false, the iris will be open
		///   and the screen will consist of the section.
		/// </param>
		public void SetIrisState(bool closed)
		{
			irisEffect.Set((closed) ? EffectDirection.Backward : EffectDirection.Forward, Color.Black);
		}

		/// <summary>
		///   Updates this section.
		/// </summary>
		public void Update()
		{
			System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

			irisEffect.Update();

			if (!IsActive)
			{
				foreach (var exit in SectionExits) { exit.Update(); }
				Background.Update();
				stopwatch.Stop();
				return;
			}

			AddAndRemoveSpritesForNextFrame();

			if (!Owner.EditorActive)
			{
				if (GameServices.CollisionDebuggerActive && !isCollisionDebuggingInitialized) { InitializeCollisionDebugging(); }
				else if (!GameServices.CollisionDebuggerActive && isCollisionDebuggingInitialized) { UninitializeCollisionDebugging(); }

				UpdateObjectActiveStates();

				Tiles.Where(t => t.IsActive).ForEach(t => t.Update());
				SpritesGrid.Where(s => s.ActiveState == SpriteActiveState.Active || s.ActiveState == SpriteActiveState.AlwaysActive).ForEach(s => s.Update());
				SpritesGrid.Update();
				UpdatePhysics();
				SpritesGrid.ForEach(s => s.SpritesCollidedWithThisFrame.Clear());
				SpritesGrid.Update();
				UpdateParticles();
				SectionExits.ForEach(s => s.Update());
			}

			SpritesGrid.RemoveAllWhere(s => s.RemoveOnNextFrame);
			RemoveParticlesToRemoveOnNextFrame();
			CameraSystem.Update();
			Background.Update();
			TempUpdate();

			debugText = $"{EditorActive}";

			stopwatch.Stop();
		}

		internal void ActivateEditor(EditorCameraTrackingObject trackingObject, EditorSelectedObject selectedObject)
		{
			CameraSystem.StayInBounds = false;
			CameraSystem.TrackingObjects.Clear();
			CameraSystem.TrackingObjects.Add(trackingObject);

			trackingObject.Position = LastEditorCameraPosition;

			AddSpriteOnNextFrame(trackingObject);
			AddSpriteOnNextFrame(selectedObject);
		}

		internal void CollisionDebugSelectSprite(Sprite sprite)
		{
			if (sprite == null) { throw new ArgumentNullException(nameof(sprite), "The sprite to select for collision debugging was null."); }

			CollisionDebugSelectedSprite = sprite;
			CameraSystem.TrackingObjects.Clear();
			CameraSystem.TrackingObjects.Add(sprite);
			GameServices.CollisionDebuggerForm.SelectedSprite = sprite;
		}

		internal void DeactivateEditor()
		{
			CameraSystem.StayInBounds = true;
			CameraSystem.TrackingObjects.Clear();
			CameraSystem.TrackingObjects.AddRange(Players.Where(p => !p.HasAttribute("Dead")));

			SpriteList.Where(s => s is EditorCameraTrackingObject || s is EditorSelectedObject)
			.ForEach(s => RemoveSpriteOnNextFrame(s));
		}

		private void AddAndRemoveSpritesForNextFrame()
		{
			if (SpritesToAddOnNextFrame.Any())
			{
				SpritesToAddOnNextFrame.ForEach(s => AddSprite(s));
				SpritesToAddOnNextFrame.Clear();
			}

			if (SpritesToRemoveOnNextFrame.Any())
			{
				SpritesToRemoveOnNextFrame.ForEach(s => RemoveSprite(s));
				SpritesToRemoveOnNextFrame.Clear();
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

		private void InitializeCollisionDebugging()
		{
			GameServices.CollisionDebuggerForm.Section = this;
			SpritesGrid.Add(selectorSprite);
			isCollisionDebuggingInitialized = true;
		}

		private void RemoveParticlesToRemoveOnNextFrame()
		{
			Particles.RemoveAll(p => particlesToRemoveOnNextFrame.Contains(p));
			particlesToRemoveOnNextFrame.Clear();
		}

		private void ResolveSpriteCollisionWithSlopes(Sprite sprite, float delta, ref Vector2 spritePositionWithoutResolutions, ref int numberOfCollidingTiles, out bool slopeCollisionOccurred)
		{
			int resolutionDirection = 0;
			int slopeResolutionDirection = 0;
			Vector2 adjacentCell = new Vector2(float.NaN);
			slopeCollisionOccurred = false;

			float xMovement = sprite.Velocity.X * delta;
			sprite.Position = new Vector2((sprite.Position.X + xMovement), sprite.Position.Y);
			spritePositionWithoutResolutions.X += xMovement;

			foreach (Layer layer in GetLayersIntersectingRectangle(sprite.Hitbox))
			{
				Vector2 cellRangeTopLeft = layer.GetClampedCellNumberAtPosition(sprite.Position);
				Vector2 cellRangeBottomRight = layer.GetClampedCellNumberAtPosition(sprite.Hitbox.BottomRight);

				for (int y = (int)cellRangeTopLeft.Y; y <= (int)cellRangeBottomRight.Y; y++)
				{
					for (int x = (int)cellRangeTopLeft.X; x <= (int)cellRangeBottomRight.X; x++)
					{
						Tile tile = layer.SafeGetTile(new Vector2(x, y));
						if (tile != null && tile.TileShape == CollidableShape.RightTriangle)
						{
							HorizontalDirection tileSlopeDirection = tile.SlopedSides.GetHorizontalDirection();
							HorizontalDirection adjacentDirection = (tileSlopeDirection == HorizontalDirection.Left) ? HorizontalDirection.Right : HorizontalDirection.Left;
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
								tile.HandleCollision(sprite, resolutionDistance);

								numberOfCollidingTiles++;
								collisionDebugCollidedTiles.Add(tile);
								slopeCollisionOccurred = true;
							}
							else
							{
								sprite.IsEmbedded = true;
								Debug.Logger.LogInfo($"Sprite embedded at {sprite.Position}");
								continue;
							}
						}
					}
				}
			}
		}

		private void ResolveSpriteHorizontalCollision(Sprite sprite, float delta, ref Vector2 spritePositionWithoutResolutions, ref int numberOfCollidingTiles, bool slopeCollisionOccurred)
		{
			int resolutionDirection = 0;

			foreach (var layer in GetLayersIntersectingRectangle(sprite.Hitbox))
			{
				Vector2 cellRangeTopLeft = layer.GetClampedCellNumberAtPosition(sprite.Position);
				Vector2 cellRangeBottomRight = layer.GetClampedCellNumberAtPosition(sprite.Hitbox.BottomRight);

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
							// The horizontal resolution direction is equal to
							// the sign of the resolution distance.

							if (resolutionDistance.X < 0f && (tile.AdjacencyFlags & TileAdjacencyFlags.SlopeOnLeft) == TileAdjacencyFlags.SlopeOnLeft) { continue; }
							else if (resolutionDistance.X > 0f && (tile.AdjacencyFlags & TileAdjacencyFlags.SlopeOnRight) == TileAdjacencyFlags.SlopeOnRight) { continue; }

							if (resolutionDirection == 0 || Math.Sign(resolutionDistance.X) == Math.Sign(resolutionDirection))  // If there has been no other horizontal resolution this frame, or if the last horizontal resolution was in the same direction as this one...
							{
								resolutionDirection = Math.Sign(resolutionDistance.X);                                          // The resolution direction is equal to the sign of the resolution distance.
								sprite.Position = new Vector2((sprite.Position.X + resolutionDistance.X), sprite.Position.Y);   // Move the sprite to resolve the collision.
								sprite.Velocity = new Vector2(0f, sprite.Velocity.Y);
								sprite.Acceleration = new Vector2(0f, sprite.Acceleration.Y);
								sprite.HandleTileCollision(tile, resolutionDistance);
								tile.HandleCollision(sprite, resolutionDistance);

								numberOfCollidingTiles++;
								collisionDebugCollidedTiles.Add(tile);
							}
							else
							{
								sprite.IsEmbedded = true;   // The sprite is embedded.
								UpdateCollisionDebuggerInfo(sprite, numberOfCollidingTiles, slopeCollisionOccurred, sprite.Position - spritePositionWithoutResolutions);
								return;
							}
						}
					}
				}
			}
		}

		private void ResolveSpriteSpriteCollisions(Sprite sprite, IEnumerable<Sprite> nearbySprites, Vector2 tileCollisionOffset)
		{
			if (sprite.SpriteCollisionMode == SpriteCollisionMode.NoCollision) { return; }

			foreach (Sprite collidableSprite in nearbySprites)
			{
				if (collidableSprite.SpriteCollisionMode == SpriteCollisionMode.NoCollision) { continue; }

				BoundingRectangle hitboxA = sprite.Hitbox;
				BoundingRectangle hitboxB = collidableSprite.Hitbox;

				Vector2 resolutionA = hitboxA.GetCollisionResolution(hitboxB);
				Vector2 resolutionB = hitboxB.GetCollisionResolution(hitboxA);

				if (!resolutionA.IsNaN() && !resolutionB.IsNaN() && !sprite.SpritesCollidedWithThisFrame.Contains(collidableSprite) && !collidableSprite.SpritesCollidedWithThisFrame.Contains(sprite))
				{
					Vector2 resolutionDistance = hitboxA.GetCollisionResolution(resolutionA);

					// Move the sprite out of the other sprite if we don't move
					// it into a tile.
					if (sprite.SpriteCollisionMode == SpriteCollisionMode.OffsetNotify || sprite.SpriteCollisionMode == SpriteCollisionMode.OffsetOnly)
					{
						//if (Math.Sign(resolutionDistance.X) == Math.Sign(tileCollisionOffset.X)) { sprite.Position = new Vector2((sprite.Position.X + resolutionDistance.X), sprite.Position.Y); }
						//if (Math.Sign(resolutionDistance.Y) == Math.Sign(tileCollisionOffset.Y)) { sprite.Position = new Vector2(sprite.Position.X, (sprite.Position.Y + resolutionDistance.Y)); }
						sprite.SpritesCollidedWithThisFrame.Add(collidableSprite);
					}

					if (sprite.SpriteCollisionMode == SpriteCollisionMode.OffsetNotify || sprite.SpriteCollisionMode == SpriteCollisionMode.NotifyOnly)
					{
						sprite.HandleSpriteCollision(collidableSprite, resolutionA);
					}

					if (collidableSprite.SpriteCollisionMode == SpriteCollisionMode.OffsetNotify || collidableSprite.SpriteCollisionMode == SpriteCollisionMode.NotifyOnly)
					{
						collidableSprite.HandleSpriteCollision(sprite, resolutionB);
					}

					collidableSprite.SpritesCollidedWithThisFrame.Add(sprite);
				}
			}
		}

		private void ResolveSpriteVerticalCollision(Sprite sprite, float delta, ref Vector2 spritePositionWithoutResolutions, ref int numberOfCollidingTiles, bool slopeCollisionOccurred)
		{
			int resolutionDirection = 0;

			sprite.Position = new Vector2(sprite.Position.X, (sprite.Position.Y + sprite.Velocity.Y * delta));
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
								tile.HandleCollision(sprite, resolutionDistance);

								numberOfCollidingTiles++;
								collisionDebugCollidedTiles.Add(tile);
							}
							else  // ...but if the last vertical resolution was in the opposite direction...
							{
								sprite.IsEmbedded = true;   // ...the sprite is embedded (up and down resolutions in the same frame).
								UpdateCollisionDebuggerInfo(sprite, numberOfCollidingTiles, slopeCollisionOccurred, sprite.Position - spritePositionWithoutResolutions);
								return;
							}
						}
					}
				}
			}
		}

		private void SaveLevel()
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Title = "Super Mario Limitless";
			sfd.Filter = "SML Level File (*.lvl)|*.lvl|All files (*.*)|*.*";

			if (sfd.ShowDialog() == DialogResult.Cancel) { return; }

			string json = new IO.LevelSerializers.Serializer003().Serialize(Owner);
			System.IO.File.WriteAllText(sfd.FileName, json);
		}

		private void SnapToGround(Sprite sprite)
		{
			//if (!sprite.IsOnGround) { return; }

			//Vector2 checkPoint = sprite.Hitbox.BottomCenter;
			//checkPoint.Y += GameServices.GameObjectSize.Y / 4f;
			//Tile tileAtCheckPoint = GetTileAtPosition(checkPoint);

			//if (tileAtCheckPoint != null)
			//{
			//	sprite.Position = new Vector2(sprite.Position.X, (tileAtCheckPoint.Hitbox.GetTopPoint(sprite.Hitbox.Center.X).Y - sprite.Hitbox.Height));
			//}
		}

		private void TempUpdate()
		{
			if (InputManager.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.OemTilde))
			{
				if (!GameServices.DebugForm.Visible) { GameServices.DebugForm.Show(); }
				else { GameServices.DebugForm.Hide(); }
			}
			else if (InputManager.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.E))
			{
				Owner.ToggleEditor();
			}
			else if (InputManager.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.G))
			{
				SaveLevel();
			}
			else if (InputManager.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.H))
			{
				IrisOut(90, PlayerList.First().Position, (sender, e) => { });
			}
			else if (InputManager.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.K))
			{
				IrisIn(90, PlayerList.First().Position, (sender, e) => { });
			}
			Tile tileUnderCursor = (!MousePosition.IsNaN()) ? GetTileAtPosition(MousePosition) : null;
			if (GameServices.CollisionDebuggerActive) { GameServices.CollisionDebuggerForm.SetTileInfo(tileUnderCursor); }

			foreach (var exit in SectionExits)
			{
				foreach (var player in Players)
				{
					if (exit.CanPlayerEnter(player))
					{
						exit.PlayerEntered(player);
					}
				}
			}
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

		private void UpdateCollisionDebuggerInfo(Sprite sprite, int numberOfCollidingTiles, bool slopeCollisionOccurred, Vector2 totalOffset)
		{
			if (sprite == CollisionDebugSelectedSprite && GameServices.CollisionDebuggerActive)
			{
				GameServices.CollisionDebuggerForm.Update(numberOfCollidingTiles, slopeCollisionOccurred, totalOffset);
			}
		}

		private void UpdateObjectActiveStates()
		{
			// The only objects that need to be updated are those that are
			// active; that is, those within the CameraSystem.ActiveBounds rectangle.

			if (EditorActive) { return; }

			int updatedTiles = 0;
			int updatedSprites = 0;

			foreach (Tile tile in Tiles)
			{
				bool currentActiveState = tile.IsActive;
				tile.IsActive = tile.Hitbox.Bounds.IntersectsIncludingEdges(CameraSystem.ActiveBounds);
				if (tile.IsActive != currentActiveState)
				{
					updatedTiles++;
				}
			}

			foreach (Sprite sprite in Sprites)
			{
				if (sprite.IsPlayer) { continue; }

				SpriteActiveState currentActiveState = sprite.ActiveState;
				bool withinBounds = sprite.Hitbox.IntersectsIncludingEdges(CameraSystem.ActiveBounds);
				bool initialPositionWithinBounds = CameraSystem.ActiveBounds.Within(sprite.InitialPosition, true);

				if (currentActiveState == SpriteActiveState.AlwaysActive) { continue; }
				if (currentActiveState == SpriteActiveState.Active && !withinBounds)
				{
					if (!sprite.HasAttribute("DoNotRespawn"))
					{
						sprite.ActiveState = SpriteActiveState.WaitingToLeaveBounds;
						sprite.Deactivate();
						updatedSprites++;
					}
					else
					{
						RemoveSpriteOnNextFrame(sprite);
					}
				}
				else if (currentActiveState == SpriteActiveState.WaitingToLeaveBounds && !initialPositionWithinBounds)
				{
					sprite.ActiveState = SpriteActiveState.Inactive;
					sprite.Position = sprite.InitialPosition;
					updatedSprites++;
				}
				else if (currentActiveState == SpriteActiveState.Inactive && initialPositionWithinBounds)
				{
					sprite.ActiveState = SpriteActiveState.Active;
					sprite.Activate();
					updatedSprites++;
				}
			}
		}

		private void UpdateParticles()
		{
			foreach (Particle particle in Particles)
			{
				if (!CameraSystem.ActiveBounds.Within(particle.Position, false))
				{
					particlesToRemoveOnNextFrame.Add(particle);
				}
				else
				{
					particle.Update();
				}
			}
		}

		/// <summary>
		///   Moves sprites according to their velocity and performs collision
		///   detection and resolution.
		/// </summary>
		private void UpdatePhysics()
		{
			collisionDebugCollidedTiles.Clear();
			float delta = GameServices.GameTime.GetElapsedSeconds();    // The number of seconds that have elapsed since the last Update call.

			foreach (Sprite sprite in Sprites)
			{
				if (sprite.TileCollisionMode == SpriteCollisionMode.NoCollision)
				{
					sprite.Position += sprite.Velocity * delta;
					continue;
				}

				Vector2 spritePositionWithoutResolutions = sprite.Position;
				Vector2 initialSpritePosition = sprite.Position;
				sprite.IsOnGround = false;

				UpdateSpriteEmbeddedState(sprite);

				int numberOfCollidingTiles = 0;
				bool slopeCollisionOccurred = false;

				ResolveSpriteCollisionWithSlopes(sprite, delta, ref spritePositionWithoutResolutions, ref numberOfCollidingTiles, out slopeCollisionOccurred);
				ResolveSpriteHorizontalCollision(sprite, delta, ref spritePositionWithoutResolutions, ref numberOfCollidingTiles, slopeCollisionOccurred);
				ResolveSpriteVerticalCollision(sprite, delta, ref spritePositionWithoutResolutions, ref numberOfCollidingTiles, slopeCollisionOccurred);
				SnapToGround(sprite);

				var spritesNear = SpritesGrid.GetItemsNearItem(sprite).ToList();
				ResolveSpriteSpriteCollisions(sprite, spritesNear, sprite.Position - initialSpritePosition);
			}
		}

		private void UpdateSpriteEmbeddedState(Sprite sprite)
		{
			if (!sprite.IsEmbedded) { return; }

			// Does the sprite collide with any tiles?

			foreach (Layer layer in GetLayersIntersectingRectangle(sprite.Hitbox))
			{
				Vector2 cellRangeTopLeft = layer.GetClampedCellNumberAtPosition(sprite.Position);
				Vector2 cellRangeBottomRight = layer.GetClampedCellNumberAtPosition(sprite.Hitbox.BottomRight);

				for (int y = (int)cellRangeTopLeft.Y; y <= (int)cellRangeBottomRight.Y; y++)
				{
					for (int x = (int)cellRangeTopLeft.X; x <= (int)cellRangeBottomRight.X; x++)
					{
						Tile tile = layer.SafeGetTile(new Vector2(x, y));

						if (tile == null) { continue; }

						// Is the tile solid on the left and/or right sides?
						if (tile.TileShape == CollidableShape.Rectangle)
						{
							if (((tile.RectSolidSides & TileRectSolidSides.Left) != 0) || ((tile.RectSolidSides & TileRectSolidSides.Right) != 0))
							{
								return;
							}
						}
						else if (tile.TileShape == CollidableShape.RightTriangle)
						{
							if (((tile.TriSolidSides & TileTriSolidSides.Slope) != 0) || ((tile.TriSolidSides & TileTriSolidSides.VerticalLeg) != 0))
							{
								return;
							}
						}
					}
				}

				sprite.IsEmbedded = false;
			}
		}

        /// <summary>
        /// Gets a value indicating whether the resources of this object have been released.
        /// </summary>
        public bool IsDisposed { get; private set; }

        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    if (form != null && !form.IsDisposed) { form.Dispose(); }
                    if (Background != null && !Background.IsDisposed) { Background.Dispose(); }
                    if (irisEffect != null && !irisEffect.IsDisposed) { irisEffect.Dispose(); }
                }

                Tiles = null;
                Sprites = null;
                SpritesGrid = null;
                Players = null;

                IsDisposed = true;
            }
        }

        /// <summary>
        /// Releases resources used by this <see cref="Section"/> class. 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
