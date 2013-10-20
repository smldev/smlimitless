//-----------------------------------------------------------------------
// <copyright file="Layer.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;
using SMLimitless.Physics;

namespace SMLimitless.Sprites.Collections
{
    /// <summary>
    /// A collection of sprites and tiles that form levels.
    /// </summary>
    public sealed class Layer
    {
        /// <summary>
        /// A value indicating whether this is the main layer for the level.
        /// </summary>
        private bool isMainLayer;

        /// <summary>
        /// A value indicating whether this layer is actively drawn and updated.
        /// </summary>
        private bool isActive;

        /// <summary>
        /// The level that created this layer.
        /// </summary>
        private Level owner;

        /// <summary>
        /// The collection of tiles in this layer.
        /// </summary>
        private List<Tile> tiles;

        /// <summary>
        /// The collection of sprites in this layer.
        /// </summary>
        private List<Sprite> sprites;

        /// <summary>
        /// The velocity of the contents of the layer.
        /// </summary>
        private Vector2 velocity;

        /// <summary>
        /// The position of the layer's anchor point.
        /// </summary>
        private LayerAnchorPosition anchorPosition;
        
        /// <summary>
        /// The backing field for the AnchorPoint property.
        /// </summary>
        private Vector2 anchorPointBackingField;

        /// <summary>
        /// Gets or sets the anchor point of the layer.
        /// The anchor point is used to move the layer.
        /// </summary>
        private Vector2 AnchorPoint
        {
            get
            {
                return this.anchorPointBackingField;
            }

            set
            {
                if (!this.isMainLayer)
                {
                    this.anchorPointBackingField = value;
                }            
            }
        }

        /// <summary>
        /// A backing field for the Bounds property.
        /// </summary>
        private BoundingRectangle boundsBackingField;

