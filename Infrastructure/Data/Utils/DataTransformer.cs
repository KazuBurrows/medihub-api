using System.Data;

namespace MediHub.Infrastructure.Data.Utils
{
    public static class DataTransformer
    {
        public static DataTable ToIntListTable(List<int> ids)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            foreach (var id in ids)
                dt.Rows.Add(id);
            return dt;
        }

    }
}