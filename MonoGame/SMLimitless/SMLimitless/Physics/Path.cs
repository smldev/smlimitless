//-----------------------------------------------------------------------
// <copyright file="Path.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;

namespace SMLimitless.Physics
{
    /// <summary>
    /// A collection of points that creates
    /// a set of lines that objects can follow.
    /// </summary>
    public sealed class Path
    {
        /// <summary>
        /// The object currently attached to this path.
        /// This contains a null reference if no object is attached.
        /// </summary>
        private IPositionable pathingObject;

		/// <summary>
		/// Gets or sets a collection of the points representing this path.
		/// </summary>
		internal List<Vector2> Points { get; set; }

		/// <summary>
        /// An event notifying an attached object that it has reached
        /// one of the points and should change its velocity to continue
        /// along the path.
        /// </summary>
        public event PointNotifierEventHandler PointNotifierEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Path"/> class.
        /// </summary>
        /// <param name="pathingObject">An object to attach to this path.</param>
        public Path(IPositionable pathingObject)
        {
			Points = new List<Vector2>();
            this.pathingObject = pathingObject;
        }

        /// <summary>
        /// Calculates a vector with component between 0 and 1,
        /// based on the angle between two given points, to ensure that
        /// velocity is distributed such that an attached object will follow that path.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>A vector with components between 0 and 1.</returns>
        private Vector2 CalculateVelocityMultiplier(Vector2 a, Vector2 b)
        {
            // Paths are made of a collection of points represented by Vector2s.
            // Objects can follow a path using an anchor point, which is a point on the object used to move it along the path.
            // Objects following a path have a base velocity stored as a float. They also have a translated velocity that helps them follow the path.
            // Between any two points there is an angle. This angle can be used to derive the translated velocity of the object.
            // Given two points, a and b, the first step is to determine where b is in relation to a.
            // If b is to the right of a, the translated velocity's X component will be positive; if b is to the left of a, it'll be negative.
            // Likewise for the Y component - if b is above a, the component is negative; if b is below a, the component is positive.
            // The translated velocity is made from multiplying the base velocity with a number between 0 and 1 for each component.
            // This number is called the multiplier, and it's calculated based on the angle between the two points along the path.
            // If, for example, b is directly above a (the X coordinates are equal), then all the velocity will be straight up.
            // On the other hand, if b is directly to the right of a (the Y coordinates are equal), then all the velocity will be to the right.
            // If b is exactly halfway between being above a and to the right of a, the velocity will be split in half across the components.
            // This applies to the other directions, and it results in quadrants, much like the Cartesian plane's quadrants I, II, III, and IV.
            // The overall angle doesn't matter as much as the angle in relation to the axis lines. Thus, the angle is modulus divided by 90 to get a number in that range.
            // Determining how to split up the two components of the translated velocity varies based on the quadrant.
            // Quadrant I (+X, -Y): Angles less than 45° are closer to the Y-axis, and the Y component will be larger. Angles greater than 45° will have the larger X component.
            // Quadrant II (-X, -Y): Angles less than 45° are closer to the X-axis, and the X component will be larger. Angles greater will have the larger Y component.
            // Quadrant III (-X, +Y): Angles less than 45° are closer to the Y-axis, and the Y component will be larger. Angles greater will have the larger X component.
            // Quarant IV (+X, +Y): Angles less than 45° are closer to the X-axis and the X component will be larger. Angles greater will have the larger Y component.
            // Notice that Quadrants I and III are the same, and so are Quadrants II and IV. We can set up a two-case conditional block to help us out here.
            // We'll divide the angle by 90 to get a value between 0 and 1. We will also get a new number by subtracting the divided angle from 1.
            // These are the multipliers. The larger of the two goes to the closer axis and the smaller of the two goes to the farther axis.
            float xDirection = (b.X - a.X).Sign();
            float yDirection = (b.Y - a.Y).Sign();
            float angle = a.GetAngleBetweenVectors(b);
            float multiplierA = (angle % 90f) / 90f;
            float multiplierB = 1f - multiplierA;

            if ((xDirection <= 0f && yDirection >= 0f) || (xDirection >= 0f && yDirection <= 0f))
            {
                // The Y-axis is closer.
                if (multiplierA > multiplierB) { return new Vector2(multiplierB, multiplierA); }
                else { return new Vector2(multiplierA, multiplierB); }
            }
            else if ((xDirection <= 0f && yDirection <= 0f) || (xDirection >= 0f && yDirection >= 0f))
            {
                // The X-axis is closer.
                if (multiplierA > multiplierB) { return new Vector2(multiplierA, multiplierB); }
                else { return new Vector2(multiplierB, multiplierA); }
            }
            else
            {
                return new Vector2(float.NaN);
            }
        }

