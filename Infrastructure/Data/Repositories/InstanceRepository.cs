using MediHub.Domain.DTOs;
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
                    id,
                    asset_id AS AssetId,
                    session_id AS SessionId,
                    start_datetime AS StartDatetime,
                    end_datetime AS EndDatetime
                FROM dbo.instance";

            return await QueryAsync<Instance>(sql);
        }

        public async Task<IEnumerable<ScheduleDTO>> GetAllByStaffId(int staffId)
{
    const string sql = @"
        SELECT DISTINCT
            i.id,
            f.name AS FacilityName,
            a.name AS AssetName,
            s.name AS SessionName,
            s.is_acute AS IsAcute,
            s.is_pediatric AS IsPediatric,
            s.anaesthetic_type AS AnaestheticType,
            su.first_name + ' ' + su.last_name AS SurgeonName,
            i.start_datetime AS StartDateTime,
            i.end_datetime   AS EndDateTime
        FROM dbo.instance i
        INNER JOIN dbo.instance_staff isf
            ON isf.instance_id = i.id
        LEFT JOIN dbo.asset a
            ON i.asset_id = a.id
        LEFT JOIN dbo.facility f
            ON a.facility_id = f.id
        LEFT JOIN dbo.session s
            ON i.session_id = s.id
        LEFT JOIN dbo.staff su
            ON s.surgeon_id = su.id
        WHERE isf.staff_id = @StaffId
        ORDER BY i.start_datetime";

    return await QueryAsync<ScheduleDTO>(
        sql,
        new { StaffId = staffId }
    );
}





        public async Task<Instance?> GetById(int id)
        {
            const string sql = @"
                SELECT 
                    id,
                    asset_id AS AssetId,
                    session_id AS SessionId,
                    start_datetime AS StartDatetime,
                    end_datetime AS EndDatetime
                FROM dbo.instance
                WHERE id = @id";

            return await QuerySingleOrDefaultAsync<Instance>(
                sql,
                new { id }
            );
        }

        public async Task<int> Create(Instance i)
        {
            const string sql = @"
                INSERT INTO dbo.instance (asset_id, session_id, start_datetime, end_datetime)
                VALUES (@AssetId, @SessionId, @StartDatetime, @EndDatetime)";
            return await ExecuteAsync(sql, i);
        }


        public async Task<int> Update(Instance i)
        {
            const string sql = @"
                UPDATE dbo.instance
                SET asset_id = @AssetId,
                    session_id = @SessionId,
                    start_datetime = @StartDatetime,
                    end_datetime = @EndDatetime
                WHERE id = @Id";
            return await ExecuteAsync(sql, i);
        }


        public async Task<int> Delete(int id)
        {
            const string sql = @"
                BEGIN TRANSACTION;

                -- 1. Delete child relationships first
                DELETE FROM dbo.instance_staff
                WHERE instance_id = @Id;

                -- 2. Delete the instance itself
                DELETE FROM dbo.instance
                WHERE id = @Id;

                COMMIT;
            ";

            return await ExecuteAsync(sql, new { Id = id });
        }



    }
}
