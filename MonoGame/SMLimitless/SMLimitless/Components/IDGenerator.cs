using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMLimitless.Components
{
	/// <summary>
	/// Assigns unique, contiguous <see cref="int"/> IDs. 
	/// </summary>
	public sealed class IDGenerator
	{
		private int lastIDAssigned = -1;
		
		public int GetNewID()
		{
			if (lastIDAssigned < int.MaxValue) { return ++lastIDAssigned; }
			else { throw new InvalidOperationException("Exhausted the ID space (seriously?)."); }
		}
	}
}
