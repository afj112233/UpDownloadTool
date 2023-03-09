using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;

namespace ICSStudio.Database.Database
{
    public class BaseDbHelper
    {
        protected IEnumerable<T> DoQuery<T>(string sql,string connectionString)
        {
            IEnumerable<T> searchResult;

            using (var sqliteConnection = new SQLiteConnection(connectionString))
            {
                sqliteConnection.Open();

                searchResult = sqliteConnection.Query<T>(sql);

                sqliteConnection.Close();
            }

            return searchResult;
        }
    }
}
