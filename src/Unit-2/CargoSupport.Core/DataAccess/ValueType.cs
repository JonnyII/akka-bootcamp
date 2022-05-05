using CargoSupport.Core.Exceptions;

namespace CargoSupport.Core.DataAccess;
// NullableValueType and NotNullableValueType must be separate classes, since the optional
// parameter in GetNullable is incompatible with native values like int, if the typeParameter
// is not restricted to struct, which we cant do on class level and does not work on method level.

/// <summary>
///     this type should not be used directly. use <see cref="NonNullableValueType{T,TThis}" />
///     or <see cref="NullableValueType{T,TThis}" /> instead for improved null-handling.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TThis"></typeparam>
public abstract record ValueType<T, TThis>
    where TThis : ValueType<T, TThis>, new()
{
    private readonly T _value = default!;

    internal ValueType()
    {
    }

    protected T Value
    {
        get => _value;
        private init
        {
            T fixedValue = FixValue(value);
            Validate(fixedValue);
            _value = fixedValue;
        }
    }

    /// <summary>
    ///     creates an instance of this valueType.
    ///     might be wrapped by derived class with operator to improve usability.
    ///     throws a <see cref="NullValueException{TValueType}" /> if the parameter is null
    ///     the derived classes implement GetNullable, which does not throw the exception but instead returns null
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="NullValueException{TValueType}"></exception>
    /// thrown in validate
    /// <returns></returns>
    public static TThis Get(T value)
    {
        return new() { Value = value };
    }

    /// <summary>
    ///     called before Validate.
    ///     used to adjust the given value (i.e. remove leading spaces etc)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected T FixValue(T value)
    {
        return value;
    }

    /// <summary>
    ///     called after fixValue
    ///     use this to validate your value
    ///     throws an exception if the value is invalid
    /// </summary>
    /// <param name="value"></param>
    protected virtual void Validate(T value)
    {
    }
}
