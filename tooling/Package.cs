using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Text;
using BlackBerry.ApplicationDescriptor;

namespace MonoBerry.Tool
{
	public class Package : Command
	{
		public override string Name {
			get { return "package"; }
		}
		
		public override string Description {
			get { return "Packages a Mono Assembly as BlackBerry Archive."; }
		}

		private static Assembly LoadAssembly (string name)
		{
			try {
				return Assembly.LoadFile (name + ".dll");
			} catch (FileNotFoundException) {
				return Assembly.Load (name);
			}
		}

		private static Assembly LoadAssembly (AssemblyName name)
		{
			try {
				return LoadAssembly (name.Name);
			} catch {
				return Assembly.Load (name);
			}
		}

		private static void LoadDependencies (Assembly assembly, ISet<Assembly> assemblies)
		{
			if (assemblies.Contains (assembly)) {
				return;
			}

			assemblies.Add (assembly);
			Console.WriteLine ("Adding {0}", assembly.FullName);
			foreach (var i in assembly.GetReferencedAssemblies ()) {
				/*var ignored = new HashSet<string> (new string[]{"Mono.Security", "System.Configuration"});
				if (ignored.Contains (i.Name)) {
					continue;
				}*/
				Console.WriteLine (" - {0}", i.Name);
				//assemblies.Add (LoadAssembly (i.Name));
				//LoadDependencies (LoadAssembly (i.Name), assemblies);
				LoadDependencies (LoadAssembly (i), assemblies);
			}
		}

		private static T GetAttribute<T> (Assembly assembly) where T : Attribute
		{
			foreach (var i in assembly.GetCustomAttributes (false)) {
				if (i.GetType ().Namespace == typeof (T).Namespace &&
				    i.GetType ().Name == typeof (T).Name) {
					if (!i.GetType ().Equals (typeof (T))) {
						throw new NotSupportedException ("Assembly references incompatible MonoBerry libappdesc.dll!");
					}
					// Somehow looks evil, right? :-)
					return (T)i;
				}
			}

			return (T)null;
		}

		private static string GetAttributeValue<T> (Assembly assembly) where T : Attribute
		{
			foreach (var i in assembly.GetCustomAttributes (false)) {
				if (i.GetType ().Namespace == typeof (T).Namespace &&
				    i.GetType ().Name == typeof (T).Name) {
					return i.ToString ();
				}
			}

			return null;
		}

		private static string UpPath (string path, int times = 1)
		{
			for (int i = 0; i < times; i++) {
				path = Path.GetDirectoryName (path);
			}
			return path;
		}

