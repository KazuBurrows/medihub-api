using MediHub.Domain.DTOs;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class SessionRepository : BaseRepository, ISessionRepository
    {
        public SessionRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<IEnumerable<SessionDTO>> GetAll()
        {
            var sql = @"
                SELECT 
                    s.id AS Id,
                    s.name AS Name,
                    s.is_acute AS IsAcute,
                    s.is_pediatric AS IsPediatric,
                    s.anaesthetic_type AS AnaestheticType,
                    (st.first_name + ' ' + st.last_name) AS SurgeonName,
                    sp.name AS SpecialtyName,
                    sub.name AS SubspecialtyName,
                    COUNT(i.id) AS InstanceCount
                FROM session s
                LEFT JOIN staff st ON st.id = s.surgeon_id
                LEFT JOIN specialty sp ON sp.id = s.specialty_id
                LEFT JOIN subspecialty sub ON sub.id = s.subspecialty_id
                LEFT JOIN instance i ON i.session_id = s.id
                GROUP BY 
                    s.id, s.name, s.is_acute, s.is_pediatric, s.anaesthetic_type,
                    st.first_name, st.last_name,
                    sp.name, sub.name
                ORDER BY s.name;
            ";


    return await QueryAsync<SessionDTO>(sql);

        }


        public async Task<Session?> GetById(int id)
        {
            return await QuerySingleOrDefaultAsync<Session>(
                "SELECT * FROM dbo.session WHERE id = @id",
                new { id }
            );
        }

        public async Task<int> Create(Session s)
        {
            const string sql = @"
                INSERT INTO dbo.session (name, is_acute, is_pediatric, anaesthetic_type)
                VALUES (@name, @IsAcute, @IsPediatric, @AnaestheticType)";
            return await ExecuteAsync(sql, s);
        }


        public async Task<int> Update(Session s)
        {
            const string sql = @"
                UPDATE dbo.session
                SET name = @Name,
                    is_acute = @IsAcute,
                    is_pediatric = @IsPediatric,
                    anaesthetic_type = @AnaestheticType
                WHERE id = @Id";
            return await ExecuteAsync(sql, s);
        }


        public async Task<int> Delete(int id)
        {
            const string sql = "DELETE FROM dbo.session WHERE id = @id";
            return await ExecuteAsync(sql, new { id });
        }
    }
}
