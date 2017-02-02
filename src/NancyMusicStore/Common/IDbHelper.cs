using System.Collections.Generic;
using System.Data;

namespace NancyMusicStore.Common
{
    public interface IDbHelper
    {
        int Execute(string sql, object param = null, IDbTransaction transaction = null,
           int? commandTimeout = null, CommandType? commandType = null);
        object ExecuteScalar(string cmd, object param = null, IDbTransaction transaction = null,
        int? commandTimeout = null, CommandType? commandType = null);
        IList<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null,
        bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
        T QueryFirstOrDefault<T>(string sql, object param = null, IDbTransaction transaction = null,
        int? commandTimeout = null, CommandType? commandType = null);


    }
}