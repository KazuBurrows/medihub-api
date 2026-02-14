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
        public IReadOnlyList<AssetFormat> Assets { get; }

        public FacilityFormat(
            int facilityId,
            string facilityName,
            IEnumerable<AssetFormat> assets)
        {
            FacilityId = facilityId;
            FacilityName = facilityName;
            Assets = assets.ToList();
        }
    }

    public sealed class AssetFormat
    {
        public int AssetId { get; }
        public string AssetName { get; }
        public int SortOrder { get; }

        public AssetFormat(int assetId, string assetName, int sortOrder)
        {
            AssetId = assetId;
            AssetName = assetName;
            SortOrder = sortOrder;
        }
    }

    public sealed class MatrixFormatRow
    {
        public int FacilityId { get; init; }
        public string FacilityName { get; init; }

        public int AssetId { get; init; }
        public string AssetName { get; init; }
        public int AssetSortOrder { get; init; }
    }


}
