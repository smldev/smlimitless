using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMLimitless.Sprites;

namespace SmlSprites.Helpers
{
	public sealed class ListenerTriggeredEventArgs : EventArgs
	{
		public Sprite TriggeringSprite { get; private set; }

		public ListenerTriggeredEventArgs(Sprite triggeringSprite)
		{
			TriggeringSprite = triggeringSprite;
		}
	}
}
