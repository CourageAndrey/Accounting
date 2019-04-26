using System.Collections.Generic;
using System.Linq;

namespace ComfortIsland.BusinessLogic
{
	public class DocumentState
	{
		#region Properties

		public Xml.DocumentState Enum
		{ get; }

		public string Name
		{ get; }

		#endregion

		private DocumentState(Xml.DocumentState enumValue, string name)
		{
			Enum = enumValue;
			Name = name;
		}

		public override string ToString()
		{
			return Name;
		}

		#region List

		public static readonly DocumentState Active = new DocumentState(Xml.DocumentState.Active, "действует");
		public static readonly DocumentState Edited = new DocumentState(Xml.DocumentState.Edited, "был изменён");
		public static readonly DocumentState Deleted = new DocumentState(Xml.DocumentState.Deleted, "был удалён");

		public static readonly IDictionary<Xml.DocumentState, DocumentState> AllStates = new[] { Active, Edited, Deleted }.ToDictionary(
			state => state.Enum,
			state => state);

		#endregion
	}
}
