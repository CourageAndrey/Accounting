﻿using System;
using System.Collections.Generic;

using ComfortIsland.BusinessLogic;
using ComfortIsland.Dialogs;

namespace ComfortIsland.Reports
{
	public class BalanceReportDescriptor : ReportDescriptor
	{
		public override string Title
		{ get { return "Складские остатки"; } }

		private IEnumerable<ReportColumn> columns;

		public override IEnumerable<ReportColumn> GetColumns()
		{
			return columns ?? (columns = new List<ReportColumn>
			{
				new ReportColumn
				(
					"Товар",
					"BoundProduct.Name",
					false,
					300
				),
				new ReportColumn
				(
					"Ед/изм",
					"BoundProduct.Unit.Name",
					false,
					50
				),
				new ReportColumn
				(
					"Остатки",
					"Count",
					true,
					100
				),
			});
		}

		public override bool CreateReport(IAccountingApplication application, Database database, out IReport report)
		{
			var dialog = new SelectDateDialog { EditValue = DateTime.Now };
			dialog.ConnectTo(application);
			if (dialog.ShowDialog() == true)
			{
				report = new BalanceReport(application.Database, dialog.EditValue, dialog.IncludeAllProducts);
				return true;
			}
			else
			{
				report = null;
				return false;
			}
		}
	}
}
