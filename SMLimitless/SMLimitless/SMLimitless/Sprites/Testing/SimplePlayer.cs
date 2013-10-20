//-----------------------------------------------------------------------
// <copyright file="SimplePlayer.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Input;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites.Testing
{
    /// <summary>
    /// A testing sprite that the user can control
    /// with Left, Right, and Jump.
    /// </summary>
    public class SimplePlayer : Sprite
    {
        /// <summary>
        /// Gets the name of the category that this sprite is
        /// categorized within in the level editor.
        /// </summary>
        public override string EditorCategory
        {
            get { return "Testing"; }
        }

        /// <summary>
        /// The graphics for this sprite.
        /// </summary>
        private StaticGraphicsObject graphics;

        /// <summary>
        /// The maximum number of frames that a jump impulse will be applied for.
        /// </summary>
        private int jumpTimeout = 5;

        /// <summary>
        /// Initializes this sprite.
        /// </summary>
        /// <param name="owner">The level that owns this sprite.</param>
        public void Initialize(TestLevel owner)
        {
            this.Size = new Vector2(16f, 16f);
            this.IsActive = true;
        }

        /// <summary>
        /// Loads the content for this sprite.
        /// </summary>
        public override void LoadContent()
        {
            this.graphics = (StaticGraphicsObject)SMLimitless.Content.ContentPackageManager.GetGraphicsResource("simple_player");
            this.graphics.LoadContent();
        }

        /// <summary>
        /// Updates this sprite.
        /// </summary>
        public override void Update()
        {
            const float Friction = 0.08f;
            const float AccelerationImpulse = 5f;
            const float JumpImpulse = 50f;
            const float MaxJumpVelocity = -150f;
            const int MaxJumpTimeout = 25;

            bool isLeftDown = InputManager.IsCurrentActionPress(InputAction.Left);
            bool isRightDown = InputManager.IsCurrentActionPress(InputAction.Right);

            if (this.Velocity.X != 0f && (!isLeftDown && !isRightDown))
            {
                if (this.Velocity.X > 0.5f)
                {
                    this.Velocity = new Vector2((float)(this.Velocity.X - (this.Velocity.X * Friction)), this.Velocity.Y);
                }
                else
                {
                    this.Velocity = new Vector2(0f, this.Velocity.Y);
                }
            }

            if (isLeftDown)
            {
                this.Direction = SpriteDirection.Left;
                this.AdjustVelocity(-AccelerationImpulse);
            }
            else if (isRightDown)
            {
                this.Direction = SpriteDirection.Right;
                this.AdjustVelocity(AccelerationImpulse);
            }

            if (InputManager.IsCurrentActionPress(InputAction.Jump) && this.jumpTimeout > 0)
            {
                this.Velocity = new Vector2(this.Velocity.X, (this.Velocity.Y <= MaxJumpVelocity) ? MaxJumpVelocity : this.Velocity.Y - JumpImpulse);
                this.jumpTimeout--;
            }

            if (this.IsOnGround && this.jumpTimeout == 0)
            {
                this.jumpTimeout = MaxJumpTimeout;
            }

            base.Update();
        }

        /// <summary>
        /// Draws this sprite.
        /// </summary>
        public override void Draw()
        {
            SpriteEffects effects = (this.Direction == SpriteDirection.Right) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            this.graphics.Draw(new Vector2(this.Position.X, this.Position.Y - 16f), Color.White, effects);
            this.Velocity.ToString().DrawStringDefault();
        }

        /// <summary>
        /// Handles a tile collision.
        /// </summary>
        /// <param name="tile">The colliding tile.</param>
        /// <param name="intersect">The depth of the collision.</param>
        public override void HandleTileCollision(Tile tile, Vector2 intersect)
        {
        }

        /// <summary>
        /// Handles a sprite collision.
        /// </summary>
        /// <param name="sprite">The colliding sprite.</param>
        /// <param name="intersect">The depth of the collision.</param>
        public override void HandleSpriteCollision(Sprite sprite, Vector2 intersect)
        {
        }

        /// <summary>
        /// Adjusts the velocity of this sprite.
        /// </summary>
        /// <param name="amount">The amount to adjust it by.</param>
        private void AdjustVelocity(float amount)
        {
            const float MaxVelocity = 150f;
            if (Math.Abs(this.Velocity.X + amount) > MaxVelocity)
            {
                this.Velocity = new Vector2((this.Velocity.X >= 0) ? MaxVelocity : -MaxVelocity, this.Velocity.Y);
            }
            else
            {
                this.Velocity = new Vector2(this.Velocity.X + amount, this.Velocity.Y);
            }
        }
    }
}
