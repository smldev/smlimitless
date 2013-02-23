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

        public abstract void Initialize(string parameters);

        public abstract void Draw();
        public abstract void Update();

        public void Start()
        {
            this.DoUpdate = true;
        }

        public void Stop()
        {
            this.DoUpdate = false;
        }

        /// <summary>
        /// Exits this screen and gives control to the parent screen.
        /// </summary>
        /// <returns>Parameters indicating the exit condition.  Varies by screen.</returns>
        public abstract string Exit();
    }
}
