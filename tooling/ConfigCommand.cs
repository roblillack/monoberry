using System;
using System.Collections.Generic;

namespace MonoBerry.Tool
{
	public class ConfigCommand : Command
	{
		public override string Name {
			get { return "cfg"; }
		}
		
		public override string Description {
			get { return "Display MonoBerry settings."; }
		}
		
		public override void Execute (IList<string> parameters)
		{
			if (parameters.Count >= 1) {
				Console.WriteLine (Application.Configuration.Get (parameters [0]));
			}
		}
	}

}


