using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MonoBerry.Tool
{
	public class Connect : Command
	{
		public override string Name {
			get { return "connect"; }
		}
		
		public override string Description {
			get { return "Starts the SSH daemon on the device."; }
		}
		
		public override void Execute (IList<string> parameters)
		{
			var device = GetDevice (parameters);
			var cmd = String.Format ("{0}/usr/bin/blackberry-connect {1} -password {2} " +
			                         "-sshPublicKey {3}",
			                         Application.Configuration.NativeSDKHostDir,
			                         device.IP,
			                         device.Password,
			                         Application.Configuration.SSHPublicKey);
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
		
		private Device GetDevice (IList<string> parameters)
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
