using MediHub.Common.Exceptions.Infrastructure;
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
                    ROLE_KEY AS Id,
                    ROLE_NAME AS Name
                FROM dbo.role";

            return await QueryAsync<Role>(sql);
        }



        public async Task<Role?> GetById(int id)
        {
            const string sql = @"
                SELECT 
                    ROLE_KEY AS Id,
                    ROLE_NAME AS Name
                FROM dbo.role
                WHERE ROLE_KEY = @Id";

            return await QuerySingleOrDefaultAsync<Role>(
                sql,
                new { Id = id }
            );
        }


        public async Task<int> Create(Role r)
        {
            const string sql = @"
                INSERT INTO dbo.role (ROLE_NAME)
                OUTPUT INSERTED.ROLE_KEY
                VALUES (@Name)";

            return await ExecuteScalarAsync<int>(sql, r);
        }



        public async Task<int> Update(Role r)
        {
            const string sql = @"
                UPDATE dbo.role
                SET ROLE_NAME = @Name
                WHERE ROLE_KEY = @Id";

            return await ExecuteAsync(sql, r);
        }



        public async Task Delete(int id)
        {
            const string sql = @"
                DELETE FROM dbo.role
                WHERE ROLE_KEY = @Id";

            var rowsAffected = await ExecuteAsync(sql, new { Id = id });
            if (rowsAffected == 0)
            {
                throw new NotFoundException($"No item found with ID {id}.");
            }
        }


    }
}
