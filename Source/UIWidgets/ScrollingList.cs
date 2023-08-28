using System;
using System.Linq;
using SimplyHideHats.Extensions;
using UnityEngine;
using Verse;

namespace SimplyHideHats.UIWidgets
{
	internal class ScrollingList
	{
		private readonly IDataProvider _dataProvider;

		private float _relativeScrollPosition = 0;

		private Vector2 _scrollPosition = Vector2.zero;

		public ScrollingList(IDataProvider dataProvider)
		{
			this._dataProvider = dataProvider;
		}

		public void Draw(Rect canvas)
		{
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.color = Color.white;

			Widgets.BeginGroup(canvas);

			int itemCount = this._dataProvider.Count;
			float itemHeight = this._dataProvider.ItemHeight;
			var visible = new Rect(canvas.x, 0, canvas.width, canvas.height);
			var full = new Rect(visible.x, visible.y, visible.width - GenUI.ScrollBarWidth, itemHeight * itemCount);

			float maxScrollable = full.height - visible.height;
			this._scrollPosition.y = Mathf.Lerp(0, maxScrollable, this._relativeScrollPosition);
			Widgets.BeginScrollView(visible, ref this._scrollPosition, full, true);
			this._relativeScrollPosition = Mathf.InverseLerp(0, maxScrollable, this._scrollPosition.y);

			int selected = full.Contains(Event.current.mousePosition) ?
				(int)(Event.current.mousePosition.y / itemHeight) : -1;

			var (index, count) = GetVisibleRange(this._scrollPosition.y, this._scrollPosition.y + visible.height, itemHeight);
			count = Math.Min(count, itemCount - index);
			if (count > 0) {
				var range = this._dataProvider.GetRange(index, count);
				full.CutTop(index * itemHeight);
				foreach (var (renderer, i) in range.Zip(Enumerable.Range(index, count))) {
					GUI.color = Color.white;
					var rect = full.CutTop(itemHeight);

					if (i % 2 == 0) {
						Widgets.DrawHighlight(rect);
					}

					renderer.Draw(rect, selected == i);
				}
			}

			Widgets.EndScrollView();
			Widgets.EndGroup();
		}

		private static (int Index, int Count) GetVisibleRange(float yMin, float yMax, float itemHeight)
		{
			int first = (int)(yMin / itemHeight);
			int last = (int)Math.Ceiling(yMax / itemHeight);
			return (first, last - first);
		}
	}
}
