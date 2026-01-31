namespace MediHub.Infrastructure.Data.Utils
{
    public static class AggregateHelper
    {
        /// <summary>
        /// Maps junction table rows into aggregate entities.
        /// Null-safe for empty aggregates or junction rows.
        /// </summary>
        public static void MapJunction<TAggregate, TId>(
            IEnumerable<TAggregate> aggregates,
            IEnumerable<(TId AggregateId, int RelatedId)>? junctionRows,
            Func<TAggregate, TId> getId,
            Func<TAggregate, List<int>> getList)
        {
            if (aggregates == null || !aggregates.Any() || junctionRows == null)
                return;

            var aggDict = aggregates.ToDictionary(getId);

            foreach (var row in junctionRows)
            {
                if (!aggDict.TryGetValue(row.AggregateId, out var agg))
                    continue;

                var list = getList(agg); // guaranteed to exist
                list.Add(row.RelatedId);
            }
        }

    }
}
