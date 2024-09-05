namespace Jakar.Extensions;


public partial class WebRequester
{
    [DefaultValue( nameof(Default) )]
    public readonly record struct RetryPolicy( TimeSpan Delay, TimeSpan Scale, bool AllowRetries = true, ushort MaxRetires = 3 )
    {
        private static readonly TimeSpan    _time = TimeSpan.FromSeconds( 2 );
        public static           RetryPolicy Default => new(_time, _time);


        public Task Wait( ref ushort count, CancellationToken token )
        {
            if ( ++count > MaxRetires ) { count = MaxRetires; }

            TimeSpan time = Delay + Scale * count;
            return Task.Delay( time, token );
        }
    }
}
