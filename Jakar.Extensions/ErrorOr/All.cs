namespace Jakar.Extensions;


public readonly record struct All;



public readonly record struct All<TValue>( TValue Value )
{
    public static implicit operator All<TValue>( TValue value ) => new(value);
}
