namespace MediHub.Functions.Helpers.Exceptions
{
    public class InstanceClashException : Exception
    {
        public IEnumerable<int> Conflicts { get; }

        public InstanceClashException(string message, IEnumerable<int> conflicts)
            : base(message)
        {
            Conflicts = conflicts;
        }
    }

}
