namespace Jakar.Extensions;


public partial class WebRequester
{
    public readonly struct RetryPolicy
    {
        public TimeSpan Delay        { get; } = TimeSpan.FromSeconds( 2 );
        public TimeSpan Scale        { get; } = TimeSpan.FromSeconds( 2 );
        public bool     AllowRetries { get; } = false;
        public int      MaxRetires   { get; } = 3;


        public RetryPolicy() { }
        public RetryPolicy( bool allowRetries ) => AllowRetries = allowRetries;
        public RetryPolicy( bool allowRetries, int maxRetires ) : this( allowRetries ) => MaxRetires = Math.Max( 1, maxRetires );
        public RetryPolicy( TimeSpan delay, TimeSpan scale, int maxRetires ) : this( true, maxRetires )
        {
            Delay = delay;
            Scale = scale;
        }


        public Task Wait( ref int count, CancellationToken token = default )
        {
            TimeSpan time = Delay + Scale * count++;
            return time.Delay( token );
        }
    }
}
