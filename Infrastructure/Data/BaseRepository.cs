using System.Data;
using Dapper;

namespace MediHub.Infrastructure.Data.Repositories
{
    public abstract class BaseRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        protected BaseRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /**
        *** Use when: You expect zero, one, or many rows from a SELECT query.
        **/
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null)
        {
            using var conn = await _connectionFactory.GetOpenConnectionAsync();
            return await conn.QueryAsync<T>(sql, parameters);
        }


        /**
        *** Use when: You expect at most one row from a SELECT query.
        **/
        protected async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null)
        {
            await using var conn = await _connectionFactory.GetOpenConnectionAsync();
            return await conn.QuerySingleOrDefaultAsync<T>(sql, parameters);
        }

        /**
        *** Use when: You are running a non-query SQL command like INSERT, UPDATE, or DELETE.
        **/
        protected async Task<int> ExecuteAsync(string sql, object? parameters = null)
        {
            await using var conn = await _connectionFactory.GetOpenConnectionAsync();
            return await conn.ExecuteAsync(sql, parameters);
        }


        /// <summary>
        /// Executes a SQL query that returns a single value (like OUTPUT INSERTED.id)
        /// </summary>
        protected async Task<T> ExecuteScalarAsync<T>(string sql, object? parameters = null)
        {
            await using var conn = await _connectionFactory.GetOpenConnectionAsync();
            return await conn.ExecuteScalarAsync<T>(sql, parameters);
        }
    }
}
