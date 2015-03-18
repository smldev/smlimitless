//-----------------------------------------------------------------------
// <copyright file="Section.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.Physics;
using SMLimitless.Sprites.Assemblies;

namespace SMLimitless.Sprites.Collections
{
    /// <summary>
    /// A part of a level.
    /// </summary>
    public sealed class Section : ISerializable
    {
		private Debug.DebugForm form = new Debug.DebugForm();

		/// <summary>
		/// The backing field for the AutoscrollSpeed property.
		/// </summary>
		private Vector2 autoscrollSpeed;

		/// <summary>
        /// The backing field for the AutoscrollPathName property.
        /// </summary>
        private string autoscrollPathName;

		/// <summary>
		/// Text to draw onscreen. Use to diagnose/debug stuff.
		/// </summary>
		private string debugText = "";

		/// <summary>
        /// A list of all tiles in this section.
        /// </summary>
        /// <remarks>This list is a reflection of all the layers' tile lists.</remarks>
        private List<Tile> tiles;

		/// <summary>
        /// A field that stores whether the section has been initialized.
        /// </summary>
        private bool isInitialized;

		/// <summary>
		/// A field that stores whether the content for this section has been loaded.
		/// </summary>
		private bool isContentLoaded;

		/// <summary>
		/// Gets the name of the path that the camera is following.
		/// </summary>
		public string AutoscrollPathName
		{
			get
			{
				if (this.ScrollType != CameraScrollType.AutoScrollAlongPath)
				{
					throw new InvalidOperationException("Section.AutoscrollPathName.get: Section scroll type is not autoscrolling.");
				}

				return this.autoscrollPathName;
			}

			internal set
			{
				if (this.ScrollType != CameraScrollType.AutoScrollAlongPath && value != null)
				{
					throw new InvalidOperationException("Section.AutoscrollPathName.set: Section scroll type is not autoscrolling.");
				}

				this.autoscrollPathName = value;
			}
		}

		/// <summary>
		/// Gets the speed at which the camera is autoscrolling through the level.
		/// </summary>
		public Vector2 AutoscrollSpeed
		{
			get
			{
				if (this.ScrollType != CameraScrollType.AutoScroll)
				{
					throw new InvalidOperationException("Section.AutoscrollSpeed.get: Section scroll type is not autoscrolling.");
				}

				return this.autoscrollSpeed;
			}

			internal set
			{
				if (this.ScrollType != CameraScrollType.AutoScroll && !value.IsNaN())
				{
					throw new InvalidOperationException("Section.AutoscrollSpeed.set: Section scroll type is not autoscrolling.");
				}

				this.autoscrollSpeed = value;
			}
		}

		/// <summary>
		/// Gets the background of this section.
		/// </summary>
		public Background Background { get; internal set; }

		/// <summary>
        /// Gets a rectangle representing the bounds of this section.
        /// </summary>
        public BoundingRectangle Bounds { get; internal set; }

		/// <summary>
		/// Gets the camera that is used to display a part of the section.
		/// </summary>
		public Camera2D Camera { get; private set; }

		/// <summary>
        /// Gets or sets the index number of this section.
        /// </summary>
        public int Index { get; set; }

		/// <summary>
		/// Gets or sets a collection of all the layers in this section.
		/// </summary>
		internal List<Layer> Layers { get; set; }

		/// <summary>
		/// Gets the main layer of this section.
		/// </summary>
		internal Layer MainLayer { get; private set; }

		/// <summary>
        /// Gets a reference to the level that contains this section.
        /// </summary>
        public Level Owner { get; private set; }

		/// <summary>
		/// Gets or sets a collection of all the paths in this section.
		/// </summary>
		internal List<Path> Paths { get; set; }

		/// <summary>
        /// Gets or sets the name of this section, or the empty string if there is no name.
        /// </summary>
        public string Name { get; set; }

