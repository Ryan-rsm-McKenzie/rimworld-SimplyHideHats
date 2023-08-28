using UnityEngine;

namespace SimplyHideHats.UIWidgets
{
	internal interface IListItemRenderer
	{
		void Draw(Rect canvas, bool selected);
	}
}
