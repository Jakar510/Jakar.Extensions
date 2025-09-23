// Jakar.Extensions :: Jakar.Extensions
// 08/23/2023  8:34 AM

namespace Jakar.Extensions;


public static partial class Tasks
{
    [NotSerializable]
    public readonly record struct Caller( OneOf<Func<Task>, Func<ValueTask>, Func<CancellationToken, Task>, Func<CancellationToken, ValueTask>> Func, CancellationToken Token )
    {
        public async Task Execute()
        {
            if ( Func.IsT0 ) { await Func.AsT0(); }
            else if ( Func.IsT1 ) { await Func.AsT1(); }
            else if ( Func.IsT2 ) { await Func.AsT2( Token ); }
            else { await Func.AsT3( Token ); }
        }
    }


    
    [NotSerializable]
    public readonly record struct Caller<TResult>( OneOf<Func<Task<TResult>>, Func<ValueTask<TResult>>, Func<CancellationToken, Task<TResult>>, Func<CancellationToken, ValueTask<TResult>>> Func, CancellationToken Token )
    {
        public async Task<TResult> Execute()
        {
            if ( Func.IsT0 ) { return await Func.AsT0(); }

            if ( Func.IsT1 ) { return await Func.AsT1(); }

            if ( Func.IsT2 ) { return await Func.AsT2( Token ); }

            return await Func.AsT3( Token );
        }
    }


    
    [NotSerializable]
    public readonly record struct Caller<T1, TResult>( OneOf<Func<T1, Task<TResult>>, Func<T1, ValueTask<TResult>>, Func<T1, CancellationToken, Task<TResult>>, Func<T1, CancellationToken, ValueTask<TResult>>> Func, T1 Arg1, CancellationToken Token )
    {
        public async Task<TResult> Execute()
        {
            if ( Func.IsT0 ) { return await Func.AsT0( Arg1 ); }

            if ( Func.IsT1 ) { return await Func.AsT1( Arg1 ); }

            if ( Func.IsT2 ) { return await Func.AsT2( Arg1, Token ); }

            return await Func.AsT3( Arg1, Token );
        }
    }


    
    [NotSerializable]
    public readonly record struct Caller<T1, T2, TResult>( OneOf<Func<T1, T2, Task<TResult>>, Func<T1, T2, ValueTask<TResult>>, Func<T1, T2, CancellationToken, Task<TResult>>, Func<T1, T2, CancellationToken, ValueTask<TResult>>> Func, T1 Arg1, T2 Arg2, CancellationToken Token )
    {
        public async Task<TResult> Execute()
        {
            if ( Func.IsT0 ) { return await Func.AsT0( Arg1, Arg2 ); }

            if ( Func.IsT1 ) { return await Func.AsT1( Arg1, Arg2 ); }

            if ( Func.IsT2 ) { return await Func.AsT2( Arg1, Arg2, Token ); }

            return await Func.AsT3( Arg1, Arg2, Token );
        }
    }
}
