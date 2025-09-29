namespace Jakar.Extensions;


public readonly record struct Some;



public readonly record struct Some<TValue>( TValue Value )
{
    public static implicit operator Some<TValue>( TValue value ) => new(value);
}
