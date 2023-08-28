using System;
using RimWorld;
using SimplyHideHats.Extensions;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SimplyHideHats.UIWidgets
{
	internal class SearchBar
	{
		public QuickSearchFilter Filter = new QuickSearchFilter();

		public bool NoMatches = false;

		public Action OnChanged = null;

		private readonly string _controlName;

		public SearchBar()
		{
			this._controlName = $"QuickSearchWidget_{QuickSearchWidget.instanceCounter++}";
		}

		public void Draw(Rect canvas)
		{
			if (this.HasFocus() && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape) {
				this.KillFocus();
				Event.current.Use();
			}

			if (OriginalEventUtility.EventType == EventType.MouseDown && !canvas.Contains(Event.current.mousePosition)) {
				this.KillFocus();
			}

			GUI.color = Color.white;

			var icon = canvas
				.CutLeft(canvas.height)
				.Contract(0.10f * canvas.height);
			canvas.CutLeft(GenUI.GapTiny);
			GUI.DrawTexture(icon, TexButton.Search);

			var clear = canvas
				.CutRight(canvas.height)
				.Contract(0.10f * canvas.height);
			canvas.CutRight(GenUI.GapTiny);

			GUI.SetNextControlName(this._controlName);
			if (this.NoMatches && this.Filter.Active) {
				GUI.color = ColorLibrary.RedReadable;
			} else if (!this.Filter.Active && !this.HasFocus()) {
				GUI.color = ColorLibrary.Grey;
			}

			var textfield = canvas;
			this.SetText(Widgets.TextField(textfield, this.Filter.Text));

			if (this.Filter.Active) {
				if (Widgets.ButtonImage(clear, TexButton.CloseXSmall, true)) {
					this.SetText("");
					SoundDefOf.CancelMode.PlayOneShotOnCamera(null);
				}
			}
		}

		public bool HasFocus()
		{
			return GUI.GetNameOfFocusedControl() == this._controlName;
		}

		public void KillFocus()
		{
			if (this.HasFocus()) {
				UI.UnfocusCurrentControl();
			}
		}

		public void SetFocus()
		{
			GUI.FocusControl(this._controlName);
		}

		private void SetText(string text)
		{
			if (text != this.Filter.Text) {
				this.Filter.Text = text;
				this.OnChanged?.Invoke();
			}
		}
	}
}
