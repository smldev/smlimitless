using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using SMLimitless.Extensions;
using SMLimitless.Screens.Effects;
using SMLimitless.Graphics;
using SMLimitless.Sprites;
using SMLimitless.Physics;

namespace SMLimitless.Screens
{
    public class TestScreen : Screen
    {
        // Testing collision
        private BoundingRectangle a;
        private BoundingRectangle b;
        private BoundingRectangle c;

        public override void Initialize(Screen owner, string parameters)
        {
            a = new BoundingRectangle(0f, 0f, 100f, 100f);
            b = new BoundingRectangle(200f, 200f, 100f, 100f);
            c = new BoundingRectangle(200f, 300f, 100f, 100f);
        }

        public override void LoadContent()
        {
        }

        public override void Update()
        {
            // Move a if the arrow keys are being held down
            float distance = 1f;
            if (InputManager.IsCurrentKeyPress(Keys.Up))
            {
                a = new BoundingRectangle(a.X, a.Y - distance, 100f, 100f);
            }
            else if (InputManager.IsCurrentKeyPress(Keys.Down))
            {
                a = new BoundingRectangle(a.X, a.Y + distance, 100f, 100f);
            }
            else if (InputManager.IsCurrentKeyPress(Keys.Left))
            {
                a = new BoundingRectangle(a.X - distance, a.Y, 100f, 100f);
            }
            else if (InputManager.IsCurrentKeyPress(Keys.Right))
            {
                a = new BoundingRectangle(a.X + distance, a.Y, 100f, 100f);
            }
        }

        public override void Draw()
        {
            GameServices.SpriteBatch.DrawRectangleEdges(a.ToRectangle(), Color.White);
            GameServices.SpriteBatch.DrawRectangleEdges(b.ToRectangle(), Color.White);
            GameServices.SpriteBatch.DrawRectangleEdges(c.ToRectangle(), Color.White);

            BoundingRectangle d = new BoundingRectangle(a.X, a.Y, 100f, 100f);

            Intersection intersect1 = new Intersection(a, b);
            Intersection intersect2 = new Intersection(a, c);
            Vector2 resolution1 = intersect1.GetIntersectionResolution();
            Vector2 resolution2 = intersect2.GetIntersectionResolution();

            if (intersect1.Direction == Direction.Up || intersect2.Direction == Direction.Up)
            {
                if (Math.Abs(resolution1.Y) > Math.Abs(resolution2.Y)) d = new BoundingRectangle(a.X + resolution1.X, a.Y + resolution1.Y, 100f, 100f);
                else if (Math.Abs(resolution2.Y) > Math.Abs(resolution1.Y)) d = new BoundingRectangle(a.X + resolution2.X, a.Y + resolution2.Y, 100f, 100f);
                else d = new BoundingRectangle(a.X + resolution1.X, a.Y + resolution2.Y, 100f, 100f);
            }
            else if (intersect1.Direction == Direction.Down || intersect2.Direction == Direction.Down)
            {
                if (Math.Abs(resolution1.Y) > Math.Abs(resolution2.Y)) d = new BoundingRectangle(a.X + resolution1.X, a.Y + resolution1.Y, 100f, 100f);
                else if (Math.Abs(resolution2.Y) > Math.Abs(resolution1.Y)) d = new BoundingRectangle(a.X + resolution2.X, a.Y + resolution2.Y, 100f, 100f);
                else d = new BoundingRectangle(a.X + resolution1.X, a.Y + resolution2.Y, 100f, 100f);
            }
            else if (intersect1.Direction == Direction.Left || intersect2.Direction == Direction.Right)
            {
                if (Math.Abs(resolution1.X) > Math.Abs(resolution2.X)) d = new BoundingRectangle(a.X + resolution1.X, a.Y + resolution1.Y, 100f, 100f);
                else if (Math.Abs(resolution2.X) > Math.Abs(resolution1.X)) d = new BoundingRectangle(a.X + resolution2.X, a.Y + resolution2.Y, 100f, 100f);
                else d = new BoundingRectangle(a.X + resolution1.X, a.Y + resolution2.Y, 100f, 100f);
            }

            GameServices.SpriteBatch.DrawRectangle(d.ToRectangle(), Color.Blue);
        }

        private Vector2 ResolveIntersection(Intersection intersection)
        {
            return intersection.GetIntersectionResolution();
        }

        public override void Start(string parameters = "")
        {
            base.Start();
            //this.effect.Start(30, Interfaces.EffectDirection.Backward, Vector2.Zero, Color.Black);
        }

        void effect_effectCompletedEvent(object sender, Interfaces.EffectDirection direction)
        {
            if (nextScreen != null)
            {
                ScreenManager.AddScreen(this, nextScreen);
                ScreenManager.SwitchScreen(nextScreen);
                nextScreen = null;
            }
        }
    }
}
