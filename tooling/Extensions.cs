using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoBerry.Tool
{
	public static class Extensions
	{
		public static string Join (this IEnumerable me, string str = null)
		{
			StringBuilder b = new StringBuilder ();
			bool first = true;
			foreach (var i in me) {
				if (!first && str != null) {
					b.Append (str);
				}
				b.Append (i);
				first = false;
			}
			return b.ToString ();
		}
		
		public static T[] Subarray<T> (this T[] me, int offset, int length = -1)
		{
			if (offset >= me.Length) {
				return new T[0];
			}

			if (length == -1) {
				length = me.Length - offset;
			}
			T[] r = new T[length];
			Array.Copy (me, offset, r, 0, length);
			return r;
		}
		
		public static bool Contains (this char[] me, char c)
		{
			foreach (var i in me) {
				if (c == i) {
					return true;
				}
			}
			
			return false;
		}

		public static bool IsEmpty (this string me)
		{
			return me == null || "".Equals (me);
		}

		public static string ReadPassword (string text = null)
		{
			Console.Write (text);
			var stack = new Stack<char> ();
			for (;;) {
				var key = Console.ReadKey(true);
				switch (key.Key) {
				case ConsoleKey.Backspace:
					if (stack.Count > 0) {
						stack.Pop ();
					}
					continue;
				case ConsoleKey.Enter:
					Console.WriteLine ();
					return stack.Reverse ().Join ();
				}
				stack.Push (key.KeyChar);
			}
		}

		static readonly char[] NEEDS_ESCAPING = {'"', ';', ':', ',', '\\'};

		public static string Escape (this string me)
		{
			var b = new StringBuilder (me.Length);
			
			foreach (char c in me) {
				if (c == '\n') {
					b.Append (@"\n");
					continue;
				}
				
				if (NEEDS_ESCAPING.Contains (c)) {
					b.Append ('\\');
				}
				
				b.Append (c);
			}
			
			return b.ToString ();
		}
		
		public static string Unescape (this string me) {
			var b = new StringBuilder (me.Length);
			
			for (int i = 0; i < me.Length; i++) {
				if (i + 1 == me.Length) {
					b.Append (me [i]);
					continue;
				}
				
				if (me.Substring(i, 2).Equals (@"\n")) {
					b.Append ("\n");
					++i;
					continue;
				}
				
				if (me [i] == '\\' && NEEDS_ESCAPING.Contains (me [i + 1])) {
					b.Append (me [++i]);
					continue;
				}
				
				b.Append (me [i]);
			}
			
			return b.ToString ();
		}
		
		public static bool HasCommonSubsetWith<T> (this IEnumerable<T> me, IEnumerable<T> other)
		{
			foreach (var i in me) {
				if (other.Contains (i)) {
					return true;
				}
			}
			
			foreach (var i in other) {
				if (me.Contains (i)) {
					return true;
				}
			}
			
			return false;
		}
	}
}

