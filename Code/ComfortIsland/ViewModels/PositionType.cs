using System;
using System.Collections.Generic;
using System.Windows.Markup;

namespace ComfortIsland.ViewModels
{
	public class PositionType : MarkupExtension
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return typeof(KeyValuePair<BusinessLogic.Product, decimal>);
		}
	}
}
