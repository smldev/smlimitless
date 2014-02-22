using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Content;
using SMLimitless.Extensions;
using SMLimitless.Input;
using SMLimitless.Sprites;
using SMLimitless.Graphics;

namespace SmlSample
{
    public sealed class PainterSprite : Sprite
    {
        private List<StaticGraphicsObject> tileGraphics;
        private string[] tileTypeNames = new string[] { "SmlSample.TestTile", "SmlSample.TestTile2", "SmlSample.TestTile3", "SmlSample.TestTile4", "SmlSample.TestTile5", "SmlSample.SlopedTestTile1", "SmlSample.SlopedTestTile2" };
        private string[] graphicsObjectNames = new string[] { "smw_grass_top", "smw_grass_center", "smw_concrete_block", "smw_grass_slope_bottom1", "smw_grass_slope_bottom2", "smw_grass_slope1", "smw_grass_slope2" };
        private int currentGraphicIndex;
        private Assembly currentAssembly; // because I'm so lazy I need reflection to instantiate known types in the same namespace as this type

        public override string EditorCategory
        {
            get { return "Testing"; }
        }

        public PainterSprite()
        {
            this.tileGraphics = new List<StaticGraphicsObject>();
            this.Size = new Vector2(16f);
            this.currentAssembly = Assembly.GetExecutingAssembly();
        }

        public override void Initialize(SMLimitless.Sprites.Collections.Section owner)
        {
            for (int i = 0; i < 7; i++)
            {
                this.tileGraphics.Add((StaticGraphicsObject)ContentPackageManager.GetGraphicsResource(this.graphicsObjectNames[i]));
            }

            base.Initialize(owner);
        }

        public override void LoadContent()
        {
            this.tileGraphics.ForEach(g => g.LoadContent());
        }

        public override void Draw()
        {
            this.tileGraphics[currentGraphicIndex].Draw(this.Position, Color.White);
        }

        public override void Update()
        {
            this.Position = (InputManager.MousePosition + this.Owner.Camera.Position).FloorDivide(16f) * 16f;
            Vector2 checkPosition = this.Position + (this.Size / 2f); // add/remove tiles by checking the center point of this sprite

            if (InputManager.IsCurrentMousePress(MouseButtons.LeftButton))
            {
                // Place a tile if there already wasn't one
                if (this.Owner.GetTileAtPosition(checkPosition, false) == null)
                {
                    Tile tile = (Tile)Activator.CreateInstance(this.currentAssembly.GetType(this.tileTypeNames[this.currentGraphicIndex]));
                    tile.Position = this.Position;
                    tile.Initialize(this.Owner);
                    tile.LoadContent();
                    this.Owner.AddTile(tile);
                }
            }
            else if (InputManager.IsCurrentMousePress(MouseButtons.RightButton))
            {
                // Remove the tile under the cursor, if there is one
                Tile tile = this.Owner.GetTileAtPosition(checkPosition, true);

                if (tile != null)
                {
                    this.Owner.RemoveTile(tile);
                }
            }
            else if (InputManager.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.C))
            {
                // Cycle through to the next tile type
                if (currentGraphicIndex == 6)
                {
                    currentGraphicIndex = 0;
                }
                else
                {
                    currentGraphicIndex++;
                }
            }
        }

        public override object GetCustomSerializableObjects()
        {
            return null;
        }

        public override void DeserializeCustomObjects(SMLimitless.Sprites.Assemblies.JsonHelper customObjects)
        {
        }

        public override void HandleSpriteCollision(Sprite sprite, Vector2 intersect)
        {
        }

        public override void HandleTileCollision(Tile tile, Vector2 intersect)
        {
        }
    }
}
