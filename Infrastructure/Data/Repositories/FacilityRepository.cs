using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class FacilityRepository : BaseRepository, IFacilityRepository
    {
        public FacilityRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<IEnumerable<Facility>> GetAll()
        {
            const string sql = @"
                SELECT
                    FACILITY_KEY               AS Id,
                    FACILITY_CODE              AS Code,
                    FACILITY_NAME              AS Name,
                    FACILITY_TYPE_CODE         AS TypeCode,
                    FACILITY_TYPE_DESCRIPTION  AS TypeDescription,
                    FACILITY_DHB_CODE          AS DhbCode,
                    FACILITY_DHB_NAME          AS DhbName
                FROM dbo.facility";

            return await QueryAsync<Facility>(sql);
        }



        public async Task<Facility?> GetById(int id)
        {
            const string sql = @"
                SELECT
                    FACILITY_KEY               AS Id,
                    FACILITY_CODE              AS Code,
                    FACILITY_NAME              AS Name,
                    FACILITY_TYPE_CODE         AS TypeCode,
                    FACILITY_TYPE_DESCRIPTION  AS TypeDescription,
                    FACILITY_DHB_CODE          AS DhbCode,
                    FACILITY_DHB_NAME          AS DhbName
                FROM dbo.facility
                WHERE FACILITY_KEY = @Id";

            return await QuerySingleOrDefaultAsync<Facility>(
                sql,
                new { Id = id }
            );
        }


        public async Task<Facility> Create(Facility f)
        {
            const string sql = @"
                INSERT INTO dbo.facility (
                    FACILITY_CODE,
                    FACILITY_NAME,
                    FACILITY_TYPE_CODE,
                    FACILITY_TYPE_DESCRIPTION,
                    FACILITY_DHB_CODE,
                    FACILITY_DHB_NAME
                )
                OUTPUT INSERTED.FACILITY_KEY
                VALUES (
                    @Code,
                    @Name,
                    @TypeCode,
                    @TypeDescription,
                    @DhbCode,
                    @DhbName
                )";

            var id = await ExecuteScalarAsync<int>(sql, f);
            return await GetById(id);
        }



        public async Task<Facility> Update(Facility f)
        {
            const string sql = @"
                UPDATE dbo.facility
                SET
                    FACILITY_CODE = @Code,
                    FACILITY_NAME = @Name,
                    FACILITY_TYPE_CODE = @TypeCode,
                    FACILITY_TYPE_DESCRIPTION = @TypeDescription,
                    FACILITY_DHB_CODE = @DhbCode,
                    FACILITY_DHB_NAME = @DhbName
                WHERE FACILITY_KEY = @Id";

            await ExecuteAsync(sql, f);
            return await GetById(f.Id);
        }



        public async Task Delete(int id)
        {
            const string sql = @"
                DELETE FROM dbo.facility
                WHERE FACILITY_KEY = @Id";

            var rowsAffected = await ExecuteAsync(sql, new { Id = id });

            if (rowsAffected == 0)
            {
                throw new NotFoundException($"No item found with ID {id}.");
            }
        }

    }
}
