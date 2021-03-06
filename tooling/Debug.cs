using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MonoBerry.Tool
{
	public class Debug : Command
	{
		public override string Name {
			get { return "debug"; }
		}
		
		public override string Description {
			get { return "Runs an assembly on a BlackBerry device in dev mode."; }
		}
		
		public override void Execute (IList<string> parameters)
		{
			var appName = Path.GetFileNameWithoutExtension (parameters [0]);
			var device = GetDevice (parameters);
			var file = parameters [0];

			if (file.EndsWith (".exe")) {
				Package p = Application.GetCommand<Package> ();
				p.CreateAppDescriptor (parameters [0], device.Architecture, true);
				file = "app-descriptor.xml";
			} else if (file.EndsWith (".xml")) {
				// nothing to do …
			} else {
				throw new ArgumentException (String.Format ("Unknown file format: {0}", file));
			}

			// TODO: blackberry-nativepackager -package assemblyname.bar app-descriptor.xml
			// -devMode -target bar-debug -installApp -launchApp -device XXX -password XXX
			// /Developer/SDKs/bbndk-10.0.4-beta/host/macosx/x86/usr/bin/blackberry-nativepackager
			var cmd = String.Format ("{0} -package {1}.bar {2} " +
			                         "-devMode -target bar-debug -installApp -launchApp -device {3} -password {4}",
									 Application.GetToolPath ("blackberry-nativepackager"),
			                         appName,
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
			} else if (devs.Count == 0) {
				throw new CommandErrorException ("No devices configured.");
			} else if (parameters.Count == 2) {
				return devs [parameters [1]];
			}

			throw new CommandErrorException ("Please specify a device.");
		}
	}

}
