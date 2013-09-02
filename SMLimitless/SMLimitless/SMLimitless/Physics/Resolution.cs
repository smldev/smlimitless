//-----------------------------------------------------------------------
// <copyright file="Resolution.cs" company="Chris Akridge">
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
    public struct Resolution
    {
        private Vector2 resolution;
        private ResolutionType type;

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

        public static Resolution Zero
        {
            get
            {
                return new Resolution();
            }
        }

        public Resolution(Vector2 resolution)
        {
            this.resolution = resolution;
            this.type = (resolution == Vector2.Zero) ? ResolutionType.None : ResolutionType.Normal;
        }

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

        public static bool operator ==(Resolution left, Resolution right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Resolution left, Resolution right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Resolution))
            {
                return false;
            }

            Resolution other = (Resolution)obj;
            return (this.ResolutionDistance == other.ResolutionDistance) && (this.Type == other.Type);
        }

        public override int GetHashCode()
        {
            return this.ResolutionDistance.GetHashCode() ^ this.Type.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Distance: {0}, Type: {1}", this.ResolutionDistance, this.Type);
        }

        private bool Equals(Resolution other)
        {
            return (this.ResolutionDistance == other.ResolutionDistance) && (this.Type == other.Type);
        }
    }
}
