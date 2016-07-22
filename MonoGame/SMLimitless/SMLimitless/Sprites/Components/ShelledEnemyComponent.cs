using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SMLimitless.Components;

namespace SMLimitless.Sprites.Components
{
	/// <summary>
	/// A component for an enemy that is shelled (i.e. behaves like
	/// a Koopa Troopa or Buzzy Beetle.)
	/// </summary>
	public sealed class ShelledEnemyComponent : SpriteComponent
	{
		/// <summary>
		/// Enumerates the behaviors of this component.
		/// </summary>
		public enum ShelledEnemyBehavior
		{
			/// <summary>
			/// The enemy will not turn when it reaches a cliff (i.e. green Koopa).
			/// </summary>
			DontTurnOnCliffs = 0,

			/// <summary>
			/// The enemy will turn when it reaches a cliff (i.e. red Koopa).
			/// </summary>
			TurnOnCliffs = 1,

			/// <summary>
			/// The enemy will chase the nearest player (i.e. yellow Koopa).
			/// </summary>
			ChasePlayer = 2
		}

		/// <summary>
		/// Enumerates the states a shelled enemy can be in.
		/// </summary>
		public enum ShelledEnemyState
		{
			/// <summary>
			/// The default; the shelled enemy is walking.
			/// </summary>
			Walking,

			/// <summary>
			/// The shelled enemy has retreated to its shell.
			/// </summary>
			Shell,

			/// <summary>
			/// The shelled enemy is emerging and will soon switch to the <see cref="Walking"/> state.
			/// </summary>
			Emerging,

			/// <summary>
			/// The shelled enemy has retreated to its spinning shell, able to damage other sprites and players.
			/// </summary>
			ShellSpinning
		}

		private const int StateTransitionWaitTime = 5;	// 5 frames = 0.8333 seconds

		private ShelledEnemyBehavior behavior;
		private ShelledEnemyState state;
		private float walkingVelocity;
		private float shellSpinningVelocity;
		private int framesFromShellToEmerging;
		private int framesFromEmergingToWalking;
		private ActionScheduler.ScheduledAction emergeAction;
		private FrameTimer stateTransitionWaitTimer = new FrameTimer();

		// The components below are NOT owned by this component;
		// components cannot own other components. These components
		// are owned by the Owner sprite, we just need to hold on to
		// the references to them.
		private WalkerComponent spriteWalker;
		private DamageComponent spriteDamage;
		private ChasePlayerComponent spriteChasePlayer;

		/// <summary>
		/// Gets or sets the behavior of this component.
		/// </summary>
		public ShelledEnemyBehavior Behavior
		{
			get { return behavior; }
			set
			{
				behavior = value;
				spriteWalker.TurnOnCliffs = (value == ShelledEnemyBehavior.TurnOnCliffs);
				spriteChasePlayer.IsActive = (value == ShelledEnemyBehavior.ChasePlayer);
			}
		}

		/// <summary>
		/// Gets or sets the state of this component.
		/// </summary>
		public ShelledEnemyState State
		{
			get { return state; }
			set
			{
				state = value;
				if (value == ShelledEnemyState.Walking)
				{
					spriteWalker.IsActive = true;
					spriteWalker.CurrentVelocity = walkingVelocity;
					spriteWalker.TurnOnSpriteCollisions = true;
					spriteWalker.TurnOnCliffs = (Behavior == ShelledEnemyBehavior.TurnOnCliffs);
				}
				else if (value == ShelledEnemyState.ShellSpinning)
				{
					spriteWalker.IsActive = true;
					spriteWalker.CurrentVelocity = shellSpinningVelocity;
					spriteWalker.TurnOnCliffs = false;
				}
				else if (value == ShelledEnemyState.Shell || value == ShelledEnemyState.Emerging)
				{
					spriteWalker.IsActive = false;
					spriteWalker.TurnOnSpriteCollisions = false;
				}
				OnStateChanged();
			}
		}

