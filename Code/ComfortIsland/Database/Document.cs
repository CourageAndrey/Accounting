using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ComfortIsland.Database
{
	partial class Document : IEditable<Document>
	{
		public DocumentTypeEnum DocumentTypeEnum
		{
			get { return (DocumentTypeEnum) TypeID; }
			set { TypeID = (short) value; }
		}

		public List<DocumentPosition> BindingPositions
		{ get; private set; }

		public string TypeName
		{ get; internal set; }

		public Document()
		{
			BindingPositions = new List<DocumentPosition>();
		}

		public void Update(Document other)
		{
			this.ID = other.ID;
			this.TypeID = other.TypeID;
			this.Number = other.Number;
			this.Date = other.Date;
			this.BindingPositions = new List<DocumentPosition>(other.BindingPositions);
		}

		public bool Validate(ComfortIslandDatabase database, out StringBuilder errors)
		{
			errors = new StringBuilder();
			if (string.IsNullOrEmpty(Number))
			{
				errors.AppendLine("Номер не может быть пустой строкой.");
			}
			if (BindingPositions.Count <= 0)
			{
				errors.AppendLine("В документе не выбрано ни одного продукта.");
			}
			List<DocumentPosition> positionsToCheck;
			if (DocumentTypeEnum == DocumentTypeEnum.Income)
			{
				positionsToCheck = new List<DocumentPosition>();
			}
			else if (DocumentTypeEnum == DocumentTypeEnum.Outcome)
			{
				positionsToCheck = BindingPositions;
			}
			else if (DocumentTypeEnum == DocumentTypeEnum.Produce)
			{
				positionsToCheck = new List<DocumentPosition>();
				foreach (var position in positionsToCheck)
				{
					var product = database.Product.Execute(MergeOption.NoTracking).First(p => p.ID == position.ProductId);
					if (!product.Children.IsLoaded)
					{
						product.Children.Load(MergeOption.NoTracking);
					}
					foreach (var child in product.Children)
					{
						positionsToCheck.Add(new DocumentPosition
						{
							ProductId = child.ChildID,
							Count = position.Count * child.Count,
						});
					}
				}
			}
			else
			{
				throw new NotSupportedException();
			}
			foreach (var position in positionsToCheck)
			{
				var balance = database.Balance.Execute(MergeOption.NoTracking).FirstOrDefault(b => b.ProductID == position.ProductId);
				long count = balance != null ? balance.Count : 0;
				if (count < position.Count)
				{
					errors.AppendLine(string.Format(
						CultureInfo.InvariantCulture,
						DocumentPosition.InsufficientProductsCount,
						database.Product.Execute(MergeOption.NoTracking).First(p => p.ID == position.ProductId).PrepareToDisplay(database).DisplayMember,
						count,
						position.Count));
				}
			}
			foreach (var position in BindingPositions)
			{
				if (position.Count <= 0)
				{
					errors.AppendLine(countMustBePositive);
				}
			}
			var products = BindingPositions.Select(p => p.ProductId).ToList();
			if (products.Count > products.Distinct().Count())
			{
				errors.Append("Дублирование позиций в документе");
			}
			return errors.Length == 0;
		}

		private const string countMustBePositive = "Количество товара во всех позициях должно быть строго больше ноля.";

		public Document PrepareToDisplay(ComfortIslandDatabase database)
		{
			if (!TypeReference.IsLoaded)
			{
				TypeReference.Load(MergeOption.NoTracking);
			}
			TypeName = Type.Name;
			if (!Positions.IsLoaded)
			{
				Positions.Load(MergeOption.NoTracking);
			}
			BindingPositions = Positions.Select(p => new DocumentPosition(p)).ToList();
			return this;
		}

		public void PrepareToSave(ComfortIslandDatabase database)
		{
			var oldPositions = database.Position.Where(p => p.DocumentID == ID).ToList();
			foreach (var position in oldPositions)
			{
				var newPosition = BindingPositions.FirstOrDefault(p => p.ProductId != position.ProductID);
				if (newPosition == null)
				{
					database.DeleteObject(position);
				}
				else
				{
					position.Count = newPosition.Count;
				}
			}
			foreach (var position in BindingPositions)
			{
				if (oldPositions.All(p => p.ProductID != position.ProductId))
				{
					database.AddToPosition(new Position
					{
						DocumentID = ID,
						ProductID = position.ProductId,
						Count = position.Count,
					});
				}
			}
		}
	}

	public class DocumentPosition
	{
		public long ProductId
		{ get; set; }

		public long Count
		{ get; set; }

		public DocumentPosition()
		{ }

		public DocumentPosition(Position position)
		{
			ProductId = position.ProductID;
			Count = position.Count;
		}

		public void IncreaseBalance(ComfortIslandDatabase database)
		{
			var balance = database.Balance.FirstOrDefault(b => b.ProductID == ProductId);
			if (balance == null)
			{
				balance = new Balance { ProductID = ProductId };
				database.AddToBalance(balance);
			}
			balance.Count += Count;
		}

		public void DecreaseBalance(ComfortIslandDatabase database)
		{
			var balance = database.Balance.FirstOrDefault(b => b.ProductID == ProductId);
			long count = balance != null ? balance.Count : 0;
			if (count >= Count)
			{
				balance.Count -= Count;
			}
			else
			{
				throw new Exception(string.Format(
					CultureInfo.InvariantCulture,
					InsufficientProductsCount,
					database.Product.Execute(MergeOption.NoTracking).First(p => p.ID == ProductId).PrepareToDisplay(database).DisplayMember,
					count,
					Count));
			}
		}

		internal const string InsufficientProductsCount = "Недостаточно товара \"{0}\". Имеется по факту: {1}, требуется {2}.";
	}
}
