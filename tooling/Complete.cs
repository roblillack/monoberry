using System;
using System.Collections.Generic;

namespace MonoBerry.Tool
{
	public class Complete : Command
	{
		public override string Name {
			get { return "complete"; }
		}
		
		public override string Description {
			get { throw new NotSupportedException(); }
		}

		public override bool IsVisible {
			get { return false; }
		}
		
		public override void Execute (IList<string> parameters)
		{
			// Bash complete -C pretty much is bullshit.
			// We are not able to complete correctly without getting the WHOLE context …
			string current = parameters.Count > 2 ? parameters [1] : "";
			string previous = parameters [parameters.Count - 1];

			if (!current.StartsWith ("-") && !previous.StartsWith ("-")) {
				foreach (var i in Application.Commands) {
					if (i.IsVisible && i.Name.StartsWith (current)) {
						Console.WriteLine (i.Name);
					}
				}
			}
		}
	}

}

