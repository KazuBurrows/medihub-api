using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;
using Version = MediHub.Domain.Models.Version;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class VersionRepository : BaseRepository, IVersionRepository
    {
        public VersionRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<IEnumerable<Version>> GetAll()
        {
            const string sql = @"
                SELECT
                    VERSION_KEY AS Id,
                    VERSION_NAME AS Name,
                    VERSION_DESCRIPTION AS Description,
                    VERSION_IS_ACTIVE AS IsActive
                FROM dbo.version";

            return await QueryAsync<Version>(sql);
        }

        public async Task<Version?> GetById(int id)
        {
            const string sql = @"
                SELECT
                    VERSION_KEY AS Id,
                    VERSION_NAME AS Name,
                    VERSION_DESCRIPTION AS Description,
                    VERSION_IS_ACTIVE AS IsActive
                FROM dbo.version
                WHERE VERSION_KEY = @Id";

            return await QuerySingleOrDefaultAsync<Version>(sql, new { Id = id });
        }

        public async Task<Version> Create(Version v)
        {
            const string sql = @"
                INSERT INTO dbo.version (VERSION_NAME, VERSION_DESCRIPTION, VERSION_IS_ACTIVE)
                OUTPUT INSERTED.VERSION_KEY
                VALUES (@Name, @Description, @IsActive)";

            var id = await ExecuteScalarAsync<int>(sql, v);
            return await GetById(id);
        }

        public async Task<Version> Update(Version v)
        {
            const string sql = @"
                UPDATE dbo.version
                SET VERSION_NAME = @Name,
                    VERSION_DESCRIPTION = @Description,
                    VERSION_IS_ACTIVE = @IsActive
                WHERE VERSION_KEY = @Id";

            await ExecuteAsync(sql, v);
            return await GetById(v.Id);
        }

        public async Task Delete(int id)
        {
            const string sql = @"
                DELETE FROM dbo.version
                WHERE VERSION_KEY = @Id";

            var rowsAffected = await ExecuteAsync(sql, new { Id = id });
            if (rowsAffected == 0)
            {
                throw new NotFoundException($"No item found with ID {id}.");
            }
        }
    }
}