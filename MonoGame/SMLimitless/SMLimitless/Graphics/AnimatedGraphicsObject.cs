//-----------------------------------------------------------------------
// <copyright file="AnimatedGraphicsObject.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Extensions;
using SMLimitless.IO;

namespace SMLimitless.Graphics
{
	/// <summary>
	///   A graphics object with multiple textures that are drawn in sequence.
	/// </summary>
	public class AnimatedGraphicsObject : IGraphicsObject
	{
		/// <summary>
		///   A constant field equaling 16.67.
		/// </summary>
		private const float FrameLengthInMilliseconds = 1000f / 60f; // precision is nice

		/// <summary>
		///   The field containing the animation cycle length.
		/// </summary>
		private decimal animationCycleLength;

		/// <summary>
		///   The ComplexGraphicsObject that owns this object.
		///   Null if this object is not owned by a ComplexGraphicsObject.
		/// </summary>
		private ComplexGraphicsObject cgoOwner;

		/// <summary>
		///   A field containing the path to the configuration file that this
		///   object was loaded from.
		/// </summary>
		private string configFilePath;

		/// <summary>
		///   A field containing the path to the image that this object was
		///   loaded from.
		/// </summary>
		private string filePath;

		/// <summary>
		///   The zero-based number of textures in this object. Set to -1 until
		///   the object is loaded.
		/// </summary>
		private int frameCount = -1;

		/// <summary>
		///   The index of the current texture.
		/// </summary>
		private int frameIndex;

		/// <summary>
		///   The width of each texture, measured in pixels.
		/// </summary>
		private int frameWidth;

		/// <summary>
		///   Set when the LoadContent method is called successfully.
		/// </summary>
		private bool isContentLoaded;

		/// <summary>
		///   Set when the Load method is called successfully.
		/// </summary>
		private bool isLoaded;

		/// <summary>
		///   How many rendered frames have been drawn since the last texture change.
		/// </summary>
		private int renderedFramesElapsed;

		/// <summary>
		///   The textures of this object.
		/// </summary>
		private List<Texture2D> textures;

		/// <summary>
		///   Gets or sets the time, measured in seconds, for the animation to
		///   play through all the frames.
		/// </summary>
		public decimal AnimationCycleLength
		{
			get
			{
				return animationCycleLength;
			}

			set
			{
				if (value < ((1m / 60m) * textures.Count))
				{
					animationCycleLength = (1m / 60m) * textures.Count;
				}
				else
				{
					animationCycleLength = value;
				}
			}
		}

		/// <summary>
		///   Gets or sets a value indicating whether the object runs through the textures.
		/// </summary>
		public bool IsRunning { get; set; }

		/// <summary>
		///   Gets a value indicating whether this object will run once. Run-once
		///   objects will cycle through their textures once, and then
		///   continuously draw the last texture until the Reset method is called.
		/// </summary>
		public bool IsRunOnce { get; internal set; }

		/// <summary>
		///   Gets or sets the list of source rectangles of the textures of this
		///   object from the complex graphic texture. This field is required by
		///   ComplexGraphicsObjects and not to be used otherwise.
		/// </summary>
		internal List<Rectangle> CgoSourceRects { get; set; }

