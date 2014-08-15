//-----------------------------------------------------------------------
// <copyright file="ScreenManager.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLimitless.Collections;

namespace SMLimitless.Screens
{
    /// <summary>
    /// Manages loading and running screens for the game.
    /// </summary>
    public static class ScreenManager
    {
        /// <summary>
        /// A hierarchal collection of managed screens.
        /// </summary>
        private static Hierarchy<Screen> screens;

        /// <summary>
        /// The currently running screen.
        /// </summary>
        private static Screen activeScreen;

        /// <summary>
        /// Initializes this ScreenManager.
        /// </summary>
        public static void Initialize()
        {
            screens = new Hierarchy<Screen>(null);
        }

        /// <summary>
        /// Updates the active screen.
        /// </summary>
        public static void Update()
        {
            activeScreen.Update();
        }

        /// <summary>
        /// Draws the active screen.
        /// </summary>
        public static void Draw()
        {
            activeScreen.Draw();
        }

        /// <summary>
        /// Sets the highest screen in the hierarchy.
        /// This screen has no owner.
        /// </summary>
        /// <param name="screen">The screen to set as root.</param>
        /// <param name="parameters">Parameters that are used to determine certain settings of a screen. Varies by screen; check the documentation.</param>
        public static void SetRootScreen(Screen screen, string parameters)
        {
            screens.Data = screen;
            activeScreen = screen;
            activeScreen.Initialize(null, parameters);
            activeScreen.Start();
        }

        /// <summary>
        /// Loads the content for the active screen.
        /// </summary>
        public static void LoadContent()
        {
            activeScreen.LoadContent();
        }

        /// <summary>
        /// Adds a screen to the manager.
        /// </summary>
        /// <param name="parent">The screen that owns the screen to add.</param>
        /// <param name="child">The screen to add.</param>
        public static void AddScreen(Screen parent, Screen child)
        {
            var childNode = new Hierarchy<Screen>(child);
            var parentNode = screens.Search(parent);

            if (parentNode != null)
            {
                parentNode.Add(childNode);
            }
            else
            {
                throw new ArgumentException("ScreenManager.AddScreen(Screen, Screen): Could not find the screen's parent node.");
            }
        }

        /// <summary>
        /// Removes a screen from the manager.
        /// </summary>
        /// <param name="screen">The screen to remove.</param>
        /// <param name="removeChildren">If true, all child screens are removed. If false, all child screens are assigned to their grandparent.</param>
        public static void RemoveScreen(Screen screen, bool removeChildren)
        {
            var screenNode = screens.Search(screen);
            var parentNode = screenNode.Parent;
            if (screenNode == null)
            { 
                return; 
            }

            if (!removeChildren)
            {
                // Move all screens one level up
                foreach (var child in screenNode.Children)
                {
                    parentNode.Add(child);
                }
            }

            parentNode.Remove(screenNode);
        }

        /// <summary>
        /// Switches to another screen.
        /// </summary>
        /// <param name="switchTo">The screen to switch to.</param>
        public static void SwitchScreen(Screen switchTo)
        {
            if (screens.Search(switchTo) != null)
            {
                activeScreen.Stop();
                switchTo.LoadContent();
                activeScreen = switchTo;
                activeScreen.Start();
            }
            else
            {
                throw new ArgumentException("ScreenManager.SwitchScreen(Screen): The screen to switch to was not present in the manager. Please use the AddScreen(Screen, string) method to add it first.");
            }
        }

        /// <summary>
        /// Exits a screen and switches the active screen to its owner.
        /// </summary>
        /// <param name="screen">The screen to exit.</param>
        /// <param name="exitMessage">An exit message.</param>
        public static void ExitScreen(Screen screen, string exitMessage)
        {
            Screen parent = screen.Owner;
            RemoveScreen(screen, false);
            activeScreen = parent;
            parent.Start(exitMessage);
        }
    }
}
