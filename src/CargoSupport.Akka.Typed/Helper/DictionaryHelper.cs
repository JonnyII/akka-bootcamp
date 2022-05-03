namespace CargoSupport.Akka.Typed;

public static class DictionaryHelper
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
        Func<TValue> valueFactory)
    {
        if (dictionary.TryGetValue(key, out var value))
            return value;

        value = valueFactory();
        dictionary.Add(key, value);

        return value;
    }

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        where TValue : new()
        => dictionary.GetOrAdd(key, () => new());
}