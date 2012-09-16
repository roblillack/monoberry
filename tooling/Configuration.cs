using Nini.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MonoBerry.Tool
{
	public struct Device {
		public string Name;
		public string IP;
		public string Password;
		public string PIN;
		public Architecture Architecture;
	}

    public class Configuration
	{
		static readonly string DEFAULT_SECTION = "core";

		string configFile;
		IniConfigSource configSource;

		public string NativeSDKPath { get { return Get ("nativesdk"); } }
		public string Location { get { return Get ("location"); } }
		public string SSHPublicKey { get { return Get ("public_key"); } }
		public string SSHPrivateKey { get { return Get ("private_key"); } }
		public string DebugToken { get { return Get ("debug_token"); } }
		public string CSKPassword { get { return Get ("csk_password"); } }

		public string ConfigFile {
			get {
				return configFile ?? Path.Combine (HomeDir, (IsUNIX ? "." : "_") + "monoberryrc");
			}
		}

		public string DefaultConfigDir {
			get {
				return configFile ?? Path.Combine (HomeDir, (IsUNIX ? "." : "_") + "monoberry");
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

		public string NativeSDKHostDir {
			get {
				return Path.Combine (NativeSDKPath, "host", "macosx", "x86");
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

		private string FindLocation ()
		{
			var assemblyLoc = typeof (MonoBerry).Assembly.Location;
			if (assemblyLoc == null || assemblyLoc.Length == 0) {
				throw new Exception ("Unable to locate " + MonoBerry.NAME + " installation.");
			}

			var path = Path.GetDirectoryName (assemblyLoc);
			if (Path.GetFileName (path) == "Debug" ||
			    Path.GetFileName (path) == "Release") {
				return Path.GetFullPath (Path.Combine (path, "..", "..", "..", "target"));
			}

			return path;
		}

		private string ReadConfigSetting (string section, string key)
		{
			if (configSource == null || section == null || key == null) {
				return null;
			}

			var cfg = configSource.Configs [section];
			return cfg == null ? null : cfg.GetString (key);
		}

		public string Get (string section, string key)
		{
			string retval = ReadConfigSetting (section, key);

			if (retval == null && section == DEFAULT_SECTION) {
				switch (key) {
				case "location": return FindLocation ();
				case "nativesdk": return FindNativeSDK ();
				case "debug_token": return Path.Combine (DefaultConfigDir, "debugtoken.bar");
				case "private_key": return Path.Combine (DefaultConfigDir, "id_rsa");
				case "public_key": return Path.Combine (DefaultConfigDir, "id_rsa.pub");
				}
			}

			return retval;
		}

		public string Get (string absoluteKey)
		{
			string[] secKey = absoluteKey.Split (new char[] {'.'}, 2);
			return Get (secKey.Length < 2 ? DEFAULT_SECTION : secKey [0], secKey [secKey.Length < 2 ? 0 : 1]);
		}

		public IDictionary<string, Device> GetDevices ()
		{
			var devices = new Dictionary<string, Device> ();

			foreach (var section in configSource.Configs) {
				var sec = section as IConfig;
				if (sec.Name.StartsWith ("device.")) {
					var name = sec.Name.Substring ("device.".Length);
					Architecture arch;
					try {
						arch = Architecture.FromName (sec.GetString ("arch"));
					} catch {
						arch = Architecture.ARM;
					}
					devices.Add (name, new Device {
						Name = name,
						IP = sec.GetString ("ip"),
						Password = sec.GetString ("password"),
						PIN = sec.GetString ("pin"),
						Architecture = arch
					}); 
				}
			}

			return devices;
		}

		public Configuration () : this(null) {}

		public Configuration (string configFile)
		{
			this.configFile = configFile;
			configSource = null;

			try {
				configSource = new IniConfigSource (ConfigFile);
			} catch (Exception) {
				if (configFile != null) {
					Console.Error.WriteLine ("Error loading configuration file {0}", configFile);
				}
			}
		}
	}
}

