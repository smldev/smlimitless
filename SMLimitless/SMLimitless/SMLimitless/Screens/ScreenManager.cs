using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SMLimitless.Screens
{
    public static class ScreenManager
    {
        private static Hierarchy<Screen> screens;
        private static Screen activeScreen;

        public static void Initalize()
        {
            screens = new Hierarchy<Screen>(null);
        }

        public static void Update()
        {
            activeScreen.Update();
        }

        public static void Draw()
        {
            activeScreen.Draw();
        }

        public static void SetRootScreen(Screen screen, string parameters)
        {
            screens.Data = screen;
            activeScreen = screen;
            activeScreen.Initialize(parameters);
            activeScreen.Start();
        }

        public static void LoadContent()
        {
            activeScreen.LoadContent();
        }

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
                throw new Exception("ScreenManager.AddScreen: Could not find the screen's parent node.");
            }
        }

        public static void RemoveScreen(Screen screen, bool removeChildren)
        {
            var screenNode = screens.Search(screen);
            var parentNode = screenNode.Parent;
            if (screenNode == null) return;

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

        public static void SwitchScreen(Screen switchTo)
        {
            activeScreen.Stop();
            switchTo.LoadContent();
            activeScreen = switchTo;
            activeScreen.Start();
        }
    }
}
