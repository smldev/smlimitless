using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SMLimitless.Interfaces
{
    public interface ISerializable
    {
        string Serialize();
        object GetSerializableObjects();
        void Deserialize(string json);
    }
}
