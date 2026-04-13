using MediHub.Common.Exceptions.Infrastructure;
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



        public async Task Delete(int id)
        {
            const string sql = @"
                DELETE FROM dbo.specialty
                WHERE SPECIALTY_KEY = @Id";

            var rowsAffected = await ExecuteAsync(sql, new { Id = id });
            if (rowsAffected == 0)
            {
                throw new NotFoundException($"No item found with ID {id}.");
            }
        }

        public async Task<IEnumerable<SpecialtyDTO>> GetAllDTO()
        {
            const string sql = @"
                SELECT
                    s.SPECIALTY_KEY AS Id,
                    s.SPECIALTY_CODE AS Code,
                    s.SPECIALTY_DESCRIPTION AS Description,
                    COUNT(sess.SESSION_KEY) AS SessionCount
                FROM dbo.specialty s
                LEFT JOIN dbo.session sess
                    ON sess.SESSION_SPECIALTY_KEY = s.SPECIALTY_KEY
                GROUP BY
                    s.SPECIALTY_KEY,
                    s.SPECIALTY_CODE,
                    s.SPECIALTY_DESCRIPTION
                ORDER BY
                    s.SPECIALTY_DESCRIPTION
            ";

            return await QueryAsync<SpecialtyDTO>(sql);
        }

        public async Task<IEnumerable<Subspecialty>> GetSubspecialtiesBySpecialty(int id)
        {
            const string sql = @"
                SELECT
                    s.SUBSPECIALTY_KEY AS Id,
                    s.SUBSPECIALTY_NAME AS Name
                FROM dbo.subspecialty s
                INNER JOIN dbo.specialty_subspecialty ss
                    ON ss.SUBSPECIALTY_KEY = s.SUBSPECIALTY_KEY
                WHERE ss.SPECIALTY_KEY = @Id";

            return await QueryAsync<Subspecialty>(sql, new { Id = id });
        }
    }
}
