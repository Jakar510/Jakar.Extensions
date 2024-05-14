namespace Jakar.Extensions;


public readonly record struct Some;



public readonly record struct Some<T>( T Value )
{
    public static implicit operator Some<T>( T value ) => new(value);
}
