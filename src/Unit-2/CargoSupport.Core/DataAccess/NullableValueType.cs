using CargoSupport.Core.Exceptions;

namespace CargoSupport.Core.DataAccess;

/// <summary>
///     valueType where the base value is nullable. (i.e. string, other valueTypes etc)
///     throws <see cref="NullValueException{TValueType}" /> when the valueType is created from a null value (default
///     behavior)
///     provides additional
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TThis"></typeparam>
/// <exception cref="NullValueException{TValueType}"></exception>
public abstract record NullableValueType<T, TThis>
    : ValueType<T, TThis>
    where T : class
    where TThis : ValueType<T, TThis>, new()
{
    /// <summary>
    ///     creates an instance of this valueType.
    ///     might be wrapped by derived class with operator to improve usability.
    ///     returns null if the given parameter is null
    ///     <seealso cref="ValueType{T,TThis}.Get" />
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static TThis? FromNullable(T? value)
    {
        return value is null ? null : Get(value);
    }

    protected override void Validate(T value)
    {
        base.Validate(value);
        if (value is null)
        {
            throw new NullValueException<T>();
        }
    }

    public static explicit operator T?(NullableValueType<T, TThis>? value)
    {
        return value?.Value;
    }
}
