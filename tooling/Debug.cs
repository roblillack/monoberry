using System;
using System.Collections.Generic;
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
			Package p = Application.GetCommand<Package> ();
			p.CreateAppDescriptor (parameters [0], true);

			var appName = Path.GetFileNameWithoutExtension (parameters [0]);
			var device = GetDevice (parameters);

			// TODO: blackberry-nativepackager -package assemblyname.bar app-descriptor.xml
			// -devMode -target bar-debug -installApp -launchApp -device XXX -password XXX
			// /Developer/SDKs/bbndk-10.0.4-beta/host/macosx/x86/usr/bin/blackberry-nativepackager
			var cmd = String.Format ("{0}/usr/bin/blackberry-nativepackager -package {1}.bar {2} " +
			                         "-devMode -target bar-debug -installApp -launchApp -device {3} -password {4}",
			                         Application.Configuration.NativeSDKHostDir,
			                         appName,
			                         "app-descriptor.xml",
			                         device.Ip,
			                         device.Password);
			Console.WriteLine (cmd);
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
			} else {
				throw new Error ("Multiple devices configured. Please specify using --device!");
			}
		}
	}

}
