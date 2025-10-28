namespace Jakar.Extensions;


[NotSerializable] public readonly record struct Some;



[NotSerializable]
public readonly record struct Some<TValue>( TValue Value )
{
    public static implicit operator Some<TValue>( TValue value ) => new(value);
}
