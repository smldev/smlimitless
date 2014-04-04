using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Collections;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Sprites.Collections
{
    public sealed class GridLayer : IName, IPositionable, ISerializable
    {
        private SizedGrid<Tile> tileGrid;
        private List<Tile> tiles;
        private bool isLoaded;
        private bool isMainLayer;
        public bool IsActive { get; set; }
        public Section Owner { get; private set; }
        public int Index { get; private set; }
        public string Name { get; private set; }
        public Vector2 InitialPosition { get; private set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        public Vector2 Size
        {
            get
            {
                if (this.tileGrid != null)
                {
                    return new Vector2((this.tileGrid.CellWidth * this.tileGrid.Width), (this.tileGrid.CellHeight * this.tileGrid.Height));
                }
                return Vector2.Zero;
            }
        }

        public bool IsMainLayer
        {
            get
            {
                return this.isMainLayer;
            }
            set
            {
                this.isMainLayer = value;
                if (value)
                {
                    this.Owner.SetMainLayer(null); // TODO: change "null" to "this"
                }
            }
        }

        public GridLayer(Section owner)
        {
            this.Owner = owner;
            this.IsActive = true;
            this.tiles = new List<Tile>();
        }

        public string Serialize()
        {
            return JObject.FromObject(this.GetSerializableObjects()).ToString();
        }

        public object GetSerializableObjects()
        {
            List<object> tileObjects = new List<object>(this.tiles.Count);
            this.tiles.ForEach(t => tileObjects.Add(t.GetSerializableObjects()));

            return new
            {
                index = this.Index,
                name = this.Name,
                isMainLayer = this.IsMainLayer,
                position = this.InitialPosition.Serialize(),
                gridSize = new Vector2(this.tileGrid.Width, this.tileGrid.Height).Serialize(),
                tiles = tileObjects
            };
        }

        public void Deserialize(string json)
        {
            JObject obj = JObject.Parse(json);

            // Deserialize the root level items first.
            this.Index = (int)obj["index"];
            this.Name = (string)obj["name"];
            this.IsMainLayer = (bool)obj["isMainLayer"];
            this.Position = obj["position"].ToVector2();
            
            // Next, create the tile grid with the given width.
            Vector2 gridWidth = obj["gridSize"].ToVector2();
            this.tileGrid = new SizedGrid<Tile>((int)GameServices.GameObjectSize.X, (int)GameServices.GameObjectSize.Y, (int)gridWidth.X, (int)gridWidth.Y);

            // Finally, deserialize the tiles.
            JArray tilesData = (JArray)obj["tiles"];

            foreach (var tileData in tilesData)
            {
                string typeName = (string)tileData["typeName"];
                Tile tile = AssemblyManager.GetTileByFullName(typeName);
                tile.Deserialize(tileData.ToString());
                tile.Initialize(this.Owner);
            }
        }
    }
}
