// Jakar.Extensions :: Jakar.Extensions
// 07/11/2025  15:58

namespace Jakar.Shapes;


public static class Shapes
{
    public static TValue[]? Create<TValue>( this TValue[]? self, RefSelect<TValue> func )
    {
        ReadOnlySpan<TValue> span = self;
        if ( span.IsEmpty ) { return null; }

        TValue[] buffer = GC.AllocateUninitializedArray<TValue>(span.Length);
        int      index  = 0;

        foreach ( ref readonly TValue value in span ) { buffer[index++] = func(in value); }

        return buffer;
    }
    public static TOutput[]? Create<TInput, TOutput>( this TInput[]? self, RefSelect<TInput, TOutput> func )
    {
        ReadOnlySpan<TInput> span = self;
        if ( span.IsEmpty ) { return null; }

        TOutput[] buffer = GC.AllocateUninitializedArray<TOutput>(span.Length);
        int       index  = 0;

        foreach ( ref readonly TInput value in span ) { buffer[index++] = func(in value); }

        return buffer;
    }
}
