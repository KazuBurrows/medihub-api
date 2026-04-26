using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class EquipmentRepository : BaseRepository, IEquipmentRepository
    {
        public EquipmentRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<IEnumerable<Equipment>> GetAll()
        {
            const string sql = @"
                SELECT 
                    EQUIPMENT_KEY AS Id,
                    EQUIPMENT_NAME AS Name
                FROM dbo.equipment";

            return await QueryAsync<Equipment>(sql);
        }



        public async Task<Equipment?> GetById(int id)
        {
            const string sql = @"
                SELECT 
                    EQUIPMENT_KEY AS Id,
                    EQUIPMENT_NAME AS Name
                FROM dbo.equipment
                WHERE EQUIPMENT_KEY = @Id";

            return await QuerySingleOrDefaultAsync<Equipment>(
                sql,
                new { Id = id }
            );
        }


        public async Task<Equipment> Create(Equipment e)
        {
            const string sql = @"
                INSERT INTO dbo.equipment (EQUIPMENT_NAME)
                OUTPUT INSERTED.EQUIPMENT_KEY
                VALUES (@Name)";

            var id = await ExecuteScalarAsync<int>(sql, e);
            return await GetById(id);
        }



        public async Task<Equipment> Update(Equipment e)
        {
            const string sql = @"
                UPDATE dbo.equipment
                SET EQUIPMENT_NAME = @Name
                WHERE EQUIPMENT_KEY = @Id";

            await ExecuteAsync(sql, e);
            return await GetById(e.Id);
        }



        public async Task Delete(int id)
        {
            const string sql = @"
                DELETE FROM dbo.equipment
                WHERE EQUIPMENT_KEY = @Id";

            var rowsAffected = await ExecuteAsync(sql, new { Id = id });

            if (rowsAffected == 0)
            {
                throw new NotFoundException($"No item found with ID {id}.");
            }
        }

    }
}
