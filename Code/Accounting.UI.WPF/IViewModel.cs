﻿using Accounting.Core.BusinessLogic;

namespace ComfortIsland
{
	public interface IViewModel<out T>
	{
		long? ID
		{ get; }

		T ConvertToBusinessLogic(Database database);
	}
}