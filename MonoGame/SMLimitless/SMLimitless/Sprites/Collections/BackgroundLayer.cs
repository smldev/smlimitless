//-----------------------------------------------------------------------
// <copyright file="BackgroundLayer.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Content;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Physics;

namespace SMLimitless.Sprites.Collections
{
	/// <summary>
	///   A single layer of a section background. Contains one image that scrolls
	///   at a given rate.
	/// </summary>
	public sealed class BackgroundLayer
	{
		/// <summary>
		///   The background texture of this layer.
		/// </summary>
		private IGraphicsObject backgroundTexture;

		/// <summary>
		///   A reference to the camera that this background is following.
		/// </summary>
		private Camera2D camera;

		/// <summary>
		///   The overall displacement of this layer.
		/// </summary>
		private Vector2 delta;

		/// <summary>
		///   Determines whether the content for this background layer has been loaded.
		/// </summary>
		private bool isContentLoaded;

		/// <summary>
		///   The camera's position in the last frame.
		/// </summary>
		private Vector2 oldCameraPosition;

		/// <summary>
		///   The bounds of the section that this layer is contained within.
		/// </summary>
		private BoundingRectangle sectionBounds;

		/// <summary>
		///   The size of the texture in pixels.
		/// </summary>
		private Vector2 textureSize;

		/// <summary>
		///   Gets the overall displacement of this layer.
		/// </summary>
		public Vector2 Delta
		{
			get
			{
				return delta;
			}

			private set
			{
				delta = value.Mod(textureSize);
			}
		}

		/// <summary>
		///   Gets the direction that this background layer scrolls.
		/// </summary>
		public BackgroundScrollDirection ScrollDirection { get; internal set; }

		/// <summary>
		///   Gets the rate that this background scrolls. A value of 1 will
		///   scroll as fast as the camera. Values greater or lesser than 1 will
		///   scroll faster or slower than the camera, respectively.
		/// </summary>
		public float ScrollRate { get; internal set; }

		/// <summary>
		///   Gets or sets the resource name of the background texture.
		/// </summary>
		internal string BackgroundTextureResourceName { get; set; }

		/// <summary>
		///   Initializes a new instance of the <see cref="BackgroundLayer" /> class.
		/// </summary>
		/// <param name="ownerCamera">
		///   The camera of the section that contains this layer.
		/// </param>
		/// <param name="sectionBounds">
		///   The bounds of the section that contains this layer.
		/// </param>
		public BackgroundLayer(Camera2D ownerCamera, BoundingRectangle sectionBounds)
		{
			camera = ownerCamera;
			this.sectionBounds = sectionBounds;
		}

		/// <summary>
		///   Loads a background layer, given a JSON string containing a
		///   background layer.
		/// </summary>
		/// <param name="json">A valid JSON string.</param>
		[Obsolete]
		public void Deserialize(string json)
		{
			JObject obj = JObject.Parse(json);

			string resourceName = (string)obj["resourceName"];
			BackgroundScrollDirection direction = (BackgroundScrollDirection)(int)obj["scrollDirection"];
			float scrollRate = (float)obj["scrollRate"];

			BackgroundTextureResourceName = resourceName;
			backgroundTexture = ContentPackageManager.GetGraphicsResource(BackgroundTextureResourceName);
			ScrollDirection = direction;
			ScrollRate = scrollRate;
		}

		/// <summary>
		///   Draws this layer to the screen.
		/// </summary>
		public void Draw()
		{
			if (ScrollDirection == BackgroundScrollDirection.Fixed)
			{
				Vector2 textureCenter = textureSize / 2;
				Vector2 viewportCenter = camera.Viewport.Center;

				backgroundTexture.Draw(new Vector2(viewportCenter.X - textureCenter.X, viewportCenter.Y - textureCenter.Y), Color.White);
			}
			else if (ScrollDirection == BackgroundScrollDirection.Horizontal)
			{
				float baseTextureDrawPositionX = camera.Viewport.X + -Delta.X;
				float baseTextureDrawPositionY;

				if (textureSize.Y >= camera.ViewportSize.Y)
				{
					float minorAxisScrollableHeight = -sectionBounds.Height - camera.ViewportSize.Y;
					float viewportOriginRelatingToScrollableHeight = camera.Position.Y / minorAxisScrollableHeight;
					float viewportMinorAxisLine = camera.ViewportSize.Y * viewportOriginRelatingToScrollableHeight;
					float textureMinorAxisLine = textureSize.Y * viewportOriginRelatingToScrollableHeight;
					baseTextureDrawPositionY = -(viewportMinorAxisLine - textureMinorAxisLine) + camera.Viewport.Y;
				}
				else
				{
					baseTextureDrawPositionY = camera.Viewport.Y;
				}

				backgroundTexture.Draw(new Vector2(baseTextureDrawPositionX, baseTextureDrawPositionY), Color.White);

				foreach (Vector2 textureDrawPosition in GetTextureTileDrawPoints(new Vector2(baseTextureDrawPositionX, baseTextureDrawPositionY)))
				{
					backgroundTexture.Draw(textureDrawPosition, Color.White);
				}
			}
			else if (ScrollDirection == BackgroundScrollDirection.Vertical)
			{
				float baseTextureDrawPositionX;
				float baseTextureDrawPositionY = camera.Viewport.Y + -Delta.Y;

				if (textureSize.X >= camera.ViewportSize.X)
				{
					float minorAxisScrollableWidth = -sectionBounds.Width - camera.ViewportSize.X;
					float viewportOriginRelatingToScrollableWidth = camera.Viewport.X / minorAxisScrollableWidth;
					float viewportMinorAxisLine = camera.ViewportSize.X * viewportOriginRelatingToScrollableWidth;
					float textureMinorAxisLine = textureSize.X * viewportOriginRelatingToScrollableWidth;
					baseTextureDrawPositionX = -(viewportMinorAxisLine - textureMinorAxisLine) + camera.Viewport.X;
				}
				else
				{
					baseTextureDrawPositionX = camera.Viewport.X;
				}

				backgroundTexture.Draw(new Vector2(baseTextureDrawPositionX, baseTextureDrawPositionY), Color.White);

				foreach (Vector2 textureDrawPosition in GetTextureTileDrawPoints(new Vector2(baseTextureDrawPositionX, baseTextureDrawPositionY)))
				{
					backgroundTexture.Draw(textureDrawPosition, Color.White);
				}
			}
			else if (ScrollDirection == BackgroundScrollDirection.Both)
			{
				float baseTextureDrawPositionX = camera.Viewport.X + -Delta.X;
				float baseTextureDrawPositionY = camera.Viewport.Y + -Delta.Y;

				backgroundTexture.Draw(new Vector2(baseTextureDrawPositionX, baseTextureDrawPositionY), Color.White);

				foreach (Vector2 textureDrawPosition in GetTextureTileDrawPoints(new Vector2(baseTextureDrawPositionX, baseTextureDrawPositionY)))
				{
					backgroundTexture.Draw(textureDrawPosition, Color.White);
				}
			}
		}

