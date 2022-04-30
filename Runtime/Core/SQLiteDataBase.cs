using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Mono.Data.Sqlite;
using UnityEngine;

namespace SQLiteLocalCommunicator
{
	public abstract class SQLiteDatabase
	{

		#region Fields

		public SQLiteDatabaseConfiguration Configuration { get; private set; } = default;

		public string DatabasePath { get; private set; } = default;

		private SqliteConnection _connection = default;

		#endregion

		#region Constructor and Destructor

		public SQLiteDatabase(SQLiteDatabaseConfiguration configuration)
		{
			Configuration = configuration;

			SetDatabasePath();

			Open();
		}

		~SQLiteDatabase()
		{
			Close();

			_connection.Dispose();
		}

		private void SetDatabasePath()
		{
			

			var DirectoryPath = Configuration.DirectoryPath;

			if (!Directory.Exists(DirectoryPath))
			{
				Directory.CreateDirectory(DirectoryPath);
			}

			DatabasePath = $"URI=file:{Configuration.FullPath}";
		}

		#endregion

		#region Open and Close

		private void Open()
		{
			_connection = new SqliteConnection(DatabasePath);
			_connection.Open();
		}

		public void Close()
		{
			_connection.Close();
		}

		#endregion

		#region Commands

		protected void _CommandAndExecuteNonQuery(string commandText)
		{
			var command = _connection.CreateCommand();

			command.CommandText = commandText;

			try
			{
				command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				Debug.LogError(commandText);
			}
		}

		protected IDataReader _CommandAndExecuteReader(string commandText)
		{
			var command = _connection.CreateCommand();

			command.CommandText = commandText;

			try
			{
				return command.ExecuteReader();
			}
			catch (Exception)
			{
				Debug.LogError(commandText);
			}

			return null;
		}

		public T[] CommandAndExecuteReaders<T>(string commandText) where T : SQLiteBox<T>, new()
		{
			return ProceedDataToBoxs<T>(_CommandAndExecuteReader(commandText));
		}

		public T CommandAndExecuteReader<T>(string commandText) where T : SQLiteBox<T>, new()
		{
			return ProceedDataToBox<T>(_CommandAndExecuteReader(commandText));
		}

		public bool CommandAndExecuteReader<T>(string commandText, out T box) where T : SQLiteBox<T>, new()
		{
			return ProceedDataToBox(_CommandAndExecuteReader(commandText), out box);
		}

		#endregion

		#region ProceedDataToBox

		public T ProceedDataToBox<T>(IDataReader reader) where T : SQLiteBox<T>, new()
		{
			return reader.Read() ? new T().Read(reader) : null;
		}

		public bool ProceedDataToBox<T>(IDataReader reader, out T box) where T : SQLiteBox<T>, new()
		{
			if (reader.Read())
			{
				box = new T().Read(reader);
				return true;
			}

			box = null;
			return false;
		}

		public T[] ProceedDataToBoxs<T>(IDataReader reader) where T : SQLiteBox<T>, new()
		{
			var boxs = new List<T>();

			while (reader.Read())
			{
				boxs.Add(new T().Read(reader));
			}

			return boxs.ToArray();
		}

		#endregion

		#region Get

		protected T[] _GetAllData<T>(string tableName) where T : SQLiteBox<T>, new()
		{
			return CommandAndExecuteReaders<T>($"SELECT {SQLiteBox<T>.TKeys} FROM {tableName}");
		}

		#endregion

		#region Insert

		protected void _InsertData<T>(string tableName, params T[] boxs) where T : SQLiteBox<T>, new()
		{
			if (boxs.Length == 1)
			{
				_CommandAndExecuteNonQuery($"INSERT OR REPLACE INTO {tableName} ({SQLiteBox<T>.TKeys}) VALUES {boxs[0].Values}");
			}
			else if (boxs.Length > 1)
			{
				var stringBuilder = new StringBuilder("BEGIN TRANSACTION; ");

#if UNITY_EDITOR
				stringBuilder.Append("\n\n");
				string INSERT = $"INSERT OR REPLACE INTO {tableName} ({SQLiteBox<T>.TKeys}) \nVALUES ";
#else
				string INSERT = $"INSERT OR REPLACE INTO {tableName} ({SQLiteBox<T>.TKeys}) VALUES ";
#endif

				foreach (var box in boxs)
				{
					stringBuilder.Append(INSERT);
					stringBuilder.Append(box.Values);
					stringBuilder.Append("; ");
#if UNITY_EDITOR
					stringBuilder.Append("\n\n");
#endif
				}

				stringBuilder.Append("COMMIT;");

				_CommandAndExecuteNonQuery(stringBuilder.ToString());
			}
		}

		#endregion

		#region Delete

		protected void _DeleteAllData(string tableName)
		{
			_CommandAndExecuteNonQuery($"DELETE FROM {tableName}");
		}

		protected void _DeleteTable(string tableName)
		{
			_CommandAndExecuteNonQuery($"DROP TABLE IF EXISTS {tableName}");
		}

		#endregion

		#region NumOfRows

		protected int _GetNumOfRows(string tableName)
		{
			var data = _CommandAndExecuteReader($"SELECT COUNT() FROM {tableName}");

			return data.Read() ? data.GetInt32(0) : 0;
		}

		#endregion

	}
}