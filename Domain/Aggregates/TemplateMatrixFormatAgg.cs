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
        public IReadOnlyList<TemplateAssetFormat> Assets { get; }

        public TemplateFacilityFormat(
            int facilityId,
            string facilityName,
            IEnumerable<TemplateAssetFormat> assets)
        {
            FacilityId = facilityId;
            FacilityName = facilityName;
            Assets = assets.ToList();
        }
    }

    public sealed class TemplateAssetFormat
    {
        public int AssetId { get; }
        public string AssetName { get; }
        public int SortOrder { get; }

        public TemplateAssetFormat(int assetId, string assetName, int sortOrder)
        {
            AssetId = assetId;
            AssetName = assetName;
            SortOrder = sortOrder;
        }
    }

    public sealed class TemplateMatrixFormatRow
    {
        public int FacilityId { get; init; }
        public string FacilityName { get; init; }

        public int AssetId { get; init; }
        public string AssetName { get; init; }
        public int AssetSortOrder { get; init; }
    }


}
