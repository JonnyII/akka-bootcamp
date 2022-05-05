using CargoSupport.Core.DataAccess;

namespace CargoSupport.Core.Exceptions;

/// <summary>
///     baseClass for all validation errors, typically used as baseType for all exceptions thrown by
///     <see cref="ValueType{T,TThis}.Validate" />
/// </summary>
public abstract class CargoSupportValidationException : CargoSupportException
{
    protected CargoSupportValidationException() : this("The validation has detected an error")
    {
    }

    protected CargoSupportValidationException(string? message) : this(message, null)
    {
    }

    protected CargoSupportValidationException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}