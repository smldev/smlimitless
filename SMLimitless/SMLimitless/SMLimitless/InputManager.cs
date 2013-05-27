//-----------------------------------------------------------------------
// <copyright file="InputManager.cs" company="Chris Akridge">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
namespace SMLimitless
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Provides access to current and previous keyboard and mouse states for use in input.
    /// This class is mostly built from RCIX's XNA InputManager class that he posted on Stack Exchange.
    /// </summary>
    public static class InputManager
    {
        /// <summary>
        /// The keyboard state at the call before last to Update().
        /// </summary>
        private static KeyboardState lastKeyboardState;

        /// <summary>
        /// The keyboard state at the last call to Update().
        /// </summary>
        private static KeyboardState currentKeyboardState;

        /// <summary>
        /// The mouse state at the call before last to Update().
        /// </summary>
        private static MouseState lastMouseState;

        /// <summary>
        /// The mouse state at the last call to Update().
        /// </summary>
        private static MouseState currentMouseState;

        /// <summary>
        /// Gets the keyboard state at the call before last to Update().
        /// </summary>
        public static KeyboardState LastKeyboardState
        {
            get
            {
                return lastKeyboardState;
            }
        }

        /// <summary>
        /// Gets the keyboard state at the last call to Update().
        /// </summary>
        public static KeyboardState CurrentKeyboardState
        {
            get
            {
                return currentKeyboardState;
            }
        }

        /// <summary>
        /// Gets the mouse state at the last call to Update().
        /// </summary>
        public static MouseState LastMouseState
        {
            get
            {
                return lastMouseState;
            }
        }

        /// <summary>
        /// Gets the mouse state at the last call to Update().
        /// </summary>
        public static MouseState CurrentMouseState
        {
            get
            {
                return currentMouseState;
            }
        }

        /// <summary>
        /// Gets the current position of the mouse.
        /// </summary>
        public static Vector2 MousePosition
        {
            get
            {
                return new Vector2(currentMouseState.X, currentMouseState.Y);
            }
        }

        /// <summary>
        /// Gets the current velocity of the mouse,
        /// determined as the difference in the positions
        /// of the current and the last mouse states.
        /// </summary>
        public static Vector2 MouseVelocity
        {
            get
            {
                return new Vector2(currentMouseState.X, currentMouseState.Y) - new Vector2(lastMouseState.X, lastMouseState.Y);
            }
        }

        /// <summary>
        /// Gets the current position of the mouse's scroll wheel.
        /// </summary>
        public static float MouseWheelPosition
        {
            get
            {
                return currentMouseState.ScrollWheelValue;
            }
        }

        /// <summary>
        /// Gets the current velocity of the mouse's scroll wheel,
        /// determined by the difference of the
        /// current and the last scroll wheel positions.
        /// </summary>
        public static float MouseWheelVelocity
        {
            get
            {
                return currentMouseState.ScrollWheelValue - lastMouseState.ScrollWheelValue;
            }
        }

        /// <summary>
        /// Updates the keyboard and mouse states.
        /// The current states becomes the last state.
        /// </summary>
        public static void Update()
        {
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }

        /// <summary>
        /// Determines if a key has been pressed on this frame but not the last.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key press is new, false otherwise.</returns>
        public static bool IsNewKeyPress(Keys key)
        {
            return lastKeyboardState.IsKeyUp(key) && currentKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Determines if a key has been pressed on this frame and the last.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key press is current, false otherwise.</returns>
        public static bool IsCurrentKeyPress(Keys key)
        {
            return lastKeyboardState.IsKeyDown(key) && currentKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Determines if a key was pressed on the last frame but not this one.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key press is old, false otherwise.</returns>
        public static bool IsOldKeyPress(Keys key)
        {
            return lastKeyboardState.IsKeyDown(key) && currentKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Determines if a mouse button was pressed on this frame but not the last.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <returns>True if the mouse press is new, false otherwise.</returns>
        public static bool IsNewMousePress(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed;
                case MouseButtons.MiddleButton:
                    return lastMouseState.MiddleButton == ButtonState.Released && currentMouseState.MiddleButton == ButtonState.Pressed;
                case MouseButtons.RightButton:
                    return lastMouseState.RightButton == ButtonState.Released && currentMouseState.RightButton == ButtonState.Pressed;
                case MouseButtons.ExtraButton1:
                    return lastMouseState.XButton1 == ButtonState.Released && currentMouseState.XButton1 == ButtonState.Pressed;
                case MouseButtons.ExtraButton2:
                    return lastMouseState.XButton2 == ButtonState.Released && currentMouseState.XButton2 == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines if a mouse button was pressed in the last frame and this one.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <returns>True if the mouse press is current, false if otherwise.</returns>
        public static bool IsCurrentMousePress(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return lastMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Pressed;
                case MouseButtons.MiddleButton:
                    return lastMouseState.MiddleButton == ButtonState.Pressed && currentMouseState.MiddleButton == ButtonState.Pressed;
                case MouseButtons.RightButton:
                    return lastMouseState.RightButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Pressed;
                case MouseButtons.ExtraButton1:
                    return lastMouseState.XButton1 == ButtonState.Pressed && currentMouseState.XButton1 == ButtonState.Pressed;
                case MouseButtons.ExtraButton2:
                    return lastMouseState.XButton2 == ButtonState.Pressed && currentMouseState.XButton2 == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines if a mouse button was pressed on the last frame but not this one.
        /// </summary>
        /// <param name="button">The button to check.</param>
        /// <returns>True if the mouse press is old, false if otherwise.</returns>
        public static bool IsOldMousePress(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return lastMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released;
                case MouseButtons.MiddleButton:
                    return lastMouseState.MiddleButton == ButtonState.Pressed && currentMouseState.MiddleButton == ButtonState.Released;
                case MouseButtons.RightButton:
                    return lastMouseState.RightButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Released;
                case MouseButtons.ExtraButton1:
                    return lastMouseState.XButton1 == ButtonState.Pressed && currentMouseState.XButton1 == ButtonState.Released;
                case MouseButtons.ExtraButton2:
                    return lastMouseState.XButton2 == ButtonState.Pressed && currentMouseState.XButton2 == ButtonState.Released;
                default:
                    return false;
            }
        }
    }
}
