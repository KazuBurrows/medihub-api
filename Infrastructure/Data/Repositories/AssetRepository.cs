using MediHub.Domain.DTOs;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;
using MediHub.Infrastructure.Data.Utils;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class AssetRepository : BaseRepository, IAssetRepository
    {
        public AssetRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<IEnumerable<Asset>> GetAll()
        {
            const string sql = @"
                SELECT 
                    id,
                    name,
                    facility_id AS FacilityId,
                    scope_asset_code AS ScopeAssetCode,
                    pediatric
                FROM dbo.asset";

            return await QueryAsync<Asset>(sql);
        }


        public async Task<Asset?> GetById(int id)
        {
            const string sql = @"
                SELECT 
                    id,
                    name,
                    facility_id AS FacilityId,
                    scope_asset_code AS ScopeAssetCode,
                    pediatric
                FROM dbo.asset
                WHERE id = @id";

            return await QuerySingleOrDefaultAsync<Asset>(
                sql,
                new { id }
            );
        }

        public async Task<int> Create(Asset t)
        {
            const string sql = @"
                INSERT INTO dbo.asset (name, facility_id, scope_asset_code, pediatric)
                VALUES (@Name, @FacilityId, @ScopeAssetCode, @Pediatric)";
            return await ExecuteAsync(sql, t);
        }


        public async Task<int> Update(Asset t)
        {
            const string sql = @"
                UPDATE dbo.asset
                SET name = @Name,
                    facility_id = @FacilityId,
                    scope_asset_code = @ScopeAssetCode,
                    pediatric = @Pediatric
                WHERE id = @Id";
            return await ExecuteAsync(sql, t);
        }


        public async Task<int> Delete(int id)
        {
            const string sql = "DELETE FROM dbo.asset WHERE id = @id";
            return await ExecuteAsync(sql, new { id });
        }


        public async Task<IEnumerable<AssetAggregate>> GetAllAgg()
        {
            var assets = (await QueryAsync<AssetAggregate>(@"SELECT id, name, facility_id AS FacilityId, scope_asset_code AS ScopeAssetCode, pediatric FROM dbo.asset")).ToList();
            if (!assets.Any()) return assets;

            var assetIds = assets.Select(t => t.Id).ToList();

            var assetEquipment = await QueryAsync<(int AssetId, int EquipmentId)>("SELECT asset_id AS AssetId, equipment_id AS EquipmentId FROM dbo.asset_equipment WHERE asset_id IN @AssetIds", new { AssetIds = assetIds });

            AggregateHelper.MapJunction(assets, assetEquipment, t => t.Id, t => t.EquipmentIds);

            return assets;
        }

        public async Task<int> CreateAgg(AssetAggregate t)
        {
            const string sql = @"
                INSERT INTO dbo.asset (name, facility_id, scope_asset_code, pediatric)
                OUTPUT INSERTED.id
                VALUES (@Name, @FacilityId, @ScopeAssetCode, @Pediatric)";

            var assetId = await ExecuteScalarAsync<int>(sql, t);


            // 2️⃣ Insert junction table rows if any
            if (t.EquipmentIds.Any())
            {
                const string equipmentSql = @"
                    INSERT INTO dbo.asset_equipment (asset_id, equipment_id)
                    VALUES (@AssetId, @EquipmentId)";

                foreach (var eqId in t.EquipmentIds)
                {
                    await ExecuteAsync(equipmentSql, new { AssetId = assetId, EquipmentId = eqId });
                }
            }

            return assetId;
        }

        public async Task<AssetAggregate?> GetByIdAgg(int id)
        {
            const string sql = @"
                SELECT 
                    id,
                    name,
                    facility_id AS FacilityId,
                    scope_asset_code AS ScopeAssetCode,
                    pediatric
                FROM dbo.asset
                WHERE id = @Id";

            var asset = await QuerySingleOrDefaultAsync<AssetAggregate>(sql, new { Id = id });
            if (asset == null) return null;

            // Load junction tables
            asset.EquipmentIds = (await QueryAsync<int>(
                "SELECT equipment_id FROM dbo.asset_equipment WHERE asset_id = @Id",
                new { Id = id }
            )).ToList();

            return asset;
        }

        public async Task<int> UpdateAgg(AssetAggregate agg)
        {
            const string sql = @"
                UPDATE dbo.asset
                SET name = @Name,
                    facility_id = @FacilityId,
                    scope_asset_code = @ScopeAssetCode,
                    pediatric = @Pediatric
                WHERE id = @Id";

            var rows = await ExecuteAsync(sql, agg);

            // Update junction tables
            await ExecuteAsync("DELETE FROM dbo.asset_equipment WHERE asset_id = @Id", new { Id = agg.Id });
            foreach (var eqId in agg.EquipmentIds)
            {
                await ExecuteAsync(
                    "INSERT INTO dbo.asset_equipment (asset_id, equipment_id) VALUES (@Id, @EquipmentId)",
                    new { Id = agg.Id, EquipmentId = eqId }
                );
            }

            return rows;
        }

        public async Task<int> DeleteAgg(int id)
        {
            // Delete junction table rows first
            await ExecuteAsync("DELETE FROM dbo.asset_equipment WHERE asset_id = @Id", new { Id = id });

            // Delete the asset itself
            return await ExecuteAsync("DELETE FROM dbo.asset WHERE id = @Id", new { Id = id });
        }

        public async Task<IEnumerable<AssetDTO>> GetAllDTO()
        {
            // Main SQL: asset info + facility name
            const string sql = @"
                SELECT 
                    t.id AS Id,
                    t.name AS AssetName,
                    t.scope_asset_code AS ScopeAssetCode,
                    t.pediatric AS Pediatric,
                    f.name AS FacilityName
                FROM dbo.asset t
                LEFT JOIN dbo.facility f ON t.facility_id = f.id
                ORDER BY t.name;
            ";

            var assets = (await QueryAsync<AssetDTO>(sql, new { })).ToList();

            if (!assets.Any())
                return assets;

            // Get all asset equipment
            const string equipmentSql = @"
                SELECT 
                    te.asset_id AS AssetId,
                    e.name AS EquipmentName
                FROM dbo.asset_equipment te
                INNER JOIN dbo.equipment e ON te.equipment_id = e.id;
            ";

            var allEquipment = await QueryAsync<(int AssetId, string EquipmentName)>(equipmentSql);

            // Map equipment names to their asset
            foreach (var asset in assets)
            {
                asset.EquipmentNames = allEquipment
                    .Where(e => e.AssetId == asset.Id)
                    .Select(e => e.EquipmentName)
                    .ToArray();
            }

            return assets;
        }

        public async Task<AssetDTO> GetByIdDTO(int id)
        {
            // Main SQL: asset info + facility name
            const string sql = @"
                SELECT 
                    t.id AS Id,
                    t.name AS AssetName,
                    t.scope_asset_code AS ScopeAssetCode,
                    t.pediatric AS Pediatric,
                    f.name AS FacilityName
                FROM dbo.asset t
                LEFT JOIN dbo.facility f ON t.facility_id = f.id
                WHERE t.id = @Id;
            ";

            var asset = (await QueryAsync<AssetDTO>(sql, new { Id = id }))
                        .FirstOrDefault();

            if (asset == null)
                return null!; // or throw an exception if preferred

            // Get equipment for this asset
            const string equipmentSql = @"
                SELECT 
                    te.asset_id AS AssetId,
                    e.name AS EquipmentName
                FROM dbo.asset_equipment te
                INNER JOIN dbo.equipment e ON te.equipment_id = e.id
                WHERE te.asset_id = @Id;
            ";

            var equipment = await QueryAsync<(int AssetId, string EquipmentName)>(equipmentSql, new { Id = id });

            asset.EquipmentNames = equipment.Select(e => e.EquipmentName).ToArray();

            return asset;
        }

    }
}
