namespace MediHub.Common.Exceptions.Infrastructure;

public class ConflictException : Exception
{
    public List<int> ConflictingIds { get; }

    public ConflictException(List<int> ids)
        : base($"Conflict occurred for IDs: {string.Join(", ", ids)}")
    {
        ConflictingIds = ids;
    }

    public ConflictException(string message, IEnumerable<int> ids)
        : base(message)
    {
        ConflictingIds = ids.ToList();
    }
}