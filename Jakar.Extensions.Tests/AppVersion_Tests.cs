namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf( typeof(AppVersion) )]

// ReSharper disable once InconsistentNaming
public class AppVersion_Tests : Assert
{
    [Test]
    [TestCase( 1, 0, 0, 0, 0, 0, "1.0.0.0.0.0" )]
    public void Construct_Complete( int major, int? minor, int? maintenance, int? majorRevision, int? minorRevision, int? build, string expected )
    {
        var version = new AppVersion( major, minor, maintenance, majorRevision, minorRevision, build );
        this.AreEqual( expected, version.ToString() );
    }

    [Test]
    [TestCase( 1, 0, 0, 0, 0, "1.0.0.0.0" )]
    public void Construct_DetailedRevisions( int major, int minor, int maintenance, int majorRevision, int build, string expected )
    {
        var version = new AppVersion( major, minor, maintenance, majorRevision, build );
        this.AreEqual( expected, version.ToString() );
    }

    [Test]
    [TestCase( 1, 0, 0, 0, "1.0.0.0" )]
    public void Construct_Detailed( int major, int minor, int maintenance, int build, string expected )
    {
        var version = new AppVersion( major, minor, maintenance, build );
        this.AreEqual( expected, version.ToString() );
    }

    [Test]
    [TestCase( 1, 0, 0, "1.0.0" )]
    public void Construct_Typical( int major, int minor, int build, string expected )
    {
        var version = new AppVersion( major, minor, build );
        this.AreEqual( expected, version.ToString() );
    }

    [Test]
    [TestCase( 1, 0, "1.0" )]
    public void Construct_Minimal( int major, int minor, string expected )
    {
        var version = new AppVersion( major, minor );
        this.AreEqual( expected, version.ToString() );
    }

    [Test]
    [TestCase( 1, "1" )]
    public void Construct_Singular( int major, string expected )
    {
        var version = new AppVersion( major );
        this.AreEqual( expected, version.ToString() );
    }


    [Test]
    [TestCase( "1" )]
    [TestCase( "1.0" )]
    [TestCase( "1.2.3" )]
    [TestCase( "1.2.3.4" )]
    [TestCase( "1.2.3.4.5" )]
    [TestCase( "1.2.3.4.5.6" )]
    [TestCase( "0.7.0.25" )]
    public void Parse( string s ) => DoesNotThrow( () => AppVersion.Parse( s ) );


    [Test]
    [TestCase( "1" )]
    [TestCase( "1.0" )]
    [TestCase( "1.2.3" )]
    [TestCase( "1.2.3.4" )]
    [TestCase( "1.2.3.4.5" )]
    [TestCase( "1.2.3.4.5.6" )]
    [TestCase( "0.7.0.25" )]
    [TestCase( "2147483647.2147483647.2147483647.2147483647.2147483647.2147483647" )]
    public void AsSpans( string s )
    {
        AppVersion version = AppVersion.Parse( s );

        string result = version.ToString();
        result.WriteToConsole();
        this.AreEqual( s, result );
    }


    [Test]
    [TestCase( "1" )]
    [TestCase( "1.0" )]
    [TestCase( "1.2.3" )]
    [TestCase( "1.2.3.4" )]
    [TestCase( "1.2.3.4.5" )]
    [TestCase( "1.2.3.4.5.6" )]
    [TestCase( "0.7.0.25" )]
    [TestCase( "2147483647.2147483647.2147483647.2147483647.2147483647.2147483647" )]
    public void Clone( string s )
    {
        AppVersion version = AppVersion.Parse( s );
        AppVersion result  = version.Clone();
        result.WriteToConsole();
        this.AreEqual( version, result );
    }

    [Test]
    [TestCase( "1.0" )]
    [TestCase( "1.2.3" )]
    [TestCase( "1.2.3.4" )]
    [TestCase( "0.7.0.25" )]
    [TestCase( "2147483647.2147483647.2147483647.2147483647" )]
    public void ToVersion( string s )
    {
        var value = AppVersion.Parse( s ).ToVersion();

        this.AreEqual( Version.Parse( s ), value );
    }


    [Test]
    [TestCase( "", false )]
    [TestCase( "1" )]
    [TestCase( "1.0" )]
    [TestCase( "1.2.3" )]
    [TestCase( "1.2.3.4" )]
    [TestCase( "1.2.3.4.5" )]
    [TestCase( "1.2.3.4.5.6" )]
    [TestCase( "1.2.3.4.5.6.7", false )]
    [TestCase( "1.0.0.0.0.0.0", false )]
    [TestCase( "1.0.0...0.0.0", false )]
    [TestCase( "1.a1",          false )]
    [TestCase( "0.7.0.25" )]
    public void TryParse( string s, bool shouldWork = true )
    {
        bool result = AppVersion.TryParse( s, out AppVersion? version );
        this.AreEqual( shouldWork, result );

        if ( result ) { this.NotNull( version ); }
    }


