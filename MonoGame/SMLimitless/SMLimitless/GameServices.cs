﻿//-----------------------------------------------------------------------
// <copyright file="GameServices.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
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
	///   Provides global access to game services.
	/// </summary>
	public static class GameServices
	{
		private static GameServiceContainer container;

		/// <summary>
		///   Gets or sets the current camera.
		/// </summary>
		public static Camera2D Camera { get; set; }

		/// <summary>
		///   Gets or sets a value indicating whether the collision debugger is active.
		/// </summary>
		public static bool CollisionDebuggerActive { get; set; }

		/// <summary>
		///   Gets or sets the current <see cref="Forms.CollisionDebuggerForm" /> instance.
		/// </summary>
		public static CollisionDebuggerForm CollisionDebuggerForm { get; set; }

		/// <summary>
		///   Gets a bitmap font used for drawing debug text to screen.
		/// </summary>
		public static BitmapFont DebugFont { get; private set; }

		/// <summary>
		///   Gets a debug form used for printing log messages and receiving
		///   debug commands.
		/// </summary>
		public static DebugForm DebugForm { get; private set; }

		/// <summary>
		///   Gets or sets a dictionary containing <see cref="Effects" />
		///   instances and their names.
		/// </summary>
		public static Dictionary<string, Effect> Effects { get; set; } = new Dictionary<string, Effect>();

		/// <summary>
		///   Gets the standard size for a game object (tile/sprite/etc). Subject
		///   to change.
		/// </summary>
		public static Vector2 GameObjectSize => new Vector2(16f);

		/// <summary>
		///   Gets a reference to the GameTime instance.
		/// </summary>
		public static GameTime GameTime => GetService<GameTime>();

		/// <summary>
		///   Gets a reference to the GraphicsDevice.
		/// </summary>
		public static GraphicsDevice Graphics => GetService<GraphicsDevice>();

		/// <summary>
		///   Gets a form used to edit global physics settings for game objects.
		/// </summary>
		public static PhysicsSettingsEditorForm PhysicsSettingsEditorForm { get; private set; }

		/// <summary>
		///   Gets the size, in pixels, of a QuadTree cell.
		/// </summary>
		public static Vector2 QuadTreeCellSize => new Vector2(64f);

		/// <summary>
		///	  Gets a global random number generator.
		/// </summary>
		public static Random Random = new Random();

		/// <summary>
		///   Gets or sets the size of the window in pixels.
		/// </summary>
		public static Vector2 ScreenSize { get; set; }

		/// <summary>
		///   Gets a reference to the SpriteBatch.
		/// </summary>
		public static SpriteBatch SpriteBatch => GetService<SpriteBatch>();

		/// <summary>
		///   Gets the GameServicesContainer that stores references to the
		///   services added.
		/// </summary>
		internal static GameServiceContainer Container => container ?? (container = new GameServiceContainer());

		static GameServices()
		{
			DebugForm = new DebugForm();
			PhysicsSettingsEditorForm = new PhysicsSettingsEditorForm();
		}

		/// <summary>
		///   Adds a service to the container.
		/// </summary>
		/// <typeparam name="T">The type of the service to add.</typeparam>
		/// <param name="service">The service to add.</param>
		public static void AddService<T>(T service) => Container.AddService(service);

		/// <summary>
		///   Draws a given string using the debug font at {X: 16, Y:16} at
		///   double scale.
		/// </summary>
		/// <param name="text">The text to draw.</param>
		public static void DrawStringDefault(string text) => DebugFont?.DrawString(text, new Vector2(16f), 2f);

		/// <summary>
		///   Returns a game service.
		/// </summary>
		/// <typeparam name="T">The type of game service to retrieve.</typeparam>
		/// <returns>A game service of the specified type.</returns>
		public static T GetService<T>() where T : class => Container.GetService<T>();

		/// <summary>
		///   Initializes the debug font.
		/// </summary>
		/// <param name="contentResourceName">
		///   The name of the font's content resource.
		/// </param>
		public static void InitializeFont(string contentResourceName)
		{
			DebugFont = new BitmapFont();
			DebugFont.Initialize(contentResourceName);
			DebugFont.LoadContent();
		}

		/// <summary>
		///   Initializes the services container with key game services.
		/// </summary>
		/// <param name="graphicsDevice">
		///   The GraphicsDevice to add to the container.
		/// </param>
		/// <param name="spriteBatch">The SpriteBatch to add to the container.</param>
		/// <param name="content">The ContentManager to add to the container.</param>
		public static void InitializeServices(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, ContentManager content)
		{
			AddService<GraphicsDevice>(graphicsDevice);
			AddService<SpriteBatch>(spriteBatch);
			AddService<ContentManager>(content);
		}

		/// <summary>
		///   Removes a service from the container.
		/// </summary>
		/// <typeparam name="T">The type of the service to remove.</typeparam>
		public static void RemoveService<T>() => Container.RemoveService(typeof(T));
	}
}
