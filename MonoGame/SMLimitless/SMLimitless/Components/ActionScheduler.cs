using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMLimitless.Components
{
	/// <summary>
	/// A class that can schedule an action to occur in a certain number of frames.
	/// </summary>
	public sealed class ActionScheduler
	{
		/// <summary>
		/// Represents an action to be executed in a certain number of frames.
		/// </summary>
		public class ScheduledAction
		{
			/// <summary>
			/// The action to execute.
			/// </summary>
			public Action Action { get; }
			
			/// <summary>
			/// The number of frames left until the action is executed.
			/// </summary>
			public int FramesUntilExecution { get; set; }

			/// <summary>
			/// Initializes a new instance of the <see cref="ScheduledAction"/> class.
			/// </summary>
			/// <param name="action">The action to execute.</param>
			/// <param name="framesUntilExecution">The number of frames until the action is executed.</param>
			public ScheduledAction(Action action, int framesUntilExecution)
			{
				Action = action;
				FramesUntilExecution = framesUntilExecution;
			}
		}

		public static ActionScheduler Instance { get; } = new ActionScheduler();

		private List<ScheduledAction> scheduledActions = new List<ScheduledAction>();
		private List<ScheduledAction> actionsToScheduleNextFrame = new List<ScheduledAction>();

		/// <summary>
		/// Schedules an action to occur in a certain number of frames.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		/// <param name="framesUntilExecution">The number of frames until the action is executed.</param>
		public ScheduledAction ScheduleAction(Action action, int framesUntilExecution)
		{
			var result = new ScheduledAction(action, framesUntilExecution);
			scheduledActions.Add(result);
			return result;
		}

		public ScheduledAction ScheduleActionOnNextFrame(Action action, int framesUntilExecution)
		{
			// Subtract one frame from the frames until execution because we
			// have to wait until the next frame to actually schedule the action.
			var result = new ScheduledAction(action, framesUntilExecution - 1);
			actionsToScheduleNextFrame.Add(result);
			return result;
		}

		public bool CancelScheduledAction(ScheduledAction action)
		{
			if (scheduledActions.Contains(action))
			{
				scheduledActions.Remove(action);
				return true;
			}
			else
			{
				return actionsToScheduleNextFrame.Remove(action);
			}
		}

		/// <summary>
		/// Updates this action scheduler, decrementing the frames-left of all scheduled actions,
		/// and executing actions that have 0 frames left.
		/// </summary>
		public void Update()
		{
			foreach (var scheduledAction in scheduledActions)
			{
				if (scheduledAction.FramesUntilExecution == 1)
				{
					scheduledAction.Action();
				}
				scheduledAction.FramesUntilExecution--;
			}

			scheduledActions.RemoveAll(s => s.FramesUntilExecution == 0);
			scheduledActions.AddRange(actionsToScheduleNextFrame);
			actionsToScheduleNextFrame.Clear();
		}
	}
}
