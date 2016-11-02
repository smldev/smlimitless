using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SMLimitless.Content;
using SMLimitless.Extensions;
using SMLimitless.Input;
using SMLimitless.Interfaces;
using SMLimitless.Physics;
using SMLimitless.Sounds;

namespace SMLimitless.Sprites.Collections
{
	/// <summary>
	/// An object in a section that players can use to teleport inside and between sections.
	/// </summary>
	public sealed class SectionExit
	{
		/// <summary>
		/// Keeps track of the state of an exit effect - the visual effect of
		/// a player moving into an exit.
		/// </summary>
		/// <remarks>
		/// Exit effects can have up to two stages - one where the player moves
		/// laterally to be centered in the exit, and another where the player
		/// moves into the exit itself. Lateral motion occurs iff the player is
		/// off-center on a vertical pipe. If it occurs, it will take half the
		/// duration of the effect.
		/// 
		/// Shift motion moves the player into the exit. It is either the second
		/// half of the effect or the entire effect.
		/// </remarks>
		private class ExitEffect
		{
			// roughly the length of the pipe sound effect
			public const int StandardDurationFrames = 47;

			private Sprite player;
			private Direction shiftDirection = Direction.None;

			/// <summary>
			/// Gets the total length of the effect in frames.
			/// </summary>
			public int TotalEffectDurationFrames { get; }

			/// <summary>
			/// Gets the number of frames remaining in the effect.
			/// </summary>
			public int RemainingEffectFrames { get; private set; }

			/// <summary>
			/// Gets the total distance the player must move laterally.
			/// </summary>
			public float TotalLateralShiftDistance { get; }

			/// <summary>
			/// Gets the remaining distance the player must move laterally.
			/// </summary>
			public float RemainingLateralShiftDistance { get; private set; }

			/// <summary>
			/// Gets the distance the player must move laterally on each frame.
			/// </summary>
			public float LateralShiftDelta => (TotalLateralShiftDistance / 60f) * 2f;

			/// <summary>
			/// Gets the total distance the player must shift.
			/// </summary>
			public float TotalShiftDistance { get; private set; }

			/// <summary>
			/// Gets the remaining distance the player must shift.
			/// </summary>
			public float RemainingShiftDistance { get; }

			/// <summary>
			/// An event fired when an effect completes.
			/// </summary>
			public event EventHandler<Sprite> EffectCompletedEvent;

			/// <summary>
			/// Gets the distance the player must shift on each frame.
			/// </summary>
			public float ShiftDelta
			{
				get
				{
					if (TotalLateralShiftDistance > 0f) { return (TotalShiftDistance / 60f) * 2f; }
					else { return TotalShiftDistance / 60f; }
				}
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="ExitEffect"/> class. 
			/// </summary>
			/// <param name="player">The player entering or leaving the exit.</param>
			/// <param name="exitHitbox">The hitbox of the exit being entered or left.</param>
			/// <param name="shiftDirection">The direction the player must move to enter or emerge from the exit.</param>
			public ExitEffect(Sprite player, BoundingRectangle exitHitbox, Direction shiftDirection)
			{
				this.player = player;
				this.shiftDirection = shiftDirection;

				TotalEffectDurationFrames = StandardDurationFrames;
				RemainingEffectFrames = TotalEffectDurationFrames;

				if (Math.Abs(exitHitbox.Center.X - player.Hitbox.Center.X) < 0.1f)
				{
					// No lateral movement required
					TotalLateralShiftDistance = 0f;
					RemainingLateralShiftDistance = 0f;
				}
				else
				{
					// Lateral movement required
					TotalLateralShiftDistance = player.Hitbox.Center.X - exitHitbox.Center.X;
					RemainingLateralShiftDistance = TotalLateralShiftDistance;
				}

				if (shiftDirection == Direction.Up) { TotalShiftDistance = exitHitbox.Bottom - player.Hitbox.Bottom; }
				else if (shiftDirection == Direction.Down){ TotalShiftDistance = exitHitbox.Bottom - player.Hitbox.Top;}
				else if (shiftDirection == Direction.Left) { TotalShiftDistance = exitHitbox.Left - player.Hitbox.Right; }
				else if (shiftDirection == Direction.Right) { TotalShiftDistance = exitHitbox.Right - player.Hitbox.Left; }
				else { throw new ArgumentException($"Invalid direction {shiftDirection}", nameof(shiftDirection)); }

				RemainingShiftDistance = TotalShiftDistance;
			}

			/// <summary>
			/// Updates this effect.
			/// </summary>
			public void Update()
			{
				if (RemainingEffectFrames > 0)
				{
					RemainingEffectFrames--;

					if (Math.Abs(RemainingLateralShiftDistance) > 0.1f)
					{
						player.Position = new Vector2(player.Position.X + LateralShiftDelta, player.Position.Y);
					}
					else
					{ 
						if (shiftDirection == Direction.Up || shiftDirection == Direction.Down)
						{
							player.Position = new Vector2(player.Position.X, player.Position.Y + ShiftDelta);
						}
						else if (shiftDirection == Direction.Left || shiftDirection == Direction.Right)
						{
							player.Position = new Vector2(player.Position.X + ShiftDelta, player.Position.Y);
						}
						else { throw new InvalidOperationException($"The shift direction has been modified to {shiftDirection}"); }
					}
				}
				else
				{
					OnEffectCompleted();
				}
			}

			/// <summary>
			/// Draws the player entering this effect.
			/// </summary>
			public void Draw()
			{
				player.Draw();
			}

			private void OnEffectCompleted()
			{
				EffectCompletedEvent?.Invoke(this, player);
			}
		}

