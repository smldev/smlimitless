using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLimitless.Interfaces;
using Newtonsoft.Json.Linq;

namespace SMLimitless.Sprites.Collections
{
    public class EventScript : ISerializable
    {
        public string Script { get; set; }

        public EventScript()
        {
        }

        public object GetSerializableObjects()
        {
            return new
            {
                script = this.Script
            };
        }

        public string Serialize()
        {
            return JObject.FromObject(this.GetSerializableObjects()).ToString();
        }

        public void Deserialize(string json)
        {
            JObject obj = JObject.Parse(json);
            this.Script = (string)obj["script"];
        }
    }
}
