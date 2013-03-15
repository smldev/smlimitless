using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using SMLimitless.Extensions;
using SMLimitless.Physics;
using SMLimitless.Sprites;

namespace SMLimitless
{
    public static class Testing
    {
        public static Stopwatch Stopwatch;
        //private static QuadTree quadTree;

        public static void Benchmark()
        {
            if (Stopwatch == null) Stopwatch = new Stopwatch();
            //if (quadTree == null) quadTree = new QuadTree(new Vector2(64, 64));

            //TestSprite sprite = new TestSprite();

            //Stopwatch.Start();
            //quadTree.GetIntersectingCells(sprite);
            //Stopwatch.Stop();
            
            GameServices.SpriteBatch.DrawString(GameServices.DebugFontLarge, Stopwatch.ElapsedTicks.ToString(), Vector2.Zero, Color.White);
            Stopwatch.Reset();
        }
    }
}
