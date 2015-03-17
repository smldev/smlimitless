using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless.Physics
{
	public sealed class PhysicsSetting<T> where T : struct
	{
		public string Name { get; private set; }
		public T Value { get; set; }
		public T LowRange { get; private set; }
		public T HighRange { get; private set; }
	}
}
