using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class StaffRepository : BaseRepository, IStaffRepository
    {
        public StaffRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<IEnumerable<Staff>> GetAll()
        {
            const string sql = @"
                SELECT 
                    id,
                    first_name AS FirstName,
                    last_name AS LastName,
                    code,
                    email
                FROM dbo.staff";

            return await QueryAsync<Staff>(sql);
        }


        public async Task<Staff?> GetById(int id)
        {
            const string sql = @"
                SELECT 
                    id,
                    first_name AS FirstName,
                    last_name AS LastName,
                    code,
                    email
                FROM dbo.staff
                WHERE id = @id";

            return await QuerySingleOrDefaultAsync<Staff>(
                sql,
                new { id }
            );
        }

        public async Task<int> Create(Staff s)
        {
            const string sql = @"
                INSERT INTO dbo.staff (first_name, last_name, code, email)
                VALUES (@FirstName, @LastName, @Code, @Email)";
            return await ExecuteAsync(sql, s);
        }


        public async Task<int> Update(Staff s)
        {
            const string sql = @"
                UPDATE dbo.staff
                SET first_name = @FirstName,
                    last_name = @LastName,
                    code = @Code,
                    email = @Email
                WHERE id = @Id";
            return await ExecuteAsync(sql, s);
        }


        public async Task<int> Delete(int id)
        {
            const string sql = "DELETE FROM dbo.staff WHERE id = @id";
            return await ExecuteAsync(sql, new { id });
        }

        public async Task<Staff?> GetByEmail(string email)
        {
            const string sql = @"
                SELECT TOP (1)
                    id,
                    first_name AS FirstName,
                    last_name AS LastName,
                    code,
                    email
                FROM dbo.staff
                WHERE email = @Email
                ORDER BY id";

            return await QuerySingleOrDefaultAsync<Staff>(
                sql,
                new { Email = email }
            );
        }

        
    }
}
