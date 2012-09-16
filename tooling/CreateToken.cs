using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MonoBerry.Tool
{
	public class CreateToken : Command
	{
		public override string Name {
			get { return "create-token"; }
		}
		
		public override string Description {
			get { return "Creates a new debug token for all configured devices."; }
		}

		public override void Execute (IList<string> parameters)
		{
			var pins = new List<string> ();
			foreach (var i in Application.Configuration.GetDevices ().Values) {
				if (i.PIN.IsEmpty ()) {
					Console.Error.WriteLine ("No PIN configured for device '{0}'--ignoring.", i.Name);
				}
				pins.Add (String.Format ("-devicepin {0}", i.PIN));
			}
			var pw = Application.Configuration.CSKPassword;
			while (pw.IsEmpty ()) {
				pw = Extensions.ReadPassword ("CSK Password (will not echo): ");
			}
			Directory.GetParent (Application.Configuration.DebugToken).Create ();
			var cmd = String.Format ("{0}/usr/bin/blackberry-debugtokenrequest {1} -storepass \"{2}\" {3}",
			                         Application.Configuration.NativeSDKHostDir,
			                         pins.Join (" "),
			                         pw,
			                         Application.Configuration.DebugToken);
			Run (cmd);
		}
		
		private static void Run (string cmd)
		{
			try {
				using (Process proc = Process.Start ("/bin/sh", String.Format ("-c '{0}'", cmd))) {
					proc.WaitForExit();
				}
			} catch (Exception e) {
				throw new Error (String.Format ("Error running command {0}: {1}", cmd, e.Message));
			}
		}
	}
}