		/// <summary>
		/// The length of delay between an iris-in finishing and an iris-out
		/// beginning after a player has entered a section exit.
		/// </summary>
		private const float DefaultTransitionTimeout = 1.0f;

		/// <summary>
		/// The length of time that all players have to enter a section exit.
		/// </summary>
		private const float DefaultMultiplayerWait = 5.0f;

		private ExitTransitionState state = ExitTransitionState.NoTransitionOccurring;
		private int remainingTransitionTimeoutFrames;
		private int remainingMultiplayerWaitFrames;
		private ExitEffect exitEffect = null;
		private CachedSound warpSound;

		/// <summary>
		/// Gets a value indicating the type of this section exit.
		/// </summary>
		public SectionExitType ExitType { get; internal set; }

		/// <summary>
		/// Gets the section that this exit is within.
		/// </summary>
		public Section Owner { get; private set; }

		/// <summary>
		/// Gets an integer used to identify this exit.
		/// </summary>
		public int ID { get; internal set; }

		/// <summary>
		/// Gets an integer used to identify the other exit in the pair.
		/// </summary>
		public int OtherID { get; internal set; }

		/// <summary>
		/// Gets the position of this section exit in the section.
		/// </summary>
		public Vector2 Position { get; internal set; }

		/// <summary>
		/// Gets the size of this section exit, guaranteed to be a multiple
		/// of <see cref="GameServices.GameObjectSize"/>. 
		/// </summary>
		public Vector2 Size { get; internal set; }

		/// <summary>
		/// Gets the hitbox of this section exit.
		/// </summary>
		public BoundingRectangle Hitbox => new BoundingRectangle(Position.X, Position.Y, Size.X, Size.Y);

		/// <summary>
		/// Gets the source behavior of this exit.
		/// </summary>
		public ExitSourceBehavior SourceBehavior { get; internal set; }

		/// <summary>
		/// Gets the destination behavior of this exit.
		/// </summary>
		public ExitDestinationBehavior DestinationBehavior { get; internal set; }

		/// <summary>
		/// Gets a point for the iris effect to close to or open from.
		/// </summary>
		public Vector2 IrisPoint
		{
			get
			{
				if (ExitType == SectionExitType.Source || ExitType == SectionExitType.TwoWay)
				{
					switch (SourceBehavior)
					{
						case ExitSourceBehavior.PipeDown: return Hitbox.BottomCenter;
						case ExitSourceBehavior.PipeUp: return Hitbox.TopCenter;
						case ExitSourceBehavior.PipeLeft: return Hitbox.LeftCenter;
						case ExitSourceBehavior.PipeRight: return Hitbox.RightCenter;
						case ExitSourceBehavior.Door:
						case ExitSourceBehavior.Immediate: return Hitbox.Center;
						default: throw new InvalidOperationException($"A source section exit cannot have a behavior of {SourceBehavior}.");
					}
				}
				else if (ExitType == SectionExitType.Destination)
				{
					switch (DestinationBehavior)
					{
						case ExitDestinationBehavior.PipeUp: return Hitbox.BottomCenter;
						case ExitDestinationBehavior.PipeDown: return Hitbox.TopCenter;
						case ExitDestinationBehavior.PipeRight: return Hitbox.LeftCenter;
						case ExitDestinationBehavior.PipeLeft: return Hitbox.RightCenter;
						case ExitDestinationBehavior.None: return Hitbox.Center;
						default: throw new InvalidOperationException($"A destination section exit cannot have a behavior of {DestinationBehavior}.");
					}
				}
				else { throw new InvalidOperationException($"This section exit is neither a source nor a destination."); }
			}
		}

