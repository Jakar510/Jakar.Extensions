namespace Jakar.Extensions;


public readonly record struct Success;



public readonly record struct Success<T>( T Value )
{
    public static implicit operator Success<T>( T value ) => new(value);
}
