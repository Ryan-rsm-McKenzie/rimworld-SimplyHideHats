﻿#pragma warning disable IDE1006 // Naming Styles

using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace SimplyHideHats
{
	[HarmonyPatch(typeof(PawnRenderer))]
	[HarmonyPatch("DrawHeadHair")]
	[HarmonyPatch(new Type[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(PawnRenderFlags), typeof(bool) })]
	internal class LetterStack_ReceiveLetter
	{
		public static bool Prefix(PawnRenderer __instance, ref PawnRenderFlags flags)
		{
			if (flags.HasFlag(PawnRenderFlags.Headgear)) {
				bool hide = __instance.graphics
					.apparelGraphics
					.Any(x => ModMain.Mod.ShouldHide(x.sourceApparel.def));
				if (hide) {
					flags ^= PawnRenderFlags.Headgear;
				}
			}
			return true;
		}
	}
}
