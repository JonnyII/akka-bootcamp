namespace CargoSupport.Core.Exceptions;

public class NullValueException<TValueType> : NullValueException
{
    internal NullValueException() : base(
        $"value of type {typeof(TValueType).Name} must not be null. This might be caused by using the from keyword where fromNullable should have been used.")
    {
    }
}

public abstract class NullValueException : CargoSupportValidationException
{
    protected internal NullValueException(string message) : base(message)
    {
    }
}