using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SMLimitless
{
    /// <summary>
    /// Provides global access to game services.
    /// </summary>
    public static class GameServices
    {
        private static GameServiceContainer container;
        public static GameServiceContainer Container
        {
            get
            {
                if (container == null)
                {
                    container = new GameServiceContainer();
                }
                return container;
            }
        }

        public static Vector2 ScreenSize;

        public static void InitializeServices(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, ContentManager content)
        {
            AddService<GraphicsDevice>(graphicsDevice);
            AddService<SpriteBatch>(spriteBatch);
            AddService<ContentManager>(content);
        }

        public static GraphicsDevice Graphics
        {
            get { return GetService<GraphicsDevice>(); }
        }

        public static SpriteBatch SpriteBatch
        {
            get { return GetService<SpriteBatch>(); }
        }

        public static T GetService<T>()
        {
            return (T)Container.GetService(typeof(T));
        }

        public static void AddService<T>(T service)
        {
            Container.AddService(typeof(T), service);
        }

        public static void RemoveService<T>()
        {
            Container.RemoveService(typeof(T));
        }

        #region Global Debug Fonts
        private static SpriteFont debugFontSmall;
        private static SpriteFont debugFontLarge;

        public static SpriteFont DebugFontSmall
        {
            get
            {
                if (debugFontSmall == null) debugFontSmall = GetService<ContentManager>().Load<SpriteFont>("DebugFontSmall");
                return debugFontSmall;
            }
        }

        public static SpriteFont DebugFontLarge
        {
            get
            {
                if (debugFontLarge == null) debugFontLarge = GetService<ContentManager>().Load<SpriteFont>("DebugFontLarge");
                return debugFontLarge;
            }
        }
        #endregion
    }
}
