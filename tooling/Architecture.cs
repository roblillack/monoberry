using System;

namespace MonoBerry.Tool
{
	public class Architecture {
		public string Name { get; protected set; }
		protected Architecture (string name)
		{
			Name = name;
		}

		public static readonly Architecture ARM = new Architecture ("armle-v7");
		public static readonly Architecture X86 = new Architecture ("x86");

		public bool Matches (BlackBerry.ApplicationDescriptor.Architecture arch)
		{
			return (arch == BlackBerry.ApplicationDescriptor.Architecture.ALL ||
			        (this == ARM && arch == BlackBerry.ApplicationDescriptor.Architecture.ARM) ||
			        (this == X86 && arch == BlackBerry.ApplicationDescriptor.Architecture.X86));
		}

		public static Architecture FromName (string name)
		{
			if (name == null) {
				throw new ArgumentNullException ();
			}

			switch (name.Trim ().ToLower ()) {
			case "armle-v7":
				return ARM;
			case "x86":
				return X86;
			}
			throw new ArgumentException (String.Format ("Unknown architecture: '{0}'", name));
		}

		public override string ToString ()
		{
			return Name;
		}
	}
}

