using Dapper;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace NancyMusicStore.Common
{
    public class DBHelper : IDbHelper
    {
        private readonly string connectionString;
        public DBHelper(string connection)
        {
            connectionString = connection;
        }
        //open connection       
        public  IDbConnection Connection  => new NpgsqlConnection(connectionString);
        
        //execute 
        public  int Execute(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            using (Connection)
            {
                return Connection.Execute(sql, param, transaction, commandTimeout, commandType);
            }
        }

        //execute 
        public  object ExecuteScalar(string cmd, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            using (Connection)
            {
                return Connection.ExecuteScalar(cmd, param, transaction, commandTimeout, commandType);
            }
        }

        //do query and return a list
        public  IList<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null,
            bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) 
        {
            using (Connection)
            {
                return Connection.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType).ToList();
            }
        }

        //do query and return the first entity
        public  T QueryFirstOrDefault<T>(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null) 
        {
            using (Connection)
            {
                return Connection.QueryFirstOrDefault<T>(sql, param, transaction, commandTimeout, commandType);
            }
        }
    }
}