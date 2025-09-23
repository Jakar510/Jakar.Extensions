// Jakar.Extensions :: Jakar.Extensions
// 08/23/2023  8:40 AM

namespace Jakar.Extensions;


public static partial class Tasks
{
    [NotSerializable]
    public readonly record struct Callers( OneOf<IEnumerable<Func<CancellationToken, Task>>, IEnumerable<Func<CancellationToken, ValueTask>>> Func, CancellationToken Token )
    {
        public async Task Execute() => await Task.WhenAll( Functions() );
        private IEnumerable<Task> Functions()
        {
            if ( Func.IsT0 )
            {
                foreach ( Func<CancellationToken, Task> func in Func.AsT0 ) { yield return func( Token ); }
            }
            else
            {
                foreach ( Func<CancellationToken, ValueTask> func in Func.AsT1 ) { yield return new Caller( func, Token ).Execute(); }
            }
        }
    }


    
    [NotSerializable]
    public readonly record struct Callers<TResult>( OneOf<IEnumerable<Func<CancellationToken, Task<TResult>>>, IEnumerable<Func<CancellationToken, ValueTask<TResult>>>> Func, CancellationToken Token )
    {
        public async Task<IEnumerable<TResult>> Execute() => await Task.WhenAll( Functions() );
        private IEnumerable<Task<TResult>> Functions()
        {
            if ( Func.IsT0 )
            {
                foreach ( Func<CancellationToken, Task<TResult>> func in Func.AsT0 ) { yield return func( Token ); }
            }
            else
            {
                foreach ( Func<CancellationToken, ValueTask<TResult>> func in Func.AsT1 ) { yield return new Caller<TResult>( func, Token ).Execute(); }
            }
        }
    }


    
    [NotSerializable]
    public readonly record struct Callers<T1, TResult>( OneOf<IEnumerable<Func<T1, CancellationToken, Task<TResult>>>, IEnumerable<Func<T1, CancellationToken, ValueTask<TResult>>>> Func, T1 Arg1, CancellationToken Token )
    {
        public async Task<IEnumerable<TResult>> Execute() => await Task.WhenAll( Functions() );
        private IEnumerable<Task<TResult>> Functions()
        {
            if ( Func.IsT0 )
            {
                foreach ( Func<T1, CancellationToken, Task<TResult>> func in Func.AsT0 ) { yield return func( Arg1, Token ); }
            }
            else
            {
                foreach ( Func<T1, CancellationToken, ValueTask<TResult>> func in Func.AsT1 ) { yield return new Caller<T1, TResult>( func, Arg1, Token ).Execute(); }
            }
        }
    }


    
    [NotSerializable]
    public readonly record struct Callers<T1, T2, TResult>( OneOf<IEnumerable<Func<T1, T2, CancellationToken, Task<TResult>>>, IEnumerable<Func<T1, T2, CancellationToken, ValueTask<TResult>>>> Func, T1 Arg1, T2 Arg2, CancellationToken Token )
    {
        public async Task<IEnumerable<TResult>> Execute() => await Task.WhenAll( Functions() );
        private IEnumerable<Task<TResult>> Functions()
        {
            if ( Func.IsT0 )
            {
                foreach ( Func<T1, T2, CancellationToken, Task<TResult>> func in Func.AsT0 ) { yield return func( Arg1, Arg2, Token ); }
            }
            else
            {
                foreach ( Func<T1, T2, CancellationToken, ValueTask<TResult>> func in Func.AsT1 ) { yield return new Caller<T1, T2, TResult>( func, Arg1, Arg2, Token ).Execute(); }
            }
        }
    }
}
