using System;
using System.Collections.Generic;

namespace BlackBerry.ApplicationDescriptor
{
	public class ApplicationIdentifierAttribute : Attribute
	{
		public string Identifier { get; protected set; }

		public ApplicationIdentifierAttribute (string val)
		{
			Identifier = val;
		}

		public override string ToString ()
		{
			return Identifier;
		}
	}

	public enum RestrictedFunctionality {
		AccessSharedData,
		RecordAudio,
		ReadGeoLocation,
		UseCamera,
		AccessInternet,
		PostNotifications,
		ReadDeviceIdentifyingInformation
	}

	public static class RestrictedFunctionalityExt
	{
		public static string GetValue (this RestrictedFunctionality me)
		{
			switch (me) {
			case RestrictedFunctionality.AccessSharedData:
				return "access_shared";
			case RestrictedFunctionality.RecordAudio:
				return "record_audio";
			case RestrictedFunctionality.ReadGeoLocation:
				return "read_geolocation";
			case RestrictedFunctionality.UseCamera:
				return "use_camera";
			case RestrictedFunctionality.AccessInternet:
				return "access_internet";
			case RestrictedFunctionality.PostNotifications:
				return "post_notification";
			case RestrictedFunctionality.ReadDeviceIdentifyingInformation:
				return "read_device_identifying_information";
			default:
				throw new System.ArgumentException ();
			}
		}
	}

	public class RequestedPermissionsAttribute : Attribute
	{

		public ISet<RestrictedFunctionality> Functions { get; protected set; }

		public RequestedPermissionsAttribute (params RestrictedFunctionality[] actions)
		{
			Functions = new HashSet<RestrictedFunctionality> (actions);
		}
	}

	[AttributeUsage (AttributeTargets.Assembly, AllowMultiple = true)]
	public class AssetAttribute : Attribute
	{
		public string Path { get; protected set; }

		public AssetAttribute (string path)
		{
			Path = path;
		}
	}

	public class IconAttribute : Attribute
	{
		public string Path { get; protected set; }
		
		public IconAttribute (string path)
		{
			Path = path;
		}
	}

	public enum AspectRatio {
		LANDSCAPE,
		PORTRAIT
	}

	public class AspectRatioAttribute : Attribute
	{
		public AspectRatio AspectRatio { get; protected set; }
		
		public AspectRatioAttribute (AspectRatio ar)
		{
			AspectRatio = ar;
		}

		public override string ToString ()
		{
			return AspectRatio.ToString ().ToLower ();
		}
	}

	public enum Architecture {
		ALL,
		ARM,
		X86
	}

	[AttributeUsage (AttributeTargets.Assembly, AllowMultiple = true)]
	public class NativeLibraryAttribute : Attribute
	{
		public string Path { get; protected set; }
		public Architecture Architecture;
		
		public NativeLibraryAttribute (Architecture arch, string path)
		{
			Architecture = arch;
			Path = path;
		}

		public NativeLibraryAttribute (string path)
		{
			Architecture = BlackBerry.ApplicationDescriptor.Architecture.ALL;
			Path = path;
		}
	}

	public class PlatformVersionAttribute : Attribute
	{
		public string Version { get; protected set; }

		public PlatformVersionAttribute (string val)
		{
			Version = val;
		}

		public override string ToString ()
		{
			return Version;
		}
	}
}

