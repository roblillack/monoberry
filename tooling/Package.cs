using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Text;

namespace MonoBerry.Tool
{
	public class Package : Command
	{
		public override string Name {
			get { return "package"; }
		}
		
		public override string Description {
			get { return "Packages a Mono Assembly as BlackBery Archive."; }
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
			System.Console.WriteLine ("Adding {0}", assembly.FullName);
			foreach (var i in assembly.GetReferencedAssemblies ()) {
				var ignored = new HashSet<string> (new string[]{"Mono.Security", "System.Configuration"});
				if (ignored.Contains (i.Name)) {
					continue;
				}
				System.Console.WriteLine (" - {0}", i.Name);
				//assemblies.Add (LoadAssembly (i.Name));
				//LoadDependencies (LoadAssembly (i.Name), assemblies);
				LoadDependencies (LoadAssembly (i), assemblies);
			}
		}

		private static T GetAttribute<T> (Assembly assembly) where T : Attribute
		{
			foreach (var i in assembly.GetCustomAttributes (typeof (T), false)) {
				return (T)i;
			}

			return (T)null;
		}

		private static string GetApplicationIdentifier (Assembly assembly)
		{
			foreach (var i in assembly.GetCustomAttributes (false)) {
				if (i.GetType ().Namespace == "BlackBerry.ApplicationDescriptor" &&
				    i.GetType ().Name == "ApplicationIdentifierAttribute") {
					return i.ToString ();
				}
			}

			return null;
		}

		private static string GetPlatformVersion (Assembly assembly)
		{
			foreach (var i in assembly.GetCustomAttributes (false)) {
				if (i.GetType ().Namespace == "BlackBerry.ApplicationDescriptor" &&
				    i.GetType ().Name == "PlatformVersionAttribute") {
					return i.ToString ();
				}
			}

			return null;
		}

		public void CreateAppDescriptor (string assemblyFile, bool devMode = true)
		{
			var assembly = Assembly.LoadFile (assemblyFile);
			Console.WriteLine ("Assembly: {0}", assembly.Location);
			Console.WriteLine ("Path: {0}", Directory.GetParent (assembly.Location));
			Directory.SetCurrentDirectory (Directory.GetParent (assembly.Location).FullName);

			ISet<Assembly> deps = new HashSet<Assembly> ();
			LoadDependencies (assembly, deps);

			foreach (var i in deps) {
				Console.WriteLine ("- Location: {0}", i.Location);
			}

			var id = GetApplicationIdentifier (assembly);
			if (id == null || id.Length < 10) {
				Console.Error.WriteLine ("Application Identifier not specified or too short.");
				return;
			}

			var name = GetAttribute<AssemblyTitleAttribute> (assembly);
			if (name == null || name.Title.Length < 3) {
				Console.Error.WriteLine ("Assembly Title not specified or too short.");
				return;
			}

			var platform = GetPlatformVersion (assembly) ?? "2.0.0.0";

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
					xml.WriteElementString ("author", Application.Configuration.Get ("debugtoken", "author"));
					xml.WriteElementString ("authorId", Application.Configuration.Get ("debugtoken", "token"));
				}

				xml.WriteStartElement ("initialWindow");
				xml.WriteElementString ("systemChrome", "none");
				xml.WriteElementString ("transparent", "true");
				xml.WriteEndElement ();

				xml.WriteStartElement ("env");
				xml.WriteAttributeString ("var", "MONO_PATH");
				xml.WriteAttributeString ("value", "app/native/lib");
				xml.WriteEndElement ();

				foreach (var i in deps) {
					string path = null;

					if (Path.GetFileName (i.Location) == "mscorlib.dll") {
						path = Path.Combine (Application.Configuration.Location, "lib", "mono",
							                     i.ImageRuntimeVersion.StartsWith ("v2.") ? "2.0" : "4.0", "mscorlib.dll");						                  
					}

					xml.WriteStartElement ("asset");
					xml.WriteAttributeString ("path", path ?? i.Location);
					xml.WriteString ("lib/" + Path.GetFileName (i.Location));
					xml.WriteEndElement ();
				}

				xml.WriteStartElement ("asset");
				xml.WriteAttributeString ("path", Path.Combine (Application.Configuration.Location, "target", "armle-v7", "bin", "mono"));
				xml.WriteAttributeString ("entry", "true");
				xml.WriteAttributeString ("type", "Qnx/Elf");
				xml.WriteString ("bin/mono");
				xml.WriteEndElement ();

				xml.WriteElementString ("arg", "app/native/lib/" + Path.GetFileName (assembly.Location));

				xml.WriteEndElement ();
				xml.WriteEndDocument ();
				xml.Close ();
			}
		}
		
		public override void Execute (IList<string> parameters)
		{
			CreateAppDescriptor (parameters [0]);
		}
	}

}