		/// <summary>
		/// An event raised when this component's state changes.
		/// </summary>
		public event EventHandler StateChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="ShelledEnemyComponent"/> sprite.
		/// </summary>
		/// <param name="owner">The sprite that owns this component.</param>
		/// <param name="walkingVelocity">The horizontal velocity at which the owner will walk.</param>
		/// <param name="shellSpinningVelocity">The horizontal velocity at which the owner will spin.</param>
		/// <param name="framesFromShellToEmerging">The number of frames between entering the Shell state and entering the Emerging state.</param>
		/// <param name="framesFromEmergingToWalking">The number of frames between entering the Emerging state and entering the Walking state.</param>
		public ShelledEnemyComponent(Sprite owner, float walkingVelocity, float shellSpinningVelocity, int framesFromShellToEmerging, int framesFromEmergingToWalking)
		{
			WalkerComponent walker = owner.GetComponent<WalkerComponent>();
			DamageComponent damage = owner.GetComponent<DamageComponent>();
			ChasePlayerComponent chasePlayer = owner.GetComponent<ChasePlayerComponent>();

			if (walker == null || damage == null || chasePlayer == null)
			{
				throw new ArgumentException("This component requires its owner to have WalkerComponent, DamageComponent, and ChasePlayerComponent instances before constructing this ShelledEnemyComponent.");
			}

			IsActive = true;
			Owner = owner;
			spriteWalker = walker;
			spriteDamage = damage;
			spriteChasePlayer = chasePlayer;

			behavior = ShelledEnemyBehavior.DontTurnOnCliffs;
			state = ShelledEnemyState.Walking;

			this.walkingVelocity = walkingVelocity;
			this.shellSpinningVelocity = shellSpinningVelocity;
			this.framesFromShellToEmerging = framesFromShellToEmerging;
			this.framesFromEmergingToWalking = framesFromEmergingToWalking;

			spriteWalker.CurrentVelocity = walkingVelocity;
		}

		/// <summary>
		/// Handles a collision with another sprite.
		/// </summary>
		/// <param name="collidingSprite">The sprite that has collided with the owner sprite.</param>
		/// <param name="resolutionDistance">The depth of the collision.</param>
		public override void HandleSpriteCollision(Sprite collidingSprite, Vector2 resolutionDistance)
		{
			if (!IsActive) { return; }

			switch (State)
			{
				case ShelledEnemyState.Walking:
					HandleSpriteCollisionWhileWalking(collidingSprite, resolutionDistance);
					break;
				case ShelledEnemyState.Shell:
				case ShelledEnemyState.Emerging:
					HandleSpriteCollisionWhileShell(collidingSprite, resolutionDistance);
					break;
				case ShelledEnemyState.ShellSpinning:
					HandleSpriteCollisionWhileShellSpinning(collidingSprite, resolutionDistance);
					break;
				default:
					throw new InvalidOperationException();
			}
		}

		private void HandleSpriteCollisionWhileWalking(Sprite sprite, Vector2 resolutionDistance)
		{
			if (sprite.IsPlayer)
			{
				// so the Player gets updated before the shelled enemy does,
				// and since the Player bounces off sprites, when we get here,
				// the player has upward velocity. So a stomp doesn't look like a
				// stomp.
				if (Sprite.IsStomping(sprite, Owner))
				{
					GoToShell();
				}
			}
		}

		private void HandleSpriteCollisionWhileShell(Sprite sprite, Vector2 resolutionDistance)
		{
			if (sprite.IsPlayer)
			{
				// If the player's center is to the right (or equal to) our center, go left
				// Otherwise, go right
				spriteWalker.Direction = (sprite.Hitbox.Center.X >= Owner.Hitbox.X) ? Direction.Left : Direction.Right;
				State = ShelledEnemyState.ShellSpinning;
				ActionScheduler.Instance.CancelScheduledAction(emergeAction);
				emergeAction = null;
			}
		}

		private void HandleSpriteCollisionWhileShellSpinning(Sprite sprite, Vector2 resolutionDistance)
		{
			if (sprite.IsPlayer)
			{
				if (Sprite.IsStomping(sprite, Owner))
				{
					GoToShell();
				}
				else
				{
					spriteDamage.PerformDamage(sprite, SpriteDamageTypes.ShellSpinning, 1);
				}
			}
			else
			{
				var shelledEnemy = sprite.GetComponent<ShelledEnemyComponent>();
				if (shelledEnemy != null && shelledEnemy.State == ShelledEnemyState.ShellSpinning)
				{
					spriteDamage.PerformDamage(sprite, SpriteDamageTypes.ShellSpinning, 1);
					spriteDamage.PerformDamage(Owner, SpriteDamageTypes.ShellSpinning, 1);
				}
				else
				{
					spriteDamage.PerformDamage(sprite, SpriteDamageTypes.ShellSpinning, 1);
				}
			}
		}

		private void GoToShell()
		{
			State = ShelledEnemyState.Shell;
			spriteChasePlayer.IsActive = false;
			emergeAction = ActionScheduler.Instance.ScheduleAction(() =>
			{
				State = ShelledEnemyState.Emerging;
				emergeAction = ActionScheduler.Instance.ScheduleActionOnNextFrame(() =>
				{
					State = ShelledEnemyState.Walking;
					spriteChasePlayer.IsActive = (Behavior == ShelledEnemyBehavior.ChasePlayer);
				}, framesFromEmergingToWalking);
			}, framesFromShellToEmerging);
		}

		/// <summary>
		/// Updates this component.
		/// </summary>
		public override void Update()
		{
			stateTransitionWaitTimer.Update();
		}

		private void OnStateChanged()
		{
			if (StateChanged != null)
			{
				StateChanged(this, new EventArgs());
			}
		}
	}
}
