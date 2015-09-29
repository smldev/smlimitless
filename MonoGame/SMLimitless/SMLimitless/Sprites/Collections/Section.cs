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
    public sealed class Section
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

			stopwatch.Stop();

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
            }
        }

        /// <summary>
        /// Removes a sprite from this section.
        /// </summary>
        /// <param name="sprite">The sprite to remove.</param>
        public void RemoveSprite(Sprite sprite)
        {
            this.Sprites.Remove(sprite);
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
			throw new NotImplementedException();
        }

		/// <summary>
		/// Gets a tile at a given position by the tile's bounds.
		/// </summary>
		/// <param name="position">The position for which to get the tile.</param>
		/// <param name="adjacentPointsAreWithin">A field that indicates whether positions that lie directly on the tile's edges count as within the tile.</param>
		/// <returns>The first tile at the position, or null if there is no tile.</returns>
        public Tile GetTileAtPositionByBounds(Vector2 position, bool adjacentPointsAreWithin)
        {
           throw new NotImplementedException();
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
    }
}