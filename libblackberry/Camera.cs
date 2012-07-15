using System;
using System.IO;
using System.Runtime.InteropServices;

namespace BlackBerry
{
	public class Camera : IDisposable
	{
		[Flags]
		public enum Mode : uint {
			PREAD =      1<<0,
			PWRITE =     1<<1,   
			DREAD =      1<<2,   
			DWRITE =     1<<3,   
			ROLL =       1<<4,   
			PRIVILEGED = 1<<5,   

			RO =         (PREAD | DREAD),
			RW =         (PREAD | PWRITE | DREAD | DWRITE)
		}

		public enum Error : uint {
			NoError                    = 0,
			WasNotAvailableTryAgain    = 11,
			CallFailedInvalidParam     = 22,
			NoSuchCameraFound          = 19,
			FileTableOverflow          = 24,
			InvalidCameraHandle        = 9,
			PermissionDenied           = 13,
			InvalidFileDescriptor      = 51,
			NoSuchFileOrDirectory      = 2,
			MemoryAllocationFailed     = 12,
			OperationNotSupported      = 103,
			CameraTimeout              = 260,
			OperationAlreadyInProgress = 16,
			// Cam specific stuff
			CameraLibNotInitialized    = 0x1000,
			CallbackRegistrationFailed = 0x1001,
			MicrophoneAlreadyInUse     = 0x1002
		}

		public enum Unit : uint {
			None  = 0,
			Front,
			Rear
		}

		public enum ImageProperty : int {
			CAMERA_IMGPROP_END = -1,
			CAMERA_IMGPROP_FORMAT = 0,
			CAMERA_IMGPROP_WIDTH,
			CAMERA_IMGPROP_HEIGHT,
			CAMERA_IMGPROP_FRAMERATE,
			CAMERA_IMGPROP_BITRATE,
			CAMERA_IMGPROP_KEYFRAMEINTERVAL,
			CAMERA_IMGPROP_ROTATION,
			CAMERA_IMGPROP_STABILIZATION,
			CAMERA_IMGPROP_ZOOMFACTOR,
			CAMERA_IMGPROP_HWOVERLAY,
			CAMERA_IMGPROP_JPEGQFACTOR,
			CAMERA_IMGPROP_WIN_GROUPID,
			CAMERA_IMGPROP_WIN_ID,
			CAMERA_IMGPROP_BURSTMODE,
			CAMERA_IMGPROP_BURSTDIVISOR,
			CAMERA_IMGPROP_SLICESIZE
		}

		[DllImport ("camapi")]
		private static extern Error camera_close (IntPtr handle);

		[DllImport ("camapi")]
		private static extern Error camera_open (Unit unit, Mode mode, ref IntPtr handle);

		[DllImport ("camapi")]
		private static extern Error camera_start_photo_viewfinder (IntPtr handle,
		                                                           IntPtr viewfinderCallback, //void(*viewfinder_callback)(camera_handle_t, camera_buffer_t *, void *),
		                                                           IntPtr statusCallback, //void(*status_callback)(camera_handle_t, camera_devstatus_t, uint16_t, void *),
		                                                           IntPtr arg);
		[DllImport ("camapi")]
		private static extern Error camera_take_photo (IntPtr handle,
		                                               IntPtr shutterCallback,
		                                               IntPtr rawCallback,
		                                               IntPtr postViewCallback,
		                                               IntPtr imageCallback,
		                                               IntPtr arg,
		                                               bool wait);
		[DllImport ("camapi")]
		static extern Error _camera_set_photovf_property (IntPtr handle, ImageProperty property, ref string value, ImageProperty end);

		private IntPtr handle;
		public Camera (Unit unit, Mode mode)
		{
			HandleError (camera_open (unit, mode, ref handle));
		}

		public void TakePhoto ()
		{
			string bla = "bla";
			HandleError (_camera_set_photovf_property (handle,
			                                           ImageProperty.CAMERA_IMGPROP_WIN_ID,
			                                           ref bla,
			                                           ImageProperty.CAMERA_IMGPROP_END));
			HandleError (camera_start_photo_viewfinder (handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero));
			System.Threading.Thread.Sleep (3000);
			HandleError (camera_take_photo (handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, true));
		}

		public void Dispose ()
		{
			HandleError (camera_close (handle));
			handle = IntPtr.Zero;
		}

		public static void HandleError (Error e)
		{
			switch (e) {
			case Error.NoError: return;
			case Error.NoSuchFileOrDirectory: throw new FileNotFoundException ();
			case Error.PermissionDenied: throw new AccessViolationException ();
			default:
				throw new Exception ("Error: " + e.ToString ());
			}
		}
	}
}

