using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

namespace MediHub.Infrastructure.Data
{
    public class SqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration config)
        {
            // _connectionString =
            //     config["SqlConnectionString"]
            //     ?? throw new InvalidOperationException("SQL connection string missing.");
            _connectionString = "Server=s-cdhb.database.windows.net;Database=TheatreSchedule;User ID=testuser;Password=P@ssword123!;Encrypt=True;TrustServerCertificate=True;Connect Timeout=30;Max Pool Size=300;Min Pool Size=5;Pooling=true;";

            // Warm up connection pool on startup
            // WarmUp().GetAwaiter().GetResult();
        }

        public async Task<SqlConnection> GetOpenConnectionAsync()
        {
            var conn = new SqlConnection(_connectionString);

            await conn.OpenAsync();

            return conn;
        }

        private async Task WarmUp()
        {
            await using var conn = new SqlConnection(_connectionString);

            await conn.OpenAsync();
            await conn.CloseAsync();
        }
    }
}
