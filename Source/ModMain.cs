using HarmonyLib;
using UnityEngine;
using Verse;

namespace SimplyHideHats
{
	public class ModMain : Mod
	{
		public static ModMain Mod;

		private readonly Harmony _harmony;

		private Settings _settings = null;

		public ModMain(ModContentPack content)
			: base(content)
		{
			this._harmony = new Harmony(this.Content.PackageIdPlayerFacing);
			this._harmony.PatchAll();
			Mod = this;
		}

		public void DelayedInit()
		{
			this._settings = this.GetSettings<Settings>();
		}

		public override void DoSettingsWindowContents(Rect canvas)
		{
			this._settings.DoWindowContents(canvas);
		}

		public override string SettingsCategory()
		{
			return this.Content.Name;
		}

		public bool ShouldHide(ThingDef hat)
		{
			return this._settings.ShouldHide(hat);
		}
	}

	[StaticConstructorOnStartup]
	internal static class StartMeUp
	{
		static StartMeUp()
		{
			ModMain.Mod.DelayedInit();
		}
	}
}
