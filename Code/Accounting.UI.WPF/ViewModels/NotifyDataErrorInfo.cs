using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Accounting.UI.WPF.ViewModels
{
	public abstract class NotifyDataErrorInfo : INotifyDataErrorInfo
	{
		private readonly IDictionary<string, List<string>> _errorMessages = new Dictionary<string, List<string>>();

		public bool HasErrors
		{ get { return _errorMessages.Count > 0; } }

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		public IEnumerable GetErrors(string propertyName)
		{
			List<string> stringList;
			_errorMessages.TryGetValue(propertyName, out stringList);
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
			_errorMessages[propertyName] = new List<string>(errors);
			RaiseErrorsChanged(propertyName);
		}

		protected void SetError(string error, [CallerMemberName] string propertyName = null)
		{
			SetErrors(new[] { error }, propertyName);
		}

		protected void AddErrors(IEnumerable<string> errors, [CallerMemberName] string propertyName = null)
		{
			List<string> stringList;
			if (!_errorMessages.TryGetValue(propertyName, out stringList))
			{
				_errorMessages[propertyName] = stringList = new List<string>();
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
				_errorMessages.Clear();
			}
			else
			{
				_errorMessages.Remove(propertyName);
			}
			RaiseErrorsChanged(propertyName);
		}
	}
}
