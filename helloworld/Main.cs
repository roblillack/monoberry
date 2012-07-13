using System;
using System.Threading;
using BlackBerry;
using BlackBerry.Screen;

namespace helloworld
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (var ctx = new Context ())
			using (var win = new Window (ctx, WindowType.SCREEN_APPLICATION_WINDOW)) {
				win.AddBuffers (10);
				var r = new Random();
				foreach (var b in win.Buffers) {
					b.Fill (r.Next ());
					win.Render (b);
					System.Threading.Thread.Sleep (200);
				}

				using (var dlg = new Dialog ("CLOSE ME!", "jpo1jo1j1oj1oj1")) {
					dlg.AddButton (new Button ("Timer", Timer));
					dlg.AddButton (new Button ("Camera", Cam));
					dlg.AddButton (new Button ("Close", PlatformServices.Stop));

					var run = true;
					while (run) {
						dlg.Show ();
						PlatformServices.Run ();
					}

				}

				//System.Threading.Thread.Sleep (5000);
			}
		}

		public static void Timer ()
		{
			using (var dlg = Dialog.Show ("OMFG–“UNICODE”", "I'm running MØNØ!!!!1")) {
				for (int i = 50; i > 0; i--) {
					dlg.Message = String.Format("Closing in {0:0.00} seconds.", i/10.0);
					Thread.Sleep (100);
				}
			}
		}

		public static void Cam ()
		{
			using (var c = new Camera (Camera.Unit.Front, Camera.Mode.RW)) {
				Dialog.Show ("OMFG", "I got camera!1!1");
				c.TakePhoto ();
			}
		}
	}
}
