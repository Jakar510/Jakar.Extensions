namespace Jakar.Extensions;


public partial class WebRequester
{
    [DefaultValue( nameof(Default) )]
    public readonly struct RetryPolicy( TimeSpan delay, TimeSpan scale, ushort maxRetires )
    {
        public static readonly TimeSpan    Time         = TimeSpan.FromSeconds( 2 );
        public static readonly RetryPolicy Default      = new(Time, Time, 3);
        public static readonly RetryPolicy None         = new(TimeSpan.Zero, TimeSpan.Zero, 0);
        public static readonly RetryPolicy Single       = new(TimeSpan.Zero, TimeSpan.Zero, 1);
        public readonly        bool        AllowRetries = maxRetires > 0;
        public readonly        ushort      MaxRetires   = maxRetires;
        public readonly        TimeSpan    Delay        = delay;
        public readonly        TimeSpan    Scale        = scale;


        public static RetryPolicy Create( ushort   maxRetries )                               => Create( Time,  maxRetries );
        public static RetryPolicy Create( TimeSpan delay, ushort   maxRetries )               => Create( delay, delay, maxRetries );
        public static RetryPolicy Create( TimeSpan delay, TimeSpan scale, ushort maxRetries ) => new(delay, scale, maxRetries);


        public Task IncrementAndWait( ref ushort count, CancellationToken token )
        {
            if ( !AllowRetries || count > MaxRetires ) { return Task.CompletedTask; }

            count++;
            TimeSpan scale = Scale * count;
            TimeSpan time  = Delay + scale;
            return Task.Delay( time, token );
        }
    }
}
