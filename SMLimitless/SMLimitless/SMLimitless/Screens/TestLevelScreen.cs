using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLimitless.Sprites.Testing;

namespace SMLimitless.Screens
{
    public class TestLevelScreen : Screen
    {
        private TestLevel testLevel;

        public override void Initialize(Screen owner, string parameters)
        {
            this.testLevel = new TestLevel();
            this.testLevel.Initialize();
        }

        public override void LoadContent()
        {
            this.testLevel.LoadContent();
        }

        public override void Update()
        {
            this.testLevel.Update();
        }

        public override void Draw()
        {
            this.testLevel.Draw();
        }
    }
}
