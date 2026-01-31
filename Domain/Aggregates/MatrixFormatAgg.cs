namespace MediHub.Domain.DTOs
{
    public sealed class MatrixFormatAgg
    {
        public IReadOnlyList<FacilityFormat> Facilities { get; }

        public MatrixFormatAgg(IEnumerable<FacilityFormat> facilities)
        {
            Facilities = facilities.ToList();
        }
    }

    public sealed class FacilityFormat
    {
        public int FacilityId { get; }
        public string FacilityName { get; }
        public IReadOnlyList<TheatreFormat> Theatres { get; }

        public FacilityFormat(
            int facilityId,
            string facilityName,
            IEnumerable<TheatreFormat> theatres)
        {
            FacilityId = facilityId;
            FacilityName = facilityName;
            Theatres = theatres.ToList();
        }
    }

    public sealed class TheatreFormat
    {
        public int TheatreId { get; }
        public string TheatreName { get; }
        public int SortOrder { get; }

        public TheatreFormat(int theatreId, string theatreName, int sortOrder)
        {
            TheatreId = theatreId;
            TheatreName = theatreName;
            SortOrder = sortOrder;
        }
    }

    public sealed class MatrixFormatRow
    {
        public int FacilityId { get; init; }
        public string FacilityName { get; init; }

        public int TheatreId { get; init; }
        public string TheatreName { get; init; }
        public int TheatreSortOrder { get; init; }
    }


}
