// Jakar.Extensions :: Experiments
// 11/18/2023  10:37 PM

using System.Collections.Frozen;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Configs;



namespace Experiments.Benchmarks;


[ Config( typeof(BenchmarkConfig) ), GroupBenchmarksBy( BenchmarkLogicalGroupRule.ByCategory ), SimpleJob( RuntimeMoniker.HostProcess ), MemoryDiagnoser, SuppressMessage( "ReSharper", "LoopCanBeConvertedToQuery" ) ]
public class DictionaryLookupBenchmarks
{
    private Dictionary<string, int>          _dictionary;
    private FrozenDictionary<string, int>    _frozenDictionary;
    private FrozenDictionary<string, int>    _frozenDictionaryReadOptimized;
    private ImmutableDictionary<string, int> _immutableDictionary;
    private KeyValuePair<string, int>[]      _items;
    private ReadOnlyDictionary<string, int>  _readOnlyDictionary;
    private string[]                         _keys;

    [ Params( 10, 100, 1000, 10_000, 100_000 ) ] public int Items { get; set; }


    [ GlobalSetup ]
    public void GlobalSetup()
    {
        _items = Enumerable.Range( 0, Items ).Select( static _ => new KeyValuePair<string, int>( Guid.NewGuid().ToString(), Random.Shared.Next() ) ).ToArray();
        _keys  = _items.Select( k => k.Key ).ToArray();

        _dictionary          = new Dictionary<string, int>( _items );
        _readOnlyDictionary  = new ReadOnlyDictionary<string, int>( _items.ToDictionary( i => i.Key, i => i.Value ) );
        _immutableDictionary = _items.ToImmutableDictionary();
        _frozenDictionary    = _items.ToFrozenDictionary();
    }


    [ BenchmarkCategory( "Construct" ), Benchmark( Baseline = true ) ] public Dictionary<string, int>          ConstructDictionary()          => new(_items);
    [ BenchmarkCategory( "Construct" ), Benchmark ]                    public ReadOnlyDictionary<string, int>  ConstructReadOnlyDictionary()  => new(_items.ToDictionary( i => i.Key, i => i.Value ));
    [ BenchmarkCategory( "Construct" ), Benchmark ]                    public ImmutableDictionary<string, int> ConstructImmutableDictionary() => _items.ToImmutableDictionary();
    [ BenchmarkCategory( "Construct" ), Benchmark ]                    public FrozenDictionary<string, int>    ConstructFrozenDictionary()    => _items.ToFrozenDictionary();


    [ BenchmarkCategory( "TryGetValue_Found" ), Benchmark( Baseline = true ) ]
    public bool Dictionary_TryGetValue_Found()
    {
        bool allFound = true;

        foreach ( string key in _keys ) { allFound &= _dictionary.TryGetValue( key, out int _ ); }

        return allFound;
    }

    [ BenchmarkCategory( "TryGetValue_Found" ), Benchmark ]
    public bool ReadOnlyDictionary_TryGetValue_Found()
    {
        bool allFound = true;

        foreach ( string key in _keys ) { allFound &= _readOnlyDictionary.TryGetValue( key, out int _ ); }

        return allFound;
    }

    [ BenchmarkCategory( "TryGetValue_Found" ), Benchmark ]
    public bool ImmutableDictionary_TryGetValue_Found()
    {
        bool allFound = true;

        foreach ( string key in _keys ) { allFound &= _immutableDictionary.TryGetValue( key, out int _ ); }

        return allFound;
    }

    [ BenchmarkCategory( "TryGetValue_Found" ), Benchmark ]
    public bool FrozenDictionary_TryGetValue_Found()
    {
        bool allFound = true;

        foreach ( string key in _keys ) { allFound &= _frozenDictionary.TryGetValue( key, out int _ ); }

        return allFound;
    }

    [ BenchmarkCategory( "TryGetValue_Found" ), Benchmark ]
    public bool FrozenDictionaryOptimized_TryGetValue_Found()
    {
        bool allFound = true;

        foreach ( string key in _keys ) { allFound &= _frozenDictionaryReadOptimized.TryGetValue( key, out int _ ); }

        return allFound;
    }
}
