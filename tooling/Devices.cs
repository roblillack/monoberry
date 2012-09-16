using System;
using System.Collections.Generic;

namespace MonoBerry.Tool
{
	public class Devices : Command
	{
		public override string Name {
			get { return "devices"; }
		}
		
		public override string Description {
			get { return "Lists all configured BlackBerry devices."; }
		}
		
		public override void Execute (IList<string> parameters)
		{
			var format =  "{0,-20} {1,-10} {2}";
			Console.WriteLine (format, "NAME", "ARCH", "IP");
			foreach (var d in Application.Configuration.GetDevices ()) {
				Console.WriteLine (format, d.Value.Name, d.Value.Architecture, d.Value.IP);
			}
		}
	}

}
