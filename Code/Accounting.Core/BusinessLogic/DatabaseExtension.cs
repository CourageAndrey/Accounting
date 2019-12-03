using System.Collections.Generic;
using System.Linq;

namespace Accounting.Core.BusinessLogic
{
	public static class DatabaseExtension
	{
		public static IEnumerable<Document> GetActiveDocuments(this Database database)
		{
			return database.Documents.Where(d => d.State == DocumentState.Active).OrderByDescending(d => d.Date);
		}
	}
}
