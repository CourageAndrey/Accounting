using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ComfortIsland.Database
{
	public enum DocumentType
	{
		Income,
		Outcome,
		Produce,
	}

	public static class DocumentTypeHelper
	{
		public static string ToStringRepresentation(this DocumentType type)
		{
			return AllTypes[type];
		}

		public static readonly IDictionary<DocumentType, string> AllTypes = new ReadOnlyDictionary<DocumentType, string>(new Dictionary<DocumentType, string>
		{
			{ DocumentType.Income, "приход" },
			{ DocumentType.Outcome, "продажа" },
			{ DocumentType.Produce, "производство" },
		});
	}
}
