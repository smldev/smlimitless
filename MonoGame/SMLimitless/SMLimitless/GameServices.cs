//-----------------------------------------------------------------------
// <copyright file="GameServices.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Debug;
using SMLimitless.Forms;
using SMLimitless.Graphics;
using SMLimitless.Physics;
    
namespace SMLimitless
{
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

		public static Dictionary<string, Effect> Effects { get; set; } = new Dictionary<string, Effect>();

        /// <summary>
        /// Gets or sets the size of the window. Measured in pixels.
        /// </summary>
        public static Vector2 ScreenSize { get; set; }

        /// <summary>
        /// Gets a bitmap font used for drawing debug text to screen.
        /// </summary>
        public static BitmapFont DebugFont { get; private set; }

		/// <summary>
		/// Gets a debug form used for printing log messages and receiving debug commands.
		/// </summary>
		public static DebugForm DebugForm { get; private set; }

		public static PhysicsSettingsEditorForm PhysicsSettingsEditorForm { get; private set; }

        /// <summary>
        /// Gets the size, in pixels, of a QuadTree cell.
        /// </summary>
        public static Vector2 QuadTreeCellSize
        {
            get
            {
                return new Vector2(64f, 64f);
            }
        }

        /// <summary>
        /// Gets the standard size for a game object (tile/sprite/etc). Subject to change.
        /// </summary>
        public static Vector2 GameObjectSize
        {
            get
            {
                return new Vector2(16f);
            }
        }

		static GameServices()
		{
			DebugForm = new DebugForm();
			PhysicsSettingsEditorForm = new PhysicsSettingsEditorForm();
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
        /// Initializes the debug font.
        /// </summary>
        /// <param name="contentResourceName">The name of the font's content resource.</param>
        public static void InitializeFont(string contentResourceName)
        {
            DebugFont = new BitmapFont();
            DebugFont.Initialize(contentResourceName);
            DebugFont.LoadContent();
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

        /// <summary>
        /// Draws a given string using the debug font at {X: 16, Y:16} at double scale.
        /// </summary>
        /// <param name="text">The text to draw.</param>
        public static void DrawStringDefault(string text)
        {
            if (DebugFont != null)
            {
                DebugFont.DrawString(text, new Vector2(16f, 16f), 2f);
            }
        }
    }
}
