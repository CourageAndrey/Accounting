using System.Text;

namespace ComfortIsland.Database
{
	partial class Price : IEditable<Price>
	{
		public void Update(Price other)
		{
			this.ID = other.ID;
			this.ProductID = other.ProductID;
			this.Value = other.Value;
		}

		public string ProductCode
		{ get; private set; }

		public string ProductName
		{ get; private set; }

		public string ProductUnit
		{ get; private set; }

		public bool Validate(ComfortIslandDatabase database, out StringBuilder errors)
		{
			errors = new StringBuilder();
			if (ProductID <= 0)
			{
				errors.AppendLine("Не выбран товар.");
			}
			if (Value <= 0)
			{
				errors.AppendLine("Цена должна быть строго больше ноля.");
			}
			return errors.Length == 0;
		}

		public Price PrepareToDisplay(ComfortIslandDatabase database)
		{
			ProductCode = Product.Code;
			ProductName = Product.Name;
			ProductUnit = Product.Unit.ShortName;
			return this;
		}

		public void PrepareToSave(ComfortIslandDatabase database)
		{ }
	}
}
