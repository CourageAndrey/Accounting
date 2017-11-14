using ComfortIsland.Database;

namespace ComfortIsland
{
	interface IEditDialog<T>
	{
		T EditValue { get; set; }

		void Initialize(ComfortIslandDatabase database);
	}
}
