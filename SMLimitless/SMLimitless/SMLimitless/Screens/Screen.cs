using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SMLimitless.Interfaces;

namespace SMLimitless.Screens
{
    /// <summary>
    /// A drawable, updatable component that contains an internal object.
    /// Must be inherited.
    /// </summary>
    public abstract class Screen
    {
        public Screen Owner;

        public bool IsRunning = false;

        protected IEffect effect;
        protected Screen nextScreen;

        protected bool isInitialized;
        protected bool isContentLoaded;

        public abstract void Initialize(string parameters);
        public abstract void LoadContent();
        public abstract void Draw();
        public abstract void Update();
        public virtual void UnloadContent() { }

        public virtual void Start()
        {
            this.IsRunning = true;
        }

        public virtual void Stop()
        {
            this.IsRunning = false;
        }

        /// <summary>
        /// Exits this screen and gives control to the parent screen.
        /// </summary>
        /// <returns>Parameters indicating the exit condition.  Varies by screen.</returns>
        public abstract string Exit();
    }
}
