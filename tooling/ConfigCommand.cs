using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Reflection;

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
		
		public override void Execute (MonoBerry app, IList<string> parameters)
		{
			if (parameters.Count >= 1) {
				Console.WriteLine (app.Configuration.Get (parameters [0]));
			}
		}
	}

}


