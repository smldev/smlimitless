using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless.Screens
{
    /// <summary>
    /// A drawable, updatable component that contains an internal object.
    /// Must be inherited.
    /// </summary>
    public abstract class Screen
    {
        public bool DoDraw;
        public bool DoUpdate;

        public abstract void Draw();
        public abstract void Update();
    }
}
