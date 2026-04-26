using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class AnaestheticTypeRepository : BaseRepository, IAnaestheticTypeRepository
    {
        public AnaestheticTypeRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<IEnumerable<AnaestheticType>> GetAll()
        {
            const string sql = @"
                SELECT
                    ANAESTHETIC_TYPE_KEY AS Id,
                    ANAESTHETIC_TYPE_CODE AS Code,
                    ANAESTHETIC_TYPE_DESCRIPTION AS Description
                FROM dbo.anaesthetic_type";

            return await QueryAsync<AnaestheticType>(sql);
        }



        public async Task<AnaestheticType?> GetById(int id)
        {
            const string sql = @"
                SELECT
                    ANAESTHETIC_TYPE_KEY AS Id,
                    ANAESTHETIC_TYPE_CODE AS Code,
                    ANAESTHETIC_TYPE_DESCRIPTION AS Description
                FROM dbo.anaesthetic_type
                WHERE ANAESTHETIC_TYPE_KEY = @Id";

            return await QuerySingleOrDefaultAsync<AnaestheticType>(sql, new { Id = id });
        }


        public async Task<AnaestheticType> Create(AnaestheticType s)
        {
            const string sql = @"
                INSERT INTO dbo.anaesthetic_type (ANAESTHETIC_TYPE_CODE, ANAESTHETIC_TYPE_DESCRIPTION)
                OUTPUT INSERTED.ANAESTHETIC_TYPE_KEY
                VALUES (@Code, @Description)";

            var id = await ExecuteScalarAsync<int>(sql, s);

            return await GetById(id);
        }



        public async Task<AnaestheticType> Update(AnaestheticType s)
        {
            const string sql = @"
                UPDATE dbo.anaesthetic_type
                SET ANAESTHETIC_TYPE_CODE = @Code, ANAESTHETIC_TYPE_DESCRIPTION = @Description
                WHERE ANAESTHETIC_TYPE_KEY = @Id";

            await ExecuteAsync(sql, s);
            return await GetById(s.Id);
        }



        public async Task Delete(int id)
        {
            const string sql = @"
                DELETE FROM dbo.anaesthetic_type
                WHERE ANAESTHETIC_TYPE_KEY = @Id";

           var rowsAffected = await ExecuteAsync(sql, new { Id = id });
            if (rowsAffected == 0)
            {
                throw new NotFoundException($"No item found with ID {id}.");
            }
        }

    }
}