    [Test]
    [TestCase( "", false )]
    [TestCase( "1" )]
    [TestCase( "1.0" )]
    [TestCase( "1.2.3" )]
    [TestCase( "1.2.3.4" )]
    [TestCase( "1.2.3.4.5" )]
    [TestCase( "1.2.3.4.5.6" )]
    [TestCase( "1.2.3.4.5.6.7", false )]
    [TestCase( "1.0.0.0.0.0.0", false )]
    [TestCase( "1.0.0...0.0.0", false )]
    [TestCase( "0.7.0.25" )]
    public void TryParse_Span( string s, bool shouldWork = true ) => TryParse_Span( s.AsSpan(), shouldWork );

    private void TryParse_Span( ReadOnlySpan<char> s, bool shouldWork )
    {
        if ( AppVersion.TryParse( s, out AppVersion? version ) )
        {
            this.NotNull( version );
            return;
        }

        this.False( shouldWork );
    }


    // [Test]
    // [TestCase("1.1rc", AppVersion.Option.RC)]
    // [TestCase("1.1-rc", AppVersion.Option.RC)]
    // [TestCase("1.1alpha", AppVersion.Option.Alpha)]
    // [TestCase("1.1-alpha", AppVersion.Option.Alpha)]
    // [TestCase("1.1beta", AppVersion.Option.Beta)]
    // [TestCase("1.1-beta", AppVersion.Option.Beta)]
    // [TestCase("0.7", AppVersion.Option.Stable)]
    // [TestCase("0.7.0", AppVersion.Option.Stable)]
    // [TestCase("0.7.0.25", AppVersion.Option.Stable)]
    // public void Check_Options( string s, AppVersion.Option options ) { Check_Options(s.AsSpan(), options); }
    //
    // private  void Check_Options(  ReadOnlySpan<char> s, AppVersion.Option options )
    // {
    //     AppVersion version = AppVersion.Parse(s);
    //     this.AreEqual(options, version.Type);
    // }


    [Test]
    [TestCase( "1" )]
    [TestCase( "1.0" )]
    [TestCase( "1.2.3" )]
    [TestCase( "1.2.3.4" )]
    [TestCase( "1.2.3.4.5" )]
    [TestCase( "1.2.3.4.5.6" )]
    [TestCase( "1.0.0.0.0.0.0", false )]
    [TestCase( "1.0.0...0.0.0", false )]
    public void ToString( string s, bool shouldWork = true )
    {
        if ( AppVersion.TryParse( s, out AppVersion? version ) )
        {
            this.AreEqual( s, version.ToString() );
            return;
        }

        this.False( shouldWork );
    }


    [Test]
    [TestCase( "1",              AppVersion.Format.Singular )]
    [TestCase( "1.2",            AppVersion.Format.Minimal )]
    [TestCase( "1.2.3",          AppVersion.Format.Typical )]
    [TestCase( "1.2.3.4",        AppVersion.Format.Detailed )]
    [TestCase( "1.2.3.4.5",      AppVersion.Format.DetailedRevisions )]
    [TestCase( "1.2.3.4.5.6",    AppVersion.Format.Complete )]
    [TestCase( "1.2.3.4.5.6.7",  null )]
    [TestCase( "1.2.3..4.5.6",   null )]
    [TestCase( "1.2.3..4.5.6.7", null )]
    public void Format( string s, AppVersion.Format? expectedFormat )
    {
        if ( AppVersion.TryParse( s, out AppVersion? version ) )
        {
            this.NotNull( version );
            this.NotNull( expectedFormat );
            this.AreEqual( expectedFormat!.Value, version.Scheme );

            return;
        }

        this.IsNull( expectedFormat );
    }


    [Test]
    [TestCase( "20.12.1", "21.10.3", false )]
    [TestCase( "20.6.1",  "21.10.3", false )]
    [TestCase( "21.10.2", "21.10.3", false )]
    [TestCase( "21.10.3", "21.10.3" )]
    [TestCase( "21.10.4", "21.10.3" )]
    [TestCase( "21.11.3", "21.10.3" )]
    [TestCase( "21.12.1", "21.10.3" )]
    [TestCase( "22.10.3", "21.10.3" )]
    [TestCase( "22.12.1", "21.10.3" )]
    public void Compare_Test_GreaterEqual( string left, string right, bool expected = true ) => this.AreEqual( expected, AppVersion.Parse( left ) >= AppVersion.Parse( right ) );


    [Test]
    [TestCase( "20.6.1",  "21.10.3", false )]
    [TestCase( "20.12.1", "21.10.3", false )]
    [TestCase( "21.10.2", "21.10.3", false )]
    [TestCase( "21.10.3", "21.10.3", false )]
    [TestCase( "21.10.4", "21.10.3" )]
    [TestCase( "21.11.3", "21.10.3" )]
    [TestCase( "21.12.1", "21.10.3" )]
    [TestCase( "22.10.3", "21.10.3" )]
    [TestCase( "22.12.1", "21.10.3" )]
    public void Compare_Test_Greater( string left, string right, bool expected = true ) => this.AreEqual( expected, AppVersion.Parse( left ) > AppVersion.Parse( right ) );


