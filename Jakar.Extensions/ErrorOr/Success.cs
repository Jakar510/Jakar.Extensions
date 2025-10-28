namespace Jakar.Extensions;


[NotSerializable] public readonly record struct Success;



[NotSerializable]
public readonly record struct Success<TValue>( TValue Value )
{
    public static implicit operator Success<TValue>( TValue value ) => new(value);
}
