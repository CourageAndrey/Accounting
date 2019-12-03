using System;
using System.Collections.Generic;
using System.Windows.Markup;

namespace Accounting.UI.WPF.ViewModels
{
	public class PositionType : MarkupExtension
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return typeof(KeyValuePair<Accounting.Core.BusinessLogic.Product, decimal>);
		}
	}
}
