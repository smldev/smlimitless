//-----------------------------------------------------------------------
// <copyright file="Level.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
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

namespace SMLimitless.Sprites.Collections
{
    /// <summary>
    /// The main unit of gameplay.
    /// </summary>
    public sealed class Level
    {
		/// <summary>
		/// Gets or sets the section that the player is currently in.
		/// </summary>
		internal Section ActiveSection { get; set; }

		/// <summary>
		/// Gets the name of the author who created this level.
		/// </summary>
		public string Author { get; internal set; }

		/// <summary>
		/// Gets or sets a collection of all the paths to the content package folders used in this level.
		/// </summary>
		internal List<string> ContentFolderPaths { get; set; }

		/// <summary>
		/// Gets or sets the event script of this level.
		/// </summary>
		internal EventScript EventScript { get; set; }

		/// <summary>
		/// Gets or sets a collection of all the level exits in this level.
        /// </summary>
		internal List<LevelExit> LevelExits { get; set; }

		/// <summary>
		/// Gets the name of the level, which is presented on menu screens.
		/// </summary>
		public string Name { get; internal set; }

		/// <summary>
		/// Gets or sets a collection of all the sections in this level.
        /// </summary>
		internal List<Section> Sections { get; set; }

		/// <summary>
        /// Gets a string placed in all level files indicating
        /// the version of the serializer used to create it.
        /// </summary>
		[Obsolete]
        public static string SerializerVersion
        {
            get
            {
                return "Version 0.01";
            }
        }

        /// <summary>
        /// The acceleration caused by gravity, measured in pixels per second per second.
        /// </summary>
        public const float GravityAcceleration = 250f;

        /// <summary>
        /// Initializes a new instance of the <see cref="Level"/> class.
        /// </summary>
        public Level()
        {
            this.LevelExits = new List<LevelExit>();
            this.Sections = new List<Section>();
            this.EventScript = new EventScript();
        }

        /// <summary>
        /// Initializes this level.
        /// </summary>
        public void Initialize() 
		{ 
			this.Sections.ForEach(s => s.Initialize());
		}

        /// <summary>
        /// Loads the content of this level.
        /// </summary>
        public void LoadContent() 
        {
            this.Sections.ForEach(s => s.LoadContent());
        }

        /// <summary>
        /// Updates this level.
        /// </summary>
        public void Update() 
        {
            this.ActiveSection.Update();
        }

        /// <summary>
        /// Draws this level.
        /// </summary>
        public void Draw() 
        {
            this.ActiveSection.Draw();
        }

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
		[Obsolete]
        public object GetSerializableObjects()
        {
            List<object> levelExitObjects = new List<object>(this.LevelExits.Count);
            List<object> sectionObjects = new List<object>(this.Sections.Count);
            this.LevelExits.ForEach(l => levelExitObjects.Add(l.GetSerializableObjects()));
            this.Sections.ForEach(s => sectionObjects.Add(s.GetSerializableObjects()));

            return new
            {
                header = new
                {
                    version = Level.SerializerVersion,
                    name = this.Name,
                    author = this.Author,
                },
                contentPackages = this.ContentFolderPaths,
                levelExits = levelExitObjects,
                sections = sectionObjects,
                script = this.EventScript.Script
            };
        }

        /// <summary>
        /// Serializes the key objects of this level into a JSON string.
        /// </summary>
        /// <returns>A JSON string containing key objects of this level.</returns>
		[Obsolete]
        public string Serialize()
        {
            return JObject.FromObject(this.GetSerializableObjects()).ToString();
        }

        /// <summary>
        /// Loads a level given a JSON string containing its key objects.
        /// </summary>
        /// <param name="json">A valid JSON string.</param>
		[Obsolete]
        public void Deserialize(string json)
        {
            JObject obj = JObject.Parse(json);

            // Check if the versions match.
            if ((string)obj["header"]["version"] != Level.SerializerVersion)
            {
                throw new FormatException(string.Format("Level.Deserialize(string): This level was created with a different version of the serializer. Expected {0}, got {1}.", Level.SerializerVersion, (string)obj["header"]["version"]));
            }

            // Deserialize the root objects first.
            this.Name = (string)obj["header"]["name"];
            this.Author = (string)obj["header"]["author"];
            this.EventScript.Script = (string)obj["script"];

            // Then deserialize the nested objects.
            JArray contentObjects = (JArray)obj["contentPackages"];
            JArray sectionObjects = (JArray)obj["sections"];
            JArray levelExitObjects = (JArray)obj["levelExits"];

            this.ContentFolderPaths = contentObjects.ToObject<List<string>>();
            Content.ContentPackageManager.AddPackageFromFolder(System.IO.Directory.GetCurrentDirectory() + @"\" + this.ContentFolderPaths[0]); // TODO: temporary -- how did you want me to change it, past self?

            foreach (var sectionObject in sectionObjects)
            {
                Section section = new Section(this);
                section.Initialize();
                section.Deserialize(sectionObject.ToString());
                this.Sections.Add(section);
            }

            foreach (var levelExitObject in levelExitObjects)
            {
                LevelExit levelExit = new LevelExit();
                levelExit.Deserialize(levelExitObject.ToString());
                this.LevelExits.Add(levelExit);
            }

            this.ActiveSection = this.Sections.First(s => s.Index == 0);
        }
    }
}
