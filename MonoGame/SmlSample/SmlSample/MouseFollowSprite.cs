//-----------------------------------------------------------------------
// <copyright file="MouseFollowSprite.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SMLimitless;
using SMLimitless.Extensions;
using SMLimitless.Input;
using SMLimitless.Physics;
using SMLimitless.Sprites;


namespace SmlSample
{
    /// <summary>
    /// A testing sprite.
    /// </summary>
    public class MouseFollowSprite : Sprite
    {
        /// <summary>
        /// Gets the name of the category that this sprite is
        /// categorized within in the level editor.
        /// </summary>
        public override string EditorCategory
        {
            get { return "Testing"; }
        }

        public MouseFollowSprite()
        {
            this.Size = new Vector2(16f, 16f);
        }

		public override bool IsCompelledToMove
		{
			get { return false; }
		}
        
        /// <summary>
        /// Updates this sprite.
        /// </summary>
        public override void Update()
        {
            float distance = 2f;
            if (InputManager.IsCurrentKeyPress(Keys.Up))
            {
                this.Position = new Vector2(Position.X, Position.Y - distance);
            }

            if (InputManager.IsCurrentKeyPress(Keys.Down))
            {
                this.Position = new Vector2(Position.X, Position.Y + distance);
            }

            if (InputManager.IsCurrentKeyPress(Keys.Left))
            {
                this.Position = new Vector2(Position.X - 1.0f, Position.Y);
            }

            if (InputManager.IsCurrentKeyPress(Keys.Right))
            {
                this.Position = new Vector2(Position.X + 1.0f, Position.Y);
            }

            if (InputManager.IsNewActionPress(InputAction.Jump))
            {
                this.Position = new Vector2(Position.X, Position.Y + -20f);
            }
        }

        /// <summary>
        /// Draws this sprite.
        /// </summary>
        public override void Draw()
        {
            GameServices.SpriteBatch.DrawRectangle(Hitbox.ToRectangle(), Color.Red);
        }

        /// <summary>
        /// Handles a collision between this sprite and another.
        /// </summary>
        /// <param name="sprite">The sprite that has collided with this one.</param>
        /// <param name="intersect">The depth of the intersection.</param>
        public override void HandleSpriteCollision(Sprite sprite, Vector2 intersect)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles a collision between this sprite and a tile.
        /// </summary>
        /// <param name="tile">The tile that this sprite has collided with.</param>
        /// <param name="intersect">The depth of the intersection.</param>
        public override void HandleTileCollision(Tile tile, Vector2 intersect)
        {
        }

        /// <summary>
        /// Loads the content for this sprite.
        /// </summary>
        public override void LoadContent()
        {
        }

        public override void DeserializeCustomObjects(SMLimitless.Sprites.Assemblies.JsonHelper customObjects)
        {
        }

        public override object GetCustomSerializableObjects()
        {
            return null;
        }
    }
}
