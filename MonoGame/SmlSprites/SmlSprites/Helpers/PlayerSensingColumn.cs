using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless.Physics;
using SMLimitless.Sprites;
using SMLimitless.Sprites.Collections;

namespace SmlSprites.Helpers
{
	public sealed class PlayerSensingColumn
	{
		private VerticalLinePlayerListener columnLeft;
		private VerticalLinePlayerListener columnRight;
		private Section owner;
		private List<Sprite> players = new List<Sprite>();

		public IReadOnlyList<Sprite> PlayersWithin { get { return players.AsReadOnly(); } }
		public bool HasPlayers { get { return players.Any(); } }

		private BoundingRectangle ColumnBounds
		{
			get
			{
				return new BoundingRectangle(columnLeft.Position.X, 0f, (columnRight.Position.X - columnLeft.Position.X), owner.Bounds.Height);
			}
		}

		public PlayerSensingColumn(Section section, float columnLeftX, float columnRightX)
		{
			owner = section;

			columnLeft = new VerticalLinePlayerListener();
			columnRight = new VerticalLinePlayerListener();

			columnLeft.Position = new Vector2(columnLeftX, 0f);
			columnRight.Position = new Vector2(columnRightX, 0f);

			columnLeft.Initialize(section);
			columnRight.Initialize(section);

			columnLeft.ListenerTriggeredEvent += Column_ListenerTriggeredEvent;
			columnRight.ListenerTriggeredEvent += Column_ListenerTriggeredEvent;

			section.AddSprite(columnLeft);
			section.AddSprite(columnRight);
		}

		private void Column_ListenerTriggeredEvent(object sender, ListenerTriggeredEventArgs e)
		{
			
		}

		public void Update()
		{
			players.Clear();
			BoundingRectangle columnBounds = ColumnBounds;
			foreach (Sprite player in owner.PlayerList)
			{
				if (player.Hitbox.IntersectsIncludingEdges(columnBounds)) { players.Add(player); }
			}
		}
	}
}
