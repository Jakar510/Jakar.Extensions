#nullable enable
namespace Jakar.Extensions;


public class ExpectedValueTypeException<TKey> : Exception // Jakar.Api.Exceptions.Networking.HeaderException
{
    public TKey?        Key      { get; private set; }
    public Type?        Actual   { get; private set; }
    public IList<Type>? Expected { get; private set; }


    public ExpectedValueTypeException( TKey key, object? value, params Type[] expected ) : this(key, value?.GetType(), expected) { }
    public ExpectedValueTypeException( TKey key, Type?   value, params Type[] expected ) : base(GetMessage(value, expected, key)) => Update(value, expected, key);

    public ExpectedValueTypeException( Exception inner, TKey key, object? value, params Type[] expected ) : base(GetMessage(value?.GetType(), expected, key), inner) =>
        Update(value?.GetType(), expected, key);

    public ExpectedValueTypeException( Exception inner, TKey key, Type? value, params Type[] expected ) : base(GetMessage(value, expected, key), inner) => Update(value, expected, key);


    public ExpectedValueTypeException( object?   value, params Type[] expected ) : this(value?.GetType(), expected) => Update(value?.GetType(),                             expected);
    public ExpectedValueTypeException( Type?     value, params Type[] expected ) : base(GetMessage(value, expected)) => Update(value,                                       expected);
    public ExpectedValueTypeException( Exception inner, object?       value, params Type[] expected ) : this(inner, value?.GetType(), expected) => Update(value?.GetType(), expected);
    public ExpectedValueTypeException( Exception inner, Type?         value, params Type[] expected ) : base(GetMessage(value, expected), inner) => Update(value,           expected);


    protected void Update( Type? value, Type[] expected, TKey? key = default )
    {
        Key      = key;
        Actual   = value;
        Expected = expected;

        Data[nameof(Key)]      = Key?.ToString();
        Data[nameof(Actual)]   = Actual?.FullName;
        Data[nameof(Expected)] = GetTypeNames(expected);
    }


    protected static IEnumerable<string?> GetTypeNames( params Type[] expected ) => expected.Select(item => item.FullName);
    protected static string GetTypes( params                   Type[] expected ) => GetTypeNames(expected).ToPrettyJson();


    protected static string GetMessage( Type? actual, Type[] expected, TKey? key = default )
    {
        var builder = new StringBuilder();

        builder.AppendLine(key is null
                               ? "The passed value was of an unexpected Type."
                               : @$"For the key ""{key}"", the passed value was of an unexpected Type.");

        builder.AppendLine(@$"Actual type: ""{actual?.FullName ?? "null"}"".");
        builder.AppendLine("It can be any of the following types: ");
        builder.AppendLine(GetTypes(expected));

        return builder.ToString().Replace("\r\n", "\n");
    }


    public static T Verify<T>( object? item, TKey key )
    {
        if ( item is T value ) { return value; }

        throw new ExpectedValueTypeException<TKey>(key, item?.GetType(), typeof(T));
    }

    public static T Verify<T>( object? item )
    {
        if ( item is T value ) { return value; }

        throw new ExpectedValueTypeException<TKey>(item?.GetType(), typeof(T));
    }
}



public class ExpectedValueTypeException : ExpectedValueTypeException<string> // Jakar.Api.Exceptions.Networking.HeaderException
{
    public ExpectedValueTypeException( string    key,   object? value, params Type[] expected ) : base(key, value?.GetType(), expected) { }
    public ExpectedValueTypeException( string    key,   Type?   value, params Type[] expected ) : base(key, value, expected) { }
    public ExpectedValueTypeException( Exception inner, string  key,   object?       value, params Type[] expected ) : base(inner, key, value, expected) { }
    public ExpectedValueTypeException( Exception inner, string  key,   Type?         value, params Type[] expected ) : base(inner, key, value, expected) { }


    public ExpectedValueTypeException( object?   value, params Type[] expected ) : base(value, expected) { }
    public ExpectedValueTypeException( Type?     value, params Type[] expected ) : base(value, expected) { }
    public ExpectedValueTypeException( Exception inner, object?       value, params Type[] expected ) : base(inner, value, expected) { }
    public ExpectedValueTypeException( Exception inner, Type?         value, params Type[] expected ) : base(inner, value, expected) { }


    public new static T Verify<T>( object? item, string key ) => ExpectedValueTypeException<string>.Verify<T>(item, key);
    public new static T Verify<T>( object? item ) => ExpectedValueTypeException<string>.Verify<T>(item);
}
