namespace CargoSupport.Core.Exceptions;

public class InvalidNullCastException<TInnerValue, TValueType> : NullValueException
{
    internal InvalidNullCastException() :
        base(
            $"you tried to convert a null-value of type {typeof(TValueType).Name} to its inner value {typeof(TInnerValue).Name} in a non-nullable typecast.")
    {
    }
}

public abstract class InvalidNullCastException : CargoSupportValidationException
{
    protected internal InvalidNullCastException(string message) : base(message)
    {
    }
}