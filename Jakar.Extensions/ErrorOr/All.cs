namespace Jakar.Extensions;


public readonly record struct All;



public readonly struct All<T>( T Value )
{
    public static implicit operator All<T>( T value ) => new(value);
}
