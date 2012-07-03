using Nini.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MonoBerry.Tool
{
    public class Configuration
	{
		string configFile;

		public string NativeSDKPath { get; protected set; }

		public string ConfigFile {
			get {
				return configFile ?? Path.Combine (HomeDir, (IsUNIX ? "." : "_") + "monoberryrc");
			}
		}
				
		public bool IsUNIX {
			get {
				return Environment.OSVersion.Platform == PlatformID.Unix ||
					   Environment.OSVersion.Platform == PlatformID.MacOSX;
			}
		}
		
		public string HomeDir {
			get {
				return IsUNIX ?
					   Environment.GetEnvironmentVariable ("HOME") :
					   Environment.GetFolderPath (Environment.SpecialFolder.UserProfile);
			}
		}

		private string FindNativeSDK ()
		{
			foreach (var i in new string[] { "/Developer/SDKs/bbndk-10.0.4-beta" }) {
				if (Directory.Exists (i)) {
					return i;
				}
			}

			throw new Exception ("Unable to find BlackBerry Native SDK. Please speficy in " + ConfigFile);
		}

		public Configuration () : this(null) {}

		public Configuration (string configFile)
		{
			this.configFile = configFile;
			IniConfigSource configSource = null;

			try {
				configSource = new IniConfigSource (ConfigFile);
			} catch (Exception) {
				if (configFile != null) {
					Console.Error.WriteLine ("Error loading configuration file {0}", configFile);
				}
			}

			if (configSource != null) {
				try {
					var baseCfg = configSource.Configs ["monoberry"];
					
					foreach (var i in baseCfg.GetKeys ()) {
						if (i.Equals ("nativesdk")) {
							NativeSDKPath = baseCfg.Get (i);
						}
					}
				} catch (Exception e) {
					Console.Error.WriteLine ("Error reading configuration file: {0}", e.Message);
				}
			}

			NativeSDKPath = NativeSDKPath ?? FindNativeSDK ();
		}
	}
}

