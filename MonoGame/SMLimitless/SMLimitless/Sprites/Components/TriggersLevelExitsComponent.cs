using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMLimitless.Sprites.Components
{
	/// <summary>
	/// A component for a sprite that can trigger level exits (like players).
	/// </summary>
	public sealed class TriggersLevelExitsComponent : SpriteComponent
	{
		public event EventHandler<LevelExitTriggeredEventArgs> LevelExitTriggered;

		/// <summary>
		/// Updates this component. Called once per frame.
		/// </summary>
		public override void Update() { }

		/// <summary>
		/// Call this method when the sprite triggers a level exit.
		/// </summary>
		/// <param name="levelExit">The exit that was triggered.</param>
		public void ExitTriggered(Sprite levelExit)
		{
			LevelExitTriggered?.Invoke(this, new LevelExitTriggeredEventArgs(levelExit));
		}
	}

	/// <summary>
	/// Arguments for an event that occurs when a level exit is triggered.
	/// </summary>
	public sealed class LevelExitTriggeredEventArgs : EventArgs
	{
		/// <summary>
		/// The level exit that was triggered.
		/// </summary>
		public Sprite LevelExitSprite { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LevelExitTriggeredEventArgs"/> class.
		/// </summary>
		/// <param name="levelExit">The level exit that was triggered.</param>
		public LevelExitTriggeredEventArgs(Sprite levelExit)
		{
			LevelExitSprite = levelExit ?? throw new ArgumentNullException(nameof(levelExit), "The level exit sprite was null.");
		}
	}
}
