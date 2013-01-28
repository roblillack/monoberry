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

	public enum Action {
		AccessSharedData,
		RecordAudio,
		ReadGeoLocation,
		UseCamera,
		AccessInternet,
		PlayAudio,
		PostNotifications,
		SetAudioVolume,
		ReadDeviceIdentifyingInformation,
		AccessLedControl
	}

	public class RequestedPermissionsAttribute : Attribute
	{

		public ISet<Action> Actions { get; protected set; }

		public RequestedPermissionsAttribute (params Action[] actions)
		{
			Actions = new HashSet<Action> (actions);
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