		public void CreateAppDescriptor (string assemblyFile, Architecture arch, bool devMode = true)
		{
			var assembly = Assembly.LoadFile (assemblyFile);
			Console.WriteLine ("Assembly: {0}", assembly.Location);
			Console.WriteLine ("Architecture: {0}", arch);
			Console.WriteLine ("Path: {0}", Directory.GetParent (assembly.Location));
			Directory.SetCurrentDirectory (Directory.GetParent (assembly.Location).FullName);

			ISet<Assembly> deps = new HashSet<Assembly> ();
			LoadDependencies (assembly, deps);

			foreach (var i in deps) {
				Console.WriteLine ("- Location: {0}", i.Location);
			}

			var id = GetAttributeValue<ApplicationIdentifierAttribute> (assembly);
			if (id == null || id.Length < 10) {
				Console.Error.WriteLine ("Application Identifier not specified or too short.");
				return;
			}

			var name = GetAttribute<AssemblyTitleAttribute> (assembly);
			if (name == null || name.Title.Length < 3) {
				Console.Error.WriteLine ("Assembly Title not specified or too short.");
				return;
			}

			var platform = GetAttributeValue<PlatformVersionAttribute> (assembly) ?? "2.0.0.0";

			var icon = GetAttribute<IconAttribute> (assembly);


			using (var xml = new XmlTextWriter ("app-descriptor.xml", Encoding.UTF8)) {
				xml.Formatting = Formatting.Indented;
				xml.WriteStartDocument ();
				xml.WriteStartElement ("qnx", "http://www.qnx.com/schemas/application/1.0");
				xml.WriteElementString ("id", id);
				xml.WriteElementString ("name", name.Title);
				xml.WriteElementString ("versionNumber", String.Format("{0}.{1}.{2}", assembly.GetName ().Version.Major, assembly.GetName ().Version.Minor, assembly.GetName ().Version.Build));
				xml.WriteElementString ("platformVersion", platform);

				// We work around a problem with the NDK here
				xml.WriteElementString ("buildId", Math.Abs ((short)assembly.GetName ().Version.Revision).ToString ());

				if (devMode) {
					xml.WriteElementString ("author", Application.Configuration.Get ("author_name"));
					xml.WriteElementString ("authorId", Application.Configuration.Get ("author_id"));
				}

				var permissions = GetAttribute<RequestedPermissionsAttribute> (assembly);
				if (permissions != null) {
					foreach (var i in permissions.Functions) {
						Console.WriteLine ("- Requested permission: {0}", i.ToString ());
						xml.WriteElementString ("permission", i.GetValue ());
					}
				}

				xml.WriteStartElement ("initialWindow");
				xml.WriteElementString ("systemChrome", "none");
				xml.WriteElementString ("transparent", "true");
				var aspectRatio = GetAttributeValue<AspectRatioAttribute> (assembly);
				if (aspectRatio != null) {
					xml.WriteElementString ("aspectRatio", aspectRatio);
				}
				xml.WriteEndElement ();

				xml.WriteStartElement ("env");
				xml.WriteAttributeString ("var", "MONO_PATH");
				xml.WriteAttributeString ("value", "app/native/lib");
				xml.WriteEndElement ();

				xml.WriteStartElement ("env");
				xml.WriteAttributeString ("var", "LD_LIBRARY_PATH");
				xml.WriteAttributeString ("value", "/lib:/usr/lib/:app/native/lib");
				xml.WriteEndElement ();

				if (devMode) {
					xml.WriteStartElement ("env");
					xml.WriteAttributeString ("var", "MONO_TRACE_LISTENER");
					xml.WriteAttributeString ("value", "Console.Out");
					xml.WriteEndElement ();

					//xml.WriteStartElement ("env");
					//xml.WriteAttributeString ("var", "MONO_LOG_LEVEL");
					//xml.WriteAttributeString ("value", "debug");
					//xml.WriteEndElement ();
				}

				string monopath = null;
				string monoplat = "4.0";
				foreach (var i in deps) {
					string path = null;

					if (Path.GetFileName (i.Location) == "mscorlib.dll") {
						monopath = UpPath (i.Location, 4);
						monoplat = i.ImageRuntimeVersion.StartsWith ("v2.") ? "2.0" : "4.0";
						path = Path.Combine (Application.Configuration.Location, "lib", "mono", monoplat, "mscorlib.dll");						                  
					}

					xml.WriteStartElement ("asset");
					xml.WriteAttributeString ("path", path ?? i.Location);
					xml.WriteString ("lib/" + Path.GetFileName (i.Location));
					xml.WriteEndElement ();

					var cfg = (path ?? i.Location) + ".config";
					if (File.Exists (cfg)) {
						Console.Out.WriteLine ("- Adding assembly config from {0}", cfg);
						xml.WriteStartElement ("asset");
						xml.WriteAttributeString ("path", cfg);
						xml.WriteString ("lib/" + Path.GetFileName (cfg));
						xml.WriteEndElement ();
					}
				}

				var machinecfg = Path.Combine (monopath, "etc", "mono", monoplat, "machine.config");
				if (monopath != null && File.Exists (machinecfg)) {
					Console.Out.WriteLine ("- Adding machine config from {0}", machinecfg);
					xml.WriteStartElement ("asset");
					xml.WriteAttributeString ("path", machinecfg);
					xml.WriteString ("lib/" + Path.GetFileName (assembly.Location) + ".config");
					xml.WriteEndElement ();
				}

				foreach (var i in assembly.GetCustomAttributes (typeof (BlackBerry.ApplicationDescriptor.NativeLibraryAttribute), false)) {
					var attr = (BlackBerry.ApplicationDescriptor.NativeLibraryAttribute)i;
					if (!arch.Matches (attr.Architecture)) {
						continue;
					}
					Console.Out.WriteLine ("- Adding native library {0}", attr.Path);
					xml.WriteStartElement ("asset");
					xml.WriteAttributeString ("path", attr.Path);
					xml.WriteString ("lib/" + Path.GetFileName (attr.Path));
					xml.WriteEndElement ();
				}

				foreach (var i in assembly.GetCustomAttributes (typeof (BlackBerry.ApplicationDescriptor.AssetAttribute), false)) {
					var attr = (BlackBerry.ApplicationDescriptor.AssetAttribute)i;
					Console.Out.WriteLine ("- Adding asset {0}", attr.Path);
					xml.WriteStartElement ("asset");
					xml.WriteAttributeString ("path", attr.Path);
					xml.WriteString (Path.GetFileName (attr.Path));
					xml.WriteEndElement ();
				}

				if (icon != null && icon.Path != null) {
					Console.WriteLine ("ICON: {0}", icon.Path);
					xml.WriteStartElement ("icon");
					xml.WriteElementString ("image", icon.Path);
					xml.WriteEndElement ();

					xml.WriteStartElement ("asset");
					xml.WriteAttributeString ("path", Path.Combine (Path.GetDirectoryName (assembly.Location), icon.Path));
					//xml.WriteAttributeString ("entry", "true");
					//xml.WriteAttributeString ("type", "Qnx/Elf");
					xml.WriteString (icon.Path);
					xml.WriteEndElement ();

				}

				xml.WriteStartElement ("asset");
				xml.WriteAttributeString ("path", Path.Combine (Application.Configuration.Location, "target", arch.Name, "bin", "mono"));
				xml.WriteAttributeString ("entry", "true");
				xml.WriteAttributeString ("type", "Qnx/Elf");
				xml.WriteString ("bin/mono");
				xml.WriteEndElement ();

				//xml.WriteElementString ("arg", "--trace=N:OpenTK,N:OpenTK.Graphics.ES10,OpenTK.Graphics.ES11,OpenTK.Graphics.ES20,N:OpenTK.Platform.Egl,N:OpenTK.Platform.BlackBerry,N:OpenTK.Platform,N:OpenTK.Graphics,program");
				xml.WriteElementString ("arg", "app/native/lib/" + Path.GetFileName (assembly.Location));

				xml.WriteEndElement ();
				xml.WriteEndDocument ();
				xml.Close ();
			}
		}
		
		public override void Execute (IList<string> parameters)
		{
			Architecture arch;
			try {
				arch = Architecture.FromName (parameters [1]);
			} catch {
				arch = Architecture.ARM;
			}
			CreateAppDescriptor (parameters [0], arch, false);
			var appName = Path.GetFileNameWithoutExtension (parameters [0]);
			var pw = Application.Configuration.CSKPassword;
			while (pw.IsEmpty ()) {
				pw = Extensions.ReadPassword ("CSK Password (will not echo): ");
			}
			var cmd = String.Format ("{0}/usr/bin/blackberry-nativepackager -package {1}.bar {2} " +
			                         "-target bar -sign -storepass \"{3}\"",
			                         Application.Configuration.QNXHostPath,
			                         appName,
			                         "app-descriptor.xml",
			                         pw);

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


