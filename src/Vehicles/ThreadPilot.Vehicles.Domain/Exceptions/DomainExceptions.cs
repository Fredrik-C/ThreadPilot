namespace ThreadPilot.Vehicles.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception inner) : base(message, inner) { }
}

public class ValidationException : DomainException
{
    public ValidationException(string message) : base(message) { }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message) { }
}

