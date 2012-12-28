using System;
using System.Threading;
using BlackBerry;
using BlackBerry.Screen;

namespace MonoBerry.Samples.Paint
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (var nav = new Navigator ())
			using (var ctx = Context.GetInstance (ContextType.Application))
			using (var win = new Window (ctx)) {
				win.Usage = Usage.SCREEN_USAGE_NATIVE;
				win.AddBuffers (2);
				var bufs = win.Buffers;
				var pic = bufs[0];
				var brush = bufs[1];

				pic.Fill (0xffff0000);
				brush.Fill (0xff000000);
				win.Render (pic);

				nav.OnSwipeDown = () => Dialog.Alert ("#MonoBerry", "Another Event Loop", new Button ("Ack"));
				ctx.OnFingerTouch = (x,y) => {
					try {
						pic.Blit (brush, 0, 0, 10, 10, Math.Max (x - 5, 0), Math.Max (y - 5, 0));
						win.Render (pic);
					} catch (Exception e) {
						Console.WriteLine (e.Message);
					}
				};
				ctx.OnFingerMove = ctx.OnFingerTouch;
				ctx.OnFingerRelease = ctx.OnFingerTouch;

				PlatformServices.Run ();
			}
		}
	}
}
