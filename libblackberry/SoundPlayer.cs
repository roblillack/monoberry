using System;
using System.Runtime.InteropServices;

namespace BlackBerry
{
	public class SoundPlayer
	{
		[DllImport ("bps")]
		static extern int soundplayer_prepare_sound (string name);

		[DllImport ("bps")]
		static extern int soundplayer_play_sound (string name);

		public enum SystemSound
		{
			InputKeypress,
			NotificationGeneral,
			NotificationSapphire,
			AlarmBattery,
			EventBrowserStart,
			EventCameraShutter,
			EventRecordingStart,
			EventRecordingStop,
			EventDeviceLock,
			EventDeviceUnlock,
			EventDeviceTether,
			EventDeviceUntether,
			EventVideoCall,
			EventVideoCallOutgoing,
			SystemMasterVolumeReference
		}

		private static string GetSoundName (SystemSound sound)
		{
			switch (sound) {
			case SystemSound.InputKeypress: return "input_keypress";
			case SystemSound.NotificationSapphire: return "notification_sapphire";
			case SystemSound.AlarmBattery: return "alarm_battery";
			case SystemSound.EventBrowserStart: return "event_browser_start";
			case SystemSound.EventCameraShutter: return "event_camera_shutter";
			case SystemSound.EventRecordingStart: return "event_recording_start";
			case SystemSound.EventRecordingStop: return "event_recording_stop";
			case SystemSound.EventDeviceLock: return "event_device_lock";
			case SystemSound.EventDeviceUnlock: return "event_device_unlock";
			case SystemSound.EventDeviceTether: return "event_device_tether";
			case SystemSound.EventDeviceUntether: return "event_device_untether";
			case SystemSound.EventVideoCall: return "event_video_call";
			case SystemSound.EventVideoCallOutgoing: return "event_video_call_outgoing";
			case SystemSound.SystemMasterVolumeReference: return "system_master_volume_reference";
			case SystemSound.NotificationGeneral:
			default:
				return "notification_general";
			}
		}

		public static void Prepare (SystemSound sound)
		{
			soundplayer_prepare_sound (GetSoundName (sound));
		}

		public static void Play (SystemSound sound)
		{
			soundplayer_play_sound (GetSoundName (sound));
		}
	}
}

