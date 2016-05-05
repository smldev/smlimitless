using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMLimitless.Components
{
	public sealed class ActionScheduler
	{
		private class ScheduledAction
		{
			public Action Action { get; }
			public int FramesUntilExecution { get; set; }

			public ScheduledAction(Action action, int framesUntilExecution)
			{
				Action = action;
				FramesUntilExecution = framesUntilExecution;
			}
		}

		private List<ScheduledAction> scheduledActions = new List<ScheduledAction>();

		public void ScheduleAction(Action action, int framesUntilExecution)
		{
			scheduledActions.Add(new ScheduledAction(action, framesUntilExecution));
		}

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
		}
	}
}
