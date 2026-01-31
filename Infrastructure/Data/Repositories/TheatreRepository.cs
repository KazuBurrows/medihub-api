using MediHub.Domain.DTOs;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;
using MediHub.Infrastructure.Data.Utils;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class TheatreRepository : BaseRepository, ITheatreRepository
    {
        public TheatreRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<IEnumerable<Theatre>> GetAll()
        {
            const string sql = @"
                SELECT 
                    id,
                    name,
                    facility_id AS FacilityId,
                    scope_theatre_code AS ScopeTheatreCode,
                    pediatric
                FROM dbo.theatre";

            return await QueryAsync<Theatre>(sql);
        }


        public async Task<Theatre?> GetById(int id)
        {
            const string sql = @"
                SELECT 
                    id,
                    name,
                    facility_id AS FacilityId,
                    scope_theatre_code AS ScopeTheatreCode,
                    pediatric
                FROM dbo.theatre
                WHERE id = @id";

            return await QuerySingleOrDefaultAsync<Theatre>(
                sql,
                new { id }
            );
        }

        public async Task<int> Create(Theatre t)
        {
            const string sql = @"
                INSERT INTO dbo.theatre (name, facility_id, scope_theatre_code, pediatric)
                VALUES (@Name, @FacilityId, @ScopeTheatreCode, @Pediatric)";
            return await ExecuteAsync(sql, t);
        }


        public async Task<int> Update(Theatre t)
        {
            const string sql = @"
                UPDATE dbo.theatre
                SET name = @Name,
                    facility_id = @FacilityId,
                    scope_theatre_code = @ScopeTheatreCode,
                    pediatric = @Pediatric
                WHERE id = @Id";
            return await ExecuteAsync(sql, t);
        }


        public async Task<int> Delete(int id)
        {
            const string sql = "DELETE FROM dbo.theatre WHERE id = @id";
            return await ExecuteAsync(sql, new { id });
        }


        public async Task<IEnumerable<TheatreAggregate>> GetAllAgg()
        {
            var theatres = (await QueryAsync<TheatreAggregate>(@"SELECT id, name, facility_id AS FacilityId, scope_theatre_code AS ScopeTheatreCode, pediatric FROM dbo.theatre")).ToList();
            if (!theatres.Any()) return theatres;

            var theatreIds = theatres.Select(t => t.Id).ToList();

            var theatreEquipment = await QueryAsync<(int TheatreId, int EquipmentId)>("SELECT theatre_id AS TheatreId, equipment_id AS EquipmentId FROM dbo.theatre_equipment WHERE theatre_id IN @TheatreIds", new { TheatreIds = theatreIds });

            AggregateHelper.MapJunction(theatres, theatreEquipment, t => t.Id, t => t.EquipmentIds);

            return theatres;
        }

        public async Task<int> CreateAgg(TheatreAggregate t)
        {
            const string sql = @"
                INSERT INTO dbo.theatre (name, facility_id, scope_theatre_code, pediatric)
                OUTPUT INSERTED.id
                VALUES (@Name, @FacilityId, @ScopeTheatreCode, @Pediatric)";

            var theatreId = await ExecuteScalarAsync<int>(sql, t);


            // 2️⃣ Insert junction table rows if any
            if (t.EquipmentIds.Any())
            {
                const string equipmentSql = @"
                    INSERT INTO dbo.theatre_equipment (theatre_id, equipment_id)
                    VALUES (@TheatreId, @EquipmentId)";

                foreach (var eqId in t.EquipmentIds)
                {
                    await ExecuteAsync(equipmentSql, new { TheatreId = theatreId, EquipmentId = eqId });
                }
            }

            return theatreId;
        }

        public async Task<TheatreAggregate?> GetByIdAgg(int id)
        {
            const string sql = @"
                SELECT 
                    id,
                    name,
                    facility_id AS FacilityId,
                    scope_theatre_code AS ScopeTheatreCode,
                    pediatric
                FROM dbo.theatre
                WHERE id = @Id";

            var theatre = await QuerySingleOrDefaultAsync<TheatreAggregate>(sql, new { Id = id });
            if (theatre == null) return null;

            // Load junction tables
            theatre.EquipmentIds = (await QueryAsync<int>(
                "SELECT equipment_id FROM dbo.theatre_equipment WHERE theatre_id = @Id",
                new { Id = id }
            )).ToList();

            return theatre;
        }

        public async Task<int> UpdateAgg(TheatreAggregate agg)
        {
            const string sql = @"
                UPDATE dbo.theatre
                SET name = @Name,
                    facility_id = @FacilityId,
                    scope_theatre_code = @ScopeTheatreCode,
                    pediatric = @Pediatric
                WHERE id = @Id";

            var rows = await ExecuteAsync(sql, agg);

            // Update junction tables
            await ExecuteAsync("DELETE FROM dbo.theatre_equipment WHERE theatre_id = @Id", new { Id = agg.Id });
            foreach (var eqId in agg.EquipmentIds)
            {
                await ExecuteAsync(
                    "INSERT INTO dbo.theatre_equipment (theatre_id, equipment_id) VALUES (@Id, @EquipmentId)",
                    new { Id = agg.Id, EquipmentId = eqId }
                );
            }

            return rows;
        }

        public async Task<int> DeleteAgg(int id)
        {
            // Delete junction table rows first
            await ExecuteAsync("DELETE FROM dbo.theatre_equipment WHERE theatre_id = @Id", new { Id = id });

            // Delete the theatre itself
            return await ExecuteAsync("DELETE FROM dbo.theatre WHERE id = @Id", new { Id = id });
        }

        public async Task<IEnumerable<TheatreDTO>> GetAllDTO()
        {
            // Main SQL: theatre info + facility name
            const string sql = @"
                SELECT 
                    t.id AS Id,
                    t.name AS TheatreName,
                    t.scope_theatre_code AS ScopeTheatreCode,
                    t.pediatric AS Pediatric,
                    f.name AS FacilityName
                FROM dbo.theatre t
                LEFT JOIN dbo.facility f ON t.facility_id = f.id
                ORDER BY t.name;
            ";

            var theatres = (await QueryAsync<TheatreDTO>(sql, new { })).ToList();

            if (!theatres.Any())
                return theatres;

            // Get all theatre equipment
            const string equipmentSql = @"
                SELECT 
                    te.theatre_id AS TheatreId,
                    e.name AS EquipmentName
                FROM dbo.theatre_equipment te
                INNER JOIN dbo.equipment e ON te.equipment_id = e.id;
            ";

            var allEquipment = await QueryAsync<(int TheatreId, string EquipmentName)>(equipmentSql);

            // Map equipment names to their theatre
            foreach (var theatre in theatres)
            {
                theatre.EquipmentNames = allEquipment
                    .Where(e => e.TheatreId == theatre.Id)
                    .Select(e => e.EquipmentName)
                    .ToArray();
            }

            return theatres;
        }

        public async Task<TheatreDTO> GetByIdDTO(int id)
        {
            // Main SQL: theatre info + facility name
            const string sql = @"
                SELECT 
                    t.id AS Id,
                    t.name AS TheatreName,
                    t.scope_theatre_code AS ScopeTheatreCode,
                    t.pediatric AS Pediatric,
                    f.name AS FacilityName
                FROM dbo.theatre t
                LEFT JOIN dbo.facility f ON t.facility_id = f.id
                WHERE t.id = @Id;
            ";

            var theatre = (await QueryAsync<TheatreDTO>(sql, new { Id = id }))
                        .FirstOrDefault();

            if (theatre == null)
                return null!; // or throw an exception if preferred

            // Get equipment for this theatre
            const string equipmentSql = @"
                SELECT 
                    te.theatre_id AS TheatreId,
                    e.name AS EquipmentName
                FROM dbo.theatre_equipment te
                INNER JOIN dbo.equipment e ON te.equipment_id = e.id
                WHERE te.theatre_id = @Id;
            ";

            var equipment = await QueryAsync<(int TheatreId, string EquipmentName)>(equipmentSql, new { Id = id });

            theatre.EquipmentNames = equipment.Select(e => e.EquipmentName).ToArray();

            return theatre;
        }

    }
}
