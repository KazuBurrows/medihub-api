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
                    t.id AS TheatreId,
                    t.name AS TheatreName,
                    f.id AS FacilityId,
                    f.name AS FacilityName,
                    se.id AS SessionId,
                    se.name AS SessionName,
                    s.start_datetime AS StartDateTime,
                    s.end_datetime AS EndDateTime
                FROM dbo.instance s
                LEFT JOIN dbo.theatre t ON s.theatre_id = t.id
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
                    t.id AS TheatreId,
                    t.name AS TheatreName,
                    f.id AS FacilityId,
                    f.name AS FacilityName,
                    se.id AS SessionId,
                    se.name AS SessionName,
                    s.start_datetime AS StartDateTime,
                    s.end_datetime AS EndDateTime
                FROM dbo.instance s
                LEFT JOIN dbo.theatre t ON s.theatre_id = t.id
                LEFT JOIN dbo.facility f ON t.facility_id = f.id
                LEFT JOIN dbo.session se ON s.session_id = se.id
                WHERE s.start_datetime >= @Start
                AND s.end_datetime <= @End";

            var parameters = new { Start = startDateTime, End = endDateTime };

            return await QueryAsync<ScheduleDTO>(sql, parameters);
        }


        public async Task<IEnumerable<MatrixDTO>> GetMatrix(DateTime monday, DateTime sunday, int? facility, int? theatre)
        {
            // SQL query: join instances, sessions, theatres, facilities, staff
            const string sql = @"
                SELECT
                    i.id AS Id,
                    f.name AS FacilityName,
                    t.name AS TheatreName,
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
                INNER JOIN dbo.theatre t ON i.theatre_id = t.id
                INNER JOIN dbo.facility f ON t.facility_id = f.id
                LEFT JOIN dbo.staff st ON s.surgeon_id = st.id
                WHERE i.start_datetime >= @StartDate
                AND i.end_datetime <= @EndDate
                /**FACILITY_FILTER**/
                /**THEATRE_FILTER**/
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

            if (theatre.HasValue)
            {
                dynamicSql = dynamicSql.Replace("/**THEATRE_FILTER**/", "AND t.id = @Theatre");
                parameters["Theatre"] = theatre.Value;
            }
            else
            {
                dynamicSql = dynamicSql.Replace("/**THEATRE_FILTER**/", "");
            }

            // Execute the query
            return await QueryAsync<MatrixDTO>(dynamicSql, parameters);
        }

        public async Task<MatrixFormatAgg> GetMatrixFormat(int facilityId)
        {
            const string sql = @"
                SELECT
                    f.id   AS FacilityId,
                    f.name AS FacilityName,
                    t.id   AS TheatreId,
                    t.name AS TheatreName
                FROM dbo.facility f
                LEFT JOIN dbo.theatre t ON t.facility_id = f.id
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
                    fg.Select(t => new TheatreFormat(
                        t.TheatreId,
                        t.TheatreName,
                        0   // no sort_order in schema (yet)
                    ))
                ))
                .ToList();

            return new MatrixFormatAgg(facilities);
        }

        public async Task<InstanceDetailDTO> GetInstanceDetailDTO(int instanceId)
        {
            // SQL: pull instance and session details + include SessionId and TheatreId
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
                    t.id AS TheatreId,
                    t.name AS TheatreName,
                    f.name AS FacilityName,
                    i.start_datetime AS StartDateTime,
                    i.end_datetime AS EndDateTime
                FROM dbo.instance i
                INNER JOIN dbo.session s ON i.session_id = s.id
                INNER JOIN dbo.theatre t ON i.theatre_id = t.id
                INNER JOIN dbo.facility f ON t.facility_id = f.id
                LEFT JOIN dbo.staff st ON s.surgeon_id = st.id
                LEFT JOIN dbo.specialty sp ON s.specialty_id = sp.id
                LEFT JOIN dbo.subspecialty sub ON s.subspecialty_id = sub.id
                WHERE i.id = @InstanceId;
            ";

            // Execute main query
            var instance = (await QueryAsync<InstanceDetailDTO>(sql, new { InstanceId = instanceId }))
                           .FirstOrDefault();

            if (instance == null)
                return null!; // Or throw exception

            // If you have multiple staff per instance (like a staff_instance table)
            const string staffSql = @"
                SELECT 
                    st.id AS Id,
                    st.first_name AS FirstName,
                    st.last_name AS LastName
                FROM dbo.staff st
                INNER JOIN dbo.instance_staff isf ON st.id = isf.staff_id
                WHERE isf.instance_id = @InstanceId;

            ";

            var staff = await QueryAsync<Staff>(staffSql, new { InstanceId = instanceId });
            instance.Staffs = staff.ToArray();

            return instance;
        }



        public async Task<InstanceDetailDTO> PutInstanceDetailDTO(
            int id,
            int sessionId,
            int theatreId,
            string startDatetime,
            string endDatetime,
            List<int> staffs,
            bool force)
        {
            if (!DateTime.TryParse(startDatetime, out var start))
                throw new ArgumentException("Invalid startDatetime", nameof(startDatetime));

            if (!DateTime.TryParse(endDatetime, out var end))
                throw new ArgumentException("Invalid endDatetime", nameof(endDatetime));

            /* ---------------- INSTANCE CLASH CHECK ---------------- */
            var conflictMessage = await _schedulingClash.CheckForInstanceClashes(id, theatreId, start, end, staffs);

            if (!force && conflictMessage != null)
                throw new InstanceClashException(conflictMessage);


            const string sql = @"
                BEGIN TRANSACTION;

                UPDATE dbo.instance
                SET
                    session_id     = @SessionId,
                    theatre_id     = @TheatreId,
                    start_datetime = @StartDateTime,
                    end_datetime   = @EndDateTime
                WHERE id = @Id;

                IF @@ROWCOUNT = 0
                BEGIN
                    ROLLBACK;
                    RETURN;
                END

                DELETE isf
                FROM dbo.instance_staff isf
                LEFT JOIN @StaffIds s ON isf.staff_id = s.Id
                WHERE isf.instance_id = @Id
                AND s.Id IS NULL;

                INSERT INTO dbo.instance_staff (instance_id, staff_id)
                SELECT @Id, s.Id
                FROM @StaffIds s
                LEFT JOIN dbo.instance_staff isf
                    ON isf.instance_id = @Id
                AND isf.staff_id = s.Id
                WHERE isf.id IS NULL;

                COMMIT;
            ";

            var dt = DataTransformer.ToIntListTable(staffs);

            var parameters = new
            {
                Id = id,
                SessionId = sessionId,
                TheatreId = theatreId,
                StartDateTime = start,
                EndDateTime = end,
                StaffIds = dt.AsTableValuedParameter("dbo.IntList")
            };

            await ExecuteAsync(sql, parameters);

            return await GetInstanceDetailDTO(id);
        }

        public async Task<InstanceDetailDTO> CreateInstanceDetailDTO(
            int sessionId,
            int theatreId,
            string startDatetime,
            string endDatetime,
            List<int> staffs,
            bool force)
        {
            // Parse datetime
            if (!DateTime.TryParse(startDatetime, out var start))
                throw new ArgumentException("Invalid startDatetime", nameof(startDatetime));

            if (!DateTime.TryParse(endDatetime, out var end))
                throw new ArgumentException("Invalid endDatetime", nameof(endDatetime));

            // Check for instance time clashes
            var conflictMessage = await _schedulingClash.CheckForInstanceClashes(null, theatreId, start, end, staffs);

            if (!force && conflictMessage != null)
                throw new InstanceClashException(conflictMessage);

            // If no clashes or force=true, proceed with insert
            const string sql = @"
                BEGIN TRANSACTION;

                INSERT INTO dbo.instance (session_id, theatre_id, start_datetime, end_datetime)
                VALUES (@SessionId, @TheatreId, @StartDateTime, @EndDateTime);

                DECLARE @NewId INT = SCOPE_IDENTITY();

                INSERT INTO dbo.instance_staff (instance_id, staff_id)
                SELECT @NewId, s.Id
                FROM @StaffIds s;

                COMMIT;

                SELECT @NewId AS Id;
            ";

            var dt = DataTransformer.ToIntListTable(staffs);
            var parameters = new
            {
                SessionId = sessionId,
                TheatreId = theatreId,
                StartDateTime = start,
                EndDateTime = end,
                StaffIds = dt.AsTableValuedParameter("dbo.IntList")
            };

            var newId = (await QueryAsync<int>(sql, parameters)).First();

            return await GetInstanceDetailDTO(newId);
        }



        public async Task<IEnumerable<ListDTO>> GetList()
        {
            const string sql = @"
                SELECT
                    i.id AS Id,
                    f.name AS FacilityName,
                    t.name AS TheatreName,
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
                INNER JOIN dbo.theatre t ON i.theatre_id = t.id
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
            return await QueryAsync<ListDTO>(dynamicSql);
        }

    }
}
