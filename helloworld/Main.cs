using System;
using BlackBerry;
using BlackBerry.Screen;

namespace helloworld
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			PlatformServices.Initialize ();

			using (var ctx = new Context())
			using (var win = new Window (ctx, WindowType.SCREEN_APPLICATION_WINDOW)) {
				win.AddBuffers (10);
				var r = new Random();
				foreach (var b in win.Buffers) {
					b.Fill (r.Next ());
					win.Render (b);
					System.Threading.Thread.Sleep (200);
				}

				PlatformServices.Alert("OMFG", "I'm running MONO!!!!1");

				using (var c = new Camera (Camera.Unit.Front, Camera.Mode.RW)) {
					PlatformServices.Alert("OMFG", "I got camera!1!1");
					c.TakePhoto ();
				}

				//System.Threading.Thread.Sleep (5000);
			}

			PlatformServices.Shutdown ();
		}
	}
}
