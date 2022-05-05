using CargoSupport.Core.Exceptions;

namespace CargoSupport.Core.DataAccess;

/// <summary>
///     valueType where the base value is not nullable (i.e. int etc)
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TThis"></typeparam>
public abstract record NonNullableValueType<T, TThis>
    : ValueType<T, TThis>
    where T : struct
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
    public static TThis? GetNullable(T? value)
    {
        return value is null ? null : Get(value.Value);
    }


    public static explicit operator T?(NonNullableValueType<T, TThis>? value)
    {
        return value?.Value;
    }

    /// <summary>
    ///     note: this overload uses intellisense only to detect nullable types.
    ///     this means, that it is possible to pass null-values at runtime.
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="InvalidNullCastException{TValueType,TInnerValue}"></exception>
    public static explicit operator T(NonNullableValueType<T, TThis> value)
    {
        return value?.Value ?? throw new InvalidNullCastException<T, TThis>();
    }
}
