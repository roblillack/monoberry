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
		public MonoBerryApp Application { set; get; }

		public class CommandErrorException : Exception
		{
			public CommandErrorException (string msg) : base (msg) {}
		}
	}
}

