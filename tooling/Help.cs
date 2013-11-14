using System;
using System.Collections.Generic;

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
			Console.WriteLine ("{0} version {1}", MonoBerryApp.NAME, MonoBerryApp.VERSION);
			if (MonoBerryApp.DESCRIPTION != null) {
				Console.WriteLine (MonoBerryApp.DESCRIPTION);
			}
			if (MonoBerryApp.COPYRIGHT != null) {
				Console.WriteLine (MonoBerryApp.COPYRIGHT);
			}
			Console.WriteLine ();
			Console.WriteLine ("Usage: {0} <verb> [parameters]", MonoBerryApp.COMMAND);
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

