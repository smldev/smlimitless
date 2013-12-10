using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SMLimitless.Sprites.Assemblies
{
    public class JsonHelper
    {
        private JToken value;

        public JsonHelper(JToken token)
        {
            this.value = token;
        }

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

        public char GetChar(string key)
        {
            return (char)this.value[key];
        }

        public string GetString(string key)
        {
            return (string)this.value[key];
        }

        public static string Serialize(object value)
        {
            return JObject.FromObject(value).ToString();
        }
    }
}
