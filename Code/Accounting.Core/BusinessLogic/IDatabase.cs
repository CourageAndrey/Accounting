namespace Accounting.Core.BusinessLogic
{
	public interface IDatabase
	{
		IRegistry<Unit> Units
		{ get; }

		IRegistry<Product> Products
		{ get; }

		IWarehouse Balance
		{ get; }

		IRegistry<Document> Documents
		{ get; }
	}
}