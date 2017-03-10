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
        public  IDbConnection OpenConnection()
        {
            var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            return conn;
        }

        //execute 
        public  int Execute(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var conn = OpenConnection())
            {
                return conn.Execute(sql, param, transaction, commandTimeout, commandType);
            }
        }

        //execute 
        public  object ExecuteScalar(string cmd, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var conn = OpenConnection())
            {
                return conn.ExecuteScalar(cmd, param, transaction, commandTimeout, commandType);
            }
        }

        //do query and return a list
        public  IList<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null,
            bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) 
        {
            using (var conn = OpenConnection())
            {
                return conn.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType).ToList();
            }
        }

        //do query and return the first entity
        public  T QueryFirstOrDefault<T>(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null) 
        {
            using (var conn = OpenConnection())
            {
                return conn.QueryFirstOrDefault<T>(sql, param, transaction, commandTimeout, commandType);
            }
        }
    }
}