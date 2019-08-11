using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ComfortIsland.BusinessLogic;
using ComfortIsland.Helpers;

namespace ComfortIsland.Dialogs
{
	public partial class DocumentDialog : IEditDialog<ViewModels.Document>
	{
		public DocumentDialog()
		{
			InitializeComponent();
		}

		public void ConnectTo(IApplication application)
		{
			_application = application;
			FontSize = application.Settings.FontSize;
			comboBoxProducts.ItemsSource = ProductsGetter != null
				 ? ProductsGetter(application.Database)
				 : application.Database.Products;
		}

		private IApplication _application;

		public ViewModels.Document EditValue
		{
			get { return (ViewModels.Document) contextControl.DataContext; }
			set
			{
				buttonOk.IsEnabled = !value.HasErrors;
				value.ErrorsChanged += (sender, args) => { buttonOk.IsEnabled = !value.HasErrors; };
				contextControl.DataContext = value;
			}
		}

		public Func<Database, IEnumerable<Product>> ProductsGetter
		{ get; set; }

		private void okClick(object sender, RoutedEventArgs e)
		{
			if (!EditValue.HasErrors)
			{
				StringBuilder errors = new StringBuilder();
				bool isValid =	Position.PositionsDoNotDuplicate(EditValue.Positions, "товарные позиции", errors) &
								Document.PositionsCountHasToBePositive(EditValue.Positions, errors);
				for (int line = 0; line < EditValue.Positions.Count; line++)
				{
					isValid &= Position.ProductIsSet(EditValue.Positions[line].ID, line + 1, errors);
					isValid &= Position.CountIsPositive(EditValue.Positions[line].Count, line + 1, errors);
				}
				if (isValid)
				{
					var documentStub = new Document(EditValue.ID, EditValue.Type, DocumentState.Active);
					EditValue.ApplyChanges(documentStub, _application.Database.Products);
					isValid &= (!EditValue.ID.HasValue || _application.Database.Documents[EditValue.ID.Value].State != DocumentState.Active)
						? _application.Settings.BalanceValidationStrategy.VerifyCreate(_application.Database, documentStub, errors)
						: _application.Settings.BalanceValidationStrategy.VerifyEdit(_application.Database, documentStub, errors);
				}
				if (isValid)
				{
					DialogResult = true;
				}
				else
				{
					MessageBox.Show(errors.ToString(), "Невозможно применить документ", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}

		private void cancelClick(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}

		public void SetReadOnly()
		{
			foreach (var child in contextControl.Children)
			{
				if (child is TextBox)
				{
					(child as TextBox).IsReadOnly = true;
				}
				else if (child is DatePicker)
				{
					(child as DatePicker).IsEnabled = false;
				}
				else if (child is GroupBox)
				{
					var grid = (child as GroupBox).Content as DataGrid;
					if (grid != null)
					{
						grid.IsReadOnly = true;
					}
				}
			}
			buttonOk.Visibility = Visibility.Hidden;
		}

		#region ComboBox autocomplete

		private void previewTextInput(object sender, TextCompositionEventArgs e)
		{
			AutoCompleteHelper.PreviewTextInput((ComboBox)sender, e);
		}

		private void pasting(object sender, DataObjectPastingEventArgs e)
		{
			AutoCompleteHelper.Pasting((ComboBox)sender, e);
		}

		private void previewKeyUp(object sender, KeyEventArgs e)
		{
			AutoCompleteHelper.PreviewKeyUp((ComboBox)sender, e);
		}

		#endregion
	}
}
