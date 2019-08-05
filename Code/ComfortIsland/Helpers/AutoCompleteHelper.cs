using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ComfortIsland.Helpers
{
	public static class AutoCompleteHelper
	{
		public static void PreviewTextInput(ComboBox comboBox, TextCompositionEventArgs e)
		{
			comboBox.Tag = comboBox.Tag ?? comboBox.ItemsSource;

			comboBox.IsDropDownOpen = true;

			if (!string.IsNullOrEmpty(comboBox.Text))
			{
				string fullText = comboBox.Text.Insert(getChildOfType<TextBox>(comboBox).CaretIndex, e.Text);
				comboBox.ItemsSource = comboBox.getOriginalItemSource().filter(fullText).ToList();
			}
			else if (!string.IsNullOrEmpty(e.Text))
			{
				comboBox.ItemsSource = comboBox.getOriginalItemSource().filter(e.Text).ToList();
			}
			else
			{
				comboBox.ItemsSource = comboBox.getOriginalItemSource();
			}
		}

		public static void Pasting(ComboBox comboBox, DataObjectPastingEventArgs e)
		{
			comboBox.Tag = comboBox.Tag ?? comboBox.ItemsSource;

			comboBox.IsDropDownOpen = true;

			string pastedText = (string)e.DataObject.GetData(typeof(string));
			string fullText = comboBox.Text.Insert(getChildOfType<TextBox>(comboBox).CaretIndex, pastedText);

			if (!string.IsNullOrEmpty(fullText))
			{
				comboBox.ItemsSource = comboBox.getOriginalItemSource().filter(fullText).ToList();
			}
			else
			{
				comboBox.ItemsSource = comboBox.getOriginalItemSource();
			}
		}

		public static void PreviewKeyUp(ComboBox comboBox, KeyEventArgs e)
		{
			comboBox.Tag = comboBox.Tag ?? comboBox.ItemsSource;

			if (e.Key == Key.Back || e.Key == Key.Delete || (e.Key == Key.X && (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))
			{
				comboBox.IsDropDownOpen = true;

				if (!string.IsNullOrEmpty(comboBox.Text))
				{
					comboBox.ItemsSource = comboBox.getOriginalItemSource().filter(comboBox.Text).ToList();
				}
				else
				{
					comboBox.ItemsSource = comboBox.getOriginalItemSource();
				}
			}
		}

		private static IEnumerable<IListBoxItem> getOriginalItemSource(this ComboBox comboBox)
		{
			return (comboBox.Tag as System.Collections.IEnumerable ?? new IListBoxItem[0]).OfType<IListBoxItem>();
		}

		private static IEnumerable<IListBoxItem> filter(this IEnumerable<IListBoxItem> items, string searchPattern)
		{
			return items.Where(p => p.DisplayMember.ToString().IndexOf(searchPattern, StringComparison.InvariantCultureIgnoreCase) >= 0);
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
