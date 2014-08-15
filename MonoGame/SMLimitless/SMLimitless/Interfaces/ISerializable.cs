//-----------------------------------------------------------------------
// <copyright file="ISerializable.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SMLimitless.Interfaces
{
    /// <summary>
    /// A contract for an object to allow itself
    /// to be serialized in JSON format to a file.
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Returns a JSON string representing the
        /// state of this object.
        /// </summary>
        /// <returns>A JSON string.</returns>
        string Serialize();

        /// <summary>
        /// Returns an object, usually anonymous,
        /// that contains the members that should be serialized.
        /// </summary>
        /// <returns>An object to serialize.</returns>
        object GetSerializableObjects();

        /// <summary>
        /// Receives a JSON string and uses it to set its state.
        /// </summary>
        /// <param name="json">A valid JSON string containing the required elements.</param>
        void Deserialize(string json);
    }
}
