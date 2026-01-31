using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class EquipmentRepository : BaseRepository, IEquipmentRepository
    {
        public EquipmentRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<IEnumerable<Equipment>> GetAll()
        {
            return await QueryAsync<Equipment>("SELECT * FROM dbo.equipment");
        }


        public async Task<Equipment?> GetById(int id)
        {
            return await QuerySingleOrDefaultAsync<Equipment>(
                "SELECT * FROM dbo.equipment WHERE id = @id",
                new { id }
            );
        }

        public async Task<int> Create(Equipment e)
        {
            const string sql = @"
                INSERT INTO dbo.equipment (name)
                VALUES (@Name)";
            return await ExecuteAsync(sql, e);
        }


        public async Task<int> Update(Equipment e)
        {
            const string sql = @"
                UPDATE dbo.equipment
                SET name = @Name
                WHERE id = @Id";
            return await ExecuteAsync(sql, e);
        }


        public async Task<int> Delete(int id)
        {
            const string sql = "DELETE FROM dbo.equipment WHERE id = @id";
            return await ExecuteAsync(sql, new { id });
        }
    }
}
