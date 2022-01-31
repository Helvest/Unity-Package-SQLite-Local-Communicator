﻿using System.Data;
using Debug = UnityEngine.Debug;

namespace SQLiteLocalCommunicator
{
	public abstract class SQLiteBox<T> where T : SQLiteBox<T>, new()
	{

		#region SQL Strings

		public static T defaultValue;

		public static string TKeys
		{
			get
			{
				if (defaultValue == null)
				{
					Debug.LogWarning("TEST: " + typeof(T).Name);

					defaultValue = new T();
				}

				return defaultValue.Keys;
			}
		}

		public abstract string Keys { get; }

		public abstract string Values { get; }

		#endregion

		#region Read

		public abstract T Read(IDataReader reader);

		#endregion

	}
}