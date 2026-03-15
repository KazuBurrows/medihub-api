using MediHub.Common.Exceptions.Infrastructure;
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
            SUBSPECIALTY_KEY AS Id,
            SUBSPECIALTY_NAME AS Name
        FROM dbo.subspecialty";

            return await QueryAsync<Subspecialty>(sql);
        }



        public async Task<Subspecialty?> GetById(int id)
        {
            const string sql = @"
                SELECT
                    SUBSPECIALTY_KEY AS Id,
                    SUBSPECIALTY_NAME AS Name
                FROM dbo.subspecialty
                WHERE SUBSPECIALTY_KEY = @Id";

            return await QuerySingleOrDefaultAsync<Subspecialty>(sql, new { Id = id });
        }


        public async Task<int> Create(Subspecialty s)
        {
            const string sql = @"
        INSERT INTO dbo.subspecialty (SUBSPECIALTY_NAME)
        OUTPUT INSERTED.SUBSPECIALTY_KEY
        VALUES (@Name)";

            return await ExecuteScalarAsync<int>(sql, s);
        }



        public async Task<int> Update(Subspecialty s)
        {
            const string sql = @"
        UPDATE dbo.subspecialty
        SET SUBSPECIALTY_NAME = @Name
        WHERE SUBSPECIALTY_KEY = @Id";

            return await ExecuteAsync(sql, s);
        }



        public async Task Delete(int id)
        {
            const string sql = @"
                DELETE FROM dbo.subspecialty
                WHERE SUBSPECIALTY_KEY = @Id";

           var rowsAffected = await ExecuteAsync(sql, new { Id = id });
            if (rowsAffected == 0)
            {
                throw new NotFoundException($"No item found with ID {id}.");
            }
        }

    }
}
