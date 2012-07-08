using System;
using System.Collections.Generic;

namespace MonoBerry.Tool
{
	public abstract class Command
	{
		public abstract string Name { get; }
		public abstract string Description { get; }
		public abstract void Execute(MonoBerry app, IList<string> parameters);
		public virtual bool IsVisible { get { return true; } }
	}
}

