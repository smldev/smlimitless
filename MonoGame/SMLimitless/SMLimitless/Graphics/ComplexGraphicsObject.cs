//-----------------------------------------------------------------------
// <copyright file="ComplexGraphicsObject.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SMLimitless.Extensions;
using SMLimitless.Graphics;
using SMLimitless.Interfaces;
using SMLimitless.IO;

namespace SMLimitless.Graphics
{
    /// <summary>
    /// A collection of named graphics objects that can
    /// be switched between at will.
    /// </summary>
    public class ComplexGraphicsObject : IGraphicsObject
    {
        /// <summary>
        /// A collection of the named graphics objects.
        /// </summary>
        private Dictionary<string, IGraphicsObject> graphicsObjects;

        /// <summary>
        /// The name of the currently active object.
        /// </summary>
        private string currentObjectName;

        /// <summary>
        /// A value indicating whether this object is loaded.
        /// </summary>
        private bool isLoaded;

        /// <summary>
        /// A value indicating whether the content for this object is loaded.
        /// </summary>
        private bool isContentLoaded;

        /// <summary>
        /// The texture of the image for this object.
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// The path to the configuration file.
        /// </summary>
        private string configFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexGraphicsObject"/> class.
        /// </summary>
        public ComplexGraphicsObject()
        {
            this.graphicsObjects = new Dictionary<string, IGraphicsObject>();
        }

        /// <summary>
        /// Gets or sets the currently active object.
        /// </summary>
        public string CurrentObjectName
        {
            get
            {
                return this.currentObjectName;
            }

            set
            {
                if (this.graphicsObjects.Keys.Contains(value))
                {
                    this.currentObjectName = value;
                }
                else
                {
                    throw new ArgumentException(string.Format("ComplexGraphicsObject.CurrentObjectName.set: There is no object named {0} loaded. Please check the config at {1}.", value, this.configFilePath));
                }
            }
        }

        /// <summary>
        /// Gets the path to the image.
        /// </summary>
        internal string FilePath { get; private set; }

        /// <summary>
        /// Gets the size of each frame in pixels.
        /// </summary>
        internal Vector2 FrameSize { get; private set; }

        /// <summary>
        /// Loads a ComplexGraphicsObject.
        /// This overload is only present to fulfill the IGraphicsObject contract.
        /// Do not call it, call the Load(string, DataReader) overload instead.
        /// </summary>
        /// <param name="filePath">The file path to the image.</param>
        public void Load(string filePath)
        {
            throw new InvalidOperationException("ComplexGraphicsObject.Load(string): Cannot load from a single file. Please use either StaticGraphicsObject or the (string, DataReader) overload.");
        }

        /// <summary>
        /// Loads a ComplexGraphicsObject.
        /// </summary>
        /// <param name="filePath">The file path to the image.</param>
        /// <param name="config">A DataReader containing the configuration file for this object.</param>
        public void Load(string filePath, DataReader config)
        {
            if (!this.isLoaded)
            {
                this.FilePath = filePath;
                this.configFilePath = config.FilePath;

                if (!(config[0] == "[Complex]"))
                {
                    throw new FormatException("ComplexGraphicsObject.Load(string, DataReader): Invalid or corrupt configuration file.");
                }

                var settings = config.ReadFullSection("[Complex]");
                int totalGraphicsObjects;
                string startingObject;

                this.FrameSize = Vector2Extensions.Parse(settings["FrameSize"]);
                if (!int.TryParse(settings["TotalGraphicsObjects"], out totalGraphicsObjects))
                { 
                    throw new FormatException(string.Format("ComplexGraphicsObject.Load(string, DataReader): Invalid number of graphics objects specified. Got {0}.", settings["TotalGraphicsObjects"])); 
                }

                startingObject = settings["StartingObject"];

                for (int i = 0; i < totalGraphicsObjects; i++)
                {
                    string objectHeader = string.Concat("[Object", i, "]");
                    var objectData = config.ReadFullSection(objectHeader);
                    string name = objectData["Name"];
                    string type = objectData["Type"];
                    switch (type)
                    {
                        case "static":
                            StaticGraphicsObject staticObject = new StaticGraphicsObject();
                            staticObject.Load(objectData, this);
                            this.graphicsObjects.Add(name, staticObject);
                            break;
                        case "animated":
                            AnimatedGraphicsObject animatedObject = new AnimatedGraphicsObject();
                            animatedObject.Load(objectData, this);
                            this.graphicsObjects.Add(name, animatedObject);
                            break;
                        case "animated_runonce":
                            AnimatedGraphicsObject animatedRunOnceObject = new AnimatedGraphicsObject();
                            animatedRunOnceObject.Load(objectData, this);
                            animatedRunOnceObject.IsRunOnce = true;
                            this.graphicsObjects.Add(name, animatedRunOnceObject);
                            break;
                        default:
                            break;
                    }
                }

                this.CurrentObjectName = startingObject;
                this.isLoaded = true;
            }
        }

