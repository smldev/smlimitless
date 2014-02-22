//-----------------------------------------------------------------------
// <copyright file="Section.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.Physics;
using SMLimitless.Sprites.Assemblies;

namespace SMLimitless.Sprites.Collections
{
    /// <summary>
    /// A part of a level.
    /// </summary>
    public sealed class Section : ISerializable
    {
        /// <summary>
        /// Gets a reference to the level that contains this section.
        /// </summary>
        public Level Owner { get; private set; }

        /// <summary>
        /// Gets or sets the index number of this section.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the name of this section, or the empty string if there is no name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a rectangle representing the bounds of this section.
        /// </summary>
        public BoundingRectangle Bounds { get; private set; }

        /// <summary>
        /// Gets the camera that is used to display a part of the section.
        /// </summary>
        public Camera2D Camera { get; private set; }

        /// <summary>
        /// Gets the method the camera uses to scroll across the section.
        /// </summary>
        public CameraScrollType ScrollType { get; private set; }

        /// <summary>
        /// The backing field for the AutoscrollSpeed property.
        /// </summary>
        private Vector2 autoscrollSpeed;

        /// <summary>
        /// Gets the speed at which the camera is autoscrolling through the level.
        /// </summary>
        public Vector2 AutoscrollSpeed
        {
            get
            {
                if (this.ScrollType != CameraScrollType.AutoScroll)
                {
                    throw new Exception("Section.AutoscrollSpeed.get: Section scroll type is not autoscrolling.");
                }

                return this.autoscrollSpeed;
            }

            private set
            {
                if (this.ScrollType != CameraScrollType.AutoScroll)
                {
                    throw new Exception("Section.AutoscrollSpeed.set: Section scroll type is not autoscrolling.");
                }

                this.autoscrollSpeed = value;
            }
        }

        /// <summary>
        /// The backing field for the AutoscrollPathName property.
        /// </summary>
        private string autoscrollPathName;

        /// <summary>
        /// Gets the name of the path that the camera is following.
        /// </summary>
        public string AutoscrollPathName
        {
            get
            {
                if (this.ScrollType != CameraScrollType.AutoScrollAlongPath)
                {
                    throw new Exception("Section.AutoscrollPathName.get: Section scroll type is not autoscrolling.");
                }

                return this.autoscrollPathName;
            }

            private set
            {
                if (this.ScrollType != CameraScrollType.AutoScrollAlongPath)
                {
                    throw new Exception("Section.AutoscrollPathName.set: Section scroll type is not autoscrolling.");
                }

                this.autoscrollPathName = value;
            }
        }

        /// <summary>
        /// Gets the background of this section.
        /// </summary>
        public Background Background { get; private set; }

        /// <summary>
        /// Gets the lazy QuadTree for this section.
        /// </summary>
        public QuadTree QuadTree { get; private set; }

        /// <summary>
        /// Gets the main layer of this section.
        /// </summary>
        internal Layer MainLayer { get; private set; }

        /// <summary>
        /// A collection of all the layers in this section.
        /// </summary>
        private List<Layer> layers;

        private List<Tile> tiles;

        /// <summary>
        /// A collection of all the sprites in this section.
        /// </summary>
        private List<Sprite> sprites;

        /// <summary>
        /// A collection of all the paths in this section.
        /// </summary>
        private List<Path> paths;

        private bool isInitialized;
        private bool isContentLoaded;
        private bool isSectionLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="Section"/> class.
        /// </summary>
        /// <param name="bounds">The bounds of this section.</param>
        /// <param name="owner">The level that owns this section.</param>
        public Section(BoundingRectangle bounds, Level owner)
        {
            // TODO: temporary
            this.Bounds = bounds;
            this.Camera = new Camera2D();
            this.Owner = owner;
            this.QuadTree = new QuadTree(GameServices.QuadTreeCellSize);
            this.layers = new List<Layer>();
            this.tiles = new List<Tile>();
            this.sprites = new List<Sprite>();
            this.paths = new List<Path>();
            this.Background = new Background(this);

            GameServices.Camera = this.Camera;
        }

