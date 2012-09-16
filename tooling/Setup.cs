using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MonoBerry.Tool
{
	public class Setup : Command
	{
		public override string Name {
			get { return "setup"; }
		}
		
		public override string Description {
			get { return "Install debug token on the specified device."; }
		}

		public override void Execute (IList<string> parameters)
		{
			if (!File.Exists (Application.Configuration.DebugToken)) {
				Console.WriteLine ("Debug token does not exist. Creating ...");
				Application.Execute (typeof (CreateToken));
			}


			var dev = GetDevice (parameters);
			var cmd = String.Format ("{0}/usr/bin/blackberry-nativepackager -installDebugToken {1} -device {2} -password \"{3}\"",
			                         Application.Configuration.NativeSDKHostDir,
			                         Application.Configuration.DebugToken,
			                         dev.IP,
			                         dev.Password);
			Run (cmd);
		}
		
		static void Run (string cmd)
		{
			try {
				using (Process proc = Process.Start ("/bin/sh", String.Format ("-c '{0}'", cmd))) {
					proc.WaitForExit();
				}
			} catch (Exception e) {
				throw new Error (String.Format ("Error running command {0}: {1}", cmd, e.Message));
			}
		}

		Device GetDevice (IList<string> parameters)
		{
			var devs = Application.Configuration.GetDevices ();
			
			if (devs.Count == 1) {
				var e = devs.Values.GetEnumerator ();
				e.MoveNext ();
				return e.Current;
			} else if (devs.Count == 0) {
				throw new Error ("No devices configured.");
			} else if (parameters.Count == 1) {
				return devs [parameters [0]];
			}
			
			throw new Error ("Please specify a device.");
		}
	}
}
