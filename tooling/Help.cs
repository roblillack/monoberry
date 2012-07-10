using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Reflection;

namespace MonoBerry.Tool
{
	public class Help : Command
	{
		public override string Name {
			get { return "help"; }
		}
		
		public override string Description {
			get { return "Displays this help screen"; }
		}
		
		public override void Execute (IList<string> parameters)
		{
			Console.WriteLine ("{0} version {1}", MonoBerry.NAME, MonoBerry.VERSION);
			if (MonoBerry.DESCRIPTION != null) {
				Console.WriteLine (MonoBerry.DESCRIPTION);
			}
			if (MonoBerry.COPYRIGHT != null) {
				Console.WriteLine (MonoBerry.COPYRIGHT);
			}
			Console.WriteLine ();
			Console.WriteLine ("Usage: {0} <verb> [parameters]", MonoBerry.COMMAND);
			Console.WriteLine ();
			Console.WriteLine ("Available verbs:");
			foreach (Command i in Application.Commands) {
				if (!i.IsVisible) {
					continue;
				}
				Console.WriteLine ("    {0,-15} {1}", i.Name, i.Description);
			}
		}
	}

}

