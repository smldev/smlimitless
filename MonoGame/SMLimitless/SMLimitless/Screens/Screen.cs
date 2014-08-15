//-----------------------------------------------------------------------
// <copyright file="Screen.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLimitless.Interfaces;

namespace SMLimitless.Screens
{
    /// <summary>
    /// A component that can be drawn and updated. It contains an internal object.
    /// Must be inherited.
    /// </summary>
    public abstract class Screen
    {
        /// <summary>
        /// A backing field for this IsRunning property.
        /// </summary>
        private bool isRunning = false;

        /// <summary>
        /// Gets or sets the screen that owns (has created) this one.
        /// </summary>
        public Screen Owner { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this screen is running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.isRunning;
            }

            set
            {
                this.isRunning = value;
            }
        }

        /// <summary>
        /// Gets or sets an effect that can be used as a transition between screens.
        /// </summary>
        protected IEffect Effect { get; set; }

        /// <summary>
        /// Gets or sets a reference to a screen that is switched to once this screen exits.
        /// </summary>
        protected Screen NextScreen { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this screen is initialized.
        /// </summary>
        protected bool IsInitialized { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this screen has had its content loaded.
        /// </summary>
        protected bool IsContentLoaded { get; set; }

        /// <summary>
        /// Initializes this screen.
        /// </summary>
        /// <param name="owner">The screen that is creating this one.</param>
        /// <param name="parameters">Parameters that are used to determine certain settings of a screen. Varies by screen; check the documentation.</param>
        public abstract void Initialize(Screen owner, string parameters);

        /// <summary>
        /// Loads the content for this screen.
        /// </summary>
        public abstract void LoadContent();

        /// <summary>
        /// Draws this screen.
        /// </summary>
        public abstract void Draw();

        /// <summary>
        /// Updates this screen.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Unloads the content for this screen.
        /// </summary>
        public virtual void UnloadContent() 
        {
        }

        /// <summary>
        /// Starts this screen.
        /// </summary>
        /// <param name="parameters">Parameters that are used to determine certain settings of a screen. Varies by screen; check the documentation.</param>
        public virtual void Start(string parameters = "")
        {
            this.IsRunning = true;
        }

        /// <summary>
        /// Stops this screen.
        /// </summary>
        public virtual void Stop()
        {
            this.IsRunning = false;
        }

        /// <summary>
        /// Exits this screen and gives control to the parent screen.
        /// </summary>
        /// <param name="parameters">Parameters that are used to determine certain settings of a screen. Varies by screen; check the documentation.</param>
        public virtual void Exit(string parameters = "")
        {
            this.Stop();
            ScreenManager.ExitScreen(this, parameters);
        }
    }
}
