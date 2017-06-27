using System;
using System.Collections.Generic;
using System.Linq;
using SMLimitless.Extensions;
using SMLimitless.Physics;

namespace SMLimitless.Sprites.Components
{
	/// <summary>
	///   A component that indicates in which direction the nearest player is to
	///   a sprite.
	/// </summary>
	public sealed class ChasePlayerComponent : SpriteComponent
	{
		private List<float> distancesToPlayers = new List<float>();
		private int framesUntilNextCheck;

		/// <summary>
		///   Gets the number of frames between any two checks of the nearest
		///   player's direction.
		/// </summary>
		public int FramesBetweenDirectionChecks { get; private set; }

		/// <summary>
		///   Gets a value which indicates the direction the nearest player is to
		///   a sprite.
		/// </summary>
		public FlaggedDirection NearestPlayerDirection { get; private set; } = FlaggedDirection.None;

		/// <summary>
		///   An event raised when the direction of the nearest player has been updated.
		/// </summary>
		/// <remarks>
		///   This event is raised any time that <see
		///   cref="PerformDirectionCheck" /> is called, even if the direction
		///   itself doesn't change.
		/// </remarks>
		public event EventHandler NearestPlayerDirectionUpdated;

		/// <summary>
		///   Initializes a new instance of the <see cref="ChasePlayerComponent"
		///   /> class.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="framesBetweenChecks"></param>
		public ChasePlayerComponent(Sprite owner, int framesBetweenChecks)
		{
			Owner = owner;
			FramesBetweenDirectionChecks = framesBetweenChecks;
		}

		/// <summary>
		///   Updates this component.
		/// </summary>
		public override void Update()
		{
			if (!IsActive) { return; }
			if (framesUntilNextCheck == 0)
			{
				PerformDirectionCheck();
				framesUntilNextCheck = FramesBetweenDirectionChecks;
			}
			else
			{
				framesUntilNextCheck--;
			}
		}

		private Sprite GetNearestPlayer()
		{
			var players = Owner.Owner.Players;
			distancesToPlayers.Clear();

			for (int i = 0; i < players.Count; i++)
			{
				distancesToPlayers.Add(players[i].Hitbox.Center.GetDistance(Owner.Hitbox.Center));
			}

			int indexOfNearestPlayer = distancesToPlayers.IndexOf(distancesToPlayers.Min());
			return players[indexOfNearestPlayer];
		}

		private void OnNearestPlayerDirectionUpdated()
		{
			NearestPlayerDirectionUpdated?.Invoke(this, new EventArgs());
		}

		private void PerformDirectionCheck()
		{
			var nearestPlayer = GetNearestPlayer();
			var playerCenter = nearestPlayer.Hitbox.Center;
			var ownerCenter = Owner.Hitbox.Center;

			NearestPlayerDirection = FlaggedDirection.None;

			if (playerCenter.X < ownerCenter.X) { NearestPlayerDirection |= FlaggedDirection.Left; }
			else { NearestPlayerDirection |= FlaggedDirection.Right; }

			if (playerCenter.Y < ownerCenter.Y) { NearestPlayerDirection |= FlaggedDirection.Up; }
			else { NearestPlayerDirection |= FlaggedDirection.Down; }

			OnNearestPlayerDirectionUpdated();
		}
	}
}
