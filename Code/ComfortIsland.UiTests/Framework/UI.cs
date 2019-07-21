// Original post: https://habr.com/ru/post/124110/
// Author: sdramare
// Source code: http://code.google.com/p/wpf-auto-tests/source/checkout
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Media;
using System.Linq;

namespace ComfortIsland.UiTests.Framework
{
	public static class UI
	{
		public static TResult Run<TResult>(Func<TResult> activator)
			where TResult : Application
		{
			if (activator == null)
			{
				throw new ArgumentNullException("activator");
			}

			TResult app = null;
			var mutex = new Mutex();
			var thread = new Thread(() =>
			{
				mutex.WaitOne();
				app = activator();
				app.Startup += delegate { mutex.ReleaseMutex(); };
				app.Run(app.MainWindow);
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Name = "UI_run_thread";
			thread.Start();
			Thread.Sleep(1000);
			mutex.WaitOne();
			return app;
		}

		public static void Raise<T>([NotNull] this T element, [NotNull] RoutedEvent @event)
			where T : UIElement
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			if (@event == null)
			{
				throw new ArgumentNullException("event");
			}

			element.Dispatcher.Invoke((Action) (() => element.RaiseEvent(new RoutedEventArgs(@event))));
		}

		public static void Invoke<T>(this T element, Action<T> action)
			where T : DispatcherObject
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			element.Dispatcher.Invoke(action, element);
		}

		public static TResult Get<T, TResult>(this T element, Func<T, TResult> selector)
			where T : DispatcherObject
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			if (selector == null)
			{
				throw new ArgumentNullException("selector");
			}

			return (TResult) element.Dispatcher.Invoke(selector, element);
		}

		public static IEnumerable<T> FindChilds<T>(this DependencyObject parent, Func<T, bool> predicate = null)
			where T : DependencyObject
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}

			predicate = predicate ?? (x => true);

			if (Thread.CurrentThread != parent.Dispatcher.Thread)
			{
				foreach (var child in parent.Get(x => x.FindChilds(predicate).ToList()))
				{
					yield return child;
				}
				yield break;
			}

			var queue = new Queue<DependencyObject>();

			queue.Enqueue(parent);

			while (queue.Count != 0)
			{
				var currentElement = queue.Dequeue();
				if ((currentElement as T).With(predicate))
				{
					yield return (T) currentElement;
				}

				Enumerable.Range(0, VisualTreeHelper.GetChildrenCount(currentElement))
					.Select(i => VisualTreeHelper.GetChild(currentElement, i))
					.ForEach(queue.Enqueue);
			}
		}

		public static T FindChild<T>(this DependencyObject parent, Func<T, bool> predicate = null)
			where T : DependencyObject
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			return Thread.CurrentThread != parent.Dispatcher.Thread
				? parent.Get(x => x.FindChild(predicate))
				: parent.FindChilds(predicate).FirstOrDefault();
		}

		public static byte[] Render(this FrameworkElement element)
		{
			return element.Get(x =>
			{
				var png = new RenderTargetBitmap((int) element.ActualWidth + 1, (int) element.ActualHeight + 1, 96, 96,
					PixelFormats.Pbgra32);
				png.Render(element);
				var encoder = new PngBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(png));

				using (var stream = new MemoryStream())
				{
					encoder.Save(stream);
					stream.Position = 0;
					return stream.ToArray();
				}
			});

		}
	}
}