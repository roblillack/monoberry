using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MonoBerry.Tool
{
	public class MonoBerryApp
	{
		public Configuration Configuration { get; protected set; }
		readonly List<Command> commands = new List<Command> ();
		
		public ICollection<Command> Commands {
			get { return commands; }
		}

		public void RegisterCommands (Assembly assembly)
		{
			foreach (Type t in assembly.GetTypes ()) {
				if (!typeof(Command).IsAssignableFrom (t)) {
					continue;
				}
				
				var ctor = t.GetConstructor (Type.EmptyTypes);
				if (ctor == null) {
					continue;
				}
				
				var c = ctor.Invoke (null) as Command;
				if (c == null) {
					continue;
				}

				c.Application = this;
				commands.Add (c);
			}
		}
		
		public void Execute (string cmd, List<string> parameters)
		{
			foreach (var c in commands) {
				if (cmd.Equals (c.Name.ToLower ())) {
					c.Execute (parameters);
					return;
				}
			}
			
			Console.Error.WriteLine ("Unknown command: {0}", cmd);
		}

		public void Execute (Type cmd, List<string> parameters)
		{
			foreach (var c in commands) {
				if (c.GetType () == cmd) {
					c.Execute (parameters);
					return;
				}
			}
			
			Console.Error.WriteLine ("Unknown command: {0}", cmd);
		}

		public void Execute (Type cmd) {
			Execute (cmd, new List<string> ());
		}

		public string GetToolPath (string tool)
		{
			return Configuration.NDKToolsDir == null ? tool : Path.Combine (Configuration.NDKToolsDir, tool);
		}

		public T GetCommand<T> ()
		{
			foreach (object c in commands) {
				if (c is T) {
					return (T)c;
				}
			}

			throw new InvalidOperationException ();
		}

		public MonoBerryApp (string[] args)
		{
			Configuration = new Configuration ();
		}

		public static void Main (string[] args)
		{
			var app = new MonoBerryApp (args);
			var cmd = args.Length > 0 ? (args [0]).ToLower () : "help";
			var parameters = new List<string> (args);
			if (parameters.Count > 0) {
				parameters.RemoveAt (0);
			}
			
			app.RegisterCommands (Assembly.GetExecutingAssembly ());
			try {
				app.Execute (cmd, parameters);
			} catch (Command.CommandErrorException e) {
				Console.Error.WriteLine ("ERROR: {0}", e.Message);
				Environment.Exit (1);
			}
		}

		static T ReadAttrib<T> ()
		{
			foreach (var i in typeof (MonoBerryApp).Assembly.GetCustomAttributes (typeof (T), true)) {
				return (T)i;
			}

			return default (T);
		}

		public static string NAME {
			get {
				var v = ReadAttrib<AssemblyProductAttribute> ();
				return v == null ? typeof (MonoBerryApp).Name : v.Product;
			}
		}

		public static string COPYRIGHT {
			get {
				var v = ReadAttrib<AssemblyCopyrightAttribute> ();
				return v != null ? v.Copyright : null;
			}
		}

		public static string DESCRIPTION {
			get {
				var v = ReadAttrib<AssemblyDescriptionAttribute> ();
				return v != null ? v.Description : null;
			}
		}

		public static string VERSION {
			get {
				return typeof (MonoBerryApp).Assembly.GetName ().Version.ToString ();
			}
		}

		public static string COMMAND {
			get {
				return typeof (MonoBerryApp).Assembly.GetName ().Name;
			}
		}
	}
}