        /// <summary>
        /// Fires the point notifier event, providing a velocity multiplier.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        private void OnPointNotifier(Vector2 a, Vector2 b)
        {
            if (PointNotifierEvent != null)
            {
				PointNotifierEvent(new PointNotifierEventArgs(CalculateVelocityMultiplier(a, b)));
            }
        }

        /// <summary>
        /// Returns a list of the points in this path.
        /// </summary>
        /// <returns>A list of points in this path.</returns>
        public List<Vector2> GetPoints()
        {
            return new List<Vector2>(Points.OrderBy(p => p.X));
        }

        /// <summary>
        /// Draws the lines between the points in the path.
        /// </summary>
        /// <param name="color">The color to draw the lines.</param>
        public void Draw(Color color)
        {
            // Draw lines between the points, from the leftmost to the rightmost.
            var sortedPoints = Points.OrderBy(p => p.X).ToList();

            for (int i = 0; i < sortedPoints.Count - 1; i++)
            {
                GameServices.SpriteBatch.DrawLine(1f, color, sortedPoints[i], sortedPoints[i + 1]);
            }
        }

        /// <summary>
        /// Updates an attached object along this path.
        /// </summary>
        public void Update() { }

        /// <summary>
        /// Returns the serializable objects for this path.
        /// </summary>
        /// <returns>An anonymous object containing a list of points.</returns>
        public object GetSerializableObjects()
        {
            return new
            {
                points = Points.Serialize(),
            };
        }

        /// <summary>
        /// Returns the serialized form of this path.
        /// </summary>
        /// <returns>A JSON string containing this path's data.</returns>
        public string Serialize()
        {
            return JObject.FromObject(GetSerializableObjects()).ToString();
        }

        /// <summary>
        /// Sets the points in this object.
        /// </summary>
        /// <param name="json">A JSON string containing a valid Path state.</param>
        public void Deserialize(string json)
        {
            JObject obj = JObject.Parse(json);
            JArray points = (JArray)obj["points"];

            foreach (var point in points)
            {
				Points.Add(point.ToVector2());
            }
        }
    }

    /// <summary>
    /// A delegate for the PointNotifier event.
    /// </summary>
    /// <param name="e">Event arguments containing the new velocity multiplier for the attached object.</param>
    public delegate void PointNotifierEventHandler(PointNotifierEventArgs e);

	/// <summary>
	/// Arguments for the <see cref="PointNotifierEventHandler"/>.
	/// </summary>
	public sealed class PointNotifierEventArgs : EventArgs
	{
		/// <summary>
		/// Gets a value indicating the new multiplier to apply to the object's path velocity.
		/// </summary>
		public Vector2 NewVelocityMultiplier { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PointNotifierEventArgs"/> class.
		/// </summary>
		/// <param name="newVelocityMultiplier">The value indicating the new multiplier to apply to the object's path velocity.</param>
		public PointNotifierEventArgs(Vector2 newVelocityMultiplier)
		{
			NewVelocityMultiplier = newVelocityMultiplier;
		}
	}
}