        /// <summary>
        /// Loads the content for this object.
        /// </summary>
        public void LoadContent()
        {
            if (this.isLoaded && !this.isContentLoaded)
            {
                this.texture = GraphicsManager.LoadTextureFromFile(this.FilePath);
                foreach (IGraphicsObject graphicsObject in this.graphicsObjects.Values)
                {
                    StaticGraphicsObject objectAsStatic = graphicsObject as StaticGraphicsObject;
                    AnimatedGraphicsObject objectAsAnimated = graphicsObject as AnimatedGraphicsObject;
                    if (objectAsStatic != null)
                    {
                        objectAsStatic.LoadContentCGO(this.texture);
                    }
                    else if (objectAsAnimated != null)
                    {
                        objectAsAnimated.LoadContentCGO(this.texture);
                    }
                    else
                    {
                        throw new InvalidCastException(string.Format("ComplexGraphicsObject.LoadContent(): Unrecognized graphics object type. Please check config at {0}.", this.configFilePath));
                    }
                }

                this.isContentLoaded = true;
            }
        }

		/// <summary>
		/// Gets a graphic suitable for display in the level editor's object selection window.
		/// </summary>
		/// <returns>A <see cref="Texture2D"/> instance containing the graphic to display.</returns>
		public Texture2D GetEditorGraphics()
		{
			return graphicsObjects[CurrentObjectName].GetEditorGraphics();
		}

        /// <summary>
        /// Updates the currently active object in this ComplexGraphicsObject.
        /// </summary>
        public void Update()
        {
            this.graphicsObjects[this.CurrentObjectName].Update();
        }

        /// <summary>
        /// Draws the currently active object in this ComplexGraphicsObject.
        /// </summary>
        /// <param name="position">The position at which to draw the object.</param>
        /// <param name="color">The color to shade the object. Use Color.White for no shading.</param>
        public void Draw(Vector2 position, Color color)
        {
            this.graphicsObjects[this.CurrentObjectName].Draw(position, color);
        }

        /// <summary>
        /// Draws the currently active object in this ComplexGraphicsObject.
        /// </summary>
        /// <param name="position">The position at which to draw the object.</param>
        /// <param name="color">The color to shade the object. Use Color.White for no shading.</param>
        /// <param name="debug">If true, and the current object is an AnimatedGraphicsObject, the animated object's current frame number is drawn at the top-left corner of the sprite.</param>
        public void Draw(Vector2 position, Color color, bool debug)
        {
            AnimatedGraphicsObject objectAsAnimated = this.graphicsObjects[this.CurrentObjectName] as AnimatedGraphicsObject;
            if (objectAsAnimated != null)
            {
                objectAsAnimated.Draw(position, color, debug);
            }
            else
            {
                this.graphicsObjects[this.CurrentObjectName].Draw(position, color);
            }
        }

        /// <summary>
        /// Draws the currently active object in this ComplexGraphicsObject.
        /// </summary>
        /// <param name="position">The position at which to draw the object.</param>
        /// <param name="color">The color to shade the object. Use Color.White for no shading.</param>
        /// <param name="spriteEffects">How to mirror this object.</param>
        public void Draw(Vector2 position, Color color, SpriteEffects spriteEffects)
        {
            this.graphicsObjects[this.CurrentObjectName].Draw(position, color, spriteEffects);
        }

