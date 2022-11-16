namespace Jakar.Extensions;


public partial class WebRequester
{
    public readonly struct RetryPolicy
    {
        public TimeSpan Delay        { get; init; } = TimeSpan.FromSeconds( 2 );
        public TimeSpan Scale        { get; init; } = TimeSpan.FromSeconds( 2 );
        public bool     AllowRetries { get; init; } = false;
        public int      MaxRetires   { get; init; } = 3;


        public RetryPolicy() { }


        public Task Wait( ref int count, CancellationToken token = default) => (Delay + Scale * count++).Delay(token);
    }
}
