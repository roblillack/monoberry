using System;
using System.Threading;
using BlackBerry;
using BlackBerry.Screen;

namespace invocation
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (var nav = new Navigator ())
			using (var ctx = Context.GetInstance (ContextType.Application)) {
				nav.OnInvokeResult = (InvokeTargetReply e) => {
					var info = String.Format ("Id: {0}\nError: {1}", e.Id, e.Error);
					var i = e.Invocation;
					Dialog.Alert ("Got InvokeTargetReply",  info + "\n\n" + (i == null ? "?" : i.MimeType),
					              new Button ("Quit", () => PlatformServices.Stop ())); 
				};

				using (var req = new InvokeRequest ()) {
					req.Action = "bb.action.CAPTURE";
					req.Target = "sys.camera.card";
					//req.Data = new System.Text.ASCIIEncoding ().GetBytes ("photo");
					//req.Action = "";
					//req.MimeType = "image/jpeg";
					nav.Invoke (req);
				}

				PlatformServices.Run ();
				PlatformServices.Shutdown (0);
			}
		}
	}
}
