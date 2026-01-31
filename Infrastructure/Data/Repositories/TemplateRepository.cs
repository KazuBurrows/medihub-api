using Dapper;
using MediHub.Domain.DTOs;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;
using MediHub.Infrastructure.Data.Utils;
using Microsoft.Extensions.Logging;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class TemplateRepository : BaseRepository, ITemplateRepository
    {
        private readonly ILogger<TemplateRepository> _logger;

        public TemplateRepository(
            SqlConnectionFactory connectionFactory,
            ILogger<TemplateRepository> logger
        ) : base(connectionFactory)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<TemplateMatrixDTO>> GetMatrix(
            int week,
            int? facilityId,
            int? theatreId)
        {
            const string sql = @"
                SELECT
                    st.id AS Id,
                    f.name AS FacilityName,
                    t.name AS TheatreName,
                    s.name AS SessionName,
                    s.is_pediatric AS IsPediatric,
                    s.is_acute AS IsAcute,
                    s.anaesthetic_type AS AnaestheticType,
                    CONCAT(stf.first_name, ' ', stf.last_name) AS SurgeonName,
                    (SELECT COUNT(*) 
                    FROM dbo.schedule_template_staff sts 
                    WHERE sts.schedule_template_id = st.id) AS StaffCount,
                    st.week AS Week,
                    st.day_of_week AS DayOfWeek,
                    st.start_time AS StartTime,
                    st.end_time AS EndTime,
                    st.is_active AS IsActive
                FROM dbo.schedule_template st
                INNER JOIN dbo.session s ON st.session_id = s.id
                INNER JOIN dbo.theatre t ON st.theatre_id = t.id
                INNER JOIN dbo.facility f ON t.facility_id = f.id
                LEFT JOIN dbo.staff stf ON s.surgeon_id = stf.id
                WHERE st.week = @Week
                /**FACILITY_FILTER**/
                /**THEATRE_FILTER**/
                ORDER BY st.day_of_week, st.start_time;
            ";

            // Build parameters dictionary
            var parameters = new Dictionary<string, object>
            {
                ["Week"] = week
            };

            // Apply dynamic filters
            var dynamicSql = sql;
            if (facilityId.HasValue)
            {
                dynamicSql = dynamicSql.Replace("/**FACILITY_FILTER**/", "AND f.id = @FacilityId");
                parameters["FacilityId"] = facilityId.Value;
            }
            else
            {
                dynamicSql = dynamicSql.Replace("/**FACILITY_FILTER**/", "");
            }

            if (theatreId.HasValue)
            {
                dynamicSql = dynamicSql.Replace("/**THEATRE_FILTER**/", "AND t.id = @TheatreId");
                parameters["TheatreId"] = theatreId.Value;
            }
            else
            {
                dynamicSql = dynamicSql.Replace("/**THEATRE_FILTER**/", "");
            }

            // Execute query using Dapper-style QueryAsync
            var result = (await QueryAsync<TemplateMatrixDTO>(dynamicSql, parameters)).ToList();

            return result;
        }




        public async Task<TemplateDetailDTO> GetTemplateDetailDTO(int templateId)
        {
            const string sql = @"
                SELECT
                    st.id AS Id,
                    st.session_id AS SessionId,
                    s.name AS SessionName,
                    s.is_acute AS IsAcute,
                    s.is_pediatric AS IsPediatric,
                    s.anaesthetic_type AS AnaestheticType,
                    CONCAT(stf.first_name, ' ', stf.last_name) AS SurgeonName,
                    sp.name AS SpecialtyName,
                    sub.name AS SubspecialtyName,
                    st.theatre_id AS TheatreId,
                    t.name AS TheatreName,
                    f.name AS FacilityName,
                    st.week AS Week,
                    st.day_of_week AS DayOfWeek,
                    st.start_time AS StartTime,
                    st.end_time AS EndTime,
                    st.is_active AS IsActive
                FROM dbo.schedule_template st
                INNER JOIN dbo.session s ON st.session_id = s.id
                INNER JOIN dbo.theatre t ON st.theatre_id = t.id
                INNER JOIN dbo.facility f ON t.facility_id = f.id
                LEFT JOIN dbo.staff stf ON s.surgeon_id = stf.id
                LEFT JOIN dbo.specialty sp ON s.specialty_id = sp.id
                LEFT JOIN dbo.subspecialty sub ON s.subspecialty_id = sub.id
                WHERE st.id = @TemplateId;
            ";

            var template = (await QueryAsync<TemplateDetailDTO>(sql, new { TemplateId = templateId }))
                            .FirstOrDefault();

            if (template == null)
                return null!; // or throw an exception

            // Fetch full staff info (without 'role')
            const string staffSql = @"
                SELECT
                    st.id,
                    st.first_name AS FirstName,
                    st.last_name AS LastName,
                    st.code AS Code
                FROM dbo.staff st
                INNER JOIN dbo.schedule_template_staff sts ON st.id = sts.staff_id
                WHERE sts.schedule_template_id = @TemplateId;
            ";

            var staffList = await QueryAsync<Staff>(staffSql, new { TemplateId = templateId });
            template.Staffs = staffList.ToArray();

            return template;
        }




        public async Task<TemplateMatrixFormatAgg> GetMatrixFormat(int facilityId)
        {
            const string sql = @"
                SELECT
                    f.id   AS FacilityId,
                    f.name AS FacilityName,
                    t.id   AS TheatreId,
                    t.name AS TheatreName,
                    0      AS TheatreSortOrder -- placeholder, no sort_order in schema yet
                FROM dbo.facility f
                LEFT JOIN dbo.theatre t ON t.facility_id = f.id
                WHERE (@FacilityId = 0 OR f.id = @FacilityId)
                ORDER BY
                    f.name,
                    t.name;
            ";

            // Parameters for SQL query
            var parameters = new { FacilityId = facilityId };

            // Execute the query into row DTOs
            var rows = await QueryAsync<TemplateMatrixFormatRow>(sql, parameters);

            // Group by facility and build nested structure
            var facilities = rows
                .GroupBy(r => new { r.FacilityId, r.FacilityName })
                .Select(fg => new TemplateFacilityFormat(
                    fg.Key.FacilityId,
                    fg.Key.FacilityName,
                    fg.Select(t => new TemplateTheatreFormat(
                        t.TheatreId,
                        t.TheatreName,
                        t.TheatreSortOrder
                    ))
                ))
                .ToList();

            return new TemplateMatrixFormatAgg(facilities);
        }


        public async Task<TemplateDetailDTO> CreateTemplateDetailDTO(
            int sessionId,
            int theatreId,
            int week,
            byte dayOfWeek,
            TimeSpan startTime,
            TimeSpan endTime,
            List<int> staffs)
        {
            const string sql = @"
                BEGIN TRANSACTION;

                -- Insert new template
                INSERT INTO dbo.schedule_template (session_id, theatre_id, week, day_of_week, start_time, end_time)
                VALUES (@SessionId, @TheatreId, @Week, @DayOfWeek, @StartTime, @EndTime);

                -- Get the new template ID
                DECLARE @NewId INT = SCOPE_IDENTITY();

                -- Insert staff relationships
                INSERT INTO dbo.schedule_template_staff (schedule_template_id, staff_id)
                SELECT @NewId, s.Id
                FROM @StaffIds s;

                COMMIT;

                -- Return the new template ID for retrieval
                SELECT @NewId AS Id;
            ";

            var dt = DataTransformer.ToIntListTable(staffs);

            var parameters = new
            {
                SessionId = sessionId,
                TheatreId = theatreId,
                Week = week,
                DayOfWeek = dayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                StaffIds = dt.AsTableValuedParameter("dbo.IntList")
            };

            var newId = (await QueryAsync<int>(sql, parameters)).First();

            return await GetTemplateDetailDTO(newId);
        }



        public async Task<TemplateDetailDTO> PutTemplateDetailDTO(
            int id,
            int sessionId,
            int theatreId,
            int week,
            byte dayOfWeek,
            TimeSpan startTime,
            TimeSpan endTime,
            List<int> staffs)
        {
            const string sql = @"
                BEGIN TRANSACTION;

                -- Update template
                UPDATE dbo.schedule_template
                SET
                    session_id = @SessionId,
                    theatre_id = @TheatreId,
                    week = @Week,
                    day_of_week = @DayOfWeek,
                    start_time = @StartTime,
                    end_time = @EndTime
                WHERE id = @Id;

                IF @@ROWCOUNT = 0
                BEGIN
                    ROLLBACK;
                    RETURN;
                END

                -- Delete removed staff
                DELETE sts
                FROM dbo.schedule_template_staff sts
                LEFT JOIN @StaffIds s ON sts.staff_id = s.Id
                WHERE sts.schedule_template_id = @Id
                AND s.Id IS NULL;

                -- Insert new staff
                INSERT INTO dbo.schedule_template_staff (schedule_template_id, staff_id)
                SELECT @Id, s.Id
                FROM @StaffIds s
                LEFT JOIN dbo.schedule_template_staff sts
                    ON sts.schedule_template_id = @Id
                    AND sts.staff_id = s.Id
                WHERE sts.id IS NULL;

                COMMIT;
            ";

            var dt = DataTransformer.ToIntListTable(staffs);

            var parameters = new
            {
                Id = id,
                SessionId = sessionId,
                TheatreId = theatreId,
                Week = week,
                DayOfWeek = dayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                StaffIds = dt.AsTableValuedParameter("dbo.IntList")
            };

            await ExecuteAsync(sql, parameters);

            return await GetTemplateDetailDTO(id);
        }


        public async Task<int> Delete(int id)
        {
            const string sql = @"
                DELETE FROM dbo.schedule_template_staff
                WHERE schedule_template_id = @Id;

                DELETE FROM dbo.schedule_template
                WHERE id = @Id;
            ";

            return await ExecuteAsync(sql, new { Id = id });
        }


        public async Task ApplyTemplate(DateOnly date)
        {
            _logger.LogInformation("=== ApplyTemplate STARTED === BaseDate: {Date}", date);

            const string sqlTemplate = @"
                BEGIN TRANSACTION;

                INSERT INTO dbo.instance (theatre_id, session_id, start_datetime, end_datetime)
                VALUES (@TheatreId, @SessionId, @StartDateTime, @EndDateTime);

                DECLARE @NewInstanceId INT = SCOPE_IDENTITY();

                -- 🔥 COPY STAFF FROM TEMPLATE TO INSTANCE
                INSERT INTO dbo.instance_staff (instance_id, staff_id)
                SELECT @NewInstanceId, sts.staff_id
                FROM dbo.schedule_template_staff sts
                WHERE sts.schedule_template_id = @TemplateId;

                COMMIT;
            ";

            try
            {
                // Loop through weeks 1 to 4
                for (int week = 1; week <= 4; week++)
                {
                    var weekStartDate = date.AddDays((week - 1) * 7);
                    _logger.LogInformation("Processing Week {Week} | WeekStartDate: {WeekStartDate}", week, weekStartDate);

                    var templates = (await QueryAsync<ScheduleTemplate>(@"
                        SELECT 
                            id,
                            session_id  AS SessionId,
                            theatre_id  AS TheatreId,
                            week,
                            day_of_week AS DayOfWeek,
                            start_time  AS StartTime,
                            end_time    AS EndTime
                        FROM dbo.schedule_template
                        WHERE week = @Week AND is_active = 1
                    ", new { Week = week })).ToList();


                    _logger.LogInformation("Week {Week} → {TemplateCount} templates found", week, templates.Count);

                    if (!templates.Any())
                    {
                        _logger.LogWarning("No active templates found for week {Week}", week);
                        continue;
                    }

                    foreach (var t in templates)
                    {
                        var startDateTime = weekStartDate.ToDateTime(TimeOnly.MinValue)
                                                        .Add(t.StartTime)
                                                        .AddDays(t.DayOfWeek - 1);

                        var endDateTime = weekStartDate.ToDateTime(TimeOnly.MinValue)
                                                    .Add(t.EndTime)
                                                    .AddDays(t.DayOfWeek - 1);

                        _logger.LogInformation(
                            "Creating Instance | TemplateId: {TemplateId} | Theatre: {TheatreId} | Session: {SessionId} | Start: {Start} | End: {End}",
                            t.Id, t.TheatreId, t.SessionId, startDateTime, endDateTime);

                        try
                        {
                            await QueryAsync<int>(sqlTemplate, new
                            {
                                TemplateId = t.Id,
                                SessionId = t.SessionId,
                                TheatreId = t.TheatreId,
                                StartDateTime = startDateTime,
                                EndDateTime = endDateTime
                            });

                            _logger.LogInformation("Instance created successfully for TemplateId {TemplateId}", t.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex,
                                "FAILED to create instance | TemplateId: {TemplateId} | Week: {Week}",
                                t.Id, week);
                        }
                    }
                }

                _logger.LogInformation("=== ApplyTemplate COMPLETED SUCCESSFULLY ===");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "ApplyTemplate CRASHED for BaseDate {Date}", date);
                throw; // important — let Azure know it failed
            }
        }




    }
}