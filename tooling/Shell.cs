using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace MonoBerry.Tool
{
	public class Shell : Command
	{
		Device dev;
		volatile bool connecting;
		readonly List<string> errors = new List<string> ();

		public override string Name {
			get { return "shell"; }
		}
		
		public override string Description {
			get { return "Open a shell on the specified device."; }
		}

		public override void Execute (IList<string> parameters)
		{
			dev = GetDevice (parameters);

			Console.Write ("Connecting: .");
			var connection = new Thread (SetupDaemon);
			connecting = true;
			connection.Start ();
			try {
				var chars = "\\|/-";
				var i = 0;
				Console.CursorVisible = false;
				while (connecting) {
					Thread.Sleep (100);
					Console.SetCursorPosition (Console.CursorLeft - 1, Console.CursorTop);
					Console.Write (chars [i++ % chars.Length]);
				}
			} finally {
				Console.WriteLine ();
				Console.CursorVisible = true;
			}
			Thread.Sleep (1000);
			if (errors.Count == 0) {
				try {
					Run (String.Format ("ssh -i {0} devuser@{1}", Application.Configuration.SSHPrivateKey, dev.IP));
				} finally {
					connection.Interrupt ();
				}
			} else {
				foreach (var i in errors) {
					Console.Error.WriteLine (i);
				}
			}
			connection.Join ();
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

		void SetupDaemon ()
		{
			var cmd = String.Format ("{0} {1} -password {2} " +
			                         "-sshPublicKey {3}",
									 Application.GetToolPath ("blackberry-connect"),
			                         dev.IP,
			                         dev.Password,
			                         Application.Configuration.SSHPublicKey);
			using (var proc = new Process ()) {
				try {
					var si = new ProcessStartInfo ("/bin/sh", String.Format ("-c '{0}'", cmd));
					si.RedirectStandardOutput = true;
					si.RedirectStandardError = true;
					si.UseShellExecute = false;
					proc.StartInfo = si;
					proc.Start ();
					proc.OutputDataReceived += (sender, args) => connecting &= !args.Data.Contains ("Successfully connected.");
					proc.BeginOutputReadLine ();
					proc.ErrorDataReceived += (sender, args) => errors.Add (args.Data);
					proc.BeginErrorReadLine ();
					proc.WaitForExit();
					connecting = false;
				} catch (ThreadInterruptedException) {
				} catch (Exception e) {
					throw new CommandErrorException (String.Format ("Error running command {0}: {1}", cmd, e.Message));
				} finally {
					if (!proc.HasExited) {
						proc.Kill ();
					}
				}
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

			if (parameters.Count == 1) {
				return devs [parameters [0]];
			}
			
			throw new CommandErrorException ("Please specify a device.");
		}
	}
	
}
