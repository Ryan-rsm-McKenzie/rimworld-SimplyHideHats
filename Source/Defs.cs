using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace SimplyHideHats.Defs
{
	[StaticConstructorOnStartup]
	internal static class ThingDefOf
	{
		public static readonly List<ThingDef> Headgear;

		static ThingDefOf()
		{
			Headgear = DefDatabase<ThingDef>.AllDefs
				.Where((def) => def.apparel != null && PawnApparelGenerator.IsHeadgear(def))
				.ToList();
		}
	}
}