    [Test]
    [TestCase( "20.6.1",  "21.10.3" )]
    [TestCase( "20.12.1", "21.10.3" )]
    [TestCase( "21.10.2", "21.10.3" )]
    [TestCase( "21.10.3", "21.10.3", false )]
    [TestCase( "21.10.4", "21.10.3", false )]
    [TestCase( "21.11.3", "21.10.3", false )]
    [TestCase( "21.12.1", "21.10.3", false )]
    [TestCase( "22.10.3", "21.10.3", false )]
    [TestCase( "22.12.1", "21.10.3", false )]
    public void Compare_Test_Less( string left, string right, bool expected = true ) => this.AreEqual( expected, AppVersion.Parse( left ) < AppVersion.Parse( right ) );


    [Test]
    [TestCase( "20.6.1",  "21.10.3" )]
    [TestCase( "20.12.1", "21.10.3" )]
    [TestCase( "21.10.2", "21.10.3" )]
    [TestCase( "21.10.3", "21.10.3" )]
    [TestCase( "21.10.4", "21.10.3", false )]
    [TestCase( "21.11.3", "21.10.3", false )]
    [TestCase( "21.12.1", "21.10.3", false )]
    [TestCase( "22.10.3", "21.10.3", false )]
    [TestCase( "22.12.1", "21.10.3", false )]
    public void Compare_Test_LessEqual( string left, string right, bool expected = true ) => this.AreEqual( expected, AppVersion.Parse( left ) <= AppVersion.Parse( right ) );


    [Test]
    [TestCase( "21",        "21.10.3" )]
    [TestCase( "21.10",     "21.10.3" )]
    [TestCase( "21.10.0.0", "21.10.3" )]
    [TestCase( "21.10.0.0", "21.10.3.0.0.0" )]
    public void Compare_Test_FormatErrors( string left, string right ) => Throws<FormatException>( () => _ = AppVersion.Parse( left ) <= AppVersion.Parse( right ) );


    [Test]
    [TestCase( "21.10.3",     "20.6.1",   false )]
    [TestCase( "21.10.3",     "20.12.1",  false )]
    [TestCase( "21.10.3",     "21.10.02", false )]
    [TestCase( "21.10.3",     "21.10.2",  false )]
    [TestCase( "21.10.3",     "21.10.3" )]
    [TestCase( "21.10.3",     "21.10.4" )]
    [TestCase( "21.10.3",     "21.10.5" )]
    [TestCase( "21.10.3",     "21.10.10" )]
    [TestCase( "21.10.3",     "21.10.20" )]
    [TestCase( "21.10.3",     "21.11.3",   false )]
    [TestCase( "21.10.3",     "21.12.1",   false )]
    [TestCase( "21.10.3",     "22.10.3",   false )]
    [TestCase( "21.10.3",     "22.12.1",   false )]
    [TestCase( "21.10.3.1",   "21.10.2.1", false )]
    [TestCase( "21.10.3.1",   "21.10.3.1" )]
    [TestCase( "21.10.3.1",   "21.10.3.2" )]
    [TestCase( "21.10.3.1",   "21.10.4.1" )]
    [TestCase( "21.10.3.1",   "21.10.5.1" )]
    [TestCase( "21.10.3.1",   "21.10.10.1" )]
    [TestCase( "21.10.3.1",   "21.10.20.1" )]
    [TestCase( "21.10.3.1",   "21.11.3.1",   false )]
    [TestCase( "21.10.3.1",   "21.12.1.1",   false )]
    [TestCase( "21.10.3.1",   "22.10.3.1",   false )]
    [TestCase( "21.10.3.1",   "22.12.1.1",   false )]
    [TestCase( "21.10.3.5.1", "21.10.2.5.1", false )]
    [TestCase( "21.10.3.5.1", "21.10.3.5.1" )]
    [TestCase( "21.10.3.5.1", "21.10.3.5.2" )]
    [TestCase( "21.10.3.5.1", "21.10.4.5.1" )]
    [TestCase( "21.10.3.5.1", "21.10.5.5.1" )]
    [TestCase( "21.10.3.5.1", "21.10.10.5.1" )]
    [TestCase( "21.10.3.5.1", "21.10.20.5.1" )]
    [TestCase( "21.10.3.5.1", "21.11.3.5.1", false )]
    [TestCase( "21.10.3.5.1", "21.12.1.5.1", false )]
    [TestCase( "21.10.3.5.1", "22.10.3.5.1", false )]
    [TestCase( "21.10.3.5.1", "22.12.1.5.1", false )]
    public void FuzzyEquals_Test( string left, string right, bool expected = true ) => this.AreEqual( expected, AppVersion.Parse( left ).FuzzyEquals( AppVersion.Parse( right ) ) );
}
