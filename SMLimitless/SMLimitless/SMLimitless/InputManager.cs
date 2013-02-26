using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace SMLimitless
{
    // Credit to RCIX of StackExchange GameDev for some assistance
    public static class InputManager
    {
        private static KeyboardState lastKeyboardState;
        private static KeyboardState currentKeyboardState;
        private static MouseState lastMouseState;
        private static MouseState currentMouseState;

        #region Properties
        public static KeyboardState LastKeyboardState
        {
            get
            {
                return lastKeyboardState;
            }
        }

        public static KeyboardState CurrentKeyboardState
        {
            get
            {
                return currentKeyboardState;
            }
        }

        public static MouseState LastMouseState
        {
            get
            {
                return lastMouseState;
            }
        }

        public static MouseState CurrentMouseState
        {
            get
            {
                return currentMouseState;
            }
        }


        public static Vector2 MousePosition
        {
            get
            {
                return new Vector2(currentMouseState.X, currentMouseState.Y);
            }
        }

        public static Vector2 MouseVelocity
        {
            get
            {
                return (new Vector2(currentMouseState.X, currentMouseState.Y) - new Vector2(lastMouseState.X, lastMouseState.Y));
            }
        }

        public static float MouseWheelPosition
        {
            get
            {
                return currentMouseState.ScrollWheelValue;
            }
        }

        public static float MouseWheelVelocity
        {
            get
            {
                return currentMouseState.ScrollWheelValue - lastMouseState.ScrollWheelValue;
            }
        }
        #endregion

        public static void Update()
        {
            if (lastKeyboardState == null && currentKeyboardState == null)
            {
                lastKeyboardState = currentKeyboardState = Keyboard.GetState();
            }
            else
            {
                lastKeyboardState = currentKeyboardState;
                currentKeyboardState = Keyboard.GetState();
            }

            if (lastMouseState == null && currentMouseState == null)
            {
                lastMouseState = currentMouseState = Mouse.GetState();
            }
            else
            {
                lastMouseState = currentMouseState;
                currentMouseState = Mouse.GetState();
            }
        }

        public static bool IsNewKeyPress(Keys key)
        {
            return (lastKeyboardState.IsKeyUp(key) && currentKeyboardState.IsKeyDown(key));
        }

        public static bool IsCurrentKeyPress(Keys key)
        {
            return (lastKeyboardState.IsKeyDown(key) && currentKeyboardState.IsKeyDown(key));
        }

        public static bool IsOldPress(Keys key)
        {
            return (lastKeyboardState.IsKeyDown(key) && currentKeyboardState.IsKeyDown(key));
        }

        public static bool IsNewMousePress(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (
                        lastMouseState.LeftButton == ButtonState.Released &&
                        currentMouseState.LeftButton == ButtonState.Pressed);
                case MouseButtons.MiddleButton:
                    return (
                        lastMouseState.MiddleButton == ButtonState.Released &&
                        currentMouseState.MiddleButton == ButtonState.Pressed);
                case MouseButtons.RightButton:
                    return (
                        lastMouseState.RightButton == ButtonState.Released &&
                        currentMouseState.RightButton == ButtonState.Pressed);
                case MouseButtons.ExtraButton1:
                    return (
                        lastMouseState.XButton1 == ButtonState.Released &&
                        currentMouseState.XButton1 == ButtonState.Pressed);
                case MouseButtons.ExtraButton2:
                    return (
                        lastMouseState.XButton2 == ButtonState.Released &&
                        currentMouseState.XButton2 == ButtonState.Pressed);
                default:
                    return false;
            }
        }

        public static bool IsCurrentMousePress(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (
                        lastMouseState.LeftButton == ButtonState.Pressed &&
                        currentMouseState.LeftButton == ButtonState.Pressed);
                case MouseButtons.MiddleButton:
                    return (
                        lastMouseState.MiddleButton == ButtonState.Pressed &&
                        currentMouseState.MiddleButton == ButtonState.Pressed);
                case MouseButtons.RightButton:
                    return (
                        lastMouseState.RightButton == ButtonState.Pressed &&
                        currentMouseState.RightButton == ButtonState.Pressed);
                case MouseButtons.ExtraButton1:
                    return (
                        lastMouseState.XButton1 == ButtonState.Pressed &&
                        currentMouseState.XButton1 == ButtonState.Pressed);
                case MouseButtons.ExtraButton2:
                    return (
                        lastMouseState.XButton2 == ButtonState.Pressed &&
                        currentMouseState.XButton2 == ButtonState.Pressed);
                default:
                    return false;
            }
        }

        public static bool IsOldPress(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (
                        lastMouseState.LeftButton == ButtonState.Pressed &&
                        currentMouseState.LeftButton == ButtonState.Released);
                case MouseButtons.MiddleButton:
                    return (
                        lastMouseState.MiddleButton == ButtonState.Pressed &&
                        currentMouseState.MiddleButton == ButtonState.Released);
                case MouseButtons.RightButton:
                    return (
                        lastMouseState.RightButton == ButtonState.Pressed &&
                        currentMouseState.RightButton == ButtonState.Released);
                case MouseButtons.ExtraButton1:
                    return (
                        lastMouseState.XButton1 == ButtonState.Pressed &&
                        currentMouseState.XButton1 == ButtonState.Released);
                case MouseButtons.ExtraButton2:
                    return (
                        lastMouseState.XButton2 == ButtonState.Pressed &&
                        currentMouseState.XButton2 == ButtonState.Released);
                default:
                    return false;
            }
        }
    }
}
