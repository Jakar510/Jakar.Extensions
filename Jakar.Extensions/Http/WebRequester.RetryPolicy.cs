namespace Jakar.Extensions;


public partial class WebRequester
{
    [DefaultValue( nameof(Default) )]
    public readonly record struct RetryPolicy( TimeSpan Delay, TimeSpan Scale, ushort MaxRetires = 3 )
    {
        public static readonly TimeSpan    Time = TimeSpan.FromSeconds( 2 );
        public static          RetryPolicy Default      => new(Time, Time);
        public static          RetryPolicy None         => new(TimeSpan.Zero, TimeSpan.Zero, 0);
        public static          RetryPolicy Single       => new(TimeSpan.Zero, TimeSpan.Zero, 1);
        public                 bool        AllowRetries => MaxRetires > 0;

        public static RetryPolicy Create( ushort maxRetries ) => new(Time, Time, maxRetries);


        public Task Wait( ref ushort count, CancellationToken token )
        {
            if ( ++count > MaxRetires ) { return Task.CompletedTask; }

            TimeSpan time = Delay + Scale * count;
            return Task.Delay( time, token );
        }
    }
}
