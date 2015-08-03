//-----------------------------------------------------------------------
// <copyright file="TestScreen.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SMLimitless.Collections;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Input;
using SMLimitless.Interfaces;
using SMLimitless.Physics;
using SMLimitless.Screens.Effects;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Screens
{
    /// <summary>
    /// A "scratch pad" screen that can be used
    /// to test anything. The contents of the screen
    /// may change without warning.
    /// </summary>
    public class TestScreen : Screen
    {
		private SparseCellGrid<TestPositionable> grid = new SparseCellGrid<TestPositionable>();
		private TestPositionable currentPositionable;
		private Random random = new Random();
		private string debugText = "";

        /// <summary>
        /// Loads the content for this screen.
        /// </summary>
        public override void LoadContent()
        {
        }

        /// <summary>
        /// Initializes this screen.
        /// </summary>
        /// <param name="owner">The screen that is creating this one.</param>
        /// <param name="parameters">Parameters are unused.</param>
        public override void Initialize(Screen owner, string parameters)
        {
			Effect = new FadeEffect();
        }

		/// <summary>
		/// Updates the screen.
		/// </summary>
		public override void Update()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			Effect.Update();

			if (InputManager.IsNewMousePress(MouseButtons.LeftButton))
			{
				var mousePosition = InputManager.MousePosition;
				var positionableUnderCursor = grid.Items.FirstOrDefault(i => new BoundingRectangle(i.Position, i.Position + i.Size).Within(mousePosition, adjacentPointsAreWithin: true));

				if (positionableUnderCursor == null)
				{
					// Create a new positionable with its center under the cursor
					var newPositionable = new TestPositionable(new Vector2(mousePosition.X - 16f, mousePosition.Y - 16f));
					newPositionable.LoadContent();
					grid.Add(newPositionable);
					currentPositionable = newPositionable;
				}
				else
				{
					// Select the item under the cursor.
					currentPositionable = positionableUnderCursor;
				}
			}

			if (InputManager.IsCurrentKeyPress(Keys.Space))
			{
				for (int i = 0; i < 100; i++)
				{
					var mousePosition = InputManager.MousePosition;
					var newPositionable = new TestPositionable(new Vector2(mousePosition.X - 16f, mousePosition.Y - 16f));
					newPositionable.LoadContent();
					grid.Add(newPositionable);
					currentPositionable = newPositionable;
				}
			}

			if (InputManager.IsCurrentActionPress(InputAction.Up))
			{
				currentPositionable.Position = currentPositionable.Position.Move(Direction.Up, 2f);
			}
			else if (InputManager.IsCurrentActionPress(InputAction.Down))
			{
				currentPositionable.Position = currentPositionable.Position.Move(Direction.Down, 2f);
			}
			else if (InputManager.IsCurrentActionPress(InputAction.Left))
			{
				currentPositionable.Position = currentPositionable.Position.Move(Direction.Left, 2f);
			}
			else if (InputManager.IsCurrentActionPress(InputAction.Right))
			{
				currentPositionable.Position = currentPositionable.Position.Move(Direction.Right, 2f);
			}
			//else
			//{
			//	grid.Items.ForEach(p => p.Position += new Vector2(((float)random.Next(-10, 10)) / 10f, ((float)random.Next(-10, 10)) / 10f));
			//}

			grid.Update();
			stopwatch.Stop();
			debugText = $"{stopwatch.ElapsedMilliseconds} ms for {grid.Items.Count} items";
		}

		/// <summary>
		/// Draws this screen.
		/// </summary>
		public override void Draw()
        {
            Effect.Draw();
			grid.Draw();
			grid.Items.ForEach(i => i.Draw());
			if (currentPositionable != null)
			{
				GameServices.SpriteBatch.DrawRectangleEdges(new BoundingRectangle(currentPositionable.Position, currentPositionable.Position + currentPositionable.Size).ToRectangle(), Color.Aquamarine);
			}
			GameServices.DrawStringDefault(debugText);
        }
		/// <summary>
		/// Starts this screen.
		/// </summary>
		/// <param name="parameters">Parameters are unused.</param>
		public override void Start(string parameters = "")
        {
            base.Start();
            Effect.Set(EffectDirection.Forward, Color.Black);
            Effect.Start(30, EffectDirection.Backward, Vector2.Zero, Color.Black);
        }

        /// <summary>
        /// Handles the EffectCompletedEvent.
        /// </summary>
        /// <param name="sender">The object sending this event.</param>
        /// <param name="direction">The direction of the completed effect.</param>
        private void Effect_effectCompletedEvent(object sender, Interfaces.EffectDirection direction)
        {
            if (NextScreen != null)
            {
                ScreenManager.AddScreen(this, NextScreen);
                ScreenManager.SwitchScreen(NextScreen);
				NextScreen = null;
            }
        }
    }

	public class TestPositionable : IPositionable2
	{
		private StaticGraphicsObject graphics;
		private Vector2 position;

		public Vector2 Position
		{
			get
			{
				return position;
			}
			set
			{
				HasMoved = true;
				position = value;
			}
		}

		public Vector2 Size { get; set; }

		public bool HasMoved { get; set; }

		public TestPositionable(Vector2 startingPosition)
		{
			position = startingPosition;
			Size = new Vector2(32f);
		}

		public void LoadContent()
		{
			graphics = new StaticGraphicsObject();
			graphics.Load(System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"TestPackage\Graphics\scg_test.png"));
			graphics.LoadContent();
		}

		public void Draw()
		{
			graphics.Draw(Position, Color.White);
		}
	}
}

// Basic TestScreen implementation - copy when needed
/////// <summary>
/////// A "scratch pad" screen that can be used
/////// to test anything. The contents of the screen
/////// may change without warning.
/////// </summary>
////public class TestScreen : Screen
////{
////    /// <summary>
////    /// Updates the screen.
////    /// </summary>
////    public override void Update()
////    {
////        Effect.Update();
////    }

////    /// <summary>
////    /// Loads the content for this screen.
////    /// </summary>
////    public override void LoadContent()
////    {
////    }

////    /// <summary>
////    /// Initializes this screen.
////    /// </summary>
////    /// <param name="owner">The screen that is creating this one.</param>
////    /// <param name="parameters">Parameters are unused.</param>
////    public override void Initialize(Screen owner, string parameters)
////    {
////        this.Effect = new FadeEffect();
////    }

////    /// <summary>
////    /// Draws this screen.
////    /// </summary>
////    public override void Draw()
////    {
////        Effect.Draw();
////    }

////    /// <summary>
////    /// Starts this screen.
////    /// </summary>
////    /// <param name="parameters">Parameters are unused.</param>
////    public override void Start(string parameters = "")
////    {
////        base.Start();
////        Effect.Set(Interfaces.EffectDirection.Forward, Color.Black);
////        Effect.Start(30, Interfaces.EffectDirection.Backward, Vector2.Zero, Color.Black);
////    }

////    /// <summary>
////    /// Handles the EffectCompletedEvent.
////    /// </summary>
////    /// <param name="sender">The object sending this event.</param>
////    /// <param name="direction">The direction of the completed effect.</param>
////    private void Effect_effectCompletedEvent(object sender, Interfaces.EffectDirection direction)
////    {
////        if (this.NextScreen != null)
////        {
////            ScreenManager.AddScreen(this, this.NextScreen);
////            ScreenManager.SwitchScreen(this.NextScreen);
////            this.NextScreen = null;
////        }
////    }
////}
