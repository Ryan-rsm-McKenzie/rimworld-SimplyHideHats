using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SimplyHideHats.Extensions;
using SimplyHideHats.UIWidgets;
using UnityEngine;
using Verse;

namespace SimplyHideHats
{
	public class Settings : ModSettings
	{
		/// <summary>
		/// <list type="bullet">
		/// <item>If <c>_defaultAction</c> is set to <c>ShowMode.Shown</c>, then this is a collection of things we should hide</item>
		/// <item>If <c>_defaultAction</c> is set to <c>ShowMode.Hidden</c>, then this is a collection of things we should show</item>
		/// </list>
		/// </summary>
		private readonly HashSet<ThingDef> _things = new HashSet<ThingDef>();

		private ShowMode _defaultAction = ShowMode.Shown;

		private Window _window = null;

		private enum ShowMode
		{
			Shown,

			Hidden,
		}

		public void DoWindowContents(Rect canvas)
		{
			if (this._window == null) {
				// settings menu opened
				this._window = new Window(this._defaultAction, this._things);
			}
			this._window.Draw(canvas);
		}

		public override void ExposeData()
		{
			if (Scribe.mode == LoadSaveMode.Saving) {
				// settings menu closed
				this._defaultAction = this._window.DefaultAction;
				this._things.Clear();
				var range = this._window.Config
					.Where(x => x.Shown != (this._defaultAction == ShowMode.Shown))
					.Select(x => x.Thing);
				this._things.AddRange(range);
				this._window = null;
			}

			Scribe_Values.Look(ref this._defaultAction, "default", ShowMode.Shown);

			var things = this._things.ToList();
			Scribe_Collections.Look(ref things, "things", LookMode.Def);
			this._things.Clear();
			if (things != null) {
				this._things.AddRange(things);
			}
		}

		public bool ShouldHide(ThingDef thing)
		{
			if (thing.apparel != null && PawnApparelGenerator.IsHeadgear(thing)) {
				return this._things.Contains(thing) ?
					this._defaultAction == ShowMode.Shown :
					this._defaultAction == ShowMode.Hidden;
			} else {
				return false;
			}
		}

		private class Window
		{
			private readonly ScrollingList _list;

			private readonly DataProvider _provider = new DataProvider();

			private readonly List<ListItemRenderer> _renderers;

			private readonly SearchBar _searcher = new SearchBar();

			private ShowMode _defaultAction;

			public Window(ShowMode defaultAction, ICollection<ThingDef> actionableThings)
			{
				this._defaultAction = defaultAction;
				this._renderers = Defs.ThingDefOf
					.Headgear
					.Select(x => new ListItemRenderer(x, actionableThings.Contains(x) ? defaultAction == ShowMode.Hidden : defaultAction == ShowMode.Shown))
					.OrderBy(x => x.LabelStripped)
					.ToList();
				this._provider.Data = this._renderers;
				this._list = new ScrollingList(this._provider);
				this._searcher.OnChanged = () => {
					this._provider.Data = this._renderers
						.Where(x => this._searcher.Filter.Matches(x.Thing.LabelCap))
						.OrderBy(x => x.LabelStripped);
					this._searcher.NoMatches = this._provider.Count == 0;
				};
			}

			public IEnumerable<(ThingDef Thing, bool Shown)> Config => this._renderers.Select(x => (x.Thing, x.Shown));

			public ShowMode DefaultAction => this._defaultAction;

			public void Draw(Rect canvas)
			{
				Text.Font = GameFont.Small;
				Text.Anchor = TextAnchor.UpperLeft;
				GUI.color = Color.white;

				this.DrawTopbar(ref canvas);
				canvas.CutTop(GenUI.GapTiny);
				this._list.Draw(canvas);

				Text.Anchor = TextAnchor.UpperLeft;
			}

			private void DrawTopbar(ref Rect canvas)
			{
				var top = canvas.CutTop(Text.LineHeight);
				Action<string, ShowMode> radio = (label, mode) => {
					var size = Text.CalcSize(label);
					var rect = top.CutLeft(size.x + GenUI.GapTiny + top.height);
					Widgets.Label(rect.CutLeft(size.x), label);
					if (rect.CutRight(top.height).RadioButton(this.DefaultAction == mode)) {
						this._defaultAction = mode;
						this._renderers
							.ForEach(x => x.Shown = this._defaultAction == ShowMode.Shown);
					}
				};

				Text.Font = GameFont.Small;
				Text.Anchor = TextAnchor.UpperLeft;
				GUI.color = Color.white;

				this._searcher.Draw(top.CutLeft(top.width / 2));
				top.CutLeft(GenUI.GapSmall);
				radio("SimplyHideHats.ShowAll".Translate(), ShowMode.Shown);
				top.CutLeft(GenUI.GapSmall);
				radio("SimplyHideHats.HideAll".Translate(), ShowMode.Hidden);
				top.CutLeft(GenUI.GapTiny);

				Text.Anchor = TextAnchor.LowerRight;
				Text.Font = GameFont.Tiny;
				GUI.color = Color.gray;

				Widgets.Label(top, "SimplyHideHats.ShowHide".Translate());
			}

			private class DataProvider : IDataProvider
			{
				private readonly List<ListItemRenderer> _list = new List<ListItemRenderer>();

				public int Count => this._list.Count;

				public IEnumerable<ListItemRenderer> Data {
					get => this._list;
					set {
						this._list.Clear();
						this._list.AddRange(value);
					}
				}

				public float ItemHeight => Text.LineHeight + GenUI.GapTiny;

				public IEnumerable<IListItemRenderer> GetRange(int index, int count)
				{
					return this._list.GetRange(index, count);
				}
			}

			private class ListItemRenderer : IListItemRenderer
			{
				public readonly string LabelStripped;

				public readonly ThingDef Thing;

				public bool Shown;

				public ListItemRenderer(ThingDef thing, bool shown)
				{
					this.LabelStripped = thing.LabelCap.ToString().StripTags();
					this.Thing = thing;
					this.Shown = shown;
				}

				public void Draw(Rect canvas, bool selected)
				{
					canvas = canvas.Contract(GenUI.GapTiny / 2);
					var canvasCopy = new Rect(canvas);

					var checkbox = canvas.CutRight(canvas.height);
					Widgets.Checkbox(checkbox.position, ref this.Shown, checkbox.width, paintable: true);
					if (selected) {
						TooltipHandler.TipRegion(checkbox, () => this.Shown ? "SimplyHideHats.Shown".Translate() : "SimplyHideHats.Hidden".Translate(), 0);
					}

					canvas.CutRight(GenUI.GapTiny);

					Widgets.ThingIcon(canvas.CutLeft(canvas.height), this.Thing);
					canvas.CutLeft(GenUI.GapTiny);

					var label = canvas;
					string full = this.Thing.LabelCap;
					string shortened = full.Truncate(label.width);
					Widgets.Label(label, shortened);

					if (selected) {
						Widgets.DrawHighlightIfMouseover(canvasCopy);
						TooltipHandler.TipRegion(label, full);
					}
				}
			}
		}
	}
}
