using System;

namespace MonoBerry.Tool
{
	public interface Command
	{
		string Name { get; }
		string Description { get; }
		void Execute(MonoBerry app, string[] parameters);
	}
}

