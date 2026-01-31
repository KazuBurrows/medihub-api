using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class SpecialtyRepository : BaseRepository, ISpecialtyRepository
    {
        public SpecialtyRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<IEnumerable<Specialty>> GetAll()
        {
            return await QueryAsync<Specialty>("SELECT * FROM dbo.specialty");
        }


        public async Task<Specialty?> GetById(int id)
        {
            return await QuerySingleOrDefaultAsync<Specialty>(
                "SELECT * FROM dbo.specialty WHERE id = @id",
                new { id }
            );
        }

        public async Task<int> Create(Specialty s)
        {
            const string sql = @"
                INSERT INTO dbo.specialty (name)
                VALUES (@Name)";
            return await ExecuteAsync(sql, s);
        }


        public async Task<int> Update(Specialty s)
        {
            const string sql = @"
                UPDATE dbo.specialty
                SET name = @Name
                WHERE id = @Id";
            return await ExecuteAsync(sql, s);
        }


        public async Task<int> Delete(int id)
        {
            const string sql = "DELETE FROM dbo.specialty WHERE id = @id";
            return await ExecuteAsync(sql, new { id });
        }
    }
}