		/// <summary>
		/// Gets a queue of players that have entered the exit.
		/// </summary>
		public Queue<Sprite> PlayersInExit { get; private set; } = new Queue<Sprite>();

		private bool IsMultiplayer => Owner.PlayerList.Count + PlayersInExit.Count > 1;

		/// <summary>
		/// Initializes a new instance of the <see cref="SectionExit"/> class. 
		/// </summary>
		/// <param name="owner">The section this exit is in.</param>
		public SectionExit(Section owner)
		{
			Owner = owner;

			warpSound = new CachedSound(ContentPackageManager.GetAbsoluteFilePath("nsmbwiiWarpPipe"));
		}

		/// <summary>
		/// Returns a value indicating whether a player can enter this exit.
		/// </summary>
		/// <param name="player">The player to check for.</param>
		/// <returns>
		/// False if the sprite is not a player, this exit is not a source or
		/// a two-way exit, other players have entered a different exit, or
		/// if the player is not intersecting the exit.
		/// True if the player meets the above conditions and the user is
		/// pressing the correct button.
		/// </returns>
		public bool CanPlayerEnter(Sprite player)
		{
			if (!player.IsPlayer) { return false; }
			if (ExitType != SectionExitType.Source && ExitType != SectionExitType.TwoWay) { return false; }
			if (!player.Hitbox.Intersects(Hitbox)) { return false; }
			if (Owner.ExitLock != null && Owner.ExitLock != this) { return false; }

			bool onGround = player.IsOnGround;
			bool upPressed = InputManager.IsCurrentActionPress(InputAction.Up);
			bool downPressed = InputManager.IsCurrentActionPress(InputAction.Down);
			bool leftPressed = InputManager.IsCurrentActionPress(InputAction.Left);
			bool rightPressed = InputManager.IsCurrentActionPress(InputAction.Right);
			bool tileAdjacent = IsSolidTileNextToExit();

			switch (SourceBehavior)
			{
				case ExitSourceBehavior.Default:
					break;
				case ExitSourceBehavior.NotASource:
					break;
				case ExitSourceBehavior.PipeDown:
					return onGround && downPressed && tileAdjacent;
				case ExitSourceBehavior.PipeUp:
					return !onGround && upPressed && tileAdjacent;
				case ExitSourceBehavior.PipeLeft:
					return onGround && leftPressed && tileAdjacent;
				case ExitSourceBehavior.PipeRight:
					return onGround && rightPressed && tileAdjacent;
				case ExitSourceBehavior.Door:
					return onGround && upPressed;
				case ExitSourceBehavior.Immediate:
					return true;
				default:
					throw new InvalidOperationException($"The source behavior is the invalid value {SourceBehavior}.");
			}

			throw new InvalidOperationException("Unreachable code reached.");
		}

		private bool IsSolidTileNextToExit()
		{
			string message = "This exit is not a source exit, or is a door/immediate source; an adjacent solid tile check is unnecessary.";
			if (ExitType != SectionExitType.Source && ExitType != SectionExitType.TwoWay)
			{
				throw new InvalidOperationException(message);
			}

			Vector2 checkPosition;

			switch (SourceBehavior)
			{
				case ExitSourceBehavior.PipeDown:
					checkPosition = Hitbox.BottomCenter.Move(Direction.Down, GameServices.GameObjectSize.Y);
					break;
				case ExitSourceBehavior.PipeUp:
					checkPosition = Hitbox.TopCenter.Move(Direction.Up, GameServices.GameObjectSize.Y);
					break;
				case ExitSourceBehavior.PipeLeft:
					checkPosition = Hitbox.LeftCenter.Move(Direction.Left, GameServices.GameObjectSize.X);
					break;
				case ExitSourceBehavior.PipeRight:
					checkPosition = Hitbox.RightCenter.Move(Direction.Right, GameServices.GameObjectSize.X);
					break;
				case ExitSourceBehavior.Door:
				case ExitSourceBehavior.Immediate:
				case ExitSourceBehavior.Default:
				case ExitSourceBehavior.NotASource:
				default:
					throw new InvalidOperationException(message);
			}

			return Owner.GetTileAtPosition(checkPosition) != null;
		}

