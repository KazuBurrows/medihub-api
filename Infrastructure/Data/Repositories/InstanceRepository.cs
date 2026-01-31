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
                    theatre_id AS TheatreId,
                    session_id AS SessionId,
                    start_datetime AS StartDatetime,
                    end_datetime AS EndDatetime
                FROM dbo.instance";

            return await QueryAsync<Instance>(sql);
        }


        public async Task<Instance?> GetById(int id)
        {
            const string sql = @"
                SELECT 
                    id,
                    theatre_id AS TheatreId,
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
                INSERT INTO dbo.instance (theatre_id, session_id, start_datetime, end_datetime)
                VALUES (@TheatreId, @SessionId, @StartDatetime, @EndDatetime)";
            return await ExecuteAsync(sql, i);
        }


        public async Task<int> Update(Instance i)
        {
            const string sql = @"
                UPDATE dbo.instance
                SET theatre_id = @TheatreId,
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