        /// <summary>
        /// Gets or sets the bounds of this layer.
        /// </summary>
        public BoundingRectangle Bounds
        {
            get
            {
                return this.boundsBackingField;
            }

            set
            {
                this.boundsBackingField = value;
                if (!this.isMainLayer)
                {
                    this.SetAnchorPoint();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of this layer.
        /// </summary>
        [DefaultValue(""), Description("The name of this layer to be used in event scripting. This field is optional.")]
        public string Name { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Layer"/> class.
        /// </summary>
        /// <param name="owner">The level that is creating this layer.</param>
        /// <param name="position">The position of the anchor point in relation to the layer.</param>
        public Layer(Level owner, LayerAnchorPosition position = LayerAnchorPosition.TopLeft)
        {
            this.owner = owner;
            this.anchorPosition = position;
            this.AnchorPoint = new Vector2(float.NaN);
            this.velocity = Vector2.Zero;
            this.tiles = new List<Tile>();
            this.sprites = new List<Sprite>();
            this.Bounds = new BoundingRectangle(new Vector2(float.NaN), new Vector2(float.NaN));
        }

        /// <summary>
        /// Sets this layer as the main layer for a level.
        /// The main layer spans the entire level and has fixed bounds.
        /// Main layers cannot be moved or translated, and levels may only have one main layer.
        /// </summary>
        /// <param name="width">The width of the layer.</param>
        /// <param name="height">The height of the layer.</param>
        internal void SetMainLayer(float width, float height)
        {
            if (this.owner.MainLayer != null)
            {
                throw new Exception("Layer.SetMainLayer(float, float): This level already has a main layer.");
            }

            this.isMainLayer = true;
            this.Bounds = new BoundingRectangle(0, 0, width, height);
            this.AnchorPoint = new Vector2(float.NaN);
        }

        /// <summary>
        /// Adds a tile to this layer.
        /// This also adjusts the bounds according to the tile's position.
        /// </summary>
        /// <param name="tile">The tile to add.</param>
        public void AddTile(Tile tile)
        {
            this.tiles.Add(tile);

            if (!this.isMainLayer)
            {
                if (this.Bounds.IsNaN())
                {
                    // The layer's empty, and has no position.
                    // Let's give it one.
                    this.Bounds = tile.Hitbox.Bounds;
                }
                else
                {
                    this.AdjustBounds(tile);
                }
            }
        }
        
        /// <summary>
        /// Adjusts the bounds of this layer to include a given tile.
        /// </summary>
        /// <param name="tile">The tile to expand the bounds around.</param>
        private void AdjustBounds(Tile tile)
        {
            float tileLeft = tile.Hitbox.Bounds.Left;
            float tileRight = tile.Hitbox.Bounds.Right;
            float tileTop = tile.Hitbox.Bounds.Top;
            float tileBottom = tile.Hitbox.Bounds.Bottom;

            if (tileLeft < this.Bounds.Left)
            {
                this.Bounds = new BoundingRectangle(tileLeft, this.Bounds.Y, this.Bounds.Right - tileLeft, this.Bounds.Height);
            }
            else if (tileRight > this.Bounds.Right)
            {
                this.Bounds = new BoundingRectangle(this.Bounds.X, this.Bounds.Y, tileRight - this.Bounds.Left, this.Bounds.Height);
            }

            if (tileTop < this.Bounds.Top)
            {
                this.Bounds = new BoundingRectangle(this.Bounds.X, tileTop, this.Bounds.Width, this.Bounds.Bottom - tileTop);
            }
            else if (tileBottom > this.Bounds.Bottom)
            {
                this.Bounds = new BoundingRectangle(this.Bounds.X, this.Bounds.Y, this.Bounds.Width, tileBottom - this.Bounds.Top);
            }
        }

        /// <summary>
        /// Removes a tile from this level.
        /// This also rebuilds the bounds of this layer, which may be expensive if there are many tiles in a non-main layer.
        /// </summary>
        /// <param name="tile">The tile to remove.</param>
        public void RemoveTile(Tile tile)
        {
            if (!this.tiles.Contains(tile))
            {
                throw new Exception("Layer.RemoveTile(tile): Tried to remove a tile that wasn't in the layer.");
            }

            this.tiles.Remove(tile);

            if (!this.isMainLayer)
            {
                if (this.tiles.Count == 0)
                {
                    this.Bounds = new BoundingRectangle(new Vector2(float.NaN), new Vector2(float.NaN));
                }
                else
                {
                    this.Bounds = this.tiles[0].Hitbox.Bounds;
                    foreach (Tile t in this.tiles)
                    {
                        this.AdjustBounds(t);
                    }
                }
            }
        }

        /// <summary>
        /// Immediately moves the layer and all its contents to a given position.
        /// </summary>
        /// <param name="position">The position to move the layer to.</param>
        public void Translate(Vector2 position)
        {
            if (!this.isMainLayer)
            {
                Vector2 distance = position - this.AnchorPoint;
                this.TranslateRelative(distance);
            }
            else
            {
                throw new Exception("Layer.Translate(Vector2): Tried to move a main layer. Main layers cannot move.");
            }
        }

        /// <summary>
        /// Immediately moves the layer by a relative distance.
        /// </summary>
        /// <param name="distance">The distance to move the layer by.</param>
        public void TranslateRelative(Vector2 distance)
        {
            if (!this.isMainLayer)
            {
                this.AnchorPoint += distance;
                this.Bounds = new BoundingRectangle(this.Bounds.X + distance.X, this.Bounds.Y + distance.Y, this.Bounds.Width, this.Bounds.Height);

                foreach (Tile tile in this.tiles)
                {
                    tile.Position += distance;
                }

                foreach (Sprite sprite in this.sprites)
                {
                    sprite.Position += distance;
                }
            }
            else
            {
                throw new Exception("Layer.TranslateRelative(Vector2): Tried to move a main layer. Main layers cannot move.");
            }
        }

        /// <summary>
        /// Updates this layer.
        /// </summary>
        public void Update()
        {
            // TODO: change this method to account for active/inactive layers, tiles and sprites

            // Determine if any sprites are no longer within the bounds of the layer.
            this.sprites.RemoveAll(s => !s.Hitbox.Intersects(this.Bounds));

            // Then, determine if any other sprites in the level are within the bounds of the layer.
            //// foreach (Sprite sprite in this.owner.Sprites)
            ////{
            ////    // TODO: This will be changed when levels are changed
            ////    if (sprite.Hitbox.Bounds.Intersects(this.Bounds))
            ////    {
            ////        this.sprites.Add(sprite);
            ////    }
            ////}
        }

        /// <summary>
        /// Draws the bounds of this layer.
        /// </summary>
        /// <param name="color">The color used to draw the bounds.</param>
        public void Draw(Color color)
        {
            this.Bounds.DrawOutline(color);
        }

        /// <summary>
        /// Sets the anchor point.
        /// </summary>
        private void SetAnchorPoint()
        {
            if (this.Bounds.IsNaN())
            {
                this.AnchorPoint = new Vector2(float.NaN);
            }

            switch (this.anchorPosition)
            {
                case LayerAnchorPosition.Invalid:
                    throw new Exception("Layer.SetAnchorPoint: Invalid anchor position.");
                case LayerAnchorPosition.TopLeft:
                    this.AnchorPoint = new Vector2(this.Bounds.Left, this.Bounds.Top);
                    break;
                case LayerAnchorPosition.TopRight:
                    this.AnchorPoint = new Vector2(this.Bounds.Right, this.Bounds.Top);
                    break;
                case LayerAnchorPosition.BottomLeft:
                    this.AnchorPoint = new Vector2(this.Bounds.Left, this.Bounds.Bottom);
                    break;
                case LayerAnchorPosition.BottomRight:
                    this.AnchorPoint = new Vector2(this.Bounds.Right, this.Bounds.Bottom);
                    break;
                case LayerAnchorPosition.TopCenter:
                    this.AnchorPoint = this.Bounds.TopCenter;
                    break;
                case LayerAnchorPosition.BottomCenter:
                    this.AnchorPoint = this.Bounds.BottomCenter;
                    break;
                case LayerAnchorPosition.LeftCenter:
                    this.AnchorPoint = this.Bounds.LeftCenter;
                    break;
                case LayerAnchorPosition.RightCenter:
                    this.AnchorPoint = this.Bounds.RightCenter;
                    break;
                case LayerAnchorPosition.Center:
                    this.AnchorPoint = this.Bounds.Center;
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Lists all of the positions that a layer's anchor point may be.
    /// </summary>
    public enum LayerAnchorPosition
    {
        /// <summary>
        /// Invalid position.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// The top-left corner.
        /// </summary>
        TopLeft,

        /// <summary>
        /// The top-right corner.
        /// </summary>
        TopRight,

        /// <summary>
        /// The bottom-left corner.
        /// </summary>
        BottomLeft,

        /// <summary>
        /// The bottom-right corner.
        /// </summary>
        BottomRight,

        /// <summary>
        /// The top-center point.
        /// </summary>
        TopCenter,

        /// <summary>
        /// The bottom-center point.
        /// </summary>
        BottomCenter,

        /// <summary>
        /// The left-center point.
        /// </summary>
        LeftCenter,

        /// <summary>
        /// The right-center point.
        /// </summary>
        RightCenter,

        /// <summary>
        /// The center point.
        /// </summary>
        Center
    }
}
