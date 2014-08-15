//-----------------------------------------------------------------------
// <copyright file="EventScript.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using SMLimitless.Interfaces;

namespace SMLimitless.Sprites.Collections
{
    /// <summary>
    /// A C# script that is used to manipulate levels and the objects within them.
    /// </summary>
    public class EventScript : ISerializable
    {
        /// <summary>
        /// Gets or sets the text of the C# script.
        /// </summary>
        public string Script { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventScript"/> class.
        /// </summary>
        public EventScript()
        {
        }
        
        /// <summary>
        /// Returns an anonymous object containing key objects of this script.
        /// </summary>
        /// <returns>An anonymous object containing the script.</returns>
        public object GetSerializableObjects()
        {
            return new
            {
                script = this.Script
            };
        }

        /// <summary>
        /// Returns a JSON string from the key objects of this script.
        /// </summary>
        /// <returns>A JSON string containing this script.</returns>
        public string Serialize()
        {
            return JObject.FromObject(this.GetSerializableObjects()).ToString();
        }

        /// <summary>
        /// Loads a script, given a JSON string containing a script.
        /// </summary>
        /// <param name="json">A valid JSON string.</param>
        public void Deserialize(string json)
        {
            JObject obj = JObject.Parse(json);
            this.Script = (string)obj["script"];
        }
    }
}
