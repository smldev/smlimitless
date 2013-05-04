using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SMLimitless.Graphics;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using SMLimitless.IO;

namespace SMLimitless.Graphics
{
    public class ComplexGraphicsObject : IGraphicsObject
    {
        private Dictionary<string, IGraphicsObject> graphicsObjects;
        private string currentObjectName;
        public string CurrentObjectName
        {
            get
            {
                return currentObjectName;
            }
            set
            {
                if (graphicsObjects.Keys.Contains(value))
                {
                    currentObjectName = value;
                }
                else
                {
                    throw new Exception(String.Format("ComplexGraphicsObject.CurrentObjectName.set: There is no object named {0} loaded. Please check the config at {1}.", value, configFilePath));
                }
            }
        }

        private bool isLoaded;
        private bool isContentLoaded;
        private Texture2D texture;
        private string configFilePath;

        internal string FilePath { get; private set; }
        internal Vector2 FrameSize { get; private set; }

        public ComplexGraphicsObject()
        {
            graphicsObjects = new Dictionary<string, IGraphicsObject>();
        }

        public void Load(string filePath)
        {
            throw new Exception("ComplexGraphicsObject.Load(string): Cannot load from a single file. Please use either StaticGraphicsObject or the (string, DataReader) overload.");
        }

        public void Load(string filePath, DataReader config)
        {
            if (!isLoaded)
            {
                this.FilePath = filePath;
                configFilePath = config.FilePath;

                if (!(config[0] == "[Complex]"))
                {
                    throw new Exception("ComplexGraphicsObject.Load(string, DataReader): Invalid or corrupt configuration file.");
                }
                var settings = config.ReadFullSection("[Complex]");
                int totalGraphicsObjects;
                string startingObject;

                FrameSize = Vector2Extensions.Parse(settings["FrameSize"]);
                if (!Int32.TryParse(settings["TotalGraphicsObjects"], out totalGraphicsObjects)) throw new Exception(String.Format("ComplexGraphicsObject.Load(string, DataReader): Invalid number of graphics objects specified. Got {0}.", settings["TotalGraphicsObjects"]));
                startingObject = settings["StartingObject"];

                for (int i = 0; i < totalGraphicsObjects; i++)
                {
                    string objectHeader = String.Concat("[Object", i, "]");
                    var objectData = config.ReadFullSection(objectHeader);
                    string name = objectData["Name"];
                    string type = objectData["Type"];
                    switch (type)
                    {
                        case "static":
                            StaticGraphicsObject staticObject = new StaticGraphicsObject();
                            staticObject.Load(objectData, this);
                            graphicsObjects.Add(name, staticObject);
                            break;
                        case "animated":
                            AnimatedGraphicsObject animatedObject = new AnimatedGraphicsObject();
                            animatedObject.Load(objectData, this);
                            graphicsObjects.Add(name, animatedObject);
                            break;
                        case "animated_runonce":
                            AnimatedGraphicsObject animatedRunOnceObject = new AnimatedGraphicsObject();
                            animatedRunOnceObject.Load(objectData, this);
                            animatedRunOnceObject.IsRunOnce = true;
                            graphicsObjects.Add(name, animatedRunOnceObject);
                            break;
                        default:
                            break;
                    }
                }

                CurrentObjectName = startingObject;
                isLoaded = true;
            }
        }

        public void LoadContent()
        {
            if (isLoaded && !isContentLoaded)
            {
                texture = GraphicsManager.LoadTextureFromFile(FilePath);
                foreach (IGraphicsObject graphicsObject in graphicsObjects.Values)
                {
                    StaticGraphicsObject objectAsStatic = graphicsObject as StaticGraphicsObject;
                    AnimatedGraphicsObject objectAsAnimated = graphicsObject as AnimatedGraphicsObject;
                    if (objectAsStatic != null)
                    {
                        objectAsStatic.LoadContentCGO(texture);
                    }
                    else if (objectAsAnimated != null)
                    {
                        objectAsAnimated.LoadContentCGO(texture);
                    }
                    else
                    {
                        throw new Exception(String.Format("ComplexGraphicsObject.LoadContent(): Unrecognized graphics object type. Please check config at {0}.", configFilePath));
                    }
                }
                isContentLoaded = true;
            }
        }

        public void Update()
        {
            graphicsObjects[CurrentObjectName].Update();
        }

        public void Draw(Vector2 position, Color color)
        {
            graphicsObjects[CurrentObjectName].Draw(position, color);
        }

        public void Draw(Vector2 position, Color color, bool debug)
        {
            AnimatedGraphicsObject objectAsAnimated = graphicsObjects[CurrentObjectName] as AnimatedGraphicsObject;
            if (objectAsAnimated != null)
            {
                objectAsAnimated.Draw(position, color, debug);
            }
            else
            {
                graphicsObjects[CurrentObjectName].Draw(position, color);
            }
        }

        public void Draw(Vector2 position, Color color, SpriteEffects spriteEffects)
        {
            graphicsObjects[CurrentObjectName].Draw(position, color, spriteEffects);
        }

        public void SetSpeed(decimal newCycleLength)
        {
            AnimatedGraphicsObject objectAsAnimated = graphicsObjects[CurrentObjectName] as AnimatedGraphicsObject;
            if (objectAsAnimated != null)
            {
                objectAsAnimated.SetSpeed(newCycleLength);
            }
            else
            {
                throw new Exception("ComplexGraphicsObject.SetSpeed(decimal): Tried to set an object that wasn't animated.");
            }
        }

        public void SetSpeed(int newFrameTime)
        {
            AnimatedGraphicsObject objectAsAnimated = graphicsObjects[CurrentObjectName] as AnimatedGraphicsObject;
            if (objectAsAnimated != null)
            {
                objectAsAnimated.SetSpeed(newFrameTime);
            }
            else
            {
                throw new Exception("ComplexGraphicsObject.SetSpeed(int): Tried to set an object that wasn't animated.");
            }
        }

        public void AdjustSpeed(float percentage)
        {
            AnimatedGraphicsObject objectAsAnimated = graphicsObjects[CurrentObjectName] as AnimatedGraphicsObject;
            if (objectAsAnimated != null)
            {
                objectAsAnimated.AdjustSpeed(percentage);
            }
            else
            {
                throw new Exception("ComplexGraphicsObjects.AdjustSpeed(float): Tried to adjust an object that wasn't animated.");
            }
        }

        public void Reset(bool startRunning)
        {
            AnimatedGraphicsObject objectAsAnimated = graphicsObjects[CurrentObjectName] as AnimatedGraphicsObject;
            if (objectAsAnimated != null)
            {
                objectAsAnimated.Reset(startRunning);
            }
            else
            {
                throw new Exception("ComplexGraphicsObjects.Reset(bool): Tried to reset an object that wasn't animated.");
            }
        }

        public IGraphicsObject Clone()
        {
            ComplexGraphicsObject result = new ComplexGraphicsObject();
            Dictionary<string, IGraphicsObject> cloneObjects = new Dictionary<string, IGraphicsObject>();
            foreach (var graphicsObjectPair in graphicsObjects)
            {
                cloneObjects.Add(graphicsObjectPair.Key, graphicsObjectPair.Value.Clone());
            }

            result.CurrentObjectName = CurrentObjectName;
            result.configFilePath = configFilePath;
            result.FilePath = FilePath;
            result.FrameSize = FrameSize;
            result.isContentLoaded = true;
            result.isLoaded = true;
            result.texture = texture;

            return result;
        }
    }
}
