namespace MediHub.Domain.DTOs
{
    public sealed class TemplateMatrixFormatAgg
    {
        public IReadOnlyList<TemplateFacilityFormat> Facilities { get; }

        public TemplateMatrixFormatAgg(IEnumerable<TemplateFacilityFormat> facilities)
        {
            Facilities = facilities.ToList();
        }
    }

    public sealed class TemplateFacilityFormat
    {
        public int FacilityId { get; }
        public string FacilityName { get; }
        public IReadOnlyList<TemplateTheatreFormat> Theatres { get; }

        public TemplateFacilityFormat(
            int facilityId,
            string facilityName,
            IEnumerable<TemplateTheatreFormat> theatres)
        {
            FacilityId = facilityId;
            FacilityName = facilityName;
            Theatres = theatres.ToList();
        }
    }

    public sealed class TemplateTheatreFormat
    {
        public int TheatreId { get; }
        public string TheatreName { get; }
        public int SortOrder { get; }

        public TemplateTheatreFormat(int theatreId, string theatreName, int sortOrder)
        {
            TheatreId = theatreId;
            TheatreName = theatreName;
            SortOrder = sortOrder;
        }
    }

    public sealed class TemplateMatrixFormatRow
    {
        public int FacilityId { get; init; }
        public string FacilityName { get; init; }

        public int TheatreId { get; init; }
        public string TheatreName { get; init; }
        public int TheatreSortOrder { get; init; }
    }


}
