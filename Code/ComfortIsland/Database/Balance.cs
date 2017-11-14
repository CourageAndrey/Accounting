using System;
using System.Data.Objects;
using System.Text;

namespace ComfortIsland.Database
{
	partial class Balance : IEditable<Balance>
	{
		public string ProductCode
		{ get; private set; }

		public string ProductName
		{ get; private set; }

		public string ProductUnit
		{ get; private set; }

		public void Update(Balance other)
		{
			throw new NotSupportedException();
		}

		public bool Validate(ComfortIslandDatabase database, out StringBuilder errors)
		{
			throw new NotSupportedException();
		}

		public Balance PrepareToDisplay(ComfortIslandDatabase database)
		{
			if (!ProductReference.IsLoaded)
			{
				ProductReference.Load(MergeOption.NoTracking);
			}
			if (!Product.UnitReference.IsLoaded)
			{
				Product.UnitReference.Load(MergeOption.NoTracking);
			}
			ProductCode = Product.Code;
			ProductName = Product.Name;
			ProductUnit = Product.Unit.ShortName;
			return this;
		}

		public void PrepareToSave(ComfortIslandDatabase database)
		{
			throw new NotSupportedException();
		}
	}
}
