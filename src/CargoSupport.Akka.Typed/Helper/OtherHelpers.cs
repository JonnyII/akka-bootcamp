namespace CargoSupport.Akka.Typed.Helper;

public static class OtherHelpers
{
    public static uint GetInheritanceDepth(this Type type)
    {
        uint i = 0;
        for (var currentType = type; currentType?.BaseType is not null; i++)
        {
            if (currentType.BaseType is null)
                return i;
            currentType = type?.BaseType;
        }

        return i;
    }
}