namespace Jakar.Extensions;


public partial class WebRequester
{
    public readonly record struct RetryPolicy( TimeSpan Delay, TimeSpan Scale, bool AllowRetries, uint MaxRetires )
    {
        private static readonly TimeSpan _time = TimeSpan.FromSeconds( 2 );

        public RetryPolicy() : this( default, default, false, default ) { }
        public RetryPolicy( bool     allowRetries ) : this( _time, _time, allowRetries, 3 ) { }
        public RetryPolicy( bool     allowRetries, uint     maxRetires ) : this( _time, _time, allowRetries, maxRetires ) { }
        public RetryPolicy( TimeSpan delay,        TimeSpan scale, uint maxRetires ) : this( delay, scale, true, maxRetires ) { }


        public Task Wait( ref uint count, CancellationToken token = default )
        {
            TimeSpan time = Delay + Scale * count++;
            return time.Delay( token );
        }
    }
}
