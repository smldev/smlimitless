﻿//-----------------------------------------------------------------------
// <copyright file="BitmapFont.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Content;
using SMLimitless.Extensions;

namespace SMLimitless.Graphics
{
	/// <summary>
	///   A font that maps parts of an image to characters.
	/// </summary>
	public sealed class BitmapFont
	{
		/// <summary>
		///   A mapping between characters and the textures that represent them.
		/// </summary>
		private Dictionary<char, Texture2D> characters;

		/// <summary>
		///   The size, in pixels, of every character.
		/// </summary>
		private Vector2 characterSize;

		/// <summary>
		///   The absolute file path to the font's configuration file.
		/// </summary>
		private string configPath;

		/// <summary>
		///   The absolute file path to the font's image.
		/// </summary>
		private string imagePath;

		/// <summary>
		///   The texture of any character not appearing in the characters dictionary.
		/// </summary>
		private Texture2D unknownCharTexture;

		/// <summary>
		///   Initializes a new instance of the <see cref="BitmapFont" /> class.
		/// </summary>
		public BitmapFont()
		{
			characters = new Dictionary<char, Texture2D>();
		}

		/// <summary>
		///   Draws a string to the screen.
		/// </summary>
		/// <param name="text">The string to draw.</param>
		/// <param name="position">Where, on the screen, to draw the string.</param>
		/// <param name="scale">
		///   The size of the text on the screen. Defaults to 1.
		/// </param>
		public void DrawString(string text, Vector2 position, float scale = 1f)
		{
			Vector2 scaledCharacterSize = characterSize * scale;
			Vector2 currentDrawPosition = position;

			foreach (char character in text)
			{
				Rectangle drawRect = currentDrawPosition.ToRectangle(scaledCharacterSize);
				if (characters.ContainsKey(character))
				{
					GameServices.SpriteBatch.Draw(characters[character], drawRect, Color.White);
				}
				else
				{
					GameServices.SpriteBatch.Draw(unknownCharTexture, drawRect, Color.White);
				}

				currentDrawPosition.X += scaledCharacterSize.X;
			}
		}

		/// <summary>
		///   Initializes this BitmapFont.
		/// </summary>
		/// <param name="contentResourceName">
		///   The name of the resource to use as the font.
		/// </param>
		public void Initialize(string contentResourceName)
		{
			string filePath = ContentPackageManager.GetAbsoluteFilePath(contentResourceName);

			if (!filePath.EndsWith(".png"))
			{
				throw new ArgumentException(string.Format("BitmapFont.Initialize(string): The file at {0} is not a valid PNG file.", filePath));
			}

			string configPath = string.Concat(filePath.Substring(0, filePath.Length - 3), "txt");
			if (!File.Exists(configPath))
			{
				throw new FileNotFoundException(string.Format("BitmapFont.Initialize(string): The font {0} requires a configuration file.", contentResourceName));
			}

			imagePath = filePath;
			this.configPath = configPath;
		}

		/// <summary>
		///   Loads the content for this BitmapFont.
		/// </summary>
		public void LoadContent()
		{
			string[] config = File.ReadAllLines(configPath);

			if (!config[0].ToLowerInvariant().StartsWith("size"))
			{
				throw new FormatException("BitmapFont.LoadContent(): The configuration file requires the first item to be the size of a character.");
			}

			characterSize = config[0].Split('=')[1].TrimStart().FromString();
			Texture2D baseTexture = GraphicsManager.LoadTextureFromFile(imagePath);

			for (int i = 1; i < config.Length; i++)
			{
				char startingChar;

				if (config[i].StartsWith("="))
				{
					config[i] = "e" + config[i].Substring(1);
					startingChar = '=';

					// this is utterly awful and you should never ever do this
					// except when this code is temporary anyway and yes I know
					// how often "temporary" code isn't temporary
				}
				else
				{
					startingChar = config[i][0];
				}

				Vector2 charPosition = config[i].Split('=')[1].TrimStart().FromString();
				Texture2D charTexture = baseTexture.Crop(new Rectangle((int)charPosition.X, (int)charPosition.Y, (int)characterSize.X, (int)characterSize.Y));
				if ((startingChar == 'u' || startingChar == 'U') && config[i].ToLowerInvariant().StartsWith("unknown"))
				{
					unknownCharTexture = charTexture;
				}
				else
				{
					characters.Add(startingChar, charTexture);
				}
			}
		}
	}
}
