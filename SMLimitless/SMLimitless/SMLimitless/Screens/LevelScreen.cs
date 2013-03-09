using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using SMLimitless.Sprites;
using SMLimitless.Sprites.Collections;

namespace SMLimitless.Screens
{
    public class LevelScreen : Screen
    {
        private Level level;

        public override void Initialize(Screen owner, string parameters)
        {
            level = new Level();
            level.Initialize();
        }

        public override void LoadContent()
        {
            level.LoadContent();
        }

        public override void Update()
        {
            level.Update();
        }

        public override void Draw()
        {
            level.Draw();
        }
    }
}
