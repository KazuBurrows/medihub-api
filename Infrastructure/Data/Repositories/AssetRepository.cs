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
            ASSET_KEY              AS Id,
            ASSET_CODE             AS Code,
            ASSET_DESCRIPTION      AS Description,
            ASSET_TYPE_CODE        AS TypeCode,
            ASSET_LOCATION         AS Location,
            ASSET_FACILITY_KEY     AS FacilityId,
            ASSET_DISTRICT_OF_SERVICE AS DistrictOfService,
            ASSET_PRIMARY_SPECIALTY_KEY AS PrimarySpecialtyKey,
            ASSET_PAEDIATRIC       AS Paediatric,
            ASSET_DEDICATED_ACUTE  AS DedicatedAcute
        FROM dbo.asset";

            return await QueryAsync<Asset>(sql);
        }



        public async Task<Asset?> GetById(int id)
        {
            const string sql = @"
        SELECT 
            ASSET_KEY              AS Id,
            ASSET_CODE             AS Code,
            ASSET_DESCRIPTION      AS Description,
            ASSET_TYPE_CODE        AS TypeCode,
            ASSET_LOCATION         AS Location,
            ASSET_FACILITY_KEY     AS FacilityId,
            ASSET_DISTRICT_OF_SERVICE AS DistrictOfService,
            ASSET_PRIMARY_SPECIALTY_KEY AS PrimarySpecialtyKey,
            ASSET_PAEDIATRIC       AS Paediatric,
            ASSET_DEDICATED_ACUTE  AS DedicatedAcute
        FROM dbo.asset
        WHERE ASSET_KEY = @Id";

            return await QuerySingleOrDefaultAsync<Asset>(sql, new { Id = id });
        }


        public async Task<int> Create(Asset t)
        {
            const string sql = @"
            INSERT INTO dbo.asset (
                ASSET_CODE,
                ASSET_DESCRIPTION,
                ASSET_TYPE_CODE,
                ASSET_LOCATION,
                ASSET_FACILITY_KEY,
                ASSET_DISTRICT_OF_SERVICE,
                ASSET_PRIMARY_SPECIALTY_KEY,
                ASSET_PAEDIATRIC,
                ASSET_DEDICATED_ACUTE
            )
            VALUES (
                @Code,
                @Description,
                @TypeCode,
                @Location,
                @FacilityId,
                @DistrictOfService,
                @PrimarySpecialtyId,
                @Paediatric,
                @DedicatedAcute
            )";

            return await ExecuteAsync(sql, t);
        }




        public async Task<int> Update(Asset t)
        {
            const string sql = @"
            UPDATE dbo.asset
            SET 
                ASSET_CODE = @Code,
                ASSET_DESCRIPTION = @Description,
                ASSET_TYPE_CODE = @TypeCode,
                ASSET_LOCATION = @Location,
                ASSET_FACILITY_KEY = @FacilityId,
                ASSET_DISTRICT_OF_SERVICE = @DistrictOfService,
                ASSET_PRIMARY_SPECIALTY_KEY = @PrimarySpecialtyId,
                ASSET_PAEDIATRIC = @Paediatric,
                ASSET_DEDICATED_ACUTE = @DedicatedAcute
            WHERE ASSET_KEY = @Id";

            return await ExecuteAsync(sql, t);
        }




        public async Task<int> Delete(int id)
        {
            const string sql = "DELETE FROM dbo.asset WHERE ASSET_KEY = @Id";
            return await ExecuteAsync(sql, new { Id = id });
        }




      public async Task<IEnumerable<AssetDTO>> GetAllDTO()
        {
            // Get all assets with facility and specialty info
            const string sql = @"
                SELECT 
                    a.ASSET_KEY AS Id,
                    a.ASSET_CODE AS Code,
                    a.ASSET_DESCRIPTION AS Description,
                    a.ASSET_TYPE_CODE AS TypeCode,
                    a.ASSET_LOCATION AS Location,
                    a.ASSET_DISTRICT_OF_SERVICE AS DistrictOfService,
                    a.ASSET_PAEDIATRIC AS Paediatric,
                    a.ASSET_DEDICATED_ACUTE AS DedicatedAcute,
                    a.ASSET_PRIMARY_SPECIALTY_KEY AS PrimarySpecialtyId,

                    f.FACILITY_CODE AS FacilityCode,
                    f.FACILITY_NAME AS FacilityName,
                    f.FACILITY_TYPE_CODE AS FacilityTypeCode,
                    f.FACILITY_TYPE_DESCRIPTION AS TypeDescription,
                    f.FACILITY_DHB_CODE AS DhbCode,
                    f.FACILITY_DHB_NAME AS DhbName,

                    s.SPECIALTY_CODE AS SpecialtyCode,
                    s.SPECIALTY_DESCRIPTION AS SpecialtyDescription

                FROM dbo.asset a
                LEFT JOIN dbo.facility f ON a.ASSET_FACILITY_KEY = f.FACILITY_KEY
                LEFT JOIN dbo.specialty s ON a.ASSET_PRIMARY_SPECIALTY_KEY = s.SPECIALTY_KEY
                ORDER BY a.ASSET_DESCRIPTION;
            ";

            var assets = (await QueryAsync<AssetDTO>(sql, new { })).ToList();

            if (!assets.Any())
                return assets;

            // Get all asset equipment
            const string equipmentSql = @"
                SELECT 
                    ae.ASSET_KEY AS AssetId,
                    e.EQUIPMENT_NAME AS EquipmentName
                FROM dbo.asset_equipment ae
                INNER JOIN dbo.equipment e ON ae.EQUIPMENT_KEY = e.EQUIPMENT_KEY;
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





        public async Task<AssetDTO?> GetByIdDTO(int id) 
        {
            // Get asset info with facility info
            const string sql = @"
                SELECT 
                    a.ASSET_KEY             AS Id,
                    a.ASSET_CODE            AS Code,
                    a.ASSET_DESCRIPTION     AS Description,
                    a.ASSET_TYPE_CODE       AS TypeCode,
                    a.ASSET_LOCATION        AS Location,
                    a.ASSET_DISTRICT_OF_SERVICE AS DistrictOfService,
                    a.ASSET_PAEDIATRIC      AS Paediatric,
                    a.ASSET_DEDICATED_ACUTE AS DedicatedAcute,
                    a.ASSET_FACILITY_KEY    AS FacilityId,
                    f.FACILITY_CODE         AS FacilityCode,
                    f.FACILITY_NAME         AS FacilityName,
                    f.FACILITY_TYPE_CODE    AS FacilityTypeCode,
                    f.FACILITY_TYPE_DESCRIPTION AS TypeDescription,
                    f.FACILITY_DHB_CODE     AS DhbCode,
                    f.FACILITY_DHB_NAME     AS DhbName,
                    a.ASSET_PRIMARY_SPECIALTY_KEY AS PrimarySpecialtyId,
                    s.SPECIALTY_CODE        AS SpecialtyCode,
                    s.SPECIALTY_DESCRIPTION AS SpecialtyDescription
                FROM dbo.asset a
                LEFT JOIN dbo.facility f ON a.ASSET_FACILITY_KEY = f.FACILITY_KEY
                LEFT JOIN dbo.specialty s ON a.ASSET_PRIMARY_SPECIALTY_KEY = s.SPECIALTY_KEY
                WHERE a.ASSET_KEY = @Id;
            ";

            var asset = (await QueryAsync<AssetDTO>(sql, new { Id = id }))
                        .FirstOrDefault();

            if (asset == null)
                return null; // or throw new KeyNotFoundException($"Asset {id} not found");

            // Get equipment for this asset by joining with the equipment table
            const string equipmentSql = @"
                SELECT 
                    ae.ASSET_KEY AS AssetId,
                    e.EQUIPMENT_NAME AS EquipmentName
                FROM dbo.asset_equipment ae
                INNER JOIN dbo.equipment e ON ae.EQUIPMENT_KEY = e.EQUIPMENT_KEY
                WHERE ae.ASSET_KEY = @Id;
            ";

            var equipment = await QueryAsync<(int AssetId, string EquipmentName)>(equipmentSql, new { Id = id });

            asset.EquipmentNames = equipment.Select(e => e.EquipmentName).ToArray();

            return asset;
        }



    }
}
