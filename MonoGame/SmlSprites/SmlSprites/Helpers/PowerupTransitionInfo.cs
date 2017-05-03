using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmlSprites.Helpers
{
	public sealed class PowerupTransitionInfo
	{
		public string NewPlayerTypeFullName { get; }
		public string TransitionGraphicsObjectName { get; }

		public PowerupTransitionInfo(string newPlayerTypeFullName, string transitionGraphicsObjectName)
		{
			NewPlayerTypeFullName = newPlayerTypeFullName;
			TransitionGraphicsObjectName = transitionGraphicsObjectName;
		}
	}
}
