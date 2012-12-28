using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading;
using BlackBerry;
using BlackBerry.Screen;

namespace MonoBerry.Samples.Paint
{
	class MainClass
	{
		const UInt32 WHITE = 0xffffffff;
		const UInt32 RED = 0xffff0000;
		const UInt32 BLACK = 0xff000000;

		public static void Main (string[] args)
		{
			using (var nav = new Navigator ())
			using (var ctx = Context.GetInstance (ContextType.Application))
			using (var win = new Window (ctx)) {
				win.AddBuffers (2);
				var bufs = win.Buffers;
				var buffer = bufs [0];
				var brush = bufs [1];
				var queue = new BlockingCollection<Point> ();

				buffer.Fill (WHITE);
				brush.Fill (BLACK);
				win.Render (buffer);

				var paintThread = new Thread (() => {
					for (;;) {
						Point point;
						Rectangle dirty;
						int taken;
						for (taken = 0; taken < 10 && queue.TryTake (out point); taken++) {
							var dst_x = Math.Min (win.Width - 10, point.X);
							var dst_y = Math.Min (win.Height - 10, point.Y);
							try {
								buffer.Blit (brush, 0, 0, 10, 10, dst_x, dst_y);
							} catch (Exception e) {
								Console.WriteLine (e.Message);
							}
							if (taken == 0) {
								dirty = new Rectangle (dst_x, dst_y, 10, 10);
							} else {
								dirty.X = Math.Min (dirty.Left, dst_x);
								dirty.Width = Math.Max (dirty.Width, (dst_x + 10) - dirty.X);
								dirty.Y = Math.Min (dirty.Top, dst_y);
								dirty.Height = Math.Max (dirty.Height, (dst_y + 10) - dirty.Y);
							}
						}
						if (taken < 1) {
							continue;
						}
						//Console.WriteLine ("Blitted {0} times before rendering", taken);
						try {
							win.Render (buffer, dirty, Flushing.SCREEN_WAIT_IDLE);
						} catch (Exception e) {
							Console.WriteLine (e.Message);
						}
					}
				});
				paintThread.Start ();

				nav.OnSwipeDown = () => Dialog.Alert ("#MonoBerry", "Mem: " + GC.GetTotalMemory (false),
				                                      new Button ("White", () => {
					try {
						buffer.Fill (WHITE);
						win.Render (buffer);
					} catch (Exception e) {
						Console.WriteLine (e.Message);
					}
				}),
				                                      new Button ("Red", () => {
					try {
						buffer.Fill (RED);
						win.Render (buffer);
					} catch (Exception e) {
						Console.WriteLine (e.Message);
					}
				}));
				win.OnCreate = () => Console.WriteLine ("win created.");
				win.OnClose = () => Console.WriteLine ("win closed.");
				ctx.OnFingerTouch = (x,y) => {
					queue.Add (new Point (x, y));
				};
				ctx.OnFingerMove = ctx.OnFingerTouch;
				ctx.OnFingerRelease = ctx.OnFingerTouch;

				nav.OnExit = () => {
					Console.WriteLine ("I am asked to shutdown!?!");
					PlatformServices.Shutdown (0);
				};
				PlatformServices.Run ();
				Console.WriteLine ("Event handler stopped. WTH?");
				PlatformServices.Shutdown (1);
			}
		}
	}
}