		/// <summary>
		///   Gets the frame time, which is the number of rendered frames that
		///   each texture is drawn for.
		/// </summary>
		private int FrameTime
		{
			get
			{
				if (textures == null || textures.Count == 0)
				{
					return 0;
				}
				else
				{
					return (int)(AnimationCycleLength * 60m) / textures.Count;
				}
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates whether this graphics object is
		/// being animated in reverse.
		/// </summary>
		public bool IsReversed { get; set; }

		/// <summary>
		///   Initializes a new instance of the <see
		///   cref="AnimatedGraphicsObject" /> class.
		/// </summary>
		public AnimatedGraphicsObject()
		{
			textures = new List<Texture2D>();
			CgoSourceRects = new List<Rectangle>();
			IsRunning = true;
		}

		/// <summary>
		///   Adjusts the speed of the animation of this object by a percentage.
		///   Rounded to the closest frame boundary (usually one-sixtieth of a second).
		/// </summary>
		/// <param name="percentage">
		///   The percentage by which to adjust the animation speed.
		/// </param>
		public void AdjustSpeed(float percentage)
		{
			percentage /= 100f;
			decimal addend = AnimationCycleLength * (decimal)percentage;
			AnimationCycleLength += addend;
			renderedFramesElapsed = 0;

			// Round it to the nearest frame boundary if necessary.
			if ((AnimationCycleLength * 60m) % 1 != 0)
			{
				decimal cycleInFrames = AnimationCycleLength * 60m;
				if (percentage > 0f)
				{
					// If we're slowing down
					cycleInFrames = NumericExtensions.RoundUp(cycleInFrames);
				}
				else if (percentage < 0f)
				{
					// If we're speeding up
					cycleInFrames = NumericExtensions.RoundDown(cycleInFrames);
				}
				else
				{
					// somehow we're zero
					return;
				}

				AnimationCycleLength = cycleInFrames / 60m;
			}
		}

		/// <summary>
		///   Returns a deep copy of this object. The texture is not cloned, but
		///   everything else is.
		/// </summary>
		/// <returns>A deep copy of this object.</returns>
		public IGraphicsObject Clone()
		{
			var clone = new AnimatedGraphicsObject();
			clone.filePath = filePath;
			clone.configFilePath = configFilePath;
			clone.CgoSourceRects = new List<Rectangle>(CgoSourceRects);
			clone.textures = textures;
			clone.frameCount = frameCount;
			clone.frameWidth = frameWidth;
			clone.AnimationCycleLength = AnimationCycleLength;
			clone.IsRunOnce = IsRunOnce;
			clone.isLoaded = isLoaded;
			clone.isContentLoaded = (textures != null || textures.Count > 0) 
				? isContentLoaded : false;
			return clone;
		}

		/// <summary>
		///   Draws this AnimatedGraphicsObject to the screen.
		/// </summary>
		/// <param name="position">The position to draw this object at.</param>
		/// <param name="color">
		///   The color to shade this object. Use Color.White for no shading.
		/// </param>
		public void Draw(Vector2 position, Color color)
		{
			GameServices.SpriteBatch.Draw(textures[frameIndex], position, color);
		}

		/// <summary>
		///   Draws this AnimatedGraphicsObject to the screen.
		/// </summary>
		/// <param name="position">The position to draw this object at.</param>
		/// <param name="color">
		///   The color to shade this object. Use Color.White for no shading.
		/// </param>
		/// <param name="spriteEffects">How to mirror this object.</param>
		public void Draw(Vector2 position, Color color, SpriteEffects spriteEffects)
		{
			GameServices.SpriteBatch.Draw(textures[frameIndex], position, color, spriteEffects);
		}

		/// <summary>
		///   Draws this AnimatedGraphicsObject to the screen.
		/// </summary>
		/// <param name="position">The position to draw this object at.</param>
		/// <param name="color">
		///   The color to shade this object. Use Color.White for no shading.
		/// </param>
		/// <param name="debug">
		///   If true, the frame index will be drawn in the top-left corner of
		///   the sprite.
		/// </param>
		public void Draw(Vector2 position, Color color, bool debug)
		{
			Draw(position, color);
			if (debug)
			{
				GameServices.DebugFont.DrawString(frameIndex.ToString(), position);
			}
		}

		/// <summary>
		///   Draws this AnimatedGraphicsObject to the screen.
		/// </summary>
		/// <param name="position">The position to draw this object at.</param>
		/// <param name="color">
		///   The color to shade this object. Use Color.White for no shading.
		/// </param>
		/// <param name="spriteEffects">How to mirror this object.</param>
		/// ///
		/// <param name="debug">
		///   If true, the frame index will be drawn in the top-left corner of
		///   the sprite.
		/// </param>
		public void Draw(Vector2 position, Color color, SpriteEffects spriteEffects, bool debug)
		{
			Draw(position, color, spriteEffects);
			if (debug)
			{
				GameServices.DebugFont.DrawString(frameIndex.ToString(), position);
			}
		}

		/// <summary>
		///   Draws this <see cref="AnimatedGraphicsObject" /> to the screen.
		/// </summary>
		/// <param name="position">The position to draw this object at.</param>
		/// <param name="cropping">The portion of this object to draw.</param>
		/// <param name="color">
		///   The color to shade this object. Use Color.White for no shading.
		/// </param>
		/// <param name="effects">How to mirror this object.</param>
		public void Draw(Vector2 position, Rectangle cropping, Color color, SpriteEffects effects)
		{
			if (!textures[frameIndex].ValidateCropping(cropping)) { throw new ArgumentException($"The cropping {cropping} was not valid for this texture. (Width: {textures[frameIndex].Width}, Height: {textures[frameIndex].Height}"); }

			Rectangle destinationRectangle = new Rectangle((int)position.X + cropping.X, (int)position.Y + cropping.Y, cropping.Width, cropping.Height);
			Rectangle sourceRectangle = cropping;

			GameServices.SpriteBatch.Draw(textures[frameIndex], destinationRectangle, sourceRectangle, color, 0f, Vector2.Zero, effects, 0f);
		}

		/// <summary>
		///   Draws this <see cref="AnimatedGraphicsObject" /> to the screen.
		/// </summary>
		/// <param name="position">The position to draw this object at.</param>
		/// <param name="cropping">The portion of this object to draw.</param>
		/// <param name="color">
		///   The color to shade this object. Use Color.White for no shading.
		/// </param>
		/// <param name="effects">How to mirror this object.</param>
		/// <param name="debug">
		///   If true, the frame index will be drawn in the top-left corner of
		///   the sprite.
		/// </param>
		public void Draw(Vector2 position, Rectangle cropping, Color color, SpriteEffects effects, bool debug)
		{
			Draw(position, cropping, color, effects);

			if (debug)
			{
				GameServices.DebugFont.DrawString(frameIndex.ToString(), position);
			}
		}

		/// <summary>
		///   Gets the graphics for this object to be displayed on editor buttons.
		/// </summary>
		/// <returns>
		///   A <see cref="Texture2D" /> instance of the first frame of the object.
		/// </returns>
		public Texture2D GetEditorGraphics()
		{
			return textures[0];
		}

		/// <summary>
		///   Returns the size, in pixels, of the frames of this object.
		/// </summary>
		/// <returns>The size of the object.</returns>
		public Vector2 GetSize()
		{
			if (isContentLoaded)
			{
				return new Vector2(textures[0].Width, textures[0].Height);
			}
			else
			{
				throw new InvalidOperationException("AnimatedGraphicsObject.GetSize(): This object hasn't been fully loaded, and cannot return its size.");
			}
		}

		/// <summary>
		///   Loads this AnimatedGraphicsObject from the specified file path.
		///   This overload is only included to fulfill the IGraphicsObject
		///   contract. Don't call it.
		/// </summary>
		/// <param name="filePath">The file path to the image to use.</param>
		public void Load(string filePath)
		{
			throw new InvalidOperationException("AnimatedGraphicsObject.Load(string): Use the overload Load(string, DataReader) instead.");
		}

		/// <summary>
		///   Loads an instance of an AnimatedGraphicsObject.
		/// </summary>
		/// <param name="filePath">The file path to the image to use.</param>
		/// <param name="config">
		///   A DataReader containing the configuration for this file.
		/// </param>
		public void Load(string filePath, DataReader config)
		{
			if (!isLoaded)
			{
				this.filePath = filePath;
				configFilePath = config.FilePath;

				if (config[0] != "[Animated]" && config[0] != "[Animated_RunOnce]")
				{
					throw new InvalidDataException(string.Format("AnimatedGraphicsObject.Load(string, DataReader): Invalid or corrupt configuration data (expected header [Animated] or [Animated_RunOnce], got header {0})", config[0]));
				}

				Dictionary<string, string> data;
				if (config[0] == "[Animated]")
				{
					data = config.ReadFullSection("[Animated]");
				}
				else
				{
					data = config.ReadFullSection("[Animated_RunOnce]");
					IsRunOnce = true;
				}

				frameWidth = int.Parse(data["FrameWidth"]);
				AnimationCycleLength = decimal.Parse(data["CycleLength"]);

				isLoaded = true;
			}
		}

		/// <summary>
		///   Loads the content for this AnimatedGraphicsObject.
		/// </summary>
		public void LoadContent()
		{
			if (isLoaded && !isContentLoaded)
			{
				Texture2D fullTexture = GraphicsManager.LoadTextureFromFile(filePath);
				int frameHeight = fullTexture.Height;

				if (fullTexture.Width % frameWidth != 0)
				{
					throw new InvalidDataException(string.Format("AnimatedGraphicsObject.LoadContent(): The specified frame width for this texture is invalid. Expected texture with width divisble by {0}, got texture of width {1}.", frameWidth, fullTexture.Width));
				}

				for (int x = 0; x < fullTexture.Width; x += frameWidth)
				{
					textures.Add(GraphicsManager.Crop(fullTexture, new Rectangle(x, 0, frameWidth, frameHeight)));
					frameCount++;
				}

				isContentLoaded = true;
			}
		}

		/// <summary>
		///   Resets this AnimatedGraphicsObject. The texture index becomes 0,
		///   and the rendered frame count also becomes 0.
		/// </summary>
		/// <param name="startRunning">If true, the object will restart.</param>
		public void Reset(bool startRunning)
		{
			renderedFramesElapsed = 0;
			frameIndex = 0;
			if (startRunning)
			{
				IsRunning = true;
			}
		}

		/// <summary>
		///   Adjusts the time it takes for this animated object to complete one
		///   loop through its frames.
		/// </summary>
		/// <param name="newCycleLength">
		///   The time, in seconds, each loop takes.
		/// </param>
		public void SetSpeed(decimal newCycleLength)
		{
			AnimationCycleLength = newCycleLength;
			renderedFramesElapsed = 0;
		}

		/// <summary>
		///   Adjusts how many rendered frames each frame of this object is shown for.
		/// </summary>
		/// <param name="newFrameTime">
		///   How many rendered frames each frame is shown for.
		/// </param>
		public void SetSpeed(int newFrameTime)
		{
			AnimationCycleLength = 60m / textures.Count;
			renderedFramesElapsed = 0;
		}

		/// <summary>
		///   Updates this AnimatedGraphicsObject.
		/// </summary>
		public void Update()
		{
			if (IsRunning)
			{
				renderedFramesElapsed++;
				if (renderedFramesElapsed == FrameTime)
				{
					bool onLastFrame = (IsReversed && frameIndex == 0) ||
									   (!IsReversed && frameIndex == frameCount);
					if (onLastFrame)
					{
						if (IsRunOnce)
						{
							IsRunning = false;
							return;
						}
						else
						{
							if (!IsReversed) { frameIndex = 0; }
							else { frameIndex = frameCount - 1; }
						}
					}
					else
					{
						if (!IsReversed) { frameIndex++; }
						else { frameIndex--; }
					}

					renderedFramesElapsed = 0;
				}
			}
		}

		public void PreviousFrame()
		{
			if (frameIndex == 0) { frameIndex = frameCount; }
			else { frameIndex--; }
			renderedFramesElapsed = 0;
		}

		public void NextFrame()
		{
			if (frameIndex == frameCount) { frameIndex = 0; }
			else { frameIndex++; }
			renderedFramesElapsed = 0;
		}

		/// <summary>
		///   Loads an AnimatedGraphicsObjects from a configuration section in a ComplexGraphicsObject.
		/// </summary>
		/// <param name="section">
		///   The section from the CGO configuration that specifies this object.
		/// </param>
		/// <param name="owner">The CGO that owns this object.</param>
		internal void Load(Dictionary<string, string> section, ComplexGraphicsObject owner)
		{
			if (!isLoaded)
			{
				int frames = int.Parse(section["Frames"]);
				Vector2 frameSize = owner.FrameSize;
				filePath = owner.FilePath;
				for (int i = 0; i < frames; i++)
				{
					CgoSourceRects.Add(Vector2Extensions.Parse(section[string.Concat("Frame", i)]).ToRectangle(frameSize));
				}

				if (section["Type"] == "animated_runonce")
				{
					IsRunOnce = true;
				}

				AnimationCycleLength = decimal.Parse(section["CycleLength"]);
				cgoOwner = owner;
				frameCount = frames - 1;
				isLoaded = true;
			}
		}

		/// <summary>
		///   Loads the content for this AnimatedGraphicsObject.
		/// </summary>
		/// <param name="fileTexture">
		///   The texture of the ComplexGraphicsObject to take the textures from.
		/// </param>
		internal void LoadContentCGO(Texture2D fileTexture)
		{
			if (isLoaded && !isContentLoaded && CgoSourceRects.Any())
			{
				foreach (Rectangle sourceRect in CgoSourceRects)
				{
					textures.Add(fileTexture.Crop(sourceRect));
				}

				isContentLoaded = true;
			}
		}
	}
}
