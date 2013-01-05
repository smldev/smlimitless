using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SMLimitless
{
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

        public static void InitializeServices(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime gameTime)
        {
            AddService<GraphicsDevice>(graphicsDevice);
            AddService<SpriteBatch>(spriteBatch);
            AddService<GameTime>(gameTime);
        }

        public static GraphicsDevice Graphics
        {
            get { return GetService<GraphicsDevice>(); }
        }

        public static SpriteBatch SpriteBatch
        {
            get { return GetService<SpriteBatch>(); }
        }

        public static GameTime GameTime
        {
            get { return GetService<GameTime>(); }
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
    }
}
