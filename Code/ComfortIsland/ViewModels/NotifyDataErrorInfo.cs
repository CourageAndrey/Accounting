using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ComfortIsland.ViewModels
{
	public abstract class NotifyDataErrorInfo : INotifyDataErrorInfo
	{
		private IDictionary<string, List<string>> errorMessages = new Dictionary<string, List<string>>();

		public bool HasErrors
		{ get { return errorMessages.Count > 0; } }

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		public IEnumerable GetErrors(string propertyName)
		{
			List<string> stringList;
			errorMessages.TryGetValue(propertyName, out stringList);
			return stringList;
		}

		protected void RaiseErrorsChanged([CallerMemberName] string propertyName = null)
		{
			var eventHandler = Volatile.Read(ref ErrorsChanged);
			if (eventHandler != null)
			{
				eventHandler(this, new DataErrorsChangedEventArgs(propertyName));
			}
		}

		protected void SetErrors(IEnumerable<string> errors, [CallerMemberName] string propertyName = null)
		{
			errorMessages[propertyName] = new List<string>(errors);
			RaiseErrorsChanged(propertyName);
		}

		protected void SetError(string error, [CallerMemberName] string propertyName = null)
		{
			SetErrors(new[] { error }, propertyName);
		}

		protected void AddErrors(IEnumerable<string> errors, [CallerMemberName] string propertyName = null)
		{
			List<string> stringList;
			if (!errorMessages.TryGetValue(propertyName, out stringList))
			{
				errorMessages[propertyName] = stringList = new List<string>();
			}
			stringList.AddRange(errors);
			RaiseErrorsChanged(propertyName);
		}

		protected void AddError(string error, [CallerMemberName] string propertyName = null)
		{
			AddErrors(new[] { error }, propertyName);
		}

		protected void ClearErrors([CallerMemberName] string propertyName = null)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				errorMessages.Clear();
			}
			else
			{
				errorMessages.Remove(propertyName);
			}
			RaiseErrorsChanged(propertyName);
		}
	}
}
