namespace MediHub.Common.Exceptions.Infrastructure;

public class BadRequestException : Exception
{
    public BadRequestException()
        : base($"Bad request")
    {
    }

    
    public BadRequestException(string message)
        : base($"Bad request: {message}")
    {
    }
}