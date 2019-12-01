using System.Collections.Generic;
using System.Linq;

namespace ComfortIsland.DataAccessLayer.Xml
{
	public enum DocumentType
	{
		Income = 0,
		Outcome = 1,
		Produce = 2,
		ToWarehouse = 3,
	}

	public static class DocumentTypeConverter
	{
		private static readonly IDictionary<DocumentType, BusinessLogic.DocumentType> _enumToClass = new Dictionary<DocumentType, BusinessLogic.DocumentType>();
		private static readonly IDictionary<BusinessLogic.DocumentType, DocumentType> _classToEnum = new Dictionary<BusinessLogic.DocumentType, DocumentType>();

		static DocumentTypeConverter()
		{
			var enums = new[]
			{
				DocumentType.Income,
				DocumentType.Outcome,
				DocumentType.Produce,
				DocumentType.ToWarehouse,
			};
			var objects = BusinessLogic.DocumentType.All.ToArray();
			for (int i = 0; i < objects.Length; i++)
			{
				_classToEnum[objects[i]] = enums[i];
				_enumToClass[enums[i]] = objects[i];
			}
		}

		public static DocumentType ToEnum(this BusinessLogic.DocumentType value)
		{
			return _classToEnum[value];
		}

		public static BusinessLogic.DocumentType ToClass(this DocumentType value)
		{
			return _enumToClass[value];
		}
	}
}
