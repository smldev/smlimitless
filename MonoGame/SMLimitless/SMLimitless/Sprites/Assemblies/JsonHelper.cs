//-----------------------------------------------------------------------
// <copyright file="JsonHelper.cs" company="The Limitless Development Team">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SMLimitless.Sprites.Assemblies
{
    /// <summary>
    /// A type that contains an instance of a JToken.
    /// </summary>
    /// <remarks>
    /// This type allows sprite and tiles to not need
    /// to explicitly reference Newtonsoft.Json.dll.
    /// </remarks>
    public class JsonHelper
    {
        /// <summary>
        /// The JToken instance.
        /// </summary>
        private JToken value;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonHelper"/> class.
        /// </summary>
        /// <param name="token">The JToken instance.</param>
        public JsonHelper(JToken token)
        {
            this.value = token;
        }

        /// <summary>
        /// Serializes a given object.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <returns>A JSON string representing the object.</returns>
        public static string Serialize(object value)
        {
            return JObject.FromObject(value).ToString();
        }

        /// <summary>
        /// Returns a boolean from the JToken instance.
        /// </summary>
        /// <param name="key">The name of the key containing the boolean.</param>
        /// <returns>A boolean from the JToken.</returns>
        public bool GetBool(string key)
        {
            try
            {
                return (bool)this.value[key];
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("The key {0} doesn't exist in the given token.", key), ex);
            }
        }

        /// <summary>
        /// Returns a byte from the JToken instance.
        /// </summary>
        /// <param name="key">The name of the key containing the byte.</param>
        /// <returns>A byte from the JToken.</returns>
        public byte GetByte(string key)
        {
            try
            {
                return (byte)this.value[key];
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("The key {0} doesn't exist in the given token.", key), ex);
            }
        }

        /// <summary>
        /// Returns an signed byte from the JToken instance.
        /// </summary>
        /// <param name="key">The name of the key containing the signed byte.</param>
        /// <returns>A signed byte from the JToken.</returns>
        public sbyte GetSByte(string key)
        {
            try
            {
                return (sbyte)this.value[key];
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("The key {0} doesn't exist in the given token.", key), ex);
            }
        }

        /// <summary>
        /// Returns a short integer from the JToken instance.
        /// </summary>
        /// <param name="key">The name of the key containing the short integer.</param>
        /// <returns>A short integer from the JToken.</returns>
        public short GetShort(string key)
        {
            try
            {
                return (short)this.value[key];
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("The key {0} doesn't exist in the given token.", key), ex);
            }
        }

        /// <summary>
        /// Returns an unsigned short integer from the JToken instance.
        /// </summary>
        /// <param name="key">The name of the key containing the unsigned short integer.</param>
        /// <returns>An unsigned short integer from the JToken.</returns>
        public ushort GetUShort(string key)
        {
            try
            {
                return (ushort)this.value[key];
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("The key {0} doesn't exist in the given token.", key), ex);
            }
        }

        /// <summary>
        /// Returns an integer from the JToken instance.
        /// </summary>
        /// <param name="key">The name of the key containing the integer.</param>
        /// <returns>An integer from the JToken.</returns>
        public int GetInt(string key)
        {
            try
            {
                return (int)this.value[key];
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("The key {0} doesn't exist in the given token.", key), ex);
            }
        }

        /// <summary>
        /// Returns an unsigned integer from the JToken instance.
        /// </summary>
        /// <param name="key">The name of the key containing the unsigned integer.</param>
        /// <returns>A unsigned integer from the JToken.</returns>
        public uint GetUInt(string key)
        {
            try
            {
                return (uint)this.value[key];
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("The key {0} doesn't exist in the given token.", key), ex);
            }
        }

        /// <summary>
        /// Returns a long integer from the JToken instance.
        /// </summary>
        /// <param name="key">The name of the key containing the long integer.</param>
        /// <returns>A long integer from the JToken.</returns>
        public long GetLong(string key)
        {
            try
            {
                return (long)this.value[key];
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("The key {0} doesn't exist in the given token.", key), ex);
            }
        }

        /// <summary>
        /// Returns an unsigned long integer from the JToken instance.
        /// </summary>
        /// <param name="key">The name of the key containing the unsigned long integer.</param>
        /// <returns>An unsigned long integer from the JToken.</returns>
        public ulong GetULong(string key)
        {
            try
            {
                return (ulong)this.value[key];
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("The key {0} doesn't exist in the given token.", key), ex);
            }
        }

        /// <summary>
        /// Returns a single-precision floating point number from the JToken instance.
        /// </summary>
        /// <param name="key">The name of the key containing the single-precision floating pointer number.</param>
        /// <returns>A single-precision floating point number from the JToken.</returns>
        public float GetFloat(string key)
        {
            try
            {
                return (float)this.value[key];
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("The key {0} doesn't exist in the given token.", key), ex);
            }
        }

        /// <summary>
        /// Returns a double-precision floating point number from the JToken instance.
        /// </summary>
        /// <param name="key">The name of the key containing the double-precision floating point number.</param>
        /// <returns>A double-precision floating point number from the JToken.</returns>
        public double GetDouble(string key)
        {
            try
            {
                return (double)this.value[key];
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("The key {0} doesn't exist in the given token.", key), ex);
            }
        }

        /// <summary>
        /// Returns a decimal number from the JToken instance.
        /// </summary>
        /// <param name="key">The name of the key containing the decimal number.</param>
        /// <returns>A decimal from the JToken.</returns>
        public decimal GetDecimal(string key)
        {
            try
            {
                return (decimal)this.value[key];
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("The key {0} doesn't exist in the given token/", key), ex);
            }
        }

        /// <summary>
        /// Returns a UTF-16 character from the JToken instance.
        /// </summary>
        /// <param name="key">The name of the key containing the UTF-16 character.</param>
        /// <returns>A UTF-16 character from the JToken.</returns>
        public char GetChar(string key)
        {
            return (char)this.value[key];
        }

        /// <summary>
        /// Returns a UTF-16 string from the JToken instance.
        /// </summary>
        /// <param name="key">The name of the key containing the UTF-16 string.</param>
        /// <returns>A UTF-16 string from the JToken.</returns>
        public string GetString(string key)
        {
            return (string)this.value[key];
        }
    }
}
