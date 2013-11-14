using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonoBerry.Tool
{
	public class Deploy : Command
	{
		public override string Name {
			get { return "deploy"; }
		}
		
		public override string Description {
			get { return "Deploys a BlackBerry Archive (.bar file) to the specified device."; }
		}
		
		public override void Execute (IList<string> parameters)
		{
			var file = parameters [0];
			var device = GetDevice (parameters);
			
			if (!file.EndsWith (".bar")) {
				throw new CommandErrorException (String.Format ("Unknown file format: {0}", file));
			}
		
			var cmd = String.Format ("{0} {1} " +
			                         "-installApp -launchApp -device {2} -password {3}",
									 Application.GetToolPath ("blackberry-nativepackager"),
			                         file,
			                         device.IP,
			                         device.Password);
			Run (cmd);
		}
		
		static void Run (string cmd)
		{
			try {
				using (Process proc = Process.Start ("/bin/sh", String.Format ("-c '{0}'", cmd))) {
					proc.WaitForExit();
				}
			} catch (Exception e) {
				throw new CommandErrorException (String.Format ("Error running command {0}: {1}", cmd, e.Message));
			}
		}
		
		Device GetDevice (IList<string> parameters)
		{
			var devs = Application.Configuration.GetDevices ();
			
			if (devs.Count == 1) {
				var e = devs.Values.GetEnumerator ();
				e.MoveNext ();
				return e.Current;
			}

			if (devs.Count == 0) {
				throw new CommandErrorException ("No devices configured.");
			}

			if (parameters.Count == 2) {
				return devs [parameters [1]];
			}
			
			throw new CommandErrorException ("Please specify a device.");
		}
	}
	
}
