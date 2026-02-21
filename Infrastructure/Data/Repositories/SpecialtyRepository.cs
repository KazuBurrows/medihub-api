using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class SpecialtyRepository : BaseRepository, ISpecialtyRepository
    {
        public SpecialtyRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<IEnumerable<Specialty>> GetAll()
        {
            const string sql = @"
        SELECT
            SPECIALTY_KEY AS Id,
            SPECIALTY_CODE AS Code,
            SPECIALTY_DESCRIPTION AS Description
        FROM dbo.specialty";

            return await QueryAsync<Specialty>(sql);
        }



        public async Task<Specialty?> GetById(int id)
        {
            const string sql = @"
        SELECT
            SPECIALTY_KEY AS Id,
            SPECIALTY_CODE AS Code,
            SPECIALTY_DESCRIPTION AS Description
        FROM dbo.specialty
        WHERE SPECIALTY_KEY = @Id";

            return await QuerySingleOrDefaultAsync<Specialty>(sql, new { Id = id });
        }


        public async Task<int> Create(Specialty s)
        {
            const string sql = @"
        INSERT INTO dbo.specialty (SPECIALTY_CODE, SPECIALTY_DESCRIPTION)
        OUTPUT INSERTED.SPECIALTY_KEY
        VALUES (@Code, @Description)";

            return await ExecuteScalarAsync<int>(sql, s);
        }



        public async Task<int> Update(Specialty s)
        {
            const string sql = @"
        UPDATE dbo.specialty
        SET
            SPECIALTY_CODE = @Code,
            SPECIALTY_DESCRIPTION = @Description
        WHERE SPECIALTY_KEY = @Id";

            return await ExecuteAsync(sql, s);
        }



        public async Task<int> Delete(int id)
        {
            const string sql = @"
        DELETE FROM dbo.specialty
        WHERE SPECIALTY_KEY = @Id";

            return await ExecuteAsync(sql, new { Id = id });
        }

    }
}