        /// <summary>
        /// Moves the camera by a given distance.
        /// </summary>
        /// <param name="offset">The distance to move the camera by.</param>
        public void MoveCamera(Vector2 offset)
        {
            if (!this.isSectionLoaded)
            {
                throw new Exception("Section.MoveCamera(Vector2): The section isn't loaded - the section needs to be loaded before anything can happen.");
            }

            // Ensure the camera falls within bounds.
            Vector2 newPosition = this.Camera.Position + offset;

            if (newPosition.X < 0f)
            {
                newPosition.X = 0f;
            }

            if (newPosition.Y < 0f)
            {
                newPosition.Y = 0f;
            }

            Vector2 viewportBottomRight = new Vector2(newPosition.X + this.Camera.ViewportSize.X, newPosition.Y + this.Camera.ViewportSize.Y);
            if (viewportBottomRight.X > this.Bounds.Right)
            {
                newPosition.X -= viewportBottomRight.X - this.Bounds.Right;
            }

            if (viewportBottomRight.Y > this.Bounds.Bottom)
            {
                newPosition.Y -= viewportBottomRight.Y - this.Bounds.Bottom;
            }

            this.Camera.Position = newPosition;
        }

        /// <summary>
        /// Initializes this section.
        /// </summary>
        public void Initialize()
        {
            if (!this.isInitialized)
            {
                this.Background.Initialize();
                this.isInitialized = true;
            }
        }

        /// <summary>
        /// Loads the content for this section.
        /// </summary>
        public void LoadContent() 
        {
            if (!this.isContentLoaded)
            {
                this.Background.LoadContent();
                this.tiles.ForEach(t => t.LoadContent());
                this.sprites.ForEach(s => s.LoadContent());
                this.isContentLoaded = true;
            }
        }
        
