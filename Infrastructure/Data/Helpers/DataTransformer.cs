using System.Data;
using MediHub.Domain.Models; // for StaffDTO
using System.Collections.Generic;

public static class DataTransformer
{
    /// <summary>
    /// Converts a list of StaffDTO to a DataTable for TVP with StaffId and RoleId
    /// </summary>
    public static DataTable ToStaffRoleTable(List<StaffDTO> staffs)
{
    var dt = new DataTable();
    dt.Columns.Add("StaffId", typeof(int));
    dt.Columns.Add("RoleId", typeof(int));

    foreach (var s in staffs)
    {
        // If staff id is 0/null but role id exists, still add the row
        object staffIdValue = s.Id > 0 ? s.Id : (object)DBNull.Value;
        object roleIdValue = s.RoleId.HasValue ? s.RoleId.Value : (object)DBNull.Value;

        // Only skip rows that have BOTH staffId and roleId null
        if (staffIdValue != DBNull.Value || roleIdValue != DBNull.Value)
        {
            dt.Rows.Add(staffIdValue, roleIdValue);
        }
    }

    return dt;
}



}
