using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

namespace MediHub.Infrastructure.Data
{
    public class SqlConnectionFactory
    {
        
        public async Task<SqlConnection> GetOpenConnectionAsync()
        {
            

            // Build connection
            var conn = new SqlConnection(
                "Server=tcp:s-cdhb.database.windows.net,1433;Initial Catalog=TheatreSchedule;Encrypt=True;TrustServerCertificate=True;Connection Timeout=60;Authentication=Active Directory Default;"
            );

            await conn.OpenAsync();
            return conn;
        }
    }
}
