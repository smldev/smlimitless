using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;

namespace SMLimitless.Physics
{
	/// <summary>
	///   A system that moves a <see cref="Camera2D" /> instance around a <see
	///   cref="Sprites.Collections.Section" /> instance.
	/// </summary>
	public sealed class CameraSystem
	{
		private const float ActiveBoundsFactor = 1.2f;
		private const float ZoomInDistanceBoundary = 0.3f;
		private const float ZoomOutDistanceBoundary = 0.1f;
		private const float ZoomRate = 0.33f;
		// the distance a tracking object has to be from the edge of the viewport
		// before the system zooms out the distance a tracking object has to be
		// from the edge of the viewport before the system zooms in

		private static PhysicsSetting<float> MaximumZoomFactor;
		private static PhysicsSetting<float> MiddleScreenBandHeight;    // expressed as a total percentage of the screen
		private Camera2D camera;
		private bool objectOutsideOfZoomOutBoundary = false;
		private BoundingRectangle totalBounds;
		// this flag is set if at least one object is outside the zoom out boundary

		/// <summary>
		///   Gets a rectangle in which game objects such as tiles and sprites
		///   are active.
		/// </summary>
		public BoundingRectangle ActiveBounds { get; private set; }

		/// <summary>
		///   Gets or sets a value indicating whether the camera cannot scroll or zoom.
		/// </summary>
		public bool IsFrozen { get; set; } = false;

		/// <summary>
		///   Gets or sets a value indicating whether the camera's viewport
		///   should stay within bounds.
		/// </summary>
		public bool StayInBounds { get; set; } = true;

		internal List<IPositionable2> TrackingObjects { get; private set; }

