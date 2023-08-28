using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SimplyHideHats.Extensions
{
	internal static class IEnumerableExt
	{
		public static void ForEach<T>(this IEnumerable<T> self, Action<T> f)
		{
			foreach (var elem in self) {
				f(elem);
			}
		}

		public static IEnumerable<(TFirst First, TSecond Second)> Zip<TFirst, TSecond>(this IEnumerable<TFirst> self, IEnumerable<TSecond> other)
		{
			return self.Zip(other, (first, second) => (first, second));
		}
	}

	internal static class RectExt
	{
		public static Rect AddBottom(this Rect self, float value)
		{
			return new Rect(self.x, self.y, self.width, self.height + value);
		}

		public static Rect AddLeft(this Rect self, float value)
		{
			return new Rect(self.x - value, self.y, self.width + value, self.height);
		}

		public static Rect AddRight(this Rect self, float value)
		{
			return new Rect(self.x, self.y, self.width + value, self.height);
		}

		public static Rect AddTop(this Rect self, float value)
		{
			return new Rect(self.x, self.y - value, self.width, self.height + value);
		}

		public static Rect Contract(this Rect self, float value)
		{
			return new Rect(self.x + value, self.y + value, self.width - 2 * value, self.height - 2 * value);
		}

		public static Rect CutBottom(this ref Rect self, float value)
		{
			self.height -= value;
			return new Rect(self.x, self.yMax, self.width, value);
		}

		public static Rect CutLeft(this ref Rect self, float value)
		{
			self.x += value;
			self.width -= value;
			return new Rect(self.x - value, self.y, value, self.height);
		}

		public static Rect CutRight(this ref Rect self, float value)
		{
			self.width -= value;
			return new Rect(self.xMax, self.y, value, self.height);
		}

		public static Rect CutTop(this ref Rect self, float value)
		{
			self.y += value;
			self.height -= value;
			return new Rect(self.x, self.y - value, self.width, value);
		}

		public static Rect Extend(this Rect self, float value)
		{
			return new Rect(self.x - value, self.y - value, self.width + 2 * value, self.height + 2 * value);
		}

		public static Rect GetBottom(this Rect self, float value)
		{
			return new Rect(self.x, self.yMax - value, self.width, value);
		}

		public static Rect GetLeft(this Rect self, float value)
		{
			return new Rect(self.x, self.y, value, self.height);
		}

		public static Rect GetRight(this Rect self, float value)
		{
			return new Rect(self.xMax - value, self.y, value, self.height);
		}

		public static Rect GetTop(this Rect self, float value)
		{
			return new Rect(self.x, self.y, self.width, value);
		}

		public static bool RadioButton(this Rect self, bool chosen)
		{
			var color = GUI.color;

			GUI.color = Color.white;
			var texture = chosen ? Widgets.RadioButOnTex : Widgets.RadioButOffTex;
			GUI.DrawTexture(self, texture);

			bool result = Widgets.ButtonInvisible(self, true);
			if (result && !chosen) {
				SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
			}

			GUI.color = color;
			return result;
		}
	}
}
