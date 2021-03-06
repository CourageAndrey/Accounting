﻿using Accounting.Core.Reports;

namespace Accounting.Core.Application
{
	public interface IReportExporter
	{
		string SaveFileDialogFilter
		{ get; }

		void ExportReport(IReport report, string fileName);
	}
}
