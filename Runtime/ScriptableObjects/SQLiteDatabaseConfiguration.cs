using UnityEngine;

namespace SQLiteLocalCommunicator
{
	[CreateAssetMenu(menuName = "SQLite/DatabaseConfiguration")]
	public class SQLiteDatabaseConfiguration : ScriptableObject
	{
		public PathData pathData = new PathData()
		{
			path = "SQLiteDatabases",
			fileName = "Global",
			extension = ".db"
		};	

		public string FullPath => pathData.GetFullPath();

		public string DirectoryPath => pathData.GetDirectoryPath();
	}
}