		/// <summary>
		/// Gets the lazy QuadTree for this section.
		/// </summary>
		public QuadTree QuadTree { get; private set; }

		/// <summary>
        /// Gets the method the camera uses to scroll across the section.
        /// </summary>
        public CameraScrollType ScrollType { get; internal set; }

        /// <summary>
		/// Gets or sets a collection of all the sprites in this section.
        /// </summary>
		internal List<Sprite> Sprites { get; set; }

        /// <summary>
		/// Gets or sets a value indicating whether the section has loaded from the file.
        /// </summary>
		internal bool IsSectionLoaded { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Section"/> class.
        /// </summary>
        /// <param name="owner">The level that owns this section.</param>
        public Section(Level owner)
        {
            this.Camera = new Camera2D();
            this.Owner = owner;
            this.QuadTree = new QuadTree(GameServices.QuadTreeCellSize);
            this.Layers = new List<Layer>();
            this.tiles = new List<Tile>();
            this.Sprites = new List<Sprite>();
            this.Paths = new List<Path>();
            this.Background = new Background(this);

            GameServices.Camera = this.Camera;
        }

        /// <summary>
        /// Initializes this section.
        /// </summary>
        public void Initialize()
        {
            if (!this.isInitialized)
            {
                this.Background.Initialize();
				this.Layers.ForEach(l => l.Initialize());
				this.Sprites.ForEach(s => s.Initialize(this));
                this.isInitialized = true;
            }
        }

        /// <summary>
        /// Loads the content for this section.
        /// </summary>
        public void LoadContent()
        {
            if (!this.isContentLoaded)
            {
                this.Background.LoadContent();
                this.tiles.ForEach(t => t.LoadContent());
                this.Sprites.ForEach(s => s.LoadContent());
                this.isContentLoaded = true;
            }
        }

        /// <summary>
        /// Updates this section.
        /// </summary>
        public void Update()
        {
			System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

            this.Background.Update();
            this.tiles.ForEach(t => t.Update());
            this.Sprites.ForEach(s => s.Update());
            this.QuadTree.Update();

            // Remove all sprites that have requested to be removed.
            this.RemoveSprites(this.Sprites.Where(s => s.RemoveOnNextFrame).ToList());

            foreach (Sprite sprite in this.Sprites)
            {
                float delta = GameServices.GameTime.GetElapsedSeconds();
                List<Tile> collidableTiles;
                List<Tile> collidingTiles = new List<Tile>();
				List<SlopedTile> collidableSlopes = new List<SlopedTile>();
				bool slopeResolutionOccurred = false;

				// First, we'll take care of horizontal movement. Move the sprite horizontally.
				sprite.Position = new Vector2(sprite.Position.X + (sprite.Velocity.X * delta), sprite.Position.Y);
				this.QuadTree.PlaceSprite(sprite);
				collidableTiles = this.QuadTree.GetCollidableTiles(sprite);

				foreach (Tile tile in collidableTiles)
				{
					if (tile.Intersects(sprite))
					{
						Vector2 resolutionDistance = tile.GetCollisionResolution(sprite);

						// If the resolution is not horizontal...
						if (resolutionDistance.X == 0)
						{
							// ...go to the next tile.
							continue;
						}
						else
						{
							// As a safeguard to avoid resolving horizontally against sides that are in the ground
							// and thus unreachable, we'll check if there's a tile next to that side and ignore it
							// if there is one.
							Vector2 checkPoint = (resolutionDistance.X < 0) ? tile.Hitbox.Bounds.LeftCenter : tile.Hitbox.Bounds.RightCenter;
							checkPoint.X += (resolutionDistance.X < 0) ? -1f : 1f;		// cast it out one pixel in the side's direction
							Tile adjacentTile = this.GetTileAtPositionByBounds(checkPoint, adjacentPointsAreWithin: true);

							// TODO: add a check that checks if the adjacent edges are actually solid

							if (this.ResolveHorizontalCollision(sprite, tile) && adjacentTile == null)
							{
								// Resolve the collision.
								sprite.Position = new Vector2(sprite.Position.X + resolutionDistance.X, sprite.Position.Y);
								sprite.Velocity = new Vector2(0f, sprite.Velocity.Y);
								collidingTiles.Add(tile);
							}
						}
					}
				}

                // Next,  we'll handle vertical collision. Move the sprite vertically.
                sprite.Position = new Vector2(sprite.Position.X, sprite.Position.Y + (sprite.Velocity.Y * delta));
                this.QuadTree.PlaceSprite(sprite);

                // Get all collidable (nearby) tiles and sloped tiles.
				this.QuadTree.GetCollidableTiles(sprite, out collidableTiles, out collidableSlopes);

				// Handle collisions with slopes first.
				foreach (SlopedTile slope in collidableSlopes)
				{
					// If the sprite intersects the slope...
					if (slope.Intersects(sprite))
					{
						// Find the collision resolution distance.
						Vector2 collisionResolution = slope.GetCollisionResolution(sprite);
						if (collisionResolution.Y > 0f)
						{
							// In the case of a sprite walking onto a sloped tile from a normal tile,
							// the bottom center point may not properly reach the slope line and instead
							// be under the tile, which will cause a downward resolution and the sprite will
							// fall through the ground. In the event of a downward resolution, we're going to check
							// the center point of the sprite's hitbox and see if it's above the slope line; if so,
							// we'll move the bottom-center point of the sprite upward onto the slope.

							RightTriangle hitTriangle = (RightTriangle)slope.Hitbox;
							float slopeLineY = hitTriangle.GetPointOnLine(sprite.Hitbox.Center.X).Y;
							if (hitTriangle.SlopedSides == RtSlopedSides.TopLeft || hitTriangle.SlopedSides == RtSlopedSides.TopRight)
							{
								if (sprite.Hitbox.TopCenter.Y <= slopeLineY) // top-center above the slope
								{
									sprite.Position = new Vector2(sprite.Position.X, slopeLineY - sprite.Hitbox.Height);
								}
							}
						}
						else if (collisionResolution.Y != 0f)
						{
							// If it's a vertical collision, handle it, and set the proper flag if the resolution was upwards and the sprite collided with the slope and not one of the flat lines.
							sprite.Position = new Vector2(sprite.Position.X, sprite.Position.Y + collisionResolution.Y);
							slopeResolutionOccurred = collisionResolution.Y < 0f && (sprite.Hitbox.BottomCenter.Y > ((RightTriangle)slope.Hitbox).GetPointOnLine(sprite.Hitbox.BottomCenter.X).Y);
							collidingTiles.Add(slope);
						}
					}
				}

				// Finally, handle collisions with rectangular tiles.
				foreach (Tile tile in collidableTiles)
				{
					// If this tile intersects the sprite...
					if (tile.Intersects(sprite))
					{
						// Get the collision resolution and check if it's either downward, or if it's upward and no slope resolutions have occured.
						Vector2 collisionResolution = tile.GetCollisionResolution(sprite);
						if (collisionResolution.Y > 0f || (collisionResolution.Y < 0f && !slopeResolutionOccurred))
						{
							Vector2 checkpoint = tile.Hitbox.Bounds.TopCenter;
							checkpoint.Y -= 1;
							Tile tileAbove = this.GetTileAtPositionByBounds(checkpoint, adjacentPointsAreWithin: true);
							if (tileAbove != null && tileAbove is SlopedTile)
							{
								SlopedTile slope = tileAbove as SlopedTile;
								RightTriangle hitTriangle = tileAbove.Hitbox as RightTriangle;
								float slopeLineY = hitTriangle.GetPointOnLine(sprite.Hitbox.Center.X).Y;
								if (hitTriangle.SlopedSides == RtSlopedSides.TopLeft || hitTriangle.SlopedSides == RtSlopedSides.TopRight)
								{
									sprite.Position = new Vector2(sprite.Position.X, slopeLineY - sprite.Hitbox.Height);
								}
							}

							// If so, resolve the collision.
							sprite.Position = new Vector2(sprite.Position.X, sprite.Position.Y + collisionResolution.Y);
							collidingTiles.Add(tile);
						}
					}
				}

                // Temporary sprite collision handler
                var collidableSprites = this.QuadTree.GetCollidableSprites(sprite);
                var collidingSprites = collidableSprites.Where(s => s != sprite && sprite.Hitbox.Intersects(s.Hitbox));
                foreach (Sprite s in collidingSprites)
                {
                    sprite.HandleSpriteCollision(s, sprite.Hitbox.GetIntersectionDepth(s.Hitbox));
                    s.HandleSpriteCollision(sprite, s.Hitbox.GetIntersectionDepth(sprite.Hitbox));
                }
			}

			stopwatch.Stop();
			this.debugText = stopwatch.Elapsed.ToString();

            this.TempUpdate();
        }

        /// <summary>
        /// Temporary code for the update routine.
        /// </summary>
        private void TempUpdate()
        {
            // Update the camera's position (temp).
            Sprite player = this.Sprites.Where(s => s.GetType().Name.Contains("SimplePlayer")).FirstOrDefault();
            if (player != null)
            {
                float cameraX = MathHelper.Clamp(player.Hitbox.Center.X - 400f, this.Bounds.X, this.Bounds.Width);
                float cameraY = MathHelper.Clamp(player.Hitbox.Center.Y - 240f, this.Bounds.Y, this.Bounds.Height);
                Vector2 offset = new Vector2(cameraX - this.Camera.Position.X, cameraY - this.Camera.Position.Y);
                this.MoveCamera(offset);
            }

            if (Input.InputManager.IsCurrentKeyPress(Microsoft.Xna.Framework.Input.Keys.K))
            {
                this.Camera.Zoom += 0.01f;
            }
            else if (Input.InputManager.IsCurrentKeyPress(Microsoft.Xna.Framework.Input.Keys.M))
            {
                this.Camera.Zoom -= 0.01f;
            }
            else if (Input.InputManager.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.W))
            {
                string file = this.Owner.Serialize();
                System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + @"\level.txt", file);
            }
			else if (Input.InputManager.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.N))
			{
				string file = new IO.LevelSerializers.Serializer001().Serialize(this.Owner);
				System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + @"\level_new.txt", file);
			}
			else if (Input.InputManager.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.M))
			{
				string file = new IO.LevelSerializers.Serializer002().Serialize(this.Owner);
				System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + @"\level_serializer002.txt", file);
			}
			else if (Input.InputManager.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.J))
			{
				IO.LevelSerializers.Serializer002Types.LayerTileSaveData tileSaves = new IO.LevelSerializers.Serializer002Types.LayerTileSaveData(this.MainLayer);
			}
			else if (Input.InputManager.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.OemTilde))
			{
				if (!GameServices.DebugForm.Visible)
				{
					GameServices.DebugForm.Show();
				}
				else
				{
					GameServices.DebugForm.Hide();
				}
			}
			else if (Input.InputManager.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.I))
			{
				Debug.Logger.LogInfo("I key pressed.");
			}
        }

        /// <summary>
        /// Draws this section to the screen.
        /// </summary>
        public void Draw()
        {
            this.Background.Draw();

            this.tiles.ForEach(t => t.Draw());

            this.Sprites.ForEach(s => s.Draw());

			GameServices.DrawStringDefault(this.debugText);

            Sprite player = this.Sprites.Where(s => s.GetType().Name.EndsWith("SimplePlayer")).FirstOrDefault();
            BoundingRectangle drawRect = new BoundingRectangle(player.Hitbox.Left, player.Hitbox.Center.Y, 8f, 8f);
            Rectangle checkRect = new Rectangle((int)player.Hitbox.Center.X, (int)(player.Hitbox.Bottom + 3f), 1, 1);
            Tile tile = this.GetTileAtPosition(new Vector2(checkRect.X, checkRect.Y), true);
            GameServices.SpriteBatch.DrawRectangle(drawRect.ToRectangle(), Color.Red);
            GameServices.SpriteBatch.DrawRectangle(checkRect, Color.White);
            if (tile != null)
            {
                GameServices.SpriteBatch.DrawRectangleEdges(tile.Hitbox.Bounds.ToRectangle(), Color.Wheat);
            }

			Vector2 mousePos = new Vector2(Input.InputManager.CurrentMouseState.X, Input.InputManager.CurrentMouseState.Y);
			Tile mouseTile = this.GetTileAtPositionByBounds(mousePos, true);
        }

        /// <summary>
        /// Adds a tile to this section.
        /// </summary>
        /// <param name="tile">The tile to add.</param>
        public void AddTile(Tile tile)
        {
            if (tile == null)
            {
                throw new ArgumentNullException("Section.AddTile(Tile): Tile cannot be null.");
            }

            this.tiles.Add(tile);
            this.MainLayer.AddTile(tile);
            this.QuadTree.Add(tile);
        }

        /// <summary>
        /// Removes a tile from this level.
        /// </summary>
        /// <param name="tile">The tile to remove.</param>
        public void RemoveTile(Tile tile)
        {
            if (this.tiles.Contains(tile))
            {
                this.tiles.Remove(tile);
                this.QuadTree.Remove(tile);
                this.Layers.ForEach(l => l.RemoveTile(tile));
            }
        }

        /// <summary>
        /// Removes a sprite from this section.
        /// </summary>
        /// <param name="sprite">The sprite to remove.</param>
        public void RemoveSprite(Sprite sprite)
        {
            this.Sprites.Remove(sprite);
            this.QuadTree.Remove(sprite);
        }

        /// <summary>
        /// Removes a list of sprites from the section at once.
        /// </summary>
        /// <param name="spritesToRemove">A list of the sprites to remove.</param>
        /// <remarks>This method will not throw an exception if any of the sprites to remove are not present in this section.</remarks>
        public void RemoveSprites(List<Sprite> spritesToRemove)
        {
            foreach (Sprite sprite in spritesToRemove)
            {
                this.Sprites.Remove(sprite);
                this.QuadTree.Remove(sprite);
            }
        }

		/// <summary>
        /// Gets the tile at the given position.
        /// </summary>
        /// <param name="position">The position from which to get the tile.</param>
        /// <param name="adjacentPointsAreWithin">If true, the method will return a tile if the point is on the edge of the tile.</param>
        /// <returns>The tile at the position, or null if there is no tile there.</returns>
        public Tile GetTileAtPosition(Vector2 position, bool adjacentPointsAreWithin)
        {
            var tiles = this.QuadTree.GetTilesInCell(this.QuadTree.GetCellNumberAtPosition(position));

            if (tiles == null)
            {
                return null;
            }

            foreach (Tile tile in tiles)
            {
                if (tile.Hitbox.Within(position, adjacentPointsAreWithin))
                {
                    return tile;
                }
            }

            return null;
        }

		/// <summary>
		/// Gets a tile at a given position by the tile's bounds.
		/// </summary>
		/// <param name="position">The position for which to get the tile.</param>
		/// <param name="adjacentPointsAreWithin">A field that indicates whether positions that lie directly on the tile's edges count as within the tile.</param>
		/// <returns>The first tile at the position, or null if there is no tile.</returns>
        public Tile GetTileAtPositionByBounds(Vector2 position, bool adjacentPointsAreWithin)
        {
            var tiles = this.QuadTree.GetTilesInCell(this.QuadTree.GetCellNumberAtPosition(position));

            if (tiles == null)
            {
                return null;
            }

            foreach (Tile tile in tiles)
            {
                if (tile.Hitbox.Bounds.Within(position, adjacentPointsAreWithin))
                {
                    return tile;
                }
            }

            return null;
        }

		/// <summary>
		/// Moves the camera by a given distance.
		/// </summary>
		/// <param name="offset">The distance to move the camera by.</param>
		public void MoveCamera(Vector2 offset)
		{
			if (!this.IsSectionLoaded)
			{
				throw new InvalidOperationException("Section.MoveCamera(Vector2): The section isn't loaded - the section needs to be loaded before anything can happen.");
			}

			// Ensure the camera falls within bounds.
			Vector2 newPosition = this.Camera.Position + offset;

			if (newPosition.X < 0f)
			{
				newPosition.X = 0f;
			}

			if (newPosition.Y < 0f)
			{
				newPosition.Y = 0f;
			}

			Vector2 viewportBottomRight = new Vector2(newPosition.X + this.Camera.ViewportSize.X, newPosition.Y + this.Camera.ViewportSize.Y);
			if (viewportBottomRight.X > this.Bounds.Right)
			{
				newPosition.X -= viewportBottomRight.X - this.Bounds.Right;
			}

			if (viewportBottomRight.Y > this.Bounds.Bottom)
			{
				newPosition.Y -= viewportBottomRight.Y - this.Bounds.Bottom;
			}

			this.Camera.Position = newPosition;
		}

        /// <summary>
        /// Determines if a sprite should follow the terrain or resolve vertical collisions.
        /// </summary>
        /// <param name="sprite">The sprite to check.</param>
        /// <param name="newYCoordinate">The Y position to move the sprite to.</param>
        /// <returns>True if the sprite doesn't need to follow terrain, false if otherwise.</returns>
        private bool ResolveVerticalCollisions(Sprite sprite, out float newYCoordinate)
        {
            // Take a square tile with a slope top-right edged slope
            // to its right. We'd like sprites to start walking down
            // the slope when the bottom-center point of the sprite
            // goes over the slope, but due to the square tile, the
            // sprite doesn't follow the slope until the left edge
            // of the sprite is past the tile. This method checks
            // if there's a tile under the bottom center point of
            // the sprite, and if there is, returns the Y-coordinate
            // to place the sprite at. If there is no tile under the
            // bottom center point, the code then checks the bottom
            // left and bottom right points.

            // TODO: implement
            newYCoordinate = float.NaN;
            return false;
        }

        /// <summary>
        /// Determines if a sprite ascending a slope should collide with a given tile.
        /// </summary>
        /// <param name="sprite">The sprite to check.</param>
        /// <param name="tile">The tile to check.</param>
        /// <returns>True if the sprite should collide with the tile, false if otherwise.</returns>
        private bool ResolveHorizontalCollision(Sprite sprite, Tile tile)
        {
            // Sprites collide with slopes at their bottom-center point,
            // otherwise they look as if they're floating above the tile.
            // However, adjacent square tiles will still be collided with
            // because part of the sprite's hitbox is within the slope.
            // This method checks if a sprite should collide with a given
            // tile by checking if the top of the tile is below the slope.
            if (sprite == null || tile == null)
            {
                throw new ArgumentNullException(string.Format("Section.ResolveHorizontalCollision(Sprite, Tile): The {0} argument is null.", (sprite == null) ? "sprite" : "tile"));
            }

            if (!sprite.IsOnSlope)
            {
                return true;
            }

            // If the sprite is moving left, we want to check the right edge of the tile; else we want to check the left edge.
            float tileEdgeToCheck = (sprite.Direction == SpriteDirection.Left) ? tile.Hitbox.Bounds.Right : tile.Hitbox.Bounds.Left;
            float slopeIntersectPoint = ((RightTriangle)sprite.RestingSlope.Hitbox).GetPointOnLine(tileEdgeToCheck).Y;

            // If the tile's top is above the slope...
            if (tile.Hitbox.Bounds.Top < slopeIntersectPoint)
            {
                return true;
            }

            return false;
        }

		/// <summary>
        /// Sets a given layer as the main layer.
        /// </summary>
        /// <param name="layer">The layer to set.</param>
        public void SetMainLayer(Layer layer)
        {
            if (this.MainLayer != null)
            {
                throw new InvalidOperationException("Section.SetMainLayer(Layer): A layer tried to set itself as this section's main layer, but this section already has a main layer.");
            }

            this.MainLayer = layer;
        }

        /// <summary>
        /// Gets an anonymous object containing key objects of this section.
        /// </summary>
        /// <returns>An anonymous object.</returns>
		[Obsolete]
        public object GetSerializableObjects()
        {
            List<object> layerObjects = new List<object>(this.Layers.Count);
            List<object> spriteObjects = new List<object>(this.Sprites.Count);
            List<object> pathObjects = new List<object>(this.Paths.Count);
            this.Layers.ForEach(l => layerObjects.Add(l.GetSerializableObjects()));
            this.Sprites.ForEach(s => spriteObjects.Add(s.GetSerializableObjects()));
            this.Paths.ForEach(p => pathObjects.Add(p.GetSerializableObjects()));

            return new
            {
                index = this.Index,
                name = this.Name,
                bounds = this.Bounds.Serialize(),
                scrollType = (int)this.ScrollType,
                autoscrollSpeed = (this.ScrollType == CameraScrollType.AutoScroll) ? this.autoscrollSpeed.Serialize() : new Vector2(float.NaN).Serialize(),
                autoscrollPathName = (this.ScrollType == CameraScrollType.AutoScrollAlongPath) ? this.autoscrollPathName : null,
                background = this.Background.GetSerializableObjects(),
                layers = layerObjects,
                sprites = spriteObjects,
                paths = pathObjects
            };
        }

        /// <summary>
        /// Returns a JSON string containing key objects of this section.
        /// </summary>
        /// <returns>A valid JSON string.</returns>
		[Obsolete]
        public string Serialize()
        {
            return JObject.FromObject(this.GetSerializableObjects()).ToString();
        }

        /// <summary>
        /// Loads a section from a JSON string containing a section.
        /// </summary>
        /// <param name="json">A valid JSON string.</param>
		[Obsolete]
        public void Deserialize(string json)
        {
            if (!this.IsSectionLoaded)
            {
                JObject obj = JObject.Parse(json);

                // First, deserialize the root objects.
                this.Index = (int)obj["index"];
                this.Name = (string)obj["name"];
                this.Bounds = BoundingRectangle.FromSimpleString((string)obj["bounds"]);
                this.ScrollType = (CameraScrollType)(int)obj["scrollType"];
                this.autoscrollSpeed = obj["autoscrollSpeed"].ToVector2();
                this.autoscrollPathName = (string)obj["autoscrollPathName"];
                this.Background.Deserialize(obj["background"].ToString());

                // Next, deserialize the nested objects.
                JArray layersData = (JArray)obj["layers"];
                JArray spritesData = (JArray)obj["sprites"];
                JArray pathsData = (JArray)obj["paths"];

                foreach (var layerData in layersData)
                {
                    Layer layer = new Layer(this);
                    layer.Deserialize(layerData.ToString());
                    layer.Initialize();
                    this.Layers.Add(layer);
                }

                foreach (var spriteData in spritesData)
                {
                    string typeName = (string)spriteData["typeName"];
                    Sprite sprite = AssemblyManager.GetSpriteByFullName(typeName);
                    sprite.Deserialize(spriteData.ToString());
                    sprite.Initialize(this);
                    this.Sprites.Add(sprite);
                    this.QuadTree.Add(sprite);
                }

                foreach (var pathData in pathsData)
                {
                    Path path = new Path(null);
                    path.Deserialize(pathData.ToString());
                    this.Paths.Add(path);
                }

                this.IsSectionLoaded = true;
            }
        }
    }
}