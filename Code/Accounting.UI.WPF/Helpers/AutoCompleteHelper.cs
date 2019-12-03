using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Accounting.Core.BusinessLogic;

namespace ComfortIsland.Helpers
{
	public static class AutoCompleteHelper
	{
		public static void PreviewTextInput(ComboBox comboBox, TextCompositionEventArgs e)
		{
			comboBox.Tag = comboBox.Tag ?? comboBox.ItemsSource;

			comboBox.IsDropDownOpen = true;

			var items = comboBox.getOriginalItemSource();
			if (!string.IsNullOrEmpty(comboBox.Text))
			{
				string fullText = comboBox.insertFullText(e.Text);
				items = items.filter(fullText);
			}
			else if (!string.IsNullOrEmpty(e.Text))
			{
				items = items.filter(e.Text);
			}
			comboBox.ItemsSource = items;
		}

		public static void Pasting(ComboBox comboBox, DataObjectPastingEventArgs e)
		{
			comboBox.Tag = comboBox.Tag ?? comboBox.ItemsSource;

			comboBox.IsDropDownOpen = true;

			string pastedText = (string) e.DataObject.GetData(typeof(string));
			string fullText = comboBox.insertFullText(pastedText);

			var items = comboBox.getOriginalItemSource();
			if (!string.IsNullOrEmpty(fullText))
			{
				items = items.filter(fullText);
			}
			comboBox.ItemsSource = items;
		}

		public static void PreviewKeyUp(ComboBox comboBox, KeyEventArgs e)
		{
			comboBox.Tag = comboBox.Tag ?? comboBox.ItemsSource;

			bool isCut = e.Key == Key.X && (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
			bool isKeyDeleting = e.Key == Key.Back || e.Key == Key.Delete || isCut;
			if (isKeyDeleting)
			{
				comboBox.IsDropDownOpen = true;

				var items = comboBox.getOriginalItemSource();
				if (!string.IsNullOrEmpty(comboBox.Text))
				{
					items = items.filter(comboBox.Text);
				}
				comboBox.ItemsSource = items;
			}
		}

		private static IEnumerable<IListItem> getOriginalItemSource(this ComboBox comboBox)
		{
			return (comboBox.Tag as System.Collections.IEnumerable ?? new IListItem[0]).OfType<IListItem>();
		}

		private static IEnumerable<IListItem> filter(this IEnumerable<IListItem> items, string searchPattern)
		{
			return items.Where(p => p.DisplayMember.IndexOf(searchPattern, StringComparison.InvariantCultureIgnoreCase) >= 0);
		}

		private static string insertFullText(this ComboBox comboBox, string searchPattern)
		{
			return comboBox.Text.Insert(getChildOfType<TextBox>(comboBox).CaretIndex, searchPattern);
		}

		private static T getChildOfType<T>(DependencyObject dependencyObject)
			where T : DependencyObject
		{
			if (dependencyObject == null) return null;
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
			{
				var child = VisualTreeHelper.GetChild(dependencyObject, i);
				var result = (child as T) ?? getChildOfType<T>(child);
				if (result != null) return result;
			}
			return null;
		}
	}
}
