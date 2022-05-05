using CargoSupport.Core.DataAccess;

namespace CargoSupport.Core.Exceptions;

public sealed class InvalidIdException<TId>
    : InvalidIdException
    where TId : IdValueType<TId>, new()
{
    public InvalidIdException(int value)
        : base($"{value} is not a valid id of type {typeof(TId).Name}.")
    {
    }
}

/// <summary>
///     general version of <see cref="InvalidIdException{TId}" />
///     use that class to create an instance
/// </summary>
public abstract class InvalidIdException : CargoSupportValidationException
{
    protected internal InvalidIdException(string message) : base(message)
    {
    }
}