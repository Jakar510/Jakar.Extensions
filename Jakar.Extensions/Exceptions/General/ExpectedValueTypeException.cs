using ZLinq;



namespace Jakar.Extensions;


public class ExpectedValueTypeException<TKey> : Exception // Jakar.Api.Exceptions.Networking.HeaderException
{
    public Type?   Actual   { get; private set; }
    public Type[]? Expected { get; private set; }
    public TKey?   Key      { get; private set; }


     public ExpectedValueTypeException( TKey      key,   object?                   value, params ReadOnlySpan<Type> expected ) : this(key, value?.GetType(), expected) { }
     public ExpectedValueTypeException( TKey      key,   Type?                     value, params ReadOnlySpan<Type> expected ) : base(GetMessage(value,                                             in expected, key)) => Update(value,                   in expected, key);
     public ExpectedValueTypeException( Exception inner, TKey                      key,   object?                   value, params ReadOnlySpan<Type> expected ) : base(GetMessage(value?.GetType(), in expected, key), inner) => Update(value?.GetType(), in expected, key);
     public ExpectedValueTypeException( Exception inner, TKey                      key,   Type?                     value, params ReadOnlySpan<Type> expected ) : base(GetMessage(value,            in expected, key), inner) => Update(value,            in expected, key);
     public ExpectedValueTypeException( object?   value, params ReadOnlySpan<Type> expected ) : this(value?.GetType(), expected) => Update(value?.GetType(),                                         in expected);
     public ExpectedValueTypeException( Type?     value, params ReadOnlySpan<Type> expected ) : base(GetMessage(value, in expected)) => Update(value,                                                in expected);
     public ExpectedValueTypeException( Exception inner, object?                   value, params ReadOnlySpan<Type> expected ) : this(inner, value?.GetType(), expected) => Update(value?.GetType(), in expected);
     public ExpectedValueTypeException( Exception inner, Type?                     value, params ReadOnlySpan<Type> expected ) : base(GetMessage(value, in expected), inner) => Update(value,        in expected);


    
    protected static string GetMessage( Type? actual, scoped ref readonly ReadOnlySpan<Type> expected, TKey? key = default )
    {
        StringBuilder builder = new(1024);

        builder.AppendLine(key is null
                               ? "The passed value was of an unexpected Type."
                               : $"""For the key "{key}", the passed value was of an unexpected Type.""");

        builder.AppendLine($"""Actual type: "{actual?.FullName ?? "null"}".""");
        builder.AppendLine("It can be any of the following types: ");
        builder.AppendLine(GetTypes(in expected));

        return builder.ToString().Replace("\r\n", "\n");
    }
     protected static string    GetTypes( scoped ref readonly     ReadOnlySpan<Type> expected ) => GetTypeNames(in expected).ToJson(JakarExtensionsContext.Default.StringArray);
    protected static                                                                                                  string?[] GetTypeNames( scoped ref readonly ReadOnlySpan<Type> expected ) => expected.AsValueEnumerable().Select(static item => item.FullName).ToArray();


    
    public static TValue Verify<TValue>( object? item, TKey key )
    {
        if ( item is TValue value ) { return value; }

        throw new ExpectedValueTypeException<TKey>(key, item?.GetType(), typeof(TValue));
    }

    
    public static TValue Verify<TValue>( object? item )
    {
        if ( item is TValue value ) { return value; }

        throw new ExpectedValueTypeException<TKey>(item?.GetType(), typeof(TValue));
    }

    
    protected void Update( Type? value, scoped ref readonly ReadOnlySpan<Type> expected, TKey? key = default )
    {
        Key      = key;
        Actual   = value;
        Expected = expected.ToArray();

        Data[nameof(Key)]      = Key?.ToString();
        Data[nameof(Actual)]   = Actual?.FullName;
        Data[nameof(Expected)] = GetTypeNames(in expected).ToJson(JakarExtensionsContext.Default.StringArray);
    }
}



public class ExpectedValueTypeException : ExpectedValueTypeException<string> // Jakar.Api.Exceptions.Networking.HeaderException
{
    public ExpectedValueTypeException( string    key,   object? value, params ReadOnlySpan<Type> expected ) : base(key, value?.GetType(), expected) { }
    public ExpectedValueTypeException( string    key,   Type?   value, params ReadOnlySpan<Type> expected ) : base(key, value, expected) { }
    public ExpectedValueTypeException( Exception inner, string  key,   object?                   value, params ReadOnlySpan<Type> expected ) : base(inner, key, value, expected) { }
    public ExpectedValueTypeException( Exception inner, string  key,   Type?                     value, params ReadOnlySpan<Type> expected ) : base(inner, key, value, expected) { }


    public ExpectedValueTypeException( object?   value, params ReadOnlySpan<Type> expected ) : base(value, expected) { }
    public ExpectedValueTypeException( Type?     value, params ReadOnlySpan<Type> expected ) : base(value, expected) { }
    public ExpectedValueTypeException( Exception inner, object?                   value, params ReadOnlySpan<Type> expected ) : base(inner, value, expected) { }
    public ExpectedValueTypeException( Exception inner, Type?                     value, params ReadOnlySpan<Type> expected ) : base(inner, value, expected) { }


    public new static TValue Verify<TValue>( object? item, string key ) => ExpectedValueTypeException<string>.Verify<TValue>(item, key);
    public new static TValue Verify<TValue>( object? item ) => ExpectedValueTypeException<string>.Verify<TValue>(item);
}
