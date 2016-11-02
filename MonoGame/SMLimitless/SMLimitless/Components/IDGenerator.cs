using System;

namespace SMLimitless.Components
{
	/// <summary>
	///   Assigns unique, contiguous <see cref="int" /> IDs.
	/// </summary>
	public sealed class IDGenerator
	{
		private int lastIDAssigned = -1;

		/// <summary>
		///   Gets a new ID to assign to an object.
		/// </summary>
		/// <returns>A new, unique integer ID.</returns>
		public int GetNewID()
		{
			if (lastIDAssigned < int.MaxValue) { return ++lastIDAssigned; }
			else { throw new InvalidOperationException("Exhausted the ID space (seriously?)."); }
		}
	}
}
