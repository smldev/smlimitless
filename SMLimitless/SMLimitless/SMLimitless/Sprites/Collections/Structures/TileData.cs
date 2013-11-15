using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SMLimitless.Sprites.Collections.Structures
{
    public class TileData
    {
        public string QualifiedTypeName { get; private set; }
        public bool IsNormalTile { get; private set; }
        public TileCollisionType CollisionType { get; private set; }
        public string Name { get; private set; }
        public string GraphicsResourceName { get; private set; }
        public Vector2 Position { get; private set; }
        public string InitialState { get; private set; }
    }
}
