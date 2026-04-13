using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Domain.DTOs;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class SessionOverrideRepository : BaseRepository, ISessionOverrideRepository
    {
        public SessionOverrideRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        // Get single SessionOverride
        public async Task<SessionOverride?> GetById(int id)
        {
            const string sql = @"
                SELECT
                    SESSION_OVERRIDE_KEY AS Id,
                    SESSION_OVERRIDE_IS_ACUTE AS IsAcute,
                    SESSION_OVERRIDE_IS_PAEDIATRIC AS IsPaediatric,
                    SESSION_OVERRIDE_ANAESTHETIC_TYPE_KEY AS AnaestheticTypeId,
                    SESSION_OVERRIDE_SURGEON_KEY AS SurgeonId,
                    SESSION_OVERRIDE_SURGEON_TYPE_KEY AS SurgeonTypeId,
                    SESSION_OVERRIDE_SPECIALTY_KEY AS SpecialtyId,
                    SESSION_OVERRIDE_SUBSPECIALTY_KEY AS SubspecialtyId
                FROM dbo.session_override
                WHERE SESSION_OVERRIDE_KEY = @Id";

            return await QuerySingleOrDefaultAsync<SessionOverride>(sql, new { Id = id });
        }

        // Get single SessionOverride DTO with names
        public async Task<SessionOverrideDTO?> GetByIdDTO(int id)
        {
            const string sql = @"
                SELECT
                    so.SESSION_OVERRIDE_KEY AS Id,
                    so.SESSION_OVERRIDE_IS_ACUTE AS IsAcute,
                    so.SESSION_OVERRIDE_IS_PAEDIATRIC AS IsPaediatric,
                    so.SESSION_OVERRIDE_ANAESTHETIC_TYPE_KEY AS AnaestheticTypeId,
                    at.ANAESTHETIC_TYPE_CODE AS AnaestheticTypeCode,
                    at.ANAESTHETIC_TYPE_DESCRIPTION AS AnaestheticTypeDescription,
                    so.SESSION_OVERRIDE_SURGEON_KEY AS SurgeonId,
                    so.SESSION_OVERRIDE_SURGEON_TYPE_KEY AS SurgeonTypeId,
                    sty.SURGEON_TYPE_CODE AS SurgeonTypeCode,
                    sty.SURGEON_TYPE_DESCRIPTION AS SurgeonTypeDescription,
                    st.STAFF_NAME AS SurgeonName,
                    so.SESSION_OVERRIDE_SPECIALTY_KEY AS SpecialtyId,
                    sp.SPECIALTY_CODE AS SpecialtyCode,
                    sp.SPECIALTY_DESCRIPTION AS SpecialtyDescription,
                    so.SESSION_OVERRIDE_SUBSPECIALTY_KEY AS SubspecialtyId,
                    sub.SUBSPECIALTY_NAME AS SubspecialtyName
                FROM dbo.session_override so
                LEFT JOIN dbo.staff st ON st.STAFF_KEY = so.SESSION_OVERRIDE_SURGEON_KEY
                LEFT JOIN dbo.specialty sp ON sp.SPECIALTY_KEY = so.SESSION_OVERRIDE_SPECIALTY_KEY
                LEFT JOIN dbo.subspecialty sub ON sub.SUBSPECIALTY_KEY = so.SESSION_OVERRIDE_SUBSPECIALTY_KEY
                LEFT JOIN dbo.surgeon_type sty ON sty.SURGEON_TYPE_KEY = so.SESSION_OVERRIDE_SURGEON_TYPE_KEY
                LEFT JOIN dbo.anaesthetic_type at ON at.ANAESTHETIC_TYPE_KEY = so.SESSION_OVERRIDE_ANAESTHETIC_TYPE_KEY
                WHERE so.SESSION_OVERRIDE_KEY = @Id";

            return await QuerySingleOrDefaultAsync<SessionOverrideDTO>(sql, new { Id = id });
        }

        // Create a new session override
        public async Task<SessionOverride> Create(int instanceId, SessionOverride s)
        {
            const string sql = @"
                INSERT INTO dbo.session_override (
                    SESSION_OVERRIDE_IS_ACUTE,
                    SESSION_OVERRIDE_IS_PAEDIATRIC,
                    SESSION_OVERRIDE_ANAESTHETIC_TYPE_KEY,
                    SESSION_OVERRIDE_SURGEON_KEY,
                    SESSION_OVERRIDE_SURGEON_TYPE_KEY,
                    SESSION_OVERRIDE_SPECIALTY_KEY,
                    SESSION_OVERRIDE_SUBSPECIALTY_KEY
                )
                OUTPUT INSERTED.SESSION_OVERRIDE_KEY
                VALUES (
                    @IsAcute,
                    @IsPaediatric,
                    @AnaestheticTypeId,
                    @SurgeonId,
                    @SurgeonTypeId,
                    @SpecialtyId,
                    @SubspecialtyId
                )";

            var id = await ExecuteScalarAsync<int>(sql, s);
            s.Id = id;
            
            await ExecuteAsync("dbo.sp_SetInstanceSessionOverride",
                new { InstanceId = instanceId, SessionOverrideId = id });

            return s;
        }

        // Update existing session override
        public async Task<SessionOverride> Update(SessionOverride s)
        {
            const string sql = @"
                UPDATE dbo.session_override
                SET
                    SESSION_OVERRIDE_IS_ACUTE = @IsAcute,
                    SESSION_OVERRIDE_IS_PAEDIATRIC = @IsPaediatric,
                    SESSION_OVERRIDE_ANAESTHETIC_TYPE_KEY = @AnaestheticTypeId,
                    SESSION_OVERRIDE_SURGEON_KEY = @SurgeonId,
                    SESSION_OVERRIDE_SURGEON_TYPE_KEY = @SurgeonTypeId,
                    SESSION_OVERRIDE_SPECIALTY_KEY = @SpecialtyId,
                    SESSION_OVERRIDE_SUBSPECIALTY_KEY = @SubspecialtyId
                WHERE SESSION_OVERRIDE_KEY = @Id";

            var rowsAffected = await ExecuteAsync(sql, s);
            if (rowsAffected == 0)
            {
                throw new NotFoundException($"No SessionOverride found with ID {s.Id}.");
            }

            return s;
        }

        // Delete a session override
        public async Task Delete(int id)
        {
            const string sql = @"
                -- Unlink instances first
                UPDATE dbo.instance
                SET INSTANCE_SESSION_OVERRIDE_KEY = NULL
                WHERE INSTANCE_SESSION_OVERRIDE_KEY = @Id;

                -- Now delete the session_override
                DELETE FROM dbo.session_override
                WHERE SESSION_OVERRIDE_KEY = @Id;
            ";

            var rowsAffected = await ExecuteAsync(sql, new { Id = id });
            if (rowsAffected == 0)
            {
                throw new NotFoundException($"No SessionOverride found with ID {id}.");
            }
        }

        public async Task<int> getIdByInstanceId(int instanceId)
        {
            const string sql = @"
                SELECT INSTANCE_SESSION_OVERRIDE_KEY
                FROM dbo.instance
                WHERE INSTANCE_KEY = @InstanceId";

            return await QuerySingleOrDefaultAsync<int?>(sql, new { InstanceId = instanceId }) ?? 0;
        }
    }
}