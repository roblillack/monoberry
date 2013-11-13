using System;
using System.Collections.Generic;

namespace MonoBerry.Tool
{
	public class Status : Command
	{
		public override string Name {
			get { return "status"; }
		}
		
		public override string Description {
			get { return "Display information about the build environment."; }
		}
		
		public override void Execute (IList<string> parameters)
		{
			Console.WriteLine ("{0} version: {1}", MonoBerry.NAME, MonoBerry.VERSION);
			Console.WriteLine ("NDK Tools Dir: {0}", Application.Configuration.NDKToolsDir ?? "-");
			Console.WriteLine ("MonoBerry Installation Location: {0}", Application.Configuration.Location);
		}
	}

}
