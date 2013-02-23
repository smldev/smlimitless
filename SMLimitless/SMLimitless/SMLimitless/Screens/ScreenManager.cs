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

        public static void Update()
        {
            activeScreen.Update();
        }

        public static void Draw()
        {
            activeScreen.Draw();
        }

        public void AddScreen(Screen parent, Screen child)
        {
            var childNode = new Hierarchy<Screen>(child);
            var parentNode = screens.Search(parent);

            if (parentNode != null)
            {
                parentNode.Add(childNode);
            }
        }

        public void RemoveScreen(Screen screen, bool removeChildren)
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
    }
}
