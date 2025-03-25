namespace Jakar.Extensions;


public readonly record struct Success;



public readonly record struct Success<TValue>( TValue Value )
{
    public static implicit operator Success<TValue>( TValue value ) => new(value);
}
