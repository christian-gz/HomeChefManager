using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Dapper;
using HomeChefManager.Configuration;

namespace HomeChefManager.DataAccess;

public class GenericRepository<T> where T : class
{
    protected GenericRepository(IConfig config)
    {
        _connectionString = config.ConnectionStrings.Default;
    }

    private readonly string _connectionString;

    protected IDbConnection GetConnection() => new SQLiteConnection(_connectionString);

    protected IEnumerable<T> Query(string sql, object? param = null)
    {
        using (var connection = GetConnection())
        {
            return connection.Query<T>(sql, param);
        }
    }

    protected int Add(string sql, T entity)
    {
        using (var connection = GetConnection())
        {
            return connection.QuerySingle<int>(sql, entity);
        }
    }

    protected void Remove(string sql, object param)
    {
        using (var connection = GetConnection())
        {
            connection.Execute(sql, param);
        }
    }

    protected void Update(string sql, T entity)
    {
        using (var connection = GetConnection())
        {
            connection.Execute(sql, entity);
        }
    }
}