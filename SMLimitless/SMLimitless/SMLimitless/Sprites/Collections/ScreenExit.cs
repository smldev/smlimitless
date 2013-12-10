using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SMLimitless.Extensions;
using SMLimitless.Interfaces;
using Newtonsoft.Json.Linq;

namespace SMLimitless.Sprites.Collections
{
    public class ScreenExit : ISerializable
    {
        public int SectionIndex { get; private set; }
        public Vector2 Position { get; private set; }
        public ScreenExitBehavior EntranceBehavior { get; private set; }

        public int DestinationSectionIndex { get; private set; }
        public Vector2 DestinationPosition { get; private set; }
        public ScreenExitBehavior ExitBehavior { get; private set; }

        public ScreenExit() { }

        public void Initialize(int sectionIndex, Vector2 position, ScreenExitBehavior entranceBehavior,
                               int destinationSectionIndex, Vector2 destinationPosition, ScreenExitBehavior exitBehavior)
        {
            this.SectionIndex = sectionIndex;
            this.Position = position;
            this.EntranceBehavior = entranceBehavior;

            this.DestinationSectionIndex = destinationSectionIndex;
            this.DestinationPosition = destinationPosition;
            this.ExitBehavior = exitBehavior;
        }

        public string Serialize()
        {
            return JObject.FromObject(this.GetSerializableObjects()).ToString();
        }

        public Object GetSerializableObjects()
        {
            return new
            {
                sectionIndex = this.SectionIndex,
                position = this.Position,
                entranceBehavior = this.EntranceBehavior,
                destinationSectionIndex = this.DestinationSectionIndex,
                destinationPosition = this.DestinationPosition,
                exitBehavior = this.ExitBehavior
            };
        }

        public void Deserialize(string json)
        {
            try
            {
                JObject obj = JObject.Parse(json);
                this.SectionIndex = (int)obj["sectionIndex"];
                this.Position = obj["position"].ToVector2();
                this.EntranceBehavior = (ScreenExitBehavior)(int)obj["entranceBehavior"];
                this.DestinationSectionIndex = (int)obj["destinationSectionIndex"];
                this.DestinationPosition = obj["destinationPosition"].ToVector2();
                this.ExitBehavior = (ScreenExitBehavior)(int)obj["exitBehavior"];
            }
            catch (Exception ex)
            {

            }
        }
    }
}
