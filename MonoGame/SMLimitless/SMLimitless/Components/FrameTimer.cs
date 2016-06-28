using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMLimitless.Components
{
	public sealed class FrameTimer
	{
		private bool isActive;
		private int framesLeft;

		public bool TimerExpired { get; private set; }

		public event EventHandler TimerExpiredEvent;

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

		public void Pause()
		{
			if (isActive) { isActive = false; }
			else
			{
				string message = (framesLeft > 0) ? "Tried to pause a timer that was already paused." : "Tried to pause a timer that wasn't running.";
				throw new InvalidOperationException(message);
			}
		}

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

		public void Reset()
		{
			isActive = false;
			TimerExpired = false;
			framesLeft = 0;
		}

		public void Restart(int frames)
		{
			isActive = false;
			framesLeft = 0;
			Start(frames);
		}

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
			if (TimerExpiredEvent != null)
			{
				TimerExpiredEvent(this, new EventArgs());
			}
		}
	}
}
