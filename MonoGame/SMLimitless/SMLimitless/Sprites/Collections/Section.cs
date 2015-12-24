using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SMLimitless.Collections;
using SMLimitless.Extensions;
using SMLimitless.Physics;

namespace SMLimitless.Sprites.Collections
{
	public sealed class Section
	{
		private Debug.DebugForm form = new Debug.DebugForm();
		private string debugText = "";

		private Vector2 autoscrollSpeed;
		private string autoscrollPathName;
		private bool isDeserialized;
		private bool isInitialized;
		private bool isContentLoaded;

		public SectionAutoscrollSettings AutoscrollSettings { get; internal set; }
		public Background Background { get; internal set; }
		public BoundingRectangle Bounds { get; internal set; }
		public Camera2D Camera { get; private set; }
		public int Index { get; set; }
		public Level Owner { get; private set; }
		public string Name { get; set; }

		internal List<Tile> Tiles { get; private set; }
		internal SparseCellGrid<Sprite> Sprites { get; private set; }

		internal List<Layer> Layers { get; private set; }
		internal Layer MainLayer { get; set; }
		internal List<Path> Paths { get; private set; }

		public Section(Level owner)
		{
			Camera = new Camera2D();
			Owner = owner;
			Sprites = new SparseCellGrid<Sprite>(GameServices.GameObjectSize * new Vector2(4f));
			Layers = new List<Layer>();
			Tiles = new List<Tile>();
			Paths = new List<Path>();
			Background = new Background(this);

			// temporary
			GameServices.Camera = Camera;
		}

		public void Initialize()
		{
			if (!isInitialized)
			{
				Background.Initialize();
				Layers.ForEach(l => l.Initialize());
				Sprites.ForEach(s => s.Initialize(this));
				isInitialized = true;
			}
		}

		public void LoadContent()
		{
			if (!isContentLoaded)
			{
				Background.LoadContent();
				Layers.ForEach(l => l.LoadContent());
				isContentLoaded = true;
			}
		}

		public void Update()
		{
			System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

			Background.Update();
			Tiles.ForEach(t => t.Update());
			Sprites.ForEach(s => s.Update());

			stopwatch.Stop();
			debugText = $"{1d / stopwatch.ElapsedMilliseconds:F2} FPS";
		}

		public void Draw()
		{
			Background.Draw();
			Tiles.ForEach(t => t.Draw());
			Sprites.ForEach(s => s.Draw());

			GameServices.DrawStringDefault(debugText);
		}

		private void TempUpdate() { }

		public void AddTile(Tile tile)
		{
			if (tile == null)
			{
				throw new ArgumentNullException(nameof(tile), "The tile to add to the section was null.");
			}

			Tiles.Add(tile);
			MainLayer.AddTile(tile);
		}

		public void AddSprite(Sprite sprite)
		{
			if (sprite == null)
			{
				throw new ArgumentNullException(nameof(sprite), "The sprite to add to the section was null.");
			}

			Sprites.Add(sprite);
			MainLayer.AddSprite(sprite);
		}
	}
}