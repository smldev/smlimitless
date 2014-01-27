using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Content;
using SMLimitless.Extensions;

namespace SMLimitless.Graphics
{
    public sealed class BitmapFont
    {
        private Dictionary<char, Texture2D> characters;
        private Texture2D unknownCharTexture;
        private Vector2 characterSize;

        private string imagePath;
        private string configPath;

        public BitmapFont()
        {
            this.characters = new Dictionary<char, Texture2D>();
        }

        public void Initialize(string contentResourceName)
        {
            string filePath = ContentPackageManager.GetAbsoluteFilePath(contentResourceName);

            if (!filePath.EndsWith(".png"))
            {
                throw new Exception(string.Format("BitmapFont.Initialize(string): The file at {0} is not a valid PNG file.", filePath));
            }

            string configPath = string.Concat(filePath.Substring(0, filePath.Length - 3), "txt");
            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException(string.Format("BitmapFont.Initialize(string): The font {0} requires a configuration file.", contentResourceName));
            }

            this.imagePath = filePath;
            this.configPath = configPath;
        }

        public void LoadContent()
        {
            string[] config = File.ReadAllLines(this.configPath);

            if (!config[0].ToLowerInvariant().StartsWith("size"))
            {
                throw new Exception("BitmapFont.LoadContent(): The configuration file requires the first item to be the size of a character.");
            }

            this.characterSize = config[0].Split('=')[1].TrimStart().FromString();
            Texture2D baseTexture = GraphicsManager.LoadTextureFromFile(this.imagePath);
            
            for (int i = 1; i < config.Length; i++)
            {
                char startingChar;

                if (config[i].StartsWith("="))
                {
                    config[i] = "e" + config[i].Substring(1); // this is utterly awful and you should never ever do this
                    startingChar = '='; // except when this code is temporary anyway
                } // and yes I know how often "temporary" code isn't temporary
                else
                {
                    startingChar = config[i][0];
                }
                Vector2 charPosition = config[i].Split('=')[1].TrimStart().FromString();
                Texture2D charTexture = baseTexture.Crop(new Rectangle((int)charPosition.X, (int)charPosition.Y, (int)this.characterSize.X, (int)this.characterSize.Y));
                if ((startingChar == 'u' || startingChar == 'U') && config[i].ToLowerInvariant().StartsWith("unknown"))
                {
                    this.unknownCharTexture = charTexture;
                }
                else
                {
                    this.characters.Add(startingChar, charTexture);
                }
            }
        }

        public void DrawString(string text, Vector2 position, float scale = 1f)
        {
            Vector2 scaledCharacterSize = this.characterSize * scale;
            Vector2 currentDrawPosition = position;

            foreach(char character in text)
            {
                Rectangle drawRect = currentDrawPosition.ToRectangle(scaledCharacterSize);
                if (this.characters.ContainsKey(character))
                {
                    GameServices.SpriteBatch.Draw(this.characters[character], drawRect, Color.White);
                }
                else
                {
                    GameServices.SpriteBatch.Draw(this.unknownCharTexture, drawRect, Color.White);
                }

                currentDrawPosition.X += scaledCharacterSize.X;
            }
        }
    }
}
