using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Text;
using Mono.Cecil;
using BlackBerry.ApplicationDescriptor;

namespace MonoBerry.Tool
{
	public enum TargetFramework
	{
		UNKNOWN,
		NET_2_0,
		NET_3_0,
		NET_3_5,
		NET_4_0,
		NET_4_5
	}

	public class Package : Command
	{
		public override string Name {
			get { return "package"; }
		}
		
		public override string Description {
			get { return "Packages a Mono Assembly as BlackBerry Archive."; }
		}

		static TargetFramework GetTargetFramework (Assembly assembly)
		{
			var attr = GetAttribute<System.Runtime.Versioning.TargetFrameworkAttribute> (assembly);
			switch (attr != null ? attr.FrameworkName : null) {
			case ".NETFramework,Version=v4.5":
				return TargetFramework.NET_4_5;
			case ".NETFramework,Version=v4.0":
				return TargetFramework.NET_4_0;
			}

			if (assembly.ImageRuntimeVersion.StartsWith ("v2.")) {
				return TargetFramework.NET_2_0;
			} else if (assembly.ImageRuntimeVersion.StartsWith ("v4.")) {
				return TargetFramework.NET_4_0;
			}

			return TargetFramework.UNKNOWN;
		}

		string ResolveDependency (AssemblyNameReference assemblyNameReference)
		{
			Assembly assembly;

			try {
				assembly =  Assembly.LoadFrom (assemblyNameReference.Name + ".dll");
			} catch (FileNotFoundException) {
				try {
					assembly = Assembly.Load (assemblyNameReference.FullName);
				} catch {
					assembly = null;
				}
			}

			if (assembly == null) {
				return null;
			}

			return assembly.CodeBase.StartsWith ("file://") ? assembly.CodeBase.Substring (7) : assembly.CodeBase;
		}

		ISet<string> ResolveDependencies (string assemblyFile, ISet<string> assemblies = null)
		{
			if (assemblies == null) {
				assemblies = new HashSet<string> ();
			} else if (assemblyFile == null || assemblies.Contains (assemblyFile)) {
				return assemblies;
			}

			assemblies.Add (assemblyFile);

			var assDef = AssemblyDefinition.ReadAssembly (assemblyFile);
			foreach (var i in assDef.MainModule.AssemblyReferences) {
				ResolveDependencies (ResolveDependency (i), assemblies);
			}


			return assemblies;
		}

		static T GetAttribute<T> (Assembly assembly) where T : Attribute
		{
			foreach (var i in GetAttributes<T> (assembly)) {
				return i;
			}

			return (T)null;
		}

		static IEnumerable<T> GetAttributes<T> (Assembly assembly) where T : Attribute
		{
			foreach (var i in assembly.GetCustomAttributes (false)) {
				if (i.GetType ().Namespace == typeof (T).Namespace &&
				    i.GetType ().Name == typeof (T).Name) {
					if (!i.GetType ().Equals (typeof (T))) {
						throw new NotSupportedException ("Assembly references incompatible MonoBerry libappdesc.dll!");
					}
					yield return (T)i;
				}
			}
		}

		static string GetAttributeValue<T> (Assembly assembly) where T : Attribute
		{
			foreach (var i in assembly.GetCustomAttributes (false)) {
				if (i.GetType ().Namespace == typeof (T).Namespace &&
				    i.GetType ().Name == typeof (T).Name) {
					return i.ToString ();
				}
			}

			return null;
		}

		static string UpPath (string path, int times = 1)
		{
			for (int i = 0; i < times; i++) {
				path = Path.GetDirectoryName (path);
			}
			return path;
		}

		public void CreateAppDescriptor (string assemblyFile, Architecture arch, bool devMode = true)
		{
			var assembly = Assembly.LoadFile (assemblyFile);
			var ver = GetTargetFramework (assembly);

			Console.WriteLine ("Assembly: {0}", assembly.Location);
			Console.WriteLine ("Assembly Runtime Version: {0}", assembly.ImageRuntimeVersion);
			Console.WriteLine ("Assembly Target Framework: {0}", ver);
			Console.WriteLine ("MonoBerry Target Architecture: {0}", arch);
			Console.WriteLine ("Path: {0}", Directory.GetParent (assembly.Location));

			if (ver != TargetFramework.NET_4_5 && ver != TargetFramework.NET_4_0) {
				throw new ArgumentException (String.Format ("Target Framework {0} not compatible with MonoBerry! Please compile for v4.5",
					ver));
			}

			Directory.SetCurrentDirectory (Directory.GetParent (assembly.Location).FullName);

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

				foreach (var i in GetAttributes<EnvironmentVariableAttribute> (assembly)) {
					Console.WriteLine ("- Environment Variable: {0}={1}", i.Name, i.Value);
					xml.WriteStartElement ("env");
					xml.WriteAttributeString ("var", i.Name);
					xml.WriteAttributeString ("value", i.Value);
					xml.WriteEndElement ();
				}

				xml.WriteStartElement ("env");
				xml.WriteAttributeString ("var", "MONO_PATH");
				xml.WriteAttributeString ("value", "app/native/lib");
				xml.WriteEndElement ();

				xml.WriteStartElement ("env");
				xml.WriteAttributeString ("var", "LD_LIBRARY_PATH");
				xml.WriteAttributeString ("value", "app/native/lib:/usr/lib/qt4/lib:/lib:/usr/lib");
				xml.WriteEndElement ();

				string monopath = null;
				foreach (var path in ResolveDependencies (assemblyFile)) {

					var p = path;
					if (Path.GetFileName (path) == "mscorlib.dll") {
						monopath = UpPath (path, 4);
						p = Path.Combine (Application.Configuration.Location, "lib", "mscorlib.dll");						                  
					}

					Console.WriteLine ("- {0}", p);
					xml.WriteStartElement ("asset");
					xml.WriteAttributeString ("path", p);
					xml.WriteString ("lib/" + Path.GetFileName (p));
					xml.WriteEndElement ();

					var cfg = path + ".config";
					if (File.Exists (cfg)) {
						Console.Out.WriteLine ("- Adding assembly config from {0}", cfg);
						xml.WriteStartElement ("asset");
						xml.WriteAttributeString ("path", cfg);
						xml.WriteString ("lib/" + Path.GetFileName (cfg));
						xml.WriteEndElement ();
					}
				}

				var machinecfg = Path.Combine (monopath, "etc", "mono", "4.0", "machine.config");
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


