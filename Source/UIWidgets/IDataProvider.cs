using System.Collections.Generic;

namespace SimplyHideHats.UIWidgets
{
	internal interface IDataProvider
	{
		int Count { get; }

		float ItemHeight { get; }

		IEnumerable<IListItemRenderer> GetRange(int index, int count);
	}
}
