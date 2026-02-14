using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

namespace MediHub.Infrastructure.Data
{
    public class SqlConnectionFactory
    {
        
        public async Task<SqlConnection> GetOpenConnectionAsync()
        {
            var connectionString = "Server=s-cdhb.database.windows.net;Database=TheatreSchedule;User ID=testuser;Password=P@ssword123!;Encrypt=True;TrustServerCertificate=True;Connect Timeout=30;Max Pool Size=50;";


            // Build connection
            var conn = new SqlConnection(
                connectionString
            );



            await conn.OpenAsync();
            return conn;
        }
    }
}
