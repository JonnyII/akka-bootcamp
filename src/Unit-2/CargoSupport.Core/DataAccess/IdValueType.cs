using CargoSupport.Core.Exceptions;

namespace CargoSupport.Core.DataAccess;

public interface IIdValueType : IEquatable<IIdValueType>
{

}
public abstract record IdValueType<TThis> : NonNullableValueType<int, TThis>, IIdValueType
    where TThis : IdValueType<TThis>, new()
{
    protected override void Validate(int value)
    {
        base.Validate(value);
        if (value < 1)
            throw new InvalidIdException<TThis>(value);
    }

    public virtual bool Equals(IIdValueType? other)
        => Equals(other as object);// just use the record default equality comparer
}
