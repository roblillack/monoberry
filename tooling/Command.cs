using System;
using System.Collections.Generic;

namespace MonoBerry.Tool
{
	public abstract class Command
	{
		public abstract string Name { get; }
		public abstract string Description { get; }
		public abstract void Execute(IList<string> parameters);
		public virtual bool IsVisible { get { return true; } }
		public MonoBerry Application { set; get; }

		public class Error : Exception
		{
			public Error (string msg) : base (msg) {}
		}
	}
}

