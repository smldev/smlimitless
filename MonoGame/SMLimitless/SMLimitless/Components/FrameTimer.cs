using System;

namespace SMLimitless.Components
{
	/// <summary>
	///   A timer that fires an event after a defined number of frames.
	/// </summary>
	public sealed class FrameTimer
	{
		private int framesLeft;
		private bool isActive;

		/// <summary>
		///   Gets a value indicating whether the timer has expired.
		/// </summary>
		public bool TimerExpired { get; private set; }

		/// <summary>
		///   An event that is fired when the timer expires.
		/// </summary>
		public event EventHandler TimerExpiredEvent;

		/// <summary>
		///   Cancels the timer and removes any frames remaining.
		/// </summary>
		public void Cancel()
		{
			if (isActive)
			{
				isActive = false;
				framesLeft = 0;
			}
			else
			{
				throw new InvalidOperationException("Tried to cancel a timer that wasn't running.");
			}
		}

		/// <summary>
		///   Pauses the timer so that it can be resumed from where it was later.
		/// </summary>
		public void Pause()
		{
			if (isActive) { isActive = false; }
			else
			{
				string message = (framesLeft > 0) ? "Tried to pause a timer that was already paused." : "Tried to pause a timer that wasn't running.";
				throw new InvalidOperationException(message);
			}
		}

		/// <summary>
		///   Resets the timer.
		/// </summary>
		public void Reset()
		{
			isActive = false;
			TimerExpired = false;
			framesLeft = 0;
		}

		/// <summary>
		///   Restarts the timer with a new number of frames.
		/// </summary>
		/// <param name="frames">The number of frames until the timer expires.</param>
		public void Restart(int frames)
		{
			isActive = false;
			framesLeft = 0;
			Start(frames);
		}

		/// <summary>
		///   Starts the timer to expire in a certain number of frames.
		/// </summary>
		/// <param name="frames">The number of frames before the timer expires.</param>
		public void Start(int frames)
		{
			if (!isActive)
			{
				isActive = true;
				TimerExpired = false;
				framesLeft = frames;
			}
			else
			{
				throw new InvalidOperationException("Tried to start a timer that was already running.");
			}
		}

		/// <summary>
		///   Updates this timer, decrementing one frame.
		/// </summary>
		public void Update()
		{
			if (!isActive) { return; }

			if (framesLeft > 0)
			{
				framesLeft--;
			}
			else
			{
				TimerExpired = true;
				OnTimerExpired();
				isActive = false;
			}
		}

		private void OnTimerExpired()
		{
			TimerExpiredEvent?.Invoke(this, new EventArgs());
		}
	}
}
