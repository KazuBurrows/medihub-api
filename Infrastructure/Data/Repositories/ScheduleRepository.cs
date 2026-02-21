using Dapper;
using MediHub.Domain.DTOs;
using MediHub.Domain.Models;
using MediHub.Functions.Helpers.Exceptions;
using MediHub.Infrastructure.Data.Helpers;
using MediHub.Infrastructure.Data.Interfaces;
using MediHub.Infrastructure.Data.Utils;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class ScheduleRepository : BaseRepository, IScheduleRepository
    {
        private readonly SchedulingClash _schedulingClash;

        public ScheduleRepository(
            SqlConnectionFactory connectionFactory,
            SchedulingClash schedulingClash
        ) : base(connectionFactory)
        {
            _schedulingClash = schedulingClash;
        }


        public async Task<IEnumerable<ScheduleDTO>> GetMatrixByWeek(DateTime monday, DateTime sunday)
        {
            const string sql = @"
                SELECT
                    s.id AS Id,
                    t.id AS AssetId,
                    t.name AS AssetName,
                    f.id AS FacilityId,
                    f.name AS FacilityName,
                    se.id AS SessionId,
                    se.name AS SessionName,
                    s.start_datetime AS StartDateTime,
                    s.end_datetime AS EndDateTime
                FROM dbo.instance s
                LEFT JOIN dbo.asset t ON s.asset_id = t.id
                LEFT JOIN dbo.facility f ON t.facility_id = f.id
                LEFT JOIN dbo.session se ON s.session_id = se.id
                WHERE s.start_datetime >= @Monday
                AND s.end_datetime   <= @Sunday";

            var parameters = new { Monday = monday, Sunday = sunday };

            return await QueryAsync<ScheduleDTO>(sql, parameters);
        }


        public async Task<IEnumerable<ScheduleDTO>> GetAllDTO(DateTime startDateTime, DateTime endDateTime)
        {
            const string sql = @"
                SELECT
                    s.id AS Id,
                    t.id AS AssetId,
                    t.name AS AssetName,
                    f.id AS FacilityId,
                    f.name AS FacilityName,
                    se.id AS SessionId,
                    se.name AS SessionName,
                    s.start_datetime AS StartDateTime,
                    s.end_datetime AS EndDateTime
                FROM dbo.instance s
                LEFT JOIN dbo.asset t ON s.asset_id = t.id
                LEFT JOIN dbo.facility f ON t.facility_id = f.id
                LEFT JOIN dbo.session se ON s.session_id = se.id
                WHERE s.start_datetime >= @Start
                AND s.end_datetime <= @End";

            var parameters = new { Start = startDateTime, End = endDateTime };

            return await QueryAsync<ScheduleDTO>(sql, parameters);
        }


        public async Task<IEnumerable<ScheduleDTO>> GetMatrix(DateTime monday, DateTime sunday, int? facility, int? asset)
        {
            // SQL query: join instances, sessions, assets, facilities, staff
            const string sql = @"
                SELECT
                    i.id AS Id,
                    f.name AS FacilityName,
                    t.name AS AssetName,
                    s.name AS SessionName,
                    s.is_acute AS IsAcute,
                    s.is_pediatric AS IsPediatric,
                    s.anaesthetic_type AS AnaestheticType,
                    CONCAT(st.first_name, ' ', st.last_name) AS SurgeonName,
                    COUNT(st.id) AS StaffCount,
                    i.start_datetime AS StartDateTime,
                    i.end_datetime AS EndDateTime
                FROM dbo.instance i
                INNER JOIN dbo.session s ON i.session_id = s.id
                INNER JOIN dbo.asset t ON i.asset_id = t.id
                INNER JOIN dbo.facility f ON t.facility_id = f.id
                LEFT JOIN dbo.staff st ON s.surgeon_id = st.id
                WHERE i.start_datetime >= @StartDate
                AND i.end_datetime <= @EndDate
                /**FACILITY_FILTER**/
                /**ASSET_FILTER**/
                GROUP BY
                    i.id, f.name, t.name, s.name,
                    s.is_acute, s.is_pediatric, s.anaesthetic_type,
                    st.first_name, st.last_name,
                    i.start_datetime, i.end_datetime
                ORDER BY i.start_datetime;

                ";

            // Build dynamic filters
            var dynamicSql = sql;
            var parameters = new Dictionary<string, object>
            {
                ["StartDate"] = monday,
                ["EndDate"] = sunday
            };

            if (facility.HasValue)
            {
                dynamicSql = dynamicSql.Replace("/**FACILITY_FILTER**/", "AND f.id = @Facility");
                parameters["Facility"] = facility.Value;
            }
            else
            {
                dynamicSql = dynamicSql.Replace("/**FACILITY_FILTER**/", "");
            }

            if (asset.HasValue)
            {
                dynamicSql = dynamicSql.Replace("/**ASSET_FILTER**/", "AND t.id = @Asset");
                parameters["Asset"] = asset.Value;
            }
            else
            {
                dynamicSql = dynamicSql.Replace("/**ASSET_FILTER**/", "");
            }

            // Execute the query
            return await QueryAsync<ScheduleDTO>(dynamicSql, parameters);
        }

        public async Task<MatrixFormatAgg> GetMatrixFormat(int facilityId)
        {
            const string sql = @"
                SELECT
                    f.id   AS FacilityId,
                    f.name AS FacilityName,
                    t.id   AS AssetId,
                    t.name AS AssetName
                FROM dbo.facility f
                LEFT JOIN dbo.asset t ON t.facility_id = f.id
                WHERE (@FacilityId = 0 OR f.id = @FacilityId)
                ORDER BY
                    f.name,
                    t.name"
            ;

            var parameters = new { FacilityId = facilityId };

            var rows = await QueryAsync<MatrixFormatRow>(sql, parameters);

            var facilities = rows
                .GroupBy(r => new { r.FacilityId, r.FacilityName })
                .Select(fg => new FacilityFormat(
                    fg.Key.FacilityId,
                    fg.Key.FacilityName,
                    fg.Select(t => new AssetFormat(
                        t.AssetId,
                        t.AssetName,
                        0   // no sort_order in schema (yet)
                    ))
                ))
                .ToList();

            return new MatrixFormatAgg(facilities);
        }

        public async Task<InstanceDetailDTO?> GetInstanceDetailDTO(int instanceId)
        {
            // ---------------- MAIN INSTANCE QUERY ----------------
            const string sql = @"
                SELECT 
                    i.id AS Id,
                    i.session_id AS SessionId,
                    s.name AS SessionName,
                    s.is_acute AS IsAcute,
                    s.is_pediatric AS IsPediatric,
                    s.anaesthetic_type AS AnaestheticType,
                    CONCAT(st.first_name, ' ', st.last_name) AS SurgeonName,
                    sp.name AS SpecialtyName,
                    sub.name AS SubspecialtyName,
                    t.id AS AssetId,
                    t.name AS AssetName,
                    f.name AS FacilityName,
                    i.start_datetime AS StartDateTime,
                    i.end_datetime AS EndDateTime
                FROM dbo.instance i
                INNER JOIN dbo.session s ON i.session_id = s.id
                INNER JOIN dbo.asset t ON i.asset_id = t.id
                INNER JOIN dbo.facility f ON t.facility_id = f.id
                LEFT JOIN dbo.staff st ON s.surgeon_id = st.id
                LEFT JOIN dbo.specialty sp ON s.specialty_id = sp.id
                LEFT JOIN dbo.subspecialty sub ON s.subspecialty_id = sub.id
                WHERE i.id = @InstanceId;
            ";

            var instance = (await QueryAsync<InstanceDetailDTO>(
                sql,
                new { InstanceId = instanceId }
            )).FirstOrDefault();

            if (instance == null)
                return null;

            // ---------------- STAFF + ROLE QUERY ----------------
            const string staffSql = @"
                SELECT 
                    st.STAFF_KEY AS Id,
                    st.STAFF_ID AS Code,
                    st.STAFF_NAME AS Name,
                    st.STAFF_EMAIL AS Email,
                    r.ROLE_KEY AS RoleId,
                    r.ROLE_NAME AS RoleName
                FROM dbo.instance_staff isf
                LEFT JOIN dbo.staff st ON isf.STAFF_KEY = st.STAFF_KEY
                LEFT JOIN dbo.role r ON isf.ROLE_KEY = r.ROLE_KEY
                WHERE isf.INSTANCE_KEY = @InstanceId
                ORDER BY r.ROLE_NAME, st.STAFF_NAME;
            ";


            var staff = await QueryAsync<StaffDTO>(
                staffSql,
                new { InstanceId = instanceId }
            );

            instance.Staffs = staff.ToArray();

            return instance;
        }




        public async Task<InstanceDetailDTO> PutInstanceDetailDTO(
            int id,
            int sessionId,
            int assetId,
            string startDatetime,
            string endDatetime,
            List<StaffDTO> staffs,
            bool force)
        {
            // Parse datetime
            if (!DateTime.TryParse(startDatetime, out var start))
                throw new ArgumentException("Invalid startDatetime", nameof(startDatetime));

            if (!DateTime.TryParse(endDatetime, out var end))
                throw new ArgumentException("Invalid endDatetime", nameof(endDatetime));

            /* ---------------- INSTANCE CLASH CHECK ---------------- */
            // var conflictMessage = await _schedulingClash.CheckForInstanceClashes(id, assetId, start, end, staffs);
            // if (!force && conflictMessage != null)
            //     throw new InstanceClashException(conflictMessage);

            // Convert StaffDTO list into a TVP with both StaffId and RoleId
            var staffTable = DataTransformer.ToStaffRoleTable(staffs);

            const string sql = @"
                BEGIN TRANSACTION;

                -- Update instance
                UPDATE dbo.instance
                SET
                    session_id     = @SessionId,
                    asset_id       = @AssetId,
                    start_datetime = @StartDateTime,
                    end_datetime   = @EndDateTime
                WHERE id = @Id;

                IF @@ROWCOUNT = 0
                BEGIN
                    ROLLBACK;
                    RETURN;
                END

                -- Delete staff no longer assigned
                DELETE isf
                FROM dbo.instance_staff isf
                LEFT JOIN @StaffTable s
                    ON isf.staff_id = s.StaffId
                WHERE isf.instance_id = @Id
                AND s.StaffId IS NULL;

                -- Insert new staff assignments (with role)
                INSERT INTO dbo.instance_staff (instance_id, staff_id, role_id)
                SELECT @Id, s.StaffId, s.RoleId
                FROM @StaffTable s
                LEFT JOIN dbo.instance_staff isf
                    ON isf.instance_id = @Id
                    AND isf.staff_id = s.StaffId
                WHERE isf.id IS NULL;

                COMMIT;
            ";

            var parameters = new
            {
                Id = id,
                SessionId = sessionId,
                AssetId = assetId,
                StartDateTime = start,
                EndDateTime = end,
                StaffTable = staffTable.AsTableValuedParameter("dbo.StaffRoleList") // TVP with StaffId + RoleId
            };

            await ExecuteAsync(sql, parameters);

            return await GetInstanceDetailDTO(id);
        }



        public async Task<InstanceDetailDTO> CreateInstanceDetailDTO(
            int sessionId,
            int assetId,
            string startDatetime,
            string endDatetime,
            List<StaffDTO> staffs,
            bool force)
        {
            // Parse datetime
            if (!DateTime.TryParse(startDatetime, out var start))
                throw new ArgumentException("Invalid startDatetime", nameof(startDatetime));

            if (!DateTime.TryParse(endDatetime, out var end))
                throw new ArgumentException("Invalid endDatetime", nameof(endDatetime));

            // Optional: Check for instance time clashes
            // var conflictMessage = await _schedulingClash.CheckForInstanceClashes(null, assetId, start, end, staffs);
            // if (!force && conflictMessage != null)
            //     throw new InstanceClashException(conflictMessage);

            // Convert staffs to TVP with StaffId and RoleId
            var staffTable = DataTransformer.ToStaffRoleTable(staffs);

            const string sql = @"
                BEGIN TRANSACTION;

                INSERT INTO dbo.instance (session_id, asset_id, start_datetime, end_datetime)
                VALUES (@SessionId, @AssetId, @StartDateTime, @EndDateTime);

                DECLARE @NewId INT = SCOPE_IDENTITY();

                -- Insert staff assignments with optional role
                INSERT INTO dbo.instance_staff (instance_id, staff_id, role_id)
                SELECT @NewId, s.StaffId, s.RoleId
                FROM @StaffTable s;

                COMMIT;

                SELECT @NewId AS Id;
            ";

            var parameters = new
            {
                SessionId = sessionId,
                AssetId = assetId,
                StartDateTime = start,
                EndDateTime = end,
                StaffTable = staffTable.AsTableValuedParameter("dbo.StaffRoleList") // TVP with StaffId + RoleId
            };

            var newId = (await QueryAsync<int>(sql, parameters)).First();

            return await GetInstanceDetailDTO(newId);
        }





        public async Task<IEnumerable<ScheduleDTO>> GetList()
        {
            const string sql = @"
                SELECT
                    i.id AS Id,
                    f.name AS FacilityName,
                    t.name AS AssetName,
                    s.name AS SessionName,
                    s.is_acute AS IsAcute,
                    s.is_pediatric AS IsPediatric,
                    s.anaesthetic_type AS AnaestheticType,
                    CONCAT(surgeon.first_name, ' ', surgeon.last_name) AS SurgeonName,
                    COUNT(DISTINCT ist.staff_id) AS StaffCount,
                    i.start_datetime AS StartDateTime,
                    i.end_datetime AS EndDateTime
                FROM dbo.instance i
                INNER JOIN dbo.session s ON i.session_id = s.id
                INNER JOIN dbo.asset t ON i.asset_id = t.id
                INNER JOIN dbo.facility f ON t.facility_id = f.id
                LEFT JOIN dbo.staff surgeon ON s.surgeon_id = surgeon.id
                LEFT JOIN dbo.instance_staff ist ON ist.instance_id = i.id
                GROUP BY
                    i.id,
                    f.name,
                    t.name,
                    s.name,
                    s.is_acute,
                    s.is_pediatric,
                    s.anaesthetic_type,
                    surgeon.first_name,
                    surgeon.last_name,
                    i.start_datetime,
                    i.end_datetime
                ORDER BY i.start_datetime;
                ";

            // Build dynamic filters
            var dynamicSql = sql;

            // Execute the query
            return await QueryAsync<ScheduleDTO>(dynamicSql);
        }

    }
}
