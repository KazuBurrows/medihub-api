using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class RoleRepository : BaseRepository, IRoleRepository
    {
        public RoleRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<IEnumerable<Role>> GetAll()
        {
            const string sql = @"
                SELECT 
                    id,
                    name AS Name
                FROM dbo.role";

            return await QueryAsync<Role>(sql);
        }


        public async Task<Role?> GetById(int id)
        {
            const string sql = @"
                SELECT 
                    id,
                    name AS Name
                FROM dbo.role
                WHERE id = @id";

            return await QuerySingleOrDefaultAsync<Role>(
                sql,
                new { id }
            );
        }

        public async Task<int> Create(Role r)
        {
            const string sql = @"
                INSERT INTO dbo.role (name)
                VALUES (@Name)";
            return await ExecuteAsync(sql, r);
        }


        public async Task<int> Update(Role r)
        {
            const string sql = @"
                UPDATE dbo.role
                SET name = @Name
                WHERE id = @Id";
            return await ExecuteAsync(sql, r);
        }


        public async Task<int> Delete(int id)
        {
            const string sql = "DELETE FROM dbo.role WHERE id = @id";
            return await ExecuteAsync(sql, new { id });
        }
        
    }
}
