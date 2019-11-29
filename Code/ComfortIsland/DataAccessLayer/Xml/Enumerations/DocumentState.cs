using System.Collections.Generic;
using System.Linq;

namespace ComfortIsland.DataAccessLayer.Xml
{
	public enum DocumentState
	{
		Active = 0,
		Edited = 1,
		Deleted = 2,
	}

	public static class DocumentStateConverter
	{
		private static readonly IDictionary<DocumentState, BusinessLogic.DocumentState> _enumToClass = new Dictionary<DocumentState, BusinessLogic.DocumentState>();
		private static readonly IDictionary<BusinessLogic.DocumentState, DocumentState> _classToEnum = new Dictionary<BusinessLogic.DocumentState, DocumentState>();

		static DocumentStateConverter()
		{
			var enums = new[]
			{
				DocumentState.Active,
				DocumentState.Edited,
				DocumentState.Deleted,
			};
			var objects = BusinessLogic.DocumentState.All.ToArray();
			for (int i = 0; i < objects.Length; i++)
			{
				_classToEnum[objects[i]] = enums[i];
				_enumToClass[enums[i]] = objects[i];
			}
		}

		public static DocumentState ToEnum(this BusinessLogic.DocumentState value)
		{
			return _classToEnum[value];
		}

		public static BusinessLogic.DocumentState ToClass(this DocumentState value)
		{
			return _enumToClass[value];
		}
	}
}
