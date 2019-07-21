// Original post: https://habr.com/ru/post/124110/
// Author: sdramare
// Source code: http://code.google.com/p/wpf-auto-tests/source/checkout
using System;
using System.Collections.Generic;

namespace ComfortIsland.UiTests.Framework
{
	public static class Monad
	{
		public static TResult With<T, TResult>(this T obj, [NotNull] Func<T, TResult> selector,
			[NotNull] Func<TResult> @default)
			where T : class
		{
			return obj == null ? @default() : selector(obj);
		}

		public static TResult With<T, TResult>(this T obj, [NotNull] Func<T, TResult> selector,
			TResult @default = default(TResult))
			where T : class
		{
			return obj.With(selector, () => @default);
		}

		public static T Do<T>(this T obj, [NotNull] Action<T> action)
			where T : class
		{
			if (obj != null)
			{
				action(obj);
			}
			return obj;
		}

		public static void ForEach<T>(this IEnumerable<T> source, [NotNull] Action<T> action)
		{
			foreach (var item in source)
			{
				action(item);
			}
		}

		public static Func<T, TResult> Safe<T, TResult, TException>(this Func<T, TResult> selector,
			Action<TException> handler = default(Action<TException>))
			where TException : Exception
		{
			return arg =>
			{
				try
				{
					return selector(arg);
				}
				catch (TException exception)
				{
					var ex = exception;
					handler.Do(x => x(ex)).Unless(() => Console.WriteLine(ex.Message));
					return default(TResult);
				}
			};

		}

		public static Func<T, TResult> Safe<T, TResult>(this Func<T, TResult> selector)
		{
			return Safe<T, TResult, Exception>(selector);
		}

		public static T If<T>(this T obj, Func<T, bool> predicate)
			where T : class
		{
			if (obj != null && predicate(obj))
			{
				return obj;
			}
			return null;
		}

		public static void Unless<T>(this T obj, Action action)
			where T : class
		{
			if (obj == null)
			{
				action();
			}
		}
	}
}
