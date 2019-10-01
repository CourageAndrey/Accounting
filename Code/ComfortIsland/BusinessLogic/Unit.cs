using System;
using System.Text;

namespace ComfortIsland.BusinessLogic
{
	public class Unit : IEntity
	{
		#region Properties

		public long ID
		{ get; set; }

		public string Name
		{
			get { return _name; }
			set
			{
				var errors = new StringBuilder();
				if (NameIsNotNullOrEmpty(value, errors))
				{
					_name = value;
				}
				else
				{
					throw new ArgumentException(errors.ToString());
				}
			}
		}

		public string ShortName
		{
			get { return _shortName; }
			set
			{
				var errors = new StringBuilder();
				if (ShortNameIsNotNullOrEmpty(value, errors))
				{
					_shortName = value;
				}
				else
				{
					throw new ArgumentException(errors.ToString());
				}
			}
		}

		private string _name, _shortName;

		#endregion

		#region Валидация

		public static bool NameIsNotNullOrEmpty(string name, StringBuilder errors)
		{
			if (string.IsNullOrEmpty(name))
			{
				errors.AppendLine("Наименование не может быть пустой строкой.");
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool ShortNameIsNotNullOrEmpty(string shortName, StringBuilder errors)
		{
			if (string.IsNullOrEmpty(shortName))
			{
				errors.AppendLine("Сокращение не может быть пустой строкой.");
				return false;
			}
			else
			{
				return true;
			}
		}

		#endregion
	}
}
