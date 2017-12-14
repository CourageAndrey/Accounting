using System.Collections.Generic;

namespace ComfortIsland.Database
{
	public enum DocumentState
	{
		Active = 0,
		Edited = 1,
		Deleted = 2,
	}

	public static class DocumentStateHelper
	{
		public static string StateToString(this DocumentState state)
		{
			return stateNames[state];
		}

		public static readonly IDictionary<DocumentState, string> stateNames = new Dictionary<DocumentState, string>
		{
			{ DocumentState.Active, "действует" },
			{ DocumentState.Edited, "был изменён" },
			{ DocumentState.Deleted, "был удалён" },
		};
	}
}
