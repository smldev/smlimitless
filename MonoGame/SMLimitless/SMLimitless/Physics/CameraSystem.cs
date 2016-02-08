using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;

namespace SMLimitless.Physics
{
	public sealed class CameraSystem
	{
		private const float ActiveBoundsFactor = 1.2f;
		private const float ZoomRate = 2f;								// two factors of zoom per second
		private const float ZoomOutDistanceBoundary = 0.1f;             // the distance a tracking object has to be from the edge of the viewport before the system zooms out
		private const float ZoomInDistanceBoundary = 0.3f;				// the distance a tracking object has to be from the edge of the viewport before the system zooms in

		private static PhysicsSetting<float> MiddleScreenBandHeight;    // expressed as a total percentage of the screen
		private static PhysicsSetting<float> MaximumZoomFactor;
		
		private Camera2D camera;
		private BoundingRectangle totalBounds;
		private List<IPositionable2> trackingObjects;
		
		/// <summary>
		/// Gets a rectangle in which game objects such as tiles and sprites are active.
		/// </summary>
		public BoundingRectangle ActiveBounds { get; private set; }

		static CameraSystem()
		{
			MiddleScreenBandHeight = new PhysicsSetting<float>("Camera: Middle Screen Band Height", 0.01f, 0.99f, 0.33f, PhysicsSettingType.FloatingPoint);
			MaximumZoomFactor = new PhysicsSetting<float>("Camera: Maximum Zoom Factor", 1f, 2.5f, 1.5f, PhysicsSettingType.FloatingPoint);
		}

		public CameraSystem(Camera2D camera, BoundingRectangle totalBounds, params IPositionable2[] trackingObjects)
		{
			if (camera == null) { throw new ArgumentNullException(nameof(camera), "Cannot create a camera system with a null camera."); }
			if (totalBounds.IsNaN() || totalBounds.Width <= 0f || totalBounds.Height <= 0f) { throw new ArgumentException($"The total bounds rectangle for the camera system has a zero or negative area. X: {totalBounds.X}, Y: {totalBounds.Y}, Width: {totalBounds.Width}, Height: {totalBounds.Height}", nameof(totalBounds)); }
			if (trackingObjects.Length == 0) { throw new ArgumentException("The provided collection of camera tracking objects had no objects.", nameof(trackingObjects)); }

			this.camera = camera;
			this.totalBounds = totalBounds;
			this.trackingObjects = trackingObjects.ToList();

			this.camera.Zoom = 1f;
			this.camera.Rotation = 0f;
			ActiveBounds = CreateActiveBounds(this.camera.Viewport);
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
			result.X += (result.Width * factor);
			result.Y += (result.Width * factor);
			result.Width -= (result.Width * (factor * 2f));
			result.Height -= (result.Height * (factor * 2f));

			return result;
		}

		private Vector2 GetCenterOfAllTrackingObjects()
		{
			Vector2 sum = Vector2.Zero;

			foreach (IPositionable2 trackingObject in trackingObjects)
			{
				Vector2 center = GetObjectCenter(trackingObject);
				sum += center;
			}

			sum /= trackingObjects.Count;
			return sum;
		}

		private bool ZoomOutRequired()
		{
			if (trackingObjects.Count == 1) { return false; }

			BoundingRectangle zoomOutBoundary = CreateInsetRectangle(camera.Viewport, ZoomOutDistanceBoundary);
			foreach (var trackingObject in trackingObjects)
			{
				Vector2 objectCenter = GetObjectCenter(trackingObject);
				BoundingRectangle objectBounds = new BoundingRectangle(trackingObject.Position.X, trackingObject.Position.Y, trackingObject.Size.X, trackingObject.Size.Y);
				if (!zoomOutBoundary.IntersectsIncludingEdges(objectCenter) && camera.Zoom >= 1f / MaximumZoomFactor.Value &&
				(totalBounds.GetIntersectionDepth(objectBounds).Abs().GreaterThan(ZoomOutDistanceBoundary)))
				{
					return true;
				}
			}

			return false;
		}

		private static Vector2 GetObjectCenter(IPositionable2 trackingObject)
		{
			return trackingObject.Position + (trackingObject.Size / 2f);
		}

		private bool CameraAtEdges()
		{
			return camera.Viewport.X == totalBounds.X || camera.Viewport.Y == totalBounds.Y ||
				   camera.Viewport.Right == totalBounds.Right || camera.Viewport.Bottom == totalBounds.Bottom;
		}

		private bool ZoomInRequired()
		{
			BoundingRectangle zoomInBoundary = CreateInsetRectangle(camera.Viewport, ZoomInDistanceBoundary);
			foreach (var trackingObject in trackingObjects)
			{
				Vector2 objectCenter = GetObjectCenter(trackingObject);
				if (!zoomInBoundary.IntersectsIncludingEdges(objectCenter) && camera.Zoom <= 1f)
				{
					return true;
				}
			}
			return false;
		}

		public void Update()
		{
			float delta = GameServices.GameTime.GetElapsedSeconds();

			// Calculate the center point of all tracking objects.
			Vector2 center = GetCenterOfAllTrackingObjects();

			// Determine if any tracking objects have exited the zoom-out distance boundary
			// and thus if we need to zoom out.
			if (ZoomOutRequired())
			{
				camera.Zoom -= ZoomRate * delta;
			}
			else if (ZoomInRequired())
			{
				camera.Zoom += ZoomRate * delta;
			}

			// Set up some variables to determine where we should be horizontally and vertically.
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
			newCameraOrigin.X = MathHelper.Clamp(camera.Position.X + cameraXTranslation, totalBounds.X, cameraOriginMaxX);
			newCameraOrigin.Y = MathHelper.Clamp(camera.Position.Y + cameraYTranslation, totalBounds.Y, cameraOriginMaxY);

			// Move the camera.
			camera.Position = newCameraOrigin;
		}
	}
}
