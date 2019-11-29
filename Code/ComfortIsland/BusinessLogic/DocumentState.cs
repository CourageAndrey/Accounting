using System.Collections.Generic;

namespace ComfortIsland.BusinessLogic
{
	public class DocumentState
	{
		#region Properties

		public string Name
		{ get; }

		#endregion

		private DocumentState(string name)
		{
			Name = name;
		}

		public override string ToString()
		{
			return Name;
		}

		#region List

		public static readonly DocumentState Active = new DocumentState("действует");
		public static readonly DocumentState Edited = new DocumentState("был изменён");
		public static readonly DocumentState Deleted = new DocumentState("был удалён");
		public static readonly IEnumerable<DocumentState> All = new[]
		{
			Active,
			Edited,
			Deleted,
		};

		#endregion
	}
}