		static CameraSystem()
		{
			MiddleScreenBandHeight = new PhysicsSetting<float>("Camera: Middle Screen Band Height", 0.01f, 0.99f, 0.33f, PhysicsSettingType.FloatingPoint);
			MaximumZoomFactor = new PhysicsSetting<float>("Camera: Maximum Zoom Factor", 1f, 2.5f, 1.5f, PhysicsSettingType.FloatingPoint);
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="CameraSystem" /> class.
		/// </summary>
		/// <param name="camera">A camera for the system to move.</param>
		/// <param name="totalBounds">
		///   The bounds of the <see cref="Sprites.Collections.Section" /> instance.
		/// </param>
		/// <param name="trackingObjects">
		///   A collection of objects that the camera should follow.
		/// </param>
		public CameraSystem(Camera2D camera, BoundingRectangle totalBounds, params IPositionable2[] trackingObjects)
		{
			if (camera == null) { throw new ArgumentNullException(nameof(camera), "Cannot create a camera system with a null camera."); }
			if (totalBounds.IsNaN() || totalBounds.Width <= 0f || totalBounds.Height <= 0f) { throw new ArgumentException($"The total bounds rectangle for the camera system has a zero or negative area. X: {totalBounds.X}, Y: {totalBounds.Y}, Width: {totalBounds.Width}, Height: {totalBounds.Height}", nameof(totalBounds)); }

			this.camera = camera;
			this.totalBounds = totalBounds;
			TrackingObjects = trackingObjects.ToList();

			this.camera.Zoom = 1f;
			this.camera.Rotation = 0f;
			ActiveBounds = CreateActiveBounds(this.camera.Viewport);
		}

		/// <summary>
		///   Draws the total bounds of the camera system as a green rectangle,
		///   plus optional debug information.
		/// </summary>
		/// <param name="debug">
		///   A value indicating whether debug information should be displayed.
		/// </param>
		public void Draw(bool debug)
		{
			if (!StayInBounds)
			{
				GameServices.SpriteBatch.DrawRectangleEdges(totalBounds.ToRectangle(), Color.LightGreen);
			}

			if (debug)
			{
				string debugText = $"Camera Pos: {camera.Position}, Zoom: {camera.Zoom}, Objcount: {TrackingObjects.Count}";
				GameServices.DebugFont.DrawString(debugText, camera.Position + new Vector2(16f, 32f), 1f / camera.Zoom);

				Vector2 cameraCenter = GetCenterOfAllTrackingObjects();
				GameServices.SpriteBatch.DrawRectangle(new Rectangle((int)(cameraCenter.X - 4f), (int)(cameraCenter.Y - 4f), 8, 8), Color.Red);
				GameServices.SpriteBatch.DrawRectangleEdges(CreateInsetRectangle(camera.Viewport, ZoomOutDistanceBoundary).ToRectangle(), Color.Red);
				GameServices.SpriteBatch.DrawRectangleEdges(CreateInsetRectangle(camera.Viewport, ZoomInDistanceBoundary).ToRectangle(), Color.Green);
			}
		}

		/// <summary>
		///   Updates this camera system.
		/// </summary>
		public void Update()
		{
			if (IsFrozen) { return; }

			float delta = GameServices.GameTime.GetElapsedSeconds();

			// Calculate the center point of all tracking objects.
			Vector2 center = GetCenterOfAllTrackingObjects();
			if (center.IsNaN()) { center = Vector2.Zero; }

			// Determine if any tracking objects have exited the zoom-out
			// distance boundary and thus if we need to zoom out.
			if (ZoomOutRequired())
			{
				camera.Zoom = MathHelper.Clamp((camera.Zoom - (ZoomRate * delta)), (1f / MaximumZoomFactor.Value), 1f);
			}
			else if (ZoomInRequired())
			{
				camera.Zoom = MathHelper.Clamp((camera.Zoom + (ZoomRate * delta)), (1f / MaximumZoomFactor.Value), 1f);
			}

			// If the collision debugger is active, we need to zoom in really close.
			const float collisionDebugMaxZoomFactorMultiplier = 8f;
			if (GameServices.CollisionDebuggerActive && TrackingObjects.Any() && !(TrackingObjects.First().GetType().FullName.Contains("Debug")))
			{
				camera.Zoom *= collisionDebugMaxZoomFactorMultiplier;
			}

			// Set up some variables to determine where we should be horizontally
			// and vertically.
			float viewportCenterX = camera.Viewport.Center.X;
			float viewportTopOfMiddleBand = camera.Viewport.Top + (camera.Viewport.Height * (MiddleScreenBandHeight.Value));
			float viewportBottomOfMiddleBand = camera.Viewport.Bottom - (camera.Viewport.Height * (MiddleScreenBandHeight.Value));

			// Determine the distance the camera needs to move horizontally.
			float cameraXTranslation = center.X - viewportCenterX;
			float cameraYTranslation = 0f;

			if (center.Y < viewportTopOfMiddleBand)
			{
				cameraYTranslation = center.Y - viewportTopOfMiddleBand;
			}
			else if (center.Y > viewportBottomOfMiddleBand)
			{
				cameraYTranslation = center.Y - viewportBottomOfMiddleBand;
			}

			// Calculate the new origin of the camera viewport.
			Vector2 newCameraOrigin = Vector2.Zero;
			float cameraOriginMaxX = totalBounds.Right - camera.Viewport.Width;
			float cameraOriginMaxY = totalBounds.Bottom - camera.Viewport.Height;
			if (StayInBounds)
			{
				newCameraOrigin.X = MathHelper.Clamp(camera.Position.X + cameraXTranslation, totalBounds.X, cameraOriginMaxX);
				newCameraOrigin.Y = MathHelper.Clamp(camera.Position.Y + cameraYTranslation, totalBounds.Y, cameraOriginMaxY);
			}
			else
			{
				newCameraOrigin.X = camera.Position.X + cameraXTranslation;
				newCameraOrigin.Y = camera.Position.Y + cameraYTranslation;
			}

			// Move the camera.
			camera.Position = newCameraOrigin;

			// Update the ActiveBounds.
			ActiveBounds = CreateActiveBounds(camera.Viewport);
		}

		private static BoundingRectangle CreateActiveBounds(BoundingRectangle viewport)
		{
			float activeBoundsMultiple = ActiveBoundsFactor - 1f;

			BoundingRectangle result = viewport;
			result.X -= (result.Width * activeBoundsMultiple);
			result.Y -= (result.Height * activeBoundsMultiple);
			result.Width += (result.Width * (activeBoundsMultiple * 2f));
			result.Height += (result.Height * (activeBoundsMultiple * 2f));

			return result;
		}

		private static BoundingRectangle CreateInsetRectangle(BoundingRectangle rect, float factor)
		{
			BoundingRectangle result = rect;
			result.X += (result.Width * (factor / 2f));
			result.Y += (result.Height * (factor / 2f));
			result.Width -= (result.Width * factor);
			result.Height -= (result.Height * factor);

			return result;
		}

		private static Vector2 GetObjectCenter(IPositionable2 trackingObject)
		{
			return trackingObject.Position + (trackingObject.Size / 2f);
		}

		private bool CameraAtEdges()
		{
			if (!StayInBounds) { return false; }
			return camera.Viewport.X == totalBounds.X || camera.Viewport.Y == totalBounds.Y ||
				   camera.Viewport.Right == totalBounds.Right || camera.Viewport.Bottom == totalBounds.Bottom;
		}

		private Vector2 GetCenterOfAllTrackingObjects()
		{
			Vector2 sum = Vector2.Zero;

			foreach (IPositionable2 trackingObject in TrackingObjects)
			{
				Vector2 center = GetObjectCenter(trackingObject);
				sum += center;
			}

			sum /= TrackingObjects.Count;
			return sum;
		}

		private bool ZoomInRequired()
		{
			return !objectOutsideOfZoomOutBoundary;
		}

		private bool ZoomOutRequired()
		{
			if (TrackingObjects.Count <= 1 || GameServices.CollisionDebuggerActive) { return false; }

			bool result = false;

			BoundingRectangle zoomOutBoundary = CreateInsetRectangle(camera.Viewport, ZoomOutDistanceBoundary);
			List<RectangularSpaceDivision> centerPointRelations = new List<RectangularSpaceDivision>(TrackingObjects.Count);
			foreach (var trackingObject in TrackingObjects)
			{
				Vector2 objectCenter = GetObjectCenter(trackingObject);
				centerPointRelations.Add(zoomOutBoundary.GetPointRelation(objectCenter));
				BoundingRectangle objectBounds = new BoundingRectangle(trackingObject.Position.X, trackingObject.Position.Y, trackingObject.Size.X, trackingObject.Size.Y);
				if (!zoomOutBoundary.IntersectsIncludingEdges(objectCenter) && camera.Zoom >= 1f / MaximumZoomFactor.Value &&
				(totalBounds.GetIntersectionDepth(objectBounds).Abs().GreaterThan(ZoomOutDistanceBoundary)))
				{
					result = true;
				}
			}

			result = result && centerPointRelations.Any(c => c != centerPointRelations[0]);    // don't zoom out if all objects are on the same side

			objectOutsideOfZoomOutBoundary = result;
			return result;
		}
	}
}