        /// <summary>
        /// If the current object is animated,
        /// this method calls through to the animated object's
        /// SetSpeed(decimal) method. Adjusts the cycle length
        /// of any animated object.
        /// </summary>
        /// <param name="newCycleLength">The new cycle length.</param>
        public void SetSpeed(decimal newCycleLength)
        {
            AnimatedGraphicsObject objectAsAnimated = this.graphicsObjects[this.CurrentObjectName] as AnimatedGraphicsObject;
            if (objectAsAnimated != null)
            {
                objectAsAnimated.SetSpeed(newCycleLength);
            }
            else
            {
                throw new InvalidOperationException("ComplexGraphicsObject.SetSpeed(decimal): Tried to set the speed of an object that wasn't animated.");
            }
        }

        /// <summary>
        /// If the current object is animated,
        /// this method calls through to the animated object's
        /// SetSpeed method. Adjusts the frame time
        /// of any animated object.
        /// </summary>
        /// <param name="newFrameTime">The new frame time.</param>
        public void SetSpeed(int newFrameTime)
        {
            AnimatedGraphicsObject objectAsAnimated = this.graphicsObjects[this.CurrentObjectName] as AnimatedGraphicsObject;
            if (objectAsAnimated != null)
            {
                objectAsAnimated.SetSpeed(newFrameTime);
            }
            else
            {
                throw new InvalidOperationException("ComplexGraphicsObject.SetSpeed(int): Tried to set the speed of an object that wasn't animated.");
            }
        }

        /// <summary>
        /// If the current object is animated,
        /// this method calls through to the animated object's
        /// AdjustSpeed(float) method. Adjusts the animation speed
        /// of any animated object.
        /// </summary>
        /// <param name="percentage">The percentage by which to adjust the speed.</param>
        public void AdjustSpeed(float percentage)
        {
            AnimatedGraphicsObject objectAsAnimated = this.graphicsObjects[this.CurrentObjectName] as AnimatedGraphicsObject;
            if (objectAsAnimated != null)
            {
                objectAsAnimated.AdjustSpeed(percentage);
            }
            else
            {
                throw new InvalidOperationException("ComplexGraphicsObjects.AdjustSpeed(float): Tried to adjust an object that wasn't animated.");
            }
        }

        /// <summary>
        /// If the current object is animated,
        /// this method calls through to the animated object's
        /// Reset method. Resets the object, and optionally
        /// starts the animation running again.
        /// </summary>
        /// <param name="startRunning">If true, the object will begin running again.</param>
        public void Reset(bool startRunning)
        {
            AnimatedGraphicsObject objectAsAnimated = this.graphicsObjects[this.CurrentObjectName] as AnimatedGraphicsObject;
            if (objectAsAnimated != null)
            {
                objectAsAnimated.Reset(startRunning);
            }
            else
            {
                throw new InvalidOperationException("ComplexGraphicsObjects.Reset(bool): Tried to reset an object that wasn't animated.");
            }
        }

        /// <summary>
        /// Returns a deep copy of this ComplexGraphicsObject.
        /// With the texture, only its reference is copied.
        /// </summary>
        /// <returns>A deep copy of this object.</returns>
        public IGraphicsObject Clone()
        {
            ComplexGraphicsObject result = new ComplexGraphicsObject();
            Dictionary<string, IGraphicsObject> cloneObjects = new Dictionary<string, IGraphicsObject>();
            foreach (var graphicsObjectPair in this.graphicsObjects)
            {
				result.graphicsObjects.Add(graphicsObjectPair.Key, graphicsObjectPair.Value.Clone());
            }

            result.CurrentObjectName = this.CurrentObjectName;
            result.configFilePath = this.configFilePath;
            result.FilePath = this.FilePath;
            result.FrameSize = this.FrameSize;
            result.isContentLoaded = true;
            result.isLoaded = true;
            result.texture = this.texture;

            return result;
        }

        /// <summary>
        /// Returns the size, in pixels, of this object.
        /// </summary>
        /// <returns>The size of this object.</returns>
        public Vector2 GetSize()
        {
            if (this.isContentLoaded)
            {
                IGraphicsObject graphicsObject = this.graphicsObjects.First().Value;
                return graphicsObject.GetSize();
            }
            else
            {
                throw new InvalidOperationException("ComplexGraphicsObject.GetSize(): This object isn't fully loaded, and thus cannot return its size.");
            }
        }
    }
}
