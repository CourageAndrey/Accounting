using System.Text;

namespace ComfortIsland.Database
{
	partial class Document : IEditable<Document>
	{
		public DocumentTypeEnum DocumentTypeEnum
		{
			get { return (DocumentTypeEnum) TypeID; }
			set { TypeID = (short) value; }
		}

		public void Update(Document other)
		{
#warning Implement
			throw new System.NotImplementedException();
		}

		public bool Validate(ComfortIslandDatabase database, out StringBuilder errors)
		{
#warning Implement
			throw new System.NotImplementedException();
		}

		public Document PrepareToDisplay(ComfortIslandDatabase database)
		{
#warning Implement
			throw new System.NotImplementedException();
		}

		public void PrepareToSave(ComfortIslandDatabase database)
		{
#warning Implement
			throw new System.NotImplementedException();
		}
	}
}
