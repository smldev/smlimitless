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
    public sealed class Layer
    {
        private bool isMainLayer;
        private bool isActive;
        private Level owner;
        private List<Tile> tiles;
        private List<Sprite> sprites;
        private Vector2 velocity;

        private LayerAnchorPosition anchorPosition;
        private Vector2 anchorPointBackingField;
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

        private BoundingRectangle boundsBackingField;
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

        [DefaultValue(""), Description("The name of this layer to be used in event scripting. This field is optional.")]
        public string Name { get; set; }

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

        internal void SetMainLayer(float width, float height)
        {
            this.isMainLayer = true;
            this.Bounds = new BoundingRectangle(0, 0, width, height);
            this.AnchorPoint = new Vector2(float.NaN);
        }

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

        public void Draw(Color color)
        {
            this.Bounds.DrawOutline(color);
        }

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

    public enum LayerAnchorPosition
    {
        Invalid = 0,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        TopCenter,
        BottomCenter,
        LeftCenter,
        RightCenter,
        Center
    }
}
