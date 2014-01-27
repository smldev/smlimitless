//-----------------------------------------------------------------------
// <copyright file="Level.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using SMLimitless.Interfaces;
using SMLimitless.Physics;
using SMLimitless.Sprites.Collections.Structures;

namespace SMLimitless.Sprites.Collections
{
    /// <summary>
    /// The main unit of gameplay.
    /// </summary>
    public sealed class Level
    {
        /// <summary>
        /// Gets a string placed in all level files indicating
        /// the version of the serializer used to create it.
        /// </summary>
        public static string SerializerVersion
        {
            get
            {
                return "Version 0.01";
            }
        }

        /// <summary>
        /// A collection of all the level exits in this level.
        /// </summary>
        private List<LevelExit> levelExits;

        /// <summary>
        /// A collection of all the sections in this level.
        /// </summary>
        private List<Section> sections;

        /// <summary>
        /// A collection of all the paths to the content package folders used in this level.
        /// </summary>
        private List<string> contentFolderPaths;

        /// <summary>
        /// The event script of this level.
        /// </summary>
        private EventScript eventScript;

        /// <summary>
        /// Gets the name of the level, which is presented on menu screens.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the name of the author who created this level.
        /// </summary>
        public string Author { get; private set; }

        /// <summary>
        /// The acceleration caused by gravity, measured in pixels per second per second.
        /// </summary>
        public const float GravityAcceleration = 250f;

        /// <summary>
        /// Initializes a new instance of the <see cref="Level"/> class.
        /// </summary>
        public Level()
        {
            this.levelExits = new List<LevelExit>();
            this.sections = new List<Section>();
        }

        /// <summary>
        /// Initializes this level.
        /// </summary>
        public void Initialize() { }

        /// <summary>
        /// Loads the content of this level.
        /// </summary>
        public void LoadContent() { }

        /// <summary>
        /// Updates this level.
        /// </summary>
        public void Update() { }

        /// <summary>
        /// Draws this level.
        /// </summary>
        public void Draw() { }

        /// <summary>
        /// Notifies this level that a level exit has been cleared.
        /// </summary>
        /// <param name="exitSpriteName">The name of the sprite that served as the level exit.</param>
        public void LevelExitCleared(string exitSpriteName)
        {
            // Look in this.levelExits for an exit with the sprite name
            // Notify the owner (world/levelpack/whatever) that this exit has been cleared
            // Give the owner the LevelExit tied to the exitSpriteName
        }

        /// <summary>
        /// Gets an anonymous object containing key objects of this level.
        /// </summary>
        /// <returns>An anonymous object containing key objects of this level.</returns>
        public object GetSerializableObjects()
        {
            List<object> levelExitObjects = new List<object>(this.levelExits.Count);
            List<object> sectionObjects = new List<object>(this.sections.Count);
            this.levelExits.ForEach(l => levelExitObjects.Add(l.GetSerializableObjects()));
            this.sections.ForEach(s => sectionObjects.Add(s.GetSerializableObjects()));

            return new
            {
                header = new
                {
                    version = Level.SerializerVersion,
                    name = this.Name,
                    author = this.Author,
                },
                contentPackages = this.contentFolderPaths,
                levelExits = levelExitObjects,
                sections = sectionObjects,
                script = this.eventScript.Script
            };
        }

        /// <summary>
        /// Serializes the key objects of this level into a JSON string.
        /// </summary>
        /// <returns>A JSON string containing key objects of this level.</returns>
        public string Serialize()
        {
            return JObject.FromObject(this.GetSerializableObjects()).ToString();
        }

        /// <summary>
        /// Loads a level given a JSON string containing its key objects.
        /// </summary>
        /// <param name="json">A valid JSON string.</param>
        public void Deserialize(string json)
        {
            JObject obj = JObject.Parse(json);

            // Check if the versions match.
            if ((string)obj["header"]["version"] != Level.SerializerVersion)
            {
                throw new Exception(string.Format("Level.Deserialize(string): This level was created with a different version of the serializer. Expected {0}, got {1}.", Level.SerializerVersion, (string)obj["header"]["version"]));
            }

            // Deserialize the root objects first.
            this.Name = (string)obj["header"]["name"];
            this.Author = (string)obj["header"]["author"];
            this.eventScript.Script = (string)obj["script"];

            // Then deserialize the nested objects.
            JArray contentObjects = (JArray)obj["contentPackages"];
            JArray sectionObjects = (JArray)obj["sections"];
            JArray levelExitObjects = (JArray)obj["levelExit"];

            this.contentFolderPaths = contentObjects.ToObject<List<string>>();

            foreach (var sectionObject in sectionObjects)
            {
                Section section = new Section(new BoundingRectangle(), this); // TODO: get rid of that first param
                section.Initialize();
                section.Deserialize(sectionObject.ToString());
                this.sections.Add(section);
            }

            foreach (var levelExitObject in levelExitObjects)
            {
                LevelExit levelExit = new LevelExit(int.MinValue, Direction.None, null); // TODO: get rid of ALL these params
                levelExit.Deserialize(levelExitObject.ToString());
                this.levelExits.Add(levelExit);
            }
        }
    }
}
