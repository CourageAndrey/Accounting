using System.Collections.Generic;
using System.Linq;

namespace Accounting.DAL.XML.Enumerations
{
	public enum DocumentState
	{
		Active = 0,
		Edited = 1,
		Deleted = 2,
	}

	public static class DocumentStateConverter
	{
		private static readonly IDictionary<DocumentState, Accounting.Core.BusinessLogic.DocumentState> _enumToClass = new Dictionary<DocumentState, Accounting.Core.BusinessLogic.DocumentState>();
		private static readonly IDictionary<Accounting.Core.BusinessLogic.DocumentState, DocumentState> _classToEnum = new Dictionary<Accounting.Core.BusinessLogic.DocumentState, DocumentState>();

		static DocumentStateConverter()
		{
			var enums = new[]
			{
				DocumentState.Active,
				DocumentState.Edited,
				DocumentState.Deleted,
			};
			var objects = Accounting.Core.BusinessLogic.DocumentState.All.ToArray();
			for (int i = 0; i < objects.Length; i++)
			{
				_classToEnum[objects[i]] = enums[i];
				_enumToClass[enums[i]] = objects[i];
			}
		}

		public static DocumentState ToEnum(this Accounting.Core.BusinessLogic.DocumentState value)
		{
			return _classToEnum[value];
		}

		public static Accounting.Core.BusinessLogic.DocumentState ToClass(this DocumentState value)
		{
			return _enumToClass[value];
		}
	}
}
