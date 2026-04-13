using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class SurgeonTypeRepository : BaseRepository, ISurgeonTypeRepository
    {
        public SurgeonTypeRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<IEnumerable<SurgeonType>> GetAll()
        {
            const string sql = @"
                SELECT
                    SURGEON_TYPE_KEY AS Id,
                    SURGEON_TYPE_CODE AS Code,
                    SURGEON_TYPE_DESCRIPTION AS Description
                FROM dbo.surgeon_type";

            return await QueryAsync<SurgeonType>(sql);
        }



        public async Task<SurgeonType?> GetById(int id)
        {
            const string sql = @"
                SELECT
                    SURGEON_TYPE_KEY AS Id,
                    SURGEON_TYPE_CODE AS Code,
                    SURGEON_TYPE_DESCRIPTION AS Description
                FROM dbo.surgeon_type
                WHERE SURGEON_TYPE_KEY = @Id";

            return await QuerySingleOrDefaultAsync<SurgeonType>(sql, new { Id = id });
        }


        public async Task<int> Create(SurgeonType s)
        {
            const string sql = @"
                INSERT INTO dbo.surgeon_type (SURGEON_TYPE_CODE, SURGEON_TYPE_DESCRIPTION)
                OUTPUT INSERTED.SURGEON_TYPE_KEY
                VALUES (@Code, @Description)";

            return await ExecuteScalarAsync<int>(sql, s);
        }



        public async Task<int> Update(SurgeonType s)
        {
            const string sql = @"
                UPDATE dbo.surgeon_type
                SET SURGEON_TYPE_CODE = @Code, SURGEON_TYPE_DESCRIPTION = @Description
                WHERE SURGEON_TYPE_KEY = @Id";

            return await ExecuteAsync(sql, s);
        }



        public async Task Delete(int id)
        {
            const string sql = @"
                DELETE FROM dbo.surgeon_type
                WHERE SURGEON_TYPE_KEY = @Id";

           var rowsAffected = await ExecuteAsync(sql, new { Id = id });
            if (rowsAffected == 0)
            {
                throw new NotFoundException($"No item found with ID {id}.");
            }
        }

    }
}
