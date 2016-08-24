using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Assemblies;
using SMLimitless.Sprites.Collections;

namespace SmlSprites.Helpers
{
	public sealed class VerticalLinePlayerListener : Sprite
	{
		public override string EditorCategory => "Helper Sprites";
		public event EventHandler<ListenerTriggeredEventArgs> ListenerTriggeredEvent;

		public VerticalLinePlayerListener()
		{
			Size = new Vector2(1f);
		}

		public override void DeserializeCustomObjects(JsonHelper customObjects)
		{
		}

		public override void Initialize(Section owner)
		{
			ActiveState = SpriteActiveState.AlwaysActive;
			base.Initialize(owner);
		}

		public override void Draw()
		{
		}

		public override void Draw(Rectangle cropping)
		{
		}

		public override object GetCustomSerializableObjects()
		{
			return null;
		}

		public override void LoadContent()
		{
		}

		public override void Update()
		{
			Position = new Vector2(Position.X, 0f);
			foreach (Sprite player in Owner.PlayerList)
			{
				float hitboxXMinimum = player.Hitbox.Left;
				float hitboxXMaximum = player.Hitbox.Right;

				if (Position.X >= hitboxXMinimum && Position.X <= hitboxXMaximum)
				{
					OnListenerTriggered(player);
				}
			}
		}

		private void OnListenerTriggered(Sprite player)
		{
			if (ListenerTriggeredEvent != null)
			{
				ListenerTriggeredEvent(this, new ListenerTriggeredEventArgs(player));
			}
		}
	}
}