		/// <summary>
		/// A method called when a player enters a section exit.
		/// </summary>
		/// <param name="player">The player that has entered the section exit.</param>
		public void PlayerEntered(Sprite player)
		{
			state = (SourceBehavior != ExitSourceBehavior.Door) ? ExitTransitionState.ExitEffectInProgress :
					(IsMultiplayer) ? (Owner.PlayerList.Count > 0) ? ExitTransitionState.MultiplayerWait : ExitTransitionState.IrisIn
					: ExitTransitionState.IrisIn;
			Owner.RemoveSpriteOnNextFrame(player);
			PlayersInExit.Enqueue(player);
			
			if (SourceBehavior != ExitSourceBehavior.Door)
			{
				AudioPlaybackEngine.Instance.PlaySound(warpSound, (sender, e) => { });
				Direction shiftDirection = GetSourceShiftDirection();
				exitEffect = new ExitEffect(player, Hitbox, shiftDirection);
				exitEffect.EffectCompletedEvent += ExitEffect_EffectCompletedEvent;
			}
		}

		private void ExitEffect_EffectCompletedEvent(object sender, Sprite e)
		{
			exitEffect = null;
			if (!IsMultiplayer)
			{
				StartIrisIn();
			}
			else
			{
				if (Owner.PlayerList.Count > 0)
				{
					Owner.ExitLock = this;
					Owner.CameraSystem.IsFrozen = true;
					remainingMultiplayerWaitFrames = (int)(DefaultMultiplayerWait * 60f);
					state = ExitTransitionState.MultiplayerWait;
				}
				else
				{
					remainingMultiplayerWaitFrames = 0;
					StartIrisIn();
				}
			}
		}

		private void StartIrisIn()
		{
			Owner.IsActive = false;
			state = ExitTransitionState.IrisIn;

			Action<object, EffectCompletedEventArgs> onIrisClose = (s, args) =>
			{
				remainingTransitionTimeoutFrames = (int)(DefaultTransitionTimeout * 60f);
				state = ExitTransitionState.TransitionDelay;
			};

			Owner.IrisIn(90, IrisPoint, onIrisClose);
		}

		/// <summary>
		/// Updates this section exit.
		/// </summary>
		public void Update()
		{
			exitEffect?.Update();

			if (state == ExitTransitionState.MultiplayerWait)
			{
				if (remainingMultiplayerWaitFrames > 0) { remainingMultiplayerWaitFrames--; }
				else
				{
					StartIrisIn();
				}
			}

			if (state == ExitTransitionState.TransitionDelay)
			{
				if (remainingTransitionTimeoutFrames > 0) { remainingTransitionTimeoutFrames--; }
				else
				{
					Owner.Owner.OnSectionExit(this);
					state = ExitTransitionState.NoTransitionOccurring;
				}
			}
		}

		/// <summary>
		/// Draws a rectangle showing the hitbox of this exit.
		/// </summary>
		public void Draw()
		{
			GameServices.SpriteBatch.DrawRectangleEdges(new Rectangle(Position.ToPoint(), Size.ToPoint()), Color.AliceBlue);
		}

		private Direction GetSourceShiftDirection()
		{
			switch (SourceBehavior)
			{
				case ExitSourceBehavior.PipeDown:
					return Direction.Down;
				case ExitSourceBehavior.PipeUp:
					return Direction.Up;
				case ExitSourceBehavior.PipeLeft:
					return Direction.Left;
				case ExitSourceBehavior.PipeRight:
					return Direction.Right;
				case ExitSourceBehavior.Door:
					throw new InvalidOperationException("A door has no shift direction.");
				case ExitSourceBehavior.Immediate:
					throw new InvalidOperationException("An immediate exit has no shift direction.");
				case ExitSourceBehavior.Default:
				case ExitSourceBehavior.NotASource:
				default:
					throw new InvalidOperationException($"Invalid source behavior {SourceBehavior}.");
			}
		}

		private Direction GetDestinationShiftDirection()
		{
			switch (DestinationBehavior)
			{
				case ExitDestinationBehavior.PipeUp:
					return Direction.Up;
				case ExitDestinationBehavior.PipeDown:
					return Direction.Left;
				case ExitDestinationBehavior.PipeRight:
					return Direction.Right;
				case ExitDestinationBehavior.PipeLeft:
					return Direction.Left;
				case ExitDestinationBehavior.None:
					throw new InvalidOperationException("A destination with no behavior has no shift direction.");
				case ExitDestinationBehavior.Default:
				case ExitDestinationBehavior.NotADestination:
				default:
					throw new InvalidOperationException($"Invalid destination behavior {DestinationBehavior}.");
			}
		}

