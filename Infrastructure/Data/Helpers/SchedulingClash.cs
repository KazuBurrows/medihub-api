using MediHub.Infrastructure.Data.Repositories;

namespace MediHub.Infrastructure.Data.Helpers
{
    public class SchedulingClash : BaseRepository
    {
        public SchedulingClash(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<string?> CheckForInstanceClashes(
            int? instanceId,      // null for create
            int assetId,
            DateTime start,
            DateTime end,
            List<int> staffs)
        {
            /* ---------------- INSTANCE CLASH ---------------- */
            const string clashCheckSql = @"
                SELECT i.Id
                FROM dbo.instance i
                WHERE i.asset_id = @AssetId
                AND (@InstanceId IS NULL OR i.id <> @InstanceId)
                AND i.start_datetime < @EndDateTime
                AND i.end_datetime > @StartDateTime
            ";

            var existingInstances = await QueryAsync<int>(clashCheckSql, new
            {
                AssetId = assetId,
                InstanceId = instanceId,
                StartDateTime = start,
                EndDateTime = end
            });

            /* ---------------- STAFF CLASH ---------------- */
            const string staffClashSql = @"
                SELECT s.staff_id
                FROM dbo.instance_staff s
                INNER JOIN dbo.instance i ON s.instance_id = i.id
                WHERE s.staff_id IN @StaffIds
                AND (@InstanceId IS NULL OR i.id <> @InstanceId)
                AND i.start_datetime < @EndDateTime
                AND i.end_datetime > @StartDateTime
            ";

            var staffClashes = await QueryAsync<int>(staffClashSql, new
            {
                StaffIds = staffs,
                InstanceId = instanceId,
                StartDateTime = start,
                EndDateTime = end
            });

            if (!existingInstances.Any() && !staffClashes.Any())
                return null;

            var lines = new List<string> { "Your changes conflict with existing data." };

            if (existingInstances.Any())
                lines.Add("Conflicting instance IDs: " + string.Join(", ", existingInstances));

            if (staffClashes.Any())
            {
                const string staffQuery = @"
                    SELECT first_name AS FirstName, last_name AS LastName
                    FROM dbo.Staff
                    WHERE Id IN @StaffIds
                ";

                var staffNames = (await QueryAsync<(string FirstName, string LastName)>(
                    staffQuery,
                    new { StaffIds = staffClashes.ToArray() }))
                    .Select(s => $"{s.FirstName} {s.LastName}");

                lines.Add("Conflicting staff: " + string.Join(", ", staffNames));
            }

            return string.Join(Environment.NewLine, lines);
        }

        public async Task<string?> CheckForTemplateClashes(
            int? templateId,     // null for create
            int assetId,
            int week,
            byte dayOfWeek,
            DateTime start,      // only time portion matters
            DateTime end
        )
        {
            var startTime = start.TimeOfDay;
            var endTime = end.TimeOfDay;

            /* ---------------- ASSET CLASH ---------------- */
            const string assetClashSql = @"
                SELECT t.id
                FROM dbo.instance_template t
                WHERE t.asset_id = @AssetId
                AND t.week = @Week
                AND t.day_of_week = @DayOfWeek
                AND (@TemplateId IS NULL OR t.id <> @TemplateId)
                AND t.start_time < @EndTime
                AND t.end_time   > @StartTime
            ";

            var assetClashes = await QueryAsync<int>(assetClashSql, new
            {
                AssetId = assetId,
                Week = week,
                DayOfWeek = dayOfWeek,
                TemplateId = templateId,
                StartTime = startTime,
                EndTime = endTime
            });

            /* ---------------- NO CLASH ---------------- */
            if (!assetClashes.Any())
                return null;

            var lines = new List<string> { "Your template overlaps with existing schedule data." };

            /* ---------------- ASSET DETAILS ---------------- */
            if (assetClashes.Any())
                lines.Add("Conflicting template IDs (asset clash): " + string.Join(", ", assetClashes));

            return string.Join(Environment.NewLine, lines);
        }

    }

}