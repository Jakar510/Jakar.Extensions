using System;
using Jakar.Extensions.Models;
using NUnit.Framework;


namespace Jakar.Extensions.Tests;


[TestFixture]

// ReSharper disable once InconsistentNaming
public class AppVersion_Tests : Assert
{
    [Test]
    [TestCase(1, 0, 0, 0, 0, 0, "1.0.0.0.0.0")]
    public void Construct_Complete( int major, int? minor, int? maintenance, int? majorRevision, int? minorRevision, int? build, in string expected )
    {
        var version = new AppVersion(major, minor, maintenance, majorRevision, minorRevision, build);
        AreEqual(expected, version.ToString());
    }

    [Test]
    [TestCase(1, 0, 0, 0, 0, "1.0.0.0.0")]
    public void Construct_DetailedRevisions( int major, int minor, int maintenance, int majorRevision, int build, in string expected )
    {
        var version = new AppVersion(major, minor, maintenance, majorRevision, build);
        AreEqual(expected, version.ToString());
    }

    [Test]
    [TestCase(1, 0, 0, 0, "1.0.0.0")]
    public void Construct_Detailed( int major, int minor, int maintenance, int build, in string expected )
    {
        var version = new AppVersion(major, minor, maintenance, build);
        AreEqual(expected, version.ToString());
    }

    [Test]
    [TestCase(1, 0, 0, "1.0.0")]
    public void Construct_Typical( int major, int minor, int build, in string expected )
    {
        var version = new AppVersion(major, minor, build);
        AreEqual(expected, version.ToString());
    }

    [Test]
    [TestCase(1, 0, "1.0")]
    public void Construct_Minimal( int major, int minor, in string expected )
    {
        var version = new AppVersion(major, minor);
        AreEqual(expected, version.ToString());
    }

    [Test]
    [TestCase(1, "1")]
    public void Construct_Singular( int major, in string expected )
    {
        var version = new AppVersion(major);
        AreEqual(expected, version.ToString());
    }


    [Test]
    [TestCase("1")]
    [TestCase("1.0")]
    [TestCase("1.2.3")]
    [TestCase("1.2.3.4")]
    [TestCase("1.2.3.4.5")]
    [TestCase("1.2.3.4.5.6")]
    [TestCase("0.7.0.25")]
    [TestCase("0.7.0.25")]
    public void Parse( string s ) { AppVersion.Parse(s); }


    [Test]
    [TestCase("", false)]
    [TestCase("1", true)]
    [TestCase("1.0", true)]
    [TestCase("1.2.3", true)]
    [TestCase("1.2.3.4", true)]
    [TestCase("1.2.3.4.5", true)]
    [TestCase("1.2.3.4.5.6", true)]
    [TestCase("1.2.3.4.5.6.7", false)]
    [TestCase("1.0.0.0.0.0.0", false)]
    [TestCase("1.0.0...0.0.0", false)]
    [TestCase("1.a1", false)]
    [TestCase("0.7.0.25", true)]
    public void TryParse( string s, bool shouldWork )
    {
        if ( AppVersion.TryParse(s, out AppVersion? version) )
        {
            NotNull(version);
            return;
        }

        False(shouldWork);
    }


    [Test]
    [TestCase("", false)]
    [TestCase("1", true)]
    [TestCase("1.0", true)]
    [TestCase("1.2.3", true)]
    [TestCase("1.2.3.4", true)]
    [TestCase("1.2.3.4.5", true)]
    [TestCase("1.2.3.4.5.6", true)]
    [TestCase("1.2.3.4.5.6.7", false)]
    [TestCase("1.0.0.0.0.0.0", false)]
    [TestCase("1.0.0...0.0.0", false)]
    [TestCase("0.7.0.25", true)]
    public void TryParse_Span( string s, bool shouldWork ) { TryParse_Span(s.AsSpan(), shouldWork); }

