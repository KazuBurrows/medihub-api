using Dapper;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Domain.DTOs;
using MediHub.Domain.Models;
using MediHub.Functions.Helpers.Exceptions;
using MediHub.Infrastructure.Data.Interfaces;
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


        public async Task<TemplateMatrixFormatAgg> GetMatrixFormat(int facilityId)
        {
            const string sql = @"
                SELECT
                    f.FACILITY_KEY   AS FacilityId,
                    f.FACILITY_NAME  AS FacilityName,

                    a.ASSET_KEY      AS AssetId,
                    a.ASSET_CODE     AS AssetName,

                    0 AS AssetSortOrder -- placeholder until column exists

                FROM FACILITY f

                LEFT JOIN ASSET a
                    ON a.ASSET_FACILITY_KEY = f.FACILITY_KEY

                WHERE (@FacilityId = 0 OR f.FACILITY_KEY = @FacilityId)

                ORDER BY
                    f.FACILITY_NAME,
                    a.ASSET_CODE;
            ";

            var parameters = new { FacilityId = facilityId };

            var rows = await QueryAsync<TemplateMatrixFormatRow>(sql, parameters);

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


        public async Task<TemplateDTO> CreateTemplateDTO(
            int sessionId,
            int assetId,
            int week,
            int dayOfWeek,
            TimeSpan startTime,
            TimeSpan endTime,
            bool isOpen,
            bool force,
            int versionId)
        {
            var conflicts = await QueryAsync<TemplateDTO>(
                @"
                SELECT 
                    t.INSTANCE_TEMPLATE_KEY AS Id,
                    t.INSTANCE_TEMPLATE_ASSET_KEY AS AssetId,
                    t.INSTANCE_TEMPLATE_CYCLE_WEEK AS CycleWeek,
                    t.INSTANCE_TEMPLATE_CYCLE_DAY AS CycleDay,
                    t.INSTANCE_TEMPLATE_START_TIME AS StartTime,
                    t.INSTANCE_TEMPLATE_END_TIME AS EndTime,
                    t.INSTANCE_TEMPLATE_VERSION_KEY AS VersionId
                FROM dbo.fnCheckInstanceTemplateConflict(
                    @AssetKey,
                    @CycleWeek,
                    @CycleDay,
                    @StartTime,
                    @EndTime,
                    @IgnoreKey,
                    @VersionId
                ) t",
                new {
                    AssetKey = assetId,
                    CycleWeek = week,
                    CycleDay = dayOfWeek,
                    StartTime = startTime,
                    EndTime = endTime,
                    IgnoreKey = (int?)null,
                    VersionId = versionId
                }
            );

            if (conflicts.Any())
            {
                _logger.LogInformation("Conflicts count: {Count}", conflicts.Count());

                List<int> ids = conflicts.Select(c => c.Id).ToList();
                // 2️⃣ Throw a controlled exception
                throw new ConflictException(ids);
            }

            const string sql = @"
                INSERT INTO dbo.instance_template
                (
                    INSTANCE_TEMPLATE_SESSION_KEY,
                    INSTANCE_TEMPLATE_ASSET_KEY,
                    INSTANCE_TEMPLATE_CYCLE_WEEK,
                    INSTANCE_TEMPLATE_CYCLE_DAY,
                    INSTANCE_TEMPLATE_START_TIME,
                    INSTANCE_TEMPLATE_END_TIME,
                    INSTANCE_TEMPLATE_IS_OPEN,
                    INSTANCE_TEMPLATE_VERSION_KEY
                )
                VALUES
                (
                    @SessionId,
                    @AssetId,
                    @Week,
                    @DayOfWeek,
                    @StartTime,
                    @EndTime,
                    @IsOpen,
                    @VersionId
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            var newId = (await QueryAsync<int>(sql, new
            {
                SessionId = sessionId,
                AssetId = assetId,
                Week = week,
                DayOfWeek = dayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                IsOpen = isOpen,
                VersionId = versionId
            })).First();

            return await GetByIdDTO(newId);
        }



        public async Task<TemplateDTO> PutTemplateDTO(
            int id,
            int sessionId,
            int assetId,
            int week,
            int dayOfWeek,
            TimeSpan startTime,
            TimeSpan endTime,
            bool isOpen,
            bool force,
            int versionId)
        {

            var conflicts = await QueryAsync<TemplateDTO>(
                @"
                SELECT 
                    t.INSTANCE_TEMPLATE_KEY             AS Id,
                    t.INSTANCE_TEMPLATE_ASSET_KEY       AS AssetId,
                    t.INSTANCE_TEMPLATE_CYCLE_WEEK      AS CycleWeek,
                    t.INSTANCE_TEMPLATE_CYCLE_DAY       AS CycleDay,
                    t.INSTANCE_TEMPLATE_START_TIME      AS StartTime,
                    t.INSTANCE_TEMPLATE_END_TIME        AS EndTime,
                    t.INSTANCE_TEMPLATE_VERSION_KEY     AS VersionId
                FROM dbo.fnCheckInstanceTemplateConflict(
                    @AssetKey,
                    @CycleWeek,
                    @CycleDay,
                    @StartTime,
                    @EndTime,
                    @IgnoreKey,
                    @VersionId
                ) t",
                new {
                    AssetKey = assetId,
                    CycleWeek = week,
                    CycleDay = dayOfWeek,
                    StartTime = startTime,
                    EndTime = endTime,
                    IgnoreKey = id,
                    VersionId = versionId
                }
            );

            if (conflicts.Any())
            {
                _logger.LogInformation("Conflicts count: {Count}", conflicts.Count());

                List<int> ids = conflicts.Select(c => c.Id).ToList();
                // 2️⃣ Throw a controlled exception
                throw new ConflictException(ids);
            }

            const string sql = @"
                BEGIN TRANSACTION;

                UPDATE dbo.instance_template
                SET
                    INSTANCE_TEMPLATE_SESSION_KEY = @SessionId,
                    INSTANCE_TEMPLATE_ASSET_KEY = @AssetId,
                    INSTANCE_TEMPLATE_CYCLE_WEEK = @Week,
                    INSTANCE_TEMPLATE_CYCLE_DAY = @DayOfWeek,
                    INSTANCE_TEMPLATE_START_TIME = @StartTime,
                    INSTANCE_TEMPLATE_END_TIME = @EndTime,
                    INSTANCE_TEMPLATE_IS_OPEN = @IsOpen,
                    INSTANCE_TEMPLATE_LAST_UPDATED_DATETIME = SYSUTCDATETIME(),
                    INSTANCE_TEMPLATE_VERSION_KEY = @VersionId
                WHERE INSTANCE_TEMPLATE_KEY = @Id;

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
                EndTime = endTime,
                IsOpen = isOpen,
                VersionId = versionId
            };

            await ExecuteAsync(sql, parameters);

            return await GetByIdDTO(id);
        }


        public async Task Delete(int id)
        {
            const string sql = @"
                DELETE FROM dbo.instance_template
                WHERE INSTANCE_TEMPLATE_KEY = @Id;
            ";

            var rowsAffected = await ExecuteAsync(sql, new { Id = id });

            if (rowsAffected == 0)
            {
                throw new NotFoundException($"No item found with ID {id}.");
            }
        }



        public async Task<string> ApplyTemplate(DateOnly date, int cycleWeek)
        {
            _logger.LogInformation("=== ApplyTemplate STARTED === BaseDate: {Date}", date);

            const string sqlTemplate = @"
                INSERT INTO dbo.instance 
                    (INSTANCE_ASSET_KEY, INSTANCE_SESSION_KEY, INSTANCE_START_DATETIME, INSTANCE_END_DATETIME, INSTANCE_TEMPLATE_IS_OPEN, INSTANCE_TEMPLATE_VERSION_KEY)
                VALUES (@AssetId, @SessionId, @StartDateTime, @EndDateTime, @IsOpen, @VersionId);
            ";

            try
            {

                    var weekStartDate = date.AddDays((cycleWeek - 1) * 7);
                    _logger.LogInformation("Processing Week {Week} | WeekStartDate: {WeekStartDate}", cycleWeek, weekStartDate);

                    var templates = (await QueryAsync<Template>(@"
                        SELECT 
                            INSTANCE_TEMPLATE_KEY           AS Id,
                            INSTANCE_TEMPLATE_SESSION_KEY   AS SessionId,
                            INSTANCE_TEMPLATE_ASSET_KEY     AS AssetId,
                            INSTANCE_TEMPLATE_CYCLE_WEEK    AS CycleWeek,
                            INSTANCE_TEMPLATE_CYCLE_DAY     AS CycleDay,
                            INSTANCE_TEMPLATE_START_TIME    AS StartTime,
                            INSTANCE_TEMPLATE_END_TIME      AS EndTime,
                            INSTANCE_TEMPLATE_IS_OPEN       AS IsOpen,
                            INSTANCE_TEMPLATE_VERSION_KEY   AS VersionId
                        FROM dbo.instance_template
                        WHERE INSTANCE_TEMPLATE_CYCLE_WEEK = @CycleWeek
                        AND INSTANCE_TEMPLATE_IS_OPEN = 1
                    ", new { CycleWeek = cycleWeek })).ToList();

                    if (!templates.Any())
                    {
                        _logger.LogWarning("No active templates found for week {Week}", cycleWeek);
                    }

                    foreach (var t in templates)
                    {
                        if (!t.StartTime.HasValue || !t.EndTime.HasValue || !t.CycleDay.HasValue || !t.AssetId.HasValue)
                        {
                            _logger.LogWarning(
                                "Template {TemplateId} skipped due to missing data (StartTime: {StartTime}, EndTime: {EndTime}, CycleDay: {CycleDay}, AssetId: {AssetId})",
                                t.Id, t.StartTime, t.EndTime, t.CycleDay, t.AssetId
                            );
                            continue;
                        }

                        var startDateTime = weekStartDate.ToDateTime(TimeOnly.MinValue)
                                                .AddDays(t.CycleDay.Value - 1)
                                                .Add(t.StartTime.Value);

                        var endDateTime = weekStartDate.ToDateTime(TimeOnly.MinValue)
                                                .AddDays(t.CycleDay.Value - 1)
                                                .Add(t.EndTime.Value);


                        await ExecuteAsync(sqlTemplate, new
                        {
                            SessionId = t.SessionId,
                            AssetId = t.AssetId,
                            StartDateTime = startDateTime,
                            EndDateTime = endDateTime,
                            IsOpen = t.IsOpen,
                            VersionId = t.VersionId
                        });
                    }

                return $"ApplyTemplate completed successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "ApplyTemplate CRASHED for BaseDate {Date}", date);
                throw;
            }
        }



        public async Task<IEnumerable<TemplateDTO>> GetAllDTO()
        {
            const string sql = @"
                SELECT
                    -- Instance template columns
                    it.INSTANCE_TEMPLATE_KEY          AS Id,
                    it.INSTANCE_TEMPLATE_SESSION_KEY  AS SessionId,
                    it.INSTANCE_TEMPLATE_ASSET_KEY    AS AssetId,
                    it.INSTANCE_TEMPLATE_CYCLE_WEEK   AS CycleWeek,
                    it.INSTANCE_TEMPLATE_CYCLE_DAY    AS CycleDay,
                    it.INSTANCE_TEMPLATE_IS_OPEN      AS IsOpen,
                    it.INSTANCE_TEMPLATE_START_TIME   AS StartTime,
                    it.INSTANCE_TEMPLATE_END_TIME     AS EndTime,
                    it.INSTANCE_TEMPLATE_LAST_UPDATED_DATETIME AS LastUpdatedDatetime,
                    it.INSTANCE_TEMPLATE_LAST_UPDATED_USER_KEY AS LastUpdatedByUserId,
                    it.INSTANCE_TEMPLATE_VERSION_KEY AS VersionId,

                    -- Asset columns
                    a.ASSET_CODE                      AS AssetCode,
                    a.ASSET_DESCRIPTION               AS AssetDescription,
                    a.ASSET_LOCATION                  AS AssetLocation,
                    a.ASSET_FACILITY_KEY               AS FacilityId,

                    -- Facility columns
                    f.FACILITY_NAME                    AS FacilityName,

                    -- Session columns
                    se.SESSION_TITLE                   AS SessionTitle,
                    se.SESSION_IS_ACUTE                AS SessionIsAcute,
                    se.SESSION_IS_PAEDIATRIC           AS SessionIsPaediatric,
                    se.SESSION_ANAESTHETIC_TYPE_KEY       AS AnaestheticTypeId,
                    at.ANAESTHETIC_TYPE_CODE           AS AnaestheticTypeCode,
                    at.ANAESTHETIC_TYPE_DESCRIPTION    AS AnaestheticTypeDescription,
                    se.SESSION_SURGEON_KEY            AS SurgeonId,
                    se.SESSION_SPECIALTY_KEY           AS SpecialtyId,
                    se.SESSION_SUBSPECIALTY_KEY        AS SubspecialtyId,

                    v.VERSION_NAME                  AS VersionName,
                    v.VERSION_DESCRIPTION           AS VersionDescription,
                    v.VERSION_IS_ACTIVE             AS VersionIsActive,

                    -- Surgeon columns (staff)
                    s.STAFF_NAME AS SurgeonName,

                    -- Specialty
                    sp.SPECIALTY_CODE                  AS SpecialtyCode,
                    sp.SPECIALTY_DESCRIPTION           AS SpecialtyDescription,

                    -- Subspecialty
                    ss.SUBSPECIALTY_NAME               AS SubspecialtyName,

                    -- Last updated staff info
                    stf.STAFF_ID                       AS LastUpdatedByUserCode,
                    stf.STAFF_NAME                     AS LastUpdatedByUserName

                FROM dbo.instance_template it
                    LEFT JOIN dbo.asset a ON it.INSTANCE_TEMPLATE_ASSET_KEY = a.ASSET_KEY
                    LEFT JOIN dbo.facility f ON a.ASSET_FACILITY_KEY = f.FACILITY_KEY
                    LEFT JOIN dbo.session se ON it.INSTANCE_TEMPLATE_SESSION_KEY = se.SESSION_KEY
                    LEFT JOIN dbo.staff s ON se.SESSION_SURGEON_KEY = s.STAFF_KEY
                    LEFT JOIN dbo.specialty sp ON se.SESSION_SPECIALTY_KEY = sp.SPECIALTY_KEY
                    LEFT JOIN dbo.subspecialty ss ON se.SESSION_SUBSPECIALTY_KEY = ss.SUBSPECIALTY_KEY
                    LEFT JOIN dbo.staff stf ON it.INSTANCE_TEMPLATE_LAST_UPDATED_USER_KEY = stf.STAFF_KEY
                    LEFT JOIN dbo.anaesthetic_type at ON at.ANAESTHETIC_TYPE_KEY = se.SESSION_ANAESTHETIC_TYPE_KEY
                    LEFT JOIN dbo.version v ON it.INSTANCE_TEMPLATE_VERSION_KEY = v.VERSION_KEY
                ORDER BY it.INSTANCE_TEMPLATE_KEY;
            ";

            return await QueryAsync<TemplateDTO>(sql);
        }


        public async Task<IEnumerable<TemplateDTO>> GetAllDTOByWeek(int week, int versionId)
        {
            const string sql = @"
                SELECT
                    -- Instance template columns
                    it.INSTANCE_TEMPLATE_KEY          AS Id,
                    it.INSTANCE_TEMPLATE_SESSION_KEY  AS SessionId,
                    it.INSTANCE_TEMPLATE_ASSET_KEY    AS AssetId,
                    it.INSTANCE_TEMPLATE_CYCLE_WEEK   AS CycleWeek,
                    it.INSTANCE_TEMPLATE_CYCLE_DAY    AS CycleDay,
                    it.INSTANCE_TEMPLATE_IS_OPEN      AS IsOpen,
                    it.INSTANCE_TEMPLATE_START_TIME   AS StartTime,
                    it.INSTANCE_TEMPLATE_END_TIME     AS EndTime,
                    it.INSTANCE_TEMPLATE_LAST_UPDATED_DATETIME AS LastUpdatedDatetime,
                    it.INSTANCE_TEMPLATE_LAST_UPDATED_USER_KEY AS LastUpdatedByUserId,
                    it.INSTANCE_TEMPLATE_VERSION_KEY AS VersionId,

                    -- Asset columns
                    a.ASSET_CODE                      AS AssetCode,
                    a.ASSET_DESCRIPTION               AS AssetDescription,
                    a.ASSET_LOCATION                  AS AssetLocation,
                    a.ASSET_FACILITY_KEY              AS FacilityId,

                    -- Facility columns
                    f.FACILITY_NAME                    AS FacilityName,

                    -- Session columns
                    se.SESSION_TITLE                   AS SessionTitle,
                    se.SESSION_IS_ACUTE                AS SessionIsAcute,
                    se.SESSION_IS_PAEDIATRIC           AS SessionIsPaediatric,
                    se.SESSION_ANAESTHETIC_TYPE_KEY       AS AnaestheticTypeId,
                    at.ANAESTHETIC_TYPE_CODE           AS AnaestheticTypeCode,
                    at.ANAESTHETIC_TYPE_DESCRIPTION    AS AnaestheticTypeDescription,
                    se.SESSION_SURGEON_KEY            AS SurgeonId,
                    se.SESSION_SPECIALTY_KEY           AS SpecialtyId,
                    se.SESSION_SUBSPECIALTY_KEY        AS SubspecialtyId,

                    v.VERSION_NAME                  AS VersionName,
                    v.VERSION_DESCRIPTION           AS VersionDescription,
                    v.VERSION_IS_ACTIVE             AS VersionIsActive,

                    -- Surgeon columns (staff)
                    s.STAFF_NAME AS SurgeonName,

                    -- Specialty
                    sp.SPECIALTY_CODE                  AS SpecialtyCode,
                    sp.SPECIALTY_DESCRIPTION           AS SpecialtyDescription,

                    -- Subspecialty
                    ss.SUBSPECIALTY_NAME               AS SubspecialtyName,

                    -- Last updated staff info
                    stf.STAFF_ID                       AS LastUpdatedByUserCode,
                    stf.STAFF_NAME                     AS LastUpdatedByUserName

                FROM dbo.instance_template it
                    LEFT JOIN dbo.asset a ON it.INSTANCE_TEMPLATE_ASSET_KEY = a.ASSET_KEY
                    LEFT JOIN dbo.facility f ON a.ASSET_FACILITY_KEY = f.FACILITY_KEY
                    LEFT JOIN dbo.session se ON it.INSTANCE_TEMPLATE_SESSION_KEY = se.SESSION_KEY
                    LEFT JOIN dbo.staff s ON se.SESSION_SURGEON_KEY = s.STAFF_KEY
                    LEFT JOIN dbo.specialty sp ON se.SESSION_SPECIALTY_KEY = sp.SPECIALTY_KEY
                    LEFT JOIN dbo.subspecialty ss ON se.SESSION_SUBSPECIALTY_KEY = ss.SUBSPECIALTY_KEY
                    LEFT JOIN dbo.staff stf ON it.INSTANCE_TEMPLATE_LAST_UPDATED_USER_KEY = stf.STAFF_KEY
                    LEFT JOIN dbo.anaesthetic_type at ON at.ANAESTHETIC_TYPE_KEY = se.SESSION_ANAESTHETIC_TYPE_KEY
                    LEFT JOIN dbo.version v ON it.INSTANCE_TEMPLATE_VERSION_KEY = v.VERSION_KEY
                WHERE it.INSTANCE_TEMPLATE_CYCLE_WEEK = @Week
                AND it.INSTANCE_TEMPLATE_VERSION_KEY = @VersionId
                ORDER BY it.INSTANCE_TEMPLATE_KEY;
            ";

            return await QueryAsync<TemplateDTO>(sql, new { Week = week, VersionId = versionId });
        }

        public async Task<Template?> GetById(int id)
        {
            const string sql = @"
                SELECT
                    INSTANCE_TEMPLATE_KEY       AS Id,
                    INSTANCE_TEMPLATE_SESSION_KEY                   AS SessionId,
                    INSTANCE_TEMPLATE_ASSET_KEY                     AS AssetId,
                    INSTANCE_TEMPLATE_CYCLE_WEEK                    AS CycleWeek,
                    INSTANCE_TEMPLATE_CYCLE_DAY                     AS CycleDay,
                    INSTANCE_TEMPLATE_IS_OPEN                       AS IsOpen,
                    INSTANCE_TEMPLATE_START_TIME                    AS StartTime,
                    INSTANCE_TEMPLATE_END_TIME                      AS EndTime,
                    INSTANCE_TEMPLATE_LAST_UPDATED_DATETIME         AS LastUpdatedDatetime,
                    INSTANCE_TEMPLATE_LAST_UPDATED_USER_KEY         AS LastUpdatedByUserId,
                    INSTANCE_TEMPLATE_VERSION_KEY                   AS VersionId
                FROM dbo.instance_template
                WHERE INSTANCE_TEMPLATE_KEY = @Id;
            ";

            return await QuerySingleOrDefaultAsync<Template>(sql, new { Id = id });
        }

        public async Task<TemplateDTO?> GetByIdDTO(int id)
        {
            const string sql = @"
                SELECT
                    -- Instance template columns
                    it.INSTANCE_TEMPLATE_KEY          AS Id,
                    it.INSTANCE_TEMPLATE_SESSION_KEY  AS SessionId,
                    it.INSTANCE_TEMPLATE_ASSET_KEY    AS AssetId,
                    it.INSTANCE_TEMPLATE_CYCLE_WEEK   AS CycleWeek,
                    it.INSTANCE_TEMPLATE_CYCLE_DAY    AS CycleDay,
                    it.INSTANCE_TEMPLATE_IS_OPEN      AS IsOpen,
                    it.INSTANCE_TEMPLATE_START_TIME   AS StartTime,
                    it.INSTANCE_TEMPLATE_END_TIME     AS EndTime,
                    it.INSTANCE_TEMPLATE_LAST_UPDATED_DATETIME AS LastUpdatedDatetime,
                    it.INSTANCE_TEMPLATE_LAST_UPDATED_USER_KEY AS LastUpdatedByUserId,
                    it.INSTANCE_TEMPLATE_VERSION_KEY AS VersionId,

                    -- Asset columns
                    a.ASSET_CODE                      AS AssetCode,
                    a.ASSET_DESCRIPTION               AS AssetDescription,
                    a.ASSET_LOCATION                  AS AssetLocation,
                    a.ASSET_FACILITY_KEY               AS FacilityId,

                    -- Facility columns
                    f.FACILITY_NAME                    AS FacilityName,

                    -- Session columns
                    se.SESSION_TITLE                   AS SessionTitle,
                    se.SESSION_IS_ACUTE                AS SessionIsAcute,
                    se.SESSION_IS_PAEDIATRIC           AS SessionIsPaediatric,
                    se.SESSION_ANAESTHETIC_TYPE_KEY       AS AnaestheticTypeId,
                    at.ANAESTHETIC_TYPE_CODE           AS AnaestheticTypeCode,
                    at.ANAESTHETIC_TYPE_DESCRIPTION    AS AnaestheticTypeDescription,
                    se.SESSION_SURGEON_KEY            AS SurgeonId,
                    se.SESSION_SPECIALTY_KEY           AS SpecialtyId,
                    se.SESSION_SUBSPECIALTY_KEY        AS SubspecialtyId,

                    v.VERSION_NAME                  AS VersionName,
                    v.VERSION_DESCRIPTION           AS VersionDescription,
                    v.VERSION_IS_ACTIVE             AS VersionIsActive,

                    -- Surgeon columns (staff)
                    s.STAFF_NAME AS SurgeonName,

                    -- Specialty
                    sp.SPECIALTY_CODE                  AS SpecialtyCode,
                    sp.SPECIALTY_DESCRIPTION           AS SpecialtyDescription,

                    -- Subspecialty
                    ss.SUBSPECIALTY_NAME               AS SubspecialtyName,

                    -- Last updated staff info
                    stf.STAFF_ID                       AS LastUpdatedByUserCode,
                    stf.STAFF_NAME                     AS LastUpdatedByUserName

                FROM dbo.instance_template it
                    LEFT JOIN dbo.asset a ON it.INSTANCE_TEMPLATE_ASSET_KEY = a.ASSET_KEY
                    LEFT JOIN dbo.facility f ON a.ASSET_FACILITY_KEY = f.FACILITY_KEY
                    LEFT JOIN dbo.session se ON it.INSTANCE_TEMPLATE_SESSION_KEY = se.SESSION_KEY
                    LEFT JOIN dbo.staff s ON se.SESSION_SURGEON_KEY = s.STAFF_KEY
                    LEFT JOIN dbo.specialty sp ON se.SESSION_SPECIALTY_KEY = sp.SPECIALTY_KEY
                    LEFT JOIN dbo.subspecialty ss ON se.SESSION_SUBSPECIALTY_KEY = ss.SUBSPECIALTY_KEY
                    LEFT JOIN dbo.staff stf ON it.INSTANCE_TEMPLATE_LAST_UPDATED_USER_KEY = stf.STAFF_KEY
                    LEFT JOIN dbo.anaesthetic_type at ON at.ANAESTHETIC_TYPE_KEY = se.SESSION_ANAESTHETIC_TYPE_KEY
                    LEFT JOIN dbo.version v ON it.INSTANCE_TEMPLATE_VERSION_KEY = v.VERSION_KEY
                WHERE it.INSTANCE_TEMPLATE_KEY = @Id;
            ";

            return await QuerySingleOrDefaultAsync<TemplateDTO>(sql, new { Id = id });
        }



        public async Task<int> Update(Template t)
        {
            const string sql = @"
                UPDATE dbo.instance_template
                SET
                    INSTANCE_TEMPLATE_SESSION_KEY = @SessionId,
                    INSTANCE_TEMPLATE_ASSET_KEY = @AssetId,
                    INSTANCE_TEMPLATE_CYCLE_WEEK = @CycleWeek,
                    INSTANCE_TEMPLATE_CYCLE_DAY = @CycleDay,
                    INSTANCE_TEMPLATE_IS_OPEN = @IsOpen,
                    INSTANCE_TEMPLATE_START_TIME = @StartTime,
                    INSTANCE_TEMPLATE_END_TIME = @EndTime,
                    INSTANCE_TEMPLATE_LAST_UPDATED_DATETIME = GETDATE(),
                    INSTANCE_TEMPLATE_LAST_UPDATED_USER_KEY = @LastUpdatedByUserId,
                    INSTANCE_TEMPLATE_VERSION_KEY = @VersionId
                WHERE
                    INSTANCE_TEMPLATE_KEY = @Id;
            ";

            return await ExecuteAsync(sql, t);
        }

        public async Task<int> Create(Template t)
        {
            var conflicts = await QueryAsync<TemplateDTO>(
                @"
                SELECT 
                    t.INSTANCE_TEMPLATE_KEY AS Id,
                    t.INSTANCE_TEMPLATE_ASSET_KEY AS AssetId,
                    t.INSTANCE_TEMPLATE_CYCLE_WEEK AS CycleWeek,
                    t.INSTANCE_TEMPLATE_CYCLE_DAY AS CycleDay,
                    t.INSTANCE_TEMPLATE_START_TIME AS StartTime,
                    t.INSTANCE_TEMPLATE_END_TIME AS EndTime,
                    t.INSTANCE_TEMPLATE_VERSION_KEY AS VersionId
                FROM dbo.fnCheckInstanceTemplateConflict(
                    @AssetKey,
                    @CycleWeek,
                    @CycleDay,
                    @StartTime,
                    @EndTime,
                    @IgnoreKey,
                    @VersionId
                ) t",
                new {
                    AssetKey = t.AssetId,
                    CycleWeek = t.CycleWeek,
                    CycleDay = t.CycleDay,
                    StartTime = t.StartTime,
                    EndTime = t.EndTime,
                    IgnoreKey = (int?)null,  // must be nullable for inserts
                    VersionId = t.VersionId
                }
            );

            if (conflicts.Any())
            {
                _logger.LogInformation("Conflicts count: {Count}", conflicts.Count());

                List<int> ids = conflicts.Select(c => c.Id).ToList();
                // 2️⃣ Throw a controlled exception
                throw new ConflictException(ids);
            }

            const string sql = @"
                INSERT INTO dbo.instance_template
                (
                    INSTANCE_TEMPLATE_SESSION_KEY,
                    INSTANCE_TEMPLATE_ASSET_KEY,
                    INSTANCE_TEMPLATE_CYCLE_WEEK,
                    INSTANCE_TEMPLATE_CYCLE_DAY,
                    INSTANCE_TEMPLATE_IS_OPEN,
                    INSTANCE_TEMPLATE_START_TIME,
                    INSTANCE_TEMPLATE_END_TIME,
                    INSTANCE_TEMPLATE_LAST_UPDATED_DATETIME,
                    INSTANCE_TEMPLATE_LAST_UPDATED_USER_KEY,
                    INSTANCE_TEMPLATE_VERSION_KEY
                )
                OUTPUT INSERTED.INSTANCE_TEMPLATE_KEY
                VALUES
                (
                    @SessionId,
                    @AssetId,
                    @CycleWeek,
                    @CycleDay,
                    @IsOpen,
                    @StartTime,
                    @EndTime,
                    GETDATE(),
                    @LastUpdatedByUserId,
                    @VersionId
                );
            ";

            return await ExecuteScalarAsync<int>(sql, t);
        }


        public async Task<List<int>> GetInstanceClashes(
            int assetId,
            DateTime startDateTime,
            DateTime endDateTime)
        {
            const string sql = @"
                SELECT i.INSTANCE_KEY
                FROM dbo.instance i
                WHERE i.INSTANCE_ASSET_KEY = @AssetId
                AND i.INSTANCE_START_DATETIME < @EndDateTime
                AND i.INSTANCE_END_DATETIME   > @StartDateTime
            ";

            var clashes = await QueryAsync<int>(sql, new
            {
                AssetId = assetId,
                StartDateTime = startDateTime,
                EndDateTime = endDateTime
            });

            return clashes.ToList();
        }


        public async Task<List<int>> GetTemplateClashes(
            int? templateId,
            int assetId,
            int week,
            int dayOfWeek,
            TimeSpan startTime,
            TimeSpan endTime,
            int versionId)
        {
            const string sql = @"
                SELECT INSTANCE_TEMPLATE_KEY
                FROM dbo.instance_template
                WHERE INSTANCE_TEMPLATE_ASSET_KEY = @AssetId
                AND INSTANCE_TEMPLATE_CYCLE_WEEK = @Week
                AND INSTANCE_TEMPLATE_CYCLE_DAY = @DayOfWeek
                AND (@TemplateId IS NULL OR INSTANCE_TEMPLATE_KEY <> @TemplateId)
                AND INSTANCE_TEMPLATE_START_TIME < @EndTime
                AND INSTANCE_TEMPLATE_END_TIME > @StartTime
                AND INSTANCE_TEMPLATE_VERSION_KEY = @VersionId
            ";

            var clashes = await QueryAsync<int>(sql, new
            {
                TemplateId = templateId,
                AssetId = assetId,
                Week = week,
                DayOfWeek = dayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                VersionId = versionId
            });

            return clashes.ToList();
        }


        public async Task<MatrixLayout> GetMatrixLayout()
        {
            // 1️⃣ Fetch all unique locations
            const string sqlLocations = @"
                SELECT DISTINCT ASSET_LOCATION
                FROM asset
                ORDER BY ASSET_LOCATION
            ";

            var locations = (await QueryAsync<string>(sqlLocations)).ToList();

            var groups = new List<MatrixNode>();

            // 2️⃣ For each location, fetch its assets
            const string sqlAssets = @"
                SELECT 
                    a.ASSET_KEY AS Id,
                    a.ASSET_CODE AS Code,
                    a.ASSET_DESCRIPTION AS Description,
                    a.ASSET_TYPE_CODE AS TypeCode,
                    a.ASSET_LOCATION AS Location,
                    a.ASSET_FACILITY_KEY AS FacilityId,
                    a.ASSET_DISTRICT_OF_SERVICE AS DistrictOfService,
                    a.ASSET_PRIMARY_SPECIALTY_KEY AS PrimarySpecialtyId,
                    f.FACILITY_KEY AS FacilityId, -- optional duplicate if you want facility info
                    f.FACILITY_CODE AS FacilityCode,
                    f.FACILITY_NAME AS FacilityName,
                    f.FACILITY_TYPE_CODE AS FacilityTypeCode,
                    f.FACILITY_TYPE_DESCRIPTION AS TypeDescription,
                    f.FACILITY_DHB_CODE AS DhbCode,
                    f.FACILITY_DHB_NAME AS DhbName
                FROM asset a
                LEFT JOIN facility f ON a.ASSET_FACILITY_KEY = f.FACILITY_KEY
                WHERE a.ASSET_LOCATION = @Location
                ORDER BY f.FACILITY_NAME, a.ASSET_DESCRIPTION
            ";

            foreach (var location in locations)
            {
                var assets = (await QueryAsync<AssetDTO>(sqlAssets, new { Location = location })).ToList();

                var locationNode = new MatrixNode
                {
                    Id = location ?? "Unknown Location",
                    Label = location ?? "Unknown Location",
                    Type = "location",
                    Children = assets
                        .GroupBy(f => f.FacilityName ?? "Unknown Facility")
                        .Select(facGroup => new MatrixNode
                        {
                            Id = facGroup.First().FacilityId?.ToString() ?? facGroup.Key,
                            Label = facGroup.Key,
                            Type = "facility",
                            Children = facGroup
                                .Select(asset => new MatrixNode
                                {
                                    Id = asset.Id.ToString(),
                                    Label = asset.Description ?? asset.Code ?? "Unnamed Asset",
                                    Type = "asset"
                                })
                                .ToList()
                        })
                        .ToList()
                };

                groups.Add(locationNode);
            }

            return new MatrixLayout
            {
                Grouping = new[] { "location", "facility", "asset" },
                Groups = groups
            };
        }

    }
}