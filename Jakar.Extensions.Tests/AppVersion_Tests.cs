using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Jakar.Extensions.General;
using Jakar.Extensions.Models;
using NUnit.Framework;


namespace Jakar.Extensions.Tests;


[TestFixture]

// ReSharper disable once InconsistentNaming
public class AppVersion_Tests : Assert
{
    /// <summary>
    /// 21.10.3
    /// </summary>
    private static readonly AppVersion _version = new(21, 10, 3);


    [Test]
    [TestCase("1")]
    [TestCase("1.0")]
    [TestCase("1.2.3")]
    [TestCase("1.2.3.4")]
    [TestCase("1.2.3.4.5")]
    [TestCase("1.2.3.4.5.6")]
    [TestCase("0.7.0.25")]
    public void Parse_Tests( string s ) => AppVersion.Parse(s);

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
    public void TryParse_Tests( string s, bool shouldWork )
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
    [TestCase("1.0.0.0.0.0.0", false)]
    [TestCase("1.0.0...0.0.0", false)]
    public void ToString_Tests( string s, bool shouldWork )
    {
        if ( AppVersion.TryParse(s, out AppVersion? version) )
        {
            AreEqual(s, version.ToString());
            return;
        }

        False(shouldWork);
    }

    [Test]
    [TestCase("", null)]
    [TestCase("1", AppVersion.Format.Singular)]
    [TestCase("1.2", AppVersion.Format.Minimal)]
    [TestCase("1.2.3", AppVersion.Format.Typical)]
    [TestCase("1.2.3.4", AppVersion.Format.Detailed)]
    [TestCase("1.2.3.4.5", AppVersion.Format.DetailedRevisions)]
    [TestCase("1.2.3.4.5.6", AppVersion.Format.Complete)]
    [TestCase("1.2.3.4.5.6.7", null)]
    [TestCase("1.2.3..4.5.6", null)]
    [TestCase("1.2.3..4.5.6.7", null)]
    public void Format_Tests( string s, AppVersion.Format? expectedFormat )
    {
        if ( AppVersion.TryParse(s, out AppVersion? version) )
        {
            NotNull(version);
            NotNull(expectedFormat);
            if ( expectedFormat is null ) { throw new NullReferenceException(s); }

            AppVersion.Format format = expectedFormat.Value;
            AreEqual(format, version.Value.GetFormat());

            return;
        }

        IsNull(expectedFormat);
    }


    [Test]
    [TestCase("", false)]
    [TestCase("21", false)]
    [TestCase("21.10", false)]
    [TestCase("20.12.1", false)]
    [TestCase("20.6.1", false)]
    [TestCase("21.10.2", false)]
    [TestCase("21.10.3", true)]
    [TestCase("21.10.4", true)]
    [TestCase("21.11.3", true)]
    [TestCase("21.12.1", true)]
    [TestCase("22.10.3", true)]
    [TestCase("22.12.1", true)]
    public void Compare_Test_GreaterEqual( string s, in bool expected )
    {
        if ( AppVersion.TryParse(s, out AppVersion? version) )
        {
            NotNull(version);

            AreEqual(expected, version >= _version);

            return;
        }

        False(expected);
    }


    [Test]
    [TestCase("", false)]
    [TestCase("21", false)]
    [TestCase("21.10", false)]
    [TestCase("20.6.1", false)]
    [TestCase("20.12.1", false)]
    [TestCase("21.10.2", false)]
    [TestCase("21.10.3", false)]
    [TestCase("21.10.4", true)]
    [TestCase("21.11.3", true)]
    [TestCase("21.12.1", true)]
    [TestCase("22.10.3", true)]
    [TestCase("22.12.1", true)]
    public void Compare_Test_Greater( string s, in bool expected )
    {
        if ( AppVersion.TryParse(s, out AppVersion? version) )
        {
            NotNull(version);

            AreEqual(expected, version > _version);

            return;
        }

        False(expected);
    }


    [Test]
    [TestCase("", false)]
    [TestCase("21", false)]
    [TestCase("21.10", false)]
    [TestCase("20.6.1", true)]
    [TestCase("20.12.1", true)]
    [TestCase("21.10.2", true)]
    [TestCase("21.10.3", false)]
    [TestCase("21.10.4", false)]
    [TestCase("21.11.3", false)]
    [TestCase("21.12.1", false)]
    [TestCase("22.10.3", false)]
    [TestCase("22.12.1", false)]
    public void Compare_Test_Less( string s, in bool expected )
    {
        if ( AppVersion.TryParse(s, out AppVersion? version) )
        {
            NotNull(version);

            AreEqual(expected, version < _version);

            return;
        }

        False(expected);
    }


    [Test]
    [TestCase("", false)]
    [TestCase("21", false)]
    [TestCase("21.10", false)]
    [TestCase("20.6.1", true)]
    [TestCase("20.12.1", true)]
    [TestCase("21.10.2", true)]
    [TestCase("21.10.3", true)]
    [TestCase("21.10.4", false)]
    [TestCase("21.11.3", false)]
    [TestCase("21.12.1", false)]
    [TestCase("22.10.3", false)]
    [TestCase("22.12.1", false)]
    public void Compare_Test_LessEqual( string s, in bool expected )
    {
        if ( AppVersion.TryParse(s, out AppVersion? version) )
        {
            NotNull(version);

            AreEqual(expected, version <= _version);

            return;
        }

        False(expected);
    }


    [Test]
    [TestCase("21.10.3", "", false)]
    [TestCase("21.10.3", default, false)]
    [TestCase("21.10.3", "21", false)]
    [TestCase("21.10.3", "21.10", false)]
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
    public void FuzzyEquals_Test( string source, string? s, in bool expected ) => FuzzyEquals_Test(AppVersion.Parse(source), s, expected);

    private static void FuzzyEquals_Test( AppVersion source, string? s, in bool expected )
    {
        if ( AppVersion.TryParse(s, out AppVersion? version) )
        {
            NotNull(version);

            AreEqual(expected, source.FuzzyEquals(version));

            return;
        }

        False(expected);
    }
}
