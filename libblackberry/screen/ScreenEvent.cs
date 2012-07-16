using System;
using System.Runtime.InteropServices;

namespace BlackBerry.Screen
{
	public class ScreenEvent
	{
		[DllImport ("screen")]
		static extern IntPtr screen_event_get_event (IntPtr bps_event);

		[DllImport ("screen")]
		static extern IntPtr screen_get_event_property_iv (IntPtr handle, Property prop, out int val);

		[DllImport ("screen")]
		static extern IntPtr screen_get_event_property_pv (IntPtr handle, Property prop, out IntPtr val);

		IntPtr handle;

		public ScreenEvent (IntPtr e)
		{
			handle = e;
		}

		public static ScreenEvent FromEventHandle (IntPtr e)
		{
			return new ScreenEvent (screen_event_get_event (e));
		}

		public int GetIntProperty (Property property)
		{
			int val;
			screen_get_event_property_iv (handle, property, out val);
			return val;
		}

		public IntPtr GetIntPtrProperty (Property property)
		{
			IntPtr val;
			screen_get_event_property_pv (handle, property, out val);
			return val;
		}
		
		public EventType Type {
			get {
				return (EventType)GetIntProperty (Property.SCREEN_PROPERTY_TYPE);
			}
		}
	}
}

