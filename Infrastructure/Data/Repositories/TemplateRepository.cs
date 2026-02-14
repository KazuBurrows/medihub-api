using Dapper;
using MediHub.Domain.DTOs;
using MediHub.Domain.Models;
using MediHub.Functions.Helpers.Exceptions;
using MediHub.Infrastructure.Data.Helpers;
using MediHub.Infrastructure.Data.Interfaces;
using MediHub.Infrastructure.Data.Utils;
using Microsoft.Extensions.Logging;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class TemplateRepository : BaseRepository, ITemplateRepository
    {
        private readonly ILogger<TemplateRepository> _logger;
        private readonly SchedulingClash _schedulingClash;

        public TemplateRepository(
            SqlConnectionFactory connectionFactory,
            ILogger<TemplateRepository> logger,
            SchedulingClash schedulingClash
        ) : base(connectionFactory)
        {
            _logger = logger;
            _schedulingClash = schedulingClash;
        }

        public async Task<IEnumerable<TemplateScheduleDTO>> GetMatrix(
            int week,
            int? facilityId,
            int? assetId)
        {
            const string sql = @"
                SELECT
                    st.id AS Id,
                    f.name AS FacilityName,
                    t.name AS AssetName,
                    s.name AS SessionName,
                    s.is_pediatric AS IsPediatric,
                    s.is_acute AS IsAcute,
                    s.anaesthetic_type AS AnaestheticType,
                    CONCAT(stf.first_name, ' ', stf.last_name) AS SurgeonName,
                    st.week AS Week,
                    st.day_of_week AS DayOfWeek,
                    st.start_time AS StartTime,
                    st.end_time AS EndTime,
                    st.is_open AS IsOpen
                FROM dbo.instance_template st
                INNER JOIN dbo.session s ON st.session_id = s.id
                INNER JOIN dbo.asset t ON st.asset_id = t.id
                INNER JOIN dbo.facility f ON t.facility_id = f.id
                LEFT JOIN dbo.staff stf ON s.surgeon_id = stf.id
                WHERE st.week = @Week
                /**FACILITY_FILTER**/
                /**ASSET_FILTER**/
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

            if (assetId.HasValue)
            {
                dynamicSql = dynamicSql.Replace("/**ASSET_FILTER**/", "AND t.id = @AssetId");
                parameters["AssetId"] = assetId.Value;
            }
            else
            {
                dynamicSql = dynamicSql.Replace("/**ASSET_FILTER**/", "");
            }

            // Execute query using Dapper-style QueryAsync
            var result = (await QueryAsync<TemplateScheduleDTO>(dynamicSql, parameters)).ToList();

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
                    st.asset_id AS AssetId,
                    t.name AS AssetName,
                    f.name AS FacilityName,
                    st.week AS Week,
                    st.day_of_week AS DayOfWeek,
                    st.start_time AS StartTime,
                    st.end_time AS EndTime,
                    st.is_open AS IsOpen
                FROM dbo.instance_template st
                INNER JOIN dbo.session s ON st.session_id = s.id
                INNER JOIN dbo.asset t ON st.asset_id = t.id
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

            return template;
        }




        public async Task<TemplateMatrixFormatAgg> GetMatrixFormat(int facilityId)
        {
            const string sql = @"
                SELECT
                    f.id   AS FacilityId,
                    f.name AS FacilityName,
                    t.id   AS AssetId,
                    t.name AS AssetName,
                    0      AS AssetSortOrder -- placeholder, no sort_order in schema yet
                FROM dbo.facility f
                LEFT JOIN dbo.asset t ON t.facility_id = f.id
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
                    fg.Select(t => new TemplateAssetFormat(
                        t.AssetId,
                        t.AssetName,
                        t.AssetSortOrder
                    ))
                ))
                .ToList();

            return new TemplateMatrixFormatAgg(facilities);
        }


        public async Task<TemplateDetailDTO> CreateTemplateDetailDTO(
            int sessionId,
            int assetId,
            int week,
            byte dayOfWeek,
            TimeSpan startTime,
            TimeSpan endTime,
            bool force)
        {
            /* ---------------- TEMPLATE CLASH CHECK ---------------- */

            // Convert to DateTime purely so we can reuse the same overlap logic style
            var baseDate = DateTime.Today;
            int diff = (7 + (dayOfWeek - (int)baseDate.DayOfWeek)) % 7;
            var templateDate = baseDate.AddDays(diff);

            var start = templateDate.Date + startTime;
            var end = templateDate.Date + endTime;

            var conflictMessage = await _schedulingClash.CheckForTemplateClashes(
                null,          // null because this is CREATE
                assetId,
                week,
                dayOfWeek,
                start,
                end
            );

            if (!force && conflictMessage != null)
                throw new TemplateClashException(conflictMessage);

            const string sql = @"
                BEGIN TRANSACTION;

                -- Insert new template
                INSERT INTO dbo.instance_template (session_id, asset_id, week, day_of_week, start_time, end_time)
                VALUES (@SessionId, @AssetId, @Week, @DayOfWeek, @StartTime, @EndTime);

                -- Get the new template ID
                DECLARE @NewId INT = SCOPE_IDENTITY();

                COMMIT;

                -- Return the new template ID for retrieval
                SELECT @NewId AS Id;
            ";

            var parameters = new
            {
                SessionId = sessionId,
                AssetId = assetId,
                Week = week,
                DayOfWeek = dayOfWeek,
                StartTime = startTime,
                EndTime = endTime
            };

            var newId = (await QueryAsync<int>(sql, parameters)).First();

            return await GetTemplateDetailDTO(newId);
        }



        public async Task<TemplateDetailDTO> PutTemplateDetailDTO(
            int id,
            int sessionId,
            int assetId,
            int week,
            byte dayOfWeek,
            TimeSpan startTime,
            TimeSpan endTime,
            bool force)
        {

            /* ---------------- TEMPLATE CLASH CHECK ---------------- */

            // Convert template slot into a comparable DateTime range
            // (Use any fixed base date — only time + day alignment matters)
            var baseDate = DateTime.Today;

            // Align base date to requested DayOfWeek
            int diff = (7 + (dayOfWeek - (int)baseDate.DayOfWeek)) % 7;
            var templateDate = baseDate.AddDays(diff);

            var start = templateDate.Date + startTime;
            var end = templateDate.Date + endTime;

            var conflictMessage = await _schedulingClash.CheckForTemplateClashes(
                id,
                assetId,
                week,
                dayOfWeek,
                start,
                end
            );

            if (!force && conflictMessage != null)
                throw new TemplateClashException(conflictMessage);

            const string sql = @"
                BEGIN TRANSACTION;

                -- Update template
                UPDATE dbo.instance_template
                SET
                    session_id = @SessionId,
                    asset_id = @AssetId,
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

                COMMIT;
            ";


            var parameters = new
            {
                Id = id,
                SessionId = sessionId,
                AssetId = assetId,
                Week = week,
                DayOfWeek = dayOfWeek,
                StartTime = startTime,
                EndTime = endTime
            };

            await ExecuteAsync(sql, parameters);

            return await GetTemplateDetailDTO(id);
        }


        public async Task<int> Delete(int id)
        {
            const string sql = @"
                DELETE FROM dbo.instance_template
                WHERE id = @Id;
            ";

            return await ExecuteAsync(sql, new { Id = id });
        }

        

        public async Task<string> ApplyTemplate(DateOnly date, bool force)
        {
            _logger.LogInformation("=== ApplyTemplate STARTED === BaseDate: {Date}", date);

            const string sqlTemplate = @"
                BEGIN TRANSACTION;

                INSERT INTO dbo.instance (asset_id, session_id, start_datetime, end_datetime)
                VALUES (@AssetId, @SessionId, @StartDateTime, @EndDateTime);

                DECLARE @NewInstanceId INT = SCOPE_IDENTITY();

                COMMIT;
            ";

            

            try
            {
                // Loop through weeks 1 to 4
                for (int week = 1; week <= 4; week++)
                {
                    var weekStartDate = date.AddDays((week - 1) * 7);
                    _logger.LogInformation("Processing Week {Week} | WeekStartDate: {WeekStartDate}", week, weekStartDate);

                    var templates = (await QueryAsync<InstanceTemplate>(@"
                        SELECT 
                            id,
                            session_id  AS SessionId,
                            asset_id  AS AssetId,
                            week,
                            day_of_week AS DayOfWeek,
                            start_time  AS StartTime,
                            end_time    AS EndTime
                        FROM dbo.instance_template
                        WHERE week = @Week AND is_open = 1
                    ", new { Week = week })).ToList();

                    if (!templates.Any())
                    {
                        _logger.LogWarning("No active templates found for week {Week}", week);
                        continue;
                    }

                    

                    // ---- Insert loop ----
                    foreach (var t in templates)
                    {
                        var startDateTime = weekStartDate.ToDateTime(TimeOnly.MinValue)
                                                        .Add(t.StartTime)
                                                        .AddDays(t.DayOfWeek - 1);
                        var endDateTime = weekStartDate.ToDateTime(TimeOnly.MinValue)
                                                      .Add(t.EndTime)
                                                      .AddDays(t.DayOfWeek - 1);

                        await QueryAsync<int>(sqlTemplate, new
                        {
                            TemplateId = t.Id,
                            SessionId = t.SessionId,
                            AssetId = t.AssetId,
                            StartDateTime = startDateTime,
                            EndDateTime = endDateTime
                        });
                    }

                }

                return $"ApplyTemplate completed successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "ApplyTemplate CRASHED for BaseDate {Date}", date);
                throw; // important — let Azure know it failed
            }
        }

        public async Task<IEnumerable<TemplateDetailDTO>> GetAllDTO()
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
                    se.is_acute AS IsAcute,
                    se.is_pediatric AS IsPediatric,
                    se.anaesthetic_type AS AnaestheticType,
                    (st.first_name + ' ' + st.last_name) AS SurgeonName,
                    sp.name AS SpecialtyName,
                    ssp.name AS SubspecialtyName,
                    s.week AS Week,
                    s.day_of_week AS DayOfWeek,
                    s.start_time AS StartTime,
                    s.end_time AS EndTime,
                    s.is_open AS IsOpen
                FROM dbo.instance_template s
                LEFT JOIN dbo.asset t ON s.asset_id = t.id
                LEFT JOIN dbo.facility f ON t.facility_id = f.id
                LEFT JOIN dbo.session se ON s.session_id = se.id
                LEFT JOIN dbo.staff st ON se.surgeon_id = st.id
                LEFT JOIN dbo.specialty sp ON se.specialty_id = sp.id
                LEFT JOIN dbo.subspecialty ssp ON se.subspecialty_id = ssp.id
            ";

            return await QueryAsync<TemplateDetailDTO>(sql);
        }

    }
}