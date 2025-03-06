Console.WriteLine( DateTimeOffset.UtcNow.ToString() );
CancellationTokenSource source = new(TimeSpan.FromSeconds( 5 ));

try
{
    Synchronized<long> sync  = new(0);
    List<Task>         tasks = [Run( source.Token ), Run( source.Token ), Run( source.Token )];
    await Task.WhenAll( tasks );

    async Task Run( CancellationToken token )
    {
        // ReSharper disable once AccessToDisposedClosure
        while ( token.ShouldContinue() )
        {
            (++sync.Value).WriteToConsole();
            int delay = Random.Shared.Next( 100 );
            delay.WriteToDebug();
            await Task.Delay( delay );
        }
    }
}
finally { source.Dispose(); }
