using System.Text;

namespace ComfortIsland.Database
{
	partial class Unit : IEditable<Unit>
	{
		public void Update(Unit other)
		{
			this.ID = other.ID;
			this.Name = other.Name;
			this.ShortName = other.ShortName;
		}

		public bool Validate(ComfortIslandDatabase database, out StringBuilder errors)
		{
			errors = new StringBuilder();
			if (string.IsNullOrEmpty(Name))
			{
				errors.AppendLine("Наименование не может быть пустой строкой.");
			}
			if (string.IsNullOrEmpty(ShortName))
			{
				errors.AppendLine("Сокращение не может быть пустой строкой.");
			}
			return errors.Length == 0;
		}

		public Unit PrepareToDisplay(ComfortIslandDatabase database)
		{
			return this;
		}

		public void PrepareToSave(ComfortIslandDatabase database)
		{ }
	}
}
