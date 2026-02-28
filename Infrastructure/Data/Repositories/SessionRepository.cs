using MediHub.Domain.DTOs;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class SessionRepository : BaseRepository, ISessionRepository
    {
        public SessionRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<IEnumerable<Session>> GetAll()
        {
            const string sql = @"
                SELECT 
                    SESSION_KEY AS Id,
                    SESSION_TITLE AS Name,
                    SESSION_IS_ACUTE AS IsAcute,
                    SESSION_IS_PAEDIATRIC AS IsPediatric,
                    SESSION_ANAESTHETIC_TYPE AS AnaestheticType,
                    SESSION_SURGEON_KEY AS SurgeonId,
                    SESSION_SPECIALTY_KEY AS SpecialtyId,
                    SESSION_SUBSPECIALTY_KEY AS SubspecialtyId
                FROM dbo.session
            ";

            return await QueryAsync<Session>(sql);
        }


        public async Task<IEnumerable<SessionDTO>> GetAllDTO()
        {
            const string sql = @"
                SELECT 
                    s.SESSION_KEY AS Id,
                    s.SESSION_TITLE AS Name,
                    s.SESSION_IS_ACUTE AS IsAcute,
                    s.SESSION_IS_PAEDIATRIC AS IsPediatric,
                    s.SESSION_ANAESTHETIC_TYPE AS AnaestheticType,
                    s.SESSION_SURGEON_KEY AS SurgeonId,
                    st.STAFF_NAME AS SurgeonName,
                    s.SESSION_SPECIALTY_KEY AS SpecialtyId,
                    sp.SPECIALTY_CODE AS SpecialtyCode,
                    sp.SPECIALTY_DESCRIPTION AS SpecialtyDescription,
                    s.SESSION_SUBSPECIALTY_KEY AS SubspecialtyId,
                    sub.SUBSPECIALTY_NAME AS SubspecialtyName
                FROM dbo.session s
                LEFT JOIN dbo.staff st ON st.STAFF_KEY = s.SESSION_SURGEON_KEY
                LEFT JOIN dbo.specialty sp ON sp.SPECIALTY_KEY = s.SESSION_SPECIALTY_KEY
                LEFT JOIN dbo.subspecialty sub ON sub.SUBSPECIALTY_KEY = s.SESSION_SUBSPECIALTY_KEY
                ORDER BY s.SESSION_TITLE;
            ";

            return await QueryAsync<SessionDTO>(sql);
        }




        public async Task<Session?> GetById(int id)
        {
            const string sql = @"
        SELECT
            SESSION_KEY AS Id,
            SESSION_TITLE AS Name,
            SESSION_IS_ACUTE AS IsAcute,
            SESSION_IS_PAEDIATRIC AS IsPediatric,
            SESSION_ANAESTHETIC_TYPE AS AnaestheticType,
            SESSION_SURGEON_KEY AS SurgeonId,
            SESSION_SPECIALTY_KEY AS SpecialtyId,
            SESSION_SUBSPECIALTY_KEY AS SubspecialtyId
        FROM dbo.session
        WHERE SESSION_KEY = @Id";

            return await QuerySingleOrDefaultAsync<Session>(sql, new { Id = id });
        }

        public async Task<SessionDTO?> GetByIdDTO(int id)
{
    const string sql = @"
        SELECT
            s.SESSION_KEY AS Id,
            s.SESSION_TITLE AS Name,
            s.SESSION_IS_ACUTE AS IsAcute,
            s.SESSION_IS_PAEDIATRIC AS IsPediatric,
            s.SESSION_ANAESTHETIC_TYPE AS AnaestheticType,
            s.SESSION_SURGEON_KEY AS SurgeonId,
            st.STAFF_NAME AS SurgeonName,
            s.SESSION_SPECIALTY_KEY AS SpecialtyId,
            sp.SPECIALTY_CODE AS SpecialtyCode,
            sp.SPECIALTY_DESCRIPTION AS SpecialtyDescription,
            s.SESSION_SUBSPECIALTY_KEY AS SubspecialtyId,
            sub.SUBSPECIALTY_NAME AS SubspecialtyName
        FROM dbo.session s
        LEFT JOIN dbo.staff st
            ON s.SESSION_SURGEON_KEY = st.STAFF_KEY
        LEFT JOIN dbo.specialty sp
            ON s.SESSION_SPECIALTY_KEY = sp.SPECIALTY_KEY
        LEFT JOIN dbo.subspecialty sub
            ON s.SESSION_SUBSPECIALTY_KEY = sub.SUBSPECIALTY_KEY
        WHERE s.SESSION_KEY = @Id";

    return await QuerySingleOrDefaultAsync<SessionDTO>(sql, new { Id = id });
}



        public async Task<int> Create(Session s)
        {
            const string sql = @"
                INSERT INTO dbo.session (
                    SESSION_TITLE,
                    SESSION_IS_ACUTE,
                    SESSION_IS_PAEDIATRIC,
                    SESSION_ANAESTHETIC_TYPE,
                    SESSION_SURGEON_KEY,
                    SESSION_SPECIALTY_KEY,
                    SESSION_SUBSPECIALTY_KEY
                )
                OUTPUT INSERTED.SESSION_KEY
                VALUES (
                    @Name,
                    @IsAcute,
                    @IsPediatric,
                    @AnaestheticType,
                    @SurgeonId,
                    @SpecialtyId,
                    @SubspecialtyId
                )";

            return await ExecuteScalarAsync<int>(sql, s);
        }



        public async Task<int> Update(Session s)
        {
            const string sql = @"
                UPDATE dbo.session
                SET
                    SESSION_TITLE = @Name,
                    SESSION_IS_ACUTE = @IsAcute,
                    SESSION_IS_PAEDIATRIC = @IsPediatric,
                    SESSION_ANAESTHETIC_TYPE = @AnaestheticType,
                    SESSION_SURGEON_KEY = @SurgeonId,
                    SESSION_SPECIALTY_KEY = @SpecialtyId,
                    SESSION_SUBSPECIALTY_KEY = @SubspecialtyId
                WHERE SESSION_KEY = @Id";

            return await ExecuteAsync(sql, s);
        }



        public async Task<int> Delete(int id)
        {
            const string sql = @"
                DELETE FROM dbo.session
                WHERE SESSION_KEY = @Id";

            return await ExecuteAsync(sql, new { Id = id });
        }

    }
}