using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Accounting.Core.BusinessLogic
{
	public class ProductBalance
	{
		#region Свойства

		public Product Product
		{ get; }

		public decimal Count
		{ get; }

		public decimal CanBeProduced
		{ get; }

		public IReadOnlyCollection<ChildProductBalance> Children
		{ get; }

		#endregion

		public ProductBalance(Product product, IWarehouse balance)
		{
			Product = product;

			Count = balance[product.ID];

			if (product.Children.Count > 0)
			{
				CanBeProduced = decimal.MaxValue;
				var children = new List<ChildProductBalance>();
				foreach (var child in product.Children.Keys)
				{
					var childBalance = new ChildProductBalance(Product, child, balance);
					CanBeProduced = Math.Min(CanBeProduced, childBalance.ParentsCount);
					children.Add(childBalance);
				}
				Children = children.AsReadOnly();
			}
			else
			{
				CanBeProduced = 0;
				Children = new ChildProductBalance[0];
			}
		}

		public override string ToString()
		{
			var result = new StringBuilder();
			result.AppendLine("Имеется на складе: " + Count);

			if (Children.Count > 0)
			{
				result.AppendLine("Может быть произведено: " + CanBeProduced);
				result.AppendLine();
				result.AppendLine("Комплектующие на складе:");

				foreach (var child in Children)
				{
					result.AppendLine("... " + child);
				}
			}

			return result.ToString();
		}
	}

	public class ChildProductBalance
	{
		#region Свойства

		public Product Parent
		{ get; }

		public Product Child
		{ get; }

		public decimal ParentsCount
		{ get; }

		public decimal ChildCount
		{ get; }

		#endregion

		public ChildProductBalance(Product parent, Product child, IWarehouse balance)
		{
			Parent = parent;
			Child = child;
			ChildCount = balance[child.ID];
			ParentsCount = ChildCount/parent.Children[child];
		}

		public override string ToString()
		{
			return string.Format(
				CultureInfo.InvariantCulture,
				"{0} х \"{1}\", что хватит для производства {2} единиц товара",
				ChildCount,
				Child.DisplayMember,
				ParentsCount);
		}
	}
}
