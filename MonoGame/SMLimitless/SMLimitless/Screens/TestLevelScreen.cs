//-----------------------------------------------------------------------
// <copyright file="TestLevelScreen.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLimitless.Sprites.Testing;

namespace SMLimitless.Screens
{
    /// <summary>
    /// A screen that wraps and displays a TestLevel instance.
    /// </summary>
    public class TestLevelScreen : Screen
    {
        /// <summary>
        /// The test level.
        /// </summary>
        private TestLevel testLevel;

        /// <summary>
        /// Initializes this screen.
        /// </summary>
        /// <param name="owner">The screen that created this one.</param>
        /// <param name="parameters">Optional parameters to use in initialization. Unused for this screen.</param>
        public override void Initialize(Screen owner, string parameters)
        {
            this.testLevel = new TestLevel();
            this.testLevel.Initialize();
        }

        /// <summary>
        /// Loads the content for this screen.
        /// </summary>
        public override void LoadContent()
        {
            this.testLevel.LoadContent();
        }

        /// <summary>
        /// Updates this screen.
        /// </summary>
        public override void Update()
        {
            this.testLevel.Update();
        }

        /// <summary>
        /// Draws this screen.
        /// </summary>
        public override void Draw()
        {
            this.testLevel.Draw();
        }
    }
}
