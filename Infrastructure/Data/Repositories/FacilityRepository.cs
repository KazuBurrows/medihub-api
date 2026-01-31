using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class FacilityRepository : BaseRepository, IFacilityRepository
    {
        public FacilityRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<IEnumerable<Facility>> GetAll()
        {
            return await QueryAsync<Facility>("SELECT * FROM dbo.facility");
        }


        public async Task<Facility?> GetById(int id)
        {
            return await QuerySingleOrDefaultAsync<Facility>(
                "SELECT * FROM dbo.facility WHERE id = @id",
                new { id }
            );
        }

        public async Task<int> Create(Facility f)
        {
            const string sql = @"
                INSERT INTO dbo.facility (name, location)
                VALUES (@Name, @Location)";
            return await ExecuteAsync(sql, f);
        }


        public async Task<int> Update(Facility f)
        {
            const string sql = @"
                UPDATE dbo.facility
                SET name = @Name,
                    location = @Location
                WHERE id = @Id";
            return await ExecuteAsync(sql, f);
        }


        public async Task<int> Delete(int id)
        {
            const string sql = "DELETE FROM dbo.facility WHERE id = @id";
            return await ExecuteAsync(sql, new { id });
        }
    }
}