    private static void TryParse_Span( in ReadOnlySpan<char> s, bool shouldWork )
    {
        if ( AppVersion.TryParse(s, out AppVersion? version) )
        {
            NotNull(version);
            return;
        }

        False(shouldWork);
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
    // private static void Check_Options( in ReadOnlySpan<char> s, AppVersion.Option options )
    // {
    //     AppVersion version = AppVersion.Parse(s);
    //     AreEqual(options, version.Type);
    // }


    [Test]
    [TestCase("1", true)]
    [TestCase("1.0", true)]
    [TestCase("1.2.3", true)]
    [TestCase("1.2.3.4", true)]
    [TestCase("1.2.3.4.5", true)]
    [TestCase("1.2.3.4.5.6", true)]
    [TestCase("1.0.0.0.0.0.0", false)]
    [TestCase("1.0.0...0.0.0", false)]
    public void ToString( string s, bool shouldWork )
    {
        if ( AppVersion.TryParse(s, out AppVersion? version) )
        {
            AreEqual(s, version.ToString());
            return;
        }

        False(shouldWork);
    }


    [Test]
    [TestCase("1", AppVersion.Format.Singular)]
    [TestCase("1.2", AppVersion.Format.Minimal)]
    [TestCase("1.2.3", AppVersion.Format.Typical)]
    [TestCase("1.2.3.4", AppVersion.Format.Detailed)]
    [TestCase("1.2.3.4.5", AppVersion.Format.DetailedRevisions)]
    [TestCase("1.2.3.4.5.6", AppVersion.Format.Complete)]
    [TestCase("1.2.3.4.5.6.7", null)]
    [TestCase("1.2.3..4.5.6", null)]
    [TestCase("1.2.3..4.5.6.7", null)]
    public void Format( string s, AppVersion.Format? expectedFormat )
    {
        if ( AppVersion.TryParse(s, out AppVersion? version) )
        {
            NotNull(version);
            NotNull(expectedFormat);
            if ( expectedFormat is null ) { throw new NullReferenceException(s); }

            AppVersion.Format format = expectedFormat.Value;
            AreEqual(format, version.Value.Scheme);

            return;
        }

        IsNull(expectedFormat);
    }


    [Test]
    [TestCase("20.12.1", "21.10.3", false)]
    [TestCase("20.6.1", "21.10.3", false)]
    [TestCase("21.10.2", "21.10.3", false)]
    [TestCase("21.10.3", "21.10.3", true)]
    [TestCase("21.10.4", "21.10.3", true)]
    [TestCase("21.11.3", "21.10.3", true)]
    [TestCase("21.12.1", "21.10.3", true)]
    [TestCase("22.10.3", "21.10.3", true)]
    [TestCase("22.12.1", "21.10.3", true)]
    public void Compare_Test_GreaterEqual( string s, in string other, in bool expected ) { Compare_Test_GreaterEqual(s, AppVersion.Parse(other), expected); }

    private static void Compare_Test_GreaterEqual( string s, in AppVersion other, in bool expected )
    {
        if ( AppVersion.TryParse(s, out AppVersion? version) )
        {
            NotNull(version);

            AreEqual(expected, version >= other);

            return;
        }

        False(expected);
    }


    [Test]
    [TestCase("20.6.1", "21.10.3", false)]
    [TestCase("20.12.1", "21.10.3", false)]
    [TestCase("21.10.2", "21.10.3", false)]
    [TestCase("21.10.3", "21.10.3", false)]
    [TestCase("21.10.4", "21.10.3", true)]
    [TestCase("21.11.3", "21.10.3", true)]
    [TestCase("21.12.1", "21.10.3", true)]
    [TestCase("22.10.3", "21.10.3", true)]
    [TestCase("22.12.1", "21.10.3", true)]
    public void Compare_Test_Greater( string s, in string other, in bool expected ) { Compare_Test_Greater(s, AppVersion.Parse(other), expected); }

    private static void Compare_Test_Greater( string s, in AppVersion other, in bool expected )
    {
        if ( AppVersion.TryParse(s, out AppVersion? version) )
        {
            NotNull(version);

            AreEqual(expected, version > other);

            return;
        }

        False(expected);
    }


    [Test]
    [TestCase("20.6.1", "21.10.3", true)]
    [TestCase("20.12.1", "21.10.3", true)]
    [TestCase("21.10.2", "21.10.3", true)]
    [TestCase("21.10.3", "21.10.3", false)]
    [TestCase("21.10.4", "21.10.3", false)]
    [TestCase("21.11.3", "21.10.3", false)]
    [TestCase("21.12.1", "21.10.3", false)]
    [TestCase("22.10.3", "21.10.3", false)]
    [TestCase("22.12.1", "21.10.3", false)]
    public void Compare_Test_Less( string s, in string other, in bool expected ) { Compare_Test_Less(s, AppVersion.Parse(other), expected); }

    private static void Compare_Test_Less( string s, in AppVersion other, in bool expected )
    {
        if ( AppVersion.TryParse(s, out AppVersion? version) )
        {
            NotNull(version);

            AreEqual(expected, version < other);

            return;
        }

        False(expected);
    }


    [Test]
    [TestCase("20.6.1", "21.10.3", true)]
    [TestCase("20.12.1", "21.10.3", true)]
    [TestCase("21.10.2", "21.10.3", true)]
    [TestCase("21.10.3", "21.10.3", true)]
    [TestCase("21.10.4", "21.10.3", false)]
    [TestCase("21.11.3", "21.10.3", false)]
    [TestCase("21.12.1", "21.10.3", false)]
    [TestCase("22.10.3", "21.10.3", false)]
    [TestCase("22.12.1", "21.10.3", false)]
    public void Compare_Test_LessEqual( string s, in string other, in bool expected ) { Compare_Test_LessEqual(s, AppVersion.Parse(other), expected); }

    private static void Compare_Test_LessEqual( string s, AppVersion other, bool expected )
    {
        if ( AppVersion.TryParse(s, out AppVersion? version) )
        {
            NotNull(version);

            AreEqual(expected, version <= other);
            return;
        }

        False(expected);
    }


    [Test]
    [TestCase("21", "21.10.3")]
    [TestCase("21.10", "21.10.3")]
    [TestCase("21.10.0.0", "21.10.3")]
    [TestCase("21.10.0.0", "21.10.3.0.0.0")]
    public void Compare_Test_FormatErrors( in string left, in string right ) { Compare_Test_FormatErrors(AppVersion.Parse(left), AppVersion.Parse(right)); }

    private static void Compare_Test_FormatErrors( AppVersion left, AppVersion right ) { Throws<FormatException>(() => _ = left <= right); }


    [Test]
    [TestCase("21.10.3", "20.6.1", false)]
    [TestCase("21.10.3", "20.12.1", false)]
    [TestCase("21.10.3", "21.10.02", false)]
    [TestCase("21.10.3", "21.10.2", false)]
    [TestCase("21.10.3", "21.10.3", true)]
    [TestCase("21.10.3", "21.10.4", true)]
    [TestCase("21.10.3", "21.10.5", true)]
    [TestCase("21.10.3", "21.10.10", true)]
    [TestCase("21.10.3", "21.10.20", true)]
    [TestCase("21.10.3", "21.11.3", false)]
    [TestCase("21.10.3", "21.12.1", false)]
    [TestCase("21.10.3", "22.10.3", false)]
    [TestCase("21.10.3", "22.12.1", false)]
    [TestCase("21.10.3.1", "21.10.2.1", false)]
    [TestCase("21.10.3.1", "21.10.3.1", true)]
    [TestCase("21.10.3.1", "21.10.3.2", true)]
    [TestCase("21.10.3.1", "21.10.4.1", true)]
    [TestCase("21.10.3.1", "21.10.5.1", true)]
    [TestCase("21.10.3.1", "21.10.10.1", true)]
    [TestCase("21.10.3.1", "21.10.20.1", true)]
    [TestCase("21.10.3.1", "21.11.3.1", false)]
    [TestCase("21.10.3.1", "21.12.1.1", false)]
    [TestCase("21.10.3.1", "22.10.3.1", false)]
    [TestCase("21.10.3.1", "22.12.1.1", false)]
    [TestCase("21.10.3.5.1", "21.10.2.5.1", false)]
    [TestCase("21.10.3.5.1", "21.10.3.5.1", true)]
    [TestCase("21.10.3.5.1", "21.10.3.5.2", true)]
    [TestCase("21.10.3.5.1", "21.10.4.5.1", true)]
    [TestCase("21.10.3.5.1", "21.10.5.5.1", true)]
    [TestCase("21.10.3.5.1", "21.10.10.5.1", true)]
    [TestCase("21.10.3.5.1", "21.10.20.5.1", true)]
    [TestCase("21.10.3.5.1", "21.11.3.5.1", false)]
    [TestCase("21.10.3.5.1", "21.12.1.5.1", false)]
    [TestCase("21.10.3.5.1", "22.10.3.5.1", false)]
    [TestCase("21.10.3.5.1", "22.12.1.5.1", false)]
    public void FuzzyEquals_Test( in string left, in string right, in bool expected ) { FuzzyEquals_Test(AppVersion.Parse(left), AppVersion.Parse(right), expected); }

    private static void FuzzyEquals_Test( in AppVersion left, in AppVersion right, in bool expected ) { AreEqual(expected, left.FuzzyEquals(right)); }
}
