using MediHub.Domain.DTOs;
using MediHub.Domain.Matrix;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class InstanceRepository : BaseRepository, IInstanceRepository
    {
        public InstanceRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<IEnumerable<Instance>> GetAll()
        {
            const string sql = @"
                SELECT 
                    INSTANCE_KEY             AS Id,
                    INSTANCE_ASSET_KEY       AS AssetId,
                    INSTANCE_SESSION_KEY     AS SessionId,
                    INSTANCE_START_DATETIME  AS StartDatetime,
                    INSTANCE_END_DATETIME    AS EndDatetime,
                    INSTANCE_IS_OPEN         AS IsOpen
                FROM dbo.instance";

            return await QueryAsync<Instance>(sql);
        }

        public async Task<IEnumerable<ScheduleDTO>> GetAllByStaffId(int staffId)
        {
            const string sql = @"
                SELECT DISTINCT
                    i.INSTANCE_KEY AS Id,
                    f.FACILITY_NAME AS FacilityName,
                    a.ASSET_DESCRIPTION AS AssetName,
                    s.SESSION_NAME AS SessionName,
                    s.SESSION_IS_ACUTE AS IsAcute,
                    s.SESSION_IS_PAEDIATRIC AS IsPediatric,
                    s.SESSION_ANAESTHETIC_TYPE AS AnaestheticType,
                    su.STAFF_FIRST_NAME + ' ' + su.STAFF_LAST_NAME AS SurgeonName,
                    i.INSTANCE_START_DATETIME AS StartDateTime,
                    i.INSTANCE_END_DATETIME   AS EndDateTime
                FROM dbo.instance i
                INNER JOIN dbo.instance_staff isf
                    ON isf.INSTANCE_STAFF_INSTANCE_KEY = i.INSTANCE_KEY
                LEFT JOIN dbo.asset a
                    ON i.INSTANCE_ASSET_KEY = a.ASSET_KEY
                LEFT JOIN dbo.facility f
                    ON a.ASSET_FACILITY_KEY = f.FACILITY_KEY
                LEFT JOIN dbo.session s
                    ON i.INSTANCE_SESSION_KEY = s.SESSION_KEY
                LEFT JOIN dbo.staff su
                    ON s.SESSION_SURGEON_KEY = su.STAFF_KEY
                WHERE isf.INSTANCE_STAFF_STAFF_KEY = @StaffId
                ORDER BY i.INSTANCE_START_DATETIME";

            return await QueryAsync<ScheduleDTO>(
                sql,
                new { StaffId = staffId }
            );
        }

        public async Task<Instance?> GetById(int id)
        {
            const string sql = @"
                SELECT 
                    INSTANCE_KEY             AS Id,
                    INSTANCE_ASSET_KEY       AS AssetId,
                    INSTANCE_SESSION_KEY     AS SessionId,
                    INSTANCE_START_DATETIME  AS StartDatetime,
                    INSTANCE_END_DATETIME    AS EndDatetime,
                    INSTANCE_IS_OPEN         AS IsOpen
                FROM dbo.instance
                WHERE INSTANCE_KEY = @Id";

            return await QuerySingleOrDefaultAsync<Instance>(
                sql,
                new { Id = id }
            );
        }

        public async Task<int> Create(Instance i)
        {
            const string sql = @"
                INSERT INTO dbo.instance (
                    INSTANCE_ASSET_KEY,
                    INSTANCE_SESSION_KEY,
                    INSTANCE_START_DATETIME,
                    INSTANCE_END_DATETIME,
                    INSTANCE_IS_OPEN
                )
                OUTPUT INSERTED.INSTANCE_KEY
                VALUES (
                    @AssetId,
                    @SessionId,
                    @StartDatetime,
                    @EndDatetime,
                    @IsOpen
                )";

            return await ExecuteScalarAsync<int>(sql, i);
        }

        public async Task<int> Update(Instance i)
        {
            const string sql = @"
                UPDATE dbo.instance
                SET
                    INSTANCE_ASSET_KEY = @AssetId,
                    INSTANCE_SESSION_KEY = @SessionId,
                    INSTANCE_START_DATETIME = @StartDatetime,
                    INSTANCE_END_DATETIME = @EndDatetime,
                    INSTANCE_IS_OPEN = @IsOpen
                WHERE INSTANCE_KEY = @Id";

            return await ExecuteAsync(sql, i);
        }

        public async Task<int> Delete(int id)
        {
            const string sql = @"
                BEGIN TRANSACTION;

                -- Delete all staff links for this instance
                DELETE FROM dbo.instance_staff
                WHERE INSTANCE_KEY = @Id;

                -- Delete the instance itself
                DELETE FROM dbo.instance
                WHERE INSTANCE_KEY = @Id;

                COMMIT;
            ";

            return await ExecuteAsync(sql, new { Id = id });
        }


        public async Task<IEnumerable<InstanceDTO>> GetAllDTO()
        {
            const string sql = @"

            SELECT

                -- Instance
                i.INSTANCE_KEY AS Id,
                i.INSTANCE_SESSION_KEY AS SessionId,
                i.INSTANCE_ASSET_KEY AS AssetId,
                i.INSTANCE_START_DATETIME AS StartDatetime,
                i.INSTANCE_END_DATETIME AS EndDatetime,
                i.INSTANCE_IS_OPEN AS IsOpen,
                i.INSTANCE_LAST_UPDATED_DATETIME AS LastUpdatedDatetime,
                i.INSTANCE_LAST_UPDATED_USER_KEY AS LastUpdatedByUserId,

                upd.STAFF_NAME AS LastUpdatedByUserName,

                -- Session
                s.SESSION_TITLE AS SessionTitle,
                s.SESSION_IS_ACUTE AS SessionIsAcute,
                s.SESSION_IS_PAEDIATRIC AS SessionIsPaediatric,
                s.SESSION_ANAESTHETIC_TYPE AS AnaestheticType,
                s.SESSION_SURGEON_KEY AS SurgeonId,
                surgeon.STAFF_NAME AS SurgeonName,

                s.SESSION_SPECIALTY_KEY AS SpecialtyId,
                sp.SPECIALTY_CODE AS SpecialtyCode,
                sp.SPECIALTY_DESCRIPTION AS SpecialtyDescription,

                s.SESSION_SUBSPECIALTY_KEY AS SubspecialtyId,
                subs.SUBSPECIALTY_NAME AS SubspecialtyName,

                -- Asset
                a.ASSET_CODE AS AssetCode,
                a.ASSET_LOCATION AS AssetLocation,
                a.ASSET_DESCRIPTION AS AssetDescription,
                
                a.ASSET_FACILITY_KEY AS FacilityId,
                f.FACILITY_NAME AS FacilityName,

                -- Staff
                st.STAFF_KEY AS StaffId,
                st.STAFF_ID,
                st.STAFF_NAME,
                st.STAFF_EMAIL,

                st.STAFF_SPECIALTY_KEY AS SpecialtyId,
                sps.SPECIALTY_CODE,
                sps.SPECIALTY_DESCRIPTION,

                r.ROLE_KEY AS RoleId,
                r.ROLE_NAME AS RoleName

            FROM dbo.instance i

            LEFT JOIN dbo.session s
                ON s.SESSION_KEY = i.INSTANCE_SESSION_KEY

            LEFT JOIN dbo.staff surgeon
                ON surgeon.STAFF_KEY = s.SESSION_SURGEON_KEY

            LEFT JOIN dbo.specialty sp
                ON sp.SPECIALTY_KEY = s.SESSION_SPECIALTY_KEY

            LEFT JOIN dbo.subspecialty subs
                ON subs.SUBSPECIALTY_KEY = s.SESSION_SUBSPECIALTY_KEY

            LEFT JOIN dbo.asset a
                ON a.ASSET_KEY = i.INSTANCE_ASSET_KEY

            LEFT JOIN dbo.facility f
                ON f.FACILITY_KEY = a.ASSET_FACILITY_KEY

            LEFT JOIN dbo.staff upd
                ON upd.STAFF_KEY = i.INSTANCE_LAST_UPDATED_USER_KEY

            LEFT JOIN dbo.instance_staff ist
                ON ist.INSTANCE_KEY = i.INSTANCE_KEY

            LEFT JOIN dbo.staff st
                ON st.STAFF_KEY = ist.STAFF_KEY

            LEFT JOIN dbo.specialty sps
                ON sps.SPECIALTY_KEY = st.STAFF_SPECIALTY_KEY

            LEFT JOIN dbo.role r
                ON r.ROLE_KEY = ist.ROLE_KEY
            ";

            var lookup = new Dictionary<int, InstanceDTO>();

            await QueryAsync<InstanceDTO, StaffDTO, InstanceDTO>(
                sql,
                (instance, staff) =>
                {
                    if (!lookup.TryGetValue(instance.Id, out var existing))
                    {
                        existing = instance;
                        existing.Staffs = new List<StaffDTO>();
                        lookup.Add(existing.Id, existing);
                    }

                    if (staff != null && staff.StaffId != null)
                    {
                        existing.Staffs.Add(staff);
                    }

                    return existing;
                },
                splitOn: "StaffId"
            );

            return lookup.Values;
        }


        public async Task<int> CreateDTO(InstanceDTO i)
        {
            const string sql = @"
                INSERT INTO dbo.instance (
                    INSTANCE_SESSION_KEY,
                    INSTANCE_ASSET_KEY,
                    INSTANCE_START_DATETIME,
                    INSTANCE_END_DATETIME,
                    INSTANCE_IS_OPEN,
                    INSTANCE_LAST_UPDATED_DATETIME,
                    INSTANCE_LAST_UPDATED_USER_KEY
                )
                VALUES (
                    @SessionId,
                    @AssetId,
                    @StartDatetime,
                    @EndDatetime,
                    @IsOpen,
                    @LastUpdatedDatetime,
                    @LastUpdatedByUserId
                );
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            var parameters = new
            {
                i.SessionId,
                i.AssetId,
                StartDatetime = i.StartDatetime,
                EndDatetime = i.EndDatetime,
                i.IsOpen,
                i.LastUpdatedDatetime,
                i.LastUpdatedByUserId
            };

            // Returns the new INSTANCE_KEY
            var newId = await ExecuteScalarAsync<int>(sql, parameters);

            // Optional: insert staff assignments if any
            if (i.Staffs != null && i.Staffs.Count > 0)
            {
                const string staffSql = @"
                    INSERT INTO dbo.instance_staff (
                        INSTANCE_KEY,
                        STAFF_KEY,
                        ROLE_KEY
                    )
                    VALUES (@InstanceId, @StaffId, @RoleId);
                ";

                foreach (var staff in i.Staffs)
                {
                    await ExecuteAsync(staffSql, new
                    {
                        InstanceId = newId,
                        StaffId = staff.StaffId,
                        RoleId = staff.RoleId
                    });
                }
            }

            return newId;
        }


        public async Task<InstanceDTO?> GetByIdDTO(int id)
        {
            const string sql = @"

                SELECT

                    -- Instance
                    i.INSTANCE_KEY AS Id,
                    i.INSTANCE_SESSION_KEY AS SessionId,
                    i.INSTANCE_ASSET_KEY AS AssetId,
                    i.INSTANCE_START_DATETIME AS StartDatetime,
                    i.INSTANCE_END_DATETIME AS EndDatetime,
                    i.INSTANCE_IS_OPEN AS IsOpen,
                    i.INSTANCE_LAST_UPDATED_DATETIME AS LastUpdatedDatetime,
                    i.INSTANCE_LAST_UPDATED_USER_KEY AS LastUpdatedByUserId,

                    upd.STAFF_NAME AS LastUpdatedByUserName,

                    -- Session
                    s.SESSION_TITLE AS SessionTitle,
                    s.SESSION_IS_ACUTE AS SessionIsAcute,
                    s.SESSION_IS_PAEDIATRIC AS SessionIsPaediatric,
                    s.SESSION_ANAESTHETIC_TYPE AS AnaestheticType,
                    s.SESSION_SURGEON_KEY AS SurgeonId,
                    surgeon.STAFF_NAME AS SurgeonName,

                    s.SESSION_SPECIALTY_KEY AS SpecialtyId,
                    sp.SPECIALTY_CODE AS SpecialtyCode,
                    sp.SPECIALTY_DESCRIPTION AS SpecialtyDescription,

                    s.SESSION_SUBSPECIALTY_KEY AS SubspecialtyId,
                    subs.SUBSPECIALTY_NAME AS SubspecialtyName,

                    -- Asset
                    a.ASSET_CODE AS AssetCode,
                    a.ASSET_LOCATION AS AssetLocation,
                    a.ASSET_DESCRIPTION AS AssetDescription,

                    a.ASSET_FACILITY_KEY AS FacilityId,
                    f.FACILITY_NAME AS FacilityName,

                    -- Staff
                    st.STAFF_KEY AS StaffId,
                    st.STAFF_ID,
                    st.STAFF_NAME,
                    st.STAFF_EMAIL,

                    st.STAFF_SPECIALTY_KEY AS SpecialtyId,
                    sps.SPECIALTY_CODE,
                    sps.SPECIALTY_DESCRIPTION,

                    r.ROLE_KEY AS RoleId,
                    r.ROLE_NAME AS RoleName

                FROM dbo.instance i

                LEFT JOIN dbo.session s
                    ON s.SESSION_KEY = i.INSTANCE_SESSION_KEY

                LEFT JOIN dbo.staff surgeon
                    ON surgeon.STAFF_KEY = s.SESSION_SURGEON_KEY

                LEFT JOIN dbo.specialty sp
                    ON sp.SPECIALTY_KEY = s.SESSION_SPECIALTY_KEY

                LEFT JOIN dbo.subspecialty subs
                    ON subs.SUBSPECIALTY_KEY = s.SESSION_SUBSPECIALTY_KEY

                LEFT JOIN dbo.asset a
                    ON a.ASSET_KEY = i.INSTANCE_ASSET_KEY

                LEFT JOIN dbo.facility f
                    ON f.FACILITY_KEY = a.ASSET_FACILITY_KEY

                LEFT JOIN dbo.staff upd
                    ON upd.STAFF_KEY = i.INSTANCE_LAST_UPDATED_USER_KEY

                LEFT JOIN dbo.instance_staff ist
                    ON ist.INSTANCE_KEY = i.INSTANCE_KEY

                LEFT JOIN dbo.staff st
                    ON st.STAFF_KEY = ist.STAFF_KEY

                LEFT JOIN dbo.specialty sps
                    ON sps.SPECIALTY_KEY = st.STAFF_SPECIALTY_KEY

                LEFT JOIN dbo.role r
                    ON r.ROLE_KEY = ist.ROLE_KEY

                WHERE i.INSTANCE_KEY = @Id
            ";

            var lookup = new Dictionary<int, InstanceDTO>();

            await QueryAsync<InstanceDTO, StaffDTO, InstanceDTO>(
                sql,
                (instance, staff) =>
                {
                    if (!lookup.TryGetValue(instance.Id, out var existing))
                    {
                        existing = instance;
                        existing.Staffs = new List<StaffDTO>();
                        lookup.Add(existing.Id, existing);
                    }

                    if (staff != null && staff.StaffId != null)
                    {
                        existing.Staffs.Add(staff);
                    }

                    return existing;
                },
                new { Id = id },
                splitOn: "StaffId"
            );

            return lookup.Values.FirstOrDefault();
        }


        public async Task<InstanceDTO?> UpdateDTO(InstanceDTO i)
        {
            // 1️⃣ Update main instance
            const string sql = @"
                UPDATE dbo.instance
                SET
                    INSTANCE_SESSION_KEY = @SessionId,
                    INSTANCE_ASSET_KEY = @AssetId,
                    INSTANCE_START_DATETIME = @StartDatetime,
                    INSTANCE_END_DATETIME = @EndDatetime,
                    INSTANCE_IS_OPEN = @IsOpen,
                    INSTANCE_LAST_UPDATED_DATETIME = @LastUpdatedDatetime,
                    INSTANCE_LAST_UPDATED_USER_KEY = @LastUpdatedByUserId
                WHERE INSTANCE_KEY = @Id;
            ";

            var parameters = new
            {
                i.Id,
                i.SessionId,
                i.AssetId,
                StartDatetime = i.StartDatetime,
                EndDatetime = i.EndDatetime,
                i.IsOpen,
                i.LastUpdatedDatetime,
                i.LastUpdatedByUserId
            };

            await ExecuteAsync(sql, parameters);

            // 2️⃣ Delete existing staff assignments for this instance
            const string deleteStaffSql = @"
                DELETE FROM dbo.instance_staff
                WHERE INSTANCE_KEY = @InstanceId;
            ";
            await ExecuteAsync(deleteStaffSql, new { InstanceId = i.Id });

            // 3️⃣ Insert new staff assignments
            if (i.Staffs != null && i.Staffs.Count > 0)
            {
                const string insertStaffSql = @"
                    INSERT INTO dbo.instance_staff (
                        INSTANCE_KEY,
                        STAFF_KEY,
                        ROLE_KEY
                    )
                    VALUES (@InstanceId, @StaffId, @RoleId);
                ";

                foreach (var staff in i.Staffs)
                {
                    await ExecuteAsync(insertStaffSql, new
                    {
                        InstanceId = i.Id,
                        StaffId = staff.StaffId,
                        RoleId = staff.RoleId
                    });
                }
            }

            // 4️⃣ Return the updated instance including its staff
            return await GetByIdDTO(i.Id);
        }

        public async Task<int> DeleteDTO(int id)
        {
            const string sql = @"
                -- 1️⃣ Delete related staff assignments
                DELETE FROM dbo.instance_staff
                WHERE INSTANCE_KEY = @Id;

                -- 2️⃣ Delete the instance itself
                DELETE FROM dbo.instance
                WHERE INSTANCE_KEY = @Id;
            ";

            return await ExecuteAsync(sql, new { Id = id });
        }


        public async Task<InstanceMatrixFacilityDTO[]> GetAllWeekMatrix(DateOnly date)
        {
            var startDate = date.ToDateTime(TimeOnly.MinValue);
            var endDate = date.AddDays(7).ToDateTime(TimeOnly.MaxValue);

            const string sql = @"
        SELECT 
            f.FACILITY_KEY AS FacilityId,
            f.FACILITY_NAME AS FacilityName,

            a.ASSET_KEY AS AssetId,
            a.ASSET_DESCRIPTION AS AssetDescription,

            i.INSTANCE_KEY AS Id,   -- MUST MATCH DTO
            f.FACILITY_KEY AS FacilityId,
            f.FACILITY_NAME AS FacilityName,
            a.ASSET_KEY AS AssetId,
            a.ASSET_DESCRIPTION AS AssetDescription,

            i.INSTANCE_SESSION_KEY AS SessionId,
            s.SESSION_TITLE AS SessionTitle,
            s.SESSION_IS_ACUTE AS IsAcute,
            s.SESSION_IS_PAEDIATRIC AS IsPediatric,
            s.SESSION_ANAESTHETIC_TYPE AS AnaestheticType,
            s.SESSION_SURGEON_KEY AS SurgeonId,
            st.STAFF_NAME AS SurgeonName,

            i.INSTANCE_START_DATETIME AS StartDateTime,
            i.INSTANCE_END_DATETIME AS EndDateTime,
            i.INSTANCE_IS_OPEN AS IsOpen,

            (SELECT COUNT(*) 
             FROM INSTANCE_STAFF ist 
             WHERE ist.INSTANCE_KEY = i.INSTANCE_KEY) AS StaffCount

        FROM FACILITY f

        INNER JOIN ASSET a 
            ON a.ASSET_FACILITY_KEY = f.FACILITY_KEY

        LEFT JOIN INSTANCE i 
            ON i.INSTANCE_ASSET_KEY = a.ASSET_KEY
            AND i.INSTANCE_START_DATETIME BETWEEN @startDate AND @endDate

        LEFT JOIN SESSION s 
            ON s.SESSION_KEY = i.INSTANCE_SESSION_KEY

        LEFT JOIN STAFF st 
            ON st.STAFF_KEY = s.SESSION_SURGEON_KEY

        ORDER BY 
            f.FACILITY_NAME,
            a.ASSET_DESCRIPTION,
            i.INSTANCE_START_DATETIME
    ";

            var lookup = new Dictionary<int, InstanceMatrixFacilityDTO>();

            await QueryAsync<InstanceMatrixFacilityDTO, InstanceMatrixAssetDTO, InstanceMatrixCellDTO, InstanceMatrixFacilityDTO>(
                sql,
                (facility, asset, cell) =>
                {
                    //--------------------------------------------------
                    // Facility
                    //--------------------------------------------------

                    if (!lookup.TryGetValue(facility.FacilityId ?? 0, out var existingFacility))
                    {
                        existingFacility = facility;
                        existingFacility.Assets = new List<InstanceMatrixAssetDTO>();

                        lookup.Add(existingFacility.FacilityId ?? 0, existingFacility);
                    }

                    //--------------------------------------------------
                    // Asset
                    //--------------------------------------------------

                    if (asset != null)
                    {
                        var existingAsset = existingFacility.Assets
                            .FirstOrDefault(a => a.AssetId == asset.AssetId);

                        if (existingAsset == null)
                        {
                            existingAsset = asset;
                            existingAsset.Days = new List<InstanceMatrixDayDTO>();

                            existingFacility.Assets.Add(existingAsset);
                        }

                        //--------------------------------------------------
                        // Cell → Day → ADD TO LIST
                        //--------------------------------------------------

                        if (cell != null && cell.Id != 0)
                        {
                            var cellDate = cell.StartDateTime?.Date ?? DateTime.MinValue;

                            var day = existingAsset.Days
                                .FirstOrDefault(d => d.Date.Date == cellDate);

                            if (day == null)
                            {
                                day = new InstanceMatrixDayDTO
                                {
                                    Date = cellDate,
                                    DayName = cellDate.ToString("ddd"),
                                    Instance = new List<InstanceMatrixCellDTO>()
                                };

                                existingAsset.Days.Add(day);
                            }

                            //--------------------------------------------------
                            // FIX: ADD TO LIST
                            //--------------------------------------------------

                            day.Instance.Add(cell);
                        }
                    }

                    return existingFacility;
                },
                new { startDate, endDate },

                //--------------------------------------------------
                // CRITICAL: splitOn must match SQL order
                //--------------------------------------------------

                splitOn: "AssetId,Id"
            );

            return lookup.Values.ToArray();
        }
    }
}
