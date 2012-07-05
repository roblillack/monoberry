using System;

namespace helloworld
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			BlackBerry.PlatformServices.Initialize ();

			BlackBerry.PlatformServices.Alert("OMFG", "I'm running MONO!!!!1");

			System.Threading.Thread.Sleep (5000);

			BlackBerry.PlatformServices.Shutdown ();
		}
	}
}
