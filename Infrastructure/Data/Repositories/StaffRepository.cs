using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Domain.Models;
using MediHub.Infrastructure.Data.Interfaces;

namespace MediHub.Infrastructure.Data.Repositories
{
    public class StaffRepository : BaseRepository, IStaffRepository
    {
        public StaffRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<IEnumerable<Staff>> GetAll()
        {
            const string sql = @"
                SELECT
                    STAFF_KEY AS Id,
                    STAFF_ID AS StaffId,
                    STAFF_NAME AS Name,
                    STAFF_EMAIL AS Email,
                    STAFF_SPECIALTY_KEY AS SpecialtyId
                FROM dbo.staff";

            return await QueryAsync<Staff>(sql);
        }

        public async Task<Staff?> GetById(int id)
        {
            const string sql = @"
                SELECT
                    STAFF_KEY AS Id,
                    STAFF_ID AS StaffId,
                    STAFF_NAME AS Name,
                    STAFF_EMAIL AS Email,
                    STAFF_SPECIALTY_KEY AS SpecialtyId
                FROM dbo.staff
                WHERE STAFF_KEY = @Id";

            return await QuerySingleOrDefaultAsync<Staff>(sql, new { Id = id });
        }


        public async Task<Staff> Create(Staff s)
        {
            const string sql = @"
                INSERT INTO dbo.staff (STAFF_ID, STAFF_NAME, STAFF_EMAIL, STAFF_SPECIALTY_KEY)
                OUTPUT INSERTED.STAFF_KEY
                VALUES (@StaffId, @Name, @Email, @SpecialtyId)";

            var id = await ExecuteScalarAsync<int>(sql, s);
            return await GetById(id);
        }


        public async Task<Staff> Update(Staff s)
        {
            const string sql = @"
                UPDATE dbo.staff
                SET 
                    STAFF_ID = @StaffId,
                    STAFF_NAME = @Name,
                    STAFF_EMAIL = @Email,
                    STAFF_SPECIALTY_KEY = @SpecialtyId
                WHERE STAFF_KEY = @Id";

            await ExecuteAsync(sql, s);
            return await GetById(s.Id);
        }



        public async Task Delete(int id)
        {
            const string sql = @"
                DELETE FROM dbo.staff
                WHERE STAFF_KEY = @Id";

            var rowsAffected = await ExecuteAsync(sql, new { Id = id });
            if (rowsAffected == 0)
            {
                throw new NotFoundException($"No item found with ID {id}.");
            }
        }


        public async Task<Staff?> GetByEmail(string email)
        {
            const string sql = @"
                SELECT TOP (1)
                    STAFF_KEY AS Id,
                    STAFF_ID AS StaffId,
                    STAFF_NAME AS Name,
                    STAFF_EMAIL AS Email,
                    STAFF_SPECIALTY_KEY AS SpecialtyId
                FROM dbo.staff
                WHERE STAFF_EMAIL = @Email
                ORDER BY STAFF_KEY";

            return await QuerySingleOrDefaultAsync<Staff>(sql, new { Email = email });
        }

        public async Task<IEnumerable<StaffDTO>> GetAllDTO()
        {
            const string sql = @"
                SELECT
                    s.STAFF_KEY AS Id,
                    s.STAFF_ID AS StaffId,
                    s.STAFF_NAME AS Name,
                    s.STAFF_EMAIL AS Email,
                    s.STAFF_SPECIALTY_KEY AS SpecialtyId,
                    sp.SPECIALTY_CODE AS SpecialtyCode,
                    sp.SPECIALTY_DESCRIPTION AS SpecialtyDescription
                FROM dbo.staff s
                LEFT JOIN dbo.specialty sp
                    ON sp.SPECIALTY_KEY = s.STAFF_SPECIALTY_KEY
            ";

            return await QueryAsync<StaffDTO>(sql);
        }

        public async Task<StaffDTO?> GetByIdDTO(int id)
        {
            const string sql = @"
                SELECT
                    s.STAFF_KEY AS Id,
                    s.STAFF_ID AS StaffId,
                    s.STAFF_NAME AS Name,
                    s.STAFF_EMAIL AS Email,
                    s.STAFF_SPECIALTY_KEY AS SpecialtyId,
                    sp.SPECIALTY_CODE AS SpecialtyCode,
                    sp.SPECIALTY_DESCRIPTION AS SpecialtyDescription
                FROM dbo.staff s
                LEFT JOIN dbo.specialty sp
                    ON sp.SPECIALTY_KEY = s.STAFF_SPECIALTY_KEY
                WHERE s.STAFF_KEY = @Id
            ";

            return await QuerySingleOrDefaultAsync<StaffDTO>(sql, new { Id = id });
        }

    }
}