        /// <summary>
        /// Updates this section.
        /// </summary>
        public void Update()
        {
            this.Background.Update();
            this.tiles.ForEach(t => t.Update());
            this.sprites.ForEach(s => s.Update());
            this.QuadTree.Update();

            Vector2 direction = Input.InputManager.GetDirectionalInputVector() * 2f;
            this.MoveCamera(direction);

            if (Input.InputManager.IsCurrentKeyPress(Microsoft.Xna.Framework.Input.Keys.A))
            {
                this.Camera.Zoom += 0.01f;
            }
            else if (Input.InputManager.IsCurrentKeyPress(Microsoft.Xna.Framework.Input.Keys.Z))
            {
                this.Camera.Zoom -= 0.01f;
            }
            else if (Input.InputManager.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.W))
            {
                string file = Owner.Serialize();
                System.IO.File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + @"\level.txt", file);
            }
        }

        /// <summary>
        /// Draws this section to the screen.
        /// </summary>
        public void Draw()
        {
            this.Background.Draw();
            GameServices.DebugFont.DrawString(this.Camera.ViewportSize.ToString() + " " + this.Camera.Position.ToString(), new Vector2(16f) + this.Camera.Position, 2f);

            this.tiles.ForEach(t => t.Draw());
            this.sprites.ForEach(s => s.Draw());
        }

        public void AddTile(Tile tile)
        {
            if (tile == null)
            {
                throw new ArgumentNullException("Section.AddTile(Tile): Tile cannot be null.");
            }

            this.tiles.Add(tile);
            this.MainLayer.AddTile(tile);
            this.QuadTree.Add(tile);
        }

        public void RemoveTile(Tile tile)
        {
            if (this.tiles.Contains(tile))
            {
                this.tiles.Remove(tile);
                this.QuadTree.Remove(tile);
                this.layers.ForEach(l => l.RemoveTile(tile));
            }
        }

        public Tile GetTileAtPosition(Vector2 position, bool adjacentPointsAreWithin)
        {
            var tiles = this.QuadTree.GetTilesInCell(this.QuadTree.GetCellNumberAtPosition(position));

            if (tiles == null)
            {
                return null;
            }

            foreach (Tile tile in tiles)
            {
                if (tile.Hitbox.Within(position, adjacentPointsAreWithin))
                {
                    return tile;
                }
            }

            return null;
        }

        public void SetMainLayer(Layer layer)
        {
            if (this.MainLayer != null)
            {
                throw new Exception("Section.SetMainLayer(Layer): A layer tried to set itself as this section's main layer, but this section already has a main layer.");
            }

            this.MainLayer = layer;
        }

        /// <summary>
        /// Gets an anonymous object containing key objects of this section.
        /// </summary>
        /// <returns>An anonymous object.</returns>
        public object GetSerializableObjects()
        {
            List<object> layerObjects = new List<object>(this.layers.Count);
            List<object> spriteObjects = new List<object>(this.sprites.Count);
            List<object> pathObjects = new List<object>(this.paths.Count);
            this.layers.ForEach(l => layerObjects.Add(l.GetSerializableObjects()));
            this.sprites.ForEach(s => spriteObjects.Add(s.GetSerializableObjects()));
            this.paths.ForEach(p => pathObjects.Add(p.GetSerializableObjects()));

            return new
            {
                index = this.Index,
                name = this.Name,
                bounds = this.Bounds.Serialize(),
                scrollType = (int)this.ScrollType,
                autoscrollSpeed = (this.ScrollType == CameraScrollType.AutoScroll) ? this.autoscrollSpeed.Serialize() : new Vector2(float.NaN).Serialize(),
                autoscrollPathName = (this.ScrollType == CameraScrollType.AutoScrollAlongPath) ? this.autoscrollPathName : null,
                background = this.Background.GetSerializableObjects(),
                layers = layerObjects,
                sprites = spriteObjects,
                paths = pathObjects
            };
        }

        /// <summary>
        /// Returns a JSON string containing key objects of this section.
        /// </summary>
        /// <returns>A valid JSON string.</returns>
        public string Serialize()
        {
            return JObject.FromObject(this.GetSerializableObjects()).ToString();
        }

        /// <summary>
        /// Loads a section from a JSON string containing a section.
        /// </summary>
        /// <param name="json">A valid JSON string.</param>
        public void Deserialize(string json)
        {
            if (!this.isSectionLoaded)
            {
                JObject obj = JObject.Parse(json);

                // First, deserialize the root objects.
                this.Index = (int)obj["index"];
                this.Name = (string)obj["name"];
                this.Bounds = BoundingRectangle.FromSimpleString((string)obj["bounds"]);
                this.ScrollType = (CameraScrollType)(int)obj["scrollType"];
                this.autoscrollSpeed = obj["autoscrollSpeed"].ToVector2();
                this.autoscrollPathName = (string)obj["autoscrollPathName"];
                this.Background.Deserialize(obj["background"].ToString());

                // Next, deserialize the nested objects.
                JArray layersData = (JArray)obj["layers"];
                JArray spritesData = (JArray)obj["sprites"];
                JArray pathsData = (JArray)obj["paths"];

                foreach (var layerData in layersData)
                {
                    Layer layer = new Layer(this);
                    layer.Deserialize(layerData.ToString());
                    layer.Initialize();
                    this.layers.Add(layer);
                }

                foreach (var spriteData in spritesData)
                {
                    string typeName = (string)spriteData["typeName"];
                    Sprite sprite = AssemblyManager.GetSpriteByFullName(typeName);
                    sprite.Deserialize(spriteData.ToString());
                    sprite.Initialize(this);
                    this.sprites.Add(sprite);
                    this.QuadTree.Add(sprite);
                }

                foreach (var pathData in pathsData)
                {
                    Path path = new Path(null);
                    path.Deserialize(pathData.ToString());
                    this.paths.Add(path);
                }

                this.isSectionLoaded = true;
            }
        }
    }
}