		/// <summary>
		///   Returns the serializable objects for this layer.
		/// </summary>
		/// <returns>
		///   An anonymous object containing the background texture's resource
		///   name, and the layer's scroll direction and rate.
		/// </returns>
		[Obsolete]
		public object GetSerializableObjects()
		{
			return new
			{
				resourceName = BackgroundTextureResourceName,
				scrollDirection = (int)ScrollDirection,
				scrollRate = ScrollRate
			};
		}

		/// <summary>
		///   Initializes this layer.
		/// </summary>
		public void Initialize()
		{
			backgroundTexture = ContentPackageManager.GetGraphicsResource(BackgroundTextureResourceName);
		}

		/// <summary>
		///   Loads the content of this layer.
		/// </summary>
		public void LoadContent()
		{
			if (!isContentLoaded)
			{
				if (backgroundTexture == null)
				{
					throw new InvalidOperationException("BackgroundLayer.LoadContent(): Tried to load content before initializing the object. Please call Initialize() first.");
				}

				backgroundTexture.LoadContent();
				textureSize = backgroundTexture.GetSize();

				isContentLoaded = true;
			}
		}

		/// <summary>
		///   Returns a JSON string containing this layer.
		/// </summary>
		/// <returns>A JSON string containing this layer.</returns>
		[Obsolete]
		public string Serialize()
		{
			return JObject.FromObject(GetSerializableObjects()).ToString();
		}

		/// <summary>
		///   Updates this layer's position based on the camera's movement.
		/// </summary>
		public void Update()
		{
			if (ScrollDirection == BackgroundScrollDirection.Fixed)
			{
				return;
			}
			else
			{
				Vector2 cameraDisplacement = camera.Position - oldCameraPosition;
				if (ScrollDirection == BackgroundScrollDirection.Horizontal)
				{
					Delta = new Vector2(Delta.X + (cameraDisplacement.X * ScrollRate), 0f);
				}
				else if (ScrollDirection == BackgroundScrollDirection.Vertical)
				{
					Delta = new Vector2(0f, Delta.Y + (cameraDisplacement.Y * ScrollRate));
				}
				else if (ScrollDirection == BackgroundScrollDirection.Both)
				{
					Delta = new Vector2(Delta.X + (cameraDisplacement.X * ScrollRate), Delta.Y + (cameraDisplacement.Y * ScrollRate));
				}
			}

			oldCameraPosition = camera.Position;
			backgroundTexture.Update();
		}

		/// <summary>
		///   Returns a list of all the points to which to draw the tiled
		///   background texture.
		/// </summary>
		/// <param name="baseTextureDrawPosition">
		///   A single point where the texture will be drawn.
		/// </param>
		/// <returns>A list of vectors.</returns>
		private List<Vector2> GetTextureTileDrawPoints(Vector2 baseTextureDrawPosition)
		{
			Vector2 topLeftCorner = baseTextureDrawPosition;
			while (topLeftCorner.X > camera.Viewport.X) { topLeftCorner.X -= textureSize.X; }
			while (topLeftCorner.Y > camera.Viewport.Y) { topLeftCorner.Y -= textureSize.Y; }

			Vector2 bottomRightCorner = baseTextureDrawPosition;
			while (bottomRightCorner.X < camera.Viewport.Right) { bottomRightCorner.X += textureSize.X; }
			while (bottomRightCorner.Y < camera.Viewport.Bottom) { bottomRightCorner.Y += textureSize.Y; }

			List<Vector2> result = new List<Vector2>();
			for (float x = topLeftCorner.X; x < bottomRightCorner.X; x += textureSize.X)
			{
				for (float y = topLeftCorner.Y; y < bottomRightCorner.Y; y += textureSize.Y)
				{
					result.Add(new Vector2(x, y));
				}
			}

			return result;
		}
	}
}
