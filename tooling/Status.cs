using System;

namespace MonoBerry.Tool
{
	public class Status : Command
	{
		public string Name {
			get { return "status"; }
		}
		
		public string Description {
			get { return "Display information about the build environment."; }
		}
		
		public void Execute (MonoBerry app, string[] parameters)
		{
			Console.WriteLine ("{0} version: {1}", MonoBerry.NAME, MonoBerry.VERSION);
			Console.WriteLine ("Native SDK Location: {0}", app.Configuration.NativeSDKPath);
			Console.WriteLine ("MonoBerry Installation Location: {0}", app.Configuration.Location);
		}
	}

}
