namespace SQLiteLocalCommunicator
{
	public abstract class SQLiteTable : SQLiteDatabase
	{

		#region Variables

		public string TableName { get; protected set; }

		#endregion

		#region Constructor

		public SQLiteTable(string tableName, bool buildIn, string DbName) : base(buildIn, DbName)
		{
			_InitTable(tableName);
		}

		public SQLiteTable(string tableName, bool buildIn) : base(buildIn)
		{
			_InitTable(tableName);
		}

		private void _InitTable(string tableName)
		{
			TableName = tableName;
			CreateTable();
		}

		/// <summary>
		/// Create the table, call by the constructor
		/// </summary>
		public abstract void CreateTable();

		#endregion

		#region Get

		public T[] GetAllData<T>() where T : SQLiteBox<T>, new()
		{
			return _GetAllData<T>(TableName);
		}

		#endregion

		#region Insert

		public void InsertData<T>(params T[] boxs) where T : SQLiteBox<T>, new()
		{
			_InsertData(TableName, boxs);
		}

		#endregion

		#region Delete

		public void DeleteAllData()
		{
			_DeleteAllData(TableName);
		}

		public void DeleteTable()
		{
			_DeleteTable(TableName);
		}

		#endregion

		#region NumOfRows

		public int GetNumOfRows()
		{
			return _GetNumOfRows(TableName);
		}

		#endregion

	}
}
