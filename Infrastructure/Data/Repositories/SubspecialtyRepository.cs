using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class SubspecialtyRepository : BaseRepository, ISubspecialtyRepository
    {
        public SubspecialtyRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<IEnumerable<Subspecialty>> GetAll()
        {
            const string sql = @"
                SELECT 
                    id,
                    code,
                    name AS Name
                FROM dbo.subspecialty";

            return await QueryAsync<Subspecialty>(sql);
        }


        public async Task<Subspecialty?> GetById(int id)
        {
            const string sql = @"
                SELECT 
                    id,
                    code,
                    name AS Name
                FROM dbo.subspecialty
                WHERE id = @id";

            return await QuerySingleOrDefaultAsync<Subspecialty>(
                sql,
                new { id }
            );
        }

        public async Task<int> Create(Subspecialty s)
        {
            const string sql = @"
                INSERT INTO dbo.subspecialty (code, name)
                VALUES (@Code, @Name)";
            return await ExecuteAsync(sql, s);
        }


        public async Task<int> Update(Subspecialty s)
        {
            const string sql = @"
                UPDATE dbo.subspecialty
                SET code = @Code,
                    name = @Name
                WHERE id = @Id";
            return await ExecuteAsync(sql, s);
        }


        public async Task<int> Delete(int id)
        {
            const string sql = "DELETE FROM dbo.subspecialty WHERE id = @id";
            return await ExecuteAsync(sql, new { id });
        }
    }
}
