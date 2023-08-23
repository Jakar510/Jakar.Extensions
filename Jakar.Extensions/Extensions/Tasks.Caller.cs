// Jakar.Extensions :: Jakar.Extensions
// 08/23/2023  8:34 AM

using OneOf;



namespace Jakar.Extensions;


public static partial class Tasks
{
    public readonly record struct Caller( OneOf<Func<CancellationToken, Task>, Func<CancellationToken, ValueTask>> Func, CancellationToken Token )
    {
        public async Task Execute()
        {
            if ( Func.IsT0 ) { await Func.AsT0( Token ); }
            else { await Func.AsT1( Token ); }
        }
    }



    public readonly record struct Caller<TResult>( OneOf<Func<CancellationToken, Task<TResult>>, Func<CancellationToken, ValueTask<TResult>>> Func, CancellationToken Token )
    {
        public async Task<TResult> Execute()
        {
            return Func.IsT0
                       ? await Func.AsT0( Token )
                       : await Func.AsT1( Token );
        }
    }



    public readonly record struct Caller<T1, TResult>( OneOf<Func<T1, CancellationToken, Task<TResult>>, Func<T1, CancellationToken, ValueTask<TResult>>> Func, T1 Arg1, CancellationToken Token )
    {
        public async Task<TResult> Execute()
        {
            return Func.IsT0
                       ? await Func.AsT0( Arg1, Token )
                       : await Func.AsT1( Arg1, Token );
        }
    }



    public readonly record struct Caller<T1, T2, TResult>( OneOf<Func<T1, T2, CancellationToken, Task<TResult>>, Func<T1, T2, CancellationToken, ValueTask<TResult>>> Func, T1 Arg1, T2 Arg2, CancellationToken Token )
    {
        public async Task<TResult> Execute()
        {
            return Func.IsT0
                       ? await Func.AsT0( Arg1, Arg2, Token )
                       : await Func.AsT1( Arg1, Arg2, Token );
        }
    }
}
