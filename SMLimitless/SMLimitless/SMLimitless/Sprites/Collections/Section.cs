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
using SMLimitless.Sprites.Collections.Structures;

namespace SMLimitless.Sprites.Collections
{
    /// <summary>
    /// A part of a level.
    /// </summary>
    public sealed class Section : ISerializable
    {
        public Level Owner { get; private set; }

        public int Index { get; set; }
        public string Name { get; set; }
        public BoundingRectangle Bounds { get; private set; }
        public Camera2D Camera { get; private set; }
        public CameraScrollType ScrollType { get; private set; }

        private Vector2 autoscrollSpeed;
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

        private string autoscrollPathName;
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

        public Background Background { get; private set; }

        public QuadTree QuadTree { get; private set; }

        internal Layer MainLayer { get; private set; }
        private List<Layer> layers;
        private List<Sprite> sprites;
        private List<Path> paths;

        public Section(BoundingRectangle bounds, Level owner)
        {
            // TODO: temporary
            this.Bounds = bounds;
            this.Camera = new Camera2D();
            this.Owner = owner;
            this.QuadTree = new QuadTree(GameServices.QuadTreeCellSize);
            this.layers = new List<Layer>();
            this.sprites = new List<Sprite>();
            this.paths = new List<Path>();
            this.Background = new Background(this);
        }

        public void MoveCamera(Vector2 offset)
        {
            this.Camera.Position += offset;
        }

        public void Initialize()
        {
  
        }

        public void LoadContent() 
        {
            this.Background.LoadContent();
        }

        public void Update()
        {
            this.Background.Update();
        }

        public void Draw()
        {
            this.Background.Draw();
        }

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
                bounds = this.Bounds.ToSimpleString(),
                scrollType = (int)this.ScrollType,
                autoscrollSpeed = (this.ScrollType == CameraScrollType.AutoScroll) ? this.autoscrollSpeed : new Vector2(float.NaN),
                autoscrollPathName = (this.ScrollType == CameraScrollType.AutoScrollAlongPath) ? this.autoscrollPathName : null,
                background = this.Background.GetSerializableObjects(),
                layers = layerObjects,
                sprites = spriteObjects,
                paths = pathObjects
            };
        }

        public string Serialize()
        {
            return JObject.FromObject(this.GetSerializableObjects()).ToString();
        }

        public void Deserialize(string json)
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
                sprite.Initialize(this.Owner);
                this.sprites.Add(sprite);
            }

            foreach (var pathData in pathsData)
            {
                Path path = new Path(null);
                path.Deserialize(pathData.ToString());
                this.paths.Add(path);
            }
        }

        public void CreateTestSection()
        {
            this.Background.CreateTestBackground();

            Tile tile = Assemblies.AssemblyManager.GetTileByFullName("SmlSample.TestTile");
            Sprite sprite = Assemblies.AssemblyManager.GetSpriteByFullName("SmlSample.TestSprite");

            tile.Initialize(null, "hello");
            tile.Position = new Vector2(654f, 290348f);
            sprite.Initialize(null);
            sprite.Position = new Vector2(48958f, 0f);

            Layer layer = new Layer(this);
            layer.AddTile(tile);
            this.sprites.Add(sprite);
            this.layers.Add(layer);
        }
    }
}
