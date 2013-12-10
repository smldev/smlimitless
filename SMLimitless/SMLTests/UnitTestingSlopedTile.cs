﻿//-----------------------------------------------------------------------
// <copyright file="UnitTestingSlopedTile.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Physics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;

namespace SMLTests
{
    /// <summary>
    /// A sloped tile with as few dependencies as possible,
    /// to be used in unit testing.
    /// </summary>
    public class UnitTestingSlopedTile : SlopedTile
    {
        /// <summary>
        /// Gets the name of the category that this tile is
        /// categorized within in the level editor.
        /// </summary>
        public override string EditorCategory
        {
            get { return "Testing"; }
        }

        /// <summary>
        /// Initializes this tile.
        /// </summary>
        /// <param name="owner">The Level that owns this tile.</param>
        public override void Initialize(Level owner, string contentResourceName)
        {
            base.Initialize(owner, contentResourceName);
            this.Size = new Vector2(16f, 16f);
            this.SlopedSides = RtSlopedSides.TopLeft;
        }

        /// <summary>
        /// Loads the content for this tile.
        /// </summary>
        public override void LoadContent()
        {
        }

        /// <summary>
        /// Updates this tile.
        /// </summary>
        public override void Update()
        {
        }

        /// <summary>
        /// Draws this tile.
        /// </summary>
        public override void Draw()
        {
        }

        /// <summary>
        /// Handles a collision between this tile and a sprite.
        /// </summary>
        /// <param name="sprite">The sprite that has collided with this tile.</param>
        /// <param name="intersect">The depth of the intersection.</param>
        public override void HandleCollision(Sprite sprite, Vector2 intersect)
        {
        }

        public override void DeserializeCustomObjects(JsonHelper customObjects)
        {
        }

        public override object GetCustomSerializableObjects()
        {
            return null;
        }
    }
}
