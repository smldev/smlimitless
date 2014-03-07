//-----------------------------------------------------------------------
// <copyright file="Resolution.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SMLimitless.Physics
{
    /// <summary>
    /// Represents a collision resolution between a sprite and a tile.
    /// </summary>
    [Obsolete]
    public struct Resolution
    {
        /// <summary>
        /// The backing field for the ResolutionDistance property.
        /// </summary>
        private Vector2 resolution;

        /// <summary>
        /// A backing field for the Type property.
        /// </summary>
        private ResolutionType type;

        /// <summary>
        /// Gets the zero resolution.
        /// </summary>
        public static Resolution Zero
        {
            get
            {
                return new Resolution();
            }
        }

        /// <summary>
        /// Gets or sets the distance of this collision resolution.
        /// </summary>
        public Vector2 ResolutionDistance
        {
            get
            {
                return this.resolution;
            }

            set
            {
                this.resolution = value;

                if (value == Vector2.Zero)
                {
                    this.type = ResolutionType.None;
                }
            }
        }

        /// <summary>
        /// Gets or sets the resolution type of this collision resolution.
        /// </summary>
        public ResolutionType Type
        {
            get
            {
                return this.type;
            }

            set
            {
                if (value == ResolutionType.None && this.resolution != Vector2.Zero)
                {
                    throw new Exception("Resolution.Type.set: Tried to set a non-zero resolution's type to None.");
                }
                else if (value != ResolutionType.None && this.resolution == Vector2.Zero)
                {
                    throw new Exception("Resolution.Type.set: Tried to set a zero resolution's type to non-zero.");
                }

                this.type = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resolution"/> struct.
        /// </summary>
        /// <param name="resolution">The distance of the resolution.</param>
        public Resolution(Vector2 resolution)
        {
            this.resolution = resolution;
            this.type = (resolution == Vector2.Zero) ? ResolutionType.None : ResolutionType.Normal;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resolution"/> struct.
        /// </summary>
        /// <param name="resolution">The distance of the resolution.</param>
        /// <param name="type">The type of the resolution.</param>
        public Resolution(Vector2 resolution, ResolutionType type)
        {
            this.resolution = resolution;

            if (resolution == Vector2.Zero && type != ResolutionType.None)
            {
                throw new Exception("Resolution.ctor(Vector2, ResolutionType): Tried to set a zero resolution to a non-zero resolution type.");
            }
            else if (resolution != Vector2.Zero && type == ResolutionType.None)
            {
                throw new Exception("Resolution.ctor(Vector2, ResolutionType): Tried to set a non-zero resolution to a zero resolution type.");
            }

            this.type = type;
        }

        /// <summary>
        /// Compares two resolutions for equality.
        /// </summary>
        /// <param name="left">The first resolution to compare.</param>
        /// <param name="right">The second resolution to compare.</param>
        /// <returns>True if the resolutions are equal, false if otherwise.</returns>
        public static bool operator ==(Resolution left, Resolution right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two resolutions for inequality.
        /// </summary>
        /// <param name="left">The first resolution to compare.</param>
        /// <param name="right">The second resolution to compare.</param>
        /// <returns>True if the resolutions are not equal, false if otherwise.</returns>
        public static bool operator !=(Resolution left, Resolution right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines if a given object is equal to this resolution.
        /// </summary>
        /// <param name="obj">The object to check for equality.</param>
        /// <returns>True if the object is equal to this resolution, false if otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Resolution))
            {
                return false;
            }

            Resolution other = (Resolution)obj;
            return (this.ResolutionDistance == other.ResolutionDistance) && (this.Type == other.Type);
        }

        /// <summary>
        /// Returns a fairly unique number that identifies this resolution.
        /// </summary>
        /// <returns>A fairly unique number.</returns>
        public override int GetHashCode()
        {
            return this.ResolutionDistance.GetHashCode() ^ this.Type.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of this resolution's value.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return string.Format("Distance: {0}, Type: {1}", this.ResolutionDistance, this.Type);
        }

        /// <summary>
        /// Checks if a given resolution is equal to this one.
        /// </summary>
        /// <param name="other">The other resolution to check.</param>
        /// <returns>True if the two resolutions are equal, false if otherwise.</returns>
        private bool Equals(Resolution other)
        {
            return (this.ResolutionDistance == other.ResolutionDistance) && (this.Type == other.Type);
        }
    }
}
