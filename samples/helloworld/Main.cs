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
			using (var nav = new Navigator ())
			using (var ctx = Context.GetInstance (ContextType.Application))
			using (var win = new Window (ctx, WindowType.SCREEN_APPLICATION_WINDOW)) {
				win.AddBuffers (10);
				win.Identifier = "bla";
				var r = new Random();
				foreach (var b in win.Buffers) {
					b.Fill ((uint)r.Next ());
					win.Render (b);
					System.Threading.Thread.Sleep (200);
				}
				//nav.AddUri ("", "Browser", "default", "http://google.com/");
				//nav.AddUri ("", "Messages", "default", "messages://");
				//return;

				var run = true;
				while (run) {
					Dialog.Alert ("CLOSE ME!", "jpo1jo1j1oj1oj1",
					              //new Button ("Timer", Timer),
					              //new Button ("Camera", Cam),
					              //new Button ("Messages", () => nav.Invoke ("messages://")),
					              new Button ("Badge", () => nav.HasBadge = true),
					              new Button ("Browser", () => nav.Invoke ("http://google.com/")),
					              new Button ("Close", () => run = false));
				}
			}
		}

		public static void Timer ()
		{
			using (var dlg = new Dialog ("OMFG–“UNICODE”", "I'm running MØNØ!!!!1")) {
				dlg.Show ();
				for (int i = 50; i > 0; i--) {
					dlg.Message = String.Format("Closing in {0:0.00} seconds.", i/10.0);
					Thread.Sleep (100);
				}
			}
		}

		public static void Cam ()
		{
			using (var c = new Camera (Camera.Unit.Front, Camera.Mode.RW)) {
				c.TakePhoto ();
			}
			Dialog.Alert ("OMFG", "I got camera!1!1", new Button ("Ok!"));
		}
	}
}
