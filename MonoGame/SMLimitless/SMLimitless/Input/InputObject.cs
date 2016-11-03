//-----------------------------------------------------------------------
// <copyright file="InputObject.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework.Input;

namespace SMLimitless.Input
{
	/// <summary>
	///   An enumeration of sources of input.
	/// </summary>
	internal enum InputSource : byte
	{
		/// <summary>
		///   A plastic board with a group of plastic keys, each printed with a
		///   character or set of characters.
		/// </summary>
		Keyboard = 0,

		/// <summary>
		///   A plastic strangely-shaped object that can easily be moved around.
		///   Controls an image on a monitor that can be used to point at other
		///   images on the monitor. Has at least two buttons that can be used to
		///   perform actions with or using the images.
		/// </summary>
		Mouse = 1
	}

	/// <summary>
	///   Represents an input from a keyboard or mouse.
	/// </summary>
	internal struct InputObject
	{
		/// <summary>
		///   Represents the enumeration value of this input, cast to int.
		/// </summary>
		private int inputValue;

		/// <summary>
		///   The backing field for the Source property.
		/// </summary>
		private InputSource source;

		/// <summary>
		///   Gets the source of this input.
		/// </summary>
		internal InputSource Source
		{
			get
			{
				return source;
			}

			private set
			{
				source = value;
			}
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="InputObject" /> struct.
		/// </summary>
		/// <param name="keys">A key.</param>
		internal InputObject(Keys keys)
		{
			inputValue = (int)keys;
			source = InputSource.Keyboard;
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="InputObject" /> struct.
		/// </summary>
		/// <param name="mouse">
		///   A MouseButtons instance used to initialize this struct.
		/// </param>
		internal InputObject(MouseButtons mouse)
		{
			inputValue = (int)mouse;
			source = InputSource.Mouse;
		}

		/// <summary>
		///   Returns a string representation of this InputObject.
		/// </summary>
		/// <returns>
		///   A string reading "Keyboard: " or "Mouse: ", followed by the key or
		///   mouse button.
		/// </returns>
		public override string ToString()
		{
			if (source == InputSource.Keyboard)
			{
				Keys key = (Keys)inputValue;
				return string.Concat("Keyboard: ", key.ToString());
			}
			else
			{
				MouseButtons mouse = (MouseButtons)inputValue;
				return string.Concat("Mouse: ", mouse.ToString());
			}
		}

		/// <summary>
		///   Parses an input object from a string.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		/// <returns>An input object as parsed by string.</returns>
		internal static InputObject Parse(string value)
		{
			Keys keys;
			MouseButtons mouseButtons;

			bool isKeyboardInput = Enum.TryParse<Keys>(value, out keys);
			bool isMouseInput = Enum.TryParse<MouseButtons>(value, out mouseButtons);

			if (isKeyboardInput)
			{
				return new InputObject(keys);
			}
			else if (isMouseInput)
			{
				return new InputObject(mouseButtons);
			}
			else
			{
				throw new ArgumentException(string.Format("InputObject.Parse(string): The value {0} cannot be parsed as a keyboard key or a mouse button.", value), "value");
			}
		}

		/// <summary>
		///   Tries to parse a value into an InputObject.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		/// <param name="output">
		///   The InputObject to output the parse result to.
		/// </param>
		/// <returns>True if the value can be parsed, false if otherwise.</returns>
		internal static bool TryParse(string value, out InputObject output)
		{
			Keys keys;
			MouseButtons mouseButtons;

			bool canParseAsKeys = Enum.TryParse<Keys>(value, out keys);
			bool canParseAsMouseButtons = Enum.TryParse<MouseButtons>(value, out mouseButtons);

			if (canParseAsKeys)
			{
				output = new InputObject(keys);
				return true;
			}
			else if (canParseAsMouseButtons)
			{
				output = new InputObject(mouseButtons);
				return true;
			}

			output = default(InputObject);
			return false;
		}

		/// <summary>
		///   Determines if the keyboard key represented by this object is down.
		/// </summary>
		/// <param name="keys">The keyboard state to check.</param>
		/// <returns>True if the key is down, false if otherwise.</returns>
		internal bool IsDown(KeyboardState keys)
		{
			if (source == InputSource.Keyboard)
			{
				return keys.IsKeyDown((Keys)inputValue);
			}
			else
			{
				throw new ArgumentException("InputObject.IsDown(KeyboardState): This input object does not represent a keyboard key.", "keys");
			}
		}

		/// <summary>
		///   Determines if the mouse button represented by this object is down.
		/// </summary>
		/// <param name="mouse">The mouse state to check.</param>
		/// <returns>True if the button is down, false if otherwise.</returns>
		internal bool IsDown(MouseState mouse)
		{
			if (source == InputSource.Mouse)
			{
				MouseButtons button = (MouseButtons)inputValue;
				switch (button)
				{
					case MouseButtons.LeftButton:
						return mouse.LeftButton == ButtonState.Pressed;
					case MouseButtons.MiddleButton:
						return mouse.MiddleButton == ButtonState.Pressed;
					case MouseButtons.RightButton:
						return mouse.RightButton == ButtonState.Pressed;
					case MouseButtons.ExtraButton1:
						return mouse.XButton1 == ButtonState.Pressed;
					case MouseButtons.ExtraButton2:
						return mouse.XButton2 == ButtonState.Pressed;
					default:
						return false;
				}
			}
			else
			{
				throw new ArgumentException("InputObject.IsDown(MouseState): This input object does not represent a mouse button.", "mouse");
			}
		}

		/// <summary>
		///   Determines if this input object is down.
		/// </summary>
		/// <param name="keys">The keyboard state to check.</param>
		/// <param name="mouse">The mouse state to check.</param>
		/// <returns>True if the input object is down, false if otherwise.</returns>
		internal bool IsDown(KeyboardState keys, MouseState mouse)
		{
			if (source == InputSource.Keyboard)
			{
				return IsDown(keys);
			}

			return IsDown(mouse);
		}

		/// <summary>
		///   Determines if the keyboard key represented by this object is up.
		/// </summary>
		/// <param name="keys">The keyboard state to check.</param>
		/// <returns>True if the key is up, false if otherwise.</returns>
		internal bool IsUp(KeyboardState keys)
		{
			if (source == InputSource.Keyboard)
			{
				return !IsDown(keys);
			}
			else
			{
				throw new ArgumentException("InputObject.IsUp(KeyboardState): This input object does not represent a keyboard key.", "keys");
			}
		}

		/// <summary>
		///   Determines if the mouse button represented by this object is up.
		/// </summary>
		/// <param name="mouse">The mouse state to check.</param>
		/// <returns>True if the button is up, false if otherwise.</returns>
		internal bool IsUp(MouseState mouse)
		{
			if (source == InputSource.Mouse)
			{
				return !IsDown(mouse);
			}
			else
			{
				throw new ArgumentException("InputObject.IsUp(MouseState): This input object does not represent a mouse button.", "mouse");
			}
		}

		/// <summary>
		///   Determines if this input object is up.
		/// </summary>
		/// <param name="keys">The keyboard state to check.</param>
		/// <param name="mouse">The mouse state to check.</param>
		/// <returns>True if the input object is up, false if otherwise.</returns>
		internal bool IsUp(KeyboardState keys, MouseState mouse)
		{
			return !IsDown(keys, mouse);
		}
	}
}
