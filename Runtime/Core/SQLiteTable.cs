using EasyPath;

namespace SQLiteLocalCommunicator
{
	public abstract class SQLiteTable : SQLiteDatabase
	{

		#region Fields

		public abstract string TableName { get; }

		#endregion

		#region Constructor

		public SQLiteTable(IPath path) : base(path)
		{
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
