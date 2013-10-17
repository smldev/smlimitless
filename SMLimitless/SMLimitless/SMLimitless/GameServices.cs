//-----------------------------------------------------------------------
// <copyright file="GameServices.cs" company="Chris Akridge">
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
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using SMLimitless.Physics;
    
    /// <summary>
    /// Provides global access to game services.
    /// </summary>
    public static class GameServices
    {
        /// <summary>
        /// The field for the container.
        /// </summary>
        private static GameServiceContainer container;

        /// <summary>
        /// The field for the small debug font.
        /// </summary>
        private static SpriteFont debugFontSmall;

        /// <summary>
        /// The field for the large debug font.
        /// </summary>
        private static SpriteFont debugFontLarge;

        /// <summary>
        /// Gets the GameServicesContainer that stores references to the services added.
        /// </summary>
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

        /// <summary>
        /// Gets a reference to the GraphicsDevice.
        /// </summary>
        public static GraphicsDevice Graphics
        {
            get { return GetService<GraphicsDevice>(); }
        }

        /// <summary>
        /// Gets a reference to the SpriteBatch.
        /// </summary>
        public static SpriteBatch SpriteBatch
        {
            get { return GetService<SpriteBatch>(); }
        }

        /// <summary>
        /// Gets a reference to the GameTime instance.
        /// </summary>
        public static GameTime GameTime
        {
            get { return GetService<GameTime>(); }
        }

        /// <summary>
        /// Gets or sets the current camera.
        /// </summary>
        public static Camera2D Camera { get; set; }

        /// <summary>
        /// Gets or sets the size of the window. Measured in pixels.
        /// </summary>
        public static Vector2 ScreenSize { get; set; }

        /// <summary>
        /// Gets a font, Consolas 6 point.
        /// </summary>
        public static SpriteFont DebugFontSmall
        {
            get
            {
                if (debugFontSmall == null)
                {
                    debugFontSmall = GetService<ContentManager>().Load<SpriteFont>("DebugFontSmall");
                }

                return debugFontSmall;
            }
        }

        /// <summary>
        /// Gets a font, Consolas 20 point.
        /// </summary>
        public static SpriteFont DebugFontLarge
        {
            get
            {
                if (debugFontLarge == null)
                {
                    debugFontLarge = GetService<ContentManager>().Load<SpriteFont>("DebugFontLarge");
                }

                return debugFontLarge;
            }
        }

        /// <summary>
        /// Initializes the services container with key game services.
        /// </summary>
        /// <param name="graphicsDevice">The GraphicsDevice to add to the container.</param>
        /// <param name="spriteBatch">The SpriteBatch to add to the container.</param>
        /// <param name="content">The ContentManager to add to the container.</param>
        public static void InitializeServices(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, ContentManager content)
        {
            AddService<GraphicsDevice>(graphicsDevice);
            AddService<SpriteBatch>(spriteBatch);
            AddService<ContentManager>(content);
        }

        /// <summary>
        /// Returns a game service.
        /// </summary>
        /// <typeparam name="T">The type of game service to retrieve.</typeparam>
        /// <returns>A game service of the specified type.</returns>
        public static T GetService<T>()
        {
            return (T)Container.GetService(typeof(T));
        }

        /// <summary>
        /// Adds a service to the container.
        /// </summary>
        /// <typeparam name="T">The type of the service to add.</typeparam>
        /// <param name="service">The service to add.</param>
        public static void AddService<T>(T service)
        {
            Container.AddService(typeof(T), service);
        }

        /// <summary>
        /// Removes a service from the container.
        /// </summary>
        /// <typeparam name="T">The type of the service to remove.</typeparam>
        public static void RemoveService<T>()
        {
            Container.RemoveService(typeof(T));
        }
    }
}
