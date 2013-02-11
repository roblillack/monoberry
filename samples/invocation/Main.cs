using System;
using System.Text;
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
					var info = new StringBuilder ();
					info.AppendLine (String.Format ("Id: {0}", e.Id));
					info.AppendLine (String.Format ("Code: {0}", e.Code));
					info.AppendLine (String.Format ("Error Message: {0}", e.ErrorMessage));
					info.AppendLine (String.Format ("Error: {0}", e.Error));
					var i = e.Invocation;
					Dialog.Alert ("Got InvokeTargetReply",  info + "\n\n" + (i == null ? "?" : i.MimeType),
					              new Button ("Quit", () => PlatformServices.Stop ())); 
				};

				using (var req = new InvokeRequest ()) {
					req.Source = "com.burningsoda.monoberry.samples.invocation";
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