		/// <summary>
		/// Places the player to emerge from the correct location in the section exit.
		/// </summary>
		/// <param name="player">The player set to emerge.</param>
		public void PlacePlayerToEmerge(Sprite player)
		{
			if (ExitType != SectionExitType.Destination && ExitType != SectionExitType.TwoWay)
			{
				throw new InvalidOperationException("A player cannot emerge from a source.");
			}

			Vector2 emergePosition = Vector2.Zero;
			switch (DestinationBehavior)
			{
				case ExitDestinationBehavior.PipeUp:
					player.Position = new Vector2(Hitbox.GetCenterAlignedInColumnPosition(player.Hitbox).X, Hitbox.Top);
					break;
				case ExitDestinationBehavior.PipeDown:
					player.Position = new Vector2(Hitbox.GetCenterAlignedInColumnPosition(player.Hitbox).X, Hitbox.Bottom - player.Hitbox.Height);
					break;
				case ExitDestinationBehavior.PipeRight:
					player.Position = new Vector2(Hitbox.Left - player.Hitbox.Width, Hitbox.Bottom);
					break;
				case ExitDestinationBehavior.PipeLeft:
					player.Position = new Vector2(Hitbox.Right + player.Hitbox.Width, Hitbox.Bottom);
					break;
				case ExitDestinationBehavior.None:
					player.Position = new Vector2(Hitbox.GetCenterAlignedInColumnPosition(player.Hitbox).X, Hitbox.Bottom - player.Hitbox.Height);
					break;
				case ExitDestinationBehavior.Default:
				case ExitDestinationBehavior.NotADestination:
				default:
					throw new InvalidOperationException($"Invalid destination behavior {DestinationBehavior}");
			}
		}

		private BoundingRectangle GetEmergeHitbox(Sprite player)
		{
			if (ExitType != SectionExitType.Destination && ExitType != SectionExitType.TwoWay)
			{
				throw new InvalidOperationException("A player cannot emerge from a source.");
			}

			switch (DestinationBehavior)
			{
				case ExitDestinationBehavior.PipeUp:
					return new BoundingRectangle(Position.X, Position.Y - Size.Y, Size.X, Size.Y);
				case ExitDestinationBehavior.PipeDown:
					return new BoundingRectangle(Position.X, Hitbox.Bottom, Size.X, Size.Y);
				case ExitDestinationBehavior.PipeRight:
					return new BoundingRectangle(Hitbox.Right, Position.Y, Size.X, Size.Y);
				case ExitDestinationBehavior.PipeLeft:
					return new BoundingRectangle(Hitbox.Left - Size.X, Position.Y, Size.X, Size.Y);
				case ExitDestinationBehavior.None:
					return Hitbox;
				case ExitDestinationBehavior.Default:
				case ExitDestinationBehavior.NotADestination:
				default:
					throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Queues a sequence of players to emerge from a section exit.
		/// </summary>
		/// <param name="players"></param>
		public void Emerge(IEnumerable<Sprite> players)
		{
			state = ExitTransitionState.ExitEffectInProgress;
			players.ForEach(p => PlayersInExit.Enqueue(p));

			var first = PlayersInExit.Dequeue();
			PlacePlayerToEmerge(first);
			exitEffect = new ExitEffect(first, GetEmergeHitbox(first), GetDestinationShiftDirection());
			AudioPlaybackEngine.Instance.PlaySound(warpSound, (sender, e) => { });

			EventHandler<Sprite> recursiveHandler = null;

			recursiveHandler = (sender, e) =>
			{
				if (PlayersInExit.Count > 0)
				{
					Owner.AddSprite(e);
					var next = PlayersInExit.Dequeue();
					exitEffect = new ExitEffect(next, GetEmergeHitbox(next), GetDestinationShiftDirection());
					exitEffect.EffectCompletedEvent += recursiveHandler;
					AudioPlaybackEngine.Instance.PlaySound(warpSound, (s, args) => { });
				}
				else
				{
					Owner.AddSprite(e);
					exitEffect = null;
				}
			};

			exitEffect.EffectCompletedEvent += recursiveHandler;
		}
	}
}