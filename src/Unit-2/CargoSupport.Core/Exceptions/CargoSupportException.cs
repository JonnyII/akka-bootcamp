namespace CargoSupport.Core.Exceptions;

/// <summary>
///     baseClass for all custom exceptions.
///     can be used to handle custom exceptions separate
/// </summary>
public abstract class CargoSupportException : Exception
{
    protected CargoSupportException() : this(null)
    {
    }

    protected CargoSupportException(string? message) : this(message, null)
    {
    }

    protected CargoSupportException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